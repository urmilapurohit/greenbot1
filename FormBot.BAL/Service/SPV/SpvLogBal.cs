using FormBot.DAL;
using FormBot.Entity;
using FormBot.Entity.SPV;
using FormBot.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace FormBot.BAL.Service.SPV
{
    public class SpvLogBAL : ISpvLogBAL
    {
        Logger _log = new Logger();
        /// <summary>
        /// Insert or update spv log details.
        /// </summary>
        /// <param name="objSpvLog"></param>
        /// <returns></returns>
        public int InsertOrUpdate(SpvLog objSpvLog)
        {
            int InsertedId = 0;
            try
            {
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(DBClient.AddParameters("Id", SqlDbType.Int, objSpvLog.SPVLogId));
                sqlParameters.Add(DBClient.AddParameters("SerialNumber", SqlDbType.NVarChar, objSpvLog.SerialNumber));
                sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, objSpvLog.JobId));
                sqlParameters.Add(DBClient.AddParameters("SPVMethod", SqlDbType.Int, objSpvLog.SPVMethod));
                sqlParameters.Add(DBClient.AddParameters("ServiceAdministrator", SqlDbType.NVarChar, objSpvLog.ServiceAdministrator));
                sqlParameters.Add(DBClient.AddParameters("RequestTime", SqlDbType.DateTime, objSpvLog.RequestTime));
                sqlParameters.Add(DBClient.AddParameters("Manufacturer", SqlDbType.NVarChar, objSpvLog.Manufacturer));
                sqlParameters.Add(DBClient.AddParameters("ModelNumber", SqlDbType.NVarChar, objSpvLog.ModelNumber));
                sqlParameters.Add(DBClient.AddParameters("Supplier", SqlDbType.NVarChar, objSpvLog.Supplier));
                if(objSpvLog.SPVLogId > 0)
                {
                    sqlParameters.Add(DBClient.AddParameters("VerificationStatus", SqlDbType.Int, objSpvLog.VerificationStatus));
                    sqlParameters.Add(DBClient.AddParameters("ResponseCode", SqlDbType.NVarChar, objSpvLog.ResponseCode));
                    sqlParameters.Add(DBClient.AddParameters("ResponseTime", SqlDbType.DateTime, objSpvLog.ResponseTime));
                    sqlParameters.Add(DBClient.AddParameters("ResponseMessage", SqlDbType.NVarChar, objSpvLog.ResponseMessage));
                }
                InsertedId = Convert.ToInt32(CommonDAL.ExecuteScalar("InsertOrUpdateSpvLog", sqlParameters.ToArray()));

                ////Cache clear
                //CacheConfiguration.RemoveByPattern(CacheConfiguration.SpvLogList);
                //CacheConfiguration.Remove(string.Format(CacheConfiguration.SpvLogList, InsertedId));
            }
            catch (Exception ex)
            {
                _log.LogException(SystemEnums.Severity.Error, $"SpvLogBal/Insert objSpvLog = {JsonConvert.SerializeObject(objSpvLog)}", ex);
            }
            return InsertedId;
        }
        /// <summary>
        /// Get all end point name from spv manufacturer list.
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllEndPointsNameFromSpvManufacturer()
        {
            List<string> lstEndPoints = new List<string>();
            try
            {
                string spName = "[GetAllEndPointsNameFromSpvManufacturer]";
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                lstEndPoints = CommonDAL.ExecuteProcedure<Spvmanufacturer>(spName, sqlParameters.ToArray()).ToList().Select(x=>x.ServiceAdministrator).ToList();
            }
            catch (Exception ex)
            {
                _log.LogException(SystemEnums.Severity.Error, $"SpvLogBal/GetAllEndPointsNameFromSpvManufacturer", ex);
            }
            return lstEndPoints.ToList();
        }
        /// <summary>
        /// Get all spv log data with passing filters.
        /// </summary>
        /// <param name="PageSize"></param>
        /// <param name="PageNumber"></param>
        /// <param name="SortCol"></param>
        /// <param name="SortDir"></param>
        /// <param name="ServiceAdministrator"></param>
        /// <param name="ResellerId"></param>
        /// <param name="SolarCompanyId"></param>
        /// <param name="JobReferenceOrId"></param>
        /// <param name="PVDSWHcode"></param>
        /// <param name="SPVMethod"></param>
        /// <param name="VerificationStatus"></param>
        /// <param name="ResponseCode"></param>
        /// <param name="Manufacturer"></param>
        /// <param name="ModelNumber"></param>
        /// <param name="SerialNumer"></param>
        /// <param name="FromRequestDate"></param>
        /// <param name="ToRequestDate"></param>
        /// <returns></returns>
        public List<SpvLog> GetSpvLogs(int PageSize, int PageNumber, string SortCol, string SortDir, string ServiceAdministrator, int ResellerId, int? SolarCompanyId = null, string JobReferenceOrId = null, string PVDSWHcode = null, int? SPVMethod = null, int? VerificationStatus = null, string ResponseCode = null, string Manufacturer = null, string ModelNumber = null, string SerialNumer = null, DateTime? FromRequestDate = null, DateTime? ToRequestDate = null)
        {
            List<SpvLog> lstSpvLogs = new List<SpvLog>();
            try
            {
                string spName = "[GetSPVLog]";
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, PageSize));
                sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, PageNumber));
                sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, SortCol));
                sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, SortDir));
                sqlParameters.Add(DBClient.AddParameters("ServiceAdministrator", SqlDbType.VarChar, (ServiceAdministrator == "All" ? "" : ServiceAdministrator)));
                sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
                sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, SolarCompanyId));
                sqlParameters.Add(DBClient.AddParameters("JobReferenceOrId", SqlDbType.VarChar, JobReferenceOrId));
                sqlParameters.Add(DBClient.AddParameters("PVDSWHcode", SqlDbType.VarChar, PVDSWHcode));
                sqlParameters.Add(DBClient.AddParameters("SPVMethod", SqlDbType.Int, SPVMethod));
                sqlParameters.Add(DBClient.AddParameters("VerificationStatus", SqlDbType.Int, VerificationStatus));
                sqlParameters.Add(DBClient.AddParameters("ResponseCode", SqlDbType.VarChar, ResponseCode));
                sqlParameters.Add(DBClient.AddParameters("Manufacturer", SqlDbType.VarChar, Manufacturer));
                sqlParameters.Add(DBClient.AddParameters("ModelNumber", SqlDbType.VarChar, ModelNumber));
                sqlParameters.Add(DBClient.AddParameters("SerialNumber", SqlDbType.VarChar, SerialNumer));
                sqlParameters.Add(DBClient.AddParameters("FromRequestDate", SqlDbType.DateTime, FromRequestDate));
                sqlParameters.Add(DBClient.AddParameters("ToRequestDate", SqlDbType.DateTime, ToRequestDate));
                lstSpvLogs = CommonDAL.ExecuteProcedure<SpvLog>(spName, sqlParameters.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                _log.LogException(SystemEnums.Severity.Error, $"SpvLogBal/GetSpvLogs", ex);
            }
            return lstSpvLogs.ToList();
        }
        /// <summary>
        /// Get Spv log details by SpvLogId
        /// </summary>
        /// <param name="spvLogId"></param>
        /// <returns></returns>
        public SPVLogDetail GetSPVLogDetailBySpvLogId(int spvLogId)
        {
            SPVLogDetail objSpvLogDetail = new SPVLogDetail();
            try
            {
                objSpvLogDetail = CommonDAL.SelectObject<SPVLogDetail>("[GetSPVLogDetailsBySpvLogId]", DBClient.AddParameters("spvLogId", SqlDbType.Int, spvLogId));
            }
            catch (Exception ex)
            {
                _log.LogException(SystemEnums.Severity.Error, $"SpvLogBal/GetSPVLogDetailBySpvLogId", ex);
            }
            return objSpvLogDetail;
        }
        /// <summary>
        /// Get all spv log data with filters.
        /// </summary>
        /// <param name="ServiceAdministrator"></param>
        /// <param name="ResellerId"></param>
        /// <param name="SolarCompanyId"></param>
        /// <param name="JobReferenceOrId"></param>
        /// <param name="PVDSWHcode"></param>
        /// <param name="SPVMethod"></param>
        /// <param name="VerificationStatus"></param>
        /// <param name="ResponseCode"></param>
        /// <param name="Manufacturer"></param>
        /// <param name="ModelNumber"></param>
        /// <param name="SerialNumer"></param>
        /// <param name="FromRequestDate"></param>
        /// <param name="ToRequestDate"></param>
        /// <returns>return table of spvlog data which has been expoted in csv.</returns>
        public DataTable GetDataForExportCSVForSpvLogs(string ServiceAdministrator, int? ResellerId, int? SolarCompanyId, string JobReferenceOrId = null, string PVDSWHcode = null, int? SPVMethod = null, int? VerificationStatus = null, string ResponseCode = null, string Manufacturer = null, string ModelNumber = null, string SerialNumer = null, DateTime? FromRequestDate = null, DateTime? ToRequestDate = null)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ServiceAdministrator", SqlDbType.VarChar, (ServiceAdministrator == "All" ? "" : ServiceAdministrator)));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int,ResellerId));
            sqlParameters.Add(DBClient.AddParameters("SolarComapanyId", SqlDbType.Int, SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("JobReferenceOrId", SqlDbType.VarChar, JobReferenceOrId));
            sqlParameters.Add(DBClient.AddParameters("PVDSWHcode", SqlDbType.VarChar, PVDSWHcode));
            sqlParameters.Add(DBClient.AddParameters("SPVMethod", SqlDbType.Int, SPVMethod));
            sqlParameters.Add(DBClient.AddParameters("VerificationStatus", SqlDbType.Int, VerificationStatus));
            sqlParameters.Add(DBClient.AddParameters("ResponseCode", SqlDbType.VarChar, ResponseCode));
            sqlParameters.Add(DBClient.AddParameters("Manufacturer", SqlDbType.VarChar, Manufacturer));
            sqlParameters.Add(DBClient.AddParameters("ModelNumber", SqlDbType.VarChar, ModelNumber));
            sqlParameters.Add(DBClient.AddParameters("SerialNumber", SqlDbType.VarChar, SerialNumer));
            sqlParameters.Add(DBClient.AddParameters("FromRequestDate", SqlDbType.DateTime, FromRequestDate));
            sqlParameters.Add(DBClient.AddParameters("ToRequestDate", SqlDbType.DateTime, ToRequestDate));
            DataTable dt = CommonDAL.ExecuteDataSet("ExportCSVForSpvLogs", sqlParameters.ToArray()).Tables[0];
            return dt;

        }

        public List<Spvmanufacturer> GetSPVManufacture(int PageSize, int PageNumber, string SortCol, string SortDir, string SPVManufactureName, string ServiceAdministrator)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, PageSize));
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, PageNumber));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, SortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, SortDir));
            sqlParameters.Add(DBClient.AddParameters("ServiceAdministrator", SqlDbType.VarChar, (ServiceAdministrator == "All" ? "" : ServiceAdministrator)));
            sqlParameters.Add(DBClient.AddParameters("SPVManufactureName", SqlDbType.VarChar, (SPVManufactureName == "All" ? "" : SPVManufactureName)));
            List<Spvmanufacturer> lstSPVManufactures = CommonDAL.ExecuteProcedure<Spvmanufacturer>("GetSPVManufacture", sqlParameters.ToArray()).ToList();
            return lstSPVManufactures;
        }

        ///<summary>
        ///Get supplier list by id
        ///</summary>
        ///<param name="id"></param>
        ///<returns>Return list of supplier</returns>
        public List<string> GetSupplierList(int id)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SPVManuFactureId", SqlDbType.Int, id));

            List<string> lstSupplier = CommonDAL.ExecuteProcedure<Spvmanufacturer>("GetSupplierByManufactureId", sqlParameters.ToArray()).ToList().Select(e => e.Supplier).ToList();
            return lstSupplier;
        }

        /// <summary>
        /// Exclude Reseller from manufacturer's panel
        /// </summary>
        /// <param name="resellerIds"></param>
        /// <param name="spvManufacturerId"></param>
        public void SaveExcludeReseller(string resellerIds, int spvManufacturerId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ResellerIds", SqlDbType.VarChar, resellerIds));
            sqlParameters.Add(DBClient.AddParameters("SPVManuFactureId", SqlDbType.Int, spvManufacturerId));

            CommonDAL.ExecuteProcedure<Spvmanufacturer>("SaveExcludeReseller", sqlParameters.ToArray());
        }

        /// <summary>
        /// Get excluded reseller from panel
        /// </summary>
        /// <param name="spvManufacturerId"></param>
        /// <returns></returns>
        public string GetExcludedReseller(int spvManufacturerId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SPVManuFactureId", SqlDbType.Int, spvManufacturerId));
            string strReseller = Convert.ToString(CommonDAL.ExecuteScalar("GetExcludedReseller", sqlParameters.ToArray()));
            return strReseller;
        }
    }
}

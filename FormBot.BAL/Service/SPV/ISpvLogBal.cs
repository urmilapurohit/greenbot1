using FormBot.Entity;
using FormBot.Entity.SPV;
using System;
using System.Collections.Generic;
using System.Data;

namespace FormBot.BAL.Service.SPV
{
    public interface ISpvLogBAL
    {
        /// <summary>
        /// Insert or update spv log details.
        /// </summary>
        /// <param name="objSpvLog"></param>
        /// <returns></returns>
        int InsertOrUpdate(SpvLog objSpvLog);
        /// <summary>
        /// Get all end point name from spv manufacturer list.
        /// </summary>
        /// <returns></returns>
        List<string> GetAllEndPointsNameFromSpvManufacturer();
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
        List<SpvLog> GetSpvLogs(int PageSize, int PageNumber, string SortCol, string SortDir, string ServiceAdministrator, int ResellerId, int? SolarCompanyId = null, string JobReferenceOrId = null, string PVDSWHcode = null, int? SPVMethod = null, int? VerificationStatus = null, string ResponseCode = null, string Manufacturer = null, string ModelNumber = null, string SerialNumer = null, DateTime? FromRequestDate = null, DateTime? ToRequestDate = null);
        /// <summary>
        /// Get Spv log details by SpvLogId
        /// </summary>
        /// <param name="spvLogId"></param>
        /// <returns></returns>
        SPVLogDetail GetSPVLogDetailBySpvLogId(int spvLogId);

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
        DataTable GetDataForExportCSVForSpvLogs(string ServiceAdministrator, int? ResellerId,int? SolarCompanyId, string JobReferenceOrId = null, string PVDSWHcode = null, int? SPVMethod = null, int? VerificationStatus = null, string ResponseCode = null, string Manufacturer = null, string ModelNumber = null, string SerialNumer = null, DateTime? FromRequestDate = null, DateTime? ToRequestDate = null);
         List<Spvmanufacturer> GetSPVManufacture(int PageSize, int PageNumber, string SortCol, string SortDir, string SPVManufactureName, string ServiceAdministrator);

        ///<summary>
        ///Get supplier list by id
        ///</summary>
        ///<param name="id"></param>
        ///<returns>Return list of supplier</returns>
        List<string> GetSupplierList(int id);

        /// <summary>
        /// Exclude Reseller from manufacturer's panel
        /// </summary>
        /// <param name="resellerIds"></param>
        /// <param name="spvManufacturerId"></param>
        void SaveExcludeReseller(string resellerIds, int spvManufacturerId);

        /// <summary>
        /// Get excluded reseller from panel
        /// </summary>
        /// <param name="spvManufacturerId"></param>
        /// <returns></returns>
        string GetExcludedReseller(int spvManufacturerId);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormBot.Entity;
using System.Data;
using FormBot.Entity.Job;

namespace FormBot.BAL.Service
{
    public interface ISolarCompanyBAL
    {

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <returns>solar list</returns>
        List<SolarCompany> GetData();

        /// <summary>
        /// Gets the sub contractor data.
        /// </summary>
        /// <returns>solar list</returns>
        List<SolarCompany> GetSubContractorData();

        /// <summary>
        /// Gets the solar company by reseller identifier.
        /// </summary>
        /// <param name="id">The Reseller identifier.</param>
        /// <returns>List of SolarCompanies under this Reseller</returns>
        List<SolarCompany> GetSolarCompanyByResellerID(int id);

        /// <summary>
        /// Gets the solar company by  mutliple reseller identifier.
        /// </summary>
        /// <param name="ids">The Reseller identifiers.</param>
        /// <returns>List of SolarCompanies under Resellers</returns>
        List<SolarCompany> GetSolarCompanyByMultipleResellerID(string id);

        /// <summary>
        /// Gets the solar company by ramid.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>solar list</returns>
        List<SolarCompany> GetSolarCompanyByRAMID(int id);

        /// <summary>
        /// Gets the solar company by solar company identifier.
        /// </summary>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <returns>solar object</returns>
        SolarCompany GetSolarCompanyBySolarCompanyID(int? solarCompanyId);

        /// <summary>
        /// Gets the solar company for global pricing.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <param name="ResellerId">The reseller identifier.</param>
        /// <param name="PricingType">Type of the pricing.</param>
        /// <returns>solar list</returns>
        List<SolarCompany> GetSolarCompanyForGlobalPricing(int UserId, int UserTypeId, int ResellerId, int PricingType);

        /// <summary>
        /// Gets the solar company for custom pricing.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <param name="ResellerId">The reseller identifier.</param>
        /// <param name="SolarCompany">The solar company.</param>
        /// <param name="RAMID">The ramid.</param>
        /// <param name="name">The name.</param>
        /// <returns>solar list</returns>
        List<SolarCompany> GetSolarCompanyForCustomPricing(int UserId, int UserTypeId, int ResellerId, string SolarCompany, int RAMID, string name);

        /// <summary>
        /// Gets the requested solar company to SSC.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>solar list</returns>
        List<SolarCompany> GetRequestedSolarCompanyToSSC(int id);


        /// <summary>
        /// Get SolarCompany Of SolarElectician
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<SolarCompany> GetSolarCompaanyFromSE(int id);

        List<SolarCompany> GetSolarCompany_IsSSCByResellerID(int id, bool IsSSC);

        // List<SolarCompany> GetSolarCompany_IsSSCByResellerID(bool IsSSC);


        /// <summary>
        /// Gets the solar company by whole saler identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        List<SolarCompany> GetSolarCompanyByWholeSalerID(int id);

        // bool CheckIsWholeSaler_ByResellerId(int id);
        DataSet CheckIsWholeSaler_ByResellerId(int id);

        List<SolarCompany> GetSolarCompanyForVEECGlobalPricing(int UserId, int UserTypeId, int ResellerId, int PricingType);

        void UpdateSolarCompanyAllowedSPV(string solarCompayIds, int isSPVAllowedSPV);
        /// <summary>
        /// Gets Representative for auto sign
        /// </summary>
        /// <returns>List of SolarCompanies/SCO </returns>
        List<Representative> GetRepresentativeForAutoSign(int jobId=0);
        DataSet GetAutoSignSettingsData(int UserId,bool isForDefaultSelection);
        void SaveAutoSignSettingsData(int UserId, string Position, bool isSubcontractor, bool isEmployee, string signature,bool isChangedDesign,int JobId, string latitude, string longitude);

        void UpdateInstallerWrittenStatementSetting(int jobId, bool isEmployee, bool isChangedDesign);
    }
}



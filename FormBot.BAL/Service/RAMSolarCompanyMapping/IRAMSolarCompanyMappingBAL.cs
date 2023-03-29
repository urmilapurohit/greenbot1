using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormBot.Entity;
using System.Data;
using System.Web.Mvc;

namespace FormBot.BAL.Service
{
    public interface IRAMSolarCompanyMappingBAL
    {
        /// <summary>
        /// get GetResellerAccountManager
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Get Single Reseller</returns>
        List<User> GetResellerAccountManager(int userId);

        /// <summary>
        /// get solar company list
        /// </summary>
        /// <param name="userID">The UserId</param>
        /// <returns>Get List Of Solar Company</returns>
        IEnumerable<SelectListItem> RAMSolarCompanyList(int userID);

        /// <summary>
        /// insert record into RAMSolarCompanyMapping
        /// </summary>
        /// <param name="rAMID">The ram id.</param>
        /// <param name="solarCompanyIDs">The solar company Ids.</param>
        /// <returns>
        /// The List of Solar Company Mapping
        /// </returns>
        object CreateRAMSolarCompanyMapping(int rAMID, string solarCompanyIDs);

        /// <summary>
        /// Gets the assigned solar company to ram.
        /// </summary>
        /// <param name="id">The RAM identifier.</param>
        /// <returns>List Of SolarCompanies assigned to this RAM</returns>
        List<SolarCompany> GetAssignedSolarCompanyToRAM(int id);

        /// <summary>
        /// Gets the assign ram solar company list.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <param name="ramID">The ram identifier.</param>
        /// <returns>select list</returns>
        List<SelectListItem> GetAssignRAMSolarCompanyList(int userID, int ramID);

        /// <summary>
        /// Gets all ram solar company list.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <returns>select list</returns>
        IEnumerable<SelectListItem> GetAllRAMSolarCompanyList(int userID);

        /// <summary>
        /// Gets the ram by user identifier.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <returns>user identifier</returns>
        int GetRAMByUserId(int userID);

        /// <summary>
        /// get GetRAMByReseller
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Get RAM</returns>
        List<User> GetRAMByReseller(int resellerId);

    }
}

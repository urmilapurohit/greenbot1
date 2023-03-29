using FormBot.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormBot.Helper;
using System.Data;
namespace FormBot.BAL.Service
{
    public interface ICERImportBAL
    {
        /// <summary>
        /// Read excel file from given path and execute procedure
        /// </summary>
        /// <param name="ExcelStream">The excel stream.</param>
        /// <param name="cerType">The cer Type.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="version">The version.</param>
        /// <param name="subtype">The subtype.</param> 
        /// <returns>Merge Data Table</returns>
        string MergeDataTable(Stream ExcelStream, SystemEnums.CERType cerType, string filePath, string version, SystemEnums.CERSubType subtype = 0);
        
        /// <summary>
        /// Gets the cer log.
        /// </summary>
        /// <param name="fileType">Type of the file.</param>
        /// <param name="subType">Type of the sub.</param>
        /// <returns>cer text</returns>
        string GetCERLog(SystemEnums.CERType fileType, SystemEnums.CERSubType subType);

       /// <summary>
        /// Gets the hw brand model data into list.
       /// </summary>
        /// <param name="PageNumber">Page Number</param>
        /// <param name="PageSize">Page Size</param>
        /// <param name="SortColumn">Sort Column</param>
        /// <param name="SortDirection">Sort Direction</param>
        /// <param name="Item">Item</param>
        /// <param name="Brand">Brand</param>
        /// <param name="Model">Model</param>
        /// <param name="EligibleFrom">Eligible From</param>
        /// <param name="EligibleTo">Eligible To</param>
        /// <returns>HW Brand Model List</returns>
        IList<HWBrandModel> HWBrandModelList(int PageNumber, int PageSize, string SortColumn, string SortDirection, string Item, string Brand, string Model, string EligibleFrom, string EligibleTo);

        /// <summary>
        /// Gets the accredited installers data into list.
        /// </summary>
        /// <param name="PageNumber">Page Number</param>
        /// <param name="PageSize">Page Size</param>
        /// <param name="SortColumn">Sort Column</param>
        /// <param name="SortDirection">Sort Direction</param>
        /// <param name="AccreditationNumber">Accreditation Number</param>
        /// <param name="name">name</param>
        /// <param name="AccountName">Account Name</param>
        /// <param name="LicensedElectricianNumber">Licensed Electrician Number</param>
        /// <param name="GridType">Grid Type</param>
        /// <returns>Accredited Installers List</returns>
        IList<AccreditedInstallers> AccreditedInstallersList(int PageNumber, int PageSize, string SortColumn, string SortDirection, string AccreditationNumber, string name, string AccountName, string LicensedElectricianNumber, string GridType);

       /// <summary>
        /// Gets the inverters data into list.
       /// </summary>
        /// <param name="PageNumber">Page Number</param>
        /// <param name="PageSize">Page Size</param>
        /// <param name="SortColumn">Sort Column</param>
        /// <param name="SortDirection">Sort Direction</param>
        /// <param name="manufacturer">manu facturer</param>
        /// <param name="series">series</param>
        /// <param name="modelNumber">model Number</param>
        /// <param name="acPowerKW">ac Power KW</param>
        /// <param name="approvalDate">approval Date</param>
        /// <param name="expiryDate">expiry Date</param>
        /// <returns>Inverters List</returns>
        IList<Inverters> InvertersList(int PageNumber, int PageSize, string SortColumn, string SortDirection, string manufacturer, string series, string modelNumber, string acPowerKW, string approvalDate, string expiryDate);

      /// <summary>
        /// Gets the pv modules data into list.
      /// </summary>
        /// <param name="PageNumber">Page Number</param>
        /// <param name="PageSize">Page Size</param>
        /// <param name="SortColumn">Sort Column</param>
        /// <param name="SortDirection">Sort Direction</param>
        /// <param name="CertificateHolder">Certificate Holder</param>
        /// <param name="ModelNumber">Model Number</param>
        /// <param name="Wattage">Wattage</param>
        /// <param name="CECApprovedDate">CEC Approved Date</param>
        /// <param name="ExpiryDate">Expiry Date</param>
        /// <returns>Modules List</returns>
        IList<PVModules> ModulesList(int PageNumber, int PageSize, string SortColumn, string SortDirection, string CertificateHolder, string ModelNumber, string Wattage, string CECApprovedDate, string ExpiryDate);

        IList<BatteryStorage> BatteryStorageList(int PageNumber, int PageSize, string SortColumn, string SortDirection, string manufacturer, string ModelNumber);

        /// <summary>
        /// Updates the pv modules.
        /// </summary>
        /// <param name="PVModuleId">The pv module identifier.</param>
        /// <param name="Wattage">The wattage.</param>
        void UpdatePVModules(int PVModuleId, int Wattage);

        IList<ElectricityProvider> ElectricityProviderList(int pageNumber, int pageSize, string sortColumn, string sortDirection, string provider, string type, string state, string preapprovals, string connections);

        void DeleteElectricityProvider(int electricityProviderId);

        void UpdateElectricityProvider(string electricityProviderId, string provider, string type, string state, string preapproval, string connection);

        /// <summary>
        /// Gets all pv modules.
        /// </summary>
        /// <returns>Data Set</returns>
        DataSet GetAllModules();

        AccreditedInstallers GetAccreditedInstallerDetailByAccreditedInstallerId(int AccreditedInstallerId);

        /// <summary>
        /// Insert Spv manufacture and its verification url
        /// </summary>
        /// <param name="SPVDatatable"></param>
        void SyncSPVJson(DataTable SPVDatatable,bool isFromSyncJson=false,bool isFromUploadJson=false,string fileName=null);
        IList<Spvmanufacturer> GetManufacturerForSetSpvByManufacturer();

        void SaveSpvSetByManufacturerPopUp(string Spvmanufacturerid);

        void SyncAccreditedInstallerList(DataTable SyncAccreditedInstallerListDatatable);
    }
}

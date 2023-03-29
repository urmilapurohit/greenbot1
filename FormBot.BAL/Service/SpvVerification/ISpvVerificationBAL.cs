using FormBot.Entity;
using FormBot.Entity.SpvVerification;
using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL
{
    public interface ISpvVerificationBAL
    {
        List<SpvPanelManufacturer> GetData();
        void BulkInsertSpvPanelManufacturer(DataTable dtSpvPanelManufacturer, int SpvUserId);
        List<SpvPanelDetailsModel> GetSpvPanelDetailsSearchByManufacturer(int SpvUserId, string SerialNumber, string ModelNumber, bool isDownload = false);
        List<SpvPanelDetails> GetSpvPanelDetails(string SerialNumber,string ModelNumber,string Supplier,string Manufacturer);
        bool CheckManufacturerIsExsistOrNot(string ManufacturerName);
        void InsertSpvVerifiedSerialNos(string serialnumbers, int createdBy);
        void ReleaseSerailNumbers (string spvPanelDetailsIds);
        SpvPanelDetailsUploadHistory SpvPanelDetailsUploadHistoryInsert(string FilePath, string FileName, int ManufacturerId);
        List<SpvPanelDetailsUploadHistory> GetSpvPanelDetailsUploadedHistory(int ManufacturerId);
        /// <summary>
        /// Get supplier list by manufacturer name
        /// </summary>
        /// <param name="ManufacturerName"></param>
        /// <returns>list of supplier</returns>
        List<string> GetSupplierByManufacturer(string ManufacturerName);
    }
}

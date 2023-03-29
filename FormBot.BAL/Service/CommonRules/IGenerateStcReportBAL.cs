using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormBot.Entity;
using System.Web.Mvc;
using System.Data;

namespace FormBot.BAL.Service.CommonRules
{
    public interface IGenerateStcReportBAL
    {
        //String CreateStcReport(string Filename, string ExportType, int STCJobDetailsID, string InvoiceNo, string solarCompanyId, string userTypeId, int userId, int rId, bool IsWindowService, bool RegenerateRemittanceFile = false, bool IsBackgroundRecProcess = false);
        String CreateStcReportNew(string Filename, string ExportType, int STCJobDetailsID, string InvoiceNo, string solarCompanyId, string userTypeId, int userId, int rId, bool IsWindowService, bool RegenerateRemittanceFile = false, bool IsBackgroundRecProcess = false);

        //int GenerateRemittance(DataSet remittanceData, string resellerId);
        int GenerateRemittanceNew(Remittance remittanceData, string resellerId);

        Entity.Settings.Settings GetSettingsData(string SolarCompanyId, string UserTypeId, int UserId, int ResellerId);

        String ByteArrayToFile(string _FileName, byte[] _ByteArray, string jobID, bool IsWindowService);

        string MoveDeletedDocuments(string sourcePath, string JobId, string UserId = null);
    }
}

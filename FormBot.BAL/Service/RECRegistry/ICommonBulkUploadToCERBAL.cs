using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL.Service.RECRegistry
{
    public interface ICommonBulkUploadToCERBAL 
    {
        StringBuilder GetBulkUploadCSV_PVD(string JobID, string FilePath, ref DataSet dtCSV_JobID, ref DataTable dtSPVXmlPath, bool IsFileCreation = false, bool isDownloadCSVFile = false);
        StringBuilder GetBulkUploadSWHCSV(string JobID, string FilePath, ref DataSet dtCSV_JobID, bool IsFileCreation = false, bool isDownloadCSVFile = false);
    }
}

using FormBot.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FormBot.BAL.Service.CommonRules
{
    public interface IJobRulesBAL
    {
        List<string> RemoveRequiredFields(CreateJob createJob);

        KeyValuePair<bool,Int32> InsertCreateJobData(ref CreateJob createJob, string panelXml, string inverterXml, int solarCompanyId, int userId, string calculatSTCUrl = "");

        KeyValuePair<bool, string> CalculateSTC(string sguType, string expectedInstallDate, string deemingPeriod, string postcode, string systemsize, string CalculateSTCUrl);

        KeyValuePair<bool, string> CalculateSWHSTC(string expectedInstallDate, string postcode, string systemBrand, string systemModel, string CalculateSWHSTCUrl);

        KeyValuePair<bool, decimal?> GetSTCValue(int jobType, string jobInstallationDate, string deemingPeriod, string postcode, decimal? SystemSize, string systemBrand, string systemModel, string CalculateSTCUrl);

        //string CalculateSTC(string sguType, string expectedInstallDate, string deemingPeriod, string postcode, string systemsize, string CalculateSTCUrl);

        //string CalculateSWHSTC(string expectedInstallDate, string postcode, string systemBrand, string systemModel, string CalculateSWHSTCUrl);

        bool IsDigitsOnly(string str);

        List<SelectListItem> GetDeemingPeriod(string year);

        void SendMailOnCERFailed(string stcjobids, bool isWindowService = false);

        void CreateSTCInvoicePDFForRECData(DataTable dt, bool isWindowService = false);

    }
}

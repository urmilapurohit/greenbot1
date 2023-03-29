using FormBot.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL.Service
{
    public interface IInvoicerDetails
    {
        void SaveAccountCodes(string Code, string Name, string TaxType, int isSync);

        void SaveInvoicerDetails(Invoicer invoicer);

        List<Invoicer> GetInvoicerList(int PageNumber, int PageSize, string InvoicerName, string AccountCode);
    }
}

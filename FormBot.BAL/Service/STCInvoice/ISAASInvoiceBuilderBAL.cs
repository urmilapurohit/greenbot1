using FormBot.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL.Service
{
    public interface ISAASInvoiceBuilderBAL
    {
        DataSet GetSAASInvoiceBuilderList(int PageNumber, int pageSize, string SAASUserId, string userTypeID, string UserRole);

        DataSet GetSAASInvoiceBuilderListBasedOnTerms(int PageNumber, int pageSize, string SAASUserId, string userTypeID, string UserRole,bool IsIsArchive);

        dynamic GetSAASInvoiceBuilderDetail(string InvoiceNumber);

        dynamic GetSAASInvoiceBuilderDetailBasedOnTerms(string InvoiceNumber);

        //void SendToSAASInvoices(string invoiceIds);
        void SendToSAASInvoices(SAASInvoiceBuilder objSAASInvoiceBuilder);
        dynamic GetUserTypes();

        dynamic GetUserRoles(int id);

        /// <summary>
        /// Gets the sass users list.
        /// </summary>
        /// <returns>List</returns>
        List<Reseller> GetSAASUsers();

        void DeleteSAASInvoiceFomBuilderByID(string jobid);

        void CreateNewInvoice(string UserID, string ResellerID, string ResellerName, string SettelmentTerm, string Rate, string QTY, string BillingPeriod, string GlobalTermId, string IsGlobalGST, string JobID);

        dynamic GetMonthAndQTYBasedOnTerms(string SettelmentTerm, string Rate,string UserID);

        List<SAASPricingManager> GetGlobalBillingTermsList(string UserID);
    }
}

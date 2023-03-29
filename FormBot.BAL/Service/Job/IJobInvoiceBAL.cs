using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormBot.Entity;
using FormBot.Entity.Job;

namespace FormBot.BAL.Service.Job
{
    public interface IJobInvoiceBAL
    {
        /// <summary>
        /// Gets the job invoice.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <param name="userID">The user identifier.</param>
        /// <returns>invoice list</returns>
        List<JobInvoice> GetJobInvoice(int jobID, int pageNumber, int pageSize,string sortColumn, string sortDirection, int userID);

        /// <summary>
        /// Deletes the invoice.
        /// </summary>
        /// <param name="InvoiceID">The invoice identifier.</param>
        void DeleteInvoice(int InvoiceID);

        /// <summary>
        /// Sends the invoice.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="invoiceId">The invoice identifier.</param>
        /// <param name="userType">Type of the user.</param>
        void SendInvoice(string id, string invoiceId, string userType);

        /// <summary>
        /// Updates the invoice number.
        /// </summary>
        /// <param name="invoiceID">The invoice identifier.</param>
        /// <param name="invoiceNumber">The invoice number.</param>
        /// <param name="status">The status.</param>
        /// <returns>invoice number</returns>
        string UpdateInvoiceNumber(string invoiceID, string invoiceNumber, int status);

        /// <summary>
        /// Updates the invoice xero detail.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="invoiceID">The invoice identifier.</param>
        /// <param name="jobInvoiceIDXero">The job invoice identifier xero.</param>
        /// <param name="idsXml">The ids XML.</param>
        /// <param name="invoicePaidStatus">The invoice paid status.</param>
        /// <param name="invoiceNumber">The invoice number.</param>
        void UpdateInvoiceXeroDetail(string paramName, int invoiceID, string jobInvoiceIDXero,string idsXml, int invoicePaidStatus, string invoiceNumber);

        /// <summary>
        /// Gets the invoice send to details.
        /// </summary>
        /// <typeparam name="T">type param</typeparam>
        /// <param name="userID">The user identifier.</param>
        /// <param name="userTypeID">The user type identifier.</param>
        /// <param name="jobID">The job identifier.</param>
        /// <returns>T type</returns>
        List<T> GetInvoiceSendToDetails<T>(int userID, int userTypeID, int jobID);

        /// <summary>
        /// Gets the job invoice by identifier.
        /// </summary>
        /// <param name="jobInvoiceID">The job invoice identifier.</param>
        /// <returns>invoice object</returns>
        JobInvoice GetJobInvoiceById(int jobInvoiceID);

        /// <summary>
        /// Gets the invoice extra description.
        /// </summary>
        /// <param name="jobInvoiceID">The job invoice identifier.</param>
        /// <param name="IsJobAddress">if set to <c>true</c> [is job address].</param>
        /// <param name="IsJobDate">if set to <c>true</c> [is job date].</param>
        /// <param name="IsJobDescription">if set to <c>true</c> [is job description].</param>
        /// <param name="IsTitle">if set to <c>true</c> [is title].</param>
        /// <param name="IsName">if set to <c>true</c> [is name].</param>
        /// <returns>tuple list</returns>
        Tuple<DateTime, string, string, string> GetInvoiceExtraDescription(int jobInvoiceID, bool IsJobAddress,bool IsJobDate, bool IsJobDescription, bool IsTitle, bool IsName);

        /// <summary>
        /// Gets all job invoice.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <param name="userID">The user identifier.</param>
        /// <param name="InvoiceNumber">The invoice number.</param>
        /// <param name="InvoiceDate">The invoice date.</param>
        /// <param name="Status">The status.</param>
        /// <param name="OwnerName">Name of the owner.</param>
        /// <returns>invoice list</returns>
        List<JobInvoice> GetAllJobInvoice(int pageNumber, int pageSize, string sortColumn, string sortDirection, int userID, string InvoiceNumber, DateTime? InvoiceDate, int Status, string OwnerName);

        /// <summary>
        /// Updates the is invoiced by invoice identifier.
        /// </summary>
        /// <param name="invoiceId">The invoice identifier.</param>
        /// <returns>invoice string</returns>
        string UpdateIsInvoicedByInvoiceID(int invoiceId);
    }
}

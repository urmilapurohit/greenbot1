using FormBot.DAL;
using FormBot.Entity;
using FormBot.Entity.Job;
using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL.Service.Job
{
    public class JobInvoiceBAL : IJobInvoiceBAL
    {
        public List<JobInvoice> GetJobInvoice(int jobID, int pageNumber, int pageSize, string sortColumn, string sortDirection, int userID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, pageSize));
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, pageNumber));
            sqlParameters.Add(DBClient.AddParameters("SortColumn ", SqlDbType.NVarChar, sortColumn));
            sqlParameters.Add(DBClient.AddParameters("SortDirection", SqlDbType.NVarChar, sortDirection));
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, userID));
            List<JobInvoice> lstJobInvoice = CommonDAL.ExecuteProcedure<JobInvoice>("Job_GetInvoiceByJobId", sqlParameters.ToArray()).ToList();
            if (lstJobInvoice.Any())
            {
                lstJobInvoice.ToList().ForEach(c => c.InvoicedTo = !string.IsNullOrEmpty(c.OwnerName) ? c.OwnerName : c.Send);
                lstJobInvoice.ToList().ForEach(c => c.InvoiceAmountDue = c.InvoiceTotal - c.InvoiceAmountPaid);
                lstJobInvoice.ToList().ForEach(c => c.FileExist = System.IO.File.Exists(System.IO.Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + "JobDocuments" + "\\" + c.JobId + "\\" + "Invoice\\Report\\" + c.InvoiceNumber + ".pdf")) ? 1 : 0);
            }

            return lstJobInvoice;
        }

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
        /// <returns>
        /// invoice list
        /// </returns>
        public List<JobInvoice> GetAllJobInvoice(int pageNumber, int pageSize, string sortColumn, string sortDirection, int userID, string InvoiceNumber, DateTime? InvoiceDate, int Status, string OwnerName)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("InvoiceNumber", SqlDbType.NVarChar, InvoiceNumber));
            sqlParameters.Add(DBClient.AddParameters("InvoiceDate", SqlDbType.Date, InvoiceDate));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, pageSize));
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, pageNumber));
            sqlParameters.Add(DBClient.AddParameters("SortColumn ", SqlDbType.NVarChar, sortColumn));
            sqlParameters.Add(DBClient.AddParameters("SortDirection", SqlDbType.NVarChar, sortDirection));
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, userID));
            sqlParameters.Add(DBClient.AddParameters("OwnerName", SqlDbType.NVarChar, OwnerName));
            sqlParameters.Add(DBClient.AddParameters("Status", SqlDbType.Int, Status));
            List<JobInvoice> lstJobInvoice = CommonDAL.ExecuteProcedure<JobInvoice>("[GetAllJobInvoices]", sqlParameters.ToArray()).ToList();
            if (lstJobInvoice.Any())
            {
                lstJobInvoice.ToList().ForEach(c => c.InvoicedTo = !string.IsNullOrEmpty(c.OwnerName) ? c.OwnerName : c.Send);
                lstJobInvoice.ToList().ForEach(c => c.InvoiceAmountDue = Math.Round(Convert.ToDecimal(c.InvoiceTotal - c.InvoiceAmountPaid), 2));
                lstJobInvoice.ToList().ForEach(c => c.FileExist = System.IO.File.Exists(System.IO.Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + "JobDocuments" + "\\" + c.JobId + "\\" + "Invoice\\Report\\" + c.InvoiceNumber + ".pdf")) ? 1 : 0);
            }

            return lstJobInvoice;
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Description</returns>
        public string GetDescription(Enum value)
        {
            string description = value.ToString();
            FieldInfo fieldInfo = value.GetType().GetField(description);
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                description = attributes[0].Description;
            }

            return description;
        }

        /// <summary>
        /// Deletes the invoice.
        /// </summary>
        /// <param name="InvoiceID">The invoice identifier.</param>
        public void DeleteInvoice(int InvoiceID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobInvoiceID", SqlDbType.Int, InvoiceID));
            CommonDAL.Crud("Job_DeleteInvoice", sqlParameters.ToArray());
        }

        /// <summary>
        /// Sends the invoice.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="invoiceId">The invoice identifier.</param>
        /// <param name="userType">Type of the user.</param>
        public void SendInvoice(string id, string invoiceId, string userType)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("id", SqlDbType.NVarChar, id));
            sqlParameters.Add(DBClient.AddParameters("invoiceId", SqlDbType.NVarChar, invoiceId));
            sqlParameters.Add(DBClient.AddParameters("userType", SqlDbType.NVarChar, userType));
            sqlParameters.Add(DBClient.AddParameters("InvoiceSentDate", SqlDbType.DateTime, DateTime.Now));
            CommonDAL.Crud("Job_UpdateInvoiceSend", sqlParameters.ToArray());
        }

        /// <summary>
        /// Updates the invoice number.
        /// </summary>
        /// <param name="invoiceId">The invoice identifier.</param>
        /// <param name="invoiceNumber">The invoice number.</param>
        /// <param name="status">The status.</param>
        /// <returns>Invoice Number</returns>
        public string UpdateInvoiceNumber(string invoiceId, string invoiceNumber, int status)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("invoiceId", SqlDbType.NVarChar, invoiceId));
            sqlParameters.Add(DBClient.AddParameters("invoiceNumber", SqlDbType.NVarChar, invoiceNumber));
            sqlParameters.Add(DBClient.AddParameters("status", SqlDbType.Int, status));
            object obj = CommonDAL.ExecuteScalar("Job_UpdateInvoiceNumber", sqlParameters.ToArray());
            return Convert.ToString(obj);
        }

        /// <summary>
        /// Updates the invoice xero detail.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="invoiceID">The invoice identifier.</param>
        /// <param name="jobInvoiceIDXero">The job invoice identifier xero.</param>
        /// <param name="idsXml">The ids XML.</param>
        /// <param name="invoicePaidStatus">The invoice paid status.</param>
        /// <param name="invoiceNumber">The invoice number.</param>
        public void UpdateInvoiceXeroDetail(string paramName, int invoiceID, string jobInvoiceIDXero, string idsXml, int invoicePaidStatus, string invoiceNumber)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("invoiceID", SqlDbType.Int, invoiceID));
            sqlParameters.Add(DBClient.AddParameters("jobInvoiceIDXero", SqlDbType.NVarChar, jobInvoiceIDXero));
            sqlParameters.Add(DBClient.AddParameters("paramName", SqlDbType.NVarChar, paramName));
            sqlParameters.Add(DBClient.AddParameters("idsXml", SqlDbType.Xml, idsXml));
            sqlParameters.Add(DBClient.AddParameters("invoicePaidStatus", SqlDbType.Int, invoicePaidStatus));
            sqlParameters.Add(DBClient.AddParameters("invoiceNumber", SqlDbType.NVarChar, invoiceNumber));
            sqlParameters.Add(DBClient.AddParameters("xeroDate", SqlDbType.DateTime, DateTime.Now));
            CommonDAL.Crud("Job_UpdateInvoiceXeroDetail", sqlParameters.ToArray());
        }

        /// <summary>
        /// Updates the is invoiced by invoice identifier.
        /// </summary>
        /// <param name="invoiceId">The invoice identifier.</param>
        /// <returns>
        /// invoice string
        /// </returns>
        public string UpdateIsInvoicedByInvoiceID(int invoiceId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("invoiceId", SqlDbType.Int, invoiceId));
            object obj = CommonDAL.ExecuteScalar("UpdateIsInvoiceDetailByInvoiceID", sqlParameters.ToArray());
            return Convert.ToString(obj);
        }

        /// <summary>
        /// Gets the invoice send to details.
        /// </summary>
        /// <typeparam name="T">type param</typeparam>
        /// <param name="userID">The user identifier.</param>
        /// <param name="userTypeID">The user type identifier.</param>
        /// <param name="jobID">The job identifier.</param>
        /// <returns>
        /// T type
        /// </returns>
        public List<T> GetInvoiceSendToDetails<T>(int userID, int userTypeID, int jobID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("jobID ", SqlDbType.Int, jobID));
            sqlParameters.Add(DBClient.AddParameters("userTypeID", SqlDbType.Int, userTypeID));
            sqlParameters.Add(DBClient.AddParameters("userID", SqlDbType.Int, userID));
            IList<T> lstJobInvoice = CommonDAL.ExecuteProcedure<T>("Job_GetInvoiceSendToDetails", sqlParameters.ToArray());
            return lstJobInvoice.ToList();
        }

        /// <summary>
        /// Gets the job invoice by identifier.
        /// </summary>
        /// <param name="jobInvoiceID">The job invoice identifier.</param>
        /// <returns>
        /// invoice object
        /// </returns>
        public JobInvoice GetJobInvoiceById(int jobInvoiceID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("jobInvoiceID", SqlDbType.Int, jobInvoiceID));
            JobInvoice lstJobInvoice = CommonDAL.SelectObject<JobInvoice>("Job_GetJobInvoiceById", sqlParameters.ToArray());
            if (lstJobInvoice != null)
            {
                lstJobInvoice.InvoicedTo = !string.IsNullOrEmpty(lstJobInvoice.OwnerUsername) ? lstJobInvoice.OwnerUsername : lstJobInvoice.Send;
                lstJobInvoice.InvoiceAmountDue = Math.Round(Convert.ToDecimal(lstJobInvoice.InvoiceTotal - lstJobInvoice.InvoiceAmountPaid), 2);
                lstJobInvoice.FileExist = System.IO.File.Exists(System.IO.Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + "JobDocuments" + "\\" + lstJobInvoice.JobId + "\\" + "Invoice\\Report\\" + lstJobInvoice.FileName + ".pdf")) ? 1 : 0;
            }
            return lstJobInvoice;
        }

        /// <summary>
        /// Gets the invoice extra description.
        /// </summary>
        /// <param name="jobInvoiceID">The job invoice identifier.</param>
        /// <param name="IsJobAddress">if set to <c>true</c> [is job address].</param>
        /// <param name="IsJobDate">if set to <c>true</c> [is job date].</param>
        /// <param name="IsJobDescription">if set to <c>true</c> [is job description].</param>
        /// <param name="IsTitle">if set to <c>true</c> [is title].</param>
        /// <param name="IsName">if set to <c>true</c> [is name].</param>
        /// <returns>
        /// tuple list
        /// </returns>
        public Tuple<DateTime, string, string, string> GetInvoiceExtraDescription(int jobInvoiceID, bool IsJobAddress, bool IsJobDate, bool IsJobDescription, bool IsTitle, bool IsName)
        {
            try
            {
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(DBClient.AddParameters("JobInvoiceID", SqlDbType.Int, jobInvoiceID));
                sqlParameters.Add(DBClient.AddParameters("IsJobAddress", SqlDbType.Bit, IsJobAddress));
                sqlParameters.Add(DBClient.AddParameters("IsJobDate", SqlDbType.Bit, IsJobDate));
                sqlParameters.Add(DBClient.AddParameters("IsJobDescription", SqlDbType.Bit, IsJobDescription));
                sqlParameters.Add(DBClient.AddParameters("IsTitle", SqlDbType.Bit, IsTitle));
                sqlParameters.Add(DBClient.AddParameters("IsName", SqlDbType.Bit, IsName));
                DataSet dataSet = CommonDAL.ExecuteDataSet("jobs_InvoiceExtraDescription", sqlParameters.ToArray());
                string description = string.Empty, title = string.Empty, address = string.Empty;
                DateTime installationDate = DateTime.MinValue;
                if (dataSet != null && dataSet.Tables.Count > 0)
                {
                    if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(Convert.ToString(dataSet.Tables[0].Rows[0]["installationdate"])))
                        {
                            installationDate = Convert.ToDateTime(dataSet.Tables[0].Rows[0]["installationdate"]);
                        }
                    }
                    if (dataSet.Tables.Count > 1 && dataSet.Tables[1].Rows.Count > 0)
                    {
                        description = Convert.ToString(dataSet.Tables[1].Rows[0]["description"]);
                    }
                    if (dataSet.Tables.Count > 2 && dataSet.Tables[2].Rows.Count > 0)
                    {
                        title = Convert.ToString(dataSet.Tables[2].Rows[0]["title"]);
                    }
                    if (dataSet.Tables.Count > 3 && dataSet.Tables[3].Rows.Count > 0)
                    {
                        address = Convert.ToString(dataSet.Tables[3].Rows[0]["address"]);
                    }
                }
                return new Tuple<DateTime, string, string, string>(installationDate, description, title, address);
            }
            catch
            {
                return new Tuple<DateTime, string, string, string>(DateTime.MinValue, "", "", "");
            }
        }
    }
}

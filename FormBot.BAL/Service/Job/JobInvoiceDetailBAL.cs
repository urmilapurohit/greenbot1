using FormBot.DAL;
using FormBot.Entity;
using FormBot.Entity.Job;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormBot.Helper;

namespace FormBot.BAL.Service.Job
{
    public class JobInvoiceDetailBAL : IJobInvoiceDetailBAL
    {
        /// <summary>
        /// Gets the job invoice detail.
        /// </summary>
        /// <param name="jobInvoiceID">The job invoice identifier.</param>
        /// <param name="jobInvoiceType">Type of the job invoice.</param>
        /// <param name="isInvoiced">The is invoiced.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <returns>
        /// invoice list
        /// </returns>
        public List<JobInvoiceDetail> GetJobInvoiceDetail(string jobInvoiceID, int jobInvoiceType, int isInvoiced, string sortColumn, string sortDirection)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobInvoiceID", SqlDbType.Int, jobInvoiceID));
            sqlParameters.Add(DBClient.AddParameters("jobInvoiceType", SqlDbType.Int, jobInvoiceType));
            sqlParameters.Add(DBClient.AddParameters("isInvoiced", SqlDbType.Int, isInvoiced));
            sqlParameters.Add(DBClient.AddParameters("SortColumn ", SqlDbType.NVarChar, sortColumn));
            sqlParameters.Add(DBClient.AddParameters("SortDirection", SqlDbType.NVarChar, sortDirection));
            IList<JobInvoiceDetail> lstJobInvoice = CommonDAL.ExecuteProcedure<JobInvoiceDetail>("Job_GetJobInvoiceDetailByID2", sqlParameters.ToArray());
            if (lstJobInvoice.Count > 0)
            {
                var dueDate = DateTime.Now;
                var accountCode = "200";
                var paymrntAccountCode = "881";
                decimal invoiceTax = 0;
                var invoiceSetting = GetInvoiceSetting();
                bool isTaxInclusive = false;

                if (invoiceSetting.Item1 != null && invoiceSetting.Item1 != DateTime.MinValue)
                {
                    dueDate = invoiceSetting.Item1;
                }

                if (invoiceSetting.Item2 != null && invoiceSetting.Item2 != "")
                {
                    accountCode = invoiceSetting.Item2;
                }

                if (invoiceSetting.Item3 != null && invoiceSetting.Item3 != "")
                {
                    paymrntAccountCode = invoiceSetting.Item3;
                }

                if (invoiceSetting.Item4 != null && invoiceSetting.Item4 != 0)
                {
                    invoiceTax = invoiceSetting.Item4;
                }
                if (invoiceSetting.Item6 != null)
                {
                    isTaxInclusive = invoiceSetting.Item6;
                }

                lstJobInvoice.ToList().ForEach(c => c.cost = Math.Round(Convert.ToDecimal(c.Purchase * c.Quantity), 2));

                lstJobInvoice.ToList().ForEach(c => c.Profit = Math.Round(Convert.ToDecimal((c.Sale * c.Quantity) - (c.Purchase * c.Quantity)), 2));

                //lstJobInvoice.ToList().ForEach(c => c.SubTotal = Math.Round(Convert.ToDecimal(c.Sale * c.Quantity), 2));

                lstJobInvoice.Where(c => c.JobInvoiceType != 3).ToList()
                    .ForEach(c => c.Total = Math.Round(Convert.ToDecimal(c.Sale * c.Quantity), 2));

                lstJobInvoice.Where(c => c.JobInvoiceType == 3).ToList()
                    .ForEach(c => c.Total = Math.Round(Convert.ToDecimal(c.PaymentAmount), 2));

                //lstJobInvoice.Where(c => c.JobInvoiceType != 3 && c.Total != 0 && c.Profit != 0).ToList()
                //    .ForEach(c => c.Margin = Math.Round(Convert.ToDecimal(100 * c.Profit) / c.Total, 2));
                lstJobInvoice.Where(c => c.JobInvoiceType != 3 && c.Total != 0 && c.Profit != 0).ToList()
                    .ForEach(c => c.Margin = Math.Round(Convert.ToDecimal((100 * c.Profit) / c.Total), 2));
                //.ForEach(c => c.Margin = Math.Round(Convert.ToDecimal((100 * c.Profit) / (c.Total)), 2));

                //if (isTaxInclusive)
                //{
                //    lstJobInvoice.Where(c => c.JobInvoiceType != 3).ToList()
                //    .ForEach(c => c.Tax = Math.Round(Convert.ToDecimal(c.Total - ((c.Total * 100) / (100 + c.Tax))), 2));
                //}
                //else
                //{
                //    lstJobInvoice.Where(c => c.JobInvoiceType != 3).ToList()
                //    .ForEach(c => c.Tax = Math.Round(Convert.ToDecimal(c.Total * c.Tax / 100 ), 2));
                //}
                

                lstJobInvoice.Where(c => c.JobInvoiceType == 3).ToList()
                    .ForEach(c => c.Tax = 0);

                lstJobInvoice.Where(c => c.IsTaxInclusive == false).ToList()
                   .ForEach(c => c.TaxAmountConsider = c.Tax);

                lstJobInvoice.Where(c => c.JobInvoiceType != 3 && c.IsTaxInclusive == false).ToList()
                    .ForEach(c => c.Total = Math.Round(Convert.ToDecimal(c.Total + c.Tax), 2));

                lstJobInvoice.Where(c => c.JobInvoiceType == 3).ToList()
                    .ForEach(c => c.Payments = Math.Round(Convert.ToDecimal(c.PaymentAmount), 2));

                lstJobInvoice.Where(c => c.JobInvoiceType != 3).ToList()
                    .ForEach(c => c.Remaning = Math.Round(Convert.ToDecimal(c.Total), 2));
                lstJobInvoice.Where(c => c.JobInvoiceType == 3).ToList()
                   .ForEach(c => c.Remaning = 0);

                lstJobInvoice.Where(c => c.JobInvoiceType == 3).ToList()
                    .ForEach(c => c.IsBillableImage = "invoice-billable-true-big.png");

                lstJobInvoice.Where(c => c.JobInvoiceType != 3 && c.IsBillable == false).ToList()
                    .ForEach(c => c.IsBillableImage = "invoice-billable-false.png");

                lstJobInvoice.Where(c => c.JobInvoiceType != 3 && c.IsBillable == true).ToList()
                    .ForEach(c => c.IsBillableImage = "invoice-billable-true.png");

                 lstJobInvoice.Where(c => c.IsTaxInclusive == true).ToList().ForEach(c => c.SubTotal =  Math.Round(Convert.ToDecimal(c.Total - c.Tax),2));
                lstJobInvoice.Where(c => c.IsTaxInclusive == false).ToList().ForEach(c => c.SubTotal = Math.Round(Convert.ToDecimal(c.Sale * c.Quantity), 2));

            }
            return lstJobInvoice.ToList();
        }

        /// <summary>
        /// Gets the invoice setting.
        /// </summary>
        /// <param name="SolarCompanyId">The solar company identifier.</param>
        /// <param name="userTypeId">The user type identifier.</param>
        /// <param name="resellerId">The reseller identifier.</param>
        /// <returns>Tuple</returns>
        public Tuple<DateTime, string, string, decimal, string, bool, decimal> GetInvoiceSetting(int? SolarCompanyId = null, int userTypeId = 0, int? resellerId = 0, string itemCode = "")
        {
            try
            {
                string accountCode = string.Empty;
                string paymentCode = string.Empty, invoiceTaxType = string.Empty;
                decimal invoiceTax = 0;
                decimal chargeTax = 0;
                bool isTaxInclusive = false;
                decimal? taxRate = 0;
                DateTime dueDate = DateTime.Now;
                SettingsBAL settingsBAL = new BAL.Service.SettingsBAL();
                var setting = settingsBAL.GetChargesPartsPaymentCodeAndSettings(ProjectSession.LoggedInUserId, userTypeId != 0 ? userTypeId : ProjectSession.UserTypeId, SolarCompanyId != null ? SolarCompanyId : ProjectSession.SolarCompanyId, resellerId != null ? resellerId : ProjectSession.ResellerId);
                if (setting != null)
                {
                    if (setting.IsXeroAccount == true)
                    {
                        if (setting.lstXeroPartsCodeId.Any())
                        {
                            if (!string.IsNullOrEmpty(itemCode) && setting.lstXeroPartsCodeId.Any(s => s.Code == itemCode))
                            {
                                var matchedAccountCodeRecord = setting.lstXeroPartsCodeId.Where(c => c.Code == itemCode).FirstOrDefault();
                                if (matchedAccountCodeRecord != null)
                                {
                                    accountCode = Convert.ToString(matchedAccountCodeRecord.Code);
                                    invoiceTax = matchedAccountCodeRecord.Tax;
                                    invoiceTaxType = matchedAccountCodeRecord.TaxType;
                                }
                            }
                            else
                            {
                                var matchedAccountCodeRecord = setting.lstXeroPartsCodeId.Where(c => c.Id == setting.XeroPartsCodeId).FirstOrDefault();
                                if (matchedAccountCodeRecord != null)
                                {
                                    accountCode = Convert.ToString(matchedAccountCodeRecord.Code);
                                    invoiceTax = matchedAccountCodeRecord.Tax;
                                    invoiceTaxType = matchedAccountCodeRecord.TaxType;
                                }
                            }

                            #region Old Code
                            //accountCode = Convert.ToString(setting.lstXeroPartsCodeId.Where(c => c.Id == setting.XeroPartsCodeId).FirstOrDefault().Code);
                            //invoiceTax = setting.lstXeroPartsCodeId.Where(c => c.Id == setting.XeroPartsCodeId).FirstOrDefault().Tax;
                            //invoiceTaxType = setting.lstXeroPartsCodeId.Where(c => c.Id == setting.XeroPartsCodeId).FirstOrDefault().TaxType;
                            #endregion
                        }
                        if (setting.lstXeroPaymentsCodeId.Any())
                        {
                            var matchedPaymentCodeRecord = setting.lstXeroPaymentsCodeId.FirstOrDefault(c => c.Id == setting.XeroPaymentsCodeId);
                            if (matchedPaymentCodeRecord != null)
                                paymentCode = Convert.ToString(matchedPaymentCodeRecord.Code);
                        }

                        if (setting.lstXeroChargesCodeId.Any())
                        {
                            var matchedPaymentCodeRecord = setting.lstXeroChargesCodeId.Where(c => c.Id == setting.XeroChargeCodeId).FirstOrDefault();
                            if (matchedPaymentCodeRecord != null)
                                chargeTax = matchedPaymentCodeRecord.Tax;
                        }

                    }
                    else
                    {
                        accountCode = Convert.ToString(setting.PartCode);
                        invoiceTax = setting.PartTax;
                        paymentCode = Convert.ToString(setting.PaymentCode);
                        chargeTax = setting.ChargeTax;
                    }

                    int InvoiceDueDateId = setting.InvoiceDueDateId;
                    dueDate = FormBot.Helper.Helper.Common.SettingDueDate(InvoiceDueDateId);
                    isTaxInclusive = setting.IsTaxInclusive;
                    taxRate = setting.TaxRate;
                }
                return new Tuple<DateTime, string, string, decimal, string, bool, decimal>(dueDate, accountCode, paymentCode, invoiceTax, invoiceTaxType, isTaxInclusive, chargeTax);
            }

            catch (Exception err)
            {
                return new Tuple<DateTime, string, string, decimal, string, bool, decimal>(DateTime.Now, "200", "881", 0, "", false, 0);
            }
        }

        /// <summary>
        /// Jobs the visit data.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userTypeId">The user type identifier.</param>
        /// <param name="jobId">The job identifier.</param>
        /// <returns>
        /// DataSet
        /// </returns>
        public DataSet JobVisitData(int userId, int userTypeId, int jobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.BigInt, userId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.BigInt, userTypeId));
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.BigInt, jobId));
            DataSet dsJobVisit = CommonDAL.ExecuteDataSet("JobScheduling_JobVisit", sqlParameters.ToArray());
            return dsJobVisit;
        }

        /// <summary>
        /// Gets the job parts by identifier.
        /// </summary>
        /// <param name="jobInvoiceDetailID">The job invoice detail identifier.</param>
        /// <returns>
        /// DataSet
        /// </returns>
        public DataSet GetJobPartsById(int jobInvoiceDetailID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobInvoiceDetailID", SqlDbType.BigInt, jobInvoiceDetailID));
            DataSet dsJobParts = CommonDAL.ExecuteDataSet("JobInvoiceDetail_GetJobPartsById", sqlParameters.ToArray());
            return dsJobParts;
        }

        /// <summary>
        /// Inserts the job invoice detail.
        /// </summary>
        /// <param name="jobInvoiceDetailID">The job invoice detail identifier.</param>
        /// <param name="jobInvoiceID">The job invoice identifier.</param>
        /// <param name="jobPartID">The job part identifier.</param>
        /// <param name="isBillable">if set to <c>true</c> [is billable].</param>
        /// <param name="jobScheduleID">The job schedule identifier.</param>
        /// <param name="staff">The staff.</param>
        /// <param name="sale">The sale.</param>
        /// <param name="timeStart">The time start.</param>
        /// <param name="itemCode">The item code.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="purchase">The purchase.</param>
        /// <param name="margin">The margin.</param>
        /// <param name="description">The description.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="timeEnd">The time end.</param>
        /// <param name="paymentType">Type of the payment.</param>
        /// <param name="PaymentAmount">The payment amount.</param>
        /// <param name="jobInvoiceType">Type of the job invoice.</param>
        /// <param name="isInvoiced">if set to <c>true</c> [is invoiced].</param>
        /// <param name="createdBy">The created by.</param>
        /// <param name="modifiedBy">The modified by.</param>
        /// <param name="isDeleted">if set to <c>true</c> [is deleted].</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <param name="isPart">if set to <c>true</c> [is part].</param>
        /// <param name="invoiceNumber">The invoice number.</param>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="OwnerUsername">The owner username.</param>
        /// <param name="SendTo">The send to.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>
        /// DataSet
        /// </returns>
        public DataSet InsertJobInvoiceDetail(int jobInvoiceDetailID, int? jobInvoiceID, int? jobPartID, bool isBillable, int jobScheduleID, int staff, decimal? sale, DateTime timeStart, string itemCode, decimal? quantity, decimal? purchase, decimal? margin, string description, string fileName, DateTime? timeEnd, int? paymentType, decimal? PaymentAmount, int jobInvoiceType, bool isInvoiced, int? createdBy, int? modifiedBy, bool isDeleted, int? solarCompanyId, bool isPart, string invoiceNumber, int jobId, string OwnerUsername = null, int? SendTo = null, FormBot.Entity.Settings.Settings settings = null, int? taxRateId = null, string saleAccountCode = null)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobInvoiceDetailID", SqlDbType.BigInt, jobInvoiceDetailID));
            sqlParameters.Add(DBClient.AddParameters("JobInvoiceID", SqlDbType.BigInt, jobInvoiceID));
            sqlParameters.Add(DBClient.AddParameters("JobPartID", SqlDbType.BigInt, jobPartID));
            sqlParameters.Add(DBClient.AddParameters("IsBillable", SqlDbType.Bit, isBillable));
            sqlParameters.Add(DBClient.AddParameters("JobScheduleID", SqlDbType.BigInt, jobScheduleID));
            sqlParameters.Add(DBClient.AddParameters("Staff", SqlDbType.BigInt, staff));
            sqlParameters.Add(DBClient.AddParameters("Sale", SqlDbType.Decimal, sale));
            sqlParameters.Add(DBClient.AddParameters("TimeStart", SqlDbType.DateTime, timeStart));
            sqlParameters.Add(DBClient.AddParameters("ItemCode", SqlDbType.NVarChar, itemCode));
            sqlParameters.Add(DBClient.AddParameters("Quantity", SqlDbType.Decimal, quantity));
            sqlParameters.Add(DBClient.AddParameters("Purchase", SqlDbType.Decimal, purchase));
            sqlParameters.Add(DBClient.AddParameters("Margin", SqlDbType.Decimal, margin));
            sqlParameters.Add(DBClient.AddParameters("Description", SqlDbType.NVarChar, description));
            sqlParameters.Add(DBClient.AddParameters("FileName", SqlDbType.NVarChar, fileName));
            sqlParameters.Add(DBClient.AddParameters("TimeEnd", SqlDbType.DateTime, timeEnd));
            sqlParameters.Add(DBClient.AddParameters("PaymentType", SqlDbType.TinyInt, paymentType));
            sqlParameters.Add(DBClient.AddParameters("PaymentAmount", SqlDbType.Decimal, PaymentAmount));
            sqlParameters.Add(DBClient.AddParameters("JobInvoiceType", SqlDbType.TinyInt, jobInvoiceType));
            sqlParameters.Add(DBClient.AddParameters("IsInvoiced", SqlDbType.Bit, isInvoiced));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.BigInt, createdBy));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.BigInt, modifiedBy));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("IsDeleted", SqlDbType.Bit, isDeleted));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.BigInt, solarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("IsPart", SqlDbType.Bit, isPart));
            sqlParameters.Add(DBClient.AddParameters("invoiceNumber", SqlDbType.NVarChar, invoiceNumber));
            sqlParameters.Add(DBClient.AddParameters("jobId", SqlDbType.BigInt, jobId));
            sqlParameters.Add(DBClient.AddParameters("OwnerUsername", SqlDbType.NVarChar, OwnerUsername));
            sqlParameters.Add(DBClient.AddParameters("SendTo", SqlDbType.Int, SendTo));
            sqlParameters.Add(DBClient.AddParameters("IsTaxInclusive", SqlDbType.Bit, settings != null ? settings.IsTaxInclusive : false));
            sqlParameters.Add(DBClient.AddParameters("TaxRateId", SqlDbType.Int, taxRateId));
            sqlParameters.Add(DBClient.AddParameters("SaleAccountCode", SqlDbType.NVarChar, saleAccountCode));

            var invoiceSetting = GetInvoiceSetting(null, 0, 0, itemCode);
            decimal tax = 0;
            if (jobInvoiceType == Convert.ToByte(SystemEnums.JobInvoiceType.Part))
            {
                tax = invoiceSetting != null && invoiceSetting.Item4 != null && invoiceSetting.Item4 != 0 ? invoiceSetting.Item4 : 0;
            }
            else if (jobInvoiceType == Convert.ToByte(SystemEnums.JobInvoiceType.Time))
            {
                tax = invoiceSetting != null && invoiceSetting.Item7 != null && invoiceSetting.Item7 != 0 ? invoiceSetting.Item7 : 0;
            }
            else
            {
                tax = 0;
            }

            sqlParameters.Add(DBClient.AddParameters("Tax", SqlDbType.Decimal, tax));
            sqlParameters.Add(DBClient.AddParameters("TaxType", SqlDbType.NVarChar, invoiceSetting != null && invoiceSetting.Item5 != null && invoiceSetting.Item5 != "" ? invoiceSetting.Item5 : ""));
            DataSet dsJobInvoice = CommonDAL.ExecuteDataSet("JobInvoiceDetail_InsertJobInvoiceDetail", sqlParameters.ToArray());
            return dsJobInvoice;
        }

        /// <summary>
        /// Deletes the invoice detail.
        /// </summary>
        /// <param name="ids">The ids.</param>
        public void DeleteInvoiceDetail(string ids)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ids", SqlDbType.NVarChar, ids));
            CommonDAL.Crud("Job_DeleteJobInvoiceDetailByID", sqlParameters.ToArray());
        }

        /// <summary>
        /// Gets the invoice detsil for report.
        /// </summary>
        /// <param name="jobInvoiceID">The job invoice identifier.</param>
        /// <param name="IsJobAddress">if set to <c>true</c> [is job address].</param>
        /// <param name="IsJobDate">if set to <c>true</c> [is job date].</param>
        /// <param name="IsJobDescription">if set to <c>true</c> [is job description].</param>
        /// <param name="IsTitle">if set to <c>true</c> [is title].</param>
        /// <param name="IsName">if set to <c>true</c> [is name].</param>
        /// <returns>
        /// DataSet
        /// </returns>
        public DataSet GetInvoiceDetsilForReport(int jobInvoiceID, bool IsJobAddress, bool IsJobDate, bool IsJobDescription, bool IsTitle, bool IsName)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobInvoiceID", SqlDbType.Int, jobInvoiceID));
            sqlParameters.Add(DBClient.AddParameters("IsJobAddress", SqlDbType.Bit, IsJobAddress));
            sqlParameters.Add(DBClient.AddParameters("IsJobDate", SqlDbType.Bit, IsJobDate));
            sqlParameters.Add(DBClient.AddParameters("IsJobDescription", SqlDbType.Bit, IsJobDescription));
            sqlParameters.Add(DBClient.AddParameters("IsTitle", SqlDbType.Bit, IsTitle));
            sqlParameters.Add(DBClient.AddParameters("IsName", SqlDbType.Bit, IsName));

            DataSet ds = CommonDAL.ExecuteDataSet("GetInvoiceDetsilForReport", sqlParameters.ToArray());
            return ds;
        }

        /// <summary>
        /// Gets the invoice all detail for CSV.
        /// </summary>
        /// <param name="jobInvoiceID">The job invoice identifier.</param>
        /// <param name="IsTaxInclusive">if set to <c>true</c> [is tax inclusive].</param>
        /// <param name="DueDate">The due date.</param>
        /// <returns>
        /// DataSet
        /// </returns>
        public DataSet GetInvoiceAllDetailForCSV(int jobInvoiceID, bool IsTaxInclusive, string DueDate)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobInvoiceID", SqlDbType.Int, jobInvoiceID));
            sqlParameters.Add(DBClient.AddParameters("IsTaxInclusive", SqlDbType.Bit, IsTaxInclusive));
            sqlParameters.Add(DBClient.AddParameters("DueDate", SqlDbType.NVarChar, DueDate));
            DataSet ds = CommonDAL.ExecuteDataSet("CSV_GetInvoiceAllDetailForCSV", sqlParameters.ToArray());
            return ds;
        }

    }
}

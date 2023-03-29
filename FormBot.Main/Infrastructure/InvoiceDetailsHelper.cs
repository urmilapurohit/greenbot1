using FormBot.Entity.Job;
using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Mvc;
using FormBot.BAL.Service.Job;
using FormBot.BAL;
using System.Linq;

namespace FormBot.Main.Infrastructure
{
    public class InvoiceDetailsHelper : Controller
    {
        #region Properties

        private readonly IJobInvoiceDetailBAL _jobInvoiceDetail;
        #endregion

        #region Constructor
        public InvoiceDetailsHelper(IJobInvoiceDetailBAL jobInvoicedetail)
        {
            this._jobInvoiceDetail = jobInvoicedetail;
        }

        #endregion

        /// <summary>
        /// Gets the time part.
        /// </summary>
        /// <param name="jobInvoiceDetailID">The job invoice detail identifier.</param>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="lstList">The LST list.</param>
        /// <returns>invoice object</returns>
        public JobInvoiceDetail GetTimePart(string jobInvoiceDetailID, string jobId, ref List<SelectListItem> lstList)
        {
            JobInvoiceDetail jobInvoiceDetail = new JobInvoiceDetail();

            DataSet dataset = _jobInvoiceDetail.JobVisitData(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, Convert.ToInt32(jobId));

            if (!string.IsNullOrEmpty(jobInvoiceDetailID))
            {
                DataSet dsJobTime = _jobInvoiceDetail.GetJobPartsById(Convert.ToInt32(jobInvoiceDetailID));

                if (dsJobTime != null && dsJobTime.Tables.Count > 0 && dsJobTime.Tables[0] != null && dsJobTime.Tables[0].Rows.Count > 0)
                {
                    jobInvoiceDetail.JobInvoiceDetailID = Convert.ToInt32(dsJobTime.Tables[0].Rows[0]["JobInvoiceDetailID"]);
                    jobInvoiceDetail.JobInvoiceID = !string.IsNullOrEmpty(Convert.ToString(dsJobTime.Tables[0].Rows[0]["JobInvoiceID"])) ? Convert.ToInt32(dsJobTime.Tables[0].Rows[0]["JobInvoiceID"]) : 0;
                    jobInvoiceDetail.JobPartID = !string.IsNullOrEmpty(Convert.ToString(dsJobTime.Tables[0].Rows[0]["JobPartID"])) ? Convert.ToInt32(dsJobTime.Tables[0].Rows[0]["JobPartID"]) : 0;
                    jobInvoiceDetail.JobScheduleID = Convert.ToInt32(dsJobTime.Tables[0].Rows[0]["JobScheduleID"]);
                    if (!string.IsNullOrEmpty(Convert.ToString(dsJobTime.Tables[0].Rows[0]["Sale"])))
                    {
                        jobInvoiceDetail.Sale = Convert.ToDecimal(dsJobTime.Tables[0].Rows[0]["Sale"]);
                    }
                    else
                    {
                        jobInvoiceDetail.Sale = null;
                    }

                    if (!string.IsNullOrEmpty(Convert.ToString(dsJobTime.Tables[0].Rows[0]["Purchase"])))
                    {
                        jobInvoiceDetail.Purchase = Convert.ToDecimal(dsJobTime.Tables[0].Rows[0]["Purchase"]);
                    }
                    else
                    {
                        jobInvoiceDetail.Purchase = null;
                    }

                    jobInvoiceDetail.InvoiceStartDate = Convert.ToString(Convert.ToDateTime(dsJobTime.Tables[0].Rows[0]["TimeStart"]).ToString("yyyy/MM/dd"));
                    jobInvoiceDetail.InvoiceEndDate = Convert.ToString(Convert.ToDateTime(dsJobTime.Tables[0].Rows[0]["TimeEnd"]).ToString("yyyy/MM/dd"));
                    jobInvoiceDetail.InvoiceStartTime = Convert.ToString(Convert.ToDateTime(dsJobTime.Tables[0].Rows[0]["TimeStart"]).ToString("HH:mm"));
                    jobInvoiceDetail.InvoiceEndTime = Convert.ToString(Convert.ToDateTime(dsJobTime.Tables[0].Rows[0]["TimeEnd"]).ToString("HH:mm"));
                    jobInvoiceDetail.TimeAdded = Convert.ToString(Convert.ToDateTime(dsJobTime.Tables[0].Rows[0]["TimeStart"]).ToShortTimeString());
                    jobInvoiceDetail.ItemCode = !string.IsNullOrEmpty(Convert.ToString(dsJobTime.Tables[0].Rows[0]["ItemCode"])) ? Convert.ToString(dsJobTime.Tables[0].Rows[0]["ItemCode"]) : "";
                    jobInvoiceDetail.Quantity = !string.IsNullOrEmpty(Convert.ToString(dsJobTime.Tables[0].Rows[0]["Quantity"])) ? Convert.ToDecimal(dsJobTime.Tables[0].Rows[0]["Quantity"]) : 0;
                    jobInvoiceDetail.Margin = !string.IsNullOrEmpty(Convert.ToString(dsJobTime.Tables[0].Rows[0]["Margin"])) ? Convert.ToDecimal(dsJobTime.Tables[0].Rows[0]["Margin"]) : 0;
                    jobInvoiceDetail.Description = Convert.ToString(dsJobTime.Tables[0].Rows[0]["Description"]);
                    jobInvoiceDetail.FileName = !string.IsNullOrEmpty(Convert.ToString(dsJobTime.Tables[0].Rows[0]["FileName"])) ? Convert.ToString(dsJobTime.Tables[0].Rows[0]["FileName"]) : "";
                    jobInvoiceDetail.IsBillable = Convert.ToBoolean(dsJobTime.Tables[0].Rows[0]["IsBillable"]);
                    jobInvoiceDetail.MimeType = MimeMapping.GetMimeMapping(jobInvoiceDetail.FileName).ToLower().StartsWith("image") ? "image" : "";
                    jobInvoiceDetail.FullFileName = jobInvoiceDetail.JobInvoiceDetailID + "_" + jobInvoiceDetail.FileName;
                }
                else
                {
                    jobInvoiceDetail.IsBillable = true;
                    jobInvoiceDetail.TimeAdded = Convert.ToString(DateTime.Now.ToShortTimeString());
                    jobInvoiceDetail.Sale = null;
                    jobInvoiceDetail.Purchase = null;
                    jobInvoiceDetail.Quantity = 1;
                    jobInvoiceDetail.Margin = 0;
                }

            }
            else
            {
                jobInvoiceDetail.IsBillable = true;
                jobInvoiceDetail.TimeAdded = Convert.ToString(DateTime.Now.ToShortTimeString());
                jobInvoiceDetail.Sale = null;
                jobInvoiceDetail.Purchase = null;
                jobInvoiceDetail.Quantity = 1;
                jobInvoiceDetail.Margin = 0;
            }

            jobInvoiceDetail.OldFileName = jobInvoiceDetail.FileName;

            jobInvoiceDetail.JobId = Convert.ToInt32(jobId);

            if (dataset.Tables.Count > 0)
            {
                foreach (DataRow item in dataset.Tables[0].Rows)
                {
                    SelectListItem selectedList = new SelectListItem()
                    {
                        Value = item.ItemArray[0].ToString(),
                        Text = item.ItemArray[1].ToString()
                    };

                    lstList.Add(selectedList);
                }
            }

            return jobInvoiceDetail;
        }

        /// <summary>
        /// Gets the job payment.
        /// </summary>
        /// <param name="jobInvoiceId">The job invoice identifier.</param>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="InvoiceNumber">The invoice number.</param>
        /// <param name="JobInvoiceDetailId">The job invoice detail identifier.</param>
        /// <returns>invoice object</returns>
        public JobInvoiceDetail GetJobPayment(int jobInvoiceId, string jobId, string InvoiceNumber, string JobInvoiceDetailId)
        {
            JobInvoiceDetail jobInvoiceDetail = new JobInvoiceDetail();
            jobInvoiceDetail.Guid = Guid.NewGuid().ToString();
            jobInvoiceDetail.JobInvoiceID = jobInvoiceId;
            jobInvoiceDetail.InvoiceNumber = InvoiceNumber;
            if (!string.IsNullOrEmpty(JobInvoiceDetailId))
            {
                List<int> df = new List<int>();
                var dsjobPayment = _jobInvoiceDetail.GetJobPartsById(Convert.ToInt32(JobInvoiceDetailId));
                List<FormBot.Entity.Job.JobInvoiceDetail> jobpayment = DBClient.DataTableToList<FormBot.Entity.Job.JobInvoiceDetail>(dsjobPayment.Tables[0]);
                jobInvoiceDetail = jobpayment.FirstOrDefault();
                jobInvoiceDetail.DateAdded = jobInvoiceDetail.TimeStart.ToString("yyyy/MM/dd");
                jobInvoiceDetail.TimeAdded = jobInvoiceDetail.TimeStart.ToString("HH:mm");
                jobInvoiceDetail.MimeType = MimeMapping.GetMimeMapping(jobInvoiceDetail.FileName).ToLower().StartsWith("image") ? "image" : "";
                jobInvoiceDetail.FullFileName = jobInvoiceDetail.JobInvoiceDetailID + "_" + jobInvoiceDetail.FileName;
                jobInvoiceDetail.Guid = Convert.ToString(jobInvoiceDetail.JobInvoiceDetailID);
            }

            if (jobInvoiceDetail.JobInvoiceDetailID == 0)
            {
                jobInvoiceDetail.TimeAdded = Convert.ToString(DateTime.Now.ToShortTimeString());
            }

            jobInvoiceDetail.OldFileName = jobInvoiceDetail.FileName;
            return jobInvoiceDetail;
        }

        public JobInvoiceDetail GetJobPart(string jobInvoiceDetailID, string jobId)
        {
            JobInvoiceDetail jobInvoiceDetail = new JobInvoiceDetail();
            int invoiceDetailID = !string.IsNullOrEmpty(jobInvoiceDetailID) ? Convert.ToInt32(jobInvoiceDetailID) : 0;
            DataSet dsJobVisit = _jobInvoiceDetail.JobVisitData(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, Convert.ToInt32(jobId));
            if (dsJobVisit != null && dsJobVisit.Tables.Count > 0 && dsJobVisit.Tables[0] != null)
            {
                var itemsJobScheduling = dsJobVisit.Tables[0].ToListof<JobScheduling>();
                jobInvoiceDetail.JobVisit = itemsJobScheduling;
            }
            else
            {
                jobInvoiceDetail.JobVisit = new List<JobScheduling>();
            }
            DataSet dsJobParts = _jobInvoiceDetail.GetJobPartsById(invoiceDetailID);
            if (dsJobParts != null && dsJobParts.Tables.Count > 0 && dsJobParts.Tables[0] != null && dsJobParts.Tables[0].Rows.Count > 0)
            {
                jobInvoiceDetail.JobInvoiceDetailID = Convert.ToInt32(dsJobParts.Tables[0].Rows[0]["JobInvoiceDetailID"]);
                jobInvoiceDetail.JobInvoiceID = !string.IsNullOrEmpty(Convert.ToString(dsJobParts.Tables[0].Rows[0]["JobInvoiceID"])) ? Convert.ToInt32(dsJobParts.Tables[0].Rows[0]["JobInvoiceID"]) : 0;
                jobInvoiceDetail.JobPartID = !string.IsNullOrEmpty(Convert.ToString(dsJobParts.Tables[0].Rows[0]["JobPartID"])) ? Convert.ToInt32(dsJobParts.Tables[0].Rows[0]["JobPartID"]) : 0;
                jobInvoiceDetail.JobScheduleID = Convert.ToInt32(dsJobParts.Tables[0].Rows[0]["JobScheduleID"]);
                jobInvoiceDetail.Sale = !string.IsNullOrEmpty(Convert.ToString(dsJobParts.Tables[0].Rows[0]["Sale"])) ? Convert.ToDecimal(dsJobParts.Tables[0].Rows[0]["Sale"]) : 0;
                jobInvoiceDetail.Purchase = !string.IsNullOrEmpty(Convert.ToString(dsJobParts.Tables[0].Rows[0]["Purchase"])) ? Convert.ToDecimal(dsJobParts.Tables[0].Rows[0]["Purchase"]) : 0;
                jobInvoiceDetail.DateAdded = Convert.ToString(Convert.ToDateTime(dsJobParts.Tables[0].Rows[0]["TimeStart"]).ToString("yyyy/MM/dd"));
                jobInvoiceDetail.TimeAdded = Convert.ToString(Convert.ToDateTime(dsJobParts.Tables[0].Rows[0]["TimeStart"]).ToShortTimeString());
                jobInvoiceDetail.ItemCode = !string.IsNullOrEmpty(Convert.ToString(dsJobParts.Tables[0].Rows[0]["ItemCode"])) ? Convert.ToString(dsJobParts.Tables[0].Rows[0]["ItemCode"]) : "";
                jobInvoiceDetail.Quantity = !string.IsNullOrEmpty(Convert.ToString(dsJobParts.Tables[0].Rows[0]["Quantity"])) ? Convert.ToDecimal(dsJobParts.Tables[0].Rows[0]["Quantity"]) : 0;
                jobInvoiceDetail.Margin = !string.IsNullOrEmpty(Convert.ToString(dsJobParts.Tables[0].Rows[0]["Margin"])) ? Convert.ToDecimal(dsJobParts.Tables[0].Rows[0]["Margin"]) : 0;
                jobInvoiceDetail.Description = Convert.ToString(dsJobParts.Tables[0].Rows[0]["Description"]);
                jobInvoiceDetail.FileName = !string.IsNullOrEmpty(Convert.ToString(dsJobParts.Tables[0].Rows[0]["FileName"])) ? Convert.ToString(dsJobParts.Tables[0].Rows[0]["FileName"]) : "";
                jobInvoiceDetail.IsBillable = Convert.ToBoolean(dsJobParts.Tables[0].Rows[0]["IsBillable"]);
                jobInvoiceDetail.MimeType = MimeMapping.GetMimeMapping(jobInvoiceDetail.FileName).ToLower().StartsWith("image") ? "image" : "";
                jobInvoiceDetail.FullFileName = jobInvoiceDetail.JobInvoiceDetailID + "_" + jobInvoiceDetail.FileName;
                jobInvoiceDetail.Guid = Convert.ToString(jobInvoiceDetail.JobInvoiceDetailID);
                jobInvoiceDetail.Tax = !string.IsNullOrEmpty(Convert.ToString(dsJobParts.Tables[0].Rows[0]["Tax"])) ? Convert.ToDecimal(dsJobParts.Tables[0].Rows[0]["Tax"]) : 0;
                jobInvoiceDetail.IsTaxInclusive = Convert.ToBoolean(dsJobParts.Tables[0].Rows[0]["isTaxInclusive"]);
                jobInvoiceDetail.TaxRateId = !string.IsNullOrEmpty(Convert.ToString(dsJobParts.Tables[0].Rows[0]["TaxRateId"])) ? Convert.ToInt32(dsJobParts.Tables[0].Rows[0]["TaxRateId"]) : 1;
                jobInvoiceDetail.SaleAccountCode = !string.IsNullOrEmpty(Convert.ToString(dsJobParts.Tables[0].Rows[0]["SaleAccountCode"])) ? Convert.ToString(dsJobParts.Tables[0].Rows[0]["SaleAccountCode"]) : ProjectSession.SaleAccountCode;
            }
            else
            {
                jobInvoiceDetail.IsBillable = true;
                jobInvoiceDetail.TimeAdded = Convert.ToString(DateTime.Now.ToShortTimeString());
                jobInvoiceDetail.Sale = 0;
                jobInvoiceDetail.Purchase = 0;
                jobInvoiceDetail.Quantity = 1;
                jobInvoiceDetail.Margin = 0;
                jobInvoiceDetail.Guid = Guid.NewGuid().ToString();
                jobInvoiceDetail.TaxRateId = 1;
            }

            jobInvoiceDetail.StaffName = Convert.ToString(ProjectSession.LoggedInName);
            jobInvoiceDetail.OldFileName = jobInvoiceDetail.FileName;
            return jobInvoiceDetail;

        }
    }
}
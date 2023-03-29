using FormBot.BAL.Service.Job;
using FormBot.Entity;
using FormBot.Entity.Job;
using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FormBot.BAL;
using FormBot.BAL.Service;

namespace FormBot.Main.Controllers
{
    public class AllJobInvoiceController : Controller
    {
        #region Properties

        private readonly IJobInvoiceBAL _jobInvoice;
        private readonly IJobInvoiceDetailBAL _jobInvoiceDetail;
        #endregion

        #region Constructor
        public AllJobInvoiceController(IJobInvoiceBAL jobInvoice, IJobInvoiceDetailBAL jobInvoicedetail)
        {
            this._jobInvoice = jobInvoice;
            this._jobInvoiceDetail = jobInvoicedetail;
        }

        #endregion

        /// <summary>
        /// Gets the random string.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns>random string</returns>
        [NonAction]
        public static string GetRandomString(int length)
        {
            var numArray = new byte[length];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(numArray);
            return SanitiseBase64String(Convert.ToBase64String(numArray), length);
        }

        // GET: /AllJobInvoice/
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// View All Job Invoices
        /// </summary>
        /// <returns>Action Result</returns>
        [UserAuthorization]
        [HttpGet]
        public ActionResult ViewAllJobInvoices()
        {
            try
            {
                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get Job invoice detail partial view.
        /// </summary>
        /// <param name="invoiceID">Invoice ID</param>
        /// <param name="jobId">Job Id</param>
        /// <returns>Action Result</returns>
        public ActionResult GetJobInvoiceDetail(string invoiceID = "", int jobId = 0)
        {
            GetJobInvoiceDetailsList(invoiceID);
            BindInvoiceTo(jobId);

            JobInvoice jobInvoice = GetInvoice(Convert.ToInt32(invoiceID));

            bool IsFileExists = System.IO.File.Exists(System.IO.Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + "JobDocuments" + "\\" + jobId + "\\" + "Invoice\\Report\\" + jobInvoice.InvoiceNumber + ".pdf")) ? true : false;
            if (IsFileExists)
            {
                ViewBag.PDFFound = true;
                ViewBag.Filename = jobInvoice.InvoiceNumber + ".pdf";
            }   

            Entity.Settings.Settings setting = GetSettingsData();
            if (setting != null)
            {
                ViewBag.isXero = setting.IsXeroAccount;
            }
            else
            {
                ViewBag.isXero = false;
            }

            if (string.IsNullOrEmpty(jobInvoice.InvoiceNumber))
            {
                jobInvoice.InvoiceNumber = string.Format("INV-{0}", GetRandomString(4));
            }

            return View("_JobInvoiceDetail", jobInvoice);
        }

        /// <summary>
        /// Gets the invoice.
        /// </summary>
        /// <param name="invoiceID">The invoice identifier.</param>
        /// <returns>Get Job Invoice.</returns>
        [NonAction]
        public JobInvoice GetInvoice(int invoiceID = 0)
        {
            JobInvoice jobInvoice = new JobInvoice();
            try
            {
                if (invoiceID != 0)
                    jobInvoice = _jobInvoice.GetJobInvoiceById(Convert.ToInt32(invoiceID));
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return jobInvoice;
        }

        /// <summary>
        /// Binds the invoice to.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        [NonAction]
        public void BindInvoiceTo(int jobId = 0)
        {
            try
            {
                if (ProjectSession.UserTypeId == 4)
                {
                    IList<JobOwnerDetails> lstJobOwnerDetails =
                        _jobInvoice.GetInvoiceSendToDetails<JobOwnerDetails>(ProjectSession.LoggedInUserId,
                        ProjectSession.UserTypeId, jobId);
                    if (lstJobOwnerDetails.Count > 0)
                    {
                        ViewBag.UserList = new SelectList(lstJobOwnerDetails.ToList(), "FirstName", "Fullname");
                        ViewBag.UserType = "2";
                        ViewBag.InvoiceToUserId = lstJobOwnerDetails.FirstOrDefault().JobOwnerID;
                        //ViewBag.InvoiceToUserDetail = lstJobOwnerDetails.FirstOrDefault().CompanyName;
                        ViewBag.InvoiceToUserDetail = lstJobOwnerDetails.FirstOrDefault().OwnerAddress;
                        ViewBag.InvoiceToEmailId = lstJobOwnerDetails.FirstOrDefault().Email;
                    }
                }
                else if (ProjectSession.UserTypeId == 6 ||
                            ProjectSession.UserTypeId == 7 ||
                            ProjectSession.UserTypeId == 9)
                {
                    IList<User> lstUser = _jobInvoice.GetInvoiceSendToDetails<User>(ProjectSession.LoggedInUserId,
                        ProjectSession.UserTypeId, jobId);
                    if (lstUser.Count > 0)
                    {
                        ViewBag.UserList = new SelectList(lstUser.ToList(), "UserId", "Fullname");
                        ViewBag.UserType = "1";
                        ViewBag.InvoiceToUserId = lstUser.FirstOrDefault().UserId;
                        ViewBag.InvoiceToUserDetail = lstUser.First().CompanyName;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// Gets the job invoice details list.
        /// </summary>
        /// <param name="invoiceID">The invoice identifier.</param>
        /// <param name="jobInvoiceType">Type of the job invoice.</param>
        /// <param name="isInvoiced">The is invoiced.</param>
        /// <param name="sortCol">The sort col.</param>
        /// <param name="sortDir">The sort dir.</param>
        /// <returns>Job Invoice Detail</returns>
        public IList<JobInvoiceDetail> GetJobInvoiceDetailsList(string invoiceID = "", int jobInvoiceType = 0,
          int isInvoiced = 0, string sortCol = "", string sortDir = "")
        {
            IList<JobInvoiceDetail> lstJobInvoiceDetail =
                _jobInvoiceDetail.GetJobInvoiceDetail(invoiceID, jobInvoiceType, isInvoiced,
                                    sortCol, sortDir);
            try
            {
                if (lstJobInvoiceDetail.Count > 0)
                {
                    ViewBag.SubTotal = lstJobInvoiceDetail.Where(c => c.JobInvoiceType != 3 && c.IsBillable == true).Sum(c => c.SubTotal);
                    ViewBag.Cost = lstJobInvoiceDetail.Where(c => c.JobInvoiceType != 3 && c.IsBillable == true).Sum(c => c.cost);
                    ViewBag.Profit = lstJobInvoiceDetail.Where(c => c.JobInvoiceType != 3 && c.IsBillable == true).Sum(c => c.Profit);

                    if (ViewBag.SubTotal != 0 && ViewBag.Profit != 0)
                        ViewBag.Margin = Math.Round(Convert.ToDecimal((100 * Convert.ToDecimal(ViewBag.Profit)) / Convert.ToDecimal(ViewBag.SubTotal)), MidpointRounding.AwayFromZero);
                    else
                        ViewBag.Margin = 0;

                    ViewBag.Tax = lstJobInvoiceDetail.Where(c => c.JobInvoiceType != 3 && c.IsBillable == true).Sum(c => c.Tax);

                    decimal totalWithOutTax = Convert.ToDecimal(lstJobInvoiceDetail.Where(c => c.JobInvoiceType != 3 &&
                        c.IsBillable == true && c.IsTaxInclusive == false).Sum(c => c.SubTotal + c.Tax));
                    decimal totalWithTax = Convert.ToDecimal(lstJobInvoiceDetail.Where(c => c.JobInvoiceType != 3 &&
                        c.IsBillable == true && c.IsTaxInclusive == true).Sum(c => c.SubTotal));
                    ViewBag.SummaryTotal = Math.Round(Convert.ToDecimal(totalWithOutTax + totalWithTax), 2);

                    ViewBag.Payments = lstJobInvoiceDetail.Where(c => c.JobInvoiceType == 3).Sum(c => c.PaymentAmount);

                    ViewBag.Ramaning = Math.Round(Convert.ToDecimal(ViewBag.SummaryTotal - ViewBag.Payments), 2);

                    if (lstJobInvoiceDetail.Any(c => c.IsInvoiced == true))
                    {
                        ViewBag.PDFFound = true;
                    }
                }
            }
            catch (Exception ex)
            {
                FormBot.Helper.Log.WriteError(ex);
            }

            TempData["lstJobInvoiceDetail"] = lstJobInvoiceDetail;
            return lstJobInvoiceDetail;
        }

        /// <summary>
        /// Get Invoice List for all Jobs
        /// </summary>
        /// <param name="InvoiceNumber">Invoice Number</param>
        /// <param name="InvoiceDate">Invoice Date</param>
        /// <param name="Status">Invoice Status</param>
        /// <param name="OwnerName">Owner Name</param>
        public void GetAllJobInvoiceList(string InvoiceNumber, DateTime? InvoiceDate, string Status, string OwnerName)
        {
            //int jobID = Convert.ToInt32(JobID);
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);
            try
            {
                List<JobInvoice> lstJobInvoice = _jobInvoice.GetAllJobInvoice(pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, ProjectSession.LoggedInUserId, InvoiceNumber, InvoiceDate, Convert.ToInt32(Status), OwnerName);
                if (gridParam.SortCol == "InvoiceTotal" || gridParam.SortCol == "InvoiceAmountDue")
                {
                    if (gridParam.SortCol == "InvoiceTotal")
                    {
                        if (gridParam.SortDir.ToLower() == "desc")
                            lstJobInvoice = lstJobInvoice.OrderByDescending(s => s.InvoiceTotal).ToList();
                        else
                            lstJobInvoice = lstJobInvoice.OrderBy(s => s.InvoiceTotal).ToList();
                    }

                    if (gridParam.SortCol == "InvoiceAmountDue")
                    {
                        if (gridParam.SortDir.ToLower() == "desc")
                            lstJobInvoice = lstJobInvoice.OrderByDescending(s => s.InvoiceAmountDue).ToList();
                        else
                            lstJobInvoice = lstJobInvoice.OrderBy(s => s.InvoiceAmountDue).ToList();
                    }

                }

                if (lstJobInvoice.Count > 0)
                {
                    gridParam.TotalDisplayRecords = lstJobInvoice.FirstOrDefault().TotalRecords;
                    gridParam.TotalRecords = lstJobInvoice.FirstOrDefault().TotalRecords;
                }

                HttpContext.Response.Write(Grid.PrepareDataSet(lstJobInvoice, gridParam));
            }
            catch (Exception ex)
            {
                FormBot.Helper.Log.WriteError(ex);
                HttpContext.Response.Write(Grid.PrepareDataSet(new List<JobInvoice>(), gridParam));
            }
        }

        /// <summary>
        /// Gets the job time by identifier.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="jobInvoiceDetailID">The job invoice detail identifier.</param>
        /// <returns>Action Result</returns>
        [HttpGet]
        public ActionResult GetJobTimeById(string jobId, string jobInvoiceDetailID)
        {
            JobInvoiceDetail jobInvoiceDetail = new JobInvoiceDetail();
            List<SelectListItem> lstList = new List<SelectListItem>();
            FormBot.Main.Infrastructure.InvoiceDetailsHelper objInvoiceDetails = new Infrastructure.InvoiceDetailsHelper(new JobInvoiceDetailBAL());
            jobInvoiceDetail = objInvoiceDetails.GetTimePart(jobInvoiceDetailID, jobId, ref lstList);
            ViewBag.JobVisit = lstList;
            jobInvoiceDetail.Guid = Guid.NewGuid().ToString();
            ViewBag.Guid = Guid.NewGuid().ToString();
            return PartialView(@"~\Views\JobInvoiceDetail\_AddTime.cshtml", jobInvoiceDetail);
        }

        /// <summary>
        /// Gets the time part.
        /// </summary>
        /// <param name="jobInvoiceDetailID">The job invoice detail identifier.</param>
        /// <param name="jobId">The job identifier.</param>
        /// <returns></returns>
        //public JobInvoiceDetail GetTimePart(string jobInvoiceDetailID, string jobId)
        //{
        //    JobInvoiceDetail jobInvoiceDetail = new JobInvoiceDetail();
        //    jobInvoiceDetail.Guid = Guid.NewGuid().ToString();
        //    ViewBag.Guid = Guid.NewGuid().ToString();

        //    List<SelectListItem> lstList = new List<SelectListItem>();
        //    DataSet dataset = _jobInvoiceDetail.JobVisitData(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, Convert.ToInt32(jobId));

        //    if (!string.IsNullOrEmpty(jobInvoiceDetailID))
        //    {
        //        DataSet dsJobTime = _jobInvoiceDetail.GetJobPartsById(Convert.ToInt32(jobInvoiceDetailID));

        //        if (dsJobTime != null && dsJobTime.Tables.Count > 0 && dsJobTime.Tables[0] != null && dsJobTime.Tables[0].Rows.Count > 0)
        //        {
        //            jobInvoiceDetail.JobInvoiceDetailID = Convert.ToInt32(dsJobTime.Tables[0].Rows[0]["JobInvoiceDetailID"]);
        //            jobInvoiceDetail.JobInvoiceID = !string.IsNullOrEmpty(Convert.ToString(dsJobTime.Tables[0].Rows[0]["JobInvoiceID"])) ? Convert.ToInt32(dsJobTime.Tables[0].Rows[0]["JobInvoiceID"]) : 0;
        //            jobInvoiceDetail.JobPartID = !string.IsNullOrEmpty(Convert.ToString(dsJobTime.Tables[0].Rows[0]["JobPartID"])) ? Convert.ToInt32(dsJobTime.Tables[0].Rows[0]["JobPartID"]) : 0;
        //            jobInvoiceDetail.JobScheduleID = Convert.ToInt32(dsJobTime.Tables[0].Rows[0]["JobScheduleID"]);
        //            if (!string.IsNullOrEmpty(Convert.ToString(dsJobTime.Tables[0].Rows[0]["Sale"])))
        //            {
        //                jobInvoiceDetail.Sale = Convert.ToDecimal(dsJobTime.Tables[0].Rows[0]["Sale"]);
        //            }
        //            else
        //            {
        //                jobInvoiceDetail.Sale = null;
        //            }

        //            if (!string.IsNullOrEmpty(Convert.ToString(dsJobTime.Tables[0].Rows[0]["Purchase"])))
        //            {
        //                jobInvoiceDetail.Purchase = Convert.ToDecimal(dsJobTime.Tables[0].Rows[0]["Purchase"]);
        //            }
        //            else
        //            {
        //                jobInvoiceDetail.Purchase = null;
        //            }

        //            //jobInvoiceDetail.Sale = !string.IsNullOrEmpty(Convert.ToString(dsJobTime.Tables[0].Rows[0]["Sale"])) ? Convert.ToDecimal(dsJobTime.Tables[0].Rows[0]["Sale"]) : null;
        //            //jobInvoiceDetail.Purchase = !string.IsNullOrEmpty(Convert.ToString(dsJobTime.Tables[0].Rows[0]["Purchase"])) ? Convert.ToDecimal(dsJobTime.Tables[0].Rows[0]["Purchase"]) : null;

        //            jobInvoiceDetail.InvoiceStartDate = Convert.ToString(Convert.ToDateTime(dsJobTime.Tables[0].Rows[0]["TimeStart"]).ToString("yyyy/MM/dd"));
        //            jobInvoiceDetail.InvoiceEndDate = Convert.ToString(Convert.ToDateTime(dsJobTime.Tables[0].Rows[0]["TimeEnd"]).ToString("yyyy/MM/dd"));
        //            jobInvoiceDetail.InvoiceStartTime = Convert.ToString(Convert.ToDateTime(dsJobTime.Tables[0].Rows[0]["TimeStart"]).ToString("HH:mm"));
        //            jobInvoiceDetail.InvoiceEndTime = Convert.ToString(Convert.ToDateTime(dsJobTime.Tables[0].Rows[0]["TimeEnd"]).ToString("HH:mm"));

        //            jobInvoiceDetail.TimeAdded = Convert.ToString(Convert.ToDateTime(dsJobTime.Tables[0].Rows[0]["TimeStart"]).ToShortTimeString());
        //            jobInvoiceDetail.ItemCode = !string.IsNullOrEmpty(Convert.ToString(dsJobTime.Tables[0].Rows[0]["ItemCode"])) ? Convert.ToString(dsJobTime.Tables[0].Rows[0]["ItemCode"]) : "";
        //            jobInvoiceDetail.Quantity = !string.IsNullOrEmpty(Convert.ToString(dsJobTime.Tables[0].Rows[0]["Quantity"])) ? Convert.ToDecimal(dsJobTime.Tables[0].Rows[0]["Quantity"]) : 0;
        //            jobInvoiceDetail.Margin = !string.IsNullOrEmpty(Convert.ToString(dsJobTime.Tables[0].Rows[0]["Margin"])) ? Convert.ToDecimal(dsJobTime.Tables[0].Rows[0]["Margin"]) : 0;
        //            jobInvoiceDetail.Description = Convert.ToString(dsJobTime.Tables[0].Rows[0]["Description"]);
        //            jobInvoiceDetail.FileName = !string.IsNullOrEmpty(Convert.ToString(dsJobTime.Tables[0].Rows[0]["FileName"])) ? Convert.ToString(dsJobTime.Tables[0].Rows[0]["FileName"]) : "";
        //            jobInvoiceDetail.IsBillable = Convert.ToBoolean(dsJobTime.Tables[0].Rows[0]["IsBillable"]);
        //            jobInvoiceDetail.MimeType = MimeMapping.GetMimeMapping(jobInvoiceDetail.FileName).ToLower().StartsWith("image") ? "image" : "";
        //            jobInvoiceDetail.FullFileName = jobInvoiceDetail.JobInvoiceDetailID + "_" + jobInvoiceDetail.FileName;
        //        }
        //        else
        //        {
        //            jobInvoiceDetail.IsBillable = true;
        //            jobInvoiceDetail.TimeAdded = Convert.ToString(DateTime.Now.ToShortTimeString());
        //            jobInvoiceDetail.Sale = null;
        //            jobInvoiceDetail.Purchase = null;
        //            jobInvoiceDetail.Quantity = 1;
        //            jobInvoiceDetail.Margin = 0;
        //        }

        //    }
        //    else
        //    {
        //        jobInvoiceDetail.IsBillable = true;
        //        jobInvoiceDetail.TimeAdded = Convert.ToString(DateTime.Now.ToShortTimeString());
        //        jobInvoiceDetail.Sale = null;
        //        jobInvoiceDetail.Purchase = null;
        //        jobInvoiceDetail.Quantity = 1;
        //        jobInvoiceDetail.Margin = 0;
        //    }

        //    jobInvoiceDetail.OldFileName = jobInvoiceDetail.FileName;

        //    jobInvoiceDetail.JobId = Convert.ToInt32(jobId);

        //    if (dataset.Tables.Count > 0)
        //    {
        //        foreach (DataRow item in dataset.Tables[0].Rows)
        //        {
        //            SelectListItem selectedList = new SelectListItem()
        //            {
        //                Value = item.ItemArray[0].ToString(),
        //                Text = item.ItemArray[1].ToString()
        //            };

        //            lstList.Add(selectedList);
        //        }
        //    }

        //    ViewBag.JobVisit = lstList;
        //    return jobInvoiceDetail;
        //}

        /// <summary>
        /// Gets the job parts by identifier.
        /// </summary>
        /// <param name="jobInvoiceDetailID">The job invoice detail identifier.</param>
        /// <param name="jobId">The job identifier.</param>
        /// <returns>Action Result</returns>
        [HttpGet]
        public ActionResult GetJobPartsById(string jobInvoiceDetailID, string jobId)
        {
            JobInvoiceDetail jobInvoiceDetail = new JobInvoiceDetail();
            FormBot.Main.Infrastructure.InvoiceDetailsHelper objInvoiceDetails = new Infrastructure.InvoiceDetailsHelper(new JobInvoiceDetailBAL());
            jobInvoiceDetail = objInvoiceDetails.GetJobPart(jobInvoiceDetailID, jobId);
            return PartialView(@"~\Views\JobInvoiceDetail\_AddEditJobPart.cshtml", jobInvoiceDetail);
        }

        /// <summary>
        /// Gets the job payment by identifier.
        /// </summary>
        /// <param name="jobInvoiceId">The job invoice identifier.</param>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="InvoiceNumber">The invoice number.</param>
        /// <param name="JobInvoiceDetailId">The job invoice detail identifier.</param>
        /// <returns>Action Result</returns>
        [HttpGet]
        public ActionResult GetJobPaymentById(int jobInvoiceId, string jobId, string InvoiceNumber, string JobInvoiceDetailId)
        {
            JobInvoiceDetail jobInvoiceDetail = new JobInvoiceDetail();
            FormBot.Main.Infrastructure.InvoiceDetailsHelper objInvoiceDetails = new Infrastructure.InvoiceDetailsHelper(new JobInvoiceDetailBAL());
            jobInvoiceDetail = objInvoiceDetails.GetJobPayment(jobInvoiceId, jobId, InvoiceNumber, JobInvoiceDetailId);

            return PartialView(@"~\Views\JobInvoiceDetail\_AddPayment.cshtml", jobInvoiceDetail);
        }

        public Entity.Settings.Settings GetSettingsData()
        {
            SettingsBAL settingsBAL = new SettingsBAL();
            Entity.Settings.Settings settings = new Entity.Settings.Settings();
            int? solarCompanyId = 0;

            if (ProjectSession.UserTypeId == 7 || ProjectSession.UserTypeId == 9)
                solarCompanyId = null;
            else
                solarCompanyId = ProjectSession.SolarCompanyId;

            settings = settingsBAL.GetChargesPartsPaymentCodeAndSettings(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, solarCompanyId, ProjectSession.ResellerId);
            return settings;
        }

        /// <summary>
        /// Sanitises the base64 string.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <returns>Sanitise base 64 string.</returns>
        [NonAction]
        private static string SanitiseBase64String(string input, int maxLength)
        {
            input = input.Replace("-", "");
            input = input.Replace("=", "");
            input = input.Replace("/", "");
            input = input.Replace("+", "");
            input = input.Replace(" ", "");
            while (input.Length < maxLength)
                input = input + GetRandomString(maxLength);
            return input.Length <= maxLength ?
                input.ToUpper() :
                input.ToUpper().Substring(0, maxLength);
        }
    }
}
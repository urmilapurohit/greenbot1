using FileHelpers.Options;
using FormBot.BAL;
using FormBot.BAL.Service;
using FormBot.BAL.Service.CommonRules;
using FormBot.Entity;
using FormBot.Entity.Job;
using FormBot.Entity.KendoGrid;
using FormBot.Helper;
using FormBot.Main.Helpers;
using GenericParsing;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Xero.Api.Example.Applications.Public;
using Xero.Api.Infrastructure.Exceptions;
using Xero.NetStandard.OAuth2.Api;
using Xero.NetStandard.OAuth2.Model;
using oauthModel = Xero.NetStandard.OAuth2.Model;

namespace FormBot.Main.Controllers
{
    public class SAASInvoiceController : Controller
    {
        #region Properties
        private readonly ISAASInvoiceBAL _saasInvoiceBAL;
        private readonly ICreateJobHistoryBAL _createJobHistoryBAL;
        private readonly ICreateJobBAL _createJobBAL;
        private readonly IUserBAL _user;
        private readonly IResellerBAL _reseller;
        private readonly ICreateJobHistoryBAL _jobHistory;
        #endregion

        #region Constructor

        public SAASInvoiceController(ISAASInvoiceBAL saasInvoiceBAL, ICreateJobHistoryBAL createJobHistoryBAL, ICreateJobBAL createJobBAL, IUserBAL user, IResellerBAL reseller, ICreateJobHistoryBAL jobHistory)
        {
            this._saasInvoiceBAL = saasInvoiceBAL;
            this._createJobHistoryBAL = createJobHistoryBAL;
            this._createJobBAL = createJobBAL;
            this._user = user;
            this._reseller = reseller;
            this._jobHistory = jobHistory;
        }
        #endregion

        // GET: SAASInvoice
        [UserAuthorization]
        public ActionResult Index()
        {
            UserWiseGridConfiguration objUserWiseGridConfiguration = new UserWiseGridConfiguration()
            {
                IsKendoGrid = true,
                PageSize = 10,
                UserId = ProjectSession.LoggedInUserId,
                ViewPageId = SystemEnums.ViewPageId.SAASInvoiceView.GetHashCode(),
                UserWiseGridConfigurationId = 0
            };
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            ViewBag.IsKendo = true;
            UserWiseGridConfiguration sessionObjUserWiseGridConfiguration = null;
            List<UserWiseGridConfiguration> lstUserWiseGridConfigurations = new List<UserWiseGridConfiguration>();
            var index = -1;
            try
            {
                lstUserWiseGridConfigurations = (List<UserWiseGridConfiguration>)ProjectSession.UserWiseGridConfiguration;
                sessionObjUserWiseGridConfiguration = lstUserWiseGridConfigurations.Where(m => m.ViewPageId == SystemEnums.ViewPageId.SAASInvoiceView.GetHashCode()).FirstOrDefault();
                index = lstUserWiseGridConfigurations.IndexOf(sessionObjUserWiseGridConfiguration);
            }
            catch (Exception ex) { }

            objUserWiseGridConfiguration.IsKendoGrid = true;
            objUserWiseGridConfiguration.PageSize = sessionObjUserWiseGridConfiguration == null ? 10 : sessionObjUserWiseGridConfiguration.PageSize;
            objUserWiseGridConfiguration.UserWiseGridConfigurationId = sessionObjUserWiseGridConfiguration == null ? 0 : sessionObjUserWiseGridConfiguration.UserWiseGridConfigurationId;
            objUserWiseGridConfiguration.UserWiseGridConfigurationId = _user.InsertUpdateUserWiseGridConfiguration(objUserWiseGridConfiguration);
            sessionObjUserWiseGridConfiguration = objUserWiseGridConfiguration;

            if (index != -1)
            {
                lstUserWiseGridConfigurations[index] = sessionObjUserWiseGridConfiguration;
                ProjectSession.UserWiseGridConfiguration = lstUserWiseGridConfigurations;
            }
            else
            {
                lstUserWiseGridConfigurations.Add(sessionObjUserWiseGridConfiguration);
                ProjectSession.UserWiseGridConfiguration = lstUserWiseGridConfigurations;
            }
            SAASInvoice sAASInvoice = new SAASInvoice();
            sAASInvoice.UserTypeId = ProjectSession.UserTypeId;
            sAASInvoice.lstSettlementTerm = _saasInvoiceBAL.GetSAASSettlementTerms();
            //List<SelectListItem> lstItems = new List<SelectListItem>();
            //if (sAASInvoice.lstSettlementTerm != null)
            //{
            //    for (int i = 0; i <= sAASInvoice.lstSettlementTerm.Count - 1; i++)
            //    {
            //        lstItems.Add(new SelectListItem() { Text = sAASInvoice.lstSettlementTerm[i].SettlementTerms, Value = sAASInvoice.lstSettlementTerm[i].SettlementTermId.ToString() });
            //    }
            //}
            //ViewBag.lstSettlementTerm = new SelectList(lstItems);

            // get the previous url and store it in tempdata (It's value is used inside /Authorization/Callback method to redirect to views based on visited page)
            TempData["PreviousUrl"] = System.Web.HttpContext.Current.Request.Url;
            TempData.Keep("PreviousUrl");

            return View(sAASInvoice);
        }

        /// <summary>
        /// Gets SAAS Invoice List
        /// </summary>
        /// <param name="PageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="reseller"></param>
        /// <param name="InvoiceId"></param>
        /// <param name="createdFromDate"></param>
        /// <param name="createdToDate"></param>
        /// <param name="invoiceDueFromDate"></param>
        /// <param name="invoiceDueToDate"></param>
        /// <param name="Owner"></param>
        /// <param name="BillingPeriod"></param>
        /// <param name="JobID"></param>
        /// <param name="settlementTermId"></param>
        /// <param name="IsSent"></param>
        /// <returns>List of SAAS Invoices</returns>
        public JsonResult GetSAASInvoiceList(int PageNumber = 0, int pageSize = 10, string reseller = "", string InvoiceId = "", string createdFromDate = "", string createdToDate = "", string invoiceDueFromDate = "", string invoiceDueToDate = "", string Owner = "", string BillingPeriod = "", string JobID = "", string settlementTermId = "", bool IsSent = false, List<KendoGridSorting> sort = null, KendoGridData filter = null)
        {
            int ResellerId = 0;

            if (ProjectSession.UserTypeId == 1 || ProjectSession.UserTypeId == 3)
            {
                ResellerId = !string.IsNullOrEmpty(reseller) ? Convert.ToInt32(reseller) : 0;
            }
            else if (ProjectSession.UserTypeId == 2)
            {
                ResellerId = ProjectSession.ResellerId;
            }
            else if (ProjectSession.UserTypeId == 5)
            {
                ResellerId = ProjectSession.ResellerId;
            }

            DateTime? CreatedFromDate = null, CreatedToDate = null, InvoiceDueFromDate = null, InvoiceDueToDate = null;
            if (!string.IsNullOrEmpty(createdFromDate) && !string.IsNullOrEmpty(createdToDate))
            {
                CreatedFromDate = Convert.ToDateTime(createdFromDate);
                CreatedToDate = Convert.ToDateTime(createdToDate);
            }
            if (!string.IsNullOrEmpty(invoiceDueFromDate) && !string.IsNullOrEmpty(invoiceDueToDate))
            {
                InvoiceDueFromDate = Convert.ToDateTime(invoiceDueFromDate);
                InvoiceDueToDate = Convert.ToDateTime(invoiceDueToDate);
            }

            DataSet dsAllColumnsData = new DataSet();
            dsAllColumnsData = _saasInvoiceBAL.GetSAASInvoiceList(1, pageSize, "InvoiceId", "asc", reseller, InvoiceId, createdFromDate, createdToDate, invoiceDueFromDate, invoiceDueToDate, Owner, JobID, settlementTermId, IsSent, BillingPeriod);
            //if (CacheConfiguration.IsContainsKey(CacheConfiguration.dsSAASInvoiceData))
            //{
            //    dsAllColumnsData = CacheConfiguration.Get<DataSet>(CacheConfiguration.dsSAASInvoiceData).Copy();
            //    dsAllColumnsData.Tables[0].ToListof<SAASInvoice>().Where(x => x.InvoiceId == Convert.ToInt32(InvoiceId));
            //}
            //else
            //{
            //    dsAllColumnsData = _saasInvoiceBAL.GetSAASInvoiceList(1, pageSize, "InvoiceNumber", "asc", reseller, InvoiceId, createdFromDate, createdToDate, invoiceDueFromDate, invoiceDueToDate, Owner, JobID, settlementTermId, IsSent);
            //    CacheConfiguration.Set(CacheConfiguration.dsSAASInvoiceData, dsAllColumnsData.Copy());
            //}

            DataTable newDatatable = new DataTable();
            newDatatable = dsAllColumnsData.Tables[0];

            newDatatable = FilteringAndSortingSAASInvoiceDetails(newDatatable, filter, sort);
            int total = newDatatable.Rows.Count;
            if (total > 0)
            {
                DataTable dataTable = newDatatable.Rows.Cast<System.Data.DataRow>().Skip((PageNumber - 1) * pageSize).Take(pageSize).CopyToDataTable();
                dsAllColumnsData.Tables[0].Clear();
                dsAllColumnsData.Tables[0].Merge(dataTable);
            }
            else
            {
                dsAllColumnsData.Tables[0].Clear();
            }
            IList<dynamic> lstSAASInvoice = new List<dynamic>();
            lstSAASInvoice = DataTableExtension.ToDynamicList(dsAllColumnsData.Tables[0], "SAASInvoice");

            if (lstSAASInvoice == null || lstSAASInvoice.Count == 0)
                return Json(new { total = 0, data = lstSAASInvoice }, JsonRequestBehavior.AllowGet);
            return Json(new { total = lstSAASInvoice.FirstOrDefault().TotalRecords, data = lstSAASInvoice }, JsonRequestBehavior.AllowGet);
        }

        private DataTable FilteringAndSortingSAASInvoiceDetails(DataTable dt, KendoGridData filter, List<KendoGridSorting> sort)
        {

            string searchQuery = "";
            string query = "";
            if (filter != null && filter.Filters != null && filter.Filters.Count > 0)
            {
                query += "(";

                for (int i = 0; i < filter.Filters.Count; i++)
                {
                    if (filter.Filters[i] != null)
                    {
                        if (i > 0)
                        {
                            query += " " + filter.Logic + " ";
                        }
                        if (filter.Filters[i].Filters != null)
                        {
                            query = query + "(";
                            for (int j = 0; j < filter.Filters[i].Filters.Count; j++)
                            {
                                if (j > 0)
                                {
                                    query += " " + filter.Filters[i].Logic + " ";
                                }
                                query = FilterSubmissionValue(filter.Filters[i], query, j);
                            }
                            query = query + ")";
                            //query = query.Substring(0, query.LastIndexOf(filter.Filters[i].Logic));
                        }
                        else
                        {
                            query = FilterSubmissionValue(filter, query, i);
                        }
                    }

                }

                query = query + ")";
            }

            string sortQuery = "";
            if (sort != null && sort.Count > 0)
            {
                for (int i = 0; i < sort.Count; i++)
                {
                    if (!string.IsNullOrEmpty(sort[i].Field))
                    {
                        sortQuery = sort[i].Field + " " + sort[i].Dir;
                    }
                }
            }
            if (!string.IsNullOrEmpty(searchQuery) && !string.IsNullOrEmpty(query))
            {
                query = query + " AND " + searchQuery;
            }
            else if (string.IsNullOrEmpty(query))
            {
                query = searchQuery;
            }
            if (sortQuery.Length > 0 || query.Length > 0)
            {
                dt = dt.Select(query, sortQuery).Length > 0 ? dt.Select(query, sortQuery).CopyToDataTable() : new DataTable();
            }

            return dt;
        }

        public string FilterSubmissionValue(KendoGridData filter, string query, int i)
        {

            var op = "LIKE";
            if (filter.Filters[i].Operator != "contains")
            {
                int result = 0;
                DateTime date;
                double value = 0;
                var isInt = int.TryParse(filter.Filters[i].Value, out result);
                var isDouble = double.TryParse(filter.Filters[i].Value, out value);
                var isDate = DateTime.TryParse(filter.Filters[i].Value, out date);
                var isNull = filter.Filters[i].Value.ToLower() == "null";
                if (filter.Filters[i].Field == "SolarCompany")
                {
                    filter.Filters[i].Field = "SolarCompanyId";
                }
                if (filter.Filters[i].Field == "STCStatus")
                {
                    filter.Filters[i].Field = "StcStatusId";
                }
                if (filter.Filters[i].Field == "ComplianceOfficer")
                {
                    filter.Filters[i].Field = "ComplianceBy";
                }
                if (filter.Filters[i].Field == "AccountManager")
                {
                    filter.Filters[i].Field = "RamId";
                }
                if (isDate)
                {
                    if (filter.Filters[i].Operator == "gte")
                    {
                        op = ">=";
                        query += "(" + filter.Filters[i].Field + " " + op + " '" + filter.Filters[i].Value.Trim() + "')";
                    }
                    else if (filter.Filters[i].Operator == "lte")
                    {
                        op = "<";
                        query += "(" + filter.Filters[i].Field + " " + op + " '" + Convert.ToDateTime(filter.Filters[i].Value.Trim()).AddDays(1) + "')";
                    }

                }
                if (filter.Filters[i].Operator == "eq")
                {
                    if (isInt || isDouble)
                    {
                        op = "=";
                        query += filter.Filters[i].Field + " " + op + " " + filter.Filters[i].Value.Trim();
                    }
                    else if (isNull)
                    {
                        query += filter.Filters[i].Field + " is null";
                    }
                    else if (isDate)
                    {
                        op = ">=";
                        query += "(" + filter.Filters[i].Field + " " + op + " '" + filter.Filters[i].Value.Trim() + "'" + " AND " + filter.Filters[i].Field + " " + "<" + " '" + date.AddDays(1).ToString("dd-MM-yyyy") + "')";
                    }
                    else if (filter.Filters[i].Field.ToLower().Equals("jobstage"))
                    {
                        query += filter.Filters[i].Field + " " + op + " '" + filter.Filters[i].Value.Trim() + "'";
                    }
                    else
                    {
                        query += filter.Filters[i].Field + " " + op + " '%" + filter.Filters[i].Value.Trim() + "%'";
                    }
                }

                if (filter.Filters[i].Operator == "neq")
                {
                    if (isInt || isDouble)
                    {
                        op = "<>";
                        query += filter.Filters[i].Field + " " + op + " " + filter.Filters[i].Value.Trim();
                    }
                    else if (isDate)
                    {
                        op = "<";
                        query += "(" + filter.Filters[i].Field + " " + op + " '" + filter.Filters[i].Value.Trim() + "'" + " OR " + filter.Filters[i].Field + " " + ">" + " '" + date.AddDays(1).ToString("dd-MM-yyyy") + "')";
                    }
                    else
                    {
                        op = "NOT LIKE";
                        query += filter.Filters[i].Field + " " + op + " '%" + filter.Filters[i].Value.Trim() + "%'";
                    }

                }

            }
            else
            {
                query += filter.Filters[i].Field + " " + op + " '%" + filter.Filters[i].Value.Trim() + "%'";
            }


            return query;

        }

        /// <summary>
        /// Get Invoice Details for an Invoice
        /// </summary>
        /// <param name="InvoiceId"></param>
        /// <returns></returns>
        /*public JsonResult GetSAASInvoiceDetail(int InvoiceId)
        {
            IList<SAASInvoiceDetail> lstSAASInvoiceDetail = new List<SAASInvoiceDetail>();
            lstSAASInvoiceDetail = _saasInvoiceBAL.GetSAASInvoiceDetail(InvoiceId);
            return Json(new { total = 10, data = lstSAASInvoiceDetail }, JsonRequestBehavior.AllowGet);
        }*/

        /// <summary>
        /// Get Invoice Details for an Invoice
        /// </summary>
        /// <param name="InvoiceId"></param>
        /// <returns></returns>
        public JsonResult GetSAASInvoiceDetail(string strJobID, string IsInvoiced, string InvoiceId)
        {
            IList<SAASInvoiceDetail> lstSAASInvoiceDetail = new List<SAASInvoiceDetail>();
            lstSAASInvoiceDetail = _saasInvoiceBAL.GetSAASInvoiceDetail(strJobID, IsInvoiced, InvoiceId);
            return Json(new { total = 10, data = lstSAASInvoiceDetail }, JsonRequestBehavior.AllowGet);
        }

        ///// <summary>
        ///// Generates the CSV for selected jobs.
        ///// </summary>
        ///// <param name="jobs">The jobs.</param>
        ///// <returns></returns>
        //[HttpGet]
        //public ActionResult GenerateCSVForSelectedJobs(string jobs)
        //{
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(jobs))
        //        {
        //            CreateCSV(jobs);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return this.Json(new { success = false, errormessage = ex.Message });
        //    }

        //    return Json("", JsonRequestBehavior.AllowGet);
        //}

        ///// <summary>
        ///// Creates the CSV.
        ///// </summary>
        ///// <param name="JobId">The job identifier.</param>
        //public void CreateCSV(String JobId)
        //{

        //    DataSet ds = _peakPay.GetPeakPayCSV(JobId);
        //    DataTable dt = new DataTable();
        //    if (ds != null && ds.Tables.Count > 0)
        //    {
        //        dt = ds.Tables[0];
        //    }
        //    string csv = string.Empty;
        //    foreach (DataColumn column in dt.Columns)
        //    {
        //        //Add the Header row for CSV file.
        //        csv += column.ColumnName + ',';
        //    }
        //    //Add new line.
        //    csv += "\r\n";
        //    foreach (DataRow row in dt.Rows)
        //    {
        //        foreach (DataColumn column in dt.Columns)
        //        {
        //            //Add the Data rows.
        //            csv += row[column.ColumnName].ToString().Replace(",", ";") + ',';
        //        }
        //        //Add new line.
        //        csv += "\r\n";
        //    }

        //    //Download the CSV file.
        //    Response.Clear();
        //    Response.ClearHeaders();
        //    Response.ClearContent();
        //    Response.BufferOutput = true;
        //    Response.ContentType = "application/CSV";
        //    Response.AddHeader("content-disposition", "attachment;filename=PeakPay.csv");
        //    Response.Charset = "";
        //    Response.Output.Write(csv);
        //    //Response.Flush();
        //    Response.End();

        //}

        /// <summary>
        /// Imports the CSV.
        /// </summary>
        /// <param name="resellerId">The reseller identifier.</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ImportCSV()
        {
            try
            {
                var postedFile = Request.Files[0];
                DataTable dtCSV = GetCSVTable();
                var textReader = new StreamReader(postedFile.InputStream);

                using (GenericParser parser = new GenericParser())
                {
                    parser.ColumnDelimiter = ','; //".ToCharArray();
                    parser.FirstRowHasHeader = true;
                    parser.FirstRowSetsExpectedColumnCount = true;
                    parser.SetDataSource(textReader);

                    if (parser.Read())
                    {
                        do
                        {
                            try
                            {
                                var row = dtCSV.NewRow();


                                row["SAASInvoiceId"] = Convert.ToInt32(parser["InvoiceNumber"].Trim());
                                row["STCJobDetailId"] = Convert.ToInt32(parser["InvoiceNumber"].Trim());


                                if (parser["InvoiceNumber"].Trim() != "" && parser["InvoiceNumber"].Trim() != null)
                                    row["SAASInvoiceNumber"] = Convert.ToInt32(parser["InvoiceNumber"].Trim());
                                else
                                    return Json(new { status = false, error = "Imported csv file has not data of STCJobDetailsID." }, JsonRequestBehavior.AllowGet);

                                if (parser["JobId"].Trim() != "" && parser["JobId"].Trim() != null)
                                    row["JobId"] = Convert.ToInt32(parser["JobId"].Trim());
                                else
                                    return Json(new { status = false, error = "Imported csv file has not data of JobID." }, JsonRequestBehavior.AllowGet);

                                if (parser["STCAmount"].Trim() != "" && parser["STCAmount"].Trim() != null)
                                {
                                    if (Convert.ToDecimal(parser["STCAmount"].Trim()) > 0)
                                        row["STCAmount"] = Convert.ToDecimal(parser["STCAmount"].Trim());
                                    else
                                        return Json(new { status = false, error = "STCAmount should be greater than 0." }, JsonRequestBehavior.AllowGet);
                                }
                                else
                                    return Json(new { status = false, error = "Imported csv file has not data of STCAmount." }, JsonRequestBehavior.AllowGet);

                                if (parser["Sent"].Trim() != "" && parser["Sent"].Trim() != null)
                                    row["IsSent"] = Convert.ToString(parser["Sent"].Trim().ToLower()) == "true" ? 1 : 0;
                                else
                                    return Json(new { status = false, error = "Imported csv file has not data of IsSent." }, JsonRequestBehavior.AllowGet);


                                if (parser["SettlementTerms"].Trim() != "" && parser["SettlementTerms"].Trim() != null)
                                    row["SettlementTerm"] = Convert.ToString(parser["SettlementTerms"].Trim());
                                else
                                    return Json(new { status = false, error = "Imported csv file has not data of SettlementTerms." }, JsonRequestBehavior.AllowGet);

                                if (parser["Status"].Trim() != "" && parser["Status"].Trim() != null)
                                    row["Status"] = Convert.ToString(parser["Status"].Trim());
                                else
                                    return Json(new { status = false, error = "Imported csv file has not data of Status." }, JsonRequestBehavior.AllowGet);


                                if (parser["InvoiceDueDate"].Trim() != "" && parser["InvoiceDueDate"].Trim() != null)
                                    row["InvoiceDueDate"] = Convert.ToDateTime(parser["InvoiceDueDate"].Trim());
                                else
                                    return Json(new { status = false, error = "Imported csv file has not data of SettlementTerms." }, JsonRequestBehavior.AllowGet);

                                if (parser["PaidAmount"].Trim() != "" && parser["PaidAmount"].Trim() != null)
                                {
                                    row["PaidAmount"] = Convert.ToDecimal(parser["STCAmount"].Trim());
                                }
                                else
                                    return Json(new { status = false, error = "Imported csv file has not data of PaidAmount." }, JsonRequestBehavior.AllowGet);

                                if (parser["TotalAmount"].Trim() != "" && parser["TotalAmount"].Trim() != null)
                                    row["TotalAmount"] = Convert.ToDecimal(parser["TotalAmount"].Trim());
                                else
                                    return Json(new { status = false, error = "Imported csv file has not data of PaidAmount." }, JsonRequestBehavior.AllowGet);

                                if (parser["Deleted"].Trim() != "" && parser["Deleted"].Trim() != null)
                                    row["IsDeleted"] = Convert.ToString(parser["Deleted"].Trim().ToLower()) == "true" ? 1 : 0;
                                else
                                    return Json(new { status = false, error = "Imported csv file has not data of IsDeleted." }, JsonRequestBehavior.AllowGet);

                                dtCSV.Rows.Add(row);

                            }
                            catch (Exception ex)
                            {
                                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
                            }

                        } while (parser.Read());
                    }
                    else
                        return Json(new { status = false, error = "Imported csv file has not data." }, JsonRequestBehavior.AllowGet);
                    string stcjobIds = string.Empty;
                    if (dtCSV.Rows.Count > 0)
                    {
                        int resellerId = 60;
                        _saasInvoiceBAL.ImportSAASCSV(dtCSV, resellerId);
                        /*foreach (DataRow dr in dtCSV.Rows)
                        {
                            stcjobIds = stcjobIds + (dr["STCJobDetailsID"]).ToString() + ",";

                            // CommonBAL.SetCacheDataForSTCSubmission(Convert.ToInt32(dr["STCJobDetailsID"]), 0);
                            JobHistory jobHistory = new JobHistory();
                            jobHistory.Name = ProjectSession.LoggedInName;
                            jobHistory.FunctionalityName = "Import PeakPay CSV";
                            jobHistory.JobID = Convert.ToInt32(dr["JobID"]);
                            bool isHistorySaved = _createJobHistoryBAL.LogJobHistory(jobHistory, HistoryCategory.ModifiedIsGst);
                        }
                        stcjobIds = stcjobIds.Remove(stcjobIds.Length - 1);
                        DataTable dt = _createJobBAL.GetSTCDetailsAndJobDataForCache(stcjobIds, null);
                        if (dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                string isGst = dt.Rows[i]["IsGst"].ToString();
                                string stcPrice = dt.Rows[i]["STCPrice"].ToString();
                                string InvoiceStatus = dt.Rows[i]["InvoiceStatus"].ToString();
                                SortedList<string, string> data = new SortedList<string, string>();
                                data.Add("IsGst", isGst);
                                data.Add("STCPrice", stcPrice);
                                data.Add("InvoiceStatus", InvoiceStatus);

                                CommonBAL.SetCacheDataForSTCSubmission(Convert.ToInt32(dt.Rows[i]["STCJobDetailsID"]), null, data);
                            }
                        }*/

                        return Json(new { status = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { status = false, error = "Imported csv file has not data." }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception e)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Gets the CSV table.
        /// </summary>
        /// <returns></returns>
        public static DataTable GetCSVTable()
        {
            DataTable dtCSVData = new DataTable();
            dtCSVData.Columns.Add("SAASInvoiceId", typeof(Int32));
            dtCSVData.Columns.Add("SAASInvoiceNumber", typeof(String));
            dtCSVData.Columns.Add("CreatedDate", typeof(DateTime));
            dtCSVData.Columns.Add("CreatedBy", typeof(Int32));
            dtCSVData.Columns.Add("InvoiceDueDate", typeof(DateTime));
            dtCSVData.Columns.Add("SettlementTerm", typeof(string));
            dtCSVData.Columns.Add("TotalAmount", typeof(decimal));
            dtCSVData.Columns.Add("PaidAmount", typeof(decimal));
            dtCSVData.Columns.Add("Status", typeof(string));
            dtCSVData.Columns.Add("IsSent", typeof(bool));
            dtCSVData.Columns.Add("XeroInvoiceId", typeof(string));
            dtCSVData.Columns.Add("IsDeleted", typeof(bool));
            dtCSVData.Columns.Add("JobId", typeof(Int32));
            dtCSVData.Columns.Add("STCJobDetailId", typeof(Int32));
            dtCSVData.Columns.Add("STCAmount", typeof(decimal));

            return dtCSVData;
        }

        /// <summary>
        /// Get All STC Invoice
        /// </summary>
        /// <param name="stageid"></param>
        /// <param name="resellerId"></param>
        /// <param name="ramid"></param>
        /// <param name="solarcompanyid"></param>
        /// <param name="invoicenumber"></param>
        /// <param name="RefJobId"></param>
        /// <param name="ownername"></param>
        /// <param name="installationaddress"></param>
        /// <param name="submissionfromdate"></param>
        /// <param name="submissiontodate"></param>
        /// <param name="settlementfromdate"></param>
        /// <param name="settlementtodate"></param>
        /// <param name="isstcinvoice"></param>
        /// <param name="iscreditnotes"></param>
        /// <param name="issentinvoice"></param>
        /// <param name="isunsentinvoice"></param>
        /// <param name="SettlementTermId"></param>

        public ActionResult ExportCSV(string reseller = "", string InvoiceId = "", string createdFromDate = "", string createdToDate = "", string invoiceDueFromDate = "", string invoiceDueToDate = "", string Owner = "", string BillingPeriod = "", string JobID = "", string settlementTermId = "", bool IsSent = false, List<KendoGridSorting> sort = null, string filter = null)
        {
            DateTime? CreatedFromDate = null, CreatedToDate = null, InvoiceDueFromDate = null, InvoiceDueToDate = null;
            if (!string.IsNullOrEmpty(createdFromDate) && !string.IsNullOrEmpty(createdToDate))
            {
                CreatedFromDate = Convert.ToDateTime(createdFromDate);
                CreatedToDate = Convert.ToDateTime(createdToDate);
            }
            if (!string.IsNullOrEmpty(invoiceDueFromDate) && !string.IsNullOrEmpty(invoiceDueToDate))
            {
                InvoiceDueFromDate = Convert.ToDateTime(invoiceDueFromDate);
                InvoiceDueToDate = Convert.ToDateTime(invoiceDueToDate);
            }

            int sTermId = !string.IsNullOrEmpty(settlementTermId) ? Convert.ToInt32(settlementTermId) : 0;
            DataSet ds = _saasInvoiceBAL.GetSAASInvoiceListForExport("", "", reseller, InvoiceId, createdFromDate, createdToDate, invoiceDueFromDate, invoiceDueToDate, Owner, JobID, settlementTermId, IsSent);
            DataTable newDatatable = new DataTable();
            newDatatable = ds.Tables[0];

            newDatatable = FilteringAndSortingSAASInvoiceDetails(newDatatable, Newtonsoft.Json.JsonConvert.DeserializeObject<KendoGridData>(filter), sort);
            int total = newDatatable.Rows.Count;
            if (total > 0)
            {
                GenerateCSVForSelectedInvoice(newDatatable);
                return Json(new { status = "success", message = "" }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { status = "success", message = "No records to export" }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Get All STC Invoice
        /// </summary>
        /// <param name="stageid"></param>
        /// <param name="resellerId"></param>
        /// <param name="ramid"></param>
        /// <param name="solarcompanyid"></param>
        /// <param name="invoicenumber"></param>
        /// <param name="RefJobId"></param>
        /// <param name="ownername"></param>
        /// <param name="installationaddress"></param>
        /// <param name="submissionfromdate"></param>
        /// <param name="submissiontodate"></param>
        /// <param name="settlementfromdate"></param>
        /// <param name="settlementtodate"></param>
        /// <param name="isstcinvoice"></param>
        /// <param name="iscreditnotes"></param>
        /// <param name="issentinvoice"></param>
        /// <param name="isunsentinvoice"></param>
        /// <param name="SettlementTermId"></param>

        public ActionResult ExportAllCSV(string reseller = "", string InvoiceId = "", string createdFromDate = "", string createdToDate = "", string invoiceDueFromDate = "", string invoiceDueToDate = "", string Owner = "", string BillingPeriod = "", string JobID = "", string settlementTermId = "", bool IsSent = false, List<KendoGridSorting> sort = null, string filter = null)
        {
            DateTime? CreatedFromDate = null, CreatedToDate = null, InvoiceDueFromDate = null, InvoiceDueToDate = null;
            if (!string.IsNullOrEmpty(createdFromDate) && !string.IsNullOrEmpty(createdToDate))
            {
                CreatedFromDate = Convert.ToDateTime(createdFromDate);
                CreatedToDate = Convert.ToDateTime(createdToDate);
            }
            if (!string.IsNullOrEmpty(invoiceDueFromDate) && !string.IsNullOrEmpty(invoiceDueToDate))
            {
                InvoiceDueFromDate = Convert.ToDateTime(invoiceDueFromDate);
                InvoiceDueToDate = Convert.ToDateTime(invoiceDueToDate);
            }

            int sTermId = !string.IsNullOrEmpty(settlementTermId) ? Convert.ToInt32(settlementTermId) : 0;
            DataSet ds = _saasInvoiceBAL.GetSAASInvoiceListForExportAll("", "", reseller, InvoiceId, createdFromDate, createdToDate, invoiceDueFromDate, invoiceDueToDate, Owner, JobID, settlementTermId, IsSent);
            DataTable newDatatable = new DataTable();
            newDatatable = ds.Tables[0];

            newDatatable = FilteringAndSortingSAASInvoiceDetails(newDatatable, Newtonsoft.Json.JsonConvert.DeserializeObject<KendoGridData>(filter), sort);
            int total = newDatatable.Rows.Count;
            if (total > 0)
            {
                GenerateCSVForSelectedInvoice(ds.Tables[0]);
                return Json(new { status = "success", message = "" }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { status = "success", message = "No records to export" }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Generate CSV for selected invoice
        /// </summary>
        /// <param name="resellerId"></param>
        /// <param name="jobs"></param>
        /// <returns>ActionResult</returns>
        [HttpGet]
        public ActionResult GenerateCSVForSelectedInvoice(DataTable dt)
        {
            try
            {
                Server.ClearError();
                string DueDate = string.Empty;
                //Build the CSV file data as a Comma separated string.
                string csv = string.Empty;
                FileInfo objFileInfo = new FileInfo(Path.Combine(ProjectSession.ProofDocuments + "\\" + "SAASInvoice.csv"));
                if (objFileInfo.Exists)
                {
                    objFileInfo.Delete();
                }
                CsvOptions objCsvOptions = new CsvOptions("SAASInvoice", ',', 43);
                objCsvOptions.IncludeHeaderNames = true;
                objCsvOptions.DateFormat = "dd/MM/yyyy";
                FileHelpers.CsvEngine.DataTableToCsv(dt, Path.Combine(ProjectSession.ProofDocuments + "\\" + "SAASInvoice.csv"), objCsvOptions);
                ////Download the CSV file.
                Response.Clear();
                Response.ClearHeaders();
                Response.ClearContent();
                Response.BufferOutput = true;
                Response.ContentType = "application/CSV";
                Response.AddHeader("Content-Disposition", "attachment; filename= SAASInvoice.csv");
                Response.TransmitFile(Path.Combine(ProjectSession.ProofDocuments + "\\" + "SAASInvoice.csv"));
                //Response.Flush();
                Response.End();
            }
            catch (Exception ex)
            {
                //return this.Json(new { success = false, errormessage = ex.Message });
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSelectedIdsRecord(string stcInvoiceIds, int drafts, string invoices)
        {
            // get the previous url and store it in tempdata (It's value is used inside /Authorization/Callback method to redirect to views based on visited page)
            //TempData["PreviousUrl"] = System.Web.HttpContext.Current.Request.UrlReferrer;
            //TempData.Keep("PreviousUrl");

            if (ProjectSession.LoggedInUserId == 0)
                return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);

            try
            {
                DataSet dsSTCInvoice = _saasInvoiceBAL.GetSelectdSTCInvoice(stcInvoiceIds);
                if (dsSTCInvoice != null && dsSTCInvoice.Tables.Count > 0 && dsSTCInvoice.Tables[0] != null)
                {
                    List<SAASInvoice> STCInvoiceDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SAASInvoice>>(invoices);
                    int response = SendDraftsToXero(dsSTCInvoice.Tables[0], drafts, STCInvoiceDetail);
                    if (response == 0)
                    {
                        return Json(new { status = false, error = "specified method is not supported." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        SAASInvoice sAASInvoice = new SAASInvoice();
                        for (int i = 0; i < dsSTCInvoice.Tables[0].Rows.Count; i++)
                        {
                            int quantity = 0;
                            decimal price = 0;
                            bool isGST = false;
                            quantity = !string.IsNullOrEmpty(dsSTCInvoice.Tables[0].Rows[i]["Quantity"].ToString()) ? Convert.ToInt32(dsSTCInvoice.Tables[0].Rows[i]["Quantity"]) : 0;
                            price = !string.IsNullOrEmpty(dsSTCInvoice.Tables[0].Rows[i]["Price"].ToString()) ? Convert.ToDecimal(dsSTCInvoice.Tables[0].Rows[i]["Price"]) : 0;

                            isGST = Convert.ToBoolean(dsSTCInvoice.Tables[0].Rows[i]["IsGst"]);
                            sAASInvoice.Quantity = quantity;
                            sAASInvoice.CreatedDate = DateTime.Now;
                            sAASInvoice.InvoiceNumber = dsSTCInvoice.Tables[0].Rows[i]["InvoiceNumber"].ToString();
                        }
                        return Json(new { status = true }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                    return Json(new { status = false, error = "error" }, JsonRequestBehavior.AllowGet);
            }
            catch (RenewTokenException e)
            {
                return Json(new { status = false, error = "RenewTokenException" }, JsonRequestBehavior.AllowGet);
            }
            catch (UnauthorizedException e)
            {
                return Json(new { status = false, error = "RenewTokenException" }, JsonRequestBehavior.AllowGet);
            }
            catch (RateExceededException e)
            {
                return Json(new { status = false, error = "Rate limit exceeded, Please try after 1 minute." }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationException e)
            {
                int errorCount = ((Xero.Api.Infrastructure.Exceptions.ValidationException)(e)).ValidationErrors.Count;
                string errorMsg = string.Join(", ", ((Xero.Api.Infrastructure.Exceptions.ValidationException)(e)).ValidationErrors.AsEnumerable().Select(a => a.Message));
                return Json(new { status = false, error = errorMsg }, JsonRequestBehavior.AllowGet);
            }
            catch (XeroApiException e)
            {
                return Json(new { status = false, error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { status = false, error = e.InnerException.InnerException.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public int SendDraftsToXero(DataTable dtSTCInvoice, int drafts, List<SAASInvoice> STCInvoiceDetail)
        {
            int responseUpdateXeroInvoiceId = 0;
            if (!TokenUtilities.TokenExists())
            {
                XeroConnectController xcc = new XeroConnectController();
                xcc.Connect();
                return 0;
            }
            var token = TokenUtilities.GetStoredToken();
            string accessToken = token.AccessToken;
            string xeroTenantId = token.Tenants[0].TenantId.ToString();
            var response = Task.Run(async () => await SendDraftsToXero(dtSTCInvoice, drafts, STCInvoiceDetail, accessToken, xeroTenantId));
            response.Wait();
            responseUpdateXeroInvoiceId = Convert.ToInt32(response.Result);
            return responseUpdateXeroInvoiceId;
        }

        /// <summary>
        /// Send drafts to xero
        /// </summary>
        /// <param name="dtSTCInvoice"></param>
        /// <param name="solarCompanyId"></param>
        /// <param name="drafts"></param>
        /// <param name="STCInvoiceDetail"></param>
        /// <param name="resellerId"></param>
        /// <returns>ActionResult</returns>
        public async Task<int> SendDraftsToXero(DataTable dtSTCInvoice, int drafts, List<SAASInvoice> STCInvoiceDetail, string accessToken, string xeroTenantId)
        {
            int responseUpdateXeroInvoiceId = 0;
            var api = new AccountingApi();
            string description = string.Empty;
            bool isTaxInclusive = false;
            DateTime dueDate = DateTime.Now;

            string companyName = string.Empty;
            string STCInvoiceNumAsRefXERO = string.Empty;
            string CompanyNumber = string.Empty;

            string STCInvoiceNum = string.Empty;
            decimal quantity = 0;
            decimal price = 0;
            decimal taxRate = 0;
            decimal taxAmount = 0;
            bool isGST = false;
            string refNumberOwnerNameAddress = string.Empty;
            string STCPVDCode = string.Empty;
            string AccountCode = string.Empty;

            List<KeyValuePair<int, string>> lstSTCInvoice = new List<KeyValuePair<int, string>>();

            for (int i = 0; i < dtSTCInvoice.Rows.Count; i++)
            {
                if (dtSTCInvoice != null && dtSTCInvoice.Rows.Count > 0)
                {
                    description = Convert.ToString(dtSTCInvoice.Rows[i]["description"]);
                    companyName = Convert.ToString(dtSTCInvoice.Rows[i]["CompanyName"]);
                    CompanyNumber = Convert.ToString(dtSTCInvoice.Rows[i]["CompanyNumber"]);
                    dueDate = DateTime.Now.AddDays(Convert.ToDouble(ProjectConfiguration.XeroDraftsDueDate));

                    STCInvoiceNum = Convert.ToString(dtSTCInvoice.Rows[i]["InvoiceNumber"]);
                    quantity = !string.IsNullOrEmpty(dtSTCInvoice.Rows[i]["Quantity"].ToString()) ? Convert.ToDecimal(dtSTCInvoice.Rows[i]["Quantity"]) : 0;
                    price = !string.IsNullOrEmpty(dtSTCInvoice.Rows[i]["Price"].ToString()) ? Convert.ToDecimal(dtSTCInvoice.Rows[i]["Price"]) : 0;
                    //taxRate = !string.IsNullOrEmpty(dtSTCInvoice.Rows[i]["TaxRate"].ToString()) ? Convert.ToDecimal(dtSTCInvoice.Rows[i]["TaxRate"]) : 0;
                    isGST = Convert.ToBoolean(dtSTCInvoice.Rows[i]["IsGst"]);
                    refNumberOwnerNameAddress = Convert.ToString(dtSTCInvoice.Rows[i]["description"]);

                    isTaxInclusive = isGST == true ? true : false;
                    taxAmount = taxRate != 0 ? ((quantity * price) * taxRate) / 100 : 0;
                    STCInvoiceNumAsRefXERO = STCInvoiceNum + " - " + quantity + "SAAS@" + price + (isGST ? "+GST" : "") + " - " + refNumberOwnerNameAddress + ", " + STCPVDCode;
                    AccountCode = Convert.ToString(dtSTCInvoice.Rows[i]["AccountCode"]);
                }

                List<oauthModel.LineItem> lineItems = new List<oauthModel.LineItem>();

                oauthModel.LineItem lineItem = null;

                List<oauthModel.LineItemTracking> objTracking = new List<oauthModel.LineItemTracking>();
                oauthModel.LineItemTracking objItemTrackingCategory = new oauthModel.LineItemTracking();
                objItemTrackingCategory.Name = "SAAS";
                objItemTrackingCategory.Option = Convert.ToString(dtSTCInvoice.Rows[i]["TrackingName1"]);
                objTracking.Add(objItemTrackingCategory);

                lineItem = new oauthModel.LineItem
                {
                    //mandatory
                    AccountCode = AccountCode,
                    Description = !string.IsNullOrEmpty(STCPVDCode) ? description + ", " + STCPVDCode : description,
                    UnitAmount = (float)price,
                    Quantity = (float)quantity,
                    ItemCode = "SAAS",
                    TaxAmount = (isTaxInclusive == true ? ((taxAmount) != 0 ? Convert.ToDouble(taxAmount) : 0) : 0),
                    //TaxType = "INPUT",
                    Tracking = objTracking
                };

                lineItems.Add(lineItem);
                var invoices = (List<oauthModel.Invoice>)null;
                var paymentDate = !string.IsNullOrEmpty(dtSTCInvoice.Rows[i]["createdDate"].ToString()) ? Convert.ToDateTime(dtSTCInvoice.Rows[i]["createdDate"]) : DateTime.Now.Date;
                AccountingApi accountingApi = new AccountingApi();

                IEnumerable<oauthModel.Contact> lstContactWithAccountNumber = new List<oauthModel.Contact>();

                if (!string.IsNullOrEmpty(CompanyNumber))
                {
                    var response = Task.Run(async () => await accountingApi.GetContactsAsync(accessToken, xeroTenantId, where: string.Format("AccountNumber == \"{0}\"", CompanyNumber)));
                    response.Wait();
                    lstContactWithAccountNumber = response.Result._Contacts.ToList();
                }
                if (lstContactWithAccountNumber.Any())
                {
                    if (lstContactWithAccountNumber.FirstOrDefault().Name != companyName)
                        lstContactWithAccountNumber.FirstOrDefault().Name = companyName;
                }
                oauthModel.Invoices invoicesCreate = new oauthModel.Invoices();
                List<oauthModel.Invoice> lstInv = new List<oauthModel.Invoice>();
                oauthModel.Invoice inv = new oauthModel.Invoice();
                inv.Type = oauthModel.Invoice.TypeEnum.ACCREC;
                inv.Contact = (lstContactWithAccountNumber.Any() ? lstContactWithAccountNumber.FirstOrDefault() : (new oauthModel.Contact { Name = companyName }));
                inv.Date = paymentDate;
                inv.DueDate = dueDate;
                inv.LineAmountTypes = oauthModel.LineAmountTypes.Exclusive;
                inv.InvoiceNumber = STCInvoiceNumAsRefXERO;
                inv.Reference = STCInvoiceNum;
                inv.LineItems = lineItems;
                lstInv.Add(inv);
                invoicesCreate._Invoices = lstInv;

                var invResponse = Task.Run(async () => await accountingApi.CreateInvoicesAsync(accessToken, xeroTenantId, invoicesCreate));
                invResponse.Wait();
                invoices = invResponse.Result._Invoices.ToList();

                string xeroInvoiceId = string.Empty;
                xeroInvoiceId = invoices[0].InvoiceID.ToString();
                responseUpdateXeroInvoiceId = _saasInvoiceBAL.UpdateXeroInvoiceId(Convert.ToInt32(dtSTCInvoice.Rows[i]["InvoiceID"]), xeroInvoiceId);
            }
            return responseUpdateXeroInvoiceId;
        }

        /// <summary>
        /// Get all payment detail while sync with xero
        /// </summary>
        /// <param name="resellerId"></param>
        /// <returns>ActionResult</returns>
        [HttpGet]
        public JsonResult SyncWithXero(string invoiceNumbers)
        {
            if (ProjectSession.LoggedInUserId == 0)
                return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);

            try
            {
                if (!TokenUtilities.TokenExists())
                {
                    return Json(new { status = false, error = "invalid_grant" }, JsonRequestBehavior.AllowGet);
                }
                var token = TokenUtilities.GetStoredToken();
                string accessToken = token.AccessToken;
                string xeroTenantId = token.Tenants[0].TenantId.ToString();
                int userId = ProjectSession.LoggedInUserId;
                var response = Task.Run(async () => await SyncWithXero(invoiceNumbers, accessToken, xeroTenantId, userId));
                response.Wait();
                List<Remittance> remittanceData = new List<Remittance>();
                remittanceData = response.Result;
                if (remittanceData != null && remittanceData.Count > 0)
                {
                    for (int i = 0; i < remittanceData.Count; i++)
                    {
                        //_generateStcReportBAL.GenerateRemittanceNew(remittanceData[i], resellerId);

                        var stcInvoiceId = !string.IsNullOrEmpty(Convert.ToString(remittanceData[i].STCInvoiceID)) ? Convert.ToInt32(remittanceData[i].STCInvoiceID) : 0;
                    }
                }
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (RenewTokenException e)
            {
                //Response.Write(e.Message);
                return Json(new { status = false, error = "RenewTokenException" }, JsonRequestBehavior.AllowGet);
            }
            catch (XeroApiException e)
            {
                //Response.Write(e.Message);
                int errorCount = ((Xero.Api.Infrastructure.Exceptions.ValidationException)(e)).ValidationErrors.Count;
                string errorMsg = string.Join(", ", ((Xero.Api.Infrastructure.Exceptions.ValidationException)(e)).ValidationErrors.AsEnumerable().Select(a => a.Message));

                return Json(new { status = false, error = errorMsg }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                //Response.Write(e.Message);
                return Json(new { status = false, error = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<List<Remittance>> SyncWithXero(string invoiceNumbers, string accessToken, string xeroTenantId, int userId)
        {
            try
            {
                var AccountingApi = new AccountingApi();
                //Reseller reseller = _reseller.GetResellerByResellerID(Convert.ToInt32(resellerId));

                //DateTime syncXeroDate = _user.GetSyncXeroDateBySAASInvoiceID();

                List<Xero.NetStandard.OAuth2.Model.Payment> payment = new List<Xero.NetStandard.OAuth2.Model.Payment>();

                //var result = reseller.SyncXeroDate != null ? await AccountingApi.GetPaymentsAsync(accessToken, xeroTenantId, reseller.SyncXeroDate) : await AccountingApi.GetPaymentsAsync(accessToken, xeroTenantId);
                var result = await AccountingApi.GetPaymentsAsync(accessToken, xeroTenantId);
                payment = result._Payments.ToList();

                if (payment != null)
                {
                    DataTable dtPayment = CreateXeroInvoicePaymentTable();
                    payment.ForEach(d =>
                    {
                        DataRow dr = dtPayment.NewRow();
                        dr["Amount"] = d.Amount;
                        dr["PaymentDate"] = d.Date;
                        dr["XeroInvoiceID"] = d.Invoice.InvoiceID.ToString();
                        dr["XeroPaymentID"] = d.PaymentID.ToString();
                        dr["IsDeleted"] = d.Status == Xero.NetStandard.OAuth2.Model.Payment.StatusEnum.AUTHORISED ? false : true;
                        dtPayment.Rows.Add(dr);
                    });

                    List<string> lstStatus = new List<string>();
                    lstStatus.Add("Voided");
                    lstStatus.Add("Deleted");
                    //var result1 = reseller.SyncXeroDate != null ? await AccountingApi.GetInvoicesAsync(accessToken, xeroTenantId, reseller.SyncXeroDate, statuses: lstStatus) : await AccountingApi.GetInvoicesAsync(accessToken, xeroTenantId);
                    var result1 = await AccountingApi.GetInvoicesAsync(accessToken, xeroTenantId);
                    string STCInvoiceStatusData = string.Empty;
                    var invoice = result1._Invoices.ToList();
                    if (invoice != null)
                    {
                        List<STCInvoicePayment> lstSTCInvoice = invoice.AsEnumerable().Select(row =>
                                                                new STCInvoicePayment
                                                                {
                                                                    XeroInvoiceID = row.InvoiceID.ToString()
                                                                }).ToList();
                        if (lstSTCInvoice.Count > 0)
                        {
                            STCInvoiceStatusData = string.Join(",", lstSTCInvoice.Select(a => a.XeroInvoiceID).ToList());
                        }
                    }

                    List<Remittance> remittanceData = _saasInvoiceBAL.InsertSAASInvoicePayment(dtPayment, userId, DateTime.Now, invoiceNumbers, userId, DateTime.Now, DateTime.UtcNow, STCInvoiceStatusData);

                    return remittanceData;
                }
                else
                {
                    return new List<Remittance>();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Mark UnMarkSelectedAsSentForPayment
        /// </summary>
        /// <param name="markbit"></param>
        /// <param name="saasinvoiceids"></param>
        /// <param name="xeroInvoiceIdsArray"></param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult MarkUnMarkSelectedAsSentForPayment(string markbit, string saasinvoiceids, string xeroInvoiceIdsArray)
        {
            if (ProjectSession.LoggedInUserId == 0)
                return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);

            try
            {
                bool IsInvoiced = markbit == "0" ? false : true;

                List<string> lstNotDeletedInvoices = new List<string>();
                List<SAASInvoice> lstInvoiceIds = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SAASInvoice>>(saasinvoiceids);
                List<SAASInvoice> lstXeroInvoiceIds = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SAASInvoice>>(xeroInvoiceIdsArray);
                var xeroApiHelper = new ApplicationSettingsTest();
                if (!TokenUtilities.TokenExists())
                {
                    return Json(new { status = false, error = "invalid_grant" }, JsonRequestBehavior.AllowGet);
                }
                var token = TokenUtilities.GetStoredToken();
                string accessToken = token.AccessToken;
                string xeroTenantId = token.Tenants[0].TenantId.ToString();

                AccountingApi accountingApi = new AccountingApi();
                // remove drafts from xero
                if (lstXeroInvoiceIds.Count > 0 && markbit == "0")
                {
                    if (token != null)
                        lstNotDeletedInvoices = RemoveDraftsFromXero(xeroInvoiceIdsArray);
                    else
                        return Json(new { status = false, error = "specified method is not supported." }, JsonRequestBehavior.AllowGet);
                }

                if (lstNotDeletedInvoices.Count == 0)
                {
                    var values = lstInvoiceIds.Select(x => x.InvoiceId).ToList();
                    _saasInvoiceBAL.MarkUnMarkSelectedAsSentForPayment(IsInvoiced, string.Join(",", values));

                    JobHistory jobHistory = new JobHistory();
                    jobHistory.Name = ProjectSession.LoggedInName;
                    for (int i = 0; i < lstInvoiceIds.Count; i++)
                    {
                        jobHistory.JobID = lstInvoiceIds[i].JobId;
                        jobHistory.stcInvoiceNumber = lstInvoiceIds[i].InvoiceNumber;
                        jobHistory.HistoryMessage = "";
                        if (IsInvoiced == false)
                            _jobHistory.LogJobHistory(jobHistory, HistoryCategory.UnmarkInvoice);
                        else
                            _jobHistory.LogJobHistory(jobHistory, HistoryCategory.MarkInvoice);
                    }
                    return this.Json(new { success = true });
                }
                else
                {

                    List<string> notdeletedStcInvoiceIDs = new List<string>();
                    for (int i = 0; i < lstNotDeletedInvoices.Count; i++)
                    {
                        notdeletedStcInvoiceIDs.Add(lstInvoiceIds.First(item => item.InvoiceNumber.Equals(lstNotDeletedInvoices[i])).InvoiceId.ToString());

                        JobHistory jobHistory = new JobHistory();
                        jobHistory.Name = ProjectSession.LoggedInName;

                        jobHistory.JobID = lstInvoiceIds.First(item => item.InvoiceNumber.Equals(lstNotDeletedInvoices[i])).JobId;
                        jobHistory.stcInvoiceNumber = lstInvoiceIds.First(item => item.InvoiceNumber.Equals(lstNotDeletedInvoices[i])).InvoiceNumber;
                        jobHistory.HistoryMessage = "which is not successfull because of invoice found on xero to be voided, paid or approved";
                        _jobHistory.LogJobHistory(jobHistory, HistoryCategory.UnmarkInvoice);
                        lstInvoiceIds.Remove(lstInvoiceIds.First(item => item.InvoiceNumber.Equals(lstNotDeletedInvoices[i])));

                    }


                    if (lstInvoiceIds.Count > 0)
                    {
                        var values = lstInvoiceIds.Select(x => x.InvoiceId).ToList();
                        _saasInvoiceBAL.MarkUnMarkSelectedAsSentForPayment(IsInvoiced, string.Join(",", values));
                        JobHistory jobHistory = new JobHistory();
                        jobHistory.Name = ProjectSession.LoggedInName;
                        for (int i = 0; i < lstInvoiceIds.Count; i++)
                        {
                            jobHistory.JobID = lstInvoiceIds[i].JobId;
                            jobHistory.stcInvoiceNumber = lstInvoiceIds[i].InvoiceNumber;
                            jobHistory.HistoryMessage = "";
                            _jobHistory.LogJobHistory(jobHistory, HistoryCategory.UnmarkInvoice);
                        }
                    }

                    string strNotDeletedInvoices = string.Join(", ", lstNotDeletedInvoices.ToArray());
                    string strnotdeletedStcInvoiceIDs = string.Join(",", notdeletedStcInvoiceIDs.ToArray());
                    return this.Json(new { success = true, InvoiceNum = strNotDeletedInvoices, count = lstNotDeletedInvoices.Count, notdeletedStcInvoiceIDs = strnotdeletedStcInvoiceIDs });
                }
                //to do
                /*
                 code to update status of invoices in XERO 
                 for synchronization with our system
                */
            }
            catch (RenewTokenException e)
            {
                //Response.Write(e.Message);
                return Json(new { status = false, error = "RenewTokenException" }, JsonRequestBehavior.AllowGet);
            }
            catch (XeroApiException e)
            {
                //Response.Write(e.Message);
                int errorCount = ((Xero.Api.Infrastructure.Exceptions.ValidationException)(e)).ValidationErrors.Count;
                string errorMsg = string.Join(", ", ((Xero.Api.Infrastructure.Exceptions.ValidationException)(e)).ValidationErrors.AsEnumerable().Select(a => a.Message));

                return Json(new { status = false, error = errorMsg }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                //Response.Write(e.Message);
                return Json(new { status = false, error = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// Remove drafts from xero
        /// </summary>
        /// <param name="xeroInvoiceIdsArray"></param>
        /// <returns>ActionResult</returns>
        private List<string> RemoveDraftsFromXero(string xeroInvoiceIdsArray)
        {
            List<string> lstNotDeletedInvoices = new List<string>();
            List<STCInvoice> lstXeroInvoiceIds = new List<STCInvoice>();
            lstXeroInvoiceIds = Newtonsoft.Json.JsonConvert.DeserializeObject<List<STCInvoice>>(xeroInvoiceIdsArray);

            //if (System.Web.HttpContext.Current.Session["oauth_token"] != null)
            //    Log("oauth_token :" + System.Web.HttpContext.Current.Session["oauth_token"].ToString());
            //if (System.Web.HttpContext.Current.Session["TokenKey"] != null)
            //    Log("TokenKey :" + System.Web.HttpContext.Current.Session["TokenKey"].ToString());
            //if (System.Web.HttpContext.Current.Session["TokenSecret"] != null)
            //    Log("TokenSecret :" + System.Web.HttpContext.Current.Session["TokenSecret"].ToString());

            if (lstXeroInvoiceIds.Count > 0)
            {
                for (int i = 0; i < lstXeroInvoiceIds.Count; i++)
                {
                    string xeroInvoiceId = lstXeroInvoiceIds[i].XeroInvoiceId;


                    if (xeroInvoiceId != "0")
                    {
                        if (!TokenUtilities.TokenExists())
                        {
                            //return Json(new { status = false, error = "invalid_grant" }, JsonRequestBehavior.AllowGet);
                        }
                        var token = TokenUtilities.GetStoredToken();
                        string accessToken = token.AccessToken;
                        string xeroTenantId = token.Tenants[0].TenantId.ToString();
                        var api = new AccountingApi();
                        AccountingApi accountingApi = new AccountingApi();
                        //    if (XeroApiHelper.xeroApiHelperSession != null && xeroInvoiceId != "0")
                        //{
                        //var api = XeroApiHelper.xeroApiHelperSession.CoreApi();

                        //var invoice = api.Invoices.Where("Id = " + new Guid(xeroInvoiceId)).Find().FirstOrDefault(); // Find().Where(a => a.Id == new Guid(xeroInvoiceId)).FirstOrDefault();
                        var result = Task.Run(async () => await accountingApi.GetInvoicesAsync(accessToken, xeroTenantId));
                        result.Wait();
                        var invoices = result.Result._Invoices;
                        var invoice = invoices.Where(a => a.InvoiceID == new Guid(xeroInvoiceId)).FirstOrDefault();
                        if (invoice != null)
                        {
                            Invoice.StatusEnum status = invoice.Status;
                            List<Payment> Payments = invoice.Payments;
                            if (status == Invoice.StatusEnum.DRAFT)
                            {
                                var deleteInvoice = new Invoice { Status = Invoice.StatusEnum.DELETED, InvoiceID = invoice.InvoiceID };
                                var deletedInvoices = new Invoices();
                                deletedInvoices._Invoices = new List<Invoice>() { deleteInvoice };
                                var result1 = Task.Run(async () => await accountingApi.UpdateInvoiceAsync(accessToken, xeroTenantId, new Guid(invoice.InvoiceID.ToString()), deletedInvoices));
                                result1.Wait();
                                //api.Invoices.Update(deleteInvoice);
                            }
                            else if (status == Invoice.StatusEnum.AUTHORISED || status == Invoice.StatusEnum.PAID)
                            {
                                //if (invoice.Payments.Count > 0)
                                //{
                                //    for (int pay = 0; pay < invoice.Payments.Count; pay++)
                                //    {
                                //        //var deletePayment = new Payment { Status = 
                                //        //    PaymentStatus.Deleted, Id = invoice.Payments[pay].Id };
                                //        // api.Payments.Update(deletePayment);

                                //        var deletePayment = new Payment
                                //        {
                                //            Status = Payment.StatusEnum.DELETED,
                                //            PaymentID = invoice.Payments[pay].PaymentID
                                //        };
                                //        var payments = new Payments();
                                //        payments._Payments = new List<Payment>() { deletePayment };
                                //        var result2 = Task.Run(async () => await accountingApi.DeletePaymentAsync(accessToken, xeroTenantId, new Guid(invoice.Payments[pay].PaymentID.ToString()), payments));
                                //        result2.Wait();
                                //    }
                                //}

                                //var deleteInvoice = new Invoice { Status = Invoice.StatusEnum.VOIDED, InvoiceID = invoice.InvoiceID };
                                //var deletedInvoices = new Invoices();
                                //deletedInvoices._Invoices = new List<Invoice>() { deleteInvoice };
                                //var result3 = Task.Run(async () => await accountingApi.UpdateInvoiceAsync(accessToken, xeroTenantId, new Guid(invoice.InvoiceID.ToString()), deletedInvoices));
                                //result3.Wait();
                                //api.Invoices.Update(deleteInvoice);

                                lstNotDeletedInvoices.Add(lstXeroInvoiceIds[i].STCInvoiceNumber);
                            }
                            else if (status == Invoice.StatusEnum.DELETED || status == Invoice.StatusEnum.VOIDED)
                            {
                                lstNotDeletedInvoices.Add(lstXeroInvoiceIds[i].STCInvoiceNumber);
                            }
                        }
                        else
                            lstNotDeletedInvoices.Add(lstXeroInvoiceIds[i].STCInvoiceNumber);
                    }
                    else
                        lstNotDeletedInvoices.Add(lstXeroInvoiceIds[i].STCInvoiceNumber);
                }
            }
            return lstNotDeletedInvoices;
        }
        [HttpPost]
        public ActionResult DeleteSAASInvoice(string invoiceId)
        {
            if (!string.IsNullOrEmpty(invoiceId) && Int32.TryParse(invoiceId, out int n))
            {
                _saasInvoiceBAL.DeleteSAASInvoiceByID(invoiceId);

                //if(noOfRowDeleted > 0)
                //{
                return Json(new { success = true, });
                //}
            }
            return Json(new { status = false, error = "Error Deleting row" }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult RestoreSAASInvoice(string InvoiceId)
        {
            if (!string.IsNullOrEmpty(InvoiceId))
            {
                int invoiceId = Int32.Parse(InvoiceId);
                int noOfRowDeleted = _saasInvoiceBAL.RestoreSAASInvoiceByID(invoiceId);

                if (noOfRowDeleted > 0)
                {
                    return Json(new { success = true, });
                }
            }
            return Json(new { status = false, error = "Error Restoring row" }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult RemoveSelectedSAASInvoice(string invoiceIds)
        {
            List<string> ls = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(invoiceIds);
            foreach (string invoiceId in ls)
            {
                if (string.IsNullOrEmpty(invoiceId) || !Int32.TryParse(invoiceId, out int n))
                    return this.Json(new { success = false, error = "ID is not valid " }, JsonRequestBehavior.AllowGet);
            }
            _saasInvoiceBAL.DeleteSAASInvoiceByID(string.Join(",", ls));

            //if (noOfRowDeleted > 0)
            //{
            return Json(new { success = true, });
            //}

        }

        [HttpPost]
        public JsonResult ImportExcelForUserTypeMenu()
        {
            try
            {
                var postedFile = Request.Files[0];
                var textReader = new StreamReader(postedFile.InputStream);
                DataTable dtCSV = GetCSVTableForUserTypeMenu();

                using (GenericParser parser = new GenericParser())
                {
                    parser.ColumnDelimiter = ','; //".ToCharArray();
                    parser.FirstRowHasHeader = true;
                    parser.FirstRowSetsExpectedColumnCount = true;
                    parser.SetDataSource(textReader);

                    if (parser.Read())
                    {
                        do
                        {
                            try
                            {
                                var row = dtCSV.NewRow();

                                row["MenuId"] = Convert.ToInt32(parser["MenuId"].Trim());
                                row["UserTypeId"] = Convert.ToInt32(parser["UserTypeId"].Trim());

                                dtCSV.Rows.Add(row);
                            }
                            catch (Exception ex)
                            {
                                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
                            }

                        } while (parser.Read());

                        if (dtCSV.Rows.Count > 0)
                        {
                            _saasInvoiceBAL.ImportUserTypeMenuCSV(dtCSV);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }

        private DataTable GetCSVTableForUserTypeMenu()
        {
            DataTable dtCSVData = new DataTable();
            dtCSVData.Columns.Add("MenuId", typeof(Int32));
            dtCSVData.Columns.Add("UserTypeId", typeof(Int32));

            return dtCSVData;
        }

        public DataTable CreateXeroInvoicePaymentTable()
        {
            DataTable dtPayment = new DataTable();
            dtPayment.Columns.Add("Amount", typeof(decimal));
            dtPayment.Columns.Add("PaymentDate", typeof(DateTime));
            dtPayment.Columns.Add("XeroInvoiceID", typeof(string));
            dtPayment.Columns.Add("XeroPaymentID", typeof(string));
            dtPayment.Columns.Add("IsDeleted", typeof(bool));
            return dtPayment;
        }

        public ActionResult DownloadSAASInvoice(bool IsSent, string InvoiceId, int page = 1)
        {
            var strJobID = "";
            var IsInvoiced = "";
            try
            {
                //IList<BatteryStorage> lstSAASInvoice = _saasInvoiceBAL.BatteryStorageList(1, -1, null, null, null, null);
                DataSet dsSAASInvoice = _saasInvoiceBAL.GetSAASInvoiceList(page, 10, "InvoiceId", "asc", "", InvoiceId, "", "", "", "", "", "", "", IsSent, "");

                if (dsSAASInvoice != null && dsSAASInvoice.Tables.Count > 0)
                {
                    if (dsSAASInvoice.Tables[0].Rows.Count > 0)
                    {
                        strJobID = Convert.ToString(dsSAASInvoice.Tables[0].Rows[0]["strJobID"]);
                        IsInvoiced = Convert.ToString(dsSAASInvoice.Tables[0].Rows[0]["IsSent"]);
                    }
                }

                IList<SAASInvoiceDetail> lstSAASInvoiceDetail = new List<SAASInvoiceDetail>();
                lstSAASInvoiceDetail = _saasInvoiceBAL.GetSAASInvoiceDetail(strJobID, IsInvoiced, InvoiceId);

                if (lstSAASInvoiceDetail.Count > 0)
                {
                    using (ExcelPackage pckExport = new ExcelPackage())
                    {
                        ExcelWorksheet worksheetExport = pckExport.Workbook.Worksheets.Add("SAASInvoice");

                        worksheetExport.Cells[1, 1].Value = "Invoice Id";
                        worksheetExport.Cells[1, 1].Style.Font.Bold = true;
                        worksheetExport.Cells[1, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheetExport.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(142, 207, 248));
                        worksheetExport.Cells[1, 2].Value = "Reseller Name";
                        worksheetExport.Cells[1, 2].Style.Font.Bold = true;
                        worksheetExport.Cells[1, 2].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheetExport.Cells[1, 2].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(142, 207, 248));
                        worksheetExport.Cells[1, 3].Value = "Invoice Created";
                        worksheetExport.Cells[1, 3].Style.Font.Bold = true;
                        worksheetExport.Cells[1, 3].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheetExport.Cells[1, 3].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(142, 207, 248));
                        worksheetExport.Cells[1, 4].Value = "Status";
                        worksheetExport.Cells[1, 4].Style.Font.Bold = true;
                        worksheetExport.Cells[1, 4].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheetExport.Cells[1, 4].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(142, 207, 248));
                        worksheetExport.Cells[1, 5].Value = "Invoice Due";
                        worksheetExport.Cells[1, 5].Style.Font.Bold = true;
                        worksheetExport.Cells[1, 5].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheetExport.Cells[1, 5].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(142, 207, 248));
                        worksheetExport.Cells[1, 6].Value = "Settlement Term";
                        worksheetExport.Cells[1, 6].Style.Font.Bold = true;
                        worksheetExport.Cells[1, 6].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheetExport.Cells[1, 6].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(142, 207, 248));
                        worksheetExport.Cells[1, 7].Value = "Rate";
                        worksheetExport.Cells[1, 7].Style.Font.Bold = true;
                        worksheetExport.Cells[1, 7].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheetExport.Cells[1, 7].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(142, 207, 248));
                        worksheetExport.Cells[1, 8].Value = "Quantity";
                        worksheetExport.Cells[1, 8].Style.Font.Bold = true;
                        worksheetExport.Cells[1, 8].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheetExport.Cells[1, 8].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(142, 207, 248));
                        worksheetExport.Cells[1, 9].Value = "Total Amount";
                        worksheetExport.Cells[1, 9].Style.Font.Bold = true;
                        worksheetExport.Cells[1, 9].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheetExport.Cells[1, 9].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(142, 207, 248));
                        worksheetExport.Cells[1, 10].Value = "Amount Paid";
                        worksheetExport.Cells[1, 10].Style.Font.Bold = true;
                        worksheetExport.Cells[1, 10].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheetExport.Cells[1, 10].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(142, 207, 248));
                        worksheetExport.Cells[1, 11].Value = "Billing Period";
                        worksheetExport.Cells[1, 11].Style.Font.Bold = true;
                        worksheetExport.Cells[1, 11].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheetExport.Cells[1, 11].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(142, 207, 248));
                        worksheetExport.Cells[1, 12].Value = "Sent";
                        worksheetExport.Cells[1, 12].Style.Font.Bold = true;
                        worksheetExport.Cells[1, 12].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheetExport.Cells[1, 12].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(142, 207, 248));
                        worksheetExport.Cells.AutoFitColumns();

                        int lastRowsInserted = 2;

                        /* Header Detail*/
                        for (int rowNo = 0; rowNo < dsSAASInvoice.Tables[0].Rows.Count; rowNo++)
                        {
                            worksheetExport.Cells[lastRowsInserted, 1].Value = dsSAASInvoice.Tables[0].Rows[rowNo]["InvoiceNumber"];
                            worksheetExport.Cells[lastRowsInserted, 2].Value = dsSAASInvoice.Tables[0].Rows[rowNo]["ResellerName"];

                            //var dateAndTime = DateTime.ParseExact(dsSAASInvoice.Tables[0].Rows[rowNo]["CreatedDate"].ToString(), "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);


                            string s = DateTime.ParseExact(dsSAASInvoice.Tables[0].Rows[rowNo]["CreatedDate"].ToString(), "dd/MM/yyyy hh:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                            worksheetExport.Cells[lastRowsInserted, 3].Value = s;
                            worksheetExport.Cells[lastRowsInserted, 4].Value = dsSAASInvoice.Tables[0].Rows[rowNo]["Status"];
                            //var dateAndTime2 = DateTime.ParseExact(dsSAASInvoice.Tables[0].Rows[rowNo]["InvoiceDueDate"].ToString(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                            s = DateTime.ParseExact(dsSAASInvoice.Tables[0].Rows[rowNo]["InvoiceDueDate"].ToString(), "dd/MM/yyyy hh:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                            worksheetExport.Cells[lastRowsInserted, 5].Value = s;
                            worksheetExport.Cells[lastRowsInserted, 6].Value = dsSAASInvoice.Tables[0].Rows[rowNo]["SettlementTerms"];
                            worksheetExport.Cells[lastRowsInserted, 7].Value = dsSAASInvoice.Tables[0].Rows[rowNo]["Price"];
                            worksheetExport.Cells[lastRowsInserted, 8].Value = dsSAASInvoice.Tables[0].Rows[rowNo]["Quantity"];
                            worksheetExport.Cells[lastRowsInserted, 9].Value = dsSAASInvoice.Tables[0].Rows[rowNo]["TotalAmount"];
                            worksheetExport.Cells[lastRowsInserted, 10].Value = dsSAASInvoice.Tables[0].Rows[rowNo]["PaidAmount"];
                            worksheetExport.Cells[lastRowsInserted, 11].Value = dsSAASInvoice.Tables[0].Rows[rowNo]["BillingPeriod"];
                            worksheetExport.Cells[lastRowsInserted, 12].Value = dsSAASInvoice.Tables[0].Rows[rowNo]["IsSent"];
                            lastRowsInserted++;
                        }


                        lastRowsInserted = lastRowsInserted + 2;
                        worksheetExport.Cells[lastRowsInserted, 1].Value = "SrNo.";
                        worksheetExport.Cells[lastRowsInserted, 1].Style.Font.Bold = true;
                        worksheetExport.Cells[lastRowsInserted, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheetExport.Cells[lastRowsInserted, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(218, 250, 135));
                        worksheetExport.Cells[lastRowsInserted, 2].Value = "Invoice Id";
                        worksheetExport.Cells[lastRowsInserted, 2].Style.Font.Bold = true;
                        worksheetExport.Cells[lastRowsInserted, 2].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheetExport.Cells[lastRowsInserted, 2].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(218, 250, 135));
                        worksheetExport.Cells[lastRowsInserted, 3].Value = "Job ID";
                        worksheetExport.Cells[lastRowsInserted, 3].Style.Font.Bold = true;
                        worksheetExport.Cells[lastRowsInserted, 3].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheetExport.Cells[lastRowsInserted, 3].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(218, 250, 135));
                        worksheetExport.Cells[lastRowsInserted, 4].Value = "RefNumber";
                        worksheetExport.Cells[lastRowsInserted, 4].Style.Font.Bold = true;
                        worksheetExport.Cells[lastRowsInserted, 4].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheetExport.Cells[lastRowsInserted, 4].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(218, 250, 135));
                        worksheetExport.Cells[lastRowsInserted, 5].Value = "OwnerName";
                        worksheetExport.Cells[lastRowsInserted, 5].Style.Font.Bold = true;
                        worksheetExport.Cells[lastRowsInserted, 5].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheetExport.Cells[lastRowsInserted, 5].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(218, 250, 135));
                        worksheetExport.Cells[lastRowsInserted, 6].Value = "OwnerAddress";
                        worksheetExport.Cells[lastRowsInserted, 6].Style.Font.Bold = true;
                        worksheetExport.Cells[lastRowsInserted, 6].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheetExport.Cells[lastRowsInserted, 6].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(218, 250, 135));
                        worksheetExport.Cells[lastRowsInserted, 7].Value = "STC Amount";
                        worksheetExport.Cells[lastRowsInserted, 7].Style.Font.Bold = true;
                        worksheetExport.Cells[lastRowsInserted, 7].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheetExport.Cells[lastRowsInserted, 7].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(218, 250, 135));
                        worksheetExport.Cells[lastRowsInserted, 8].Value = "CER Status";
                        worksheetExport.Cells[lastRowsInserted, 8].Style.Font.Bold = true;
                        worksheetExport.Cells[lastRowsInserted, 8].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheetExport.Cells[lastRowsInserted, 8].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(218, 250, 135));
                        worksheetExport.Cells[lastRowsInserted, 9].Value = "REC CreationDate";
                        worksheetExport.Cells[lastRowsInserted, 9].Style.Font.Bold = true;
                        worksheetExport.Cells[lastRowsInserted, 9].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheetExport.Cells[lastRowsInserted, 9].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(218, 250, 135));
                        worksheetExport.Cells[lastRowsInserted, 10].Value = "PVD Code";
                        worksheetExport.Cells[lastRowsInserted, 10].Style.Font.Bold = true;
                        worksheetExport.Cells[lastRowsInserted, 10].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheetExport.Cells[lastRowsInserted, 10].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(218, 250, 135));
                        worksheetExport.Cells.AutoFitColumns();

                        int lastChildRowsInserted = lastRowsInserted + 1;
                        /* Child Details*/
                        for (int rowNo = 0; rowNo < lstSAASInvoiceDetail.Count; rowNo++)
                        {
                            worksheetExport.Cells[lastChildRowsInserted, 1].Value = rowNo + 1;
                            worksheetExport.Cells[lastChildRowsInserted, 2].Value = lstSAASInvoiceDetail[rowNo].InvoiceId;
                            worksheetExport.Cells[lastChildRowsInserted, 3].Value = lstSAASInvoiceDetail[rowNo].JobId;
                            worksheetExport.Cells[lastChildRowsInserted, 4].Value = lstSAASInvoiceDetail[rowNo].RefNumber;
                            worksheetExport.Cells[lastChildRowsInserted, 5].Value = lstSAASInvoiceDetail[rowNo].OwnerName;
                            worksheetExport.Cells[lastChildRowsInserted, 6].Value = lstSAASInvoiceDetail[rowNo].OwnerAddress;
                            worksheetExport.Cells[lastChildRowsInserted, 7].Value = lstSAASInvoiceDetail[rowNo].STCPrice;
                            worksheetExport.Cells[lastChildRowsInserted, 8].Value = lstSAASInvoiceDetail[rowNo].Status;
                            worksheetExport.Cells[lastChildRowsInserted, 9].Value = lstSAASInvoiceDetail[rowNo].RECBulkUploadTime;
                            worksheetExport.Cells[lastChildRowsInserted, 10].Value = lstSAASInvoiceDetail[rowNo].STCPVDCode;
                            lastChildRowsInserted++;
                        }

                        string filename = "SAASInvoice" + DateTime.Now.ToString("dd-MMM-yyyy") + ".xlsx";

                        Response.Clear();
                        Response.ClearHeaders();
                        Response.ClearContent();
                        Response.ContentType = "application/octet-stream";
                        Response.AddHeader("Content-Disposition", string.Format("attachment; filename=" + filename));
                        Response.BinaryWrite(pckExport.GetAsByteArray());
                    }
                }

                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult CreateNewInvoice(string ResellerID, string SettlementTermId, string Price, string UnitQTY, bool IsGST)
        {
            try
            {
                _saasInvoiceBAL.CreateNewInvoice(ResellerID, SettlementTermId, Price, UnitQTY, IsGST);
            }
            catch (Exception)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
    }
}
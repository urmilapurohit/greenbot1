using FormBot.BAL;
using FormBot.BAL.Service;
using FormBot.BAL.Service;
using FormBot.Entity;
using FormBot.Entity.KendoGrid;
using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FormBot.Main.Controllers
{
    public class SAASInvoiceBuilderController : Controller
    {
        #region Properties
        private readonly IUserBAL _user;
        private readonly ISAASInvoiceBuilderBAL _saasInvoiceBuilderBAL;
        #endregion

        #region Constructor
        public SAASInvoiceBuilderController(IUserBAL user, ISAASInvoiceBuilderBAL saasInvoiceBuilderBAL)
        {
            _user = user;
            _saasInvoiceBuilderBAL = saasInvoiceBuilderBAL;
        }
        #endregion


        // GET: SAASInvoiceBuilder
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

            SAASInvoiceBuilder SAASInvoiceBuilder = new SAASInvoiceBuilder();
            SAASInvoiceBuilder.lstUserTypes = _saasInvoiceBuilderBAL.GetUserTypes();

            List<SelectListItem> lstItems = new List<SelectListItem>();
            if (SAASInvoiceBuilder.lstUserTypes != null)
            {
                for (int i = 0; i <= SAASInvoiceBuilder.lstUserTypes.Count - 1; i++)
                {
                    lstItems.Add(new SelectListItem() { Text = SAASInvoiceBuilder.lstUserTypes[i].UserTypeName, Value = SAASInvoiceBuilder.lstUserTypes[i].UserTypeID.ToString() });
                }
            }
            ViewBag.lstUserTypes = new SelectList(lstItems);

            return View(SAASInvoiceBuilder);
        }

        public JsonResult GetSAASInvoiceBuilderList(int PageNumber = 0, int pageSize = 10, string InvoiceId = "", string userTypeID = "", string SAASUserId = "", string UserRole = "", List<KendoGridSorting> sort = null, KendoGridData filter = null)
        {

            DataSet dsAllColumnsData = new DataSet();
            dsAllColumnsData = _saasInvoiceBuilderBAL.GetSAASInvoiceBuilderList(1, pageSize, SAASUserId, userTypeID, UserRole);

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


            //IList<dynamic> lstSAASInvoice = new List<dynamic>();

            //lstSAASInvoice.Add(new {
            //    InvoiceNumber = "PV04102022", // Create this manually 
            //    Username = "DP", // Get this from Users table
            //    ResellerCompany = "ResellerCompany", // Get this from Users/SolarCompany table
            //    UserType = "UserType", // Get this from User table
            //    UserRole = "UserRole", // Get RoleID from UserRoles than get role from Role table
            //    SettlementTerms = "SettlementTerms", // Get this from STCJobDetails and for that need JobID which we get from Newly creted table SAASInvoiceBuider
            //    InvoicedDate = DateTime.Now.ToString(), 
            //    CreatedDate = DateTime.Now.ToString(), // Get this from STCJobDetails
            //    IsSAASInvoiced = "IsSAASInvoiced",

            //});
            //return Json(new { total = 1, data = lstSAASInvoice }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSAASInvoiceBuilderListBasedOnTerms(int page = 0, int pageSize = 10, string InvoiceId = "", string userTypeID = "", string SAASUserId = "", string UserRole = "", bool IsIsArchive = false, List<KendoGridSorting> sort = null, KendoGridData filter = null)
        {

            DataSet dsAllColumnsData = new DataSet();
            dsAllColumnsData = _saasInvoiceBuilderBAL.GetSAASInvoiceBuilderListBasedOnTerms(page, pageSize, SAASUserId, userTypeID, UserRole, IsIsArchive);

            DataTable newDatatable = new DataTable();
            newDatatable = dsAllColumnsData.Tables[0];

            newDatatable = FilteringAndSortingSAASInvoiceDetails(newDatatable, filter, sort);
            //int total = newDatatable.Rows.Count;
            //if (total > 0)
            //{
            //    DataTable dataTable = newDatatable.Rows.Cast<System.Data.DataRow>().Skip((page - 1) * pageSize).Take(pageSize).CopyToDataTable();
            //    dsAllColumnsData.Tables[0].Clear();
            //    dsAllColumnsData.Tables[0].Merge(dataTable);
            //}
            //else
            //{
            //    dsAllColumnsData.Tables[0].Clear();
            //}
            int total = newDatatable.Rows.Count;
            if (total > 0)
            {
                DataTable dataTable = newDatatable.Rows.Cast<System.Data.DataRow>().CopyToDataTable();
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

        public JsonResult GetSAASInvoiceBuilderDetail(string invoiceNumber)
        {
            IList<SAASInvoiceDetail> lstSAASInvoiceDetail = new List<SAASInvoiceDetail>();
            lstSAASInvoiceDetail = _saasInvoiceBuilderBAL.GetSAASInvoiceBuilderDetail(invoiceNumber);
            return Json(new { total = 10, data = lstSAASInvoiceDetail }, JsonRequestBehavior.AllowGet);
            //IList<dynamic> lstSAASInvoice = new List<dynamic>();

            //lstSAASInvoice.Add(new
            //{
            //    //InvoiceId = "PV04102022",
            //    JobId = "DP",
            //    RefNumber = "ResellerCompany",
            //    OwnerName = "UserType",
            //    OwnerAddress = "UserRole",
            //    STCPrice = "SettlementTerms",
            //    Status = "Status",
            //    RECBulkUploadTime = DateTime.Now.ToString(),
            //    STCPVDCode = "Code69",

            //});
            //return Json(new { total = 1, data = lstSAASInvoice }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSAASInvoiceBuilderDetailBasedOnTerms(string strJobID)
        {
            IList<SAASInvoiceDetail> lstSAASInvoiceDetail = new List<SAASInvoiceDetail>();
            lstSAASInvoiceDetail = _saasInvoiceBuilderBAL.GetSAASInvoiceBuilderDetailBasedOnTerms(strJobID);
            return Json(new { total = 10, data = lstSAASInvoiceDetail }, JsonRequestBehavior.AllowGet);
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


        //[HttpPost]
        //public ActionResult SendToSAASInvoices(SAASInvoiceBuilder objSAASInvoiceBuilder)
        //{
        //    //List<string> ls = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(invoiceIds);
        //    //foreach (string invoiceId in ls)
        //    //{
        //    //    if (string.IsNullOrEmpty(invoiceId) || !Int32.TryParse(invoiceId, out int n))
        //    //        return this.Json(new { success = false, error = "ID is not valid " }, JsonRequestBehavior.AllowGet);
        //    //}
        //    _saasInvoiceBuilderBAL.SendToSAASInvoices(objSAASInvoiceBuilder);

        //    return Json(new { success = true });
        //}

        [HttpPost]
        public ActionResult SendToSAASInvoices(List<SAASInvoiceBuilder> objSAASInvoiceBuilder)
        {
            var LastActionedJobID = "";
            var InvoiceId = "";
            for (int i = 0; i < objSAASInvoiceBuilder.Count; i++)
            {
                if (!objSAASInvoiceBuilder[i].isBulkRecord)
                {
                    if (objSAASInvoiceBuilder[i].strAllJobIds == "" || objSAASInvoiceBuilder[i].strAllJobIds == "null")
                    {
                        if (InvoiceId == "")
                        {
                            objSAASInvoiceBuilder[i].strAllJobIds = LastActionedJobID.TrimEnd(',');
                        }
                        else
                        {
                            if (InvoiceId == objSAASInvoiceBuilder[i].InvoiceID)
                            {
                                objSAASInvoiceBuilder[i].strAllJobIds = LastActionedJobID.TrimEnd(',');
                            }
                            else
                            {
                                LastActionedJobID = "";
                                objSAASInvoiceBuilder[i].strAllJobIds = LastActionedJobID;
                            }
                        }
                    }
                }

                _saasInvoiceBuilderBAL.SendToSAASInvoices(objSAASInvoiceBuilder[i]);

                if (!objSAASInvoiceBuilder[i].isBulkRecord)
                {
                    LastActionedJobID += objSAASInvoiceBuilder[i].strJobID + ",";
                    InvoiceId = objSAASInvoiceBuilder[i].InvoiceID;
                }
            }

            return Json(new { success = true });
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult GetUserRolesSASS(string id)
        {
            DataSet ds = _saasInvoiceBuilderBAL.GetUserRoles(Convert.ToInt32(id));

            var Items = ds.Tables[0].AsEnumerable().Select(dataRow => new UserRolesSASS { Name = dataRow.Field<string>("Name"), RoleId = dataRow.Field<int>("RoleId") }).ToList();
            return Json(Items, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetSAASForPricingManager()
        {
            List<Reseller> items = _saasInvoiceBuilderBAL.GetSAASUsers();
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteSAASInvoiceFomBuilder(string jobid)
        {
            if (!string.IsNullOrEmpty(jobid) && Int32.TryParse(jobid, out int n))
            {
                _saasInvoiceBuilderBAL.DeleteSAASInvoiceFomBuilderByID(jobid);

                //if(noOfRowDeleted > 0)
                //{
                return Json(new { success = true, });
                //}
            }
            return Json(new { status = false, error = "Error Deleting row" }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult CreateNewInvoice(string UserID, string ResellerID, string ResellerName, string SettelmentTerm, string Rate, string QTY, string BillingPeriod, string GlobalTermId, string IsGlobalGST, string JobID)
        {
            try
            {
                _saasInvoiceBuilderBAL.CreateNewInvoice(UserID, ResellerID, ResellerName, SettelmentTerm, Rate, QTY, BillingPeriod, GlobalTermId, IsGlobalGST, JobID);
            }
            catch (Exception ex)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMonthAndQTYBasedOnTerms(string SettelmentTerm, string Rate, string UserID)
        {
            IList<SAASInvoiceDetail> lstBuilderInvoiceData = new List<SAASInvoiceDetail>();
            lstBuilderInvoiceData = _saasInvoiceBuilderBAL.GetMonthAndQTYBasedOnTerms(SettelmentTerm, Rate, UserID);
            return Json(new { total = 10, data = lstBuilderInvoiceData }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGlobalBillingTermsList(string UserID)
        {
            IList<FormBot.Entity.SAASPricingManager> lstTermsList = _saasInvoiceBuilderBAL.GetGlobalBillingTermsList(UserID);

            return Json(lstTermsList, JsonRequestBehavior.AllowGet);
        }
    }
}
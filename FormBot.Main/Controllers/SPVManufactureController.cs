using FormBot.BAL.Service.SPV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FormBot.Entity.SPV;
using FormBot.Helper;
using System.Text.RegularExpressions;
using FormBot.Entity.KendoGrid;
using FormBot.Entity;
using FormBot.BAL.Service;
using System.Data;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace FormBot.Main.Controllers
{
    public class SPVManufactureController : Controller
    {
        private readonly ISpvLogBAL _spvLog;
        private readonly Logger _log;
        private readonly ICERImportBAL _cerImportBAL;
        public SPVManufactureController(ISpvLogBAL spvLog, ICERImportBAL cerImportBAL)
        {
            _spvLog = spvLog;
            _log = new Logger();
            this._cerImportBAL = cerImportBAL;
        }
        // GET: SPVManufacture

        [UserAuthorization]
        public ActionResult Index()
        {
            Spvmanufacturer model = new Spvmanufacturer();
            ViewBag.Version = _cerImportBAL.GetCERLog(SystemEnums.CERType.SPVManufacturer, SystemEnums.CERSubType.None);
            return View(model);
        }

        [HttpPost]
        public JsonResult GetSPVManufacture(string SPVManufactureName, string ServiceAdministrator, List<KendoGridSorting> sort = null)
        {
            string pageSize = "10";
            int page = 1;

            try
            {
                List<Spvmanufacturer> lstSPVManufactures = new List<Spvmanufacturer>();
                string SortCol = "", SortDir = "";
                if (sort != null)
                    if (sort.Any())
                    {
                        SortCol = sort.FirstOrDefault().Field;
                        SortDir = sort.FirstOrDefault().Dir;
                    }
                lstSPVManufactures = _spvLog.GetSPVManufacture(Convert.ToInt32(pageSize), page, SortCol, SortDir, SPVManufactureName, ServiceAdministrator);
                //List<Spvmanufacturer> newSpvmanufacturers = new List<Spvmanufacturer>();
                
                //newSpvmanufacturers = lstSPVManufactures.Skip((page - 1) * Convert.ToInt32(pageSize)).Take(Convert.ToInt32(pageSize)).ToList();
                
                return Json(new { total = (lstSPVManufactures.Any() ? lstSPVManufactures.FirstOrDefault().TotalRecords : 0), data = lstSPVManufactures }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _log.LogException(SystemEnums.Severity.Error, $"SPVManufactureController/GetSPVManufacture", ex);
                return Json(new { total = 0, data = new List<dynamic>() }, JsonRequestBehavior.AllowGet);
            }




        }
        [HttpGet]
        public JsonResult GetSupplierList(int id)
        {
            try
            {
                List<string> lstSupplier = _spvLog.GetSupplierList(id);
                return Json(new { lstSupplier }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _log.LogException(SystemEnums.Severity.Error, $"SPVManufactureController/GetSupplierList", ex);
                return Json(new { total = 0, data = new List<dynamic>() }, JsonRequestBehavior.AllowGet);
            }
        }
        public void UpdateSPVDataForPanel(SPVReferenceJson JsonData, bool isFromSyncJson = false, bool isFromUploadJson = false, string fileName = null)
        {
            DataTable SPVdatatable = new DataTable();
            SPVdatatable.Columns.Add("Manufacture", typeof(string));
            SPVdatatable.Columns.Add("ManufactureProductVerificationUrl", typeof(string));
            SPVdatatable.Columns.Add("ManufactureInstallationVerificationUrl", typeof(string));
            SPVdatatable.Columns.Add("ServiceAdministrator", typeof(string));
            SPVdatatable.Columns.Add("Supplier", typeof(string));
            for (int i = 0; i < JsonData.manufacturers.Count; i++)
            {
                if (JsonData.manufacturers[i].suppliers.Select(x => x.endpointid).Distinct().Count() == 1)
                {
                    for (int j = 0; j < JsonData.manufacturers[i].suppliers.Count; j++)
                    {
                        DataRow dr = SPVdatatable.NewRow();
                        dr["Manufacture"] = JsonData.manufacturers[i].name;
                        dr["ManufactureProductVerificationUrl"] = JsonData.endpoints.Where(m => m.id == JsonData.manufacturers[i].suppliers[0].endpointid).Select(m => m.productverification).FirstOrDefault();
                        dr["ManufactureInstallationVerificationUrl"] = JsonData.endpoints.Where(m => m.id == JsonData.manufacturers[i].suppliers[0].endpointid).Select(m => m.installverification).FirstOrDefault();
                        dr["Supplier"] = JsonData.manufacturers[i].suppliers[j].name;
                        dr["ServiceAdministrator"] = JsonData.endpoints.Where(m => m.id == JsonData.manufacturers[i].suppliers[0].endpointid).FirstOrDefault().serviceadministrator;
                        SPVdatatable.Rows.Add(dr);
                    }
                }
            }
            _cerImportBAL.SyncSPVJson(SPVdatatable, isFromSyncJson, isFromUploadJson, fileName);
        }

        [HttpPost]
        public JsonResult UploadSPVJsonFile()
        {
            try
            {
                List<string> message = new List<string>();
                HttpPostedFileBase fileUpload = Request.Files[0];
                if (fileUpload != null && fileUpload.ContentLength > 0)
                {
                    string filename = Path.GetFileName(fileUpload.FileName);
                    using (var sr = new StreamReader(fileUpload.InputStream, Encoding.UTF8))
                    {
                        string SPVData = sr.ReadToEnd();
                        var JsonData = JsonConvert.DeserializeObject<SPVReferenceJson>(SPVData);
                        UpdateSPVDataForPanel(JsonData, false, true, filename);
                        message.Add(_cerImportBAL.GetCERLog(SystemEnums.CERType.SPVManufacturer, SystemEnums.CERSubType.None));
                        return Json(new { status = true, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }

                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// Sync SPV manufacture list from reference  data json 
        /// </summary>
        /// <returns></returns>
        public JsonResult SyncSPVJson()
        {
            try
            {
                List<string> message = new List<string>();
                var SPVData = new WebClient().DownloadString(ProjectConfiguration.SPVReferenceJsonUrl);
                var JsonData = JsonConvert.DeserializeObject<SPVReferenceJson>(SPVData);
                UpdateSPVDataForPanel(JsonData, true, false);
                message.Add(_cerImportBAL.GetCERLog(SystemEnums.CERType.SPVManufacturer, SystemEnums.CERSubType.None));
                return Json(new { status = true, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult saveSpvSetByManufacturerPopUp(string Spvmanufacturerid)
        {
            try
            {
                _cerImportBAL.SaveSpvSetByManufacturerPopUp(Spvmanufacturerid);
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult SaveExcludeReseller(string resellerIds, int spvManufacturerId)
        {
            try
            {
                _spvLog.SaveExcludeReseller(resellerIds,spvManufacturerId);
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetExcludedReseller(int spvManufacturerId)
        {
            try
            {
               string strReseller =  _spvLog.GetExcludedReseller(spvManufacturerId);
               return Json(new { status = true, data = strReseller }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using FormBot.Main.Models;
using System.ComponentModel;
using FormBot.Entity;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using FormBot.BAL.Service;
using FormBot.Helper;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;
using System.IO;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI;
using OfficeOpenXml;
using System.Net;
using Newtonsoft.Json;
using FormBot.Entity.SPV;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using LumenWorks.Framework.IO.Csv;
using FormBot.Helper.Helper;
using FormBot.DAL;

namespace FormBot.Main.Controllers
{
    public class CERImportController : BaseController
    {
        #region Properties

        private readonly ICERImportBAL _cerImportBAL;
        private const string FAILURE = "failure";
        private const string SUCCESS = "success";
        private const string FILEPATH = "CERFiles";

        #endregion

        #region Constructor

        public CERImportController(ICERImportBAL cerImportBAL)
        {
            this._cerImportBAL = cerImportBAL;
        }

        #endregion

        /// <summary>
        /// Indexes this instance for load default view.
        /// </summary>
        /// <returns>default page</returns>
        public ActionResult Index()
        {
            CERImport model = new CERImport();
            CERImportBAL cerImportbal = new CERImportBAL();
            model.FileType = (from SystemEnums.CERType e in Enum.GetValues(typeof(SystemEnums.CERType))
                              select new SelectListItem()
                              {
                                  Value = ((int)e).ToString(),
                                  Text = GetDescription(e)
                              }).ToList();
            return View(model);
        }

        /// <summary>
        /// Gets the description with using annotation.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>string name</returns>
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
        /// Upload file with  import model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="fileUpload">The file upload.</param>
        /// <returns>return model view</returns>
        [HttpPost]
        public ActionResult Index(CERImport model, HttpPostedFileBase fileUpload)
        {
            if (ModelState.IsValid)
            {
                CERImportBAL cerImportbal = new CERImportBAL();
                model.FileType = (from SystemEnums.CERType e in Enum.GetValues(typeof(SystemEnums.CERType))
                                  select new SelectListItem()
                                  {
                                      Value = ((int)e).ToString(),
                                      Text = GetDescription(e)
                                  }).ToList();

                if (fileUpload != null && fileUpload.ContentLength > 0)
                {
                    string filePath = string.Empty;
                    filePath = Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + FILEPATH + "\\" + fileUpload.FileName);
                    fileUpload.SaveAs(filePath);

                    cerImportbal.MergeDataTable(fileUpload.InputStream, SystemEnums.CERType.SerialNumbers, filePath, "");
                }
            }

            return View(model);
        }

        /// <summary>
        /// Get method for Photovoltaic the modules .
        /// </summary>
        /// <returns>modules model</returns>
        [HttpGet]
        [UserAuthorization]
        public ActionResult PhotovoltaicModules()
        {
            PVModules model = new PVModules();
            ViewBag.Version = _cerImportBAL.GetCERLog(SystemEnums.CERType.PhotovoltaicModules, SystemEnums.CERSubType.None);
            return View(model);
        }

        [HttpGet]
        [UserAuthorization]
        public ActionResult BatteryStorage()
        {
            BatteryStorage model = new BatteryStorage();
            ViewBag.Version = _cerImportBAL.GetCERLog(SystemEnums.CERType.BatteryStorage, SystemEnums.CERSubType.None);
            return View(model);
        }

        /// <summary>
        /// Post method Photovoltaic the modules file.
        /// </summary>
        /// <returns>file result</returns>
        [HttpPost]
        public ActionResult PhotovoltaicModulesFile()
        {
            List<string> message = new List<string>();
            string filePath = string.Empty;
            if (ModelState.IsValid)
            {
                HttpPostedFileBase fileUpload = Request.Files[0];
                if (fileUpload != null && fileUpload.ContentLength > 0)
                {
                    string filename = Path.GetFileName(fileUpload.FileName);
                    filePath = Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + FILEPATH + "\\" + filename);
                    fileUpload.SaveAs(filePath);
                    var msg = _cerImportBAL.MergeDataTable(fileUpload.InputStream, SystemEnums.CERType.PhotovoltaicModules, filePath, filename);
                    //if (_cerImportBAL.MergeDataTable(fileUpload.InputStream, SystemEnums.CERType.PhotovoltaicModules, filePath, filename))
                    if (msg.ToLower() == "true")
                    {
                        message.Add(SUCCESS);
                        message.Add(_cerImportBAL.GetCERLog(SystemEnums.CERType.PhotovoltaicModules, SystemEnums.CERSubType.None));
                    }
                    else
                    {
                        message.Add(msg);
                    }

                }

            }
            else
            {
                message.Add(FAILURE);
            }

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            return Json(message);
        }

        [HttpPost]
        public ActionResult UploadBatteryStorage()
        {
            List<string> message = new List<string>();
            string filePath = string.Empty;
            if (ModelState.IsValid)
            {
                HttpPostedFileBase fileUpload = Request.Files[0];
                if (fileUpload != null && fileUpload.ContentLength > 0)
                {
                    string filename = Path.GetFileName(fileUpload.FileName);
                    filePath = Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + FILEPATH + "\\" + filename);
                    fileUpload.SaveAs(filePath);
                    var msg = _cerImportBAL.MergeDataTable(fileUpload.InputStream, SystemEnums.CERType.BatteryStorage, filePath, filename);
                    //if (_cerImportBAL.MergeDataTable(fileUpload.InputStream, SystemEnums.CERType.BatteryStorage, filePath, filename) == "true")
                    if (msg.ToLower() == "true")
                    {
                        message.Add(SUCCESS);
                        message.Add(_cerImportBAL.GetCERLog(SystemEnums.CERType.BatteryStorage, SystemEnums.CERSubType.None));
                    }
                    else
                    {
                        message.Add(msg);
                    }

                }

            }
            else
            {
                message.Add(FAILURE);
            }

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            return Json(message);
        }

        /// <summary>
        /// Get method for Approved the inverters.
        /// </summary>
        /// <returns>Inverters model</returns>
        [HttpGet]
        [UserAuthorization]
        public ActionResult ApprovedInverters()
        {
            Inverters model = new Inverters();
            ViewBag.Version = _cerImportBAL.GetCERLog(SystemEnums.CERType.ApprovedInverters, SystemEnums.CERSubType.None);
            return View(model);
        }

        /// <summary>
        /// Post method for Approved the inverters file.
        /// </summary>
        /// <returns>file result</returns>
        [HttpPost]
        public ActionResult ApprovedInvertersFile()
        {
            List<string> message = new List<string>();
            string filePath = string.Empty;
            if (ModelState.IsValid)
            {
                HttpPostedFileBase fileUpload = Request.Files[0];
                if (fileUpload != null && fileUpload.ContentLength > 0)
                {
                    string filename = Path.GetFileName(fileUpload.FileName);
                    filePath = Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + FILEPATH + "\\" + filename);
                    fileUpload.SaveAs(filePath);
                    var msg = _cerImportBAL.MergeDataTable(fileUpload.InputStream, SystemEnums.CERType.ApprovedInverters, filePath, filename);
                    //if (_cerImportBAL.MergeDataTable(fileUpload.InputStream, SystemEnums.CERType.ApprovedInverters, filePath, filename) == "true")
                    if (msg.ToLower() == "true")
                    {
                        message.Add(SUCCESS);
                        message.Add(_cerImportBAL.GetCERLog(SystemEnums.CERType.ApprovedInverters, SystemEnums.CERSubType.None));
                    }
                    else
                    {
                        message.Add(msg);
                    }

                }

            }
            else
            {
                message.Add(FAILURE);
            }

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            return Json(message);
        }

        /// <summary>
        /// get method Accredited the installers.
        /// </summary>
        /// <returns>blank view</returns>
        [HttpGet]
        [UserAuthorization]
        public ActionResult AccreditedInstallers()
        {
            ViewBag.Version = _cerImportBAL.GetCERLog(SystemEnums.CERType.AccreditedInstallers, SystemEnums.CERSubType.None);
            return View();
        }

        /// <summary>
        /// Post method for Accredited the installers file.
        /// </summary>
        /// <returns>file import message</returns>
        [HttpPost]
        public ActionResult AccreditedInstallersFile()
        {
            List<string> message = new List<string>();
            string filePath = string.Empty;
            if (ModelState.IsValid)
            {
                HttpPostedFileBase fileUpload = Request.Files[0];
                if (fileUpload != null && fileUpload.ContentLength > 0)
                {
                    string filename = Path.GetFileName(fileUpload.FileName);
                    filePath = Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + FILEPATH + "\\" + filename);
                    fileUpload.SaveAs(filePath);
                    var msg = _cerImportBAL.MergeDataTable(fileUpload.InputStream, SystemEnums.CERType.AccreditedInstallers, filePath, filename);
                    //if (_cerImportBAL.MergeDataTable(fileUpload.InputStream, SystemEnums.CERType.AccreditedInstallers, filePath, filename))
                    if (msg == "true")
                    {
                        message.Add(SUCCESS);
                        message.Add(_cerImportBAL.GetCERLog(SystemEnums.CERType.AccreditedInstallers, SystemEnums.CERSubType.None));
                    }
                    else
                    {
                        message.Add(msg);
                    }

                }

            }
            else
            {
                message.Add(FAILURE);
            }

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            return Json(message);
        }
        /// <summary>
        /// Call API and return jsonstring
        /// </summary>
        /// <param name="SyncAccreditedInstallerURL">URL of API for getting json data(Current/Historical)</param>
        /// <returns>jsondata after call API in string format</returns>
        public string ResponseJsonStringOfSyncAccreditedInstallerURL(string SyncAccreditedInstallerURL)
        {
            System.Net.ServicePointManager.Expect100Continue = false;
            WebRequest req = WebRequest.Create(SyncAccreditedInstallerURL);
            req.Method = "GET";
            req.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(ProjectConfiguration.SyncAccreditedInstallerListUsername + ":" + ProjectConfiguration.SyncAccreditedInstallerListPassword));
            HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
            string jsonString;
            using (Stream stream = resp.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                jsonString = reader.ReadToEnd();
            }
            return jsonString;
        }
        /// <summary>
        /// Declare Accredited Installer datatable
        /// </summary>
        /// <returns>datatable</returns>
        public DataTable AccreditedInstallersdatatable()
        {
            DataTable AccreditedInstallersdatatable = new DataTable();

            AccreditedInstallersdatatable.Columns.Add("SyncAccreditedInstallerId", typeof(string));
            AccreditedInstallersdatatable.Columns.Add("ContactSNo", typeof(string));
            AccreditedInstallersdatatable.Columns.Add("InstallerStatus", typeof(string));
            AccreditedInstallersdatatable.Columns.Add("AccreditationNumber", typeof(string));
            AccreditedInstallersdatatable.Columns.Add("FirstName", typeof(string));
            AccreditedInstallersdatatable.Columns.Add("LastName", typeof(string));
            AccreditedInstallersdatatable.Columns.Add("ConcatenatedMailAddress", typeof(string));
            AccreditedInstallersdatatable.Columns.Add("MailingAddressUnitType", typeof(string));
            AccreditedInstallersdatatable.Columns.Add("MailingAddressUnitNumber", typeof(string));
            AccreditedInstallersdatatable.Columns.Add("MailingAddressStreetNumber", typeof(string));
            AccreditedInstallersdatatable.Columns.Add("MailingAddressStreetName", typeof(string));
            AccreditedInstallersdatatable.Columns.Add("MailingAddressStreetType", typeof(string));
            AccreditedInstallersdatatable.Columns.Add("MailingCity", typeof(string));
            AccreditedInstallersdatatable.Columns.Add("MailingState", typeof(string));
            AccreditedInstallersdatatable.Columns.Add("PostalCode", typeof(string));
            AccreditedInstallersdatatable.Columns.Add("MailingCountry", typeof(string));
            AccreditedInstallersdatatable.Columns.Add("Phone", typeof(string));
            AccreditedInstallersdatatable.Columns.Add("Fax", typeof(string));
            AccreditedInstallersdatatable.Columns.Add("Mobile", typeof(string));
            AccreditedInstallersdatatable.Columns.Add("Email", typeof(string));
            AccreditedInstallersdatatable.Columns.Add("GridType", typeof(string));
            AccreditedInstallersdatatable.Columns.Add("SPS", typeof(string));
            AccreditedInstallersdatatable.Columns.Add("InstallerFullAwardDate", typeof(string));
            AccreditedInstallersdatatable.Columns.Add("InstallerProvisionalAwardDate", typeof(string));
            AccreditedInstallersdatatable.Columns.Add("InstallerExpiryDate", typeof(string));
            AccreditedInstallersdatatable.Columns.Add("SuspensionStartDate", typeof(string));
            AccreditedInstallersdatatable.Columns.Add("SuspensionEndDate", typeof(string));
            AccreditedInstallersdatatable.Columns.Add("LicensedElectricianNumber", typeof(string));
            AccreditedInstallersdatatable.Columns.Add("Endorsements", typeof(string));
            AccreditedInstallersdatatable.Columns.Add("CreatedBy", typeof(Int32));
            AccreditedInstallersdatatable.Columns.Add("AccountName", typeof(string));
            return AccreditedInstallersdatatable;
        }
        /// <summary>
        /// Insert data in datatable
        /// </summary>
        /// <param name="JsonData">parse data from json which has been add in datatable columns value</param>
        /// <param name="dataTable">datatable of accerdited installer data</param>
        /// <returns>datatable after add all data of json into table</returns>
        public DataTable InsertDataInInstallerDatatable(AccreditedInstallerData JsonData, ref DataTable dataTable)
        {
            for (int i = 0; i < JsonData.Accredited_Solar_Installer.Count; i++)
            {
                DataRow dr = dataTable.NewRow();
                dr["SyncAccreditedInstallerId"] = JsonData.Accredited_Solar_Installer[i].details.ID;
                dr["InstallerStatus"] = JsonData.Accredited_Solar_Installer[i].details.SolarInstallerStatus;
                dr["AccreditationNumber"] = JsonData.Accredited_Solar_Installer[i].details.SolarAccreditationNumber;
                dr["FirstName"] = JsonData.Accredited_Solar_Installer[i].details.FirstName;
                dr["LastName"] = JsonData.Accredited_Solar_Installer[i].details.LastName;
                dr["MailingAddressUnitType"] = JsonData.Accredited_Solar_Installer[i].details.MailingAddressUnitType;
                dr["MailingAddressUnitNumber"] = JsonData.Accredited_Solar_Installer[i].details.MailingAddressUnitNumber;
                dr["MailingAddressStreetNumber"] = JsonData.Accredited_Solar_Installer[i].details.MailingAddressStreetNumber;
                dr["MailingAddressStreetName"] = JsonData.Accredited_Solar_Installer[i].details.MailingAddressStreetName;
                dr["MailingAddressStreetType"] = JsonData.Accredited_Solar_Installer[i].details.MailingAddressStreetType;
                dr["MailingCity"] = JsonData.Accredited_Solar_Installer[i].details.MailingCity;
                dr["MailingState"] = JsonData.Accredited_Solar_Installer[i].details.MailingStateProvince;
                dr["PostalCode"] = JsonData.Accredited_Solar_Installer[i].details.MailingZipPostalCode;
                dr["MailingCountry"] = JsonData.Accredited_Solar_Installer[i].details.Mailing_Country;
                dr["Phone"] = JsonData.Accredited_Solar_Installer[i].details.Phone;
                dr["Fax"] = JsonData.Accredited_Solar_Installer[i].details.Fax;
                dr["Mobile"] = JsonData.Accredited_Solar_Installer[i].details.Mobile;
                dr["Email"] = JsonData.Accredited_Solar_Installer[i].details.Email;
                dr["GridType"] = JsonData.Accredited_Solar_Installer[i].details.GridType;
                dr["SPS"] = JsonData.Accredited_Solar_Installer[i].details.SPS;
                dr["InstallerFullAwardDate"] = ConvertJsonDateToString(JsonData.Accredited_Solar_Installer[i].details.SolarInstallerFullAwardDate);
                dr["InstallerProvisionalAwardDate"] = ConvertJsonDateToString(JsonData.Accredited_Solar_Installer[i].details.SolarInstallerProvisionalAwardDate);
                dr["InstallerExpiryDate"] = ConvertJsonDateToString(JsonData.Accredited_Solar_Installer[i].details.Solar_InstallerExpiry_Date);
                dr["SuspensionStartDate"] = ConvertJsonDateToString(JsonData.Accredited_Solar_Installer[i].details.SuspensionStart_Date);
                dr["SuspensionEndDate"] = ConvertJsonDateToString(JsonData.Accredited_Solar_Installer[i].details.SuspensionEndDate);
                dr["LicensedElectricianNumber"] = JsonData.Accredited_Solar_Installer[i].details.LicensedElectricianNumber;
                dr["Endorsements"] = JsonData.Accredited_Solar_Installer[i].details.Endorsements;
                dr["CreatedBy"] = ProjectSession.LoggedInUserId;
                dr["ContactSNo"] = JsonData.Accredited_Solar_Installer[i].details.Contact_SNo;
                dr["ConcatenatedMailAddress"] = JsonData.Accredited_Solar_Installer[i].details.Concatenated_MailAddress;
                dr["AccountName"] = JsonData.Accredited_Solar_Installer[i].account.details.Name;
                dataTable.Rows.Add(dr);
            }
            return dataTable;
        }
        /// <summary>
        /// Convert Date from json file date to string format
        /// </summary>
        /// <param name="JsonDate">date which is getting from json data</param>
        /// <returns>date in string format</returns>
        public string ConvertJsonDateToString(string JsonDate)
        {
            string solarInstallerFullAwardDate = string.Empty;
            DateTime date;
            double decimalNumber;

            if (JsonDate != null && !string.IsNullOrEmpty(Convert.ToString(JsonDate).Trim()))
            {
                if (DateTime.TryParse(JsonDate, out date))
                {
                    solarInstallerFullAwardDate = date.ToString("yyyy/MM/dd");
                }
                else
                {
                    if (double.TryParse(JsonDate, out decimalNumber))
                    {
                        solarInstallerFullAwardDate = DateTime.FromOADate(double.Parse(Convert.ToString(JsonDate))).ToString("yyyy/MM/dd");
                    }
                }
            }
            else
            {
                solarInstallerFullAwardDate = null;
            }
            return solarInstallerFullAwardDate;
        }

        /// <summary>
        /// Sync Accerdited Installer list from json file by call API and store in DB.
        /// </summary>
        /// <returns>Json result</returns>
        public JsonResult SyncAccreditedInstallersList()
        {
            try
            {
               string jsonStringForCurrentInstaller= ResponseJsonStringOfSyncAccreditedInstallerURL(ProjectConfiguration.SyncAccreditedInstallerListCurrentURL);

                DataTable Installersdatatable = AccreditedInstallersdatatable();

                var JsonDataForCurrentInstaller = JsonConvert.DeserializeObject<AccreditedInstallerData>(jsonStringForCurrentInstaller);

                Installersdatatable = InsertDataInInstallerDatatable(JsonDataForCurrentInstaller, ref Installersdatatable);

                string jsonStringForHistoricalInstaller = ResponseJsonStringOfSyncAccreditedInstallerURL(ProjectConfiguration.SyncAccreditedInstallerListHistoricalURL);

                var JsonDataForHistoricalInstaller = JsonConvert.DeserializeObject<AccreditedInstallerData>(jsonStringForHistoricalInstaller);

                Installersdatatable = InsertDataInInstallerDatatable(JsonDataForHistoricalInstaller, ref Installersdatatable);

                _cerImportBAL.SyncAccreditedInstallerList(Installersdatatable);
                List<string> message = new List<string>();
                message.Add(_cerImportBAL.GetCERLog(SystemEnums.CERType.AccreditedInstallers, SystemEnums.CERSubType.None));

                return Json(new { status = true,message=message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }
       
        /// <summary>
        /// Get method for the brand model.
        /// </summary>
        /// <returns>brand model</returns>
        [HttpGet]
        [UserAuthorization]
        public ActionResult HWBrandModel()
        {
            HWBrandModel model = new HWBrandModel();
            ViewBag.AirSource = _cerImportBAL.GetCERLog(SystemEnums.CERType.HWBrandModel, SystemEnums.CERSubType.AirSource);
            ViewBag.SolarWaterLess700L = _cerImportBAL.GetCERLog(SystemEnums.CERType.HWBrandModel, SystemEnums.CERSubType.SolarWaterLess700L);
            ViewBag.SolarWaterMore700L = _cerImportBAL.GetCERLog(SystemEnums.CERType.HWBrandModel, SystemEnums.CERSubType.SolarWaterMore700L);
            return View(model);
        }

        /// <summary>
        /// the brand model file.
        /// </summary>
        /// <param name="subType">Type of the sub.</param>
        /// <returns>action result</returns>
        [HttpPost]
        public ActionResult HWBrandModelFile(int subType)
        {
            List<string> message = new List<string>();
            string filePath = string.Empty;
            if (ModelState.IsValid)
            {
                HttpPostedFileBase fileUpload = Request.Files[0];
                if (fileUpload != null && fileUpload.ContentLength > 0)
                {
                    string filename = Path.GetFileName(fileUpload.FileName);
                    filePath = Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + FILEPATH + "\\" + filename);
                    fileUpload.SaveAs(filePath);
                    SystemEnums.CERSubType cerSubType = (SystemEnums.CERSubType)subType;
                    var msg = _cerImportBAL.MergeDataTable(fileUpload.InputStream, SystemEnums.CERType.HWBrandModel, filePath, fileUpload.FileName, cerSubType);
                    //if (_cerImportBAL.MergeDataTable(fileUpload.InputStream, SystemEnums.CERType.HWBrandModel, filePath, fileUpload.FileName, cerSubType) == "true")
                    if (msg.ToLower() == "true")
                    {
                        message.Add(SUCCESS);
                        message.Add(_cerImportBAL.GetCERLog(SystemEnums.CERType.HWBrandModel, SystemEnums.CERSubType.AirSource));
                        message.Add(_cerImportBAL.GetCERLog(SystemEnums.CERType.HWBrandModel, SystemEnums.CERSubType.SolarWaterLess700L));
                        message.Add(_cerImportBAL.GetCERLog(SystemEnums.CERType.HWBrandModel, SystemEnums.CERSubType.SolarWaterMore700L));
                    }
                    else
                    {
                        message.Add(msg);
                    }

                }

            }
            else
            {
                message.Add(FAILURE);
            }

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            return Json(message);
        }

        /// <summary>
        /// the provider.
        /// </summary>
        /// <returns>action result</returns>
        [HttpGet]
        [UserAuthorization]
        public ActionResult ElectricityProvider()
        {
            FormBot.Entity.ElectricityProvider model = new ElectricityProvider();
            ViewBag.Version = _cerImportBAL.GetCERLog(SystemEnums.CERType.ElectricityProvider, SystemEnums.CERSubType.None);
            return View(model);
        }

        /// <summary>
        /// the provider file.
        /// </summary>
        /// <returns>action result</returns>
        [HttpPost]
        public ActionResult ElectricityProviderFile()
        {
            List<string> message = new List<string>();
            if (ModelState.IsValid)
            {
                HttpPostedFileBase fileUpload = Request.Files[0];
                string filePath = string.Empty;
                if (fileUpload != null && fileUpload.ContentLength > 0)
                {
                    //filePath = HttpContext.Server.MapPath(FILEPATH) + "\\" + fileUpload.FileName;
                    filePath = Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + FILEPATH + "\\" + fileUpload.FileName);
                    fileUpload.SaveAs(filePath);
                    var msg = _cerImportBAL.MergeDataTable(fileUpload.InputStream, SystemEnums.CERType.ElectricityProvider, filePath, fileUpload.FileName);
                    //if (_cerImportBAL.MergeDataTable(fileUpload.InputStream, SystemEnums.CERType.ElectricityProvider, filePath, fileUpload.FileName) == "true")
                    if (msg.ToLower() == "true")
                    {
                        message.Add(SUCCESS);
                        message.Add(_cerImportBAL.GetCERLog(SystemEnums.CERType.ElectricityProvider, SystemEnums.CERSubType.None));
                    }
                    else
                    {
                        message.Add(msg);
                    }

                }

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                return Json(message);
            }

            message.Add(FAILURE);
            return Json(message);
        }

        /// <summary>
        /// Updates the modules.
        /// </summary>
        /// <param name="moduleId">The module identifier.</param>
        /// <param name="wattage">The wattage.</param>
        /// <returns>update modules</returns>
        [HttpPost]
        public ActionResult UpdatePVModules(string moduleId, string wattage)
        {
            if (!string.IsNullOrEmpty(moduleId) && !string.IsNullOrEmpty(wattage))
            {
                _cerImportBAL.UpdatePVModules(Convert.ToInt32(moduleId), Convert.ToInt32(wattage));
            }

            return Json("Updated");
        }

        /// <summary>
        /// Gets the modules list and bind with grid.
        /// </summary>
        /// <param name="certificate">The certificate holder.</param>
        /// <param name="modelNumber">The model number.</param>
        /// <param name="wattage">The wattage.</param>
        /// <param name="cecApprovedDate">The approved date.</param>
        /// <param name="expiryDate">The expiry date.</param>
        public void GetPVModulesList(string certificate, string modelNumber, string wattage, string cecApprovedDate, string expiryDate)
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);
            try
            {
                IList<PVModules> lstPVModules = _cerImportBAL.ModulesList(pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, certificate, modelNumber, wattage, cecApprovedDate, expiryDate);
                if (lstPVModules.Count > 0)
                {
                    gridParam.TotalDisplayRecords = lstPVModules.FirstOrDefault().TotalRecords;
                    gridParam.TotalRecords = lstPVModules.FirstOrDefault().TotalRecords;
                }

                HttpContext.Response.Write(Grid.PrepareDataSet(lstPVModules, gridParam));
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                HttpContext.Response.Write(Grid.PrepareDataSet(new List<PVModules>(), gridParam));
            }
        }

        public void GetBatteryStorageList(string manufacturer, string modelNumber)
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);
            try
            {
                IList<BatteryStorage> lstPVModules = _cerImportBAL.BatteryStorageList(pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, manufacturer, modelNumber);
                if (lstPVModules.Count > 0)
                {
                    gridParam.TotalDisplayRecords = lstPVModules.FirstOrDefault().TotalRecords;
                    gridParam.TotalRecords = lstPVModules.FirstOrDefault().TotalRecords;
                }

                HttpContext.Response.Write(Grid.PrepareDataSet(lstPVModules, gridParam));
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                HttpContext.Response.Write(Grid.PrepareDataSet(new List<PVModules>(), gridParam));
            }
        }

        /// <summary>
        /// Gets the accredited installers list and bind with grid.
        /// </summary>
        /// <param name="accreditationNumber">The accreditation number.</param>
        /// <param name="name">The name.</param>
        /// <param name="accountName">Name of the account.</param>
        /// <param name="licensedElectricianNumber">The licensed electrician number.</param>
        /// <param name="gridType">Type of the grid.</param>
        public void GetAccreditedInstallersList(string accreditationNumber, string name, string accountName, string licensedElectricianNumber, string gridType)
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);
            try
            {
                IList<AccreditedInstallers> lstAccreditedInstallersList = _cerImportBAL.AccreditedInstallersList(pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, accreditationNumber, name, accountName, licensedElectricianNumber, gridType);
                if (lstAccreditedInstallersList.Count > 0)
                {
                    gridParam.TotalDisplayRecords = lstAccreditedInstallersList.FirstOrDefault().TotalRecords;
                    gridParam.TotalRecords = lstAccreditedInstallersList.FirstOrDefault().TotalRecords;

                }

                HttpContext.Response.Write(Grid.PrepareDataSet(lstAccreditedInstallersList, gridParam));
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                HttpContext.Response.Write(Grid.PrepareDataSet(new List<AccreditedInstallers>(), gridParam));
            }
        }

        /// <summary>
        /// Gets the brand model list and bind with grid.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="brand">The brand.</param>
        /// <param name="model">The model.</param>
        /// <param name="eligibleFrom">The eligible from.</param>
        /// <param name="eligibleTo">The eligible to.</param>
        public void GetHWBrandModelList(string item, string brand, string model, string eligibleFrom, string eligibleTo)
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);
            try
            {
                IList<HWBrandModel> lstHWBrandModel = _cerImportBAL.HWBrandModelList(pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, item, brand, model, eligibleFrom, eligibleTo);
                if (lstHWBrandModel.Count > 0)
                {
                    gridParam.TotalDisplayRecords = lstHWBrandModel.FirstOrDefault().TotalRecords;
                    gridParam.TotalRecords = lstHWBrandModel.FirstOrDefault().TotalRecords;

                }

                HttpContext.Response.Write(Grid.PrepareDataSet(lstHWBrandModel, gridParam));
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                HttpContext.Response.Write(Grid.PrepareDataSet(new List<HWBrandModel>(), gridParam));
            }
        }

        /// <summary>
        /// Gets the inverters list and bind with grid.
        /// </summary>
        /// <param name="manufacturer">The manufacturer.</param>
        /// <param name="series">The series.</param>
        /// <param name="modelNumber">The model number.</param>
        /// <param name="acPowerKW">The ac power kw.</param>
        /// <param name="approvalDate">The approval date.</param>
        /// <param name="expiryDate">The expiry date.</param>
        public void GetInvertersList(string manufacturer, string series, string modelNumber, string acPowerKW, string approvalDate, string expiryDate)
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);
            try
            {
                IList<Inverters> lstInverters = _cerImportBAL.InvertersList(pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, manufacturer, series, modelNumber, acPowerKW, approvalDate, expiryDate);
                if (lstInverters.Count > 0)
                {
                    gridParam.TotalDisplayRecords = lstInverters.FirstOrDefault().TotalRecords;
                    gridParam.TotalRecords = lstInverters.FirstOrDefault().TotalRecords;

                }

                HttpContext.Response.Write(Grid.PrepareDataSet(lstInverters, gridParam));
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                HttpContext.Response.Write(Grid.PrepareDataSet(new List<Inverters>(), gridParam));
            }
        }

        public void GetElectricityProviderList(string provider, string type, string state, string preapproval, string connection)
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);
            try
            {
                IList<ElectricityProvider> lstElectricityProvider = _cerImportBAL.ElectricityProviderList(pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, provider, type, state, preapproval, connection);
                if (lstElectricityProvider != null && lstElectricityProvider.Count > 0)
                {
                    gridParam.TotalDisplayRecords = lstElectricityProvider.FirstOrDefault().TotalRecords;
                    gridParam.TotalRecords = lstElectricityProvider.FirstOrDefault().TotalRecords;
                }

                HttpContext.Response.Write(Grid.PrepareDataSet(lstElectricityProvider, gridParam));
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                HttpContext.Response.Write(Grid.PrepareDataSet(new List<ElectricityProvider>(), gridParam));
            }
        }

        /// <summary>
        /// Deletes the electricity provider.
        /// </summary>
        /// <param name="electricityProviderId">The electricity provider identifier.</param>
        /// <returns>action result</returns>
        public ActionResult DeleteElectricityProvider(string electricityProviderId)
        {
            _cerImportBAL.DeleteElectricityProvider(Convert.ToInt32(electricityProviderId));
            return this.Json(new { success = true });
        }

        /// <summary>
        /// Updates the electricity provider.
        /// </summary>
        /// <param name="electricityProviderId">The electricity provider identifier.</param>
        /// <param name="provider">The provider.</param>
        /// <param name="type">The type.</param>
        /// <param name="state">The state.</param>
        /// <param name="preapproval">The preapproval.</param>
        /// <param name="connection">The connection.</param>
        /// <returns>action result</returns>
        public ActionResult UpdateElectricityProvider(string electricityProviderId, string provider, string type, string state, string preapproval, string connection)
        {
            _cerImportBAL.UpdateElectricityProvider(electricityProviderId, provider, type, state, preapproval, connection);      //DeleteElectricityProvider(Convert.ToInt32(electricityProviderId));
            return this.Json(new { success = true });
        }

        /// <summary>
        /// Downloads the modules.
        /// </summary>
        /// <returns>view model</returns>
        public ActionResult DownloadModules()
        {
            try
            {
                IList<PVModules> lstPVModules = _cerImportBAL.ModulesList(1, -1, null, null, null, null, null, null, null);
                using (ExcelPackage pckExport = new ExcelPackage())
                {
                    ExcelWorksheet worksheetExport = pckExport.Workbook.Worksheets.Add("PVModules");
                    worksheetExport.Cells[1, 1].Value = "CertificateHolder";
                    worksheetExport.Cells[1, 2].Value = "ModelNumber";
                    worksheetExport.Cells[1, 3].Value = "Wattage";
                    worksheetExport.Cells[1, 4].Value = "CECApprovedDate";
                    worksheetExport.Cells[1, 5].Value = "ExpiryDate";
                    worksheetExport.Cells[1, 6].Value = "FireTested";
                    int lastRowsInserted = 2;
                    for (int rowNo = 0; rowNo < lstPVModules.Count; rowNo++)
                    {
                        worksheetExport.Cells[lastRowsInserted, 1].Value = lstPVModules[rowNo].CertificateHolder;
                        worksheetExport.Cells[lastRowsInserted, 2].Value = lstPVModules[rowNo].ModelNumber;
                        worksheetExport.Cells[lastRowsInserted, 3].Value = lstPVModules[rowNo].Wattage;
                        worksheetExport.Cells[lastRowsInserted, 4].Value = lstPVModules[rowNo].CECApprovedDate != null ? lstPVModules[rowNo].CECApprovedDate.Value.ToString("dd/MM/yyyy") : "";
                        worksheetExport.Cells[lastRowsInserted, 5].Value = lstPVModules[rowNo].ExpiryDate != null ? lstPVModules[rowNo].ExpiryDate.Value.ToString("dd/MM/yyyy") : "";
                        worksheetExport.Cells[lastRowsInserted, 6].Value = lstPVModules[rowNo].FireTested;
                        lastRowsInserted++;
                    }

                    string filename = "PVModules" + DateTime.Now.ToString("dd-MMM-yyyy") + ".xlsx";
                    Response.Clear();
                    Response.ClearHeaders();
                    Response.ClearContent();
                    Response.ContentType = "application/octet-stream";
                    Response.AddHeader("content-disposition", "attachment;  filename=\"" + filename + "\"");
                    Response.BinaryWrite(pckExport.GetAsByteArray());
                    Response.End();
                }

                PVModules model = new PVModules();
                return View("PhotovoltaicModules", model);
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return View("PhotovoltaicModules", new PVModules());
            }

        }

        public ActionResult DownloadBatteryStorageFile()
        {
            try
            {
                IList<BatteryStorage> lstBatteryStorage = _cerImportBAL.BatteryStorageList(1, -1, null, null, null, null);
                using (ExcelPackage pckExport = new ExcelPackage())
                {
                    ExcelWorksheet worksheetExport = pckExport.Workbook.Worksheets.Add("BatteryStorage");
                    worksheetExport.Cells[1, 1].Value = "Manufacturer";
                    worksheetExport.Cells[1, 2].Value = "ModelNumber";

                    int lastRowsInserted = 2;
                    for (int rowNo = 0; rowNo < lstBatteryStorage.Count; rowNo++)
                    {
                        worksheetExport.Cells[lastRowsInserted, 1].Value = lstBatteryStorage[rowNo].Manufacturer;
                        worksheetExport.Cells[lastRowsInserted, 2].Value = lstBatteryStorage[rowNo].ModelNumber;
                        lastRowsInserted++;
                    }

                    string filename = "BatteryStorage" + DateTime.Now.ToString("dd-MMM-yyyy") + ".xlsx";

                    Response.Clear();
                    Response.ClearHeaders();
                    Response.ClearContent();
                    Response.ContentType = "application/octet-stream";
                    Response.AddHeader("Content-Disposition", string.Format("attachment; filename=" + filename));
                    Response.BinaryWrite(pckExport.GetAsByteArray());

                    //Response.ContentType = "application/octet-stream";
                    //Response.AddHeader("content-disposition", "attachment;  filename=\"" + filename + "\"");
                    //Response.BinaryWrite(pckExport.GetAsByteArray());
                    //Response.End();
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
        public JsonResult GetManufacturerForSetSpv()
        {
            try
            {

                List<Spvmanufacturer> lstspvManufacturers = _cerImportBAL.GetManufacturerForSetSpvByManufacturer().ToList();
                return Json(new { total = lstspvManufacturers.Count, data = lstspvManufacturers }, JsonRequestBehavior.AllowGet);
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

        public JsonResult SyncPvModuleList()
        {
            DataTable dtPVModules = new DataTable();
            try
            {
                List<string> message = new List<string>();
                var PVModulesData = new WebClient().DownloadString(ProjectConfiguration.CERApprovedPVModulesUrl);

                string[] stringSeparators = new string[] { "\r\n" };
                string[] lines = PVModulesData.Split(stringSeparators, StringSplitOptions.None);
                
                int totalColumn = 0;
                for (int i = 0; i < lines.Length; i++)
                {
                    if(i==0)
                    {
                        dtPVModules.Columns.Add("CertificateHolder", typeof(string));
                        dtPVModules.Columns.Add("ModelNumber", typeof(string));
                        dtPVModules.Columns.Add("Wattage", typeof(string));
                        dtPVModules.Columns.Add("CECApprovedDate", typeof(string));
                        dtPVModules.Columns.Add("ExpiryDate", typeof(string));
                        dtPVModules.Columns.Add("FireTested", typeof(string));
                        dtPVModules.Columns.Add("CreatedBy", typeof(Int32));
                        totalColumn = dtPVModules.Columns.Count;
                    }
                    else
                    {
                        string[] drCell = lines[i].Split(',');
                        DataRow dataRow = dtPVModules.NewRow();
                        if(drCell.Length > 1)
                        {
                            for (int j = 0; j < totalColumn; j++)
                            {
                                if (j == 2)
                                    dataRow[j] = Common.GetWattageFromModelNumber(drCell[j - 1]);
                                else if (j == 3 || j == 4 || j == 5)
                                {
                                    string data = (j <= drCell.Length) ? drCell[j - 1] : null;
                                    if(j ==3 || j ==4)
                                    {
                                        double value;
                                        DateTime date;
                                        if (DateTime.TryParse(data, out date))
                                            data = date.ToString("yyyyMMdd");
                                        else
                                        {
                                            if (double.TryParse(data, out value))
                                                data = DateTime.FromOADate(double.Parse(Convert.ToString(data))).ToString("yyyyMMdd");
                                            else
                                                data = null;
                                        }
                                    }
                                    dataRow[j] = data;
                                }
                                else if (j == 6)
                                    dataRow[j] = ProjectSession.LoggedInUserId;
                                else
                                    dataRow[j] = (j < drCell.Length) ? drCell[j] : null;
                            }
                            dtPVModules.Rows.Add(dataRow);
                        }
                    }
                }
                object result = CommonDAL.MergeDataTable(dtPVModules, "MergePVModules", "Sync PVModule", (int)SystemEnums.CERType.PhotovoltaicModules, ProjectSession.LoggedInUserId, false, 0);
                message.Add(_cerImportBAL.GetCERLog(SystemEnums.CERType.PhotovoltaicModules, SystemEnums.CERSubType.None));
                return Json(new { status = true, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
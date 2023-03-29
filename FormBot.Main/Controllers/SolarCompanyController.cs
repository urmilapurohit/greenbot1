using FormBot.BAL.Service;
using FormBot.BAL.Service.CommonRules;
using FormBot.Entity;
using FormBot.Entity.Job;
using FormBot.Helper;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FormBot.Main.Controllers
{
    public class SolarCompanyController : Controller
    {
        #region Properties
        private readonly ISolarCompanyBAL _solarCompanyService;
        private readonly ICreateJobBAL _createJobBAL;
        private readonly Logger _log;
        #endregion

        #region Constructor
        public SolarCompanyController(ISolarCompanyBAL solarCompanyService,ICreateJobBAL createJobBAL)
        {
            this._solarCompanyService = solarCompanyService;
            this._createJobBAL = createJobBAL;
            this._log = new Logger();
        }
        #endregion

        // GET: SolarCompany
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Get SolarCompanyId and SolarCompanyName.
        /// </summary>
        /// <returns>Returns List of Solar Company</returns>
        [HttpGet]
        public JsonResult GetSolarCompany()
        {
            List<SelectListItem> items = _solarCompanyService.GetData().Select(a => new SelectListItem { Text = a.CompanyName, Value = a.SolarCompanyId.ToString() }).ToList();
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get SolarCompanyId and SolarCompanyName for solar sub contractor's.
        /// </summary>
        /// <returns>Returns List of Solar Sub Contractor</returns>
        [HttpGet]
        public JsonResult GetSolarSubContractor()
        {
            List<SelectListItem> items = _solarCompanyService.GetSubContractorData().Select(a => new SelectListItem { Text = a.CompanyName, Value = a.SolarCompanyId.ToString() }).ToList();
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get SolarCompanyByResellerId
        /// </summary>
        /// <param name="id"></param>
        /// <returns>JsonResult</returns>
        public JsonResult GetSolarCompanyByResellerId(string id, bool isAll = false)
        {
            int ID = (id != "null" && !string.IsNullOrEmpty(id)) ? Convert.ToInt32(id) : 0;
            List<SelectListItem> items = _solarCompanyService.GetSolarCompanyByResellerID(ID).Select(a => new SelectListItem { Text = a.CompanyName, Value = a.SolarCompanyId.ToString() }).ToList();
            if (isAll && items.Count > 1)
                items.Add(new SelectListItem() { Value = "-1", Text = "All" });
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get SolarCompanyByMultipleResellerId
        /// </summary>
        /// <param name="id"></param>
        /// <returns>JsonResult</returns>
        public JsonResult GetSolarCompanyByMultipleResellerId(List<int> id, bool isAll = false)
        {
            string ResellerIds = string.Join(",", id);
            List<SelectListItem> items = _solarCompanyService.GetSolarCompanyByMultipleResellerID(ResellerIds).Select(a => new SelectListItem { Text = a.CompanyName, Value = a.SolarCompanyId.ToString() }).ToList();
            if (isAll && items.Count > 1)
                items.Add(new SelectListItem() { Value = "-1", Text = "All" });
            return Json(items, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSolarCompanyForFilter(string id, string solarCompanyId,string ramId, string isAllScaJobView = "", bool isAll = false)
        {

            int ID = (id != "null" && !string.IsNullOrEmpty(id)) ? Convert.ToInt32(id) : 0;
            if (ProjectSession.UserTypeId == 1 || ProjectSession.UserTypeId == 3)
            {
                if (Convert.ToInt32(solarCompanyId) != -1)
                {
                    return Json(new List<SelectListItem>(), JsonRequestBehavior.AllowGet);
                }
                List<SelectListItem> items = new List<SelectListItem>();
                if (string.IsNullOrEmpty(ramId) || Convert.ToInt32(ramId) == 0)
                {
                    items = _solarCompanyService.GetSolarCompanyByResellerID(ID).Select(a => new SelectListItem { Text = a.CompanyName, Value = a.SolarCompanyId.ToString() }).ToList();
                }
                else
                {
                    items = _solarCompanyService.GetSolarCompanyByRAMID(Convert.ToInt32(ramId)).Select(a => new SelectListItem { Text = a.CompanyName, Value = a.SolarCompanyId.ToString() }).ToList();
                }
                if (isAll && items.Count > 1)
                    items.Add(new SelectListItem() { Value = "-1", Text = "All" });
                return Json(items, JsonRequestBehavior.AllowGet);
            }
            if (ProjectSession.UserTypeId == 2 || (ProjectSession.UserTypeId == 5 && Convert.ToBoolean(isAllScaJobView)))
            {
                if (Convert.ToInt32(solarCompanyId) != -1)
                {
                    return Json(new List<SelectListItem>(), JsonRequestBehavior.AllowGet);
                }
                ID = ProjectSession.ResellerId;
                List<SelectListItem> items = new List<SelectListItem>();
                if ((string.IsNullOrEmpty(ramId) || Convert.ToInt32(ramId) != 0) && ProjectSession.UserTypeId == 2)
                {
                    items = _solarCompanyService.GetSolarCompanyByResellerID(Convert.ToInt32(ramId)).Select(a => new SelectListItem { Text = a.CompanyName, Value = a.SolarCompanyId.ToString() }).ToList();
                }
                else
                {
                    items = _solarCompanyService.GetSolarCompanyByResellerID(ID).Select(a => new SelectListItem { Text = a.CompanyName, Value = a.SolarCompanyId.ToString() }).ToList();
                }
                
                if (isAll && items.Count > 1)
                    items.Add(new SelectListItem() { Value = "-1", Text = "All" });
                return Json(items, JsonRequestBehavior.AllowGet);

            }
            if (ProjectSession.UserTypeId == 5 && !Convert.ToBoolean(isAllScaJobView))
            {
                if (Convert.ToInt32(solarCompanyId) != -1)
                {
                    return Json(new List<SelectListItem>(), JsonRequestBehavior.AllowGet);
                }
                ID = ProjectSession.LoggedInUserId;
                List<SelectListItem> items = _solarCompanyService.GetSolarCompanyByRAMID(ID).Select(a => new SelectListItem { Text = a.CompanyName, Value = a.SolarCompanyId.ToString() }).ToList();
                if (isAll && items.Count > 1)
                    items.Add(new SelectListItem() { Value = "-1", Text = "All" });
                return Json(items, JsonRequestBehavior.AllowGet);
            }
            if (ProjectSession.UserTypeId == 6)
            {
                if (Convert.ToInt32(solarCompanyId) != -1)
                {
                    return Json(new List<SelectListItem>(), JsonRequestBehavior.AllowGet);
                }
                ID = ProjectSession.LoggedInUserId;
                List<SelectListItem> items = _solarCompanyService.GetRequestedSolarCompanyToSSC(ID).Select(a => new SelectListItem { Text = a.CompanyName, Value = a.SolarCompanyId.ToString() }).ToList();
                if (isAll && items.Count > 1)
                    items.Add(new SelectListItem() { Value = "-1", Text = "All" });
                return Json(items, JsonRequestBehavior.AllowGet);
            }
            if (ProjectSession.UserTypeId == 7 || ProjectSession.UserTypeId == 10)
            {
                ID = ProjectSession.LoggedInUserId;
                List<SelectListItem> items = _solarCompanyService.GetSolarCompaanyFromSE(ID).Select(a => new SelectListItem { Text = a.CompanyName, Value = a.SolarCompanyId.ToString() }).ToList();
                if (isAll && items.Count > 1)
                    items.Add(new SelectListItem() { Value = "-1", Text = "All" });
                return Json(items, JsonRequestBehavior.AllowGet);
            }
            return Json(new List<SelectListItem>(), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetSolarCompanyByRAMID(string id)
        {
            int ID = !string.IsNullOrEmpty(id) ? Convert.ToInt32(id) : 0;
            List<SelectListItem> items = _solarCompanyService.GetSolarCompanyByRAMID(ID).Select(a => new SelectListItem { Text = a.CompanyName, Value = a.SolarCompanyId.ToString() }).ToList();
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRequestedSolarCompanyToSSC()
        {
            List<SelectListItem> items = _solarCompanyService.GetRequestedSolarCompanyToSSC(ProjectSession.LoggedInUserId).Select(a => new SelectListItem { Text = a.CompanyName, Value = a.SolarCompanyId.ToString() }).ToList();
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        ///// <summary>
        ///// Get SolarCompany By ResellerId for FSA change user
        ///// </summary>
        ///// <param name="resellerId">resellerId</param>
        ///// <param name="isTypeChange">isTypeChange</param>
        ///// <returns>JsonResult</returns>
        //[HttpGet]
        //public JsonResult GetSolarCompanyByResellerIdFSAUserChange(string resellerId, string isTypeChange)
        //{
        //    if (!string.IsNullOrEmpty(isTypeChange) && Convert.ToInt32(isTypeChange) == 1)
        //        ProjectSession.SystemSolarCompanyTableByReseller = null;

        //    if (ProjectSession.SystemSolarCompanyTableByReseller != null && ProjectSession.SystemSolarCompanyTableByReseller.Rows.Count > 0)
        //    {
        //        DataTable dtSolarCompany = ProjectSession.SystemSolarCompanyTableByReseller;
        //        List<SelectListItem> items = new List<SelectListItem>();
        //        for (int i = 0; i < dtSolarCompany.Rows.Count; i++)
        //        {
        //            items.Add(new SelectListItem()
        //            {
        //                Text = dtSolarCompany.Rows[i]["CompanyName"].ToString(),
        //                Value = dtSolarCompany.Rows[i]["SolarCompanyId"].ToString()
        //            });
        //        }
        //        return Json(items, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        List<SelectListItem> items = _solarCompanyService.GetSolarCompanyByResellerID(!string.IsNullOrEmpty(resellerId) ? Convert.ToInt32(resellerId) : 0).Select(a => new SelectListItem { Text = a.CompanyName, Value = a.SolarCompanyId.ToString() }).ToList();
        //        DataTable dtSolarCompany = new DataTable();
        //        dtSolarCompany.Columns.Add("SolarCompanyId", typeof(string));
        //        dtSolarCompany.Columns.Add("CompanyName", typeof(string));
        //        for (int i = 0; i < items.Count; i++)
        //        {
        //            dtSolarCompany.Rows.Add(new object[] { items[i].Value, items[i].Text });
        //        }
        //        ProjectSession.SystemSolarCompanyTableByReseller = dtSolarCompany;
        //        return Json(items, JsonRequestBehavior.AllowGet);
        //    }
        //}

        /// <summary>
        /// Get SolarCompany By ResellerId for FSA change user
        /// </summary>
        /// <param name="resellerId">resellerId</param>
        /// <param name="isTypeChange">isTypeChange</param>
        /// <returns>JsonResult</returns>
        [HttpGet]
        public JsonResult GetSolarCompanyByResellerIdFSAUserChange(string resellerId, string isTypeChange)
        {

            if (!string.IsNullOrEmpty(isTypeChange) && Convert.ToInt32(isTypeChange) == 1)
                ProjectSession.SystemSolarCompanyByReseller = null;

            if (ProjectSession.SystemSolarCompanyByReseller != null)
                return Json(ProjectSession.SystemSolarCompanyByReseller, JsonRequestBehavior.AllowGet);
            else
            {
                List<SelectListItem> items = _solarCompanyService.GetSolarCompanyByResellerID(!string.IsNullOrEmpty(resellerId) ? Convert.ToInt32(resellerId) : 0).Select(a => new SelectListItem { Text = a.CompanyName, Value = a.SolarCompanyId.ToString() }).ToList();
                ProjectSession.SystemSolarCompanyByReseller = JsonConvert.SerializeObject(items);
                return Json(items, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetSolarCompanyByWholeSalerId(string id)
        {
            int ID = !string.IsNullOrEmpty(id) ? Convert.ToInt32(id) : 0;
            List<SelectListItem> items = _solarCompanyService.GetSolarCompanyByWholeSalerID(ID).Select(a => new SelectListItem { Text = a.CompanyName, Value = a.SolarCompanyId.ToString() }).ToList();
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSolarCompany_IsSSCByResellerId(string id, bool IsSSC)
        {
            //int ID = !string.IsNullOrEmpty(id) ? Convert.ToInt32(id) : 0;
            List<SelectListItem> items = new List<SelectListItem>();
            int resellerId = 0;

            //for get value of SSC dropdown in creation of SCO/Contractor user
            int.TryParse(id, out resellerId);
            //if (int.TryParse(id, out resellerId);)
            //{
                items = _solarCompanyService.GetSolarCompany_IsSSCByResellerID(resellerId, IsSSC).Select(a => new SelectListItem { Text = a.CompanyName, Value = a.SolarCompanyId.ToString() }).ToList();
            //}
            return Json(items, JsonRequestBehavior.AllowGet);
            //int ID = (id != "null" && !string.IsNullOrEmpty(id)) ? Convert.ToInt32(id) : 0;
        }

        public JsonResult CheckIsWholeSaler_ByResellerId(string id)
        {
            int ID = !string.IsNullOrEmpty(id) ? Convert.ToInt32(id) : 0;
            DataSet CheckIsWholeSaler = _solarCompanyService.CheckIsWholeSaler_ByResellerId(ID);
            return Json(Newtonsoft.Json.JsonConvert.SerializeObject(CheckIsWholeSaler), JsonRequestBehavior.AllowGet);
            //bool IsWholeSaler = _solarCompanyService.CheckIsWholeSaler_ByResellerId(ID);
            //return Json(new { status = false, IsWholeSaler = IsWholeSaler }, JsonRequestBehavior.AllowGet);
        }

      /// <summary>
      /// update allowed spv flag for solar company in bulk
      /// </summary>
      /// <param name="solarCompanyIds"></param>
      /// <param name="isSPVAllowedSPV"></param>
      /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateAllowedSPV(string solarCompanyIds, int isSPVAllowedSPV, string resellerIds)
        {
            try
            {
                _log.Log(SystemEnums.Severity.Debug, "start update allowed spv");
                _solarCompanyService.UpdateSolarCompanyAllowedSPV(solarCompanyIds, isSPVAllowedSPV);
                List<string> lstsolarCompanyIds = solarCompanyIds.Split(',').ToList();
                List<string> lstresellerIds = resellerIds.Split(',').ToList();
                //for (int i =0;i< lstsolarCompanyIds.Count(); i++)
                //{
                //    CommonBAL.SetCacheDataForSTCSubmissionFromSolarCompanyId(Convert.ToInt32(lstresellerIds[i]), Convert.ToString(lstsolarCompanyIds[i]));
                //}
                _log.Log(SystemEnums.Severity.Debug, "end update allowed spv");
                return this.Json(new { success = true });
            }
            catch (Exception ex)
            {
                FormBot.Helper.Log.WriteError(ex);
                return this.Json(new { success = false });
            }
        }

        #region Make Solar Company Registered with GST          

        [HttpPost]
        public JsonResult MakeSCARegisteredWithGSTFromResellerId(string resellerId)
        {
            try
            {
                var syncTask = Task.Run(() =>
                {
                    MakeSCARegisteredWithGST(Convert.ToInt32(resellerId));
                });                
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _log.Log(SystemEnums.Severity.Debug, "Error on MakeSCARegisteredWithGST " + ex.Message.ToString());
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public void MakeSCARegisteredWithGST(int resellerId)
        {
            _log.Log(SystemEnums.Severity.Debug, "Start Get All Company To Make Registered With GST = ");
            List<SolarCompany> lstCompanyABN = _createJobBAL.GetAllCompanyToMakeRegisteredWithGSTFromResellerId(resellerId);
            _log.Log(SystemEnums.Severity.Debug, "End Get All Company To Make Registered With GST = ");
            DataTable dtSCAWithGST = SCAWithGSTValue();
            if (lstCompanyABN != null && lstCompanyABN.Count > 0)
            {
                _log.Log(SystemEnums.Severity.Debug, "Start search Company ABN ");
                for (int i = 0; i < lstCompanyABN.Count; i++)
                {
                    bool isRegistered = false;
                    string GSTText = string.Empty;
                    string abnURL = ProjectConfiguration.GetCompanyABNSearchLink + lstCompanyABN[i].CompanyABN;
                    _log.Log(SystemEnums.Severity.Debug, abnURL);
                    try
                    {
                        HttpWebRequest wreq = (HttpWebRequest)WebRequest.Create(abnURL);
                        wreq.Method = "GET";
                        wreq.Timeout = -1;
                        wreq.ContentType = "application/json; charset=UTF-8";
                        var myHttpWebResponse = (HttpWebResponse)wreq.GetResponse();
                        string strResult;
                        using (var reader = new StreamReader(myHttpWebResponse.GetResponseStream(), System.Text.Encoding.UTF8))
                        {
                            strResult = reader.ReadToEnd();
                            myHttpWebResponse.Close();
                        }

                        if (strResult != null)
                        {
                            strResult = WebUtility.HtmlDecode(strResult);
                            HtmlDocument resultat = new HtmlDocument();
                            resultat.LoadHtml(strResult);

                            HtmlNode table = resultat.DocumentNode.SelectSingleNode("//table[1]");
                            if (table != null)
                            {
                                foreach (var cell in table.SelectNodes(".//tr/th"))
                                {
                                    string someVariable = cell.InnerText;
                                    if (cell.InnerText.ToLower() == "goods & services tax (gst):")
                                    {
                                        var td = cell.ParentNode.SelectNodes("./td");
                                        string tdValue = td[0].InnerText;
                                        GSTText = tdValue;
                                        if (tdValue.ToLower().Contains("registered from"))
                                        {
                                            isRegistered = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Log(SystemEnums.Severity.Debug, "MakeSCARegisteredWithGST = " + ex.Message.ToString());
                        //return Json(false, JsonRequestBehavior.AllowGet);
                    }
                    dtSCAWithGST.Rows.Add(new object[] { lstCompanyABN[i].CompanyABN, isRegistered, GSTText.Trim() });
                }
                _log.Log(SystemEnums.Severity.Debug, "End search Company ABN ");
            }
            if (dtSCAWithGST.Rows.Count > 0)
            {
                _log.Log(SystemEnums.Severity.Debug, "Start Make SCA Registered/Deregistered With GST");
                try
                {
                    _createJobBAL.MakeSCARegisteredWithGST(dtSCAWithGST);
                }
                catch (Exception ex)
                {
                    _log.Log(SystemEnums.Severity.Debug, "SetSCAGSTInDataSet = " + ex.Message.ToString());
                    //return Json(false, JsonRequestBehavior.AllowGet);
                }
                _log.Log(SystemEnums.Severity.Debug, "End Make SCA Registered/Deregistered With GST");
            }
        }

        public DataTable SCAWithGSTValue()
        {
            DataTable dtSCAWithGST = new DataTable();
            dtSCAWithGST.Columns.Add("CompanyABN", typeof(string));
            dtSCAWithGST.Columns.Add("isRegisteredWithGST", typeof(bool));
            dtSCAWithGST.Columns.Add("GSTText", typeof(string));
            return dtSCAWithGST;
        }

        #endregion

        [HttpGet]
        public JsonResult GetRepresentative(int JobId=0)
        {
            List<SelectListItem> items = _solarCompanyService.GetRepresentativeForAutoSign(JobId).Select(a => new SelectListItem { Text = a.Name, Value = a.UserId.ToString() }).ToList();
            //items.Add(new SelectListItem() { Value = "-1", Text = "All" });
            return Json(items, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetAutoSignSettingsData(int UserId = 0,bool isForDefaultSelection=false)
        {
            try
            {
                FormBot.Entity.Job.RetailerAutoSetting retailerAutoSetting = new Entity.Job.RetailerAutoSetting();
                DataSet ds= _solarCompanyService.GetAutoSignSettingsData(UserId, isForDefaultSelection);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                   retailerAutoSetting.RetailerUserId = Convert.ToInt32(ds.Tables[0].Rows[0]["RetailerUserId"].ToString());
                    retailerAutoSetting.PositionHeld = ds.Tables[0].Rows[0]["PositionHeld"].ToString();
                    bool IsEmployee = !string.IsNullOrEmpty(ds.Tables[0].Rows[0]["IsEmployee"].ToString()) ? Convert.ToBoolean(ds.Tables[0].Rows[0]["IsEmployee"].ToString()) : false;
                    //bool IssubContractor =!string.IsNullOrEmpty(jobRetailerSetting.Tables[0].Rows[0]["IsSubContractor"].ToString())?Convert.ToBoolean(jobRetailerSetting.Tables[0].Rows[0]["IsSubContractor"].ToString()):false;
                    bool IsChangedDesign = !string.IsNullOrEmpty(ds.Tables[0].Rows[0]["IsChangedDesign"].ToString()) ? Convert.ToBoolean(ds.Tables[0].Rows[0]["IsChangedDesign"].ToString()) : false;
                    retailerAutoSetting.IsEmployee = IsEmployee == true ? 1 : 2;
                    retailerAutoSetting.IsChangedDesign = IsChangedDesign == true ? 1 : 2;
                   retailerAutoSetting.Signature = ds.Tables[0].Rows[0]["Signature"].ToString();
                    if (!string.IsNullOrEmpty(retailerAutoSetting.Signature))
                    {
                        string proofDocumentsFolder = ProjectSession.ProofDocuments;
                        string ProofDocumentsFolder = Path.Combine(proofDocumentsFolder + "\\" + "UserDocuments" + "\\" + retailerAutoSetting.RetailerUserId + "\\");
                        if (Directory.Exists(ProofDocumentsFolder))
                        {

                            string path = Path.Combine(ProofDocumentsFolder + "\\" + retailerAutoSetting.Signature);

                            if (System.IO.File.Exists(path))
                            {

                                retailerAutoSetting.base64Img = ImageToBase64(path);
                            }
                        }
                    }

                }

                    return Json(new { success = true, data = retailerAutoSetting }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { success = false}, JsonRequestBehavior.AllowGet);
            }
            
        }
        [HttpPost]
        public JsonResult SaveAutoSignSettingsData(int UserId,string Position,bool isSubcontractor,bool isEmployee,string Signature,string StringOwnerBaseSignature,string Base30,bool IsUploaded,bool isChangedDesign,string longitude,string latitude)
        {
            try
            {

                //_log.LogException(SystemEnums.Severity.Debug, "start 1 ");
                string path = Path.Combine("UserDocuments" + "\\" + UserId + "\\" + Signature);
                if (IsUploaded == false)
                {
                    //_log.LogException(SystemEnums.Severity.Debug, "dsgfj");
                    ConvertIntoImage(StringOwnerBaseSignature, UserId);
                }
                //_log.LogException(SystemEnums.Severity.Debug, "2");
                _solarCompanyService.SaveAutoSignSettingsData(UserId, Position, isSubcontractor, isEmployee,Signature, isChangedDesign,0,latitude,longitude);

               // _log.LogException(SystemEnums.Severity.Debug, "3");
                return Json(new { success = true}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _log.LogException(SystemEnums.Severity.Error, ex.InnerException.ToString());
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// Converts the into image.
        /// </summary>
        /// <param name="ownerBaseSignature">The owner base signature.</param>
        /// <param name="jobid">The job.</param>
        /// <returns>view result</returns>
        [HttpPost]
        public JsonResult ConvertIntoImage(string BaseSignature, int userId)
        {
            //_log.LogException(SystemEnums.Severity.Debug, "convert to image 1");
            string Signature = BaseSignature;
            byte[] bytIn = null;
            if (Signature != null)
            {
                //_log.LogException(SystemEnums.Severity.Debug, "convert to image 2");
                Signature = Signature.Replace("data:image/png;base64,", "").Replace(' ', '+');
                bytIn = Convert.FromBase64String(Signature);
            }
            //_log.LogException(SystemEnums.Severity.Debug, "convert to image 3");
            System.Drawing.Image img;
            string FileName = string.Empty;
            using (var ms = new MemoryStream(bytIn))
            {
                //_log.LogException(SystemEnums.Severity.Debug, "convert to image 4");
                img = System.Drawing.Image.FromStream(ms);
               

                FileName = "RetailerSign" + "_" + userId + "." + "png";


                string proofDocumentsFolder = ProjectSession.ProofDocuments;
                string ProofDocumentsFolder = Path.Combine(proofDocumentsFolder + "\\" + "UserDocuments" + "\\" + userId + "\\");
               // _log.LogException(SystemEnums.Severity.Debug, "convert to image 5" + ProofDocumentsFolder);
                if (!Directory.Exists(ProofDocumentsFolder))
                {
                    //_log.LogException(SystemEnums.Severity.Debug, "convert to image 6");
                    Directory.CreateDirectory(ProofDocumentsFolder);
                }

                string path = Path.Combine(ProofDocumentsFolder + "\\" + FileName);
                DeleteDirectory(Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + "UserDocuments" + "\\" + userId, FileName));
               // _log.LogException(SystemEnums.Severity.Debug, "convert to image 7"+ Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + "UserDocuments" + "\\" + userId, FileName));
                //_log.LogException(SystemEnums.Severity.Debug, "convert to image 8"+path);
                img.Save(path);
                ms.Close();
                ms.Dispose();
                img.Dispose();
            }

           
           // _log.LogException(SystemEnums.Severity.Debug, "convert to image 8");
            return Json(FileName, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Deletes the directory.
        /// </summary>
        /// <param name="path">The path.</param>
        private void DeleteDirectory(string path)
        {
            if (System.IO.File.Exists(path))
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                System.IO.File.Delete(path);
            }

        }
        /// <summary>
        /// Uploads the Owner Signature .
        /// </summary>
        /// <param name="jobId">job id.</param>
        /// <returns>object result</returns>
        [HttpPost]
        public JsonResult UploadRetailerSignature(string userId)
        {
            List<HelperClasses.UploadStatus> uploadStatus = new List<HelperClasses.UploadStatus>();
            if (Request.Files != null && Request.Files.Count != 0)
            {
                for (var i = 0; i < Request.Files.Count; i++)
                {
                    uploadStatus.Add(GetFileUpload(Request.Files[i], userId.ToString()));
                }
            }

            return Json(uploadStatus);
        }
        /// <summary>
        /// Gets the file upload.
        /// </summary>
        /// <param name="fileUpload">The file upload.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>result class</returns>
        public HelperClasses.UploadStatus GetFileUpload(HttpPostedFileBase fileUpload, string userId)
        {
            HelperClasses.UploadStatus uploadStatus = new HelperClasses.UploadStatus();
            uploadStatus.FileName = Request.Files[0].FileName;
            if (fileUpload != null)
            {
                if (fileUpload.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(fileUpload.FileName);
                    string proofDocumentsFolder = ProjectSession.ProofDocuments;
                    string proofDocumentsFolderURL = ProjectSession.ProofDocumentsURL;
                    if (userId != null)
                    {
                        proofDocumentsFolder = Path.Combine(proofDocumentsFolder + "\\" + "UserDocuments" + "\\" + userId + "\\");
                        proofDocumentsFolderURL = proofDocumentsFolderURL + "\\" + "UserDocuments" + "\\" + userId + "\\";
                    }

                    if (!Directory.Exists(proofDocumentsFolder))
                    {
                        Directory.CreateDirectory(proofDocumentsFolder);
                    }

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    try
                    {
                        string path = Path.Combine(proofDocumentsFolder + "\\" + fileName.Replace("%", "$"));
                        if (System.IO.File.Exists(path))
                        {
                            string orignalFileName = Path.GetFileNameWithoutExtension(path);
                            string fileExtension = Path.GetExtension(path);
                            string fileDirectory = Path.GetDirectoryName(path);
                            int i = 1;
                            while (true)
                            {
                                string renameFileName = fileDirectory + @"\" + orignalFileName + "(" + i + ")" + fileExtension;
                                if (System.IO.File.Exists(renameFileName))
                                {
                                    i++;
                                }
                                else
                                {
                                    path = renameFileName;
                                    break;
                                }
                            }
                            fileName = Path.GetFileName(path);
                        }

                        fileName = fileName.Replace("%", "$");
                        string mimeType = MimeMapping.GetMimeMapping(fileName);
                        fileUpload.SaveAs(path);
                        uploadStatus.Status = true;
                        uploadStatus.Message = "File Uploaded Successfully.";
                        uploadStatus.FileName = fileName;
                        uploadStatus.MimeType = mimeType;
                        uploadStatus.Path = proofDocumentsFolder + uploadStatus.FileName;
                    }
                    catch (Exception)
                    {
                        uploadStatus.Status = false;
                        uploadStatus.Message = "An error occured while uploading. Please try again later.";
                    }
                }
                else
                {
                    uploadStatus.Status = false;
                    uploadStatus.Message = "No data received";
                }
            }
            else
            {
                uploadStatus.Status = false;
                uploadStatus.Message = "No data received";
            }
            return uploadStatus;
        }
        /// <summary>
        /// Delete Owner Sign From Folder and Table.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="folderName">The user identifier.</param>
        /// <returns>object result</returns>
        [AllowAnonymous]
        public JsonResult deleteRetailerSignFromFolderandTable(string fileName, string folderName)
        {
            DeleteDirectory(Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + "UserDocuments" + "\\" + folderName, fileName));
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public PartialViewResult RetailerStatement(int UserId)
        {
            JsonResult data = GetAutoSignSettingsData(UserId, false);
            string obj = JsonConvert.SerializeObject(data.Data);
            JSONResponseForRetailerSetting setting = JsonConvert.DeserializeObject<JSONResponseForRetailerSetting>(obj);
            return PartialView("_RetailerStatement", setting.data);
        }
        public string ImageToBase64(string path)
        {
            string base64String = null;
            using (System.Drawing.Image image = System.Drawing.Image.FromFile(path))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();
                    base64String = Convert.ToBase64String(imageBytes);
                    return "data:image/png;base64," + base64String;
                }
            }
        }
    }
    public class JSONResponseForRetailerSetting
    {
        public string success { get; set; }
        public RetailerAutoSetting data { get; set; }
    }
}
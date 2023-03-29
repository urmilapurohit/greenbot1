using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FormBot.BAL.Service.Documents;
using FormBot.BAL.Service.Job;
using FormBot.Entity.Job;
using FormBot.Helper;
using FormBot.Entity.Documents;
using System.IO;
using FormBot.Entity.Pdf;
using iTextSharp.text.pdf;
using System.Text;
using System.Collections;
using FormBot.BAL.Service;
using FormBot.Entity;
using System.Data;
using FormBot.Entity.Notification;
using FormBot.BAL.Service.Dashboard;
using Newtonsoft.Json;
using FormBot.BAL.Service.CommonRules;
using FormBot.Helper.Helper;
using FormBot.BAL;
using System.Threading.Tasks;

namespace FormBot.Main.Controllers
{
    public class JobSettingController : Controller
    {

        #region Properties
        private readonly IJobSettingBAL _jobSettingBAL;

        /// <summary>
        /// The _document template bal
        /// </summary>
        private readonly IDocumentTemplateBAL _documentTemplateBAL;
        private readonly IJobSchedulingBAL _jobScheduling;
        private readonly IResellerBAL _resellerService;

        private readonly ICreateJobBAL _job;
        private readonly IDashboardBAL _dashboard;
        private readonly IGBsettingsBAL _gbSettingsBAL;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="JobSettingController" /> class.
        /// </summary>
        /// <param name="jobSettingBAL">The job setting bal.</param>
        /// <param name="documentTemplateBAL">The document template bal.</param>
        /// <param name="job">The job.</param>
        public JobSettingController(IJobSettingBAL jobSettingBAL, IDocumentTemplateBAL documentTemplateBAL, ICreateJobBAL job, IDashboardBAL dashboardBAL, IGBsettingsBAL gbSettingsBAL, IResellerBAL resellerService)

        {
            this._jobSettingBAL = jobSettingBAL;
            this._documentTemplateBAL = documentTemplateBAL;
            this._job = job;
            this._dashboard = dashboardBAL;
            this._gbSettingsBAL = gbSettingsBAL;
            this._resellerService = resellerService;
        }


        #endregion


        #region Events

        [HttpGet]
        [UserAuthorization]
        public ActionResult JobSetting()
        {
            JobSetting objJobSetting = new JobSetting();
            objJobSetting.solarElectrician = new List<SolarElectricianView>();

            return View();
        }

        public void JobCustomFieldList(string name = "",int solarcompanyId = 0)
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);
            IList<JobCustomField> lstCustomField = new List<JobCustomField>();

            int? companyId = 0;
            if (ProjectSession.UserTypeId == 4)
                companyId = ProjectSession.SolarCompanyId;
            else
                companyId = null;

            lstCustomField = _jobSettingBAL.JobCustomFieldList(pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, name,solarcompanyId == 0 ? ProjectSession.SolarCompanyId : solarcompanyId);

            if (lstCustomField.Count > 0)
            {
                gridParam.TotalDisplayRecords = lstCustomField.FirstOrDefault().TotalRecords;
                gridParam.TotalRecords = lstCustomField.FirstOrDefault().TotalRecords;
            }

            HttpContext.Response.Write(Grid.PrepareDataSet(lstCustomField, gridParam));
        }

        [HttpPost]
        public JsonResult SaveCustomField(JobCustomField jobCustomField)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int customFieldId = _jobSettingBAL.JobCustomFieldInsertUpdate(jobCustomField);
                    return Json(new { status = true, id = customFieldId }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string msg = string.Empty;
                    ModelState.Values.AsEnumerable().ToList().ForEach(d =>
                    {
                        if (d.Errors.Count > 0)
                            msg = d.Errors[0].ErrorMessage;
                    });
                    return Json(new { status = false, error = msg }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult CustomFieldDelete(string jobCustomFieldIds)
        {
            if (ProjectSession.LoggedInUserId == 0)
                return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);

            try
            {
                _jobSettingBAL.DeleteJobCustomField(jobCustomFieldIds);
                return Json(new { status = true, id = 1 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, error = ex.Message, id = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult GetCustomField(int jobCustomFieldId)
        {
            if (ProjectSession.LoggedInUserId == 0)
                return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);

            try
            {
                JobCustomField jobCustomField = _jobSettingBAL.GetCustomField(jobCustomFieldId);
                return Json(new { status = true, data = jobCustomField }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, error = ex.Message, id = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetAllCustomFieldOfCompany()
		{
            List<SelectListItem> Items = _jobSettingBAL.GetAllCustomFieldOfCompany().Select(a => new SelectListItem { Text = a.CustomField, Value = a.JobCustomFieldId.ToString() }).ToList();
            return Json(Items, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveDefault(DefaultSetting request, bool GlobalisAllowedSPV = false,bool IsAllowedAccessToAllUsers = false)
        {
            if (ProjectSession.LoggedInUserId == 0)
                return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);
            try
            {
                _jobSettingBAL.UpdateDefaultSetting(Convert.ToBoolean(request.IsPreapproval), Convert.ToBoolean(request.IsConnection), Convert.ToBoolean(request.IsAllowTrade),Convert.ToBoolean(request.IsCreateJobNotification));
                if (ProjectSession.UserTypeId == 1)
                {
                    _gbSettingsBAL.SetGBsettingValueByKeyName(ProjectConfiguration.IsAllowedAccessToAllUsersKeyName, IsAllowedAccessToAllUsers.ToString(), ProjectSession.LoggedInUserId);
                }
                

                //when FSA user change the value of global leval SPV flag value then set cache for STCSubmission page and that flag value in db
                string globalisAllowedSPVFromDB = _gbSettingsBAL.GetGBsettingValueByKeyName(ProjectConfiguration.GlobalLevelSpvRequiredKeyName).Value;
                if((globalisAllowedSPVFromDB.ToLower() != GlobalisAllowedSPV.ToString().ToLower()) &&(ProjectSession.UserTypeId==1))
                {
                    _gbSettingsBAL.SetGBsettingValueByKeyName(ProjectConfiguration.GlobalLevelSpvRequiredKeyName, GlobalisAllowedSPV.ToString(), ProjectSession.LoggedInUserId);
                    //List<int> lstResellerId = _resellerService.GetData(null).Select(x => x.ResellerID).ToList();
                    //foreach (var resellerid in lstResellerId)
                    //    CommonBAL.SetCacheDataForSTCSubmissionFromSolarCompanyId(resellerid);
                }
                
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult SaveDefaultForJob(DefaultSetting request)
        {
            if (ProjectSession.LoggedInUserId == 0)
                return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);
            try
            {
                _jobSettingBAL.SaveDefaultForJob(Convert.ToBoolean(request.IsPreapproval), Convert.ToBoolean(request.IsConnection), Convert.ToInt32(request.JobId));
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetDefaultSettings()
        {
            if (ProjectSession.LoggedInUserId == 0)
                return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);
            try
            {
                DefaultSetting data = new DefaultSetting();
                DataSet ds= _jobSettingBAL.GetDefaultSettings();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    data.IsAllowTrade = Convert.ToBoolean(dr["IsAllowTrade"]);
                    data.IsConnection = Convert.ToBoolean(dr["IsConnection"]);
                    data.IsPreapproval = Convert.ToBoolean(dr["IsPreapproval"]);
                    data.LastSyncDate = (dr["LastSyncDate"]).ToString();
                }
                if(ds.Tables[1].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[1].Rows[0];
                    data.IsCreateJobNotification = Convert.ToBoolean(dr["IsCreateJobNotification"]);
                }
                data.GlobalisAllowedSPV = Convert.ToBoolean(_gbSettingsBAL.GetGBsettingValueByKeyName(ProjectConfiguration.GlobalLevelSpvRequiredKeyName).Value);
                data.IsAllowedAccessToAllUsers = Convert.ToBoolean(_gbSettingsBAL.GetGBsettingValueByKeyName(ProjectConfiguration.IsAllowedAccessToAllUsersKeyName).Value);
                return Json(new { status = true, data = Newtonsoft.Json.JsonConvert.SerializeObject(data) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetDefaultSettingsForJob(int JobId)
        {
            if (ProjectSession.LoggedInUserId == 0)
                return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);
            try
            {
                DefaultSetting data = _jobSettingBAL.GetDefaultSettingsForJob(JobId);
                return Json(new { status = true, data = Newtonsoft.Json.JsonConvert.SerializeObject(data) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Opens the document template.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult OpenDocumentTemplate(string id)
        {
            try
            {
                if (ProjectSession.LoggedInUserId == 0)
                    return Redirect(Url.Action("Logout", "Account"));

                int docTemplateID = 0;
                if (!string.IsNullOrEmpty(id))
                    int.TryParse(QueryString.GetValueFromQueryString(id, "id"), out docTemplateID);

                DocumentTemplate documentTemplate = _documentTemplateBAL.GetDocumentTemplate(docTemplateID);

                string documentFullPath = ProjectSession.ProofDocumentsURL + documentTemplate.Path;

                List<SelectListItem> jobFields = _job.GetJobFieldData().Select(a => new SelectListItem { Text = a.FieldLabel, Value = a.PropertyName }).ToList();

                documentTemplate.JobFields = jobFields;

                if (System.IO.File.Exists(documentFullPath))
                {
                    string p = (ProjectSession.UploadedDocumentPath + documentTemplate.Path).Replace(@"\", @"/");
                    documentTemplate.PdfItems = GetPDFItems(documentFullPath);
                    documentTemplate.PDFURL = p;
                    documentTemplate.PDFSource = documentFullPath.Replace("\\", "/").Replace(@"\", "/");
                    documentTemplate.Data = documentTemplate.ParsedData;
                    documentTemplate.Data = documentTemplate.Data.Replace(@"\\", @"\").Replace(@"\", @"\\").Replace(System.Environment.NewLine, @"<br />");
                }
                return View(documentTemplate);
            }
            catch (Exception ex)
            {
                //WriteToLogFile(ex.Message);
                return Redirect(Url.Action("Logout", "Account"));
            }
        }

        private List<PdfItems> GetPDFItems(string fileName)
        {
            string pdfTemplate = fileName;
            bool isError = false;
        ReadAgain:
            isError = false;
            MemoryStream memStream = new MemoryStream();
            using (FileStream fileStream = System.IO.File.OpenRead(pdfTemplate))
            {
                memStream.SetLength(fileStream.Length);
                fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);
            }

            string newFile = fileName;
            PdfReader pdfReader = null;
            List<PdfItems> lstPdfItems = new List<PdfItems>();

            //tempcode
            PdfReader reader = new PdfReader(fileName);
            string tempfile = fileName.Replace(Path.GetFileNameWithoutExtension(fileName), Path.GetFileNameWithoutExtension(fileName) + "_temp");
            var out1 = System.IO.File.Open(tempfile, FileMode.Create, FileAccess.Write);
            PdfStamper stamp = new PdfStamper(reader, out1);

            try
            {
                pdfReader = new PdfReader(memStream);
                AcroFields af = pdfReader.AcroFields;
                StringBuilder sb = new StringBuilder();
                foreach (var field in af.Fields)
                {
                    //tempcode
                    iTextSharp.text.Rectangle rect = new iTextSharp.text.Rectangle(100, 100, 350, 450);

                    PdfItems k = new PdfItems(lstPdfItems.Count, field.Key, af.GetField(Convert.ToString(field.Key)), af.GetFieldType(Convert.ToString(field.Key)));
                    if (k.Type == (int)FormBot.Helper.SystemEnums.InputTypes.RADIO_BUTTON || k.Type == (int)FormBot.Helper.SystemEnums.InputTypes.CHECK_BOX || k.Type == (int)FormBot.Helper.SystemEnums.InputTypes.LIST_BOX || k.Type == (int)FormBot.Helper.SystemEnums.InputTypes.COMBO_BOX)
                    {
                        try
                        {
                            ////tempcode
                            //RadioCheckField radioG = new RadioCheckField(stamp.Writer, rect, Convert.ToString(field.Key), "");
                            //af.DecodeGenericDictionary(af.GetFieldItem(Convert.ToString(field.Key)).GetMerged(0), radioG);

                            k.AvailableValues.AddRange(GetCheckBoxExportValue(af, Convert.ToString(field.Key)));
                        }
                        catch (Exception ex)
                        {
                            Helper.Log.WriteError(ex);
                        }
                    }
                    else if (k.Type == (int)FormBot.Helper.SystemEnums.InputTypes.TEXT_FIELD)
                    {
                        try
                        {

                            //tempcode
                            TextField tx = new TextField(stamp.Writer, rect, Convert.ToString(field.Key));
                            af.DecodeGenericDictionary(af.GetFieldItem(Convert.ToString(field.Key)).GetMerged(0), tx);
                            AcroFields.Item fieldItem = af.GetFieldItem(Convert.ToString(field.Key));
                            PdfDictionary pdfDictionary = (PdfDictionary)fieldItem.GetWidget(0);
                            if (pdfDictionary.GetAsNumber(PdfName.MAXLEN) != null)
                            {
                                int maxFieldLength = Int32.Parse(pdfDictionary.GetAsNumber(PdfName.MAXLEN).ToString());
                                k.PdfItemProperties.MaxLength = maxFieldLength;
                            }

                            if (tx.Options == TextField.MULTILINE)
                                k.IsTextArea = true;

                            if (tx.TextColor != null)
                                k.PdfItemProperties.TextColor = string.Format("rgb({0}, {1}, {2})", tx.TextColor.R, tx.TextColor.G, tx.TextColor.B);

                            if (tx.BackgroundColor != null)
                                k.PdfItemProperties.BackgroundColor = string.Format("rgb({0}, {1}, {2})", tx.BackgroundColor.R, tx.BackgroundColor.G, tx.BackgroundColor.B);

                            if (tx.BorderColor != null)
                                k.PdfItemProperties.BorderColor = string.Format("rgb({0}, {1}, {2})", tx.BorderColor.R, tx.BorderColor.G, tx.BorderColor.B);


                            if (tx.Alignment == 0)
                            {
                                k.PdfItemProperties.Alignment = "left";
                            }
                            else if (tx.Alignment == 2)
                            {
                                k.PdfItemProperties.Alignment = "right";
                            }
                            else if (tx.Alignment == 1)
                            {
                                k.PdfItemProperties.Alignment = "center";
                            }
                            k.PdfItemProperties.FontSize = float.Parse((tx.FontSize * 1.3333333333333333).ToString());

                            if (tx.Font != null)
                            {
                                string[] fontProperty = tx.Font.PostscriptFontName.Split('-');

                                if (fontProperty.Count() > 0)
                                    k.PdfItemProperties.FontName = fontProperty[0];

                                if (fontProperty.Count() > 1)
                                {
                                    if (tx.Font.PostscriptFontName.IndexOf("Bold") >= 0)
                                        k.PdfItemProperties.Bold = true;

                                    if (tx.Font.PostscriptFontName.IndexOf("Oblique") >= 0 || tx.Font.PostscriptFontName.IndexOf("Italic") >= 0)
                                        k.PdfItemProperties.Italic = true;
                                }
                            }

                            if (tx.Options == TextField.MULTILINE)
                                k.IsTextArea = true;
                            if (tx.Options == 1)
                            {
                                k.ReadOnly = true;

                                if (pdfDictionary.GetAsString(PdfName.DV) != null)
                                {
                                    int aspectRatio = 0;
                                    string[] position = pdfDictionary.GetAsString(PdfName.DV).ToString().Split('_');

                                    if (position.Count() > 0 && Enum.IsDefined(typeof(SystemEnums.HorizontalAlignment), position[0]))
                                        k.PdfItemProperties.HoriAlign = position[0];

                                    if (position.Count() > 1 && Enum.IsDefined(typeof(SystemEnums.VerticalAlignment), position[1]))
                                        k.PdfItemProperties.VertAlign = position[1];

                                    if (position.Count() > 2 && int.TryParse(position[2], out aspectRatio) && (aspectRatio == 0 || aspectRatio == 1))
                                        k.PdfItemProperties.AspectRatio = Convert.ToInt32(position[2]);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Helper.Log.WriteError(ex);
                        }
                    }
                    else if (k.Type == (int)FormBot.Helper.SystemEnums.InputTypes.BUTTON)
                    {
                        PushbuttonField tx = af.GetNewPushbuttonFromField(Convert.ToString(field.Key));
                        af.DecodeGenericDictionary(af.GetFieldItem(Convert.ToString(field.Key)).GetMerged(0), tx);
                        k.Value = tx.Text;
                        k.IsImageField = tx.BorderWidth > 0 ? true : false;
                        if (k.IsImageField)
                        {
                            if(tx.Text!= null && tx.Text.Split(',').ToList().Count > 1)
                            {
                                if (tx.IconHorizontalAdjustment != 0.5)
                                {
                                    k.PdfItemProperties.FillOption = tx.IconHorizontalAdjustment * 10000;
                                }
                                if (tx.IconVerticalAdjustment != 0.5)
                                    k.PdfItemProperties.AspectRatio = tx.IconVerticalAdjustment * 10000;
                                else
                                    k.PdfItemProperties.AspectRatio = 1;
                            }
                            k.PdfItemProperties.ImageAlign = tx.FontSize;
                        }
                        if (tx.Options == 1)
                        {
                            k.ReadOnly = true;
                        }
                        if(tx.IconVerticalAdjustment == 1)
                        {
                            k.IsDraw = true;
                        }
                        k.lineWidth = tx.IconHorizontalAdjustment * 10;
                        //k.PdfItemProperties.AspectRatio = tx.FontSize;
                        //k.PdfItemProperties.ImageHeight = Convert.ToInt32(tx.IconHorizontalAdjustment * 10000);
                        //k.PdfItemProperties.ImageWidth = Convert.ToInt32(tx.IconVerticalAdjustment * 10000);
                        //if(tx.Rotation > 0)
                        //{
                        //    k.PdfItemProperties.FitToSize = 0;
                        //}
                        //else
                        //{
                        //    k.PdfItemProperties.FitToSize = Convert.ToInt32(tx.Rotation) / 90;
                        //}
                        if (tx.BackgroundColor != null)
                            k.PdfItemProperties.BackgroundColor = string.Format("rgb({0}, {1}, {2})", tx.BackgroundColor.R, tx.BackgroundColor.G, tx.BackgroundColor.B);
                        if (tx.BorderColor != null)
                            k.PdfItemProperties.BorderColor = string.Format("rgb({0}, {1}, {2})", tx.BorderColor.R, tx.BorderColor.G, tx.BorderColor.B);

                        AcroFields.Item fieldItem = af.GetFieldItem(Convert.ToString(field.Key));
                        PdfDictionary pdfDictionary = (PdfDictionary)fieldItem.GetWidget(0);
                    }

                    if (k.Type == (int)FormBot.Helper.SystemEnums.InputTypes.CHECK_BOX)
                    {
                        string a = af.GetField(Convert.ToString(field.Key));
                    }

                    lstPdfItems.Add(k);
                }

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Input string was not in a correct format.") || ex.Message.Contains("Original message: PDF startxref not found"))
                {
                    isError = true;
                    System.IO.File.AppendAllText(pdfTemplate, System.IO.File.ReadAllText(pdfTemplate).Substring(0, System.IO.File.ReadAllText(pdfTemplate).LastIndexOf("%%EOF") + 5));
                    goto ReadAgain;
                }
                Helper.Log.WriteError(ex);
            }

            finally
            {
                if (!isError)
                {
                    pdfReader.Close();
                    memStream.Close();
                    memStream.Dispose();
                    stamp.Close();
                    stamp.Dispose();
                    reader.Close();
                    reader.Dispose();
                    System.IO.File.Delete(tempfile);
                }
            }
            return lstPdfItems;
        }

        public string[] GetCheckBoxExportValue(AcroFields fields, string cbFieldName)
        {
            AcroFields.Item fd = ((AcroFields.Item)fields.GetFieldItem(cbFieldName));
            var vals = fd.GetValue(0);
            Hashtable names = new Hashtable();
            string[] outs = new string[fd.Size];
            PdfDictionary pd = ((PdfDictionary)fd.GetWidget(0)).GetAsDict(PdfName.AP);
            for (int k1 = 0; k1 < fd.Size; ++k1)
            {
                PdfDictionary dic = (PdfDictionary)fd.GetWidget(k1);
                dic = dic.GetAsDict(PdfName.AP);
                if (dic == null)
                    continue;
                dic = dic.GetAsDict(PdfName.N);
                if (dic == null)
                    continue;
                foreach (PdfName pname in dic.Keys)
                {
                    String name = PdfName.DecodeName(pname.ToString());
                    if (name.ToLower() != "off")
                    {
                        names[name] = null;
                        outs[(outs.Length - k1) - 1] = name;
                    }
                }
            }
            return outs;
        }

        [HttpPost]
        public JsonResult SaveNotification(SolarCompanyNotification solarCompanyNotification)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int notificationId = _jobSettingBAL.SolarCompanyNotification_InsertUpdate(solarCompanyNotification);
                    DateTime dt = DateTime.Now;
                    string CacheKey = string.Format(CachingKeys.UserWiseSnackBarNotificationList, ProjectSession.LoggedInUserId);

                    if (CacheConfiguration.IsContainsKey(CacheKey))
                    {
                        CacheConfiguration.Remove(string.Format(CachingKeys.UserWiseSnackBarNotificationList, ProjectSession.LoggedInUserId));

                    }

                    return Json(new { status = true, id = notificationId }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string msg = string.Empty;
                    ModelState.Values.AsEnumerable().ToList().ForEach(d =>
                    {
                        if (d.Errors.Count > 0)
                            msg = d.Errors[0].ErrorMessage;
                    });
                    return Json(new { status = false, error = msg }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public void SolarCompanyNotificationList(string name = "")
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);
            IList<SolarCompanyNotification> lstSolarCompanyNotification = new List<SolarCompanyNotification>();


            lstSolarCompanyNotification = _jobSettingBAL.SolarCompanyNotificationList(pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, name);

            if (lstSolarCompanyNotification.Count > 0)
            {
                gridParam.TotalDisplayRecords = lstSolarCompanyNotification.FirstOrDefault().TotalRecords;
                gridParam.TotalRecords = lstSolarCompanyNotification.FirstOrDefault().TotalRecords;
            }

            HttpContext.Response.Write(Grid.PrepareDataSet(lstSolarCompanyNotification, gridParam));
        }


        [HttpGet]
        public ActionResult GetNotification(int notificationId)
        {
            if (ProjectSession.LoggedInUserId == 0)
                return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);

            try
            {
                SolarCompanyNotification notification = _jobSettingBAL.GetNotification(notificationId);
                notification.StrExpiryDate = notification.ExpiryDate.ToString("yyyy/MM/dd");
                return Json(new { status = true, data = notification }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, error = ex.Message, id = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult DeleteNotification(string notificationIds)
        {
            try
            {
                _jobSettingBAL.DeleteNotification(notificationIds);
                return this.Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public PartialViewResult PreviewDocument(string docTemplateId,bool isWithoutBg,bool isWithoutBorder = false)
        {
            int documentTemplateId = 0;
            if (!string.IsNullOrEmpty(docTemplateId))
            {
                documentTemplateId = Convert.ToInt32(docTemplateId);
            }
            DocumentTemplate documentTemplate = _documentTemplateBAL.GetDocumentTemplate(documentTemplateId);
            string path = Path.Combine(ProjectSession.ProofDocumentsURL + documentTemplate.Path);
            if (System.IO.File.Exists(path))
            {
                
                string p = (ProjectSession.UploadedDocumentPath + documentTemplate.Path).Replace(@"\", @"/");
                documentTemplate.PdfItems = GetPDFItems(path);
                documentTemplate.PDFURL = p;
                documentTemplate.PDFSource = path.Replace("\\", "/").Replace(@"\", "/");
                documentTemplate.Data = documentTemplate.ParsedData;
                documentTemplate.Data = documentTemplate.Data.Replace(@"\\", @"\").Replace(@"\", @"\\").Replace(System.Environment.NewLine, @"<br />");
            }
            return PartialView("DocumentPreview",documentTemplate);
        }

        public ActionResult DownloadDocument(string docTemplateId)
        {
            int documentTemplateId = 0;
            if (!string.IsNullOrEmpty(docTemplateId))
            {
                documentTemplateId = Convert.ToInt32(docTemplateId);
            }
            DocumentTemplate documentTemplate = _documentTemplateBAL.GetDocumentTemplate(documentTemplateId);
            string path = Path.Combine(ProjectSession.ProofDocumentsURL + documentTemplate.Path);
            if (System.IO.File.Exists(path))
            {

                string p = (ProjectSession.UploadedDocumentPath + documentTemplate.Path).Replace(@"\", @"/");
                documentTemplate.PdfItems = GetPDFItems(path);
                documentTemplate.PDFURL = p;
                documentTemplate.PDFSource = path.Replace("\\", "/").Replace(@"\", "/");
                documentTemplate.Data = documentTemplate.ParsedData;
                documentTemplate.Data = documentTemplate.Data.Replace(@"\\", @"\").Replace(@"\", @"\\").Replace(System.Environment.NewLine, @"<br />");
            }
            var fileData = System.IO.File.ReadAllBytes(path);
            Response.Clear();
            Response.ClearHeaders();
            Response.ClearContent();
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition", string.Format("attachment; filename = \"{0}\"", Path.GetFileName(path)));
            Response.BinaryWrite(fileData);
            Response.End();
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SavePushNotification(PushNotificationsSend pushNotification)
        {
            try
            {
                if (ModelState.IsValid)
                {


                    var syncTask = new Task(() =>
                    {
                        new PushNotification().SendPushNotificationForFilteredUser(pushNotification);
                    });
                    syncTask.RunSynchronously();


                    _jobSettingBAL.PushNotificationInsert(pushNotification);
                    //var syncTask = new Task(() =>
                    //{
                    //    new PushNotification().SendPushNotificationForAll(pushNotification.Notification);
                    //});
                    //syncTask.RunSynchronously();
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string msg = string.Empty;
                    ModelState.Values.AsEnumerable().ToList().ForEach(d =>
                    {
                        if (d.Errors.Count > 0)
                            msg = d.Errors[0].ErrorMessage;
                    });
                    return Json(new { status = false, error = msg }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public void PushNotificationList(string name = "")
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);
            IList<PushNotificationsSend> lstPushNotification = new List<PushNotificationsSend>();


            lstPushNotification = _jobSettingBAL.PushNotificationsSendList(pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, name);
            if (lstPushNotification.Any())
                lstPushNotification.ToList().ForEach(x =>
                {
                    x.StrCreateDate = x.CreatedDate.ToShortDateString();
                });

            if (lstPushNotification.Count > 0)
            {
                gridParam.TotalDisplayRecords = lstPushNotification.FirstOrDefault().TotalRecords;
                gridParam.TotalRecords = lstPushNotification.FirstOrDefault().TotalRecords;
            }

            HttpContext.Response.Write(Grid.PrepareDataSet(lstPushNotification, gridParam));
        }


        #endregion
        
        [HttpPost]

        public string GetNotifications(DateTime dt)
        {
            string CacheKey = string.Format(CachingKeys.UserWiseSnackBarNotificationList, ProjectSession.LoggedInUserId);
            if (CacheConfiguration.IsContainsKey(CacheKey))
            {
                return JsonConvert.SerializeObject(CommonBAL.GetCacheDataForSnackbar<string>(CacheKey));
            }
            else
            {
                DataSet ds1 = _dashboard.GetNotifications(dt);
                CommonBAL.SetCacheDataForSnackbar(CacheKey, ds1.Tables[0]);
                return JsonConvert.SerializeObject(ds1.Tables[0]);
            }
        }
        [HttpPost]

        public JsonResult SetsnackbarSession(string SnackbarId)
        {
            ProjectSession.SnackbarId += string.IsNullOrEmpty(ProjectSession.SnackbarId) ? SnackbarId : "," + SnackbarId;
            return new JsonResult() { Data = true };
        }

        [HttpGet]
        public JsonResult GetSolarCompanyByResellerId(string id)
        {
            int ID = (id != "null" && !string.IsNullOrEmpty(id)) ? Convert.ToInt32(id) : 0;
            List<SelectListItem> items = _jobSettingBAL.GetSolarCompanyByResellerID(ID).Select(a => new SelectListItem { Text = a.CompanyName, Value = a.SolarCompanyId.ToString() }).ToList();
            var selectAll = new SelectListItem()
            {
                Value = "0",
                Text = "All"
            };
            items.Insert(0, selectAll);
            //items.Add(new SelectListItem() { Value = "-1", Text = "All" });
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetElectricianOfAllType(int JobType, int SolarCompanyId = 0, int ResellerId = 0)
        {
            List<SelectListItem> items = _jobSettingBAL.GetElectricianOfAllType(JobType, SolarCompanyId, ResellerId).Select(a => new SelectListItem { Text = a.Name, Value = a.UserId.ToString() }).ToList();

            var selectAll = new SelectListItem()
            {
                Value = "0",
                Text = "All"
            };
            items.Insert(0, selectAll);
            return Json(items, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetContractor(int JobType, int SolarCompanyId = 0, int ResellerId = 0)
        {

            List<SelectListItem> lstcontactor = _jobSettingBAL.GetContractor(JobType, SolarCompanyId, ResellerId).Select(a => new SelectListItem { Text = a.Name, Value = a.UserId.ToString() }).ToList();
            var selectAll = new SelectListItem()
            {
                Value = "0",
                Text = "All"
            };
            lstcontactor.Insert(0, selectAll);
            // IList<User> lstcontactor = new List<User>();
            //lstcontactor= _jobSettingBAL.GetContractor(SolarCompanyId, ResllerId).ToList();
            return Json(lstcontactor, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// Get ResellerID and ResellerName.
        /// </summary>
        /// <returns>Returns List of Reseller</returns>
        [HttpGet]
        public JsonResult GetAllResellerForCache(bool IsPeakPay = false)
        {
            List<SelectListItem> items = _resellerService.GetData(null, IsPeakPay).Select(a => new SelectListItem { Text = a.ResellerName, Value = a.ResellerID.ToString() }).ToList();
            var selectAll = new SelectListItem()
            {
                Value = "0",
                Text = "All"
            };
            items.Insert(0, selectAll);
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get ResellerIDs and BatchName.
        /// </summary>
        /// <param name="IsPeakPay"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetAllResellerBatchForCache(bool IsPeakPay = false)
        {
            List<SelectListItem> items = _resellerService.GetBatchData().Select(a => new SelectListItem { Text = string.Format("{0} - ({1})", a.BatchName, a.ResellerIDs), Value = a.ResellerIDs.ToString() }).ToList();
            var selectAll = new SelectListItem()
            {
                Value = "0",
                Text = "Select"
            };
            items.Insert(0, selectAll);
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAllowAcessLog() 
        {
            List<JobSettingLog> lstJobSettingLog = _jobSettingBAL.GetJobSettingLogs();
            for (int i = 0; i < lstJobSettingLog.Count; i++) {
                lstJobSettingLog[i].strModifiedDate = Convert.ToDateTime(lstJobSettingLog[i].ModifiedDate).ToString("dd/MM/yyy hh:mm tt");
            }
            return Json(lstJobSettingLog, JsonRequestBehavior.AllowGet);
        }

    }
}
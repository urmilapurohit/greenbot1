using FormBot.BAL;
using FormBot.BAL.Service;
using FormBot.BAL.Service.CheckList;
using FormBot.BAL.Service.Documents;
using FormBot.BAL.Service.Job;
using FormBot.BAL.Service.SPV;
using FormBot.Entity;
using FormBot.Entity.Documents;
using FormBot.Entity.Email;
using FormBot.Entity.Job;
using FormBot.Entity.Pdf;
using FormBot.Helper;
using FormBot.Helper.Helper;
using FormBot.WebAPI.Models;
using iTextSharp.text.pdf;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;
using Newtonsoft.Json;
using PdfSharp;
using PdfSharp.Drawing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Http;
using System.Web.Security;

namespace FormBot.WebAPI.Controllers
{
    public class AccountController : ApiController
    {
        private readonly IUserBAL _user;
        private readonly ILoginBAL _login;
        private readonly IEmailBAL _emailBAL;
        private readonly ICreateJobBAL _createJobBAL;
        private readonly IDocumentsBAL _documentsBAL;
        private readonly IJobSchedulingBAL _scheduleBAL;
        private readonly ICheckListItemBAL _checkListBAL;
        private readonly IGBsettingsBAL _gbSettingBAL;
        private readonly ISpvLogBAL _spvLog;

        public AccountController()
            : this(new UserBAL(), new LoginBAL(), new EmailBAL(), new CreateJobBAL(), new DocumentsBAL(), new JobSchedulingBAL(), new CheckListItemBAL(), new GBsettingsBAL(),new SpvLogBAL())
        {
        }

        public AccountController(IUserBAL user, ILoginBAL login, IEmailBAL email, ICreateJobBAL createJob, IDocumentsBAL documentBAL, IJobSchedulingBAL scheduleBAL, ICheckListItemBAL checkListBAL, IGBsettingsBAL gbSettingsBAL, ISpvLogBAL spvLog)
        {
            this._user = user;
            this._login = login;
            this._emailBAL = email;
            this._createJobBAL = createJob;
            this._documentsBAL = documentBAL;
            this._scheduleBAL = scheduleBAL;
            this._checkListBAL = checkListBAL;
            this._gbSettingBAL = gbSettingsBAL;
            this._spvLog = spvLog;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        /// <summary>
        /// Reads the image file.
        /// </summary>
        /// <param name="imageLocation">The image location.</param>
        /// <returns>static byte</returns>
        public static byte[] ReadImageFile(string imageLocation)
        {
            byte[] imageData = null;
            FileInfo fileInfo = new FileInfo(imageLocation);
            long imageFileLength = fileInfo.Length;
            FileStream fs = new FileStream(imageLocation, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            imageData = br.ReadBytes((int)imageFileLength);
            return imageData;
        }

        #region Event

        /// <summary>
        /// Logins the specified login request.
        /// </summary>
        /// <param name="loginReq">login request class.</param>
        /// <returns>Service Response</returns>
        [HttpPost]
        public ServiceResponse Login(LoginRequest loginReq)
        {
            try
            {
                Log.WriteLog(JsonConvert.SerializeObject(loginReq));
                using (var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
                {
                    var user = userManager.Find(loginReq.Username, loginReq.Password);
                    if (user != null)
                    {
                        var userDetail = _user.GetUserByAspnetUserId(user.Id);
                        if (userDetail != null)
                        {
                            if ((userDetail.UserTypeID == 7) && (!userDetail.IsActive || userDetail.Status != (int)SystemEnums.UserStatus.Approved))
                            {
                                return new ServiceResponse() { Error = "Your account is currently inactive.", Status = false };
                            }
                            else if (userDetail.UserTypeID == 9 && !userDetail.IsActive)
                            {
                                return new ServiceResponse() { Error = "Your account is currently inactive.", Status = false };
                            }
                            else if (userDetail.UserTypeID == 7 || userDetail.UserTypeID == 9)
                            {
                                string token = _user.UpdateDeviceToken(userDetail.UserId, loginReq.AccessToken, loginReq.Type, loginReq.DeviceInfo);
                                return new ServiceResponse() { Data = HelperMethods.ConvertToJSON(userDetail), Status = true, ApiToken = token };
                            }
                            else
                            {
                                return new ServiceResponse() { Error = "Only Solar Electricians and Contractors can login.", Status = false };
                            }

                        }

                    }
                    else
                    {
                        return new ServiceResponse() { Error = "Invalid username or password.", Status = false };
                    }

                }

            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { Error = "Invalid username or password.", ExceptionMessage = ex.Message, Status = false };
            }

            return new ServiceResponse() { Error = "Error occured while logged in.", ExceptionMessage = "", Status = false };
        }

        /// <summary>
        /// Updates the access token.
        /// </summary>
        /// <param name="loginReq">The login request.</param>
        /// <returns>Service Response</returns>
        [HttpPost]
        public ServiceResponse UpdateAccessToken(LoginRequest loginReq)
        {
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(loginReq.UserID), loginReq.ApiToken))
                {
                    _user.UpdateDeviceToken(Convert.ToInt32(loginReq.UserID), loginReq.AccessToken, loginReq.Type, loginReq.DeviceInfo);
                    return new ServiceResponse() { Status = true };
                }
                else
                    return new ServiceResponse() { Status = false, Error = "Invalid Token" };
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { Error = "invalid token.", ExceptionMessage = ex.Message, Status = false };
            }
        }

        /// <summary>
        /// Updates the allow access flag.
        /// </summary>
        /// <param name="loginReq">The login request.</param>
        /// <returns>Service Response</returns>
        [HttpPost]
        public ServiceResponse UpdateDeviceAllowAccessTimmer(LoginRequest loginReq)
        {
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(loginReq.UserID), loginReq.ApiToken))
                {
                    _user.UpdateDeviceAllowAccessTimmer(Convert.ToInt32(loginReq.UserID), loginReq.AccessToken, loginReq.DeviceInfo);
                    return new ServiceResponse() { Status = true };
                }
                else
                    return new ServiceResponse() { Status = false, Error = "Invalid Token" };
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { Error = "invalid token.", ExceptionMessage = ex.Message, Status = false };
            }
        }

        /// <summary>
        /// Forgot password.
        /// </summary>
        /// <param name="forgotPassword">forgot password class</param>
        /// <returns>Forgot Password Request class</returns>
        [HttpPost]
        public ServiceResponse ForgotPassword(ForgotPasswordRequest forgotPassword)
        {
            try
            {
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var user = userManager.FindByName(forgotPassword.Username);
                if (user == null)
                {
                    return new ServiceResponse() { Error = "Please check your Username.", Status = false };
                }

                var userDetail = _user.GetUserByAspnetUserId(user.Id);
                ForgotPasswordResponse passwordResp = new ForgotPasswordResponse();
                if (userDetail.UserTypeID == 7 || userDetail.UserTypeID == 9)
                {
                    SendEmailForForgotPassword(userDetail.UserId);
                    passwordResp.ForgotPasswordStatus = SystemEnums.ServiceResult.Success.ToString();
                    return new ServiceResponse() { Data = HelperMethods.ConvertToJSON(passwordResp), Status = true };
                }
                else
                {
                    return new ServiceResponse() { Error = "Only Solar Electricians and Contractors can access forgot password.", Status = false };
                }
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { ExceptionMessage = ex.Message, Status = false };
            }
        }

        /// <summary>
        /// Jobs the schedule.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Service Response</returns>
        [HttpPost]
        public ServiceResponse JobSchedule(ServiceRequest request)
        {
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(request.UserID), request.ApiToken))
                {
                    DataSet dataset = _scheduleBAL.GetJobSchedulingByUserTypeForAPI(Convert.ToInt32(request.UserID), Convert.ToInt32(request.UserTypeID), Convert.ToInt32(request.CompanyID), !string.IsNullOrEmpty(request.JobId) ? Convert.ToInt32(request.JobId) : 0);
                    if (dataset.Tables.Count > 0)
                    {
                        DataTable dt = new DataTable();
                        foreach (DataRow item in dataset.Tables[0].Rows)
                        {
                            item["JobStage"] = !string.IsNullOrEmpty(item["JobStage"].ToString()) ? item["JobStage"].ToString().Trim() : "";
                            item["strVisitStartDate"] = !string.IsNullOrEmpty(item["strVisitStartDate"].ToString()) ? Convert.ToDateTime(item["strVisitStartDate"].ToString()).Ticks.ToString() : "";
                            item["strVisitStartTime"] = !string.IsNullOrEmpty(item["strVisitStartDate"].ToString()) ? Convert.ToDateTime(item["strVisitStartTime"].ToString()).Ticks.ToString() : "";
                            item["strVisitEndDate"] = !string.IsNullOrEmpty(item["strVisitEndDate"].ToString()) ? Convert.ToDateTime(item["strVisitEndDate"].ToString()).Ticks.ToString() : "";
                            item["strVisitEndTime"] = !string.IsNullOrEmpty(item["strVisitEndTime"].ToString()) ? Convert.ToDateTime(item["strVisitEndTime"].ToString()).Ticks.ToString() : "";
                            item["NoOfPanel"] = !string.IsNullOrEmpty(item["NoOfPanel"].ToString()) ? item["NoOfPanel"] : 0;
                            //item["VisitStatus"] = !string.IsNullOrEmpty(Convert.ToString(item["VisitStatus"])) ? item["VisitStatus"].ToString() : "";
                        }
                        return new ServiceResponse() { Data = HelperMethods.ConvertToJSON(dataset.Tables[0]), Status = true };
                    }
                    return new ServiceResponse() { Status = true };
                }

                return new ServiceResponse() { Status = false, Error = "Invalid Token" };
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { ExceptionMessage = ex.Message, Status = false };
            }
        }

        /// <summary>
        /// Jobs the documents.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Service Response</returns>
        [HttpPost]
        public ServiceResponse JobDocuments(DocumentRequest request)
        {
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(request.UserID), request.ApiToken))
                {
                    List<JobDocumentResponce> lstDocumentsView = _documentsBAL.GetDocumentsByJobIdForApi(Convert.ToInt32(request.JobID), request.DocumentId, request.AlreadyAddedDocumentid);
                    List<FormBot.Entity.Documents.DocumentCollectionView> lstDocumrntCollectionView = new List<DocumentCollectionView>();
                    foreach (DocumentsView item in lstDocumentsView)
                    {
                        int tmp = item.DocumentId ?? 0;
                        List<PdfItems> lstGetPDFItem = new List<PdfItems>();
                        List<FormBot.Entity.Documents.DocumentsView> lstDocuments = _documentsBAL.GetDocument(tmp, request.IsClassic);
                        string name = lstDocuments.Count > 0 ? lstDocuments[0].Name : "";
                        item.Name = Path.GetFileName(item.Name);
                        if (name.ToLower() == "ceh")
                        {
                            name = "ceh" + (FormBot.Helper.SystemEnums.JobType.PVD.ToString().ToLower() == (lstDocuments.Count > 0 ? lstDocuments[0].ServiceProviderName.ToLower() : "") ? "pvd.pdf" : "sw.pdf");
                        }
                        var documentPath = request.JobID + "/" + (lstDocuments.Count > 0 ? lstDocuments[0].Stage : Helper.SystemEnums.JobStage.PreApprovals.ToString()) + "/" + name;
                        if (item.Stage.ToLower() == "others")
                        {
                            item.PhysicalPath = ConfigurationManager.AppSettings["LoginLink"].ToString() + "Job/ViewDownloadFile/Job?FileName=" + item.Name + "&FolderName=" + item.JobId;
                            item.DownloadURLPath = ConfigurationManager.AppSettings["UploadedDocumentPath"].ToString() + "/" + ConfigurationManager.AppSettings["JobDocumentsToSavePath"].ToString() + item.JobId + "/" + "Other" + "/" + item.Name;
                        }
                        else
                        {
                            if (request.IsClassic)
                            {
                                item.DownloadURLPath = ConfigurationManager.AppSettings["UploadedDocumentPath"].ToString() + "/" + ConfigurationManager.AppSettings["JobDocumentsToSavePath"].ToString() + item.JobId + "/" + item.Stage + "/" + item.Name;
                            }
                            else
                            {
                                item.DownloadURLPath = Path.Combine(ConfigurationManager.AppSettings["UploadedDocumentPath"].ToString(), item.PhysicalPath);
                            }
                            item.PhysicalPath = ConfigurationManager.AppSettings["LoginLink"].ToString() + "Job/DocViewrForMobile?jobid=" + item.JobIDEncrypted + "&docId=" + item.DocID;
                        }
                    }

                    return new ServiceResponse() { Data = HelperMethods.ConvertToJSON(lstDocumentsView), Status = true };
                }
                else
                {
                    return new ServiceResponse() { Status = false, Error = "Invalid Token" };
                }

            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { ExceptionMessage = ex.Message, Status = false };
            }
        }
        [HttpPost]
        public ServiceResponse GetCapturedSignatureByJobId(DocumentRequest request)
        {
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(request.UserID), request.ApiToken))
                {
                    List<CaptureUserSign> lstCaptureUserSign = _documentsBAL.GetCapturedSignByJobIdForApi(Convert.ToInt32(request.JobID), request.DocumentId);
                    return new ServiceResponse() { Data = HelperMethods.ConvertToJSON(lstCaptureUserSign), Status = true };
                }
                else
                {
                    return new ServiceResponse() { Status = false, Error = "Invalid Token" };
                }
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { ExceptionMessage = ex.Message, Status = false };
            }
        }
        [HttpPost]
        public ServiceResponse GetCapturedSignatureByJobIdUploadLive(CaptureSignRequest request)
        {
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(request.UserID), request.ApiToken))
                {
                    //Logger log = new Logger();
                    //log.Log(SystemEnums.Severity.Debug, "JobDocId:" + request.JobDocId+"Fieldname:"+request.FieldName+ "signString:" + request.signString 
                    //    + "Firstname:" + request.Firstname + "Lastname:" + request.Lastname + "mobileNumber:" + request.mobileNumber + "Email:" + request.Email 
                    //    + "CreatedDate:" + request.CreatedDate + "ModifiedDate:" + request.ModifiedDate + "IsUpdate:"+request.IsUpdate);
                    _documentsBAL.GetCapturedSignByJobIdUploadLiveForApi(
                        request.JobDocId,
                        request.FieldName,
                        request.signString,
                        request.Firstname,
                        request.Lastname,
                        request.mobileNumber,
                        request.Email,
                       DateTime.Now,
                       DateTime.Now,
                        request.IsUpdate);
                    //log.Log(SystemEnums.Severity.Debug, "successfully insert.");
                    return new ServiceResponse() { Data = "", Status = true };
                }
                else
                {
                    return new ServiceResponse() { Status = false, Error = "Invalid Token" };
                }
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { ExceptionMessage = ex.Message, Status = false };
            }
        }
        public string[] GetCheckBoxExportValue(AcroFields fields, string cbFieldName)
        {
            AcroFields.Item fd = ((AcroFields.Item)fields.GetFieldItem(cbFieldName));
            var vals = fd.GetValue(0);
            Hashtable names = new Hashtable();
            string[] outs = new string[fd.Size];
            iTextSharp.text.pdf.PdfDictionary pd = ((iTextSharp.text.pdf.PdfDictionary)fd.GetWidget(0)).GetAsDict(iTextSharp.text.pdf.PdfName.AP);
            for (int k1 = 0; k1 < fd.Size; ++k1)
            {
                iTextSharp.text.pdf.PdfDictionary dic = (iTextSharp.text.pdf.PdfDictionary)fd.GetWidget(k1);
                dic = dic.GetAsDict(iTextSharp.text.pdf.PdfName.AP);
                if (dic == null)
                    continue;
                dic = dic.GetAsDict(iTextSharp.text.pdf.PdfName.N);
                if (dic == null)
                    continue;
                foreach (iTextSharp.text.pdf.PdfName pname in dic.Keys)
                {
                    String name = iTextSharp.text.pdf.PdfName.DecodeName(pname.ToString());
                    if (name.ToLower() != "off")
                    {
                        names[name] = null;
                        outs[(outs.Length - k1) - 1] = name;
                    }
                }
            }
            return outs;
        }
        /// <summary>  
        /// Uploads the job image.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Service Response</returns>
        [HttpPost]
        public ServiceResponse UploadJobImage(ImageRequest request)
        {
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(request.UserID), request.ApiToken))
                {
                    //To replace illegal characters
                    request.Filename = FormBot.Email.FileSystem.ToValidPathName(request.Filename);
                    request.CaptureUploadImagePDFName = FormBot.Email.FileSystem.ToValidPathName(request.CaptureUploadImagePDFName);

                    byte[] imageByte = Convert.FromBase64String(request.Image);
                    MemoryStream ms = new MemoryStream(imageByte);
                    Image image = Image.FromStream(ms);
                    string ImagePath = "";
                    int VisitChecklistPhotoId = 0;
                    if (FileUploadForJob(image, request.JobID, request.Filename, ref ImagePath, request.IsClassic, (!string.IsNullOrEmpty(request.VisitCheckListItemId) ? Convert.ToInt32(request.VisitCheckListItemId) : 0), request.Latitude, request.Longitude, request.ImageTakenDate))
                    {
                        if (request.IsClassic)
                        {
                            _createJobBAL.InsertPhotoForAPI(Convert.ToInt32(request.JobID), request.Filename, Convert.ToInt32(request.Status), Convert.ToInt32(request.UserID), null);
                        }
                        else
                        {
                            bool isDefault = (string.IsNullOrEmpty(request.VisitCheckListItemId) && string.IsNullOrEmpty(request.JobSchedulingId));
                            if (request.Status == "2" && (request.Latitude == "" || request.Longitude == "" || request.Altitude == "" || request.Accuracy == ""))
                            {
                                _createJobBAL.UpdateIsSpvVerifiedWhenLatLongNullForUploadPhoto(Convert.ToInt32(request.JobID));
                            }

                            DataSet jobPhoto = _createJobBAL.InsertReferencePhoto(Convert.ToInt32(request.JobID), ImagePath, Convert.ToInt32(request.UserID), request.JobSchedulingId, request.VisitCheckListItemId, isDefault, request.Status, null, request.Status, request.Latitude, request.Longitude, request.ImageTakenDate, request.Manufacturer, request.Model, true, request.Altitude, request.Accuracy);
                            if (jobPhoto != null && jobPhoto.Tables.Count > 0)
                            {
                                if (jobPhoto.Tables[0] != null && jobPhoto.Tables[0].Rows.Count > 0)
                                {
                                    VisitChecklistPhotoId = Convert.ToInt32(jobPhoto.Tables[0].Rows[0][0]);
                                }
                            }
                            //VisitChecklistPhotoId = _createJobBAL.InsertReferencePhoto(Convert.ToInt32(request.JobID), ImagePath, Convert.ToInt32(request.UserID), request.JobSchedulingId, request.VisitCheckListItemId, isDefault, request.Status, null, request.Status, request.Latitude, request.Longitude, request.ImageTakenDate, request.Manufacturer, request.Model, true, request.Altitude, request.Accuracy);
                            request.VisitChecklistPhotoId = VisitChecklistPhotoId;

                            if (request.Status == "4")
                            {
                                string Type = request.PDFLocationId == "2" ? "CES" : "OTHER";
                                string DocPath = Path.Combine("JobDocuments", request.JobID, Type, request.VisitCheckListItemId, request.CaptureUploadImagePDFName.ToLower().Contains(".pdf") ? request.CaptureUploadImagePDFName : (request.CaptureUploadImagePDFName + ".pdf"));

                                string Source = Path.Combine(ConfigurationManager.AppSettings["ProofUploadFolder"].ToString(), ImagePath);
                                string Destination = Path.Combine(ConfigurationManager.AppSettings["ProofUploadFolder"].ToString(), DocPath);

                                bool isFileExists = File.Exists(Destination);
                                DataSet dsFiles = _createJobBAL.GetCesPhotosByVisitCheckListId(Convert.ToInt32(request.VisitCheckListItemId));
                                generatePDFfromImage(dsFiles.Tables[0], Destination);

                                //delete pdf file
                                if (dsFiles.Tables[0].Rows.Count < 1)
                                {

                                }

                                if (!isFileExists)
                                    _createJobBAL.InsertCESDocuments(Convert.ToInt32(request.JobID), DocPath, Convert.ToInt32(request.UserID), Type, null);
                            }

                        }
                        return new ServiceResponse() { Status = true, Data = HelperMethods.ConvertToJSON(request) };
                    }
                    else
                    {
                        return new ServiceResponse() { Status = false, Error = "Error in image." };
                    }

                }
                else
                {
                    return new ServiceResponse() { Status = false, Error = "Invalid Token" };
                }

            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { Status = false, ExceptionMessage = ex.Message };
            }

        }

        /// <summary>
        /// Jobs the notes.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Service Response</returns>
        [HttpPost]
        public ServiceResponse JobNotes(JobRequest request)
        {
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(request.UserID), request.ApiToken))
                {
                    DataSet dataset = _createJobBAL.GetJobNotesListForAPI(Convert.ToInt32(request.JobID), Convert.ToInt32(request.JobSchedulingID));
                    DataView dv = new DataView();
                    DataTable dt = new DataTable();
                    DataTable result = new DataTable();
                    if (dataset.Tables.Count > 0)
                    {
                        dt = dataset.Tables[0];
                        dt.Columns.Add("CreatedDateTick", typeof(string));
                        foreach (DataRow item in dt.Rows)
                        {
                            item["CreatedDateTick"] = !string.IsNullOrEmpty(item["CreatedDate"].ToString()) ? Convert.ToDateTime(item["CreatedDate"].ToString()).Ticks.ToString() : "";
                        }

                        dv = dt.DefaultView;
                        dv.Sort = "RowNumber DESC";
                        result = dv.ToTable();
                    }

                    if (result != null)
                    {
                        return new ServiceResponse() { Data = HelperMethods.ConvertToJSON(result), Status = true };
                    }

                    return new ServiceResponse() { Data = "", Status = true, Error = "No record found" };
                }
                else
                {
                    return new ServiceResponse() { Status = false, Error = "Invalid Token" };
                }

            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { Status = false, ExceptionMessage = ex.Message };
            }

        }

        /// <summary>
        /// Jobs the photo delete.
        /// </summary>
        /// <param name="request">request class.</param>
        /// <returns>Service Response class</returns>
        [HttpPost]
        public ServiceResponse JobPhotoDelete(JobPhotoDeleteRequest request)
        {
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(request.UserID), request.ApiToken))
                {
                    if (!string.IsNullOrEmpty(request.FolderName))
                    {
                        string[] array = request.FolderName.Split(',');
                        foreach (string fileName in array)
                        {
                            try
                            {

                                if (!request.IsClassic)
                                {
                                    bool isDefault = (string.IsNullOrEmpty(request.VisitCheckListItemId) && string.IsNullOrEmpty(request.JobSchedulingId));
                                    string path = Path.Combine("JobDocuments", request.JobID, (isDefault ? "DefaultFolder" : Path.Combine("checklistitem", request.VisitCheckListItemId)), fileName);
                                    // DeleteDirectoryPhoto(Path.Combine(ProjectSession.ProofDocumentsURL, path));
                                    _createJobBAL.DeleteVisitCheckListPhotosByPath(path, Convert.ToInt32(request.JobID));
                                    if (!isDefault && !string.IsNullOrEmpty(request.VisitCheckListItemId))
                                    {
                                        DataSet ds = _createJobBAL.DeleteCesPdf(Convert.ToInt32(request.VisitCheckListItemId));
                                        if (ds.Tables[0].Rows[0]["CheckListClassTypeId"] != null && ds.Tables[0].Rows[0]["CheckListClassTypeId"].ToString() == "4")
                                        {
                                            string Type = ds.Tables[0].Rows[0][1].ToString() == "2" ? "CES" : "OTHER";
                                            string FileName = ds.Tables[0].Rows[0][0].ToString();
                                            string DocPath = Path.Combine("JobDocuments", request.JobID, Type, FileName.ToLower().Contains(".pdf") ? FileName : (FileName + ".pdf"));
                                            string Destination = Path.Combine(ConfigurationManager.AppSettings["ProofUploadFolder"].ToString(), DocPath);
                                            generatePDFfromImage(ds.Tables[1], Destination);

                                            if (ds.Tables[1].Rows.Count > 0)
                                            {

                                            }
                                        }

                                    }

                                }
                                else
                                {
                                    //DeleteDirectoryPhoto(Path.Combine(ProjectSession.ProofDocumentsURL, "JobDocuments", request.JobID, fileName));
                                    _createJobBAL.DeletePhotoApi(fileName, Convert.ToInt32(request.JobID));
                                }
                            }
                            catch (Exception ex) { }
                        }

                    }

                    return new ServiceResponse() { Status = true };
                }
                else
                {
                    return new ServiceResponse() { Status = false, Error = "Invalid Token" };
                }

            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { Status = false, ExceptionMessage = ex.Message };
            }

        }

        /// <summary>
        /// Jobs the photo list.
        /// </summary>
        /// <param name="request">request class.</param>
        /// <returns>Service Response class</returns>
        [HttpPost]
        public ServiceResponse JobPhotoList(JobRequest request)
        {
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(request.UserID), request.ApiToken))
                {
                    List<UserDocumentModel> lstModel = new List<UserDocumentModel>();
                    if (request.IsClassic)
                    {
                        List<UserDocument> lstUserDocument = _createJobBAL.GetJobInstallationPhotoByJobID(Convert.ToInt32(request.JobID));
                        List<UserDocument> userDocument = _createJobBAL.GetJobInstallationSerialByJobID(Convert.ToInt32(request.JobID));

                        foreach (UserDocument item in lstUserDocument)
                        {
                            UserDocumentModel model = new UserDocumentModel();
                            model.DocumentPath = item.DocumentPath;
                            model.Index = item.index;
                            model.MimeType = item.MimeType;
                            model.Status = 1;
                            model.StrDocumentPath = item.strDocumentPath;
                            model.UserDocumentID = item.UserDocumentID;
                            model.UserID = item.UserID;
                            model.JobID = Convert.ToInt32(request.JobID);
                            lstModel.Add(model);
                        }

                        foreach (UserDocument item in userDocument)
                        {
                            UserDocumentModel model = new UserDocumentModel();
                            model.DocumentPath = item.DocumentPath;
                            model.Index = item.index;
                            model.MimeType = item.MimeType;
                            model.Status = 2;
                            model.StrDocumentPath = item.strDocumentPath;
                            model.UserDocumentID = item.UserDocumentID;
                            model.UserID = item.UserID;
                            model.JobID = Convert.ToInt32(request.JobID);
                            lstModel.Add(model);
                        }
                    }
                    else
                    {
                        List<UserDocument> lstUserDocument = _createJobBAL.GetJobSchedulingPhotosByJobID(Convert.ToInt32(request.JobSchedulingID));
                        List<UserDocument> Type1 = lstUserDocument.Where(e => e.Status == 1).ToList();
                        List<UserDocument> Type2 = lstUserDocument.Where(e => e.Status == 2).ToList();
                        List<UserDocument> Type3 = lstUserDocument.Where(e => e.Status == 3).ToList();
                        List<UserDocument> Type4 = lstUserDocument.Where(e => e.Status == 4).ToList();

                        int i = 0;
                        foreach (UserDocument t1 in Type1)
                        {
                            UserDocumentModel model = new UserDocumentModel();
                            model.DocumentPath = Path.GetFileName(@t1.DocumentPath);
                            model.Index = i;
                            model.MimeType = "image";
                            model.Status = 1;
                            model.StrDocumentPath = model.DocumentPath;
                            model.UserDocumentID = t1.UserDocumentID;
                            model.UserID = t1.UserID;
                            model.JobID = Convert.ToInt32(request.JobID);
                            model.VisitCheckListItemId = t1.VisitCheckListItemId;
                            model.JobSchedulingId = t1.JobSchedulingId;
                            model.Longitude = t1.Longitude;
                            model.Latitude = t1.Latitude;
                            model.IsClassic = false;
                            model.ImageTakenDate = t1.ImageTakenDate;
                            lstModel.Add(model);
                        }
                        i = 0;
                        foreach (UserDocument t1 in Type2)
                        {
                            UserDocumentModel model = new UserDocumentModel();
                            model.DocumentPath = Path.GetFileName(@t1.DocumentPath);
                            model.Index = i;
                            model.MimeType = "image";
                            model.Status = 2;
                            model.StrDocumentPath = model.DocumentPath;
                            model.UserDocumentID = t1.UserDocumentID;
                            model.UserID = t1.UserID;
                            model.JobID = Convert.ToInt32(request.JobID);
                            model.VisitCheckListItemId = t1.VisitCheckListItemId;
                            model.JobSchedulingId = t1.JobSchedulingId;
                            model.Longitude = t1.Longitude;
                            model.Latitude = t1.Latitude;
                            model.IsClassic = false;
                            model.ImageTakenDate = t1.ImageTakenDate;
                            model.Manufacturer = t1.Manufacturer;
                            model.Model = t1.Model;
                            lstModel.Add(model);
                        }
                        i = 0;
                        foreach (UserDocument t1 in Type3)
                        {
                            UserDocumentModel model = new UserDocumentModel();
                            model.DocumentPath = Path.GetFileName(@t1.DocumentPath);
                            model.Index = i;
                            model.MimeType = "image";
                            model.Status = 3;
                            model.StrDocumentPath = model.DocumentPath;
                            model.UserDocumentID = t1.UserDocumentID;
                            model.UserID = t1.UserID;
                            model.JobID = Convert.ToInt32(request.JobID);
                            model.VisitCheckListItemId = t1.VisitCheckListItemId;
                            model.JobSchedulingId = t1.JobSchedulingId;
                            model.Longitude = t1.Longitude;
                            model.Latitude = t1.Latitude;
                            model.IsClassic = false;
                            model.ImageTakenDate = t1.ImageTakenDate;
                            lstModel.Add(model);
                        }
                        i = 0;
                        foreach (UserDocument t1 in Type4)
                        {
                            UserDocumentModel model = new UserDocumentModel();
                            model.DocumentPath = Path.GetFileName(@t1.DocumentPath);
                            model.Index = i;
                            model.MimeType = "image";
                            model.Status = 4;
                            model.StrDocumentPath = model.DocumentPath;
                            model.UserDocumentID = t1.UserDocumentID;
                            model.UserID = t1.UserID;
                            model.JobID = Convert.ToInt32(request.JobID);
                            model.VisitCheckListItemId = t1.VisitCheckListItemId;
                            model.JobSchedulingId = t1.JobSchedulingId;
                            model.Longitude = t1.Longitude;
                            model.Latitude = t1.Latitude;
                            model.IsClassic = false;
                            model.ImageTakenDate = t1.ImageTakenDate;
                            lstModel.Add(model);
                        }
                    }
                    return new ServiceResponse() { Data = HelperMethods.ConvertToJSON(lstModel), Status = true };
                }
                else
                {
                    return new ServiceResponse() { Status = false, Error = "Invalid Token" };
                }

            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { Status = false, ExceptionMessage = ex.Message };
            }
        }

        /// <summary>
        /// Gets the image base string.
        /// </summary>
        /// <param name="request">request class.</param>
        /// <returns>Service Response class</returns>
        [HttpPost]
        public ServiceResponse GetImageBaseString(JobPhotoRequest request)
        {
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(request.UserID), request.ApiToken))
                {
                    string path;
                    if (!request.IsClassic)
                    {
                        bool isDefault = (string.IsNullOrEmpty(request.VisitCheckListItemId) && string.IsNullOrEmpty(request.JobSchedulingId));

                        if (request.IsReference != 1)
                        {
                            path = Path.Combine(ProjectSession.ProofDocumentsURL, "JobDocuments", request.JobID, (isDefault ? "DefaultFolder" : Path.Combine("checklistitem", request.VisitCheckListItemId)), request.ImageName);
                        }

                        else
                        {
                            path = Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + "JobDocuments" + "\\" + request.JobID + "\\" + "ReferencePhotos" + "\\" + request.ImageName);
                        }

                    }
                    else
                    {
                        path = Path.Combine(ProjectSession.ProofDocumentsURL, "JobDocuments", request.JobID, request.ImageName);

                    }
                    byte[] byteImage = ReadImageFile(path);
                    return new ServiceResponse() { Data = Convert.ToBase64String(byteImage), Status = true };
                }
                else
                {
                    return new ServiceResponse() { Status = false, Error = "Invalid Token" };
                }
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { Status = false, ExceptionMessage = ex.Message };
            }
        }

        /// <summary>
        /// Saves the notes.
        /// </summary>
        /// <param name="request">request class.</param>
        /// <returns>Service Response class</returns>
        [HttpPost]
        public ServiceResponse SaveNotes(JobNotesRequest request)
        {
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(request.UserID), request.ApiToken))
                {
                    int id = _createJobBAL.InsertJobNotes(request.Notes, Convert.ToInt32(request.JobID), Convert.ToInt32(request.UserID), Convert.ToInt32(request.JobSchedulingID));
                    return new ServiceResponse() { Status = true, Data = id.ToString() };
                }
                else
                {
                    return new ServiceResponse() { Status = false, Error = "Invalid Token" };
                }

            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { Status = false, ExceptionMessage = ex.Message };
            }
        }

        [HttpPost]
        public ServiceResponse GetSerialNumber(VisitCheckListItem request)
        {
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(request.UserID), request.ApiToken))
                {
                    string serialNumber = _createJobBAL.GetSerialNumber(Convert.ToInt32(request.VisitCheckListItemId));
                    return new ServiceResponse() { Status = true, Data = serialNumber };
                }
                else
                {
                    return new ServiceResponse() { Status = false, Error = "Invalid Token" };
                }

            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { Status = false, ExceptionMessage = ex.Message };
            }
        }

        /// <summary>
        /// Updates the serial number.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>service response class</returns>
        [HttpPost]
        public ServiceResponse UpdateSerialNumber(JobSerialNumberRequest request)
        {
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(request.UserID), request.ApiToken))
                {
                    _createJobBAL.UpdateSerialNumber(Convert.ToInt32(request.JobID), request.SerialNumber);
                    return new ServiceResponse() { Status = true };
                }
                else
                {
                    return new ServiceResponse() { Status = false, Error = "Invalid Token" };
                }
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { Status = false, ExceptionMessage = ex.Message };
            }
        }
        [HttpPost]
        public ServiceResponse GetSPVSerialNumber(VisitCheckListItem request)
        {
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(request.UserID), request.ApiToken))
                {
                    var lstSerialNos = new List<JobSerialNumberRequestFromAutoSyncMethod>();
                    if (request.lstJobSerialNumbers != null && request.lstJobSerialNumbers.Count() > 0)
                    {
                        lstSerialNos = request.lstJobSerialNumbers;
                        string IsSPVRequired = _createJobBAL.CheckSPVRequiredByJobId(request.lstJobSerialNumbers.FirstOrDefault().JobId).IsSPVRequired.ToString();
                        //if job spv affiliated then 
                        if (IsSPVRequired.ToLower() == "true")
                            SPVSerialNumberProductVerification(ref lstSerialNos,Convert.ToInt32(request.JobId));
                        DataTable dt = new DataTable();
                        dt.Columns.Add(new DataColumn("SerialNumber", typeof(string)));
                        dt.Columns.Add(new DataColumn("Verified", typeof(bool)));
                        foreach (var item in lstSerialNos)
                        {
                            DataRow dr = dt.NewRow();
                            dr["SerialNumber"] = item.SerialNumber;
                            if (IsSPVRequired.ToLower() == "true")
                            {
                                if (item.IsVerified.HasValue)
                                    dr["Verified"] = item.IsVerified.Value;
                                else
                                    dr["Verified"] = DBNull.Value;
                            }
                            else
                                dr["Verified"] = DBNull.Value;
                            dt.Rows.Add(dr);
                        }
                        _createJobBAL.UpdateVerifiedSerialNumber(dt, lstSerialNos[0].JobId);
                    }
                    return new ServiceResponse()
                    {
                        Status = true,
                        Data = HelperMethods.ConvertToJSON(lstSerialNos)
                    };
                }
                else
                {
                    return new ServiceResponse() { Status = false, Error = "Invalid Token" };
                }

            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { Status = false, ExceptionMessage = ex.Message };
            }
        }

        [HttpPost]
        public ServiceResponse UpdateCheckListSerialNumber(VisitCheckListItem request)
        {
            bool WithSpvVerified = false;
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(request.UserID), request.ApiToken))
                {
                    var lstSerialNos = new List<JobSerialNumberRequestFromAutoSyncMethod>();
                    DataTable dt = DtSPVSerialNumber();

                    if (request.lstJobSerialNumbers != null && request.lstJobSerialNumbers.Count() > 0)
                    {
                        lstSerialNos = request.lstJobSerialNumbers;
                        string IsSPVRequired = _createJobBAL.CheckSPVRequiredByJobId(request.lstJobSerialNumbers.FirstOrDefault().JobId).IsSPVRequired.ToString();
                        string IsScOrGlobalLevalSpvAllowed = _createJobBAL.GetSPVRequiredOrNotOnSCAOrGlobalLevelOrManufacturer( Convert.ToString(request.lstJobSerialNumbers.FirstOrDefault().JobId)).ToList().FirstOrDefault().IsSPVRequired.ToString();
                        //if job spv affiliated then 
                        if (IsSPVRequired.ToLower() == "true" && IsScOrGlobalLevalSpvAllowed.ToLower() == "true")
                        {
                            SPVSerialNumberProductVerification(ref lstSerialNos,Convert.ToInt32(request.JobId));
                            WithSpvVerified = true;
                        }


                        if (!string.IsNullOrEmpty(request.CheckListSerialNumbers))
                            request.CheckListSerialNumbers += (Environment.NewLine) + string.Join(Environment.NewLine, request.lstJobSerialNumbers.Select(X => X.SerialNumber));
                        else
                            request.CheckListSerialNumbers = string.Join(Environment.NewLine, request.lstJobSerialNumbers.Select(X => X.SerialNumber));
                        foreach (var item in lstSerialNos)
                        {
                            DataRow dr = dt.NewRow();
                            dr["JobId"] = item.JobId;
                            dr["SerialNumber"] = item.SerialNumber;
                            dr["Brand"] = item.Brand;
                            dr["Model"] = item.Model;
                            if (IsSPVRequired.ToLower() == "true" && IsScOrGlobalLevalSpvAllowed.ToLower() == "true")
                            {
                                if (item.IsVerified.HasValue)
                                    dr["IsVerified"] = item.IsVerified.Value;
                                else
                                    dr["IsVerified"] = DBNull.Value;
                            }
                            else
                                dr["IsVerified"] = DBNull.Value;
                            dt.Rows.Add(dr);
                        }
                    }
                    string IsUploaded = _createJobBAL.CheckSPVRequiredByJobId(Convert.ToInt32(request.JobId)).IsUpload.ToString();
                    string visitUniqueId = string.Empty;

                    if (IsUploaded.ToLower() == "true")
                        visitUniqueId = _createJobBAL.UpdateCheckListSerialNumber(Convert.ToInt32(request.VisitCheckListItemId), request.CheckListSerialNumbers, Convert.ToInt32(request.UserID), dt);
                    else
                        visitUniqueId = null;
                    if (string.IsNullOrEmpty(visitUniqueId))
                        return new ServiceResponse() { Status = false, Error = "Serial Number not uploaded because Visit is completed or job is traded.", WithSpvVerified = WithSpvVerified };
                    if (request.IsCreateFile)
                        if (IsUploaded.ToLower() == "true")
                            createFileForChecklistNumber(request, visitUniqueId);
                    return new ServiceResponse() { Status = true, Data = HelperMethods.ConvertToJSON(lstSerialNos), WithSpvVerified = WithSpvVerified };
                }
                else
                {
                    return new ServiceResponse() { Status = false, Error = "Invalid Token", WithSpvVerified = WithSpvVerified };
                }
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { Status = false, ExceptionMessage = ex.Message, WithSpvVerified = WithSpvVerified };
            }
        }

        /// <summary>
        /// Jobs the signature.
        /// </summary>
        /// <param name="request">request class.</param>
        /// <returns>service response class </returns>
        [HttpPost]
        public ServiceResponse JobSignature(SignatureRequest request)
        {
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(request.UserID), request.ApiToken))
                {
                    byte[] imageByte = Convert.FromBase64String(request.Image);
                    MemoryStream ms = new MemoryStream(imageByte);
                    Image image = Image.FromStream(ms);
                    string signature = string.Empty;
                    if (FileUpload(image, request.JobID, request.Signature))
                    {
                        DateTime signatureDate = new DateTime(Convert.ToInt64(request.SignatureDate));
                        signature = _createJobBAL.GetJobOwnerSignatureForAPI(Convert.ToInt32(request.JobID), request.Signature, request.Latitude, request.Longitude, request.IpAddress, request.Location, signatureDate);
                        string proofDocumentsFolder = ProjectSession.ProofDocuments;
                        string proofDocumentsFolderURL = ProjectSession.ProofDocumentsURL;
                        if (!string.IsNullOrEmpty(request.JobID))
                        {
                            proofDocumentsFolder = Path.Combine(proofDocumentsFolder + "\\" + "JobDocuments" + "\\" + request.JobID + "\\");
                            proofDocumentsFolderURL = proofDocumentsFolderURL + "\\" + "JobDocuments" + "\\" + request.JobID + "\\";
                        }

                        string path = Path.Combine(proofDocumentsFolder + "\\" + signature);
                        if (System.IO.File.Exists(path))
                        {
                            File.Delete(path);
                        }

                        return new ServiceResponse() { Status = true };
                    }
                    else
                    {
                        return new ServiceResponse() { Status = false, Error = "Error in image." };
                    }

                }
                else
                {
                    return new ServiceResponse() { Status = false, Error = "Invalid Token" };
                }

            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { Status = false, ExceptionMessage = ex.Message };
            }
        }

        /// <summary>
        /// Jobs the visit.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>service response</returns>
        [HttpPost]
        public ServiceResponse JobVisit(JobRequest request)
        {
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(request.UserID), request.ApiToken))
                {
                    List<JobScheduling> lstVisitModel = _createJobBAL.GetJobschedulingByJobID(Convert.ToInt32(request.JobID));
                    List<JobSchedulingModel> lstModel = new List<JobSchedulingModel>();
                    foreach (var jobScheduling in lstVisitModel)
                    {
                        DateTime dtTick;
                        DateTime.TryParse(Convert.ToString(jobScheduling.visitEndDate), out dtTick);

                        JobSchedulingModel shedule = new JobSchedulingModel()
                        {
                            CreatedBy = jobScheduling.CreatedBy,
                            CreatedDate = jobScheduling.CreatedDate,
                            Detail = jobScheduling.Detail,
                            CreatedDateTick = jobScheduling.CreatedDate != null ? jobScheduling.CreatedDate.Ticks : 0,
                            StartDateTick = jobScheduling.visitStartDate != null ? jobScheduling.visitStartDate.Ticks : 0,
                            StartTimeTick = !string.IsNullOrEmpty(jobScheduling.strVisitStartTime) ? Convert.ToDateTime(jobScheduling.strVisitStartTime).Ticks : 0,
                            EndDateTick = jobScheduling.visitEndDate != null ? dtTick.Ticks : 0,
                            EndDateTime = !string.IsNullOrEmpty(jobScheduling.strVisitEndTime) ? Convert.ToDateTime(jobScheduling.strVisitEndTime).Ticks : 0,
                            Id = jobScheduling.Id,
                            JobID = jobScheduling.JobID,
                            StatusName = jobScheduling.StatusName,
                            UserId = jobScheduling.UserId,
                            UserName = jobScheduling.UserName,
                            JobVisitID = jobScheduling.JobSchedulingID
                        };
                        lstModel.Add(shedule);
                    }

                    return new ServiceResponse() { Status = true, Data = HelperMethods.ConvertToJSON(lstModel) };
                }
                else
                {
                    return new ServiceResponse() { Status = false, Error = "Invalid Token" };
                }

            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { Status = false, ExceptionMessage = ex.Message };
            }
        }

        /// <summary>
        /// Updates the schedule status.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>service response</returns>
        [HttpPost]
        public ServiceResponse UpdateScheduleStatus(ScheduleStatusRequest request)
        {
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(request.UserID), request.ApiToken))
                {
                    _scheduleBAL.AcceptRejectSchedule(Convert.ToInt32(request.Status), Convert.ToInt32(request.JobSchedulingID));
                    return new ServiceResponse() { Status = true };
                }
                else
                {
                    return new ServiceResponse() { Status = false, Error = "Invalid Token" };
                }
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { Status = false, ExceptionMessage = ex.Message };
            }
        }

        /// <summary>
        /// Deletes the serial number.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>service response</returns>
        [HttpPost]
        public ServiceResponse DeleteSerialNumber(JobSerialNumberRequest request)
        {
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(request.UserID), request.ApiToken))
                {
                    _scheduleBAL.DeleteSerialNumber(Convert.ToInt32(request.JobID), request.SerialNumber, Convert.ToBoolean(request.IsAll));
                    return new ServiceResponse() { Status = true };
                }
                else
                {
                    return new ServiceResponse() { Status = false, Error = "Invalid Token" };
                }
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { Status = false, ExceptionMessage = ex.Message };
            }
        }

        /// <summary>
        /// Updates the job stage.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>service response</returns>
        [HttpPost]
        public ServiceResponse UpdateJobStage(JobStageRequest request)
        {
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(request.UserID), request.ApiToken))
                {
                    _scheduleBAL.UpdateJobStageForAPI(Convert.ToInt32(request.JobID), request.Stage);
                    return new ServiceResponse() { Status = true };
                }
                else
                {
                    return new ServiceResponse() { Status = false, Error = "Invalid Token" };
                }
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { Status = false, ExceptionMessage = ex.Message };
            }
        }

        /// <summary>
        /// Get Api Version
        /// </summary>
        /// <returns>ServiceResponse</returns>
        [HttpPost]
        public ServiceResponse GetApiVersion()
        {
            try
            {
                DataSet dsVersion = _user.GetApiVersion();
                ApiVersion getApiVersion = new ApiVersion();
                if (dsVersion != null && dsVersion.Tables[0] != null && dsVersion.Tables[0].Rows.Count > 0)
                {
                    getApiVersion.AndroidVersion = Convert.ToString(dsVersion.Tables[0].Rows[0]["AndroidVersion"]);
                    getApiVersion.IOSVersion = Convert.ToString(dsVersion.Tables[0].Rows[0]["IOSVersion"]);
                    getApiVersion.IsCompulsory = Convert.ToBoolean(dsVersion.Tables[0].Rows[0]["IsCompulsory"]);
                    getApiVersion.HighQuality = Convert.ToInt32(dsVersion.Tables[0].Rows[0]["HighQuality"]);
                    getApiVersion.LowQuality = Convert.ToInt32(dsVersion.Tables[0].Rows[0]["LowQuality"]);
                    getApiVersion.MediumQuality = Convert.ToInt32(dsVersion.Tables[0].Rows[0]["MediumQuality"]);
                    getApiVersion.ScandITAndroidKey = Convert.ToString(dsVersion.Tables[0].Rows[0]["ScandITAndroidKey"]);
                    getApiVersion.ScandITIOSKey = Convert.ToString(dsVersion.Tables[0].Rows[0]["ScandITIOSKey"]);
                    return new ServiceResponse() { Data = HelperMethods.ConvertToJSON(getApiVersion), Status = true };
                }
                else
                {
                    return new ServiceResponse() { Status = false, Error = "No record found." };
                }

            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { ExceptionMessage = ex.Message, Status = false };
            }
        }

        /// <summary>
        /// UpdateJobDocument
        /// </summary>
        /// <param name="request">request</param>
        /// <returns>ServiceResponse</returns>
        [HttpPost]
        public ServiceResponse UpdateJobDocument(UpdateDocumentRequest request)
        {
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(request.UserID), request.ApiToken))
                {
                    Log.WriteLog("request : " + JsonConvert.SerializeObject(request));
                    string jsonData = request.JsonData;

                    DataTable dt = _documentsBAL.UpdateJobDocument(Convert.ToInt32(request.DocumentId), Convert.ToInt32(request.JobId), jsonData).Tables[0];
                    bool IsClassic = Convert.ToBoolean(dt.Rows[0]["IsClassic"]);
                    List<PdfItems> model = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PdfItems>>(jsonData);
                    string path;
                    if (!IsClassic)
                    {
                        path = Path.Combine(ProjectSession.ProofDocuments, dt.Rows[0]["Path"].ToString());

                    }
                    else
                    {
                        string proofDocumentsFolder = ProjectSession.ProofDocuments;
                        if (!string.IsNullOrEmpty(request.JobId))
                        {
                            proofDocumentsFolder = Path.Combine(proofDocumentsFolder + "\\" + "JobDocuments" + "\\" + request.JobId + "\\");

                        }
                        path = Path.Combine(proofDocumentsFolder + "\\" + dt.Rows[0]["Path"].ToString().Replace("%", "$"));

                    }
                    _createJobBAL.FillPDFAndSave(model, path, false);


                    //if(!string.IsNullOrEmpty(request.lstCaptureUserSign))
                    //{
                    //	List<CaptureUserSign> lstCaptureUserSign = JsonConvert.DeserializeObject<List<CaptureUserSign>>(request.lstCaptureUserSign);
                    //	if (request.lstCaptureUserSign != null)
                    //	{
                    //		if (lstCaptureUserSign.Any())
                    //		{
                    //			foreach (var item in lstCaptureUserSign)
                    //			{
                    //				_createJobBAL.InsertUserSignatureApi(item.jobDocId.ToString(), item.signString, false, item.fieldName, item.mobileNumber, item.Firstname, item.Lastname, 0, item.Email);
                    //			}
                    //		}
                    //	}
                    //}

                    return new ServiceResponse() { Status = true };
                }
                else
                {
                    return new ServiceResponse() { Status = false, Error = "Invalid Token" };
                }
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { Status = false, ExceptionMessage = ex.Message };
            }
        }

        /// <summary>
        /// Get Solar Company List.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>service response</returns>
        [HttpPost]
        public ServiceResponse GetSolarCompanyList(SolarCompanyRequest request)
        {
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(request.UserID), request.ApiToken))
                {
                    IList<User> lstUser = _user.GetUserList(Convert.ToInt32(request.UserID), 1, 10000, string.Empty, string.Empty, "4,6", 2, string.Empty, string.Empty, string.Empty, 0, 0, 0, false, 0, string.Empty, string.Empty, 0, string.Empty, string.Empty, string.Empty, 0, 0);
                    return new ServiceResponse() { Status = true, Data = HelperMethods.ConvertToJSON(lstUser) };
                }
                else
                {
                    return new ServiceResponse() { Status = false, Error = "Invalid Token" };
                }

            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { Status = false, ExceptionMessage = ex.Message };
            }
        }

        /// <summary>
        /// Solar company's request accept/reject by solar electrician
        /// </summary>
        /// <param name="request">request</param>
        /// <returns>Service Response</returns>
        [HttpPost]
        public ServiceResponse AcceptRejectSolarCompanyRequestByAPI(SolarCompanyStatusSetBySE request)
        {
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(request.UserID), request.ApiToken))
                {
                    int status = request.SolarCompanyStatus ? 2 : 3;

                    FormBot.Entity.User scauser = _user.GetUserBySolarCompanyId(Convert.ToInt32(request.SolarCompanyId));

                    _user.SEAcceptRejectSolarCompanyRequest(Convert.ToInt32(request.UserID), Convert.ToInt32(request.SolarCompanyId), status);

                    string failReason = string.Empty;

                    EmailInfo emailInfo = new EmailInfo();
                    emailInfo.TemplateID = request.SolarCompanyStatus ? 7 : 8;
                    emailInfo.FirstName = scauser.FirstName;
                    emailInfo.LastName = scauser.LastName;
                    emailInfo.SolarElectrician = request.ElectricianName;
                    // _emailBAL.ComposeAndSendEmail(emailInfo, scauser.Email);

                    SMTPDetails smtpDetail = new SMTPDetails();

                    //smtpDetail.MailFrom = ProjectSession.MailFrom;
                    //smtpDetail.SMTPUserName = ProjectSession.SMTPUserName;
                    //smtpDetail.SMTPPassword = ProjectSession.SMTPPassword;
                    //smtpDetail.SMTPHost = ProjectSession.SMTPHost;
                    //smtpDetail.SMTPPort = Convert.ToInt32(ProjectSession.SMTPPort);
                    //smtpDetail.IsSMTPEnableSsl = ProjectSession.IsSMTPEnableSsl;
                    smtpDetail.MailFrom = ConfigurationManager.AppSettings["MailFrom"].ToString();
                    smtpDetail.SMTPPassword = ConfigurationManager.AppSettings["SMTPPassword"].ToString();
                    smtpDetail.SMTPUserName = ConfigurationManager.AppSettings["SMTPUserName"].ToString();
                    smtpDetail.SMTPHost = ConfigurationManager.AppSettings["SMTPHost"].ToString();
                    smtpDetail.SMTPPort = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);
                    smtpDetail.IsSMTPEnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSMTPEnableSsl"]);
                    EmailTemplate emailTempalte = _emailBAL.GetEmailTemplateByID(emailInfo.TemplateID);
                    string body = _emailBAL.GetEmailBody(emailInfo, emailTempalte);

                    if (body != null && !string.IsNullOrEmpty(body))
                    {
                        QueuedEmail objQueuedEmail = new QueuedEmail();
                        objQueuedEmail.FromEmail = ProjectSession.MailFrom;
                        objQueuedEmail.Body = body;
                        objQueuedEmail.Subject = emailTempalte.Subject;
                        objQueuedEmail.ToEmail = scauser.Email;
                        objQueuedEmail.CreatedDate = DateTime.Now;
                        objQueuedEmail.ModifiedDate = DateTime.Now;
                        _emailBAL.InsertUpdateQueuedEmail(objQueuedEmail);
                    }


                    //MailHelper.SendMail(smtpDetail, scauser.Email, null, null, emailTempalte.Subject, body, null, true, ref failReason, false);
                    //return statuses;

                    return new ServiceResponse() { Status = true };
                }
                else
                {
                    return new ServiceResponse() { Status = false, Error = "Invalid Token" };
                }

            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { Status = false, ExceptionMessage = ex.Message };
            }
        }



        /// <summary>
        /// Jobs the signature.
        /// </summary>
        /// <param name="request">request class.</param>
        /// <returns>service response class </returns>
        [HttpPost]
        public ServiceResponse VisitSignature(VisitSignatureRequest request)
        {
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(request.UserID), request.ApiToken))
                {
                    byte[] imageByte = Convert.FromBase64String(request.Image);
                    MemoryStream ms = new MemoryStream(imageByte);
                    Image image = Image.FromStream(ms);

                    string ImgPath = "";
                    string FoldaerName = "";// _createJobBAL.GetFolderName(Convert.ToInt32(request.CheckListItemId));
                    string filename = DateTime.Now.Ticks.ToString() + "_" + request.SignatureTypeId + ".png";
                    string path = _createJobBAL.GetVisitSignaturePath(Convert.ToInt32(request.JobSchedulingId), !string.IsNullOrEmpty(request.VisitCheckListItemId) ? Convert.ToInt32(request.VisitCheckListItemId) : 0, Convert.ToInt32(request.SignatureTypeId));
                    ImgPath = path;
                    bool isInsert = string.IsNullOrEmpty(path);
                    // request.Path = filename;
                    if (FileUpload(image, request.JobId, filename, request.CheckListItemId, FoldaerName, ref path, ref ImgPath, request.Latitude, request.Longitude, request.SignatureDate))
                    {
                        request.Path = ImgPath;
                        _createJobBAL.InsertVisitSignatureAPI(isInsert ? 0 : 1, !string.IsNullOrEmpty(request.VisitCheckListItemId) ? Convert.ToInt32(request.VisitCheckListItemId) : 0, Convert.ToInt32(request.JobSchedulingId), Convert.ToInt32(request.JobId), request.Path, Convert.ToInt32(request.SignatureTypeId), Convert.ToInt32(request.UserID), request.Latitude, request.Longitude, request.IpAddress, request.Location, request.Image);

                        return new ServiceResponse() { Status = true };
                    }
                    else
                    {
                        return new ServiceResponse() { Status = false, Error = "Error in image." };
                    }

                }
                else
                {
                    return new ServiceResponse() { Status = false, Error = "Invalid Token" };
                }

            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { Status = false, ExceptionMessage = ex.Message };
            }
        }
        #endregion

        #region Method
        /// <summary>
        /// Sends the email for forgot password.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <returns>boolean result</returns>
        public bool SendEmailForForgotPassword(int userID)
        {
            string failReason = string.Empty;
            EmailInfo emailInfo = new EmailInfo();
            var usr = _user.GetUserById(userID);
            FormBot.Entity.User uv = DBClient.DataTableToList<FormBot.Entity.User>(usr.Tables[0]).FirstOrDefault();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var provider = new MachineKeyProtectionProvider();
            userManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(provider.Create("EmailConfirmation"));
            var code = userManager.GeneratePasswordResetToken(uv.AspNetUserId);
            emailInfo.TemplateID = 9;
            emailInfo.FirstName = uv.FirstName;
            emailInfo.LastName = uv.LastName;
            var url = new System.Web.Mvc.UrlHelper(System.Web.HttpContext.Current.Request.RequestContext);
            emailInfo.PasswordLink = url.Action("ResetPassword", "Account", new { UserId = uv.AspNetUserId, code = code });
            //SMTPDetails smtpDetail = new SMTPDetails();
            //smtpDetail.MailFrom = ProjectSession.MailFrom;
            //smtpDetail.SMTPUserName = ProjectSession.SMTPUserName;
            //smtpDetail.SMTPPassword = ProjectSession.SMTPPassword;
            //smtpDetail.SMTPHost = ProjectSession.SMTPHost;
            //smtpDetail.SMTPPort = Convert.ToInt32(ProjectSession.SMTPPort);
            //smtpDetail.IsSMTPEnableSsl = ProjectSession.IsSMTPEnableSsl;
            EmailTemplate emailTempalte = _emailBAL.GetEmailTemplateByID(emailInfo.TemplateID);
            string body = _emailBAL.GetEmailBody(emailInfo, emailTempalte);

            bool status = false;
            string FailReason = string.Empty;
            try
            {
                if (body != null && !string.IsNullOrEmpty(body))
                {
                    QueuedEmail objQueuedEmail = new QueuedEmail();
                    objQueuedEmail.FromEmail = ProjectSession.MailFrom;
                    objQueuedEmail.Body = body;
                    objQueuedEmail.Subject = emailTempalte.Subject;
                    objQueuedEmail.ToEmail = uv.Email;
                    objQueuedEmail.CreatedDate = DateTime.Now;
                    objQueuedEmail.ModifiedDate = DateTime.Now;
                    _emailBAL.InsertUpdateQueuedEmail(objQueuedEmail);
                    status = true;
                }
            }
            catch (Exception ex)
            {
                FailReason = ex.Message;
            }

            //bool status = MailHelper.SendMail(smtpDetail, uv.Email, null, null, emailTempalte.Subject, body, null, true, ref failReason, false);
            return status;
        }



        [HttpPost]
        public ServiceResponse ErrorLogInsert(ErrorLog request)
        {
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(request.UserID), request.ApiToken))
                {
                    _createJobBAL.InsertErrorLogAPI(request.Error, Convert.ToInt32(request.UserID), request.DeviceToken, Convert.ToDateTime(request.DateTime));
                    return new ServiceResponse() { Status = true };
                }
                else
                {
                    return new ServiceResponse() { Status = false, Error = "Invalid Token" };
                }

            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { Status = false, ExceptionMessage = ex.Message };
            }
        }



        /// <summary>
        /// Files the upload.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="jobID">The user identifier.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>boolean object</returns>
        public bool FileUpload(Image image, string jobID, string fileName, bool isClassic = true, int VisitCheckListItemId = 0)
        {
            try
            {
                string proofDocumentsFolder = ProjectSession.ProofDocuments;
                string proofDocumentsFolderURL = ProjectSession.ProofDocumentsURL;
                if (!string.IsNullOrEmpty(jobID))
                {
                    proofDocumentsFolder = Path.Combine(proofDocumentsFolder + "\\" + "JobDocuments" + "\\" + jobID + "\\");
                    proofDocumentsFolderURL = proofDocumentsFolderURL + "\\" + "JobDocuments" + "\\" + jobID + "\\";
                }

                if (!isClassic)
                {
                    if (VisitCheckListItemId == 0)
                    {
                        proofDocumentsFolder = Path.Combine(proofDocumentsFolder + "\\" + "JobDocuments" + "\\" + jobID + "\\DefaultFolder");
                        proofDocumentsFolderURL = proofDocumentsFolderURL + "\\" + "JobDocuments" + "\\" + jobID + "\\DefaultFolder";
                    }
                    else
                    {
                        proofDocumentsFolder = Path.Combine(proofDocumentsFolder + "\\" + "JobDocuments" + "\\" + jobID + "\\checklistitem\\" + VisitCheckListItemId.ToString());
                        proofDocumentsFolderURL = proofDocumentsFolderURL + "\\" + "JobDocuments" + "\\" + jobID + "\\checklistitem\\" + VisitCheckListItemId.ToString();
                    }
                }

                if (!Directory.Exists(proofDocumentsFolder))
                {
                    Directory.CreateDirectory(proofDocumentsFolder);
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
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

                image.Save(path);
                return true;
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return false;
            }

        }

        public bool FileUploadForJob(Image image, string jobID, string fileName, ref string ImagePath, bool isClassic = true, int VisitCheckListItemId = 0, string lat = "", string lon = "", string ImageTakenDate = "")
        {
            try
            {
                string proofDocumentsFolder = ProjectSession.ProofDocuments;
                string proofDocumentsFolderURL = ProjectSession.ProofDocumentsURL;


                if (!isClassic)
                {
                    if (VisitCheckListItemId == 0)
                    {
                        proofDocumentsFolder = Path.Combine(proofDocumentsFolder + "\\" + "JobDocuments" + "\\" + jobID + "\\DefaultFolder");
                        proofDocumentsFolderURL = proofDocumentsFolderURL + "\\" + "JobDocuments" + "\\" + jobID + "\\DefaultFolder";
                    }
                    else
                    {
                        proofDocumentsFolder = Path.Combine(proofDocumentsFolder + "\\" + "JobDocuments" + "\\" + jobID + "\\checklistitem\\" + VisitCheckListItemId.ToString());
                        proofDocumentsFolderURL = proofDocumentsFolderURL + "\\" + "JobDocuments" + "\\" + jobID + "\\checklistitem\\" + VisitCheckListItemId.ToString();
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(jobID))
                    {
                        proofDocumentsFolder = Path.Combine(proofDocumentsFolder + "\\" + "JobDocuments" + "\\" + jobID + "\\");
                        proofDocumentsFolderURL = proofDocumentsFolderURL + "\\" + "JobDocuments" + "\\" + jobID + "\\";
                    }
                }

                if (!Directory.Exists(proofDocumentsFolder))
                {
                    Directory.CreateDirectory(proofDocumentsFolder);
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
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

                Geotag(new Bitmap(image), Double.Parse(string.IsNullOrEmpty(lat) ? "0" : lat), Double.Parse(string.IsNullOrEmpty(lon) ? "0" : lon), ImageTakenDate).Save(path);

                if (!isClassic)
                {
                    if (VisitCheckListItemId == 0)
                    {
                        ImagePath = "JobDocuments\\" + jobID + "\\DefaultFolder\\" + fileName.Replace("%", "$");
                    }
                    else
                    {
                        ImagePath = "JobDocuments\\" + jobID + "\\checklistitem\\" + VisitCheckListItemId.ToString() + "\\" + fileName.Replace("%", "$");
                    }
                }
                //image.Save(path);
                return true;
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return false;
            }

        }

        /// <summary>
        /// Files the upload.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="jobID">The user identifier.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>boolean object</returns>
        public bool FileUpload(Image image, string jobID, string fileName, string chklstId, string folder, ref string path, ref string ImgPath, string Latitude = "", string Longitude = "", string SignatureDate = "")
        {
            try
            {
                string proofDocumentsFolder = ProjectSession.ProofDocuments;
                // string proofDocumentsFolderURL = ProjectSession.ProofDocumentsURL;

                if (string.IsNullOrEmpty(path))
                {

                    if (!string.IsNullOrEmpty(jobID))
                    {
                        proofDocumentsFolder = Path.Combine(proofDocumentsFolder + "\\" + "JobDocuments" + "\\" + jobID + "\\" + "\\checklistitem\\" + chklstId + "\\");
                        // proofDocumentsFolderURL = proofDocumentsFolderURL + "\\" + "JobDocuments" + "\\" + jobID + "\\";
                        ImgPath = "JobDocuments" + "\\" + jobID + "\\" + "\\checklistitem\\" + chklstId + "\\";
                    }

                    if (!Directory.Exists(proofDocumentsFolder))
                    {
                        Directory.CreateDirectory(proofDocumentsFolder);
                    }

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    path = Path.Combine(proofDocumentsFolder + "\\" + fileName.Replace("%", "$"));
                    ImgPath = Path.Combine(ImgPath + "\\" + fileName.Replace("%", "$"));
                }
                else
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    path = Path.Combine(proofDocumentsFolder + "\\" + path);
                    File.Delete(path);
                }
                // image.Save(path);

                //Bitmap bm = new Bitmap(image);
                //bm.Save(path, System.Drawing.Imaging.ImageFormat.Png);

                Image imageWithTags = GeotagPNG(new Bitmap(image), Double.Parse(string.IsNullOrEmpty(Latitude) ? "0" : Latitude), Double.Parse(string.IsNullOrEmpty(Longitude) ? "0" : Longitude), new DateTime(long.Parse(SignatureDate)).ToString("yyyy:MM:dd HH:mm:ss"));
                Bitmap bm = new Bitmap(imageWithTags);
                bm.Save(path, System.Drawing.Imaging.ImageFormat.Png);

                return true;
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return false;
            }

        }


        private void generatePDFfromImage(DataTable dt, string Destination)
        {

            //Bitmap bmp = WebsiteThumbnailImageGenerator.GetWebSiteThumbnail(address);

            if (!Directory.Exists(Directory.GetParent(Destination).ToString()))
            {
                Directory.CreateDirectory(Directory.GetParent(Destination).ToString());
            }
            else
            {
                if (File.Exists(Destination))
                {
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    File.Delete(Destination);
                }
            }

            PdfSharp.Pdf.PdfDocument doc = new PdfSharp.Pdf.PdfDocument();
            //PdfPage pg = new PdfPage();
            //pg.Size = (PdfSharp.PageSize)PageSize;
            foreach (DataRow dr in dt.Rows)
            {

                string Source = Path.Combine(ConfigurationManager.AppSettings["ProofUploadFolder"].ToString(), dr["Path"].ToString());

                if (!File.Exists(Source))
                {
                    continue;
                }
                double height2 = 0;// bmp.Height;
                PdfSharp.Pdf.PdfPage pg = new PdfSharp.Pdf.PdfPage();
                pg.Size = (PdfSharp.PageSize)PageSize.A4;
                bool FitInSinglePage = false;

                doc.Pages.Add(pg);
                XGraphics xgr = XGraphics.FromPdfPage(pg);
                XImage img = XImage.FromFile(Source);

                double resHeight = 0;
                if (pg.Width.Value < img.PointWidth)
                {
                    resHeight = (pg.Width.Value * img.PointHeight) / img.PointWidth;
                }
                else
                {
                    resHeight = img.PixelHeight;
                }
                double heightScale = 1;
                if (pg.Height.Value > img.PointHeight)
                {
                    heightScale = img.PointHeight;
                }
                else
                {
                    heightScale = img.PointHeight;
                }
                if (FitInSinglePage)
                {
                    xgr.DrawImage(img, 0, 0, pg.Width.Value, pg.Height.Value);
                    //doc.Save(Destination);
                    //   doc.Close();
                    img.Dispose();
                    img = null;
                    xgr.Dispose();
                    xgr = null;

                    doc.Dispose();
                    doc = null;
                    //  System.IO.File.Delete(FullPath + ".png");
                    break;
                }
                else
                {
                    xgr.DrawImage(img, 0, 0, pg.Width.Value, resHeight);
                }

                height2 = resHeight;
                int i = 1;

                double calcHeight = resHeight;
                if (img.PointHeight > pg.Height.Value)
                {
                    height2 = height2 - pg.Height.Value;
                    while (height2 % pg.Height.Value > 0)
                    {
                        pg = new PdfSharp.Pdf.PdfPage();
                        pg.Size = (PdfSharp.PageSize)PageSize.A4;

                        doc.Pages.Add(pg);
                        //if (pg.Height.Value > height2)
                        //{
                        //    heightScale = img.PointHeight;
                        //}
                        //else
                        //{
                        //    heightScale = img.PointHeight;
                        //}
                        xgr.Dispose();
                        xgr = null;
                        xgr = XGraphics.FromPdfPage(doc.Pages[i]);
                        xgr.DrawImage(img, 0, -pg.Height.Value * i, pg.Width.Value, resHeight);
                        i = i + 1;
                        height2 = height2 - pg.Height.Value;
                    }
                }

                img.Dispose();
                img = null;
                xgr.Dispose();
                xgr = null;
            }


            doc.Save(Destination);
            doc.Close();

            doc.Dispose();
            doc = null;
            //System.IO.File.Delete(FullPath + ".png");

        }

        /// <summary>
        /// Deletes the directory.
        /// </summary>
        /// <param name="path">The path.</param>
        private void DeleteDirectoryPhoto(string path)
        {

            try
            {
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                ////Delete all files from the Directory
                System.IO.File.Delete(path);
            }
            catch (Exception ex)
            {

            }

        }

        public class MachineKeyProtectionProvider : IDataProtectionProvider
        {
            public IDataProtector Create(params string[] purposes)
            {
                return new MachineKeyDataProtector(purposes);
            }
        }

        public class MachineKeyDataProtector : IDataProtector
        {
            private readonly string[] _purposes;
            public MachineKeyDataProtector(string[] purposes)
            {
                this._purposes = purposes;
            }

            public byte[] Protect(byte[] userData)
            {
                return MachineKey.Protect(userData, this._purposes);
            }

            public byte[] Unprotect(byte[] protectedData)
            {
                return MachineKey.Unprotect(protectedData, this._purposes);
            }
        }



        /// <summary>
        /// Updates visitstatus.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>service response</returns>
        [HttpPost]
        public ServiceResponse UpdateVisitStatus(UpdateVisitStatusRequest request)
        {
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(request.UserID), request.ApiToken))
                {
                    DateTime dt = new DateTime(Convert.ToInt64(request.CompletedDate));

                    // _scheduleBAL.AcceptRejectSchedule(Convert.ToInt32(request.Status), Convert.ToInt32(request.JobSchedulingID));
                    //DataSet dsSCAInfo = _scheduleBAL.ChangeVisitStatus(Convert.ToInt32(request.JobSchedulingID), Convert.ToInt32(request.VisitStatus), dt);

                    DataSet dsSCAInfo = _scheduleBAL.ChangeVisitStatus(!string.IsNullOrEmpty(request.JobSchedulingID) ? Convert.ToInt32(request.JobSchedulingID) : 0, !string.IsNullOrEmpty(request.VisitStatus) ? Convert.ToInt32(request.VisitStatus) : 0, dt);
                    int intjobschedulingId = !string.IsNullOrEmpty(request.JobSchedulingID) ? Convert.ToInt32(request.JobSchedulingID) : 0;
                    DataSet dataForHistoryLog = _scheduleBAL.GetJobSchedulingDataForHistoryLog(intjobschedulingId, null);
                    string visitId = string.Empty;
                    string JobID = string.Empty;
                    if (dataForHistoryLog != null && dataForHistoryLog.Tables.Count > 0)
                    {
                        visitId = dataForHistoryLog.Tables[0].Rows[0]["VisitUniqueId"]
                            .ToString();
                        JobID = dataForHistoryLog.Tables[0].Rows[0]["JobID"].ToString();
                    }
                    string JobHistoryMessage = string.Empty;
                    string description = string.Empty;
                    if (Convert.ToInt32(request.VisitStatus) == 2)
                    {
                        // if (dsSCAInfo != null && dsSCAInfo.Tables.Count > 0 && dsSCAInfo.Tables[0] != null && dsSCAInfo.Tables[0].Rows.Count > 0)
                        if (dsSCAInfo != null && dsSCAInfo.Tables[0].Rows.Count > 0)
                        {
                            EmailInfo emailInfo = new EmailInfo();
                            //live or staging
                            emailInfo.TemplateID = 38;

                            //local
                            //emailInfo.TemplateID = 1037;
                            emailInfo.SolarCompanyDetails = Convert.ToString(dsSCAInfo.Tables[0].Rows[0]["SolarCompanyFullName"]);
                            _emailBAL.ComposeAndSendEmail(emailInfo, Convert.ToString(dsSCAInfo.Tables[0].Rows[0]["Email"]));

                        }
                        
                        JobHistoryMessage = "has completed a visit -<b class=\"blue-title\"> (" + JobID + ") JobRefNo</b>";
                        description = "Visit id: <b class=\"blue-title\">" + visitId + "</b> visit status has been changed to completed.";
                        Common.SaveJobHistorytoXML(Convert.ToInt32(JobID), JobHistoryMessage, "Scheduling", "ChangeVisitStatus", ProjectSession.LoggedInName, false, description);
                    }
                    else if (Convert.ToInt32(request.VisitStatus)== 1)
                    {
                        JobHistoryMessage = "has re-opened a visit -<b class=\"blue-title\"> (" + JobID + ") JobRefNo</b>";
                        description = "Visit id: <b class=\"blue-title\">" + visitId + "</b> visit status has been changed to open.";
                        Common.SaveJobHistorytoXML(Convert.ToInt32(JobID), JobHistoryMessage, "Scheduling", "ChangeVisitStatus", ProjectSession.LoggedInName, false, description);
                    }
                    return new ServiceResponse() { Status = true };
                }
                else
                {
                    return new ServiceResponse() { Status = false, Error = "Invalid Token" };
                }
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { Status = false, ExceptionMessage = ex.Message };
            }
        }



        [HttpPost]
        public ServiceResponse GetCheckListItemsByJobSchedulingId(ScheduleStatusRequest request)
        {
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(request.UserID), request.ApiToken))
                {
                    DataSet dataset = _checkListBAL.GetCheckListItemsByJobScheduleId(!string.IsNullOrEmpty(request.JobSchedulingID) ? Convert.ToString(request.JobSchedulingID) : "");
                    if (dataset.Tables.Count > 0)
                    {
                        return new ServiceResponse() { Data = Newtonsoft.Json.JsonConvert.SerializeObject(dataset.Tables[0]), Status = true };
                    }

                    return new ServiceResponse() { Status = true };
                }

                return new ServiceResponse() { Status = false, Error = "Invalid Token" };
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { ExceptionMessage = ex.Message, Status = false };
            }
        }


        /// <summary>
        /// Jobs the schedule.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Service Response</returns>
        [HttpPost]
        public ServiceResponse GetVisitSignature(GetVisitSignature request)
        {
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(request.UserID), request.ApiToken))
                {
                    DataSet dataset = _createJobBAL.GetVisitSignatureApi(!string.IsNullOrEmpty(request.JobSchedulingID) ? Convert.ToInt32(request.JobSchedulingID) : 0, !string.IsNullOrEmpty(request.VisitCheckListItemId) ? Convert.ToInt32(request.VisitCheckListItemId) : 0);
                    if (dataset.Tables.Count > 0)
                    {
                        return new ServiceResponse() { Data = HelperMethods.ConvertToJSON(dataset.Tables[0]), Status = true };
                    }

                    return new ServiceResponse() { Status = true };
                }

                return new ServiceResponse() { Status = false, Error = "Invalid Token" };
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { ExceptionMessage = ex.Message, Status = false };
            }
        }

        [HttpPost]
        public ServiceResponse GetVisitSignaturePath(VisitSignatureRequest request)
        {
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(request.UserID), request.ApiToken))
                {
                    string path = _createJobBAL.GetVisitSignaturePath(Convert.ToInt32(request.JobSchedulingId), Convert.ToInt32(request.CheckListItemId), Convert.ToInt32(request.SignatureTypeId));
                    if (!string.IsNullOrEmpty(path))
                    {
                        //return new ServiceResponse() { Data = HelperMethods.ConvertToJSON(dataset.Tables[0]), Status = true };
                    }

                    return new ServiceResponse() { Status = true };
                }

                return new ServiceResponse() { Status = false, Error = "Invalid Token" };
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { ExceptionMessage = ex.Message, Status = false };
            }
        }


        [HttpPost]
        public ServiceResponse GetUserSignature(UserSignature request)
        {
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(request.UserID), request.ApiToken))
                {
                    string baseImage;

                    string path = _createJobBAL.GetUserSignaturePath(Convert.ToInt32(request.UserID));

                    if (string.IsNullOrEmpty(path))
                    {
                        baseImage = string.Empty;
                    }
                    else
                    {
                        byte[] byteImage = ReadImageFile(Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + "UserDocuments" + "\\" + request.UserID, path));
                        baseImage = Convert.ToBase64String(byteImage);
                    }
                    request.Image = baseImage;
                    return new ServiceResponse() { Data = HelperMethods.ConvertToJSON(request), Status = true };
                }
                else
                {
                    return new ServiceResponse() { Status = false, Error = "Invalid Token" };
                }
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { Status = false, ExceptionMessage = ex.Message };
            }
        }



        [HttpPost]
        public ServiceResponse SaveUserSignature(UserSignature request)
        {
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(request.UserID), request.ApiToken))
                {
                    byte[] imageByte = Convert.FromBase64String(request.Image);
                    MemoryStream ms = new MemoryStream(imageByte);
                    Image image = Image.FromStream(ms);
                    string filename = DateTime.Now.Ticks.ToString() + "_" + request.UserID + ".png";
                    string OldFileName = _createJobBAL.GetUserSignaturePath(Convert.ToInt32(request.UserID), filename);
                    SignatureUpload(image, request.UserID, filename, OldFileName);
                    // _createJobBAL.UpdateUserSignaturePath(Convert.ToInt32(request.UserID), filename);
                    return new ServiceResponse() { Status = true };
                }
                else
                {
                    return new ServiceResponse() { Status = false, Error = "Invalid Token" };
                }
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { Status = false, ExceptionMessage = ex.Message };
            }
        }

        /// <summary>
        /// Files the upload.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="jobID">The user identifier.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>boolean object</returns>
        public bool SignatureUpload(Image image, string Userid, string fileName, string oldFileName)
        {
            try
            {
                string proofDocumentsFolder = ProjectSession.ProofDocuments;
                string proofDocumentsFolderURL = ProjectSession.ProofDocumentsURL;
                if (!string.IsNullOrEmpty(Userid))
                {
                    proofDocumentsFolder = Path.Combine(proofDocumentsFolder + "\\" + "UserDocuments" + "\\" + Userid + "\\");
                    proofDocumentsFolderURL = proofDocumentsFolderURL + "\\" + "UserDocuments" + "\\" + Userid + "\\";
                }

                if (!Directory.Exists(proofDocumentsFolder))
                {
                    Directory.CreateDirectory(proofDocumentsFolder);
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
                string path = Path.Combine(proofDocumentsFolder + "\\" + fileName.Replace("%", "$"));
                image.Save(path);

                if (!string.IsNullOrEmpty(oldFileName))
                {
                    string oldPath = Path.Combine(proofDocumentsFolder + "\\" + oldFileName.Replace("%", "$"));
                    if (System.IO.File.Exists(oldPath))
                    {
                        File.Delete(oldPath);
                    }
                }


                return true;
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return false;
            }

        }

        /// <summary>
        /// Get serial number
        /// </summary>
        /// <returns>ServiceResponse</returns>
        [HttpPost]
        public ServiceResponse GetSerialNumberOfJob(JobRequest request)
        {
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(request.UserID), request.ApiToken))
                {
                    string serialNumber = _createJobBAL.GetSerialNumberOfJob(Convert.ToInt32(request.JobID));
                    return new ServiceResponse() { Status = true, Data = serialNumber };
                }
                else
                {
                    return new ServiceResponse() { Status = false, Error = "Invalid Token" };
                }
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { Status = false, ExceptionMessage = ex.Message };
            }

        }

        [HttpPost]
        public ServiceResponse MakeVisitCheckListItemCompleted(VisitSignatureRequest request)
        {
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(request.UserID), request.ApiToken))
                {
                    _createJobBAL.MakeVisitCheckListItemCompleted(Convert.ToInt32(request.VisitCheckListItemId), Convert.ToBoolean(request.IsCompleted));
                    return new ServiceResponse() { Status = true };
                }
                else
                {
                    return new ServiceResponse() { Status = false, Error = "Invalid Token" };
                }
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { ExceptionMessage = ex.Message, Status = false };
            }
        }

        public void createFileForChecklistNumber(VisitCheckListItem request, string visitUniqueId)
        {
            if (string.IsNullOrEmpty(request.SerialNumFileName))
                return;

            string docpath = Path.Combine("JobDocuments", request.JobId, "OTHER", request.SerialNumFileName.ToLower().Contains(".txt") ? visitUniqueId + "_" + request.SerialNumFileName : visitUniqueId + "_" + request.SerialNumFileName + ".txt");
            string path = Path.Combine(ConfigurationManager.AppSettings["ProofUploadFolder"].ToString(), docpath);
            DataSet ds = _createJobBAL.GetSerialNo(Convert.ToInt32(request.VisitCheckListItemId));
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0][1].ToString() == "true")
                    {
                        string SerialNumbers = ds.Tables[0].Rows[0][0].ToString();
                        if (!string.IsNullOrEmpty(SerialNumbers))
                            request.CheckListSerialNumbers = ds.Tables[0].Rows[0][0].ToString() + "\r\n" + request.CheckListSerialNumbers;
                    }
                    else
                        request.CheckListSerialNumbers = ds.Tables[0].Rows[0][0].ToString();

                }
            }
            bool isFileExists = File.Exists(path);

            if (File.Exists(path))
            {
                GC.WaitForPendingFinalizers();
                GC.Collect();

                string fileName = Path.GetFileNameWithoutExtension(path) + "_" + DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss") + Path.GetExtension(path);
                string destinationDirectory = ConfigurationManager.AppSettings["ProofUploadFolder"] + "\\" + "JobDocuments" + "\\" + request.JobId + "\\" + "DeletedDocuments";
                if (!Directory.Exists(destinationDirectory))
                {
                    Directory.CreateDirectory(destinationDirectory);
                }
                string destPath = System.IO.Path.Combine(destinationDirectory, fileName);
                System.IO.File.Copy(path, destPath, true);

                File.Delete(path);
            }

            if (!Directory.Exists(Directory.GetParent(path).ToString()))
            {
                Directory.CreateDirectory(Directory.GetParent(path).ToString());
            }

            System.IO.File.WriteAllText(@path, request.CheckListSerialNumbers);

            if (!isFileExists)
            {
                _createJobBAL.InsertCESDocuments(Convert.ToInt32(request.JobId), docpath, Convert.ToInt32(request.UserID), "OTHER", "");
            }
        }

        static Image Geotag(Image original, double lat, double lng, string ImageTakenDate = "")
        {
            // These constants come from the CIPA DC-008 standard for EXIF 2.3
            const short ExifTypeByte = 1;
            const short ExifTypeAscii = 2;
            const short ExifTypeRational = 5;

            const int ExifTagGPSVersionID = 0x0000;
            const int ExifTagGPSLatitudeRef = 0x0001;
            const int ExifTagGPSLatitude = 0x0002;
            const int ExifTagGPSLongitudeRef = 0x0003;
            const int ExifTagGPSLongitude = 0x0004;
            const int ExifTagDateTimeOriginal = 0x9003;
            //const int ExifTagDateTaken = 0x0132;

            char latHemisphere = 'N';
            if (lat < 0)
            {
                latHemisphere = 'S';
                lat = -lat;
            }
            char lngHemisphere = 'E';
            if (lng < 0)
            {
                lngHemisphere = 'W';
                lng = -lng;
            }

            MemoryStream ms = new MemoryStream();
            original.Save(ms, ImageFormat.Jpeg);
            ms.Seek(0, SeekOrigin.Begin);

            Image img = Image.FromStream(ms);
            AddProperty(img, ExifTagGPSVersionID, ExifTypeByte, new byte[] { 2, 3, 0, 0 });
            AddProperty(img, ExifTagGPSLatitudeRef, ExifTypeAscii, new byte[] { (byte)latHemisphere, 0 });
            AddProperty(img, ExifTagGPSLatitude, ExifTypeRational, ConvertToRationalTriplet(lat));
            AddProperty(img, ExifTagGPSLongitudeRef, ExifTypeAscii, new byte[] { (byte)lngHemisphere, 0 });
            AddProperty(img, ExifTagGPSLongitude, ExifTypeRational, ConvertToRationalTriplet(lng));


            // ImageTakenDate : Date Format must be "yyyy:MM:dd HH:mm:ss"
            byte[] arrProp = null;
            if (string.IsNullOrEmpty(ImageTakenDate))
                ImageTakenDate = DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss");

            arrProp = Encoding.ASCII.GetBytes(ImageTakenDate);
            AddProperty(img, ExifTagDateTimeOriginal, ExifTypeAscii, arrProp);

            return img;
        }

        static Image GeotagPNG(Image original, double lat, double lng, string ImageTakenDate = "")
        {
            // These constants come from the CIPA DC-008 standard for EXIF 2.3
            const short ExifTypeByte = 1;
            const short ExifTypeAscii = 2;
            const short ExifTypeRational = 5;

            const int ExifTagGPSVersionID = 0x0000;
            const int ExifTagGPSLatitudeRef = 0x0001;
            const int ExifTagGPSLatitude = 0x0002;
            const int ExifTagGPSLongitudeRef = 0x0003;
            const int ExifTagGPSLongitude = 0x0004;
            const int ExifTagDateTimeOriginal = 0x9003;
            //const int ExifTagDateTaken = 0x0132;

            char latHemisphere = 'N';
            if (lat < 0)
            {
                latHemisphere = 'S';
                lat = -lat;
            }
            char lngHemisphere = 'E';
            if (lng < 0)
            {
                lngHemisphere = 'W';
                lng = -lng;
            }

            MemoryStream ms = new MemoryStream();
            original.Save(ms, ImageFormat.Png);
            ms.Seek(0, SeekOrigin.Begin);

            Image img = Image.FromStream(ms);
            AddProperty(img, ExifTagGPSVersionID, ExifTypeByte, new byte[] { 2, 3, 0, 0 });
            AddProperty(img, ExifTagGPSLatitudeRef, ExifTypeAscii, new byte[] { (byte)latHemisphere, 0 });
            AddProperty(img, ExifTagGPSLatitude, ExifTypeRational, ConvertToRationalTriplet(lat));
            AddProperty(img, ExifTagGPSLongitudeRef, ExifTypeAscii, new byte[] { (byte)lngHemisphere, 0 });
            AddProperty(img, ExifTagGPSLongitude, ExifTypeRational, ConvertToRationalTriplet(lng));


            // ImageTakenDate : Date Format must be "yyyy:MM:dd HH:mm:ss"
            byte[] arrProp = null;
            if (string.IsNullOrEmpty(ImageTakenDate))
                ImageTakenDate = DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss");

            arrProp = Encoding.ASCII.GetBytes(ImageTakenDate);
            AddProperty(img, ExifTagDateTimeOriginal, ExifTypeAscii, arrProp);

            return img;
        }

        static byte[] ConvertToRationalTriplet(double value)
        {
            int degrees = (int)Math.Floor(value);
            value = (value - degrees) * 60;
            int minutes = (int)Math.Floor(value);
            value = (value - minutes) * 60 * 100;
            int seconds = (int)Math.Round(value);
            byte[] bytes = new byte[3 * 2 * 4]; // Degrees, minutes, and seconds, each with a numerator and a denominator, each composed of 4 bytes
            int i = 0;
            Array.Copy(BitConverter.GetBytes(degrees), 0, bytes, i, 4); i += 4;
            Array.Copy(BitConverter.GetBytes(1), 0, bytes, i, 4); i += 4;
            Array.Copy(BitConverter.GetBytes(minutes), 0, bytes, i, 4); i += 4;
            Array.Copy(BitConverter.GetBytes(1), 0, bytes, i, 4); i += 4;
            Array.Copy(BitConverter.GetBytes(seconds), 0, bytes, i, 4); i += 4;
            Array.Copy(BitConverter.GetBytes(100), 0, bytes, i, 4);
            return bytes;
        }

        static void AddProperty(Image img, int id, short type, byte[] value)
        {
            PropertyItem pi = img.PropertyItems[0];
            pi.Id = id;
            pi.Type = type;
            pi.Len = value.Length;
            pi.Value = value;
            img.SetPropertyItem(pi);
        }

        /// <summary>
        /// Get Reference Photos
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>service response</returns>
        [HttpPost]
        public ServiceResponse ReferencePhoto(JobStageRequest request)
        {
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(request.UserID), request.ApiToken))
                {

                    DataSet dsReferncePhoto = _scheduleBAL.GetReferencePhotosForAPI(Convert.ToInt32(request.JobID));
                    foreach (DataRow dr in dsReferncePhoto.Tables[0].Rows)
                    {
                        dr["Path"] = Path.GetFileName(dr["Path"].ToString());
                    }
                    return new ServiceResponse() { Status = true, Data = HelperMethods.ConvertToJSON(dsReferncePhoto.Tables[0]) };
                }
                else
                {
                    return new ServiceResponse() { Status = false, Error = "Invalid Token" };
                }
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { Status = false, ExceptionMessage = ex.Message };
            }
        }

        [HttpPost]
        public ServiceResponse CheckAutoRequestStatusOverUserId(AutoRequestApprovedRequest request)
        {
            try
            {
                if (_user.IsValidToken(request.UserID, request.ApiToken))
                {

                    if (request.UserID > 0)
                    {
                        return new ServiceResponse() { Status = true, Data = (_user.CheckAutoRequestStatusOverUserId(request.UserID, request.IsApproved, request.IsGet)) };
                    }
                    else
                        return new ServiceResponse() { Status = true, Data = (false) };
                }
                else
                {
                    return new ServiceResponse() { Status = false, Error = "Invalid Token" };
                }
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { Status = false, Data = (false) };
            }
        }

        //public ServiceResponse SPVSerialNumberProductVerification(SPVSerialNumberProductVerificationRequest request)
        public void SPVSerialNumberProductVerification(ref List<JobSerialNumberRequestFromAutoSyncMethod> lstSerialNumber,int JobId)
        {
            try
            {
                //var lstSerialNumber = request.lstSerialNumber;
                //if (_user.IsValidToken(request.UserID, request.ApiToken))
                //{
                var ManufactureBrandUnique = lstSerialNumber.Select(m => new { m.Brand, m.Model }).Distinct().ToList();
                if (!(ManufactureBrandUnique.Count > 1))
                {
                    foreach (var item in ManufactureBrandUnique)
                    {
                        var filterSerialNumber = lstSerialNumber.Where(m => m.Brand == item.Brand && m.Model == item.Model && m.IsVerified == null);
                        string commaSerialNumber = string.Join(",", filterSerialNumber.Select(m => m.SerialNumber));
                        List<string> Serialnumberlist = filterSerialNumber.Select(m => m.SerialNumber).ToList();
                        DataSet dsSpv = _createJobBAL.GetSPVVerificationUrlSerialNumberAPI(item.Brand, item.Model, commaSerialNumber, JobId);
                        var verifiedSerialNumber = dsSpv.Tables[1].AsEnumerable().Select(s => s.Field<string>("SerialNumber")).ToList();
                        var notVerifiedSerialNumber = Serialnumberlist.Except(verifiedSerialNumber).ToList();
                        var ProductVerificationXMLPath = ConfigurationManager.AppSettings["ProductVerificationXMLPath"].ToString();
                        var ServerCertificate = ConfigurationManager.AppSettings["ServerCertificate"].ToString();
                        SPVVerification objSPVVerification = new SPVVerification(_spvLog,_createJobBAL);
                        var SPvDatatable = objSPVVerification.SPVProductVerificationAPI(dsSpv, ProductVerificationXMLPath, ServerCertificate, notVerifiedSerialNumber, verifiedSerialNumber);
                        //_createJobBAL.UpdateVerifiedSerialNumber(SPvDatatable);
                        if (SPvDatatable.Rows.Count > 0)
                        {
                            foreach (DataRow dr in SPvDatatable.Rows)
                            {
                                lstSerialNumber.Where(m => m.SerialNumber == Convert.ToString(dr["SerialNumber"])).Select(m => { m.IsVerified = Convert.ToBoolean(dr["Verified"]); return m; }).ToList();
                            }
                        }
                    }
                }

                //return new ServiceResponse() { Status = true, Data = HelperMethods.ConvertToJSON(lstSerialNumber) };
                //}
                //else
                //{
                //    return new ServiceResponse() { Status = false, Error = "Invalid Token" };
                //}
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                //return new ServiceResponse() { Status = false, Data = (false) };
            }
        }
        [HttpPost]
        public ServiceResponse GetJobPanelDetails(JobRequest request)
        {
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(request.UserID), request.ApiToken))
                {
                    DataSet dsjobPanel = _createJobBAL.GetJobPanelDetails(Convert.ToInt32(request.JobID));
                    List<JobPanelDetails> lstJobPanelDetails = new List<JobPanelDetails>();
                    if (dsjobPanel != null)
                    {
                        if (dsjobPanel.Tables.Count > 0)
                        {
                            lstJobPanelDetails = dsjobPanel.Tables[0].ToListof<JobPanelDetails>();
                        }
                    }
                    return new ServiceResponse() { Status = true, Data = HelperMethods.ConvertToJSON(lstJobPanelDetails) };
                }
                else
                {
                    return new ServiceResponse() { Status = false, Data = "Invalid Token" };
                }
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { ExceptionMessage = ex.Message, Status = false };
            }
        }
        [HttpPost]
        public ServiceResponse BulkUpdateJobPanelDetails(JobRequest request)
        {
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(request.UserID), request.ApiToken))
                {
                    DataSet dsjobPanel = _createJobBAL.GetBulkJobPanelDetails(request.JobID);
                    List<JobPanelDetails> lstJobPanelDetails = new List<JobPanelDetails>();
                    if (dsjobPanel != null)
                    {
                        if (dsjobPanel.Tables.Count > 0)
                        {
                            lstJobPanelDetails = dsjobPanel.Tables[0].ToListof<JobPanelDetails>();
                        }
                    }
                    return new ServiceResponse() { Status = true, Data = HelperMethods.ConvertToJSON(lstJobPanelDetails) };
                }
                else
                {
                    return new ServiceResponse() { Status = false, Data = "Invalid Token" };
                }
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { ExceptionMessage = ex.Message, Status = false };
            }
        }
        public DataTable DtSPVSerialNumber()
        {
            DataTable dtSPVSerialNumber = new DataTable();

            dtSPVSerialNumber.Columns.Add("JobId", typeof(int));
            dtSPVSerialNumber.Columns.Add("SerialNumber", typeof(string));
            dtSPVSerialNumber.Columns.Add("Brand", typeof(string));
            dtSPVSerialNumber.Columns.Add("Model", typeof(string));
            dtSPVSerialNumber.Columns.Add("IsVerified", typeof(bool));

            return dtSPVSerialNumber;
        }
        [HttpPost]
        public ServiceResponse GetConnectedDevices(JobRequest request)
        {
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(request.UserID), request.ApiToken))
                {
                    List<UserDevice> lstConnectedDevices = _user.GetUserDeviceInfo(Convert.ToInt32(request.UserID));


                    return new ServiceResponse() { Status = true, Data = HelperMethods.ConvertToJSON(lstConnectedDevices) };
                }
                else
                {
                    return new ServiceResponse() { Status = false, Data = "Invalid Token" };
                }
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { ExceptionMessage = ex.Message, Status = false };
            }
        }
        [HttpPost]
        public ServiceResponse DeviceLogout(LogoutDevices request)
        {
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(request.UserID), request.ApiToken))
                {
                    if (request.IsLogOutAll == true)
                        _user.DeviceLogout(Convert.ToInt32(request.UserID), request.IsLogOutAll);
                    else
                        _user.DeviceLogout(Convert.ToInt32(request.UserDeviceId), request.IsLogOutAll);

                    return new ServiceResponse() { Status = true, Data = HelperMethods.ConvertToJSON(new List<UserDevice>()) };
                }
                else
                {
                    return new ServiceResponse() { Status = false, Data = "Invalid Token" };
                }
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { ExceptionMessage = ex.Message, Status = false };
            }
        }
        [HttpPost]
        public ServiceResponse CheckSPVRequiredByJobId(Entity.Job.CheckSPVRequiredByJobId request)
        {
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(request.UserID), request.ApiToken))
                {
                    Entity.Job.CheckSPVRequiredByJobId objCheckSPVRequiredByJobId = new CheckSPVRequiredByJobId();
                    objCheckSPVRequiredByJobId = _createJobBAL.CheckSPVRequiredByJobId(request.JobId);
                    return new ServiceResponse() { Status = true, Data = HelperMethods.ConvertToJSON(objCheckSPVRequiredByJobId) };
                }
                else
                {
                    return new ServiceResponse() { Status = false, Data = "Invalid Token" };
                }
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { ExceptionMessage = ex.Message, Status = false };
            }
        }
        [HttpPost]
        public ServiceResponse GetSCAndGlobalLevalSPVConfiguration(SCAndGlobalLevalSPVConfiguration request)
        {
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(request.UserId), request.ApiToken))
                {
                    List<CheckSPVrequired> lstSpvRequiredSolarCompanyWise = new List<CheckSPVrequired>();
                    lstSpvRequiredSolarCompanyWise = _createJobBAL.GetSPVRequiredOrNotOnSCAOrGlobalLevelOrManufacturer(JobIds: request.JobId).ToList();
                    return new ServiceResponse() { Status = true, Data = HelperMethods.ConvertToJSON(lstSpvRequiredSolarCompanyWise) };
                }
                else
                {
                    return new ServiceResponse() { Status = false, Data = "Invalid Token" };
                }
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { ExceptionMessage = ex.Message, Status = false };
            }
        }
        [HttpPost]
        public ServiceResponse GetPhotoFromPortalWithNullLatLongIsdeletedFlag(VisitchecklistPhotoIds request)
        {
            try
            {
                if (_user.IsValidToken(Convert.ToInt32(request.UserId), request.ApiToken))
                {
                    List<GetPhotoFromPortalWithNullLatLongIsdeletedFlagApiRequest> lstDeletedImagesFromServer = _createJobBAL.GetPhotoFromPortalWithNullLatLongIsdeletedFlag(string.Join(",",request.VisitChecklistPhotoIds));
                    return new ServiceResponse() { Status = true, Data = JsonConvert.SerializeObject(lstDeletedImagesFromServer) };
                }
                else
                    return new ServiceResponse() { Status = false, Data = "Invalid Token" };
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return new ServiceResponse() { ExceptionMessage = ex.Message, Status = false };
            }
        }
    }
    #endregion

}

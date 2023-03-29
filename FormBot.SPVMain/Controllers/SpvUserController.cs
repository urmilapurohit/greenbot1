using FormBot.BAL;
using FormBot.BAL.Service;
using FormBot.Entity;
using FormBot.Entity.Email;
using FormBot.Helper;
using FormBot.SPVMain.Infrastructure;
using FormBot.SPVMain.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace FormBot.SPVMain.Controllers
{
    [RoutePrefix("SpvUser")]
    public class SpvUserController : Controller
    {

        #region Properties
        private readonly ISpvUserBAL _spvUserBAL;
        private readonly IEmailBAL _emailBAL;
        private readonly Logger _log;
        public UserManager<ApplicationUser> UserManager { get; private set; }
        #endregion

        #region Constructor
        public SpvUserController(ISpvUserBAL spvUserBAL, IEmailBAL emailBAL)
        {
            this._spvUserBAL = spvUserBAL;
            this._emailBAL = emailBAL;
        }
        #endregion
        [HttpGet]
        [UserAuthorization]
        public ActionResult AllUser()
        {
            SpvUser spvUser = new SpvUser();
            if (ProjectSession.UserTypeId == 3)
            {
                spvUser.SpvUserTypeId = 4;
            }
            return View(spvUser);
        }

        [HttpGet]
        [UserAuthorization]
        public ActionResult Create(string id = null)
        {
            SpvUser spvUser = new SpvUser();
            spvUser.SpvUserTypeId = ProjectSession.UserTypeId;
            ViewBag.PostalAddressType = new SelectList(EnumExtensions.GetPostalAddressEnumList(), "ID", "Name");

            if (ProjectSession.UserTypeId == 3)
            {
                spvUser.SpvUserTypeId = 4;
            }
            //return View("~/Views/SpvUser/Create.cshtml");
            if (id != null)
            {
                DataSet ds = _spvUserBAL.GetSpvUserById(Convert.ToInt32(id));
                DataTable dt = ds.Tables[0];

                spvUser.SpvUserId = Convert.ToInt32(dt.Rows[0]["SpvUserId"]);
                spvUser.AspNetUserId = dt.Rows[0]["AspNetUserId"].ToString();
                spvUser.SpvUserTypeId = Convert.ToInt32(dt.Rows[0]["UserTypeID"]);
                spvUser.SpvRoleId = Convert.ToInt32(dt.Rows[0]["SpvRoleId"]);
                spvUser.FirstName = dt.Rows[0]["FirstName"].ToString();
                spvUser.LastName = dt.Rows[0]["LastName"].ToString();
                spvUser.UserName = dt.Rows[0]["UserName"].ToString();
                spvUser.Password = dt.Rows[0]["PasswordHash"].ToString();
                spvUser.Email = dt.Rows[0]["Email"].ToString();
                spvUser.IsActive = Convert.ToBoolean(dt.Rows[0]["IsActive"]);
                spvUser.Phone = dt.Rows[0]["Phone"].ToString();
                spvUser.Mobile = dt.Rows[0]["Mobile"].ToString();
                spvUser.UnitTypeID = Convert.ToInt32(dt.Rows[0]["UnitTypeID"]);
                spvUser.UnitNumber = dt.Rows[0]["UnitNumber"].ToString();
                spvUser.StreetNumber = dt.Rows[0]["StreetNumber"].ToString();
                spvUser.StreetName = dt.Rows[0]["StreetName"].ToString();
                spvUser.StreetTypeID = Convert.ToInt32(dt.Rows[0]["StreetTypeID"]);
                spvUser.Town = dt.Rows[0]["Town"].ToString();
                spvUser.State = dt.Rows[0]["State"].ToString();
                spvUser.PostCode = dt.Rows[0]["PostCode"].ToString();
                spvUser.PostalDeliveryNumber = dt.Rows[0]["PostalDeliveryNumber"].ToString();
                spvUser.PostalAddressID = Convert.ToInt32(dt.Rows[0]["PostalAddressID"]);
                spvUser.ManufacturerName = dt.Rows[0]["ManufacturerName"].ToString();
            }

            //DataTable dt = ds.Tables[0];
            //List<SpvPanelDetails> objspvPanelDetails = new List<SpvPanelDetails>();
            //foreach (DataRow dr in dt.Rows)
            //{
            //    objspvPanelDetails.Add(dr);
            //}


            return View(spvUser);
        }
        [HttpPost]
        [UserAuthorization]
        public async Task<JsonResult> Create(SpvUser SpvuserView)
        {
            string guid = SpvuserView.Guid;

            // SpvuserView.SpvUserTypeId = ProjectSession.SpvUserTypeId;
            // object isUserExists;
            RequiredValidationField(SpvuserView);
            IdentityResult result;
            if (SpvuserView.SpvUserId != 0)
            {
                ModelState.Remove("Password");
            }

            if (ModelState.IsValid)
            {
                string Id = null;
                ViewBag.PostalAddressType = new SelectList(EnumExtensions.GetPostalAddressEnumList(), "ID", "Name");
                UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                UserManager.UserValidator = new UserValidator<ApplicationUser>(UserManager) { AllowOnlyAlphanumericUserNames = false };
                if (SpvuserView.SpvUserId == 0)
                {
                    var user = new ApplicationUser() { Email = SpvuserView.Email, UserName = SpvuserView.UserName.Replace(" ", ""), PasswordHash = SpvuserView.Password, IsResetPwd = false };
                    //IsSPVUser =true };

                    result = await UserManager.CreateAsync(user, SpvuserView.Password);
                    Id = user.Id;
                }
                else
                {
                    UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                    var userDetails = UserManager.FindById(SpvuserView.AspNetUserId);

                    if (userDetails != null)
                    {
                        userDetails.Email = SpvuserView.Email;
                        userDetails.UserName = SpvuserView.UserName.Replace(" ", "");
                        // userDetails.CreatedPwdDate = userView.CreatedpwdDate;

                        PasswordHasher hasher = new PasswordHasher();
                        if (SpvuserView.Password != null)
                        {
                            userDetails.PasswordHash = hasher.HashPassword(SpvuserView.Password);
                        }
                    }

                    result = await UserManager.UpdateAsync(userDetails);
                    Id = userDetails.Id;

                }
                string userName = SpvuserView.UserName;

                if (result.Succeeded)
                {
                    SpvuserView.AspNetUserId = Id;
                    SpvuserView.CreatedDate = DateTime.Today;
                    SpvuserView.ModifiedDate = DateTime.Today;
                    SpvuserView.Logo = "Images/logo.png";
                    SpvuserView.Theme = 1;

                    if (SpvuserView.AddressId == 2)
                    {
                        SpvuserView.IsPostalAddress = true;
                    }
                    else
                    {
                        SpvuserView.IsPostalAddress = false;
                    }
                    int userID = _spvUserBAL.CreateSpvUser(SpvuserView);
                    int uID = userID + 1;
                    if (SpvuserView.SpvUserTypeId == 1 || SpvuserView.SpvUserTypeId == 3)
                    {
                        SendEmailOnNewUserCreation(userID, SpvuserView.Password, userName);
                    }

                    EmailInfo emailInfo = new EmailInfo();
                    emailInfo.TemplateID = 1;
                    emailInfo.FirstName = SpvuserView.FirstName;
                    emailInfo.LastName = SpvuserView.LastName;
                    emailInfo.LoginLink = GetLoginLinkForUser(SpvuserView);
                    emailInfo.UserName = SpvuserView.UserName;
                    emailInfo.Password = SpvuserView.Password;
                    _emailBAL.ComposeAndSendEmail(emailInfo, SpvuserView.Email);

                    //EmailTemplate eTemplate = _emailBAL.PrepareEmailTemplate(1, null, SpvuserView, null, null);
                    //_emailBAL.GeneralComposeAndSendMail(eTemplate, SpvuserView.Email);

                    #region Email configuration not required

                    string strEmailConfigureMsg = string.Empty;
                    //// strEmailConfigureMsg = !ProjectSession.IsUserEmailAccountConfigured ? " (Can not send mail to user because email account is not configured)" : string.Empty;
                    #endregion

                    this.ShowMessage(SystemEnums.MessageType.Success, "User has been saved successfully. " + strEmailConfigureMsg, true);
                    //this.ShowMessage(SystemEnums.MessageType.Success, "User has been saved successfully.", true);
                    //return RedirectToAction("Index", "User");
                    return Json("true", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string errorValue = string.Empty;
                    foreach (var item in result.Errors)
                    {
                        if (!string.IsNullOrEmpty(errorValue))
                        {
                            errorValue = errorValue + "<br/>" + item;
                        }
                        else
                        {
                            errorValue = item;
                        }
                    }
                    //return this.Json(String.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage + " " + v.Exception)));
                    return this.Json(errorValue);
                }
            }
            else
            {
                //var designRole = from SystemEnums.PostalAddressType s in Enum.GetValues(typeof(SystemEnums.PostalAddressType))
                //                 select new { ID = s.GetHashCode(), Name = s.ToString().Replace('_', ' ') };
                ViewBag.PostalAddressType = new SelectList(EnumExtensions.GetPostalAddressEnumList(), "ID", "Name");

                SpvuserView.EmailSignup = new Entity.Email.EmailSignup();
                //this.ShowMessage(SystemEnums.MessageType.Error, String.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage + " " + v.Exception)), true);
                //this.ShowMessage(SystemEnums.MessageType.Error, "User has not been saved successfully.", true);
                //return RedirectToAction("Create", "User");
                //return this.View("Create", SpvuserView);
                return this.Json(String.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage + " " + v.Exception)));
            }
        }

        [HttpGet]
        [SpvUserAuthorization]
        public JsonResult GetSpvUserType()
        {
            //List<SelectListItem> items = _spvUserBAL.GetData().Select(a => new SelectListItem { Text = a.UserTypeName, Value = a.SpvUserTypeId.ToString() }).ToList();
            //return Json(items, JsonRequestBehavior.AllowGet);
            if (ProjectSession.SystemUserType != null)
            {
                List<SelectListItem> items = JsonConvert.DeserializeObject<List<SelectListItem>>(ProjectSession.SystemUserType);
                return Json(items, JsonRequestBehavior.AllowGet);
            }
            else
            {
                List<SelectListItem> items = _spvUserBAL.GetData().Select(a => new SelectListItem { Text = a.UserTypeName, Value = a.SpvUserTypeId.ToString() }).ToList();
                ProjectSession.SystemUserType = JsonConvert.SerializeObject(items);
                return Json(items, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetUserTypeforManufacturer()
        {
            List<SelectListItem> items = _spvUserBAL.GetData().Select(a => new SelectListItem { Text = a.UserTypeName, Value = a.SpvUserTypeId.ToString() }).ToList();
            List<SelectListItem> itemsToRemove = items.Where(t => t.Value == "1" || t.Value == "2" || t.Value == "3").ToList();
            items = items.Except(itemsToRemove).ToList();
            return Json(items, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [SpvUserAuthorization]
        public ActionResult GetDataForStreetTypeUnitTypeDropdown(List<CommonData> cData)
        {
            DataSet Data = _spvUserBAL.GetDataForStreetTypeUnitTypeDropdown(cData);
            return Json(new { success = true, data = Newtonsoft.Json.JsonConvert.SerializeObject(Data) }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [SpvUserAuthorization]
        public JsonResult ProcessRequest(bool excludePostBoxFlag, string q)
        {
            string ret = string.Empty;
            var webRequest = System.Net.WebRequest.Create(string.Format("https://auspost.com.au/api/postcode/search.json?q=" + q + "&excludePostBoxFlag=true"));

            if (webRequest != null)
            {
                webRequest.Headers.Add("AUTH-KEY", "0344e02f-843b-49a7-8fd6-d35acd471480");
                webRequest.Method = "GET";
                webRequest.Timeout = 20000;
                webRequest.ContentType = "application/json";
            }

            HttpWebResponse resp = (HttpWebResponse)webRequest.GetResponse();
            Stream resStream = resp.GetResponseStream();
            StreamReader reader = new StreamReader(resStream);
            ret = reader.ReadToEnd();
            ret = ret.Replace(@"\", "");
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public void SendEmailOnNewUserCreation(int SpvUserID, string password, string userName)
        {
            EmailInfo emailInfo = new EmailInfo();
            var usr = _spvUserBAL.GetSpvUserById(SpvUserID);
            FormBot.Entity.SpvUser uv = DBClient.DataTableToList<FormBot.Entity.SpvUser>(usr.Tables[0]).FirstOrDefault();
            emailInfo.TemplateID = 1;
            emailInfo.FirstName = uv.FirstName;
            emailInfo.LastName = uv.LastName;
            emailInfo.UserName = userName;
            emailInfo.Password = password;
            emailInfo.LoginLink = ConfigurationManager.AppSettings["LoginLink"].ToString();


            this.ComposeAndSendEmail(emailInfo, uv.Email);
        }
        public string GetLoginLinkForUser(FormBot.Entity.SpvUser user)
        {
            return ProjectSession.LoginLink;
        }
        public void ComposeAndSendEmail(EmailInfo emailInfo, string mailTo)
        {
            try
            {
                EmailTemplate emailTempalte = _emailBAL.GetEmailTemplateByID(emailInfo.TemplateID);
                string body = _emailBAL.GetEmailBody(emailInfo, emailTempalte);
                //ComposeEmail composeEmail = new Email.ComposeEmail();
                //composeEmail.Attachment = "";
                //composeEmail.Subject = emailTempalte.Subject;
                //composeEmail.To = mailTo;
                //composeEmail.Body = new innerBody();
                //composeEmail.Body.body = body;
                //CheckMail checkMail = new CheckMail();
                //checkMail.SendMail(composeEmail, "send");


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
                        objQueuedEmail.ToEmail = mailTo;
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
            }
            catch (Exception ex)
            {
                var abc = ex;
            }
        }
        public void RequiredValidationField(FormBot.Entity.SpvUser userView)
        {
            int SpvUserTypeId = userView.SpvUserTypeId;
            ModelState.Remove("UnitTypeID");
            ModelState.Remove("UnitNumber");
            ModelState.Remove("StreetNumber");
            ModelState.Remove("StreetName");
            ModelState.Remove("StreetTypeID");
            ModelState.Remove("PostalAddressID");
            ModelState.Remove("PostalDeliveryNumber");
            ModelState.Remove("Town");
            ModelState.Remove("State");
            ModelState.Remove("PostCode");
            ModelState.Remove("ManufacturerName");
            if (SpvUserTypeId == 3)
            {
                // ModelState.Remove("ManufacturerName");
            }

        }

        [HttpPost]
        [SpvUserAuthorization]
        public JsonResult GetAllSpvUserKendo(int page = 1, int pageSize = 10, string sortCol = "", string sortDir = "", string Name = "", string UserName = "", string Email = "", string SpvUserTypeId = "", bool? IsActive = null, string SpvRoleId = "")
        {
            int spvusertypeid = SpvUserTypeId == "" ? ProjectSession.UserTypeId == 3 ? 4 : 0 : Convert.ToInt32(SpvUserTypeId);
            int SpvRoleIdint = SpvRoleId == "" ? 0 : Convert.ToInt32(SpvRoleId);

            List<SpvUser> spvuserlst = _spvUserBAL.GetAllSpvUserKendo(page, pageSize, sortCol, sortDir, Name, UserName, Email, spvusertypeid, IsActive, SpvRoleIdint);
            if (spvuserlst.Count > 0)
            {
                return Json(new { total = spvuserlst[0].TotalRecordCount, data = spvuserlst }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { total = 0, data = spvuserlst }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        [SpvUserAuthorization]
        public JsonResult GetSpvRole(int UserTypeId = 0)
        {
            if (ProjectSession.UserTypeId == 3)
            {
                UserTypeId = 3;
            }
            List<SelectListItem> Items = _spvUserBAL.GetSpvRole(UserTypeId).Select(a => new SelectListItem { Text = a.Name, Value = a.SpvRoleId.ToString(), Selected = a.IsSystemRole == true }).ToList();
            return Json(Items, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public JsonResult CheckUserExist(string UserValue, string FieldName, int? SpvUserId = null, int? UserTypeId = null)
        {
            bool isUserExists = true;
            if (FieldName == "UserName")
            {
                isUserExists = _spvUserBAL.CheckEmailExists(UserValue, SpvUserId);
            }
            return Json(new { status = isUserExists }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult DeleteUser(string SpvuserId)
        {
            var dsUsers = _spvUserBAL.GetSpvUserById(Convert.ToInt32(SpvuserId));
            FormBot.Entity.User userToDelete = DBClient.DataTableToList<FormBot.Entity.User>(dsUsers.Tables[0]).FirstOrDefault();

            _spvUserBAL.DeleteUser(Convert.ToInt32(SpvuserId));
            EmailInfo emailInfo = new EmailInfo();
            emailInfo.TemplateID = 13;
            emailInfo.FirstName = userToDelete.FirstName;
            emailInfo.LastName = userToDelete.LastName;
            _emailBAL.ComposeAndSendEmail(emailInfo, userToDelete.Email);
            ////EmailTemplate eTemplate = _emailBAL.PrepareEmailTemplate(13, null, userToDelete, null, null);
            ////_emailBAL.GeneralComposeAndSendMail(eTemplate, userToDelete.Email);
            return this.Json(new { success = true });
            //}
            //catch (Exception ex)
            //{
            //    return this.Json(new { success = false, errormessage = ex.Message });
            //}
        }


        [HttpGet]
        [UserAuthorization]
        public ActionResult Profile()
        {
            ViewBag.PostalAddressType = new SelectList(EnumExtensions.GetPostalAddressEnumList(), "ID", "Name");

            //if (ProjectSession.UserTypeId == 3)
            //{
            //    spvUser.SpvUserTypeId = 4;
            //}
            int userId = FormBot.Helper.ProjectSession.LoggedInUserId;
            SpvUser spvUser = new SpvUser();
            DataSet ds = _spvUserBAL.GetSpvUserById(Convert.ToInt32(userId));
            DataTable dt = ds.Tables[0];
           
            spvUser.SpvUserId = Convert.ToInt32(dt.Rows[0]["SpvUserId"]);
            spvUser.AspNetUserId = dt.Rows[0]["AspNetUserId"].ToString();
            spvUser.SpvUserTypeId = Convert.ToInt32(dt.Rows[0]["UserTypeID"]);
            spvUser.SpvRoleId = Convert.ToInt32(dt.Rows[0]["SpvRoleId"]);
            spvUser.FirstName = dt.Rows[0]["FirstName"].ToString();
            spvUser.LastName = dt.Rows[0]["LastName"].ToString();
            spvUser.UserName = dt.Rows[0]["UserName"].ToString();
            spvUser.Password = dt.Rows[0]["PasswordHash"].ToString();
            spvUser.Email = dt.Rows[0]["Email"].ToString();
            spvUser.IsActive = Convert.ToBoolean(dt.Rows[0]["IsActive"]);
            spvUser.Phone = dt.Rows[0]["Phone"].ToString();
            spvUser.Mobile = dt.Rows[0]["Mobile"].ToString();
            spvUser.UnitTypeID = Convert.ToInt32(dt.Rows[0]["UnitTypeID"]);
            spvUser.UnitNumber = dt.Rows[0]["UnitNumber"].ToString();
            spvUser.StreetNumber = dt.Rows[0]["StreetNumber"].ToString();
            spvUser.StreetName = dt.Rows[0]["StreetName"].ToString();
            spvUser.StreetTypeID = Convert.ToInt32(dt.Rows[0]["StreetTypeID"]);
            spvUser.Town = dt.Rows[0]["Town"].ToString();
            spvUser.State = dt.Rows[0]["State"].ToString();
            spvUser.PostCode = dt.Rows[0]["PostCode"].ToString();
            spvUser.PostalDeliveryNumber = dt.Rows[0]["PostalDeliveryNumber"].ToString();
            spvUser.PostalAddressID = Convert.ToInt32(dt.Rows[0]["PostalAddressID"]);
            spvUser.ManufacturerName = dt.Rows[0]["ManufacturerName"].ToString();
            if (Convert.ToBoolean(dt.Rows[0]["IsPostalAddress"]) == true)
            {
                spvUser.AddressId = 2;
            }
            else
            {
                spvUser.AddressId = 1;
            }
            return View(spvUser);
        }

        [HttpPost]
        [UserAuthorization]
        public async Task<JsonResult> Profile(SpvUser SpvuserView)
        {
            try
            {
                string guid = SpvuserView.Guid;
                RequiredValidationField(SpvuserView);
                IdentityResult result;
                if (SpvuserView.SpvUserId != 0)
                {
                    ModelState.Remove("Password");
                }

                if (ModelState.IsValid)
                {
                    string Id = null;
                    ViewBag.PostalAddressType = new SelectList(EnumExtensions.GetPostalAddressEnumList(), "ID", "Name");

                    UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                    UserManager.UserValidator = new UserValidator<ApplicationUser>(UserManager) { AllowOnlyAlphanumericUserNames = false };

                    UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                    var userDetails = UserManager.FindById(SpvuserView.AspNetUserId);

                    if (userDetails != null)
                    {
                        userDetails.Email = SpvuserView.Email;
                        userDetails.UserName = SpvuserView.UserName.Replace(" ", "");
                        // userDetails.CreatedPwdDate = userView.CreatedpwdDate;

                        PasswordHasher hasher = new PasswordHasher();
                        if (SpvuserView.Password != null)
                        {
                            userDetails.PasswordHash = hasher.HashPassword(SpvuserView.Password);
                        }
                    }

                    result = await UserManager.UpdateAsync(userDetails);
                    Id = userDetails.Id;


                    string userName = SpvuserView.UserName;

                    if (result.Succeeded)
                    {
                        SpvuserView.AspNetUserId = Id;
                        SpvuserView.ModifiedDate = DateTime.Today;
                        SpvuserView.Logo = "Images/logo.png";
                        SpvuserView.Theme = 1;

                        if (SpvuserView.AddressId == 2)
                        {
                            SpvuserView.IsPostalAddress = true;
                        }
                        else
                        {
                            SpvuserView.IsPostalAddress = false;
                        }
                        int userID = _spvUserBAL.CreateSpvUser(SpvuserView);
                        int uID = userID + 1;
                        if (SpvuserView.SpvUserTypeId == 1 || SpvuserView.SpvUserTypeId == 3)
                        {
                            SendEmailOnNewUserCreation(userID, SpvuserView.Password, userName);
                        }

                        EmailInfo emailInfo = new EmailInfo();
                        emailInfo.TemplateID = 1;
                        emailInfo.FirstName = SpvuserView.FirstName;
                        emailInfo.LastName = SpvuserView.LastName;
                        emailInfo.LoginLink = GetLoginLinkForUser(SpvuserView);
                        emailInfo.UserName = SpvuserView.UserName;
                        emailInfo.Password = SpvuserView.Password;
                        _emailBAL.ComposeAndSendEmail(emailInfo, SpvuserView.Email);

                        #region Email configuration not required

                        string strEmailConfigureMsg = string.Empty;
                        #endregion

                        return Json("true", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        string errorValue = string.Empty;
                        foreach (var item in result.Errors)
                        {
                            if (!string.IsNullOrEmpty(errorValue))
                            {
                                errorValue = errorValue + "<br/>" + item;
                            }
                            else
                            {
                                errorValue = item;
                            }
                        }
                        return this.Json(errorValue);
                    }
                }
                else
                {
                    ViewBag.PostalAddressType = new SelectList(EnumExtensions.GetPostalAddressEnumList(), "ID", "Name");

                    SpvuserView.EmailSignup = new Entity.Email.EmailSignup();
                    return this.Json(String.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage + " " + v.Exception)));
                }
            }
            catch (Exception ex)
            {
                _log.LogException(SystemEnums.Severity.Error, "", ex);
                return Json("false", JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// Get all spv users by userTypeId.
        /// </summary>
        /// <param name="userTypeId">userTypeId</param>
        /// <param name="isTypeChange">isTypeChange</param>
        /// <returns>JsonResult</returns>
        [HttpGet]
        [SpvUserAuthorization]
        public JsonResult GetSpvUserByUserTypeId(string userTypeId, string isTypeChange)
        {
            if (!string.IsNullOrEmpty(isTypeChange) && Convert.ToInt32(isTypeChange) == 1)
                ProjectSession.SystemUsersOfUserType = null;

            if (ProjectSession.SystemUsersOfUserType != null)
                return Json(ProjectSession.SystemUsersOfUserType, JsonRequestBehavior.AllowGet);
            else
            {
                List<SelectListItem> items = GetAllSpvUsers(userTypeId);
                ProjectSession.SystemUsersOfUserType = JsonConvert.SerializeObject(items);
                return Json(items, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Get all user
        /// </summary>
        /// <param name="userTypeId">userTypeId</param>
        /// <returns>List</returns>
        public List<SelectListItem> GetAllSpvUsers(string userTypeId)
        {
            return _spvUserBAL.GetSpvUserByUserType(string.IsNullOrEmpty(userTypeId) ? 0 : Convert.ToInt32(userTypeId)).Select(a => new SelectListItem { Text = a.Fullname, Value = a.SpvUserId.ToString() }).ToList();
        }
    }
}
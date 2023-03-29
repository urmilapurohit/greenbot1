using FormBot.BAL.Service;
using FormBot.Entity;
using System.Collections.Generic;
using FormBot.Helper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Xml;
using System.Xml.Linq;
using System.Web.Mvc;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using FormBot.Main.Models;
using System.Linq;
using FormBot.BAL;
using System.Web;
using FormBot.Email;
using System.Data;
using FormBot.Entity.Email;
using System.Configuration;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Collections;
using FormBot.Entity.SolarElectrician;
using FormBot.Main.Helpers;
using Xero.Api.Example.Applications.Public;
using Xero.Api.Infrastructure.Exceptions;
using FormBot.BAL.Service.CommonRules;
using Excel;
using FormBot.Entity.VEEC;
using System.Text.RegularExpressions;
using FormBot.Entity.Job;
using FormBot.Entity.ActivityLog;
using FormBot.BAL.Service.ActivityLog;
using ClosedXML.Excel;
using Microsoft.Extensions.DependencyInjection;
using Xero.NetStandard.OAuth2.Config;
using Xero.NetStandard.OAuth2.Client;
using System.Net.Http;
using Xero.NetStandard.OAuth2.Token;
using Xero.NetStandard.OAuth2.Api;
using Xero.NetStandard.OAuth2.Model;
using Org.BouncyCastle.Asn1.Ocsp;
using Excel.Log;
using Xero.Api.Core.Model.Types;
using Xero.Api.Core;
using FormBot.Helper.Helper;
using HtmlAgilityPack;

namespace FormBot.Main.Controllers
{
    public class UserController : BaseController
    {
        #region Properties
        private readonly IUserBAL _userBAL;
        private readonly IRoleBAL _role;
        private readonly IEmailBAL _emailBAL;
        private readonly IResellerBAL _resellerBAL;
        private readonly ISolarCompanyBAL _solarCompanyBAL;
        private readonly IUnitTypeBAL _unitTypeBAL;
        private readonly IStreetTypeBAL _streetTypeBAL;
        private readonly ILoginBAL _login;
        private readonly ICERImportBAL _cerImportBAL;
        private readonly IGenerateStcReportBAL _generateStcReportBAL;
        private readonly ICreateJobHistoryBAL _createJobHistoryBAL;
        private readonly Logger _log;
        private readonly IActivityLogBAL _activityLog;
        private readonly ICreateJobBAL _createJobBAL;
        public UserManager<ApplicationUser> UserManager { get; private set; }
        ApplicationSettingsTest xeroApiHelper;
        #endregion

        #region Constructor
        public UserController(IUserBAL userBAL, IRoleBAL role, IEmailBAL emailBAL, IResellerBAL resellerBAL, ISolarCompanyBAL solarCompanyBAL, IUnitTypeBAL unitTypeBAL, IStreetTypeBAL streetTypeBAL, ILoginBAL login, ICERImportBAL cerImportBAL, IGenerateStcReportBAL generateStcReportBAL, ICreateJobHistoryBAL createJobHistoryBAL, IActivityLogBAL activityLog, ICreateJobBAL createJobBAL)
        {
            this._userBAL = userBAL;
            this._role = role;
            this._emailBAL = emailBAL;
            this._resellerBAL = resellerBAL;
            this._solarCompanyBAL = solarCompanyBAL;
            this._unitTypeBAL = unitTypeBAL;
            this._streetTypeBAL = streetTypeBAL;
            this._login = login;
            this._cerImportBAL = cerImportBAL;
            this._generateStcReportBAL = generateStcReportBAL;
            this._createJobHistoryBAL = createJobHistoryBAL;
            this._log = new Logger();
            this._activityLog = activityLog;
            this._createJobBAL = createJobBAL;
            var UsageTypeList = new List<SelectListItem>
            {
                new SelectListItem{ Text="Reseller", Value = "1" },
                new SelectListItem{ Text="Wholesaler", Value = "2" },
                new SelectListItem{Text="SAAS", Value = "3"}
            };
            ViewBag.UsageType = UsageTypeList;
        }
        #endregion      

        #region Event
        /// <summary>
        /// Create New User
        /// </summary>
        /// <returns>user view</returns>
        [HttpGet]
        [UserAuthorization]
        public ActionResult Create(string Id = null)
        {
            ModelState.Clear();

            FormBot.Entity.User user = new FormBot.Entity.User();
            user.lstFCOGroup1 = _userBAL.GetMultipleFCOGroupDropDown();
            //var designRole = from SystemEnums.PostalAddressType s in Enum.GetValues(typeof(SystemEnums.PostalAddressType))
            //                 select new { ID = s.GetHashCode(), Name = s.ToString().Replace('_', ' ') };
            ViewBag.PostalAddressType = new SelectList(EnumExtensions.GetPostalAddressEnumList(), "ID", "Name");

            //user.solarElectricianView = new SolarElectricianView();
            //user.solarElectricianView.IsSendRequest = true;
            user.installerDesignerView = new InstallerDesignerView();
            user.UserTypeID = ProjectSession.UserTypeId;
            if (ProjectSession.UserTypeId == 1 || ProjectSession.UserTypeId == 2 || ProjectSession.UserTypeId == 3 || ProjectSession.UserTypeId == 4 || ProjectSession.UserTypeId == 5 || ProjectSession.UserTypeId == 6 || ProjectSession.UserTypeId == 8)
            {
                var statuses = from SystemEnums.SEDesignRole s in Enum.GetValues(typeof(SystemEnums.SEDesignRole))
                               select new { ID = s, Name = s.ToString().Replace('_', ' ') };
                ViewData["SEDesignRole"] = new SelectList(statuses, "ID", "Name");
            }
            //var UsageTypeList = new List<SelectListItem>
            //{
            //    new SelectListItem{ Text="Reseller", Value = "1" },
            //    new SelectListItem{ Text="Wholesaler", Value = "2" },
            //    new SelectListItem{Text="SAAS", Value = "3"}
            //};
            //var Invoicerlist = new List<SelectListItem>
            //{
            //    new SelectListItem{ Text="Select", Value = "0" },
            //    new SelectListItem{ Text="EMERGING ENERGY SOLUTIONS GROUP PTY. LTD.", Value = "2965" },
            //};
            ViewBag.UsageType = Common.GetUsageTypeList();
            ViewBag.Invoicer = Common.GetInvoicer();
            //if (string.IsNullOrEmpty(user.UniqueContactId))
            //{
            //    user.UniqueContactId = GetUniqueContactIDforSAAS();
            //}
var NotesType = from SystemEnums.NotesType n in Enum.GetValues(typeof(SystemEnums.NotesType))
                            select new { Text = (int)n, Value = n.ToString() };
            NotesType = NotesType.Where(x => x.Text == 3).ToList();
            List<SelectListItem> NotestypeDropdown = new SelectList(NotesType, "Text", "Value").ToList();
            NotestypeDropdown.Add(new SelectListItem { Text = "Warning", Value = "5" });
            ViewBag.NotesType = NotestypeDropdown;
            user.EmailSignup = new Entity.Email.EmailSignup();
            user.RecEmailSignup = new Entity.Email.RecEmailSignup();
            user.Guid = Guid.NewGuid().ToString();
            if (FormBot.Helper.ProjectSession.UserTypeId == 2)
            {
                user.UserTypeID = 5;
                user.ResellerID = ProjectSession.ResellerId;
            }
            else if (FormBot.Helper.ProjectSession.UserTypeId == 4 || FormBot.Helper.ProjectSession.UserTypeId == 6)
                user.UserTypeID = 8;
            else if (FormBot.Helper.ProjectSession.UserTypeId == 1)
                user.UserTypeID = 3;


            int AccreditedInstallerId = 0;
            if (!string.IsNullOrWhiteSpace(Id))
                int.TryParse(QueryString.GetValueFromQueryString(Id, "id"), out AccreditedInstallerId);
            if (AccreditedInstallerId > 0)
            {
                user.UserTypeID = 7;
                AccreditedInstallers objAccreditedInstallers = _cerImportBAL.GetAccreditedInstallerDetailByAccreditedInstallerId(AccreditedInstallerId);
                if (objAccreditedInstallers != null)
                {
                    user.AccreditedInstallerId = AccreditedInstallerId;
                    user.Status = 2;
                    if (!string.IsNullOrWhiteSpace(objAccreditedInstallers.AccreditationNumber))
                        user.CECAccreditationNumber = objAccreditedInstallers.AccreditationNumber;
                    if (!string.IsNullOrWhiteSpace(objAccreditedInstallers.FirstName))
                        user.FirstName = objAccreditedInstallers.FirstName;
                    if (!string.IsNullOrWhiteSpace(objAccreditedInstallers.LastName))
                        user.LastName = objAccreditedInstallers.LastName;
                    if (!string.IsNullOrWhiteSpace(objAccreditedInstallers.AccountName))
                        user.AccountName = objAccreditedInstallers.AccountName;
                    if (objAccreditedInstallers.UnitTypeID != null)
                        user.UnitTypeID = Convert.ToInt32(objAccreditedInstallers.UnitTypeID);
                    if (!string.IsNullOrWhiteSpace(objAccreditedInstallers.MailingAddressUnitNumber))
                        user.UnitNumber = objAccreditedInstallers.MailingAddressUnitNumber;
                    if (!string.IsNullOrWhiteSpace(objAccreditedInstallers.MailingAddressStreetNumber))
                        user.StreetNumber = objAccreditedInstallers.MailingAddressStreetNumber;
                    if (!string.IsNullOrWhiteSpace(objAccreditedInstallers.MailingAddressStreetName))
                        user.StreetName = objAccreditedInstallers.MailingAddressStreetName;
                    if (objAccreditedInstallers.StreetTypeID != null)
                        user.StreetTypeID = objAccreditedInstallers.StreetTypeID;
                    if (!string.IsNullOrWhiteSpace(objAccreditedInstallers.MailingCity))
                        user.Town = objAccreditedInstallers.MailingCity;
                    if (!string.IsNullOrWhiteSpace(objAccreditedInstallers.MailingState))
                        user.State = objAccreditedInstallers.MailingState;
                    if (!string.IsNullOrWhiteSpace(objAccreditedInstallers.PostalCode))
                        user.PostCode = objAccreditedInstallers.PostalCode;
                    if (!string.IsNullOrWhiteSpace(objAccreditedInstallers.Phone))
                        user.Phone = objAccreditedInstallers.Phone;
                    if (!string.IsNullOrWhiteSpace(objAccreditedInstallers.Mobile))
                        user.Mobile = objAccreditedInstallers.Mobile;
                    if (!string.IsNullOrWhiteSpace(objAccreditedInstallers.Email))
                        user.Email = objAccreditedInstallers.Email;
                    if (!string.IsNullOrWhiteSpace(objAccreditedInstallers.GridType))
                    {
                        if (objAccreditedInstallers.GridType.ToLower() == "install only")
                            user.SEDesignRoleId = 1;
                        else if (objAccreditedInstallers.GridType.ToLower() == "design only")
                            user.SEDesignRoleId = 2;
                        else if (objAccreditedInstallers.GridType.ToLower() == "design & install" || objAccreditedInstallers.GridType.ToLower() == "design & supervise")
                            user.SEDesignRoleId = 3;
                    }
                    if (!string.IsNullOrWhiteSpace(objAccreditedInstallers.LicensedElectricianNumber))
                        user.ElectricalContractorsLicenseNumber = objAccreditedInstallers.LicensedElectricianNumber;

                    user.UserName = (user.FirstName + "." + user.LastName).ToLower();
                    user.Password = user.CECAccreditationNumber;
                }
            }

            return View(user);
        }

        /// <summary>
        /// Post method for create user.
        /// </summary>
        /// <param name="userView">The user view.</param>
        /// <returns>
        /// return user listing page.
        /// </returns>
        [HttpPost]
        public async Task<JsonResult> Create(FormBot.Entity.User userView)
        {
            ModelState.Remove("Note");
            string guid = userView.Guid;
            string sourceDirectory = ProjectSession.ProofDocumentsURL + "\\" + "UserDocuments" + "\\" + guid;

            if (userView.UserTypeID == 0)
            {
                userView.UserTypeID = ProjectSession.UserTypeId;
                if (userView.UserTypeID == 2)
                {
                    userView.UserTypeID = 5;
                }
                else if (userView.UserTypeID == 4)
                {
                    userView.SolarCompanyId = ProjectSession.SolarCompanyId;
                    userView.ResellerID = ProjectSession.ResellerId;
                    userView.UserTypeID = 8;
                    ModelState.Remove("SolarCompanyId");
                }
                else if (userView.UserTypeID == 6)
                {
                    userView.SolarCompanyId = ProjectSession.SolarCompanyId;
                    userView.UserTypeID = 8;
                    ModelState.Remove("SolarCompanyId");
                }
            }
            ModelState.Remove("CustomCompanyName");
            if (userView.UserTypeID == 5 && userView.ResellerID == null)
            {
                userView.ResellerID = ProjectSession.ResellerId;
            }

            if (userView.UserTypeID == 8 || userView.UserTypeID == 9)
            {
                ModelState.Remove("SolarCompanyId");
            }

            if (userView.IsSWHUser == true)
                userView.ElectricalContractorsLicenseNumber = userView.SWHLicenseNumber;

            RequiredValidationField(userView);
            RemoveInstallerDesignerValidation();

            //int UserExists = 0;
            if (userView.UserTypeID == 7)
            {
                string sErrorMessage = string.Empty;
                var UserExists = _userBAL.CheckAccreditationNumber(userView.CECAccreditationNumber, userView.ElectricalContractorsLicenseNumber, userView.Email, userView.IsPVDUser == null ? false : userView.IsPVDUser.Value, userView.IsSWHUser == null ? false : userView.IsSWHUser.Value);
                if (UserExists.UserId > 0)
                {
                    if (userView.IsPVDUser == true)
                    {
                        sErrorMessage = GetErrorMessageForSolarElectrician(1, UserExists);
                    }
                    if (userView.IsSWHUser == true)
                    {
                        if (!string.IsNullOrWhiteSpace(sErrorMessage))
                            sErrorMessage = GetErrorMessageForSolarElectrician(2, UserExists);
                        else
                            sErrorMessage = GetErrorMessageForSolarElectrician(3, UserExists);
                    }
                    return Json(sErrorMessage, JsonRequestBehavior.AllowGet);
                }
            }

            //if (userView.UserTypeID == 10)
            //{
            //	UserExists = 0;//_userBAL.CheckAccreditationNumber(userView.ElectricalContractorsLicenseNumber, userView.UserTypeID);
            //	if (!UserExists.Equals(0))
            //		return Json("SWH User with given license number already exists.", JsonRequestBehavior.AllowGet);
            //}
            if (!userView.IsWholeSaler)
                RemoveWholeSalerValidation();

            if(userView.UserTypeID != 2 || (userView.UserTypeID == 2 && userView.UsageType != 3))
            {
                RemoveSAASUserValidation();
            }

            if (ModelState.IsValid)
            {
                ViewBag.PostalAddressType = new SelectList(EnumExtensions.GetPostalAddressEnumList(), "ID", "Name");
                userView.lstFCOGroup1 = _userBAL.GetMultipleFCOGroupDropDown();
                userView.EmailSignup = new Entity.Email.EmailSignup();
                userView.CreatedpwdDate = DateTime.Now;
                userView.ModifiedpwdDate = DateTime.Now;

                UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                UserManager.UserValidator = new UserValidator<ApplicationUser>(UserManager) { AllowOnlyAlphanumericUserNames = false };
                var user = new ApplicationUser() { Email = userView.Email, UserName = userView.UserName.Replace(" ", ""), PasswordHash = userView.Password, CreatedPwdDate = userView.CreatedpwdDate, ModifiedPwdDate = userView.ModifiedpwdDate, IsResetPwd = false };
                IdentityResult result = await UserManager.CreateAsync(user, userView.Password);
                string userName = userView.UserName;
                if (userView.IsSTC == true && userView.UserTypeID == 4)
                {
                    UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                    UserManager.UserValidator = new UserValidator<ApplicationUser>(UserManager) { AllowOnlyAlphanumericUserNames = false };
                    object isUserExists;
                    string uName = userView.UserName + "_SE";
                    userName = userName + "_SE";
                    isUserExists = _userBAL.CheckEmailExists(userView.UserName);
                    if (!isUserExists.Equals(0))
                    {
                        int i = 0;
                        while (true)
                        {
                            userName = uName + i;
                            isUserExists = _userBAL.CheckEmailExists(userName);
                            if (isUserExists.Equals(0))
                            {
                                break;
                            }
                            else
                            {
                                i++;
                            }
                        }
                    }

                    var userSE = new ApplicationUser() { Email = userView.Email, UserName = userName.Replace(" ", ""), PasswordHash = userView.Password };
                    IdentityResult resultSE = await UserManager.CreateAsync(userSE, userView.Password);
                    userView.AspNetUserIdSE = userSE.Id;
                }

                if (result.Succeeded)
                {
                    userView.AspNetUserId = user.Id;
                    userView.CreatedDate = DateTime.Today;
                    userView.ModifiedDate = DateTime.Today;
                    userView.Logo = "Images/logo.png";
                    userView.Theme = 1;
                    userView.IsFirstLogin = true;
                    if (userView.strFromDate != null)
                    {
                        userView.FromDate = Convert.ToDateTime(userView.strFromDate);
                    }

                    if (userView.strToDate != null)
                    {
                        userView.ToDate = Convert.ToDateTime(userView.strToDate);
                    }

                    if (userView.AddressID == 2)
                    {
                        userView.IsPostalAddress = true;
                    }
                    else
                    {
                        userView.IsPostalAddress = false;
                    }

                    if (userView.WholesalerIsPostalAddress == 2)
                    {
                        userView.WholesalerIsPostalAddress = 1;
                    }
                    else
                    {
                        userView.WholesalerIsPostalAddress = 0;
                    }

                    if(userView.InvoicerAddressID == 2)
                    {
                        userView.InvoicerIsPostalAddress = true;
                    }
                    else
                    {
                        userView.InvoicerIsPostalAddress = false;
                    }
                    if (userView.UserTypeID == 2 && userView.ContractPathFile != null)
                    {
                        if (userView.ContractPathFile.Count > 0)
                        {
                            var lastuploadedfile = userView.ContractPathFile[userView.ContractPathFile.Count - 1];
                            //_userBAL.InsertUserDocument(userView.UserId, lastuploadedfile.FileName, lastuploadedfile.ProofDocumentType);
                            userView.ContractPath = lastuploadedfile.FileName;
                        }
                    }

                    if (userView.CompanyABN != null)
                    {
                        userView.CompanyABN = userView.CompanyABN.Replace(" ", "");
                    }

                    if (userView.ResellerID == 0)
                        userView.ResellerID = null;

                    userView.ComplainBy = ProjectSession.LoggedInUserId;

                    int userID = _userBAL.Create(userView);
                    // Logic to set Entity name
                    if (userView.UserTypeID == 4)
                    {
                        string entityName = strGetEntityName(userView.CompanyABN);
                        _userBAL.UpdateEntityName(userID, entityName);
                    }

                    string destinationDirectory = ProjectSession.ProofDocumentsURL + "\\" + "UserDocuments" + "\\" + userID;

                    // SCA want join as SE userID
                    int uID = userID + 1;
                    string seDestinationDirectory = ProjectSession.ProofDocumentsURL + "\\" + "UserDocuments" + "\\" + uID;

                    if (userView.UserTypeID == 4 || userView.UserTypeID == 6 || userView.UserTypeID == 9)
                    {
                        if (userView.ProofFileNamesCreate != null)
                        {
                            foreach (var files in userView.ProofFileNamesCreate)
                            {
                                if (files.ProofDocumentType > 0)
                                {
                                    userView.FileName = files.FileName;
                                    _userBAL.InsertUserDocument(userID, files.FileName, files.ProofDocumentType);
                                }

                                // Insert SCA document when join as a SE
                                if (userView.IsSTC == true && userView.UserTypeID == 4)
                                {
                                    _userBAL.InsertUserDocument(uID, files.FileName, files.ProofDocumentType);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (userView.FileNamesCreate != null)
                        {
                            foreach (var files in userView.FileNamesCreate)
                            {
                                var lstfile = JsonConvert.DeserializeObject<List<Rootobject>>(files);
                                foreach (var file in lstfile)
                                {
                                    userView.FileName = file.Name;
                                    _userBAL.InsertUserDocument(userID, userView.FileName);
                                }
                                // Insert SCA document when join as a SE
                                if (userView.IsSTC == true && userView.UserTypeID == 4)
                                {
                                    _userBAL.InsertUserDocument(uID, files);
                                }
                            }
                        }
                    }

                    if (userView.FileNamesCreate != null || userView.ProofFileNamesCreate != null || userView.Signature != null || userView.ContractPathFile != null)
                    {
                        try
                        {
                            Directory.Move(sourceDirectory, destinationDirectory);

                            if (userView.IsSTC == true && userView.UserTypeID == 4)
                            {
                                // SCA want join as SE
                                Directory.CreateDirectory(seDestinationDirectory);
                                DirectoryInfo dir = new DirectoryInfo(destinationDirectory);
                                FileInfo[] files = dir.GetFiles();
                                foreach (FileInfo file in files)
                                {
                                    // Create the path to the new copy of the file.
                                    string temppath = Path.Combine(seDestinationDirectory, file.Name);

                                    // Copy the file.
                                    file.CopyTo(temppath, false);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }

                    }

                    

                    if (userView.FCOGroupSelected != null)
                    {
                        string strFCOGroupSelected = string.Empty;
                        foreach (var fCOGroup in userView.FCOGroupSelected)
                        { 
                            strFCOGroupSelected = strFCOGroupSelected + fCOGroup + ",";
                        }

                        

                        if (userView.UserTypeID == 2)
                        {
                            _userBAL.InsertSelectedResellerFCOGroup(userID, strFCOGroupSelected);
                        }
                    }

                    if (userView.IsSTC == true && userView.UserTypeID == 4)
                    {
                        SendEmailOnNewUserCreation(uID, userView.Password, userName);
                    }

                    userView.ResellerID = _userBAL.GetResellerIdByUserId(userID);

                    if (userView.UserTypeID == 2)
                    {
                        InsertUpdateResellerRECLoginDetails(userView);
                    }

                    EmailInfo emailInfo = new EmailInfo();
                    emailInfo.TemplateID = 1;
                    emailInfo.FirstName = userView.FirstName;
                    emailInfo.LastName = userView.LastName;
                    emailInfo.LoginLink = GetLoginLinkForUser(userView);
                    emailInfo.UserName = userView.UserName;
                    emailInfo.Password = userView.Password;
                    _emailBAL.ComposeAndSendEmail(emailInfo, userView.Email);

                    //EmailTemplate eTemplate = _emailBAL.PrepareEmailTemplate(1, null, userView, null, null);
                    //_emailBAL.GeneralComposeAndSendMail(eTemplate, userView.Email);

                    #region Email configuration not required

                    string strEmailConfigureMsg = string.Empty;
                    strEmailConfigureMsg = !ProjectSession.IsUserEmailAccountConfigured ? " (Can not send mail to user because email account is not configured)" : string.Empty;
                    #endregion

                    //if (!string.IsNullOrEmpty(userView.Note))
                    //{
                    //    SaveUserNotetoXML(userID, userView.Note, userView.NotesType, userView.UserTypeID, userView.IsImportantNote, null);
                    //}

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
                        if (!string.IsNullOrWhiteSpace(errorValue))
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
                userView.lstFCOGroup1 = _userBAL.GetMultipleFCOGroupDropDown();
                userView.EmailSignup = new Entity.Email.EmailSignup();
                //this.ShowMessage(SystemEnums.MessageType.Error, String.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage + " " + v.Exception)), true);
                //this.ShowMessage(SystemEnums.MessageType.Error, "User has not been saved successfully.", true);
                //return RedirectToAction("Create", "User");
                //return this.View("Create", userView);
                return this.Json(String.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage + " " + v.Exception)));
            }
        }

        /// <summary>
        /// Post method for edit user
        /// </summary>
        /// <param name="userView">The user view.</param>
        /// <returns>Edit User</returns>
        [HttpPost]
        public async Task<ActionResult> Edit(FormBot.Entity.User userView)
        {
            RequiredValidationField(userView);
            RemoveInstallerDesignerValidation();
            string dbuserName = string.Empty;
            // string oldCompanyName = string.Empty;
            int WholeSalerIsPostalAddress = userView.WholesalerIsPostalAddress;
            if (!userView.IsWholeSaler)
                RemoveWholeSalerValidation();
            if (userView.UserTypeID != 2 || (userView.UserTypeID == 2 && userView.UsageType != 3))
            {
                RemoveSAASUserValidation();
            }
ModelState.Remove("Note");
            if (ModelState.IsValid)
            {
                //FormBot.Entity.User uv = Session["UserDetail"] as FormBot.Entity.User;
                FormBot.Entity.User uv = TempData[userView.UserId.ToString()] as FormBot.Entity.User;

                if (userView.SolarCompanyId == 0)
                {
                    userView.SolarCompanyId = null;
                }

                if (userView.ResellerID == 0)
                {
                    userView.ResellerID = null;
                }

                //var designRole = from SystemEnums.PostalAddressType s in Enum.GetValues(typeof(SystemEnums.PostalAddressType))
                //                 select new { ID = s.GetHashCode(), Name = s.ToString().Replace('_', ' ') };
                
                ViewBag.PostalAddressType = new SelectList(EnumExtensions.GetPostalAddressEnumList(), "ID", "Name");
                userView.lstFCOGroup1 = _userBAL.GetMultipleFCOGroupDropDown();
                userView.EmailSignup = new Entity.Email.EmailSignup();

                UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                UserManager.UserValidator = new UserValidator<ApplicationUser>(UserManager) { AllowOnlyAlphanumericUserNames = false };
                var userDetails = UserManager.FindById(userView.AspNetUserId);

                if (userDetails != null)
                {
                    userDetails.Email = userView.Email;
                    userDetails.UserName = userView.UserName.Replace(" ", "");
                    // userDetails.CreatedPwdDate = userView.CreatedpwdDate;
                    userDetails.ModifiedPwdDate = DateTime.Now;
                    userDetails.IsResetPwd = false;
                    PasswordHasher hasher = new PasswordHasher();
                    if (userView.Password != null)
                    {
                        userDetails.PasswordHash = hasher.HashPassword(userView.Password);
                    }
                }


                IdentityResult result = await UserManager.UpdateAsync(userDetails);

                //IdentityResult rs= await UserManager.UpdateAsync(userDetails);
                userView.ModifiedBy = ProjectSession.LoggedInUserId;
                userView.ModifiedDate = DateTime.Today;
                //userView.Logo = "Images/logo.png";
                if (userView.strFromDate != null)
                {
                    userView.FromDate = Convert.ToDateTime(userView.strFromDate);
                }

                if (userView.strToDate != null)
                {
                    userView.ToDate = Convert.ToDateTime(userView.strToDate);
                }

                if (userView.AddressID == 2)
                {
                    userView.IsPostalAddress = true;
                }
                else
                {
                    userView.IsPostalAddress = false;
                }

                if (userView.WholesalerIsPostalAddress == 2)
                {
                    userView.WholesalerIsPostalAddress = 1;
                }
                else
                {
                    userView.WholesalerIsPostalAddress = 0;
                }

                if(userView.InvoicerAddressID == 2)
                {
                    userView.InvoicerIsPostalAddress = true;
                }
                else
                {
                    userView.InvoicerIsPostalAddress = false;
                }

                if (userView.IsSTC == false && userView.UserTypeID == 4)
                {
                    userView.CECAccreditationNumber = null;
                    userView.ElectricalContractorsLicenseNumber = null;
                    userView.CECDesignerNumber = null;
                }

                if (userView.CompanyABN != null)
                {
                    userView.CompanyABN = userView.CompanyABN.Replace(" ", "");
                }

                userView.RecPassword = userView.RecPassword != null ? userView.RecPassword : uv.RecPassword;

                if (userView.UserTypeID == 5)
                {
                    DataSet ds = _createJobBAL.GetUserNameSolarCompanyForCache(userView.UserId, 0);
                    if (ds.Tables.Count > 0)
                    {
                        DataTable userData = ds.Tables[0];
                        if (userData.Rows.Count > 0)
                        {
                            dbuserName = userData.Rows[0]["UserFNameLname"].ToString();
                            //oldCompanyName = userData.Rows[0]["CompanyName"].ToString();
                        }
                    }
                }

                if (userView.UserTypeID == 2 && userView.ContractPathFile != null)
                {
                    if (userView.ContractPathFile.Count > 0)
                    {
                        var lastuploadedfile = userView.ContractPathFile[userView.ContractPathFile.Count - 1];
                        //_userBAL.InsertUserDocument(userView.UserId, lastuploadedfile.FileName, lastuploadedfile.ProofDocumentType);
                        userView.ContractPath = lastuploadedfile.FileName;
                    }
                }

                int userID = _userBAL.Create(userView);
                if (userView.UserTypeID == 4)
                {
                    string entityName = strGetEntityName(userView.CompanyABN);
                    _userBAL.UpdateEntityName(userID, entityName);
                }

                if (userView.ProofFileNamesCreate!=null &&( userView.UserTypeID == 6 || userView.UserTypeID == 9))
                {
                    if (userView.ProofFileNamesCreate != null)
                    {
                        foreach (var files in userView.ProofFileNamesCreate)
                        {
                            //userView.FileName = files;
                            if (files.ProofDocumentType > 0)
                            {
                                string UserHistoryMessage = "has uploaded a " + Enum.GetName(typeof(SystemEnums.ProofDocumentType), files.ProofDocumentType) + " file " + files.FileName;
                                Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                                _userBAL.InsertUserDocument(userView.UserId, files.FileName, files.ProofDocumentType);
                            }
                        }
                    }
                }
                else
                {
                    if (userView.FileNamesCreate != null)
                    {
                        foreach (var files in userView.FileNamesCreate)
                        {
                            userView.FileName = files;
                            string UserHistoryMessage = "has uploaded a file " + userView.FileName;
                            Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                            _userBAL.InsertUserDocument(userView.UserId, files);
                        }
                    }
                }

                

                if (userView.FCOGroupSelected != null)
                {
                    string strFCOGroupSelected = string.Empty;
                    string FCOGroupName = "";
                    List<String> lstFCOGroupName = new List<string>();
                    foreach (var fCOGroup in userView.FCOGroupSelected)
                    {
                        FCOGroupName = _userBAL.GetFCOGroupNameByGroupID(Convert.ToInt32(fCOGroup));
                        lstFCOGroupName.Add(FCOGroupName);
                        strFCOGroupSelected = strFCOGroupSelected + fCOGroup + ",";
                    }

                    if (userView.UserTypeID == 3)
                    {
                        string FCOGroupNamelist = string.Join(",", lstFCOGroupName);
                        string UserHistoryMessage = "added FCO Group(s) : " + FCOGroupNamelist;
                        Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                        _userBAL.DeleteSelectedFCOGroup(userView.UserId);
                        _userBAL.InsertSelectedFCOGroup(userView.UserId, strFCOGroupSelected);
                    }

                    if (userView.UserTypeID == 2)
                    {
                        string FCOGroupNamelist = string.Join(",", lstFCOGroupName);
                        string UserHistoryMessage = "added FCO Group(s) : " + FCOGroupNamelist;
                        Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                        _userBAL.DeleteSelectedResellerGroup(userView.UserId);
                        _userBAL.InsertSelectedResellerFCOGroup(userView.UserId, strFCOGroupSelected);
                    }
                }

                if (userView.UserTypeID == 2)
                {
                    InsertUpdateResellerRECLoginDetails(userView);
                }

                if (uv.UserName != userView.UserName || uv.Password != userView.Password)
                {
                    EmailInfo emailInfo = new EmailInfo();
                    emailInfo.TemplateID = 12;
                    emailInfo.FirstName = userView.FirstName;
                    emailInfo.LastName = userView.LastName;
                    StringBuilder sb = new StringBuilder();
                    sb.Append(uv.UserName != userView.UserName ? "UserName: " + userView.UserName : string.Empty);
                    sb.Append(uv.Password != userView.Password ? "Password: " + userView.Password : string.Empty);
                    emailInfo.Details = sb.ToString();
                    _emailBAL.ComposeAndSendEmail(emailInfo, userView.Email);
                }

                #region Email configuration not required

                string strEmailConfigureMsg = string.Empty;
                strEmailConfigureMsg = !ProjectSession.IsUserEmailAccountConfigured ? " (Can not send mail to user because email account is not configured)" : string.Empty;
                #endregion

                if (userView.UserTypeID == 5)
                {

                    DataTable dtSolarCompany = _createJobBAL.GetUserNameSolarCompanyForCache(userView.UserId, userView.UserTypeID).Tables[1];
                    if (dtSolarCompany.Rows.Count > 0)
                    {
                        SortedList<string, string> data = new SortedList<string, string>();
                        string updatedUserName = userView.FirstName + " " + userView.LastName;
                        string updatedcompanyName = userView.CompanyName;
                        if (userView.UserTypeID == 5)
                        {
                            if (dbuserName != updatedUserName)
                            {
                                data.Add("AccountManager", updatedUserName);
                                for (int i = 0; i < dtSolarCompany.Rows.Count; i++)
                                {
                                   await CommonBAL.SetCacheDataOnSCARARAMForSTCSubmission(Convert.ToInt32(dtSolarCompany.Rows[i]["SolarCompanyID"]), data);
                                }
                            }
                        }
                        //if (userView.UserTypeID == 4 && (oldCompanyName!=updatedcompanyName))
                        //{
                        //    data.Add("SolarCompany", updatedcompanyName);
                        //    CommonBAL.SetCacheDataOnSCARARAMForSTCSubmission(userView.SolarCompanyId, data);
                        //}
                    }



                }
                if(userView.PreviousFirstName != userView.FirstName)
                {
                    string PreviousFirstName = (string.IsNullOrEmpty(userView.PreviousFirstName)) ? "null" : userView.PreviousFirstName;
                    string NewFirstName = (string.IsNullOrEmpty(userView.FirstName)) ? "null" : userView.FirstName;
                    string UserHistoryMessage = "changed FirstName from " + PreviousFirstName + " to " + NewFirstName;
                    Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                }
                if (userView.PreviousLastName != userView.LastName)
                {
                    string PreviousLastName = (string.IsNullOrEmpty(userView.PreviousLastName)) ? "null" : userView.PreviousLastName;
                    string NewLastName = (string.IsNullOrEmpty(userView.LastName)) ? "null" : userView.LastName;
                    string UserHistoryMessage = "changed LastName from " + PreviousLastName + " to " + NewLastName;
                    Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                }
                if (userView.PreviousEmail != userView.Email)
                {
                    string PreviousEmail = (string.IsNullOrEmpty(userView.PreviousEmail)) ? "null" : userView.PreviousEmail;
                    string NewEmail = (string.IsNullOrEmpty(userView.Email)) ? "null" : userView.Email;
                    string UserHistoryMessage = "changed Email from " + PreviousEmail + " to " + NewEmail;
                    Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                }
                if(userView.PreviousPhone != userView.Phone)
                {
                    string PreviousPhone = (string.IsNullOrEmpty(userView.PreviousPhone)) ? "null" : userView.PreviousPhone;
                    string NewPhone = (string.IsNullOrEmpty(userView.Phone)) ? "null" : userView.Phone;
                    string UserHistoryMessage = "changed Phone from " + PreviousPhone + " to " + NewPhone;
                    Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                }
                if(userView.PreviousUserName != userView.UserName)
                {
                        string PreviousUserName = (string.IsNullOrEmpty(userView.PreviousUserName)) ? "null" : userView.PreviousUserName;
                        string NewUserName = (string.IsNullOrEmpty(userView.UserName)) ? "null" : userView.UserName;
                        string UserHistoryMessage = "changed Username from " + PreviousUserName + " to " + NewUserName;
                    Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                }
                //if(userView.PreviousPassword != userView.Password)
                //{
                //        string PreviousPassword = (string.IsNullOrEmpty(userView.PreviousPassword)) ? "null" : userView.PreviousPassword;
                //        string NewPassword = (string.IsNullOrEmpty(userView.Password)) ? "null" : userView.Password;
                //        string UserHistoryMessage = "changed Password from " + userView.PreviousPassword + " to " + userView.Password;
                //    Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                //}
                if (userView.PreviousIsActive != userView.IsActive)
                {
                    string PreviousIsActive = (userView.PreviousIsActive == true) ? "Yes" : "No";
                    string NewIsActive = (userView.IsActive == true) ? "Yes" : "No";
                    string UserHistoryMessage = "changed Active status from " + PreviousIsActive + " to " + NewIsActive;
                    Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                }
                if(userView.PreviousRoleID != userView.RoleID)
                {
                    string PreviousRoleName = (userView.PreviousRoleID > 0) ? _userBAL.GetRoleNameByID(userView.PreviousRoleID) : "null";
                    string NewRoleName = (userView.RoleID > 0) ? _userBAL.GetRoleNameByID(userView.RoleID) : "null";
                    string UserHistoryMessage = "changed Role from " + PreviousRoleName + " to " + NewRoleName;
                    Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                }
                

                //RA
                if(userView.PreviousMobile != userView.Mobile)
                {
                    string PreviousMobile = (string.IsNullOrEmpty(userView.PreviousMobile)) ? "null" : userView.PreviousMobile;
                    string NewMobile = (string.IsNullOrEmpty(userView.Mobile)) ? "null" : userView.Mobile;
                    string UserHistoryMessage = "changed Mobile from " + PreviousMobile + " to " + NewMobile;
                    Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                }
                if(userView.PreviousClientCodePrefix != userView.ClientCodePrefix)
                {
                    string PreviousClientCodePrefix = (string.IsNullOrEmpty(userView.PreviousClientCodePrefix)) ? "null" : userView.PreviousClientCodePrefix;
                    string NewClientCodePrefix = (string.IsNullOrEmpty(userView.ClientCodePrefix)) ? "null" : userView.ClientCodePrefix;
                    string UserHistoryMessage = "changed ClientCode Prefix from " + PreviousClientCodePrefix + " to " + NewClientCodePrefix;
                    Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                }
                if(userView.PreviousGB_RACode != userView.GB_RACode)
                {
                    string PreviousGB_RACode = (string.IsNullOrEmpty(userView.PreviousGB_RACode)) ? "null" : userView.PreviousGB_RACode;
                    string NewGB_RACode = (string.IsNullOrEmpty(userView.GB_RACode)) ? "null" : userView.GB_RACode;
                    string UserHistoryMessage = "changed Reseller Code from " + PreviousGB_RACode + " to " + NewGB_RACode;
                    Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                }
                if(userView.PreviousIsWholeSaler != userView.IsWholeSaler)
                {
                    string PreviousIsWholeSaler = (userView.PreviousIsWholeSaler == true) ? "Yes" : "No";
                    string NewIsWholeSaler = (userView.IsWholeSaler == true) ? "Yes" : "No";
                    string UserHistoryMessage = "changed IsWholeSaler from " + PreviousIsWholeSaler + " to " + NewIsWholeSaler;
                    Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                }

                //Address
                string OldUserAddress = "";
                string NewUserAddress = "";
                if (userView.AddressID == 1)
                {
                    string OldAddress = "";
                    string NewAddress = "";
                    string OldaddressLine3 = "";
                    string OldaddressLine1, OldaddressLine2, OldstreetAddress, OldpostCodeAddress = "";
                    string NewaddressLine1, NewaddressLine2, NewaddressLine3, NewstreetAddress, NewpostCodeAddress = "";
                    if (userView.PreviousAddressID != userView.AddressID)
                    {
                        string PostalAddressType = (userView.PreviousPostalAddressID > 0) ? _userBAL.GetPostalDeliveryNameByID(userView.PreviousPostalAddressID) : "";
                        OldaddressLine1 = (PostalAddressType == "" || PostalAddressType == null) ? "" : PostalAddressType + ((userView.PreviousPostalDeliveryNumber == "" || userView.PreviousPostalDeliveryNumber == null) ? "" : " " + userView.PreviousPostalDeliveryNumber);
                        OldaddressLine2 = userView.PreviousTown + " " + userView.PreviousState + " " + userView.PreviousPostCode;
                        if (!string.IsNullOrWhiteSpace(OldaddressLine3))
                        {
                            OldaddressLine3 = "," + OldaddressLine3;
                        }
                        else
                        {
                            OldaddressLine3 = "";
                        }
                        OldAddress =string.IsNullOrEmpty(OldaddressLine1)? OldaddressLine2+OldaddressLine3 : OldaddressLine1 + "," + OldaddressLine2 + OldaddressLine3;
                        OldUserAddress = OldAddress;
                        if (string.IsNullOrEmpty(userView.UnitTypeID.ToString()) && string.IsNullOrEmpty(userView.UnitNumber))
                        {
                            string StreetTypeName = (userView.StreetTypeID > 0) ? _userBAL.GetStreetTypeNameByID(userView.StreetTypeID) : "";
                            NewaddressLine1 = (userView.StreetNumber == "" || userView.StreetNumber == null) ? "" : userView.StreetNumber + ((userView.StreetName == "" || userView.StreetName == null) ? "" : " " + userView.StreetName) + ((StreetTypeName == "" || StreetTypeName == null) ? "" : " " + StreetTypeName);
                            NewaddressLine2 = userView.Town + " " + userView.State + " " + userView.PostCode;
                            NewaddressLine3 = "";
                        }
                        else
                        {
                            string StreetTypeName = (userView.StreetTypeID > 0) ? _userBAL.GetStreetTypeNameByID(userView.StreetTypeID) : "";
                            string UnitTypeName = (userView.UnitTypeID > 0) ? _userBAL.GetUnitTypeNameByID(userView.UnitTypeID) : "";
                            NewaddressLine1 = UnitTypeName + ((userView.UnitNumber == "" || userView.UnitNumber == null) ? "" : " " + userView.UnitNumber);
                            NewaddressLine2 = (userView.StreetNumber == "" || userView.StreetNumber == null) ? "" : userView.StreetNumber + ((userView.StreetName == "" || userView.StreetName == null) ? "" : " " + userView.StreetName) + ((StreetTypeName == "" || StreetTypeName == null) ? "" : " " + StreetTypeName);
                            NewaddressLine3 = userView.Town + " " + userView.State + " " + userView.PostCode;
                        }
                        if (!string.IsNullOrWhiteSpace(NewaddressLine3))
                        {
                            NewaddressLine3 = "," + NewaddressLine3;
                        }
                        else
                        {
                            NewaddressLine3 = "";
                        }
                        NewAddress = (string.IsNullOrEmpty(NewaddressLine1))? NewaddressLine2+NewaddressLine3 : NewaddressLine1 + "," + NewaddressLine2 + NewaddressLine3;
                        NewUserAddress = NewAddress;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(userView.PreviousUnitTypeID.ToString()) && string.IsNullOrEmpty(userView.PreviousUnitNumber))
                        {
                            string StreetTypeName = (userView.PreviousStreetTypeID > 0) ? _userBAL.GetStreetTypeNameByID(userView.PreviousStreetTypeID) : "";
                            OldaddressLine1 = (userView.PreviousStreetNumber == "" || userView.PreviousStreetNumber == null) ? "" : userView.PreviousStreetNumber + ((userView.PreviousStreetName == "" || userView.PreviousStreetName == null) ? "" : " " + userView.PreviousStreetName) + ((StreetTypeName == "" || StreetTypeName == null) ? "" : " " + StreetTypeName);
                            OldaddressLine2 = userView.PreviousTown + " " + userView.PreviousState + " " + userView.PreviousPostCode;
                            OldaddressLine3 = "";
                        }
                        else
                        {
                            string StreetTypeName = (userView.PreviousStreetTypeID > 0) ? _userBAL.GetStreetTypeNameByID(userView.PreviousStreetTypeID) : "";
                            string UnitTypeName = (userView.PreviousUnitTypeID > 0) ? _userBAL.GetUnitTypeNameByID(userView.PreviousUnitTypeID) : "";
                            OldaddressLine1 = UnitTypeName + ((userView.PreviousUnitNumber == "" || userView.PreviousUnitNumber == null) ? "" : " " + userView.PreviousUnitNumber);
                            OldaddressLine2 = (userView.PreviousStreetNumber == "" || userView.PreviousStreetNumber == null) ? "" : userView.PreviousStreetNumber + ((userView.PreviousStreetName == "" || userView.PreviousStreetName == null) ? "" : " " + userView.PreviousStreetName) + ((StreetTypeName == "" || StreetTypeName == null) ? "" : " " + StreetTypeName);
                            OldaddressLine3 = userView.PreviousTown + " " + userView.PreviousState + " " + userView.PreviousPostCode;
                        }
                        if (!string.IsNullOrWhiteSpace(OldaddressLine3))
                        {
                            OldaddressLine3 = "," + OldaddressLine3;
                        }
                        else
                        {
                            OldaddressLine3 = "";
                        }
                        OldAddress = (string.IsNullOrEmpty(OldaddressLine1))? OldaddressLine2+OldaddressLine3 : OldaddressLine1 + "," + OldaddressLine2 + OldaddressLine3;
                        OldUserAddress = OldAddress;
                        if (string.IsNullOrEmpty(userView.UnitTypeID.ToString()) && string.IsNullOrEmpty(userView.UnitNumber))
                        {
                            string StreetTypeName = (userView.StreetTypeID > 0) ? _userBAL.GetStreetTypeNameByID(userView.StreetTypeID) : "";
                            NewaddressLine1 = (userView.StreetNumber == "" || userView.StreetNumber == null) ? "" : userView.StreetNumber + ((userView.StreetName == "" || userView.StreetName == null) ? "" : " " + userView.StreetName) + ((StreetTypeName == "" || StreetTypeName == null) ? "" : " " + StreetTypeName);
                            NewaddressLine2 = userView.Town + " " + userView.State + " " + userView.PostCode;
                            NewaddressLine3 = "";
                        }
                        else
                        {
                            string StreetTypeName = (userView.StreetTypeID > 0) ? _userBAL.GetStreetTypeNameByID(userView.StreetTypeID) : "";
                            string UnitTypeName = (userView.UnitTypeID > 0) ? _userBAL.GetUnitTypeNameByID(userView.UnitTypeID) : "";
                            NewaddressLine1 = UnitTypeName + ((userView.UnitNumber == "" || userView.UnitNumber == null) ? "" : " " + userView.UnitNumber);
                            NewaddressLine2 = (userView.StreetNumber == "" || userView.StreetNumber == null) ? "" : userView.StreetNumber + ((userView.StreetName == "" || userView.StreetName == null) ? "" : " " + userView.StreetName) + ((StreetTypeName == "" || StreetTypeName == null) ? "" : " " + StreetTypeName);
                            NewaddressLine3 = userView.Town + " " + userView.State + " " + userView.PostCode;
                        }
                        if (!string.IsNullOrWhiteSpace(NewaddressLine3))
                        {
                            NewaddressLine3 = "," + NewaddressLine3;
                        }
                        else
                        {
                            NewaddressLine3 = "";
                        }
                        NewAddress = (string.IsNullOrEmpty(NewaddressLine1))? NewaddressLine2+NewaddressLine3 : NewaddressLine1 + "," + NewaddressLine2 + NewaddressLine3;
                        NewUserAddress = NewAddress;
                    }
                }
                if (userView.AddressID == 2)
                {
                    string OldAddress = "";
                    string NewAddress = "";
                    string OldaddressLine3 = "";
                    string NewaddressLine3 = "";
                    string OldaddressLine1, OldaddressLine2, OldstreetAddress, OldpostCodeAddress = "";
                    string NewaddressLine1, NewaddressLine2, NewstreetAddress, NewpostCodeAddress = "";
                    if (userView.PreviousAddressID != userView.AddressID)
                    {

                        if (string.IsNullOrEmpty(userView.PreviousUnitTypeID.ToString()) && string.IsNullOrEmpty(userView.PreviousUnitNumber))
                        {
                            string StreetTypeName = (userView.PreviousStreetTypeID > 0) ? _userBAL.GetStreetTypeNameByID(userView.PreviousStreetTypeID) : "";
                            OldaddressLine1 = (userView.PreviousStreetNumber == "" || userView.PreviousStreetNumber == null) ? "" : userView.PreviousStreetNumber + ((userView.PreviousStreetName == "" || userView.PreviousStreetName == null) ? "" : " " + userView.PreviousStreetName) + ((StreetTypeName == "" || StreetTypeName == null) ? "" : " " + StreetTypeName);
                            OldaddressLine2 = userView.PreviousTown + " " + userView.PreviousState + " " + userView.PreviousPostCode;
                            OldaddressLine3 = "";
                        }
                        else
                        {
                            string StreetTypeName = (userView.PreviousStreetTypeID > 0) ? _userBAL.GetStreetTypeNameByID(userView.PreviousStreetTypeID) : "";
                            string UnitTypeName = (userView.PreviousUnitTypeID > 0) ? _userBAL.GetUnitTypeNameByID(userView.PreviousUnitTypeID) : "";
                            OldaddressLine1 = UnitTypeName + ((userView.PreviousUnitNumber == "" || userView.PreviousUnitNumber == null) ? "" : " " + userView.PreviousUnitNumber);
                            OldaddressLine2 = (userView.PreviousStreetNumber == "" || userView.PreviousStreetNumber == null) ? "" : userView.PreviousStreetNumber + ((userView.PreviousStreetName == "" || userView.PreviousStreetName == null) ? "" : " " + userView.PreviousStreetName) + ((StreetTypeName == "" || StreetTypeName == null) ? "" : " " + StreetTypeName);
                            OldaddressLine3 = userView.PreviousTown + " " + userView.PreviousState + " " + userView.PreviousPostCode;
                        }
                        if (!string.IsNullOrWhiteSpace(OldaddressLine3))
                        {
                            OldaddressLine3 = "," + OldaddressLine3;
                        }
                        else
                        {
                            OldaddressLine3 = "";
                        }
                        OldAddress = (string.IsNullOrEmpty(OldaddressLine1)) ? OldaddressLine2+OldaddressLine3 : OldaddressLine1 + "," + OldaddressLine2 + OldaddressLine3;
                        OldUserAddress = OldAddress;

                        string PostalAddressType = (userView.PostalAddressID > 0) ? _userBAL.GetPostalDeliveryNameByID(userView.PostalAddressID) : "";
                        NewaddressLine1 = (PostalAddressType == "" || PostalAddressType == null) ? "" : PostalAddressType + ((userView.PostalDeliveryNumber == "" || userView.PostalDeliveryNumber == null) ? "" : " " + userView.PostalDeliveryNumber);
                        NewaddressLine2 = userView.Town + " " + userView.State + " " + userView.PostCode;
                        if (!string.IsNullOrWhiteSpace(NewaddressLine3))
                        {
                            NewaddressLine3 = "," + NewaddressLine3;
                        }
                        else
                        {
                            NewaddressLine3 = "";
                        }
                        NewAddress = (string.IsNullOrEmpty(NewaddressLine1)) ? NewaddressLine2 + NewaddressLine3 : NewaddressLine1 + "," + NewaddressLine2 + NewaddressLine3;
                        NewUserAddress = NewAddress;
                    }
                    else
                    {
                        string PostalAddressType = (userView.PreviousPostalAddressID > 0) ? _userBAL.GetPostalDeliveryNameByID(userView.PreviousPostalAddressID) : "";
                        OldaddressLine1 = (PostalAddressType == "" || PostalAddressType == null) ? "" : PostalAddressType + ((userView.PreviousPostalDeliveryNumber == "" || userView.PreviousPostalDeliveryNumber == null) ? "" : " " + userView.PreviousPostalDeliveryNumber);
                        OldaddressLine2 = userView.PreviousTown + " " + userView.PreviousState + " " + userView.PreviousPostCode;
                        if (!string.IsNullOrWhiteSpace(OldaddressLine3))
                        {
                            OldaddressLine3 = "," + OldaddressLine3;
                        }
                        else
                        {
                            OldaddressLine3 = "";
                        }
                        OldAddress =(string.IsNullOrEmpty(OldaddressLine1)) ? OldaddressLine2 + OldaddressLine3 : OldaddressLine1 + "," + OldaddressLine2 + OldaddressLine3;
                        OldUserAddress = OldAddress;
                        string NewPostalAddressType = (userView.PostalAddressID > 0) ? _userBAL.GetPostalDeliveryNameByID(userView.PostalAddressID) : "";
                        NewaddressLine1 = (NewPostalAddressType == "" || NewPostalAddressType == null) ? "" : NewPostalAddressType + ((userView.PostalDeliveryNumber == "" || userView.PostalDeliveryNumber == null) ? "" : " " + userView.PostalDeliveryNumber);
                        NewaddressLine2 = userView.Town + " " + userView.State + " " + userView.PostCode;
                        if (!string.IsNullOrWhiteSpace(NewaddressLine3))
                        {
                            NewaddressLine3 = "," + NewaddressLine3;
                        }
                        else
                        {
                            NewaddressLine3 = "";
                        }
                        NewAddress = (string.IsNullOrEmpty(NewaddressLine1)) ? NewaddressLine2 + NewaddressLine3 : NewaddressLine1 + "," + NewaddressLine2 + NewaddressLine3;
                        NewUserAddress = NewAddress;
                    }
                }
                if (OldUserAddress != NewUserAddress)
                {
                    string PreviousAdd = (string.IsNullOrEmpty(OldUserAddress)) ? "null" : OldUserAddress;
                    string NewAdd = (string.IsNullOrEmpty(NewUserAddress)) ? "null" : NewUserAddress;
                    string UserHistoryMessage = "changed User Address from " + PreviousAdd + " to " + NewAdd;
                    Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                }
                if(userView.PreviousBSB != userView.BSB)
                {
                    string PreviousBSB = (string.IsNullOrEmpty(userView.PreviousBSB)) ? "null" : userView.PreviousBSB;
                    string NewBSB = (string.IsNullOrEmpty(userView.BSB)) ? "null" : userView.BSB;
                    string UserHistoryMessage = "changed BSB from " +PreviousBSB + " to " + NewBSB;
                    Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                }
                if(userView.PreviousAccountNumber != userView.AccountNumber)
                {
                    string PreviousAccountNo = (string.IsNullOrEmpty(userView.PreviousAccountNumber)) ? "null" : userView.PreviousAccountNumber;
                    string NewAccountNo = (string.IsNullOrEmpty(userView.AccountNumber)) ? "null" : userView.AccountNumber;
                    string UserHistoryMessage = "changed Account Number from " + PreviousAccountNo + " to " + NewAccountNo;
                    Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                }
                if(userView.PreviousAccountName != userView.AccountName)
                {
                    string PreviousAccountName = (string.IsNullOrEmpty(userView.PreviousAccountName)) ? "null" : userView.PreviousAccountName;
                    string NewAccountName = (string.IsNullOrEmpty(userView.AccountName)) ? "null" : userView.AccountName;
                    string UserHistoryMessage = "changed Account Name from " + PreviousAccountName + " to " + NewAccountName;
                    Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                }

                if(userView.PreviousCompanyABN != userView.CompanyABN)
                {
                    string PreviousABN = (string.IsNullOrEmpty(userView.PreviousCompanyABN)) ? "null" : userView.PreviousCompanyABN;
                    string NewABN = (string.IsNullOrEmpty(userView.CompanyABN)) ? "null" : userView.CompanyABN;
                    string UserHistoryMessage = "changed Company ABN from " + PreviousABN + " to " + NewABN;
                    Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                }
                if(userView.PreviousCompanyName != userView.CompanyName)
                {
                    string PreviousCompanyName = (string.IsNullOrEmpty(userView.PreviousCompanyName)) ? "null" : userView.PreviousCompanyName;
                    string NewCompanyName = (string.IsNullOrEmpty(userView.CompanyName)) ? "null" : userView.CompanyName;
                    string UserHistoryMessage = "changed Company Name from " + PreviousCompanyName + " to " + NewCompanyName;
                    Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                }
                if (userView.PreviousCompanyWebsite != userView.CompanyWebsite)
                {
                    string PreviousCompanyWebsite = (string.IsNullOrEmpty(userView.PreviousCompanyWebsite)) ? "null" : userView.PreviousCompanyWebsite;
                    string NewCompanyWebsite = (string.IsNullOrEmpty(userView.CompanyWebsite)) ? "null" : userView.CompanyWebsite;
                    string UserHistoryMessage = "changed Company Website from " + PreviousCompanyWebsite + " to " + NewCompanyWebsite;
                    Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                }
                if (userView.PreviousLoginCompanyName != userView.LoginCompanyName)
                {
                    string PreviousLoginCompanyName = (string.IsNullOrEmpty(userView.PreviousLoginCompanyName)) ? "null" : userView.PreviousLoginCompanyName;
                    string NewLoginCompanyName = (string.IsNullOrEmpty(userView.LoginCompanyName)) ? "null" : userView.LoginCompanyName;
                    string UserHistoryMessage = "changed Login CompanyName from " + PreviousLoginCompanyName + " to " + NewLoginCompanyName;
                    Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                }
                if (userView.PreviousCECAccreditationNumber != userView.CECAccreditationNumber)
                {
                    string PreviousCECAccrdNo = (string.IsNullOrEmpty(userView.PreviousCECAccreditationNumber)) ? "null" : userView.PreviousCECAccreditationNumber;
                    string NewCECAccrdNo = (string.IsNullOrEmpty(userView.CECAccreditationNumber)) ? "null" : userView.CECAccreditationNumber;
                    string UserHistoryMessage = "changed CECAccrediationNumber from " + PreviousCECAccrdNo + " to " + NewCECAccrdNo;
                    Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                }
                if (userView.PreviousElectricalContractorsLicenseNumber != userView.ElectricalContractorsLicenseNumber)
                {
                    string PreviousLicenseNo = (string.IsNullOrEmpty(userView.PreviousElectricalContractorsLicenseNumber)) ? "null" : userView.PreviousElectricalContractorsLicenseNumber;
                    string NewLicenseNo = (string.IsNullOrEmpty(userView.ElectricalContractorsLicenseNumber)) ? "null" : userView.ElectricalContractorsLicenseNumber;
                    string UserHistoryMessage = "changed ElectricalContractorsLicenseNumber from " + PreviousLicenseNo + " to " + NewLicenseNo;
                    Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                }
                if (userView.PreviousCECDesignerNumber != userView.CECDesignerNumber)
                {
                    string PreviousCECDesignerNo = (string.IsNullOrEmpty(userView.PreviousCECDesignerNumber)) ? "null" : userView.PreviousCECDesignerNumber;
                    string NewCECDesignNo = (string.IsNullOrEmpty(userView.CECDesignerNumber)) ? "null" : userView.CECDesignerNumber;
                    string UserHistoryMessage = "changed CECDesignNumber from " + PreviousCECDesignerNo + " to " + NewCECDesignNo;
                    Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                }
                //WholesalerInvoice Details
                if (userView.UserTypeID == 2 && userView.IsWholeSaler == true)
                {
                    if(userView.PreviousWholesalerFirstName != userView.WholesalerFirstName)
                    {
                        string PreviousWHFirstname = (string.IsNullOrEmpty(userView.PreviousWholesalerFirstName)) ? "null" : userView.PreviousWholesalerFirstName;
                        string NewWHFirstName = (string.IsNullOrEmpty(userView.WholesalerFirstName)) ? "null" : userView.WholesalerFirstName;
                        string UserHistoryMessage = "changed Wholesaler FirstName from " + PreviousWHFirstname + " to " + NewWHFirstName;
                        Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                    }
                    if(userView.PreviousWholesalerLastName != userView.WholesalerLastName)
                    {
                        string PreviousWHLastname = (string.IsNullOrEmpty(userView.PreviousWholesalerLastName)) ? "null" : userView.PreviousWholesalerLastName;
                        string NewWHLastName = (string.IsNullOrEmpty(userView.WholesalerLastName)) ? "null" : userView.WholesalerLastName;
                        string UserHistoryMessage = "changed Wholesaler LastName from " + PreviousWHLastname + " to " + NewWHLastName;
                        Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                    }
                    if(userView.PreviousWholesalerEmail != userView.WholesalerEmail)
                    {
                        string PreviousWHEMail = (string.IsNullOrEmpty(userView.PreviousWholesalerEmail)) ? "null" : userView.PreviousWholesalerEmail;
                        string NewWHEmail = (string.IsNullOrEmpty(userView.WholesalerEmail)) ? "null" : userView.WholesalerEmail;
                        string UserHistoryMessage = "changed Wholesaler Email from " + PreviousWHEMail + " to " + NewWHEmail;
                        Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                    }
                    if(userView.PreviousWholesalerMobile != userView.WholesalerMobile)
                    {
                        string PreviousWHMobile = (string.IsNullOrEmpty(userView.PreviousWholesalerMobile)) ? "null" : userView.PreviousWholesalerMobile;
                        string NewWHMobile = (string.IsNullOrEmpty(userView.WholesalerMobile)) ? "null" : userView.WholesalerMobile;
                        string UserHistoryMessage = "changed Wholesaler Mobile from " + PreviousWHMobile + " to " + NewWHMobile;
                        Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                    }
                    if(userView.PreviousWholesalerCompanyABN != userView.WholesalerCompanyABN)
                    {
                        string PreviousWHCompanyABN = (string.IsNullOrEmpty(userView.PreviousWholesalerCompanyABN)) ? "null" : userView.PreviousWholesalerCompanyABN;
                        string NewWHCompanyABN = (string.IsNullOrEmpty(userView.WholesalerCompanyABN)) ? "null" : userView.WholesalerCompanyABN;
                        string UserHistoryMessage = "changed Wholesaler CompanyABN from " + PreviousWHCompanyABN + " to " + NewWHCompanyABN;
                        Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                    }
                    if(userView.PreviousWholesalerCompanyname != userView.WholesalerCompanyName)
                    {
                        string PreviousWHCompanyName = (string.IsNullOrEmpty(userView.PreviousWholesalerCompanyname)) ? "null" : userView.PreviousWholesalerCompanyname;
                        string NewWHCompanyName = (string.IsNullOrEmpty(userView.WholesalerCompanyName)) ? "null" : userView.WholesalerCompanyName;
                        string UserHistoryMessage = "changed Wholesaler CompanyName from " + PreviousWHCompanyName + " to " + NewWHCompanyName;
                        Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                    }
                    if(userView.PreviousWholesalerBSB != userView.WholesalerBSB)
                    {
                        string PreviousWHBSB = (string.IsNullOrEmpty(userView.PreviousWholesalerBSB)) ? "null" : userView.PreviousWholesalerBSB;
                        string NewWHBSB = (string.IsNullOrEmpty(userView.WholesalerBSB)) ? "null" : userView.WholesalerBSB;
                        string UserHistoryMessage = "changed Wholesaler BSB from " + PreviousWHBSB + " to " + NewWHBSB;
                        Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                    }
                    if(userView.PreviousWholesalerAccountNumber != userView.WholesalerAccountNumber)
                    {
                        string PreviousWHAccountNumber = (string.IsNullOrEmpty(userView.PreviousWholesalerAccountNumber)) ? "null" : userView.PreviousWholesalerAccountNumber;
                        string NewWHAccountNumber = (string.IsNullOrEmpty(userView.WholesalerAccountNumber)) ? "null" : userView.WholesalerAccountNumber;
                        string UserHistoryMessage = "changed Wholesaler Account Number from " + PreviousWHAccountNumber + " to " + NewWHAccountNumber;
                        Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                    }
                    if(userView.PreviousWholesalerAccountName != userView.WholesalerAccountName)
                    {
                        string PreviousWHAccountName = (string.IsNullOrEmpty(userView.PreviousWholesalerAccountName)) ? "null" : userView.PreviousWholesalerAccountName;
                        string NewWHAccountName = (string.IsNullOrEmpty(userView.WholesalerAccountName)) ? "null" : userView.WholesalerAccountName;
                        string UserHistoryMessage = "changed Wholesaler Account Name from " + PreviousWHAccountName + " to " + NewWHAccountName;
                        Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                    }
                    string OldWholesalerAddress = "";
                    string NewWholesalerAddress = "";
                    if (WholeSalerIsPostalAddress == 1 )
                    {
                        string OldAddress = "";
                        string NewAddress = "";
                        string OldaddressLine3 = "";
                        string OldaddressLine1, OldaddressLine2, OldstreetAddress, OldpostCodeAddress = "";
                        string NewaddressLine1, NewaddressLine2, NewaddressLine3, NewstreetAddress, NewpostCodeAddress = "";
                        if (userView.PreviousWholesalerIsPostalAddress != WholeSalerIsPostalAddress)
                        {
                            string PostalAddressType = (userView.PreviousWholesalerPostalAddressID > 0) ? _userBAL.GetPostalDeliveryNameByID(userView.PreviousWholesalerPostalAddressID) : "";
                            OldaddressLine1 = (PostalAddressType == "" || PostalAddressType == null) ? "" : PostalAddressType + ((userView.PreviousWholesalerPostalDeliveryNumber == "" || userView.PreviousWholesalerPostalDeliveryNumber == null) ? "" : " " + userView.PreviousWholesalerPostalDeliveryNumber);
                            OldaddressLine2 = userView.PreviousWholesalerTown + " " + userView.PreviousWholesalerState + " " + userView.PreviousWholesalerPostCode;
                            if (!string.IsNullOrWhiteSpace(OldaddressLine3))
                            {
                                OldaddressLine3 = "," + OldaddressLine3;
                            }
                            else
                            {
                                OldaddressLine3 = "";
                            }
                            OldAddress =(string.IsNullOrEmpty(OldaddressLine1))?OldaddressLine2+OldaddressLine3 : OldaddressLine1 + "," + OldaddressLine2 + OldaddressLine3;
                            OldWholesalerAddress = OldAddress;
                            if (string.IsNullOrEmpty(userView.WholesalerUnitTypeID.ToString()) && string.IsNullOrEmpty(userView.WholesalerUnitNumber))
                            {
                                string StreetTypeName = (userView.WholesalerStreetTypeID > 0) ? _userBAL.GetStreetTypeNameByID(userView.WholesalerStreetTypeID) : "";
                                NewaddressLine1 = (userView.WholesalerStreetNumber == "" || userView.WholesalerStreetNumber == null) ? "" : userView.WholesalerStreetNumber + ((userView.WholesalerStreetName == "" || userView.WholesalerStreetName == null) ? "" : " " + userView.WholesalerStreetName) + ((StreetTypeName == "" || StreetTypeName == null) ? "" : " " + StreetTypeName);
                                NewaddressLine2 = userView.WholesalerTown + " " + userView.WholesalerState + " " + userView.WholesalerPostCode;
                                NewaddressLine3 = "";
                            }
                            else
                            {
                                string StreetTypeName = (userView.WholesalerStreetTypeID > 0) ? _userBAL.GetStreetTypeNameByID(userView.WholesalerStreetTypeID) : "";
                                string UnitTypeName = (userView.WholesalerUnitTypeID > 0) ? _userBAL.GetUnitTypeNameByID(userView.WholesalerUnitTypeID) : "";
                                NewaddressLine1 = UnitTypeName + ((userView.WholesalerUnitNumber == "" || userView.WholesalerUnitNumber == null) ? "" : " " + userView.WholesalerUnitNumber);
                                NewaddressLine2 = (userView.WholesalerStreetNumber == "" || userView.WholesalerStreetNumber == null) ? "" : userView.WholesalerStreetNumber + ((userView.WholesalerStreetName == "" || userView.WholesalerStreetName == null) ? "" : " " + userView.WholesalerStreetName) + ((StreetTypeName == "" || StreetTypeName == null) ? "" : " " + StreetTypeName);
                                NewaddressLine3 = userView.WholesalerTown + " " + userView.WholesalerState + " " + userView.WholesalerPostCode;
                            }
                            if (!string.IsNullOrWhiteSpace(NewaddressLine3))
                            {
                                NewaddressLine3 = "," + NewaddressLine3;
                            }
                            else
                            {
                                NewaddressLine3 = "";
                            }
                            NewAddress =(string.IsNullOrEmpty(NewaddressLine1))?NewaddressLine2+NewaddressLine3:  NewaddressLine1 + "," + NewaddressLine2 + NewaddressLine3;
                            NewWholesalerAddress = NewAddress;
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(userView.PreviousWholesalerUnitTypeID.ToString()) && string.IsNullOrEmpty(userView.PreviousWholesalerUnitNumber))
                            {
                                string StreetTypeName = (userView.PreviousWholesalerStreetTypeID > 0) ? _userBAL.GetStreetTypeNameByID(userView.PreviousWholesalerStreetTypeID) : "";
                                OldaddressLine1 = (userView.PreviousWholesalerStreetNumber == "" || userView.PreviousWholesalerStreetNumber == null) ? "" : userView.PreviousWholesalerStreetNumber + ((userView.PreviousWholesalerStreetName == "" || userView.PreviousWholesalerStreetName == null) ? "" : " " + userView.PreviousWholesalerStreetName) + ((StreetTypeName == "" || StreetTypeName == null) ? "" : " " + StreetTypeName);
                                OldaddressLine2 = userView.PreviousWholesalerTown + " " + userView.PreviousWholesalerState + " " + userView.PreviousWholesalerPostCode;
                                OldaddressLine3 = "";
                            }
                            else
                            {
                                string StreetTypeName = (userView.PreviousWholesalerStreetTypeID > 0) ? _userBAL.GetStreetTypeNameByID(userView.PreviousWholesalerStreetTypeID) : "";
                                string UnitTypeName = (userView.PreviousWholesalerUnitTypeID > 0) ? _userBAL.GetUnitTypeNameByID(userView.PreviousWholesalerUnitTypeID) : "";
                                OldaddressLine1 = UnitTypeName + ((userView.PreviousWholesalerUnitNumber == "" || userView.PreviousWholesalerUnitNumber == null) ? "" : " " + userView.PreviousWholesalerUnitNumber);
                                OldaddressLine2 = (userView.PreviousWholesalerStreetNumber == "" || userView.PreviousWholesalerStreetNumber == null) ? "" : userView.PreviousWholesalerStreetNumber + ((userView.PreviousWholesalerStreetName == "" || userView.PreviousWholesalerStreetName == null) ? "" : " " + userView.PreviousWholesalerStreetName) + ((StreetTypeName == "" || StreetTypeName == null) ? "" : " " + StreetTypeName);
                                OldaddressLine3 = userView.PreviousWholesalerTown + " " + userView.PreviousWholesalerState + " " + userView.PreviousWholesalerPostCode;
                            }
                            if (!string.IsNullOrWhiteSpace(OldaddressLine3))
                            {
                                OldaddressLine3 = "," + OldaddressLine3;
                            }
                            else
                            {
                                OldaddressLine3 = "";
                            }
                            OldAddress = (string.IsNullOrEmpty(OldaddressLine1)) ? OldaddressLine2 + OldaddressLine3 : OldaddressLine1 + "," + OldaddressLine2 + OldaddressLine3; ;
                            OldWholesalerAddress = OldAddress;
                            if (string.IsNullOrEmpty(userView.WholesalerUnitTypeID.ToString()) && string.IsNullOrEmpty(userView.WholesalerUnitNumber))
                            {
                                string StreetTypeName = (userView.WholesalerStreetTypeID > 0) ? _userBAL.GetStreetTypeNameByID(userView.WholesalerStreetTypeID) : "";
                                NewaddressLine1 = (userView.WholesalerStreetNumber == "" || userView.WholesalerStreetNumber == null) ? "" : userView.WholesalerStreetNumber + ((userView.WholesalerStreetName == "" || userView.WholesalerStreetName == null) ? "" : " " + userView.WholesalerStreetName) + ((StreetTypeName == "" || StreetTypeName == null) ? "" : " " + StreetTypeName);
                                NewaddressLine2 = userView.WholesalerTown + " " + userView.WholesalerState + " " + userView.WholesalerPostCode;
                                NewaddressLine3 = "";
                            }
                            else
                            {
                                string StreetTypeName = (userView.WholesalerStreetTypeID > 0) ? _userBAL.GetStreetTypeNameByID(userView.WholesalerStreetTypeID) : "";
                                string UnitTypeName = (userView.WholesalerUnitTypeID > 0) ? _userBAL.GetUnitTypeNameByID(userView.WholesalerUnitTypeID) : "";
                                NewaddressLine1 = UnitTypeName + ((userView.WholesalerUnitNumber == "" || userView.WholesalerUnitNumber == null) ? "" : " " + userView.WholesalerUnitNumber);
                                NewaddressLine2 = (userView.WholesalerStreetNumber == "" || userView.WholesalerStreetNumber == null) ? "" : userView.WholesalerStreetNumber + ((userView.WholesalerStreetName == "" || userView.WholesalerStreetName == null) ? "" : " " + userView.WholesalerStreetName) + ((StreetTypeName == "" || StreetTypeName == null) ? "" : " " + StreetTypeName);
                                NewaddressLine3 = userView.WholesalerTown + " " + userView.WholesalerState + " " + userView.WholesalerPostCode;
                            }
                            if (!string.IsNullOrWhiteSpace(NewaddressLine3))
                            {
                                NewaddressLine3 = "," + NewaddressLine3;
                            }
                            else
                            {
                                NewaddressLine3 = "";
                            }
                            NewAddress = (string.IsNullOrEmpty(NewaddressLine1)) ? NewaddressLine2 + NewaddressLine3 : NewaddressLine1 + "," + NewaddressLine2 + NewaddressLine3;
                            NewWholesalerAddress = NewAddress;
                        }
                    }
                    if (WholeSalerIsPostalAddress == 2)
                    {
                        string OldAddress = "";
                        string NewAddress = "";
                        string OldaddressLine3 = "";
                        string NewaddressLine3 = "";
                        string OldaddressLine1, OldaddressLine2, OldstreetAddress, OldpostCodeAddress = "";
                        string NewaddressLine1, NewaddressLine2, NewstreetAddress, NewpostCodeAddress = "";
                        if (userView.PreviousWholesalerIsPostalAddress != WholeSalerIsPostalAddress)
                        {

                            if (string.IsNullOrEmpty(userView.PreviousWholesalerUnitTypeID.ToString()) && string.IsNullOrEmpty(userView.PreviousWholesalerUnitNumber))
                            {
                                string StreetTypeName = (userView.PreviousWholesalerStreetTypeID > 0) ? _userBAL.GetStreetTypeNameByID(userView.PreviousWholesalerStreetTypeID) : "";
                                OldaddressLine1 = (userView.PreviousWholesalerStreetNumber == "" || userView.PreviousWholesalerStreetNumber == null) ? "" : userView.PreviousWholesalerStreetNumber + ((userView.PreviousWholesalerStreetName == "" || userView.PreviousWholesalerStreetName == null) ? "" : " " + userView.PreviousWholesalerStreetName) + ((StreetTypeName == "" || StreetTypeName == null) ? "" : " " + StreetTypeName);
                                OldaddressLine2 = userView.PreviousWholesalerTown + " " + userView.PreviousWholesalerState + " " + userView.PreviousWholesalerPostCode;
                                OldaddressLine3 = "";
                            }
                            else
                            {
                                string StreetTypeName = (userView.PreviousWholesalerStreetTypeID > 0) ? _userBAL.GetStreetTypeNameByID(userView.PreviousWholesalerStreetTypeID) : "";
                                string UnitTypeName = (userView.PreviousWholesalerUnitTypeID > 0) ? _userBAL.GetUnitTypeNameByID(userView.PreviousWholesalerUnitTypeID) : "";
                                OldaddressLine1 = UnitTypeName + ((userView.PreviousWholesalerUnitNumber == "" || userView.PreviousWholesalerUnitNumber == null) ? "" : " " + userView.PreviousWholesalerUnitNumber);
                                OldaddressLine2 = (userView.PreviousWholesalerStreetNumber == "" || userView.PreviousWholesalerStreetNumber == null) ? "" : userView.PreviousWholesalerStreetNumber + ((userView.PreviousWholesalerStreetName == "" || userView.PreviousWholesalerStreetName == null) ? "" : " " + userView.PreviousWholesalerStreetName) + ((StreetTypeName == "" || StreetTypeName == null) ? "" : " " + StreetTypeName);
                                OldaddressLine3 = userView.PreviousWholesalerTown + " " + userView.PreviousWholesalerState + " " + userView.PreviousWholesalerPostCode;
                            }
                            
                            if (!string.IsNullOrWhiteSpace(OldaddressLine3))
                            {
                                OldaddressLine3 = "," + OldaddressLine3;
                            }
                            else
                            {
                                OldaddressLine3 = "";
                            }
                            OldAddress = (string.IsNullOrEmpty(OldaddressLine1)) ? OldaddressLine2 + OldaddressLine3 : OldaddressLine1 + "," + OldaddressLine2 + OldaddressLine3; ;
                            OldWholesalerAddress = OldAddress;

                            string PostalAddressType = (userView.WholesalerPostalAddressID > 0) ? _userBAL.GetPostalDeliveryNameByID(userView.WholesalerPostalAddressID) : "";
                            NewaddressLine1 = (PostalAddressType == "" || PostalAddressType == null) ? "" : PostalAddressType + ((userView.WholesalerPostalDeliveryNumber == "" || userView.WholesalerPostalDeliveryNumber == null) ? "" : " " + userView.WholesalerPostalDeliveryNumber);
                            NewaddressLine2 = userView.WholesalerTown + " " + userView.WholesalerState + " " + userView.WholesalerPostCode;
                            if (!string.IsNullOrWhiteSpace(NewaddressLine3))
                            {
                                NewaddressLine3 = "," + NewaddressLine3;
                            }
                            else
                            {
                                NewaddressLine3 = "";
                            }
                            NewAddress = (string.IsNullOrEmpty(NewaddressLine1)) ? NewaddressLine2 + NewaddressLine3 : NewaddressLine1 + "," + NewaddressLine2 + NewaddressLine3;
                            NewWholesalerAddress = NewAddress;
                        }
                        else
                        {
                            string PostalAddressType = (userView.PreviousWholesalerPostalAddressID > 0) ? _userBAL.GetPostalDeliveryNameByID(userView.PreviousWholesalerPostalAddressID) : "";
                            OldaddressLine1 = (PostalAddressType == "" || PostalAddressType == null) ? "" : PostalAddressType + ((userView.PreviousWholesalerPostalDeliveryNumber == "" || userView.PreviousWholesalerPostalDeliveryNumber == null) ? "" : " " + userView.PreviousWholesalerPostalDeliveryNumber);
                            OldaddressLine2 = userView.PreviousWholesalerTown + " " + userView.PreviousWholesalerState + " " + userView.PreviousWholesalerPostCode;
                            if (!string.IsNullOrWhiteSpace(OldaddressLine3))
                            {
                                OldaddressLine3 = "," + OldaddressLine3;
                            }
                            else
                            {
                                OldaddressLine3 = "";
                            }
                            OldAddress = (string.IsNullOrEmpty(OldaddressLine1)) ? OldaddressLine2 + OldaddressLine3 : OldaddressLine1 + "," + OldaddressLine2 + OldaddressLine3; ;
                            OldWholesalerAddress = OldAddress;
                            string NewPostalAddressType = (userView.WholesalerPostalAddressID > 0) ? _userBAL.GetPostalDeliveryNameByID(userView.WholesalerPostalAddressID) : "";
                            NewaddressLine1 = (NewPostalAddressType == "" || NewPostalAddressType == null) ? "" : NewPostalAddressType + ((userView.WholesalerPostalDeliveryNumber == "" || userView.WholesalerPostalDeliveryNumber == null) ? "" : " " + userView.WholesalerPostalDeliveryNumber);
                            NewaddressLine2 = userView.WholesalerTown + " " + userView.WholesalerState + " " + userView.WholesalerPostCode;
                            if (!string.IsNullOrWhiteSpace(NewaddressLine3))
                            {
                                NewaddressLine3 = "," + NewaddressLine3;
                            }
                            else
                            {
                                NewaddressLine3 = "";
                            }
                            NewAddress = (string.IsNullOrEmpty(NewaddressLine1)) ? NewaddressLine2 + NewaddressLine3 : NewaddressLine1 + "," + NewaddressLine2 + NewaddressLine3;
                            NewWholesalerAddress = NewAddress;
                        }
                    }
                    if(OldWholesalerAddress != NewWholesalerAddress)
                    {
                        string PreviousWHAddress = (string.IsNullOrEmpty(OldWholesalerAddress)) ? "null" : OldWholesalerAddress;
                        string NewWHAddress = (string.IsNullOrEmpty(NewWholesalerAddress)) ? "null" : NewWholesalerAddress;
                        string UserHistoryMessage = "changed Wholesaler Address from " + PreviousWHAddress + " to " + NewWHAddress;
                        Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                    }
                }
                if(userView.PreviousSolarCompanyId != userView.SolarCompanyId)
                {
                    string PreviousSolarCompany = (userView.PreviousSolarCompanyId > 0) ? _userBAL.GetSolarCompanyNamebyID(Convert.ToInt32(userView.PreviousSolarCompanyId)) : "null";
                    string NewSolarCompany = (userView.SolarCompanyId > 0) ? _userBAL.GetSolarCompanyNamebyID(Convert.ToInt32(userView.SolarCompanyId)) : "null";
                    string UserHistoryMessage = "changed SolarCompany from " + PreviousSolarCompany + " to " + NewSolarCompany;
                    Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                }
                if((userView.PreviousResellerID != userView.ResellerID))
                {
                    string PreviousReseller = (userView.PreviousResellerID > 0) ? _userBAL.GetResellerNamebyID(Convert.ToInt32(userView.PreviousResellerID)) : "null";
                    string NewReseller = (userView.ResellerID > 0) ? _userBAL.GetResellerNamebyID(Convert.ToInt32(userView.ResellerID)) : "null";
                    if(PreviousReseller != NewReseller)
                    {
                        string UserHistoryMessage = "changed Reseller from " + PreviousReseller + " to " + NewReseller;
                        Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyy-MM-ddTHH:mm:sszzz"));
                    }
                   
                }
                if(userView.UserTypeID == 2)
                {
                    if(userView.PreviousIsAllowedMasking != userView.isAllowedMasking)
                    {
                        string PreviousIsallowedmasking = (userView.PreviousIsAllowedMasking == true) ? "Yes" : "No";
                        string Newisallowedmasking = (userView.isAllowedMasking == true) ? "Yes" : "No";
                        string UserHistoryMessage = "changed Allow SPV Retailer details unmasking from " + PreviousIsallowedmasking + " to " + Newisallowedmasking;
                        Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyy-MM-ddTHH:mm:sszzz"));
                    }
                }
                this.ShowMessage(SystemEnums.MessageType.Success, "User has been saved successfully. " + strEmailConfigureMsg, true);
                //if (!string.IsNullOrEmpty(userView.Note))
                //{
                //    SaveUserNotetoXML(userView.UserId, userView.Note, userView.NotesType, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                //}
                return RedirectToAction("Index", "User");
            }
            else
            {
                this.ShowMessage(SystemEnums.MessageType.Error, String.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage + " " + v.Exception)), true);
                return RedirectToAction("Edit", "User", new { id = EncryptionDecryption.GetEncrypt("id=" + userView.UserId) });
            }
        }

        public void InsertUpdateResellerRECLoginDetails(FormBot.Entity.User userView)
        {
            bool IsDefault = false;
            bool IsDefaultAdmin = false;
            IsDefault = userView.UseCredentialFrom;
            if (!string.IsNullOrWhiteSpace(userView.hdnCERLoginId)|| !string.IsNullOrEmpty(userView.CERLoginId))
            {
                _userBAL.InsertUpdateResellerRECLoginDetails(userView.ResellerID.Value, userView.FirstName + ' ' + userView.LastName, "Reseller", userView.CERLoginId, userView.CERPassword, userView.RECCompName, userView.RecCompUserName, userView.RECName, ProjectSession.LoggedInUserId, IsDefault);
            }

            if (!string.IsNullOrWhiteSpace(userView.SuperAdminCERLoginId) || !string.IsNullOrWhiteSpace(userView.hdnSuperAdminCERLoginId))
            {
                _userBAL.InsertUpdateResellerRECLoginDetails(userView.ResellerID.Value, userView.FirstName + ' ' + userView.LastName, "Admin", userView.SuperAdminCERLoginId, userView.SuperAdminCERPassword, userView.SuperAdminRECCompName, userView.RecSuperAdminUserName, userView.SuperAdminRECName, ProjectSession.LoggedInUserId, IsDefaultAdmin);
            }
        }

        //public JsonResult SaveMaskingValue(int resellerId, bool isAllowedMasking)
        //{
        //    try
        //    {
        //        _userBAL.SaveMaskingValue(resellerId, isAllowedMasking);
        //        return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        AddLog(ex.Message);
        //        return Json(new { status = false }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        /// <summary>
        /// Get Users
        /// </summary>
        /// <returns>List of Users</returns>
        [HttpGet]
        [UserAuthorization]
        public ActionResult Index()
        {
            //Account acct = Session[FormBot.Email.Constants.sessionAccount] as Account;
            //if (acct != null)
            //{
            //    ViewBag.email = acct.Email;
            FormBot.Entity.User user = new FormBot.Entity.User();
            user.UserTypeID = ProjectSession.UserTypeId;

            FormBot.Email.Account acct = Session[FormBot.Email.Constants.sessionAccount] as FormBot.Email.Account;
            if (acct != null)
            {
                user.ToEmailAddress = acct.Email;
            }

            //if (TempData["msg"] != null)
            //{
            //    this.ShowMessage(SystemEnums.MessageType.Success, TempData["msg"].ToString(), true);
            //}
            return View("Index", user);
            //}
            //else
            //{
            //    return RedirectToAction("EmailSettings", "Email", new { area = "Email" });
            //}
        }

        /// <summary>
        /// Get Solar Company Admin
        /// </summary>
        /// <returns>SCA List</returns>
        [HttpGet]
        [UserAuthorization]
        public ActionResult SCA()
        {
            FormBot.Entity.User user = new FormBot.Entity.User();
            user.UserTypeID = ProjectSession.UserTypeId;
            if (ProjectSession.UserTypeId == 1 || ProjectSession.UserTypeId == 2 || ProjectSession.UserTypeId == 5 || ProjectSession.UserTypeId == 3)
            {
                var statuses = from SystemEnums.UserStatus s in Enum.GetValues(typeof(SystemEnums.UserStatus))
                               select new { ID = s, Name = s.ToString().Replace("_", " ") };
                ViewData["UserStatus"] = new SelectList(statuses, "ID", "Name", TempData["PendingSCADashboardStatus"]);
            }

            if (ProjectSession.UserTypeId == 7 || ProjectSession.UserTypeId == 6)
            {
                var statuses = from SystemEnums.ElectricianStatus s in Enum.GetValues(typeof(SystemEnums.ElectricianStatus))
                               select new { ID = s, Name = s.ToString().Replace('_', ' ') };
                ViewData["ElectricianStatus"] = new SelectList(statuses, "ID", "Name");
            }

            if (TempData["msg"] != null)
            {
                this.ShowMessage(SystemEnums.MessageType.Success, TempData["msg"].ToString(), true);
            }

            FormBot.Email.Account acct = Session[FormBot.Email.Constants.sessionAccount] as FormBot.Email.Account;
            if (acct != null)
            {
                user.ToEmailAddress = acct.Email;
            }

            return View("SCA", user);
        }

        /// <summary>
        /// Get Solar Electrcian
        /// </summary>
        /// <param name="Id">The identifier.</param>
        /// <returns>
        /// Returns List Of Solar Electrician
        /// </returns>
        [HttpGet]
        [UserAuthorization]
        public ActionResult SE(string Id = null)
        {
            if (!string.IsNullOrWhiteSpace(Id))
            {
                int solarElectricianId = 0;
                if (!string.IsNullOrWhiteSpace(Id))
                    int.TryParse(QueryString.GetValueFromQueryString(Id, "id"), out solarElectricianId);
                var dsUsers = _userBAL.GetSolarElectricianById(solarElectricianId);
                List<SolarElectricianView> solarElectrician = DBClient.DataTableToList<SolarElectricianView>(dsUsers.Tables[0]);
                SolarElectricianView solarElectricianView = solarElectrician.FirstOrDefault();
                //var designRole = from SystemEnums.PostalAddressType s in Enum.GetValues(typeof(SystemEnums.PostalAddressType))
                //                 select new { ID = s.GetHashCode(), Name = s.ToString().Replace('_', ' ') };
                ViewBag.PostalAddressType = new SelectList(EnumExtensions.GetPostalAddressEnumList(), "ID", "Name");
                if (solarElectricianView.IsPostalAddress == true)
                {
                    solarElectricianView.AddressID = 2;
                }
                else
                {
                    solarElectricianView.AddressID = 1;
                }

                return View("ViewRequest", solarElectricianView);
            }
            else
            {
                FormBot.Entity.User user = new FormBot.Entity.User();
                user.UserTypeID = ProjectSession.UserTypeId;
                if (ProjectSession.UserTypeId == 1 || ProjectSession.UserTypeId == 3)
                {
                    var statuses = from SystemEnums.UserStatus s in Enum.GetValues(typeof(SystemEnums.UserStatus))
                                   select new { ID = s, Name = s.ToString().Replace("_", " ") };
                    ViewData["UserStatus"] = new SelectList(statuses, "ID", "Name", TempData["PendingDashboardStatus"]);

                    var usertypes = from SystemEnums.UserType s in Enum.GetValues(typeof(SystemEnums.UserType))
                                    where ((int)s == 7 || (int)s == 10)
                                    //select new { ID = (int)s, Name = GetDescription(s) };
                                    select new { value = (((int)s) == 7 ? "pvd" : (((int)s) == 10 ? "swh" : s.ToString())), Name = GetDescription(s) };
                    ViewData["UserType"] = new SelectList(usertypes, "value", "Name");
                }

                if (ProjectSession.UserTypeId == 4 || ProjectSession.UserTypeId == 6 || ProjectSession.UserTypeId == 8)
                {
                    var statuses = from SystemEnums.ElectricianStatus s in Enum.GetValues(typeof(SystemEnums.ElectricianStatus))
                                   select new { ID = s, Name = s.ToString().Replace('_', ' ') };
                    ViewData["ElectricianStatus"] = new SelectList(statuses, "ID", "Name", TempData["SERequestPending"]);
                }

                if (TempData["msg"] != null)
                {
                    this.ShowMessage(SystemEnums.MessageType.Success, TempData["msg"].ToString(), true);
                }

                FormBot.Email.Account acct = Session[FormBot.Email.Constants.sessionAccount] as FormBot.Email.Account;
                if (acct != null)
                {
                    user.ToEmailAddress = acct.Email;
                }

                return View("SE", user);
            }

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
        /// RequestedSE
        /// </summary>
        /// <param name="Id">Id</param>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [UserAuthorization]
        public ActionResult RequestedSE(string Id = null)
        {
            if (!string.IsNullOrWhiteSpace(Id))
            {
                int solarElectricianId = 0;
                if (!string.IsNullOrWhiteSpace(Id))
                    int.TryParse(QueryString.GetValueFromQueryString(Id, "id"), out solarElectricianId);
                var dsUsers = _userBAL.GetSolarElectricianById(solarElectricianId);
                List<SolarElectricianView> solarElectrician = DBClient.DataTableToList<SolarElectricianView>(dsUsers.Tables[0]);
                SolarElectricianView solarElectricianView = solarElectrician.FirstOrDefault();
                //var designRole = from SystemEnums.PostalAddressType s in Enum.GetValues(typeof(SystemEnums.PostalAddressType))
                //                 select new { ID = s.GetHashCode(), Name = s.ToString().Replace('_', ' ') };
                ViewBag.PostalAddressType = new SelectList(EnumExtensions.GetPostalAddressEnumList(), "ID", "Name");
                if (solarElectricianView.IsPostalAddress == true)
                {
                    solarElectricianView.AddressID = 2;
                }
                else
                {
                    solarElectricianView.AddressID = 1;
                }

                IEnumerable usertypes = from SystemEnums.UserType s in Enum.GetValues(typeof(SystemEnums.UserType))
                                        where (s.GetHashCode() == 7 || s.GetHashCode() == 10)
                                        select new { ID = s.GetHashCode(), Name = GetDescription(s) };

                ViewBag.UserTypeId = new SelectList(usertypes, "ID", "Name");

                if (solarElectricianView.IsSWHUser == true)
                    solarElectricianView.SWHLicenseNumber = solarElectricianView.ElectricalContractorsLicenseNumber;

                return View("SendRequest", solarElectricianView);
            }
            else
            {
                FormBot.Entity.User user = new FormBot.Entity.User();
                user.UserTypeID = ProjectSession.UserTypeId;

                if (ProjectSession.UserTypeId == 1 || ProjectSession.UserTypeId == 2 || ProjectSession.UserTypeId == 3 || ProjectSession.UserTypeId == 4 || ProjectSession.UserTypeId == 5 || ProjectSession.UserTypeId == 6 || ProjectSession.UserTypeId == 8)
                {
                    var statuses = from SystemEnums.ElectricianStatus s in Enum.GetValues(typeof(SystemEnums.ElectricianStatus))
                                   select new { ID = s, Name = s.ToString().Replace('_', ' ') };
                    ViewData["ElectricianStatus"] = new SelectList(statuses, "ID", "Name", TempData["SERequestPending"]);
                }

                var usertypes = from SystemEnums.UserType s in Enum.GetValues(typeof(SystemEnums.UserType))
                                where ((int)s == 7 || (int)s == 10)
                                select new { value = (((int)s) == 7 ? "pvd" : (((int)s) == 10 ? "swh" : s.ToString())), Name = GetDescription(s) };
                ViewData["UserType"] = new SelectList(usertypes, "value", "Name");

                if (TempData["msg"] != null)
                {
                    this.ShowMessage(SystemEnums.MessageType.Success, TempData["msg"].ToString(), true);
                }

                return View("RequestedSE", user);
            }
        }

        /// <summary>
        /// SSCs this instance.
        /// </summary>
        /// <returns>SSC List</returns>
        [HttpGet]
        [UserAuthorization]
        public ActionResult SSC()
        {
            FormBot.Entity.User user = new FormBot.Entity.User();
            user.UserTypeID = ProjectSession.UserTypeId;

            var sscstatuses = from SystemEnums.ElectricianStatus s in Enum.GetValues(typeof(SystemEnums.ElectricianStatus))
                              select new { ID = s, Name = s.ToString().Replace('_', ' ') };
            ViewData["SscStatus"] = new SelectList(sscstatuses, "ID", "Name");

            if (TempData["msg"] != null)
            {
                this.ShowMessage(SystemEnums.MessageType.Success, TempData["msg"].ToString(), true);
            }

            return View("SSC", user);
        }

        /// <summary>
        /// Get method for edit user
        /// </summary>
        /// <param name="Id">The identifier.</param>
        /// <returns>Returns UserView</returns>
        [HttpGet]
        [UserAuthorization]
        public ActionResult View(string Id)
        {
            int userId = 0;
            if (!string.IsNullOrWhiteSpace(Id))
                int.TryParse(QueryString.GetValueFromQueryString(Id, "id"), out userId);
            var dsUsers = _userBAL.GetUserById(userId);
            List<FormBot.Entity.User> user = DBClient.DataTableToList<FormBot.Entity.User>(dsUsers.Tables[0]);
            FormBot.Entity.User userView = user.FirstOrDefault();
            userView.EmailSignup = new Entity.Email.EmailSignup();
            if (userView.SolarCompanyId == null)
            {
                userView.SolarCompanyId = 0;
            }

            if (userView.ResellerID == null)
            {
                userView.ResellerID = 0;
            }

            if (userView.IsPostalAddress == true)
            {
                userView.AddressID = 2;
            }
            else
            {
                userView.AddressID = 1;
            }

            if (userView.WholesalerIsPostalAddress == 1)
            {
                userView.WholesalerIsPostalAddress = 2;
            }
            else
            {
                userView.WholesalerIsPostalAddress = 1;
            }

            //var designRole = from SystemEnums.PostalAddressType s in Enum.GetValues(typeof(SystemEnums.PostalAddressType))
            //                 select new { ID = s.GetHashCode(), Name = s.ToString().Replace('_', ' ') };
            ViewBag.PostalAddressType = new SelectList(EnumExtensions.GetPostalAddressEnumList(), "ID", "Name");
            userView.lstUserDocument = _userBAL.GetUserUsersDocumentByUserId(userId);
            userView.lstFCOGroup1 = _userBAL.GetMultipleFCOGroupDropDown();
            userView.ArrFcoGroup = _userBAL.GetMultipleFCOGroupDropDownByUserId(userId, userView.UserTypeID).Select(a => a.FCOGroupId).ToArray();
            DateTime fromDate = Convert.ToDateTime(userView.FromDate);
            DateTime toDate = Convert.ToDateTime(userView.ToDate);
            userView.strFromDate = fromDate.ToString("yyyy-MM-dd");
            userView.strToDate = toDate.ToString("yyyy-MM-dd");
            string date = userView.CreatedDate.Hour >= 12 ? "PM AEST" : "AM AEST";
            userView.DisplayDate = userView.CreatedDate.ToShortDateString() + " " + userView.CreatedDate.Hour + ":" + userView.CreatedDate.Minute + date;
            return View(userView);
        }

        /// <summary>
        /// Get method for edit user
        /// </summary>
        /// <param name="Id">The identifier.</param>
        /// <returns>Edit User</returns>
        [HttpGet]
        [UserAuthorization]
        [GZipOrDeflate]
        public ActionResult Edit(string Id)
        {
            int userId = 0;
            if (!string.IsNullOrWhiteSpace(Id))
                int.TryParse(QueryString.GetValueFromQueryString(Id, "id"), out userId);
            var dsUsers = _userBAL.GetUserById(userId);
            //List<FormBot.Entity.User> user = DBClient.DataTableToList<FormBot.Entity.User>(dsUsers.Tables[0]);
            //FormBot.Entity.User userView = user.FirstOrDefault();
            FormBot.Entity.User userView = GetUserEntity(dsUsers.Tables[0]);
            userView.EmailSignup = new Entity.Email.EmailSignup();
            if (userView.SolarCompanyId == null)
            {
                userView.SolarCompanyId = 0;
            }

            if (userView.ResellerID == null)
            {
                userView.ResellerID = 0;
            }

            if (userView.IsPostalAddress == true)
            {
                userView.AddressID = 2;
            }
            else
            {
                userView.AddressID = 1;
            }

            if (userView.WholesalerIsPostalAddress == 1)
            {
                userView.WholesalerIsPostalAddress = 2;
            }
            else
            {
                userView.WholesalerIsPostalAddress = 1;
            }

            if(userView.InvoicerIsPostalAddress == true)
            {
                userView.InvoicerAddressID = 2;
            }
            else
            {
                userView.InvoicerAddressID = 1;
            }

            if (userView.IsSTC == true)
            {
                userView.chkSTC = 1;
            }
            else
            {
                userView.chkSTC = 0;
            }

            //var designRole = from SystemEnums.PostalAddressType s in Enum.GetValues(typeof(SystemEnums.PostalAddressType))
            //                 select new { ID = s.GetHashCode(), Name = s.ToString().Replace('_', ' ') };
            ViewBag.PostalAddressType = new SelectList(EnumExtensions.GetPostalAddressEnumList(), "ID", "Name");
            userView.lstUserDocument = _userBAL.GetUserUsersDocumentByUserId(userId);
            userView.lstFCOGroup1 = _userBAL.GetMultipleFCOGroupDropDown();
            userView.ArrFcoGroup = _userBAL.GetMultipleFCOGroupDropDownByUserId(userId, userView.UserTypeID).Select(a => a.FCOGroupId).ToArray();
            DateTime fromDate = Convert.ToDateTime(userView.FromDate);
            DateTime toDate = Convert.ToDateTime(userView.ToDate);
            userView.strFromDate = fromDate.ToString("yyyy-MM-dd");
            userView.strToDate = toDate.ToString("yyyy-MM-dd");
            string date = userView.CreatedDate.Hour >= 12 ? "PM AEST" : "AM AEST";
            userView.DisplayDate = userView.CreatedDate.ToShortDateString() + " " + userView.CreatedDate.Hour + ":" + userView.CreatedDate.Minute + date;

            userView.installerDesignerView = new InstallerDesignerView();
            if (ProjectSession.UserTypeId == 1 || ProjectSession.UserTypeId == 2 || ProjectSession.UserTypeId == 3 || ProjectSession.UserTypeId == 4 || ProjectSession.UserTypeId == 5 || ProjectSession.UserTypeId == 6 || ProjectSession.UserTypeId == 8)
            {
                var statuses = from SystemEnums.SEDesignRole s in Enum.GetValues(typeof(SystemEnums.SEDesignRole))
                               select new { ID = s, Name = s.ToString().Replace('_', ' ') };
                ViewData["SEDesignRole"] = new SelectList(statuses, "ID", "Name");
            }
            var NotesType = from SystemEnums.NotesType n in Enum.GetValues(typeof(SystemEnums.NotesType))
                            select new { Text = (int)n, Value = n.ToString() };
            NotesType = NotesType.Where(x => x.Text == 3).ToList();
            List<SelectListItem> NotestypeDropdown = new SelectList(NotesType, "Text", "Value").ToList();
            NotestypeDropdown.Add(new SelectListItem { Text = "Warning", Value = "5" });
            ViewBag.NotesType = NotestypeDropdown;
            var Invoicerlist = new List<SelectListItem>
            {
                new SelectListItem{ Text="Select", Value = "0" },
                new SelectListItem{ Text="EMERGING ENERGY SOLUTIONS GROUP PTY. LTD.", Value = "2965" },
            };
            var UsageTypeList = new List<SelectListItem>
            {
                new SelectListItem{ Text="Reseller", Value = "1" },
                new SelectListItem{ Text="Wholesaler", Value = "2" },
                new SelectListItem{Text="SAAS", Value = "3"}
            };
            ViewBag.UsageType = UsageTypeList;
            ViewBag.Invoicer = Invoicerlist;
            userView.RecEmailSignup = new RecEmailSignup();
            //if(userView.UserTypeID == 2)
            //{
            //    if(string.IsNullOrWhiteSpace(userView.UniqueContactId))
            //    {
            //        userView.UniqueContactId = GetUniqueContactIDforSAAS();
            //    }
            //}

            //Session["UserDetail"] = userView;
            TempData[userView.UserId.ToString()] = userView;

            //FSA
            userView.PreviousFirstName = userView.FirstName;
            userView.PreviousLastName = userView.LastName;
            userView.PreviousEmail = userView.Email;
            userView.PreviousPhone = userView.Phone;
            userView.PreviousUserName = userView.UserName;
            userView.PreviousPassword = userView.Password;
            userView.PreviousIsActive = userView.IsActive;
            userView.PreviousRoleID = userView.RoleID;

            //Reseller
            //1)Personal Details
            userView.PreviousMobile = userView.Mobile;
            userView.PreviousClientCodePrefix = userView.ClientCodePrefix;
            userView.PreviousGB_RACode = userView.GB_RACode;
            userView.PreviousIsWholeSaler = userView.IsWholeSaler;

            //Address Details
            userView.PreviousCompanyABN = userView.CompanyABN;
            userView.PreviousCompanyName = userView.CompanyName;
            userView.PreviousCompanyWebsite = userView.CompanyWebsite;
            userView.PreviousLoginCompanyName = userView.LoginCompanyName;
            userView.PreviousAddressID = userView.AddressID;
            userView.PreviousUnitTypeID = userView.UnitTypeID;
            userView.PreviousUnitNumber = userView.UnitNumber;
            userView.PreviousStreetName = userView.StreetName;
            userView.PreviousStreetNumber = userView.StreetNumber;
            userView.PreviousStreetTypeID = userView.StreetTypeID;
            userView.PreviousTown = userView.Town;
            userView.PreviousState = userView.State;
            userView.PreviousPostCode = userView.PostCode;
            userView.PreviousPostalAddressID = userView.PostalAddressID;
            userView.PreviousPostalDeliveryNumber = userView.PostalDeliveryNumber;

            //Account Details 
            userView.PreviousBSB = userView.BSB;
            userView.PreviousAccountNumber = userView.AccountNumber;
            userView.PreviousAccountName = userView.AccountName;

            //Wholesaler Invoice Details 
            userView.PreviousWholesalerFirstName = userView.WholesalerFirstName;
            userView.PreviousWholesalerLastName = userView.WholesalerLastName;
            userView.PreviousWholesalerEmail = userView.WholesalerEmail;
            userView.PreviousWholesalerPhone = userView.WholesalerPhone;
            userView.PreviousWholesalerMobile = userView.WholesalerMobile;
            userView.PreviousWholesalerCompanyABN = userView.WholesalerCompanyABN;
            userView.PreviousWholesalerCompanyname = userView.WholesalerCompanyName;
            userView.PreviousWholesalerBSB = userView.WholesalerBSB;
            userView.PreviousWholesalerAccountNumber = userView.WholesalerAccountNumber;
            userView.PreviousWholesalerAccountName = userView.WholesalerAccountName;
            userView.PreviousUniqueWholesalerNumber = userView.UniqueWholesalerNumber;
            userView.PreviousWholesalerIsPostalAddress = userView.WholesalerIsPostalAddress;
            userView.PreviousWholesalerUnitTypeID = userView.WholesalerUnitTypeID;
            userView.PreviousWholesalerUnitNumber = userView.WholesalerUnitNumber;
            userView.PreviousWholesalerStreetNumber = userView.WholesalerStreetNumber;
            userView.PreviousWholesalerStreetName = userView.WholesalerStreetName;
            userView.PreviousWholesalerStreetTypeID = userView.WholesalerStreetTypeID;
            userView.PreviousWholesalerTown = userView.WholesalerTown;
            userView.PreviousWholesalerState = userView.WholesalerState;
            userView.PreviousWholesalerPostCode = userView.WholesalerPostCode;
            userView.PreviousWholesalerPostalAddressID = userView.WholesalerPostalAddressID;
            userView.PreviousWholesalerPostalDeliveryNumber = userView.WholesalerPostalDeliveryNumber;

            //SCO
            //AccountDetails
            userView.PreviousCECAccreditationNumber = userView.CECAccreditationNumber;
            userView.PreviousElectricalContractorsLicenseNumber = userView.ElectricalContractorsLicenseNumber;
            userView.PreviousCECDesignerNumber = userView.CECDesignerNumber;

            userView.Note = null;
            if (userView.UserTypeID == 2)
            {
                userView.PreviousIsAllowedMasking = userView.isAllowedMasking;
            }
            if (userView.UserTypeID == 5 || userView.UserTypeID == 2 || userView.UserTypeID == 9)
            {
                userView.PreviousResellerID = userView.ResellerID;
            }
            if (userView.UserTypeID == 8 || userView.UserTypeID ==9)
            {
                userView.PreviousSolarCompanyId = userView.SolarCompanyId;
            }
            return View(userView);
        }

        /// <summary>
        /// Create page for my profile
        /// </summary>
        /// <returns>userView</returns>
        [HttpGet]
        [UserAuthorization]
        [GZipOrDeflate]
        public ActionResult Profile()
        {
            int userId = FormBot.Helper.ProjectSession.LoggedInUserId;
            var dsUsers = _userBAL.GetUserById(userId);
            FormBot.Entity.User userView = GetUserEntity(dsUsers.Tables[0]);
            //List<FormBot.Entity.User> user = DBClient.DataTableToList<FormBot.Entity.User>(dsUsers.Tables[0]);
            //FormBot.Entity.User userView = user.FirstOrDefault();
            userView.lstUserDocument = _userBAL.GetUserUsersDocumentByUserId(userId);
            //userView.solarElectricianView = new SolarElectricianView();
            //userView.solarElectricianView.IsSendRequest = true;
            userView.installerDesignerView = new InstallerDesignerView();

            userView.UserTypeID = ProjectSession.UserTypeId;
            if (ProjectSession.UserTypeId == 1 || ProjectSession.UserTypeId == 2 || ProjectSession.UserTypeId == 3 || ProjectSession.UserTypeId == 4 || ProjectSession.UserTypeId == 5 || ProjectSession.UserTypeId == 6 || ProjectSession.UserTypeId == 8)
            {
                var statuses = from SystemEnums.SEDesignRole s in Enum.GetValues(typeof(SystemEnums.SEDesignRole))
                               select new { ID = s, Name = s.ToString().Replace('_', ' ') };
                ViewData["SEDesignRole"] = new SelectList(statuses, "ID", "Name");
            }
            var UsageTypeList = new List<SelectListItem>
            {
                new SelectListItem{ Text="Reseller", Value = "1" },
                new SelectListItem{ Text="Wholesaler", Value = "2" },
                new SelectListItem{Text="SAAS", Value = "3"}
            };
            ViewBag.UsageType = UsageTypeList;
            var NotesType = from SystemEnums.NotesType n in Enum.GetValues(typeof(SystemEnums.NotesType))
                            select new { Text = (int)n, Value = n.ToString() };
            NotesType = NotesType.Where(x => x.Text == 3).ToList();
            List<SelectListItem> NotestypeDropdown = new SelectList(NotesType, "Text", "Value").ToList();
            NotestypeDropdown.Add(new SelectListItem { Text = "Warning", Value = "5" });
            ViewBag.NotesType = NotestypeDropdown;

            var Invoicerlist = new List<SelectListItem>
            {
                new SelectListItem{ Text="Select", Value = "0" },
                new SelectListItem{ Text="EMERGING ENERGY SOLUTIONS GROUP PTY. LTD.", Value = "2965" },
            };
            ViewBag.Invoicer = Invoicerlist;

            if (userView.IsPostalAddress == true)
            {
                userView.AddressID = 2;
            }
            else
            {
                userView.AddressID = 1;
            }

            if (userView.WholesalerIsPostalAddress == 1)
            {
                userView.WholesalerIsPostalAddress = 2;
            }
            else
            {
                userView.WholesalerIsPostalAddress = 1;
            }

            if(userView.InvoicerIsPostalAddress == true)
            {
                userView.InvoicerAddressID = 2;
            }
            else
            {
                userView.InvoicerAddressID = 1;
            }
            //Rec email changes
            userView.RecEmailSignup = new Entity.Email.RecEmailSignup();
            userView.EmailSignup = new Entity.Email.EmailSignup();

            #region Email configuration not required

            //Email configuration is not required at a time of first login
            userView.EmailSignup.IsRequired = false;
            userView.RecEmailSignup.RecIsRequired = false;
            #endregion


            //var designRole = EnumExtensions.GetPostalAddressEnumList();

            ViewBag.PostalAddressType = new SelectList(EnumExtensions.GetPostalAddressEnumList(), "ID", "Name");

            //Rec email changes
            RecEmailConfiguration(userView);

            if (userView.RecEmailSignup.RecConfigurationPassword != null)
            {
                userView.RecEmailSignup.RecConfigurationPassword = Utils.DecodePassword(userView.RecEmailSignup.RecConfigurationPassword);
            }

            EmailConfiguration(userView);

            if (userView.IsSTC == true)
            {
                userView.chkSTC = 1;
            }
            else
            {
                userView.chkSTC = 0;
            }

            if (userView.EmailSignup.ConfigurationPassword != null)
            {
                userView.EmailSignup.ConfigurationPassword = Utils.DecodePassword(userView.EmailSignup.ConfigurationPassword);
            }


            DateTime fromDate = Convert.ToDateTime(userView.FromDate);
            DateTime toDate = Convert.ToDateTime(userView.ToDate);
            userView.strFromDate = fromDate.ToString("yyyy-MM-dd");
            userView.strToDate = toDate.ToString("yyyy-MM-dd");

            if (ProjectSession.UserTypeId == 4)
            {
                Session["SCAUserProfile"] = userView;
            }

            if (userView.IsSWHUser == true)
            {
                userView.SWHLicenseNumber = userView.ElectricalContractorsLicenseNumber;
            }
            TempData[userView.UserId.ToString()] = userView;
            if (!string.IsNullOrWhiteSpace(Request.QueryString["flg"]))
            {
                TempData["Flag"] = Request.QueryString["flg"].ToString();
                userView.Flag = Request.QueryString["flg"].ToString();
            }
            userView.Note = null;
            return View(userView);
        }


        /// <summary>
        /// Post method for my profile user logedin
        /// </summary>
        /// <param name="userView">userView</param>
        /// <param name="emailModel">emailModel</param>
        /// <param name="recEmailModel">recEmailModel</param>
        /// <returns>Task</returns>
        [HttpPost]
        public async Task<ActionResult> Profile(FormBot.Entity.User userView, FormBot.Entity.Email.EmailSignup emailModel, FormBot.Entity.Email.RecEmailSignup recEmailModel, string SCisAllowedSPVswitch = "off")
        {
            userView.UserTypeID = ProjectSession.UserTypeId;
            userView.UserId = FormBot.Helper.ProjectSession.LoggedInUserId;

            if (userView.IsSWHUser == true)
            {
                userView.SWHLicenseNumber = userView.ElectricalContractorsLicenseNumber;
                ModelState.Remove("SWHLicenseNumber");
            }
            ModelState.Remove("CustomCompanyName");
            if (ProjectSession.UserTypeId == 1 || ProjectSession.UserTypeId == 2 || ProjectSession.UserTypeId == 3 || ProjectSession.UserTypeId == 4 || ProjectSession.UserTypeId == 5 || ProjectSession.UserTypeId == 6 || ProjectSession.UserTypeId == 8)
            {
                var statuses = from SystemEnums.SEDesignRole s in Enum.GetValues(typeof(SystemEnums.SEDesignRole))
                               select new { ID = s, Name = s.ToString().Replace('_', ' ') };
                ViewData["SEDesignRole"] = new SelectList(statuses, "ID", "Name");
            }

            #region Remove Email configuration on first time login

            //ModelState.Remove("ConfigurationEmail");
            //ModelState.Remove("ConfigurationEmail");
            #endregion

            RequiredValidationField(userView);

            if (!userView.IsWholeSaler)
                RemoveWholeSalerValidation();
            if (userView.UserTypeID != 2 || (userView.UserTypeID == 2 && userView.UsageType != 3))
            {
                RemoveSAASUserValidation();
            }
            string xmlString = string.Empty;
            ModelState.Remove("Login");

            //ModelState.Remove("solarElectricianView");
            RemoveInstallerDesignerValidation();

            //Rec Email Configuration
            ModelState.Remove("RecLogin");
            if (userView.UserTypeID == 4)
            {
                userView.SCisAllowedSPV = SCisAllowedSPVswitch.ToLower() == "on" ? true : false;
            }
            if (ModelState.IsValid)
            {
                //var designRole = from SystemEnums.PostalAddressType s in Enum.GetValues(typeof(SystemEnums.PostalAddressType))
                //                 select new { ID = s.GetHashCode(), Name = s.ToString().Replace('_', ' ') };
                ViewBag.PostalAddressType = new SelectList(EnumExtensions.GetPostalAddressEnumList(), "ID", "Name");
                userView.lstFCOGroup1 = _userBAL.GetMultipleFCOGroupDropDown();
                userView.EmailSignup = new Entity.Email.EmailSignup();
                userView.RecEmailSignup = new Entity.Email.RecEmailSignup();

                #region Rec Email Configuration
                // Email configuration not required (check if all fields are entered by user then only configure mail account)
                if (recEmailModel != null && !string.IsNullOrWhiteSpace(recEmailModel.RecConfigurationEmail) && !string.IsNullOrWhiteSpace(recEmailModel.RecConfigurationPassword) && !string.IsNullOrWhiteSpace(recEmailModel.RecIncomingMail) && !string.IsNullOrWhiteSpace(recEmailModel.RecOutgoingMail))
                {
                    #region Rec Email configuration code
                    //recEmailModel.RecLogin = recEmailModel.RecConfigurationEmail;
                    //xmlString = "<?xml version='1.0' encoding='UTF-8'?><webmail><param name='action' value='login' /><param name='request' value='' /><param name='email'><![CDATA[" + recEmailModel.RecConfigurationEmail
                    //    + "]]></param><param name='mail_inc_login'><![CDATA[" + recEmailModel.RecLogin + "]]></param><param name='mail_inc_pass'><![CDATA[" + recEmailModel.RecConfigurationPassword + "]]></param><param name='mail_inc_host'><![CDATA[" + recEmailModel.RecIncomingMail
                    //    + "]]></param><param name='mail_inc_port' value='" + recEmailModel.RecIncomingMailPort + "' /><param name='mail_protocol' value='0' /><param name='mail_out_host'><![CDATA[" + recEmailModel.RecOutgoingMail
                    //    + "]]></param><param name='mail_out_port' value='" + recEmailModel.RecOutgoingMailPort + "' /><param name='mail_out_auth' value='1' /><param name='sign_me' value='0' /><param name='language' /><param name='advanced_login' value='1' /></webmail>";
                    //CheckMail checkMail = new CheckMail();
                    //var result = checkMail.GetMessages(xmlString);
                    //XDocument xDocument = XDocument.Parse(result.OuterXml);


                    //var error = xDocument.Descendants("webmail").Elements("error").FirstOrDefault();
                    //if (error != null && !string.IsNullOrEmpty(Convert.ToString(error.Value)))
                    //{
                    //    userView.lstUserDocument = _userBAL.GetUserUsersDocumentByUserId(userView.UserId);
                    //    this.ShowMessage(SystemEnums.MessageType.Error, error.Value, false);
                    //    return View("MyProfile", userView);
                    //}
                    //else
                    //{
                    //    Account acct = Session[FormBot.Email.Constants.sessionAccount] as Account;
                    //    if (acct != null)
                    //    {
                    //        xmlString = "<?xml version='1.0' encoding='utf-8'?><webmail><param name='action' value='update'/><param name='request' value='signature'/><param name='id_acct' value='" + acct.ID
                    //            + "'/><signature type='0' opt='1'><![CDATA[" + recEmailModel.RecEmailSignature + "]]></signature></webmail>";
                    //        result = checkMail.GetMessages(xmlString);
                    //        _userBAL.Rec_EmailMappingInsertUpdate(recEmailModel.RecConfigurationEmail, recEmailModel.RecIncomingMail,recEmailModel.RecConfigurationEmail,
                    //            recEmailModel.RecConfigurationPassword,recEmailModel.RecIncomingMailPort,recEmailModel.RecOutgoingMail,recEmailModel.RecOutgoingMailPort,recEmailModel.RecEmailSignature,"1",0, ProjectSession.LoggedInUserId);

                    //        ////This will Update TimeZone
                    //        xmlString = "<?xml version='1.0' encoding='utf-8'?><webmail><param name='action' value='update'/><param name='request' value='settings'/><param name='id_acct' value='" + acct.ID
                    //            + "'/><settings id_acct='" + acct.ID + "' msgs_per_page='10000' contacts_per_page='20' auto_checkmail_interval='0' allow_dhtml_editor='1' def_charset_out='65001' def_timezone='" + recEmailModel.RecDef_TimeZone + "' time_format='1' view_mode='1'><def_skin><![CDATA[AfterLogic]]></def_skin><def_lang><![CDATA[English]]></def_lang></settings></webmail>";
                    //        checkMail.GetMessages(xmlString);
                    //    }
                    //}

                    ////_userBAL.Rec_EmailMappingInsertUpdate(recEmailModel.RecConfigurationEmail, recEmailModel.RecIncomingMail,recEmailModel.RecConfigurationEmail,, ProjectSession.LoggedInUserId); 
                    #endregion

                    _userBAL.Rec_EmailMappingInsertUpdate(recEmailModel.RecConfigurationEmail, recEmailModel.RecIncomingMail, recEmailModel.RecConfigurationEmail,
                               Utils.EncodePassword(recEmailModel.RecConfigurationPassword), recEmailModel.RecIncomingMailPort, recEmailModel.RecOutgoingMail,
                               recEmailModel.RecOutgoingMailPort, recEmailModel.RecEmailSignature, "1", 0, ProjectSession.LoggedInUserId, recEmailModel.RecDef_TimeZone);

                }

                #endregion

                Session[FormBot.Email.Constants.sessionAccount] = null;
                // Email configuration not required (check if all fields are entered by user then only configure mail account)
                if (!string.IsNullOrWhiteSpace(emailModel.ConfigurationEmail) && !string.IsNullOrWhiteSpace(emailModel.ConfigurationPassword) && !string.IsNullOrWhiteSpace(emailModel.IncomingMail) && !string.IsNullOrWhiteSpace(emailModel.OutgoingMail))
                {
                    emailModel.Login = emailModel.ConfigurationEmail;
                    xmlString = "<?xml version='1.0' encoding='UTF-8'?><webmail><param name='action' value='login' /><param name='request' value='' /><param name='email'><![CDATA[" + emailModel.ConfigurationEmail
                        + "]]></param><param name='mail_inc_login'><![CDATA[" + emailModel.Login + "]]></param><param name='mail_inc_pass'><![CDATA[" + emailModel.ConfigurationPassword + "]]></param><param name='mail_inc_host'><![CDATA[" + emailModel.IncomingMail
                        + "]]></param><param name='mail_inc_port' value='" + emailModel.IncomingMailPort + "' /><param name='mail_protocol' value='0' /><param name='mail_out_host'><![CDATA[" + emailModel.OutgoingMail
                        + "]]></param><param name='mail_out_port' value='" + emailModel.OutgoingMailPort + "' /><param name='mail_out_auth' value='1' /><param name='sign_me' value='0' /><param name='language' /><param name='advanced_login' value='1' /></webmail>";
                    CheckMail checkMail = new CheckMail();
                    var result = checkMail.GetMessages(xmlString);
                    XDocument xDocument = XDocument.Parse(result.OuterXml);

                    var error = xDocument.Descendants("webmail").Elements("error").FirstOrDefault();
                    if (error != null && !string.IsNullOrWhiteSpace(Convert.ToString(error.Value)))
                    {
                        userView.lstUserDocument = _userBAL.GetUserUsersDocumentByUserId(userView.UserId);
                        this.ShowMessage(SystemEnums.MessageType.Error, error.Value, false);
                        return View("Profile", userView);
                    }
                    else
                    {
                        FormBot.Email.Account acct = Session[FormBot.Email.Constants.sessionAccount] as FormBot.Email.Account;
                        if (acct != null)
                        {
                            xmlString = "<?xml version='1.0' encoding='utf-8'?><webmail><param name='action' value='update'/><param name='request' value='signature'/><param name='id_acct' value='" + acct.ID
                                + "'/><signature type='0' opt='1'><![CDATA[" + emailModel.EmailSignature + "]]></signature></webmail>";
                            result = checkMail.GetMessages(xmlString);
                            _userBAL.EmailMappingInsertUpdate(acct.ID, ProjectSession.LoggedInUserId);

                            ////This will Update TimeZone
                            xmlString = "<?xml version='1.0' encoding='utf-8'?><webmail><param name='action' value='update'/><param name='request' value='settings'/><param name='id_acct' value='" + acct.ID
                                + "'/><settings id_acct='" + acct.ID + "' msgs_per_page='10000' contacts_per_page='20' auto_checkmail_interval='0' allow_dhtml_editor='1' def_charset_out='65001' def_timezone='" + emailModel.Def_TimeZone + "' time_format='1' view_mode='1'><def_skin><![CDATA[AfterLogic]]></def_skin><def_lang><![CDATA[English]]></def_lang></settings></webmail>";
                            checkMail.GetMessages(xmlString);
                        }
                    }

                    #region Email configuration not required

                    ProjectSession.IsUserEmailAccountConfigured = true;
                    #endregion
                } // Email configuration not required (add } here)

                UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                UserManager.UserValidator = new UserValidator<ApplicationUser>(UserManager) { AllowOnlyAlphanumericUserNames = false };

                var userDetails = UserManager.FindById(userView.AspNetUserId);

                if (userDetails != null)
                {
                    userDetails.Email = userView.Email;
                    userDetails.UserName = userView.UserName.Replace(" ", "");
                    PasswordHasher hasher = new PasswordHasher();
                    if (userView.Password != null)
                    {
                        userDetails.PasswordHash = hasher.HashPassword(userView.Password);
                    }
                }


                await UserManager.UpdateAsync(userDetails);
                userView.ModifiedBy = ProjectSession.LoggedInUserId;
                userView.ModifiedDate = DateTime.Today;
                string dbuserName = string.Empty;
                if (userView.UserTypeID == 5)
                {
                    DataTable userData = _createJobBAL.GetUserNameSolarCompanyForCache(userView.UserId, 0).Tables[0];
                    if (userData.Rows.Count > 0)
                    {
                        dbuserName = userData.Rows[0]["UserFNameLname"].ToString();
                    }

                }

                if (userView.Signature != null && ProjectSession.UserTypeId == 2)
                {
                    userView.Logo = ProjectSession.LoggedInUserId + "/" + userView.Signature;
                    ProjectSession.Logo = userView.Logo;
                }
                else
                {
                    userView.Logo = "Images/logo.png";
                    ProjectSession.Logo = userView.Logo;
                }

                if (userView.AddressID == 2)
                {
                    userView.IsPostalAddress = true;
                }
                else
                {
                    userView.IsPostalAddress = false;
                }

                if (userView.WholesalerIsPostalAddress == 2)
                {
                    userView.WholesalerIsPostalAddress = 1;
                }
                else
                {
                    userView.WholesalerIsPostalAddress = 0;
                }

                if (userView.strFromDate != null)
                {
                    userView.FromDate = Convert.ToDateTime(userView.strFromDate);
                }

                if (userView.strToDate != null)
                {
                    userView.ToDate = Convert.ToDateTime(userView.strToDate);
                }

                if (userView.UserTypeID == 2)
                {
                    if (userView.hiddenTheme != 0)
                    {
                        ProjectSession.Theme = ((SystemEnums.Theme)userView.hiddenTheme).ToString();
                        userView.Theme = userView.hiddenTheme;
                    }
                }
                else
                {
                    if (ProjectSession.Theme != "")
                    {
                        userView.Theme = Convert.ToInt32((SystemEnums.UserStatus)Enum.Parse(typeof(SystemEnums.Theme), ProjectSession.Theme).GetHashCode());
                    }
                    else
                    {
                        userView.Theme = 1;
                    }
                }

                if (userView.CompanyABN != null)
                {
                    userView.CompanyABN = userView.CompanyABN.Replace(" ", "");
                }

                int chkSTC = _userBAL.GetISSTCforSCA(ProjectSession.LoggedInUserId);
                FormBot.Entity.User uvuser = TempData[userView.UserId.ToString()] as FormBot.Entity.User;
                userView.RecPassword = userView.RecPassword != null ? userView.RecPassword : uvuser.RecPassword;
                //DataTable dt = _userBAL.GetRecDataFromUserId(userView.UserId);
                //userView.RecUserName = dt.Rows[0]["RecUserName"].ToString();
                //userView.RecPassword = dt.Rows[0]["RecPassword"].ToString();
                _userBAL.Create(userView);
               
                    //if (ProjectSession.UserTypeId == 4)
                    //{
                    //    CommonBAL.SetCacheDataForSTCSubmissionFromSolarCompanyId(Convert.ToInt32(userView.ResellerID), Convert.ToString(ProjectSession.SolarCompanyId));
                    //}
                    if (ProjectSession.UserTypeId == 2)
                {
                    InsertUpdateResellerRECLoginDetails(userView);
                }
                if (ProjectSession.UserTypeId == 5)
                {
                    DataSet ds = _createJobBAL.GetUserNameSolarCompanyForCache(userView.UserId, ProjectSession.UserTypeId);
                    if (ds.Tables.Count > 0 && ds.Tables[1].Rows.Count > 0)
                    {
                        DataTable dtSolarCompany = ds.Tables[1];
                        if (dtSolarCompany.Rows.Count > 0)
                        {
                            SortedList<string, string> data = new SortedList<string, string>();
                            string updatedUserName = userView.FirstName + " " + userView.LastName;
                            // string updatedcompanyName = userView.CompanyName;
                            if (dbuserName != updatedUserName)
                            {
                                data.Add("AccountManager", updatedUserName);
                                for (int i = 0; i < dtSolarCompany.Rows.Count; i++)
                                {
                                   await CommonBAL.SetCacheDataOnSCARARAMForSTCSubmission(Convert.ToInt32(dtSolarCompany.Rows[i]["SolarCompanyID"]), data);
                                }
                            }
                        }
                    }

                }
                if (ProjectSession.UserTypeId == 4 && Session["SCAUserProfile"] != null)
                {
                    FormBot.Entity.User uv = Session["SCAUserProfile"] as FormBot.Entity.User;
                    EmailNotificationForSCAUser(uv, userView, ProjectSession.SolarCompanyId);
                }

                //if (ProjectSession.UserTypeId == 4 && Session["SCAUserProfile"] != null)
                //{
                //    FormBot.Entity.User uv = Session["SCAUserProfile"] as FormBot.Entity.User;
                //    EmailInfo emailInfo = new EmailInfo();
                //    emailInfo.TemplateID = 12;
                //    emailInfo.FirstName = userView.FirstName;
                //    emailInfo.LastName = userView.LastName;
                //    StringBuilder sb = new StringBuilder();

                //    if (uv.UserName != userView.UserName || uv.Password != userView.Password)
                //    {
                //        sb.Append(uv.UserName != userView.UserName ? "UserName: " + userView.UserName : string.Empty);
                //        sb.Append(uv.Password != userView.Password ? "Password: " + userView.Password : string.Empty);
                //    }
                //    if (uv.CompanyABN != userView.CompanyABN || uv.CompanyName != userView.CompanyName || uv.BSB != userView.BSB || uv.AccountNumber != userView.AccountNumber || uv.AccountName != userView.AccountName)
                //    {
                //        sb.Append("<table border=\"1\">");
                //        sb.Append("<thead><tr><td>Account Details</td><td>Old Value</td><td>New Value</td></tr></thead>");
                //        sb.Append("<tbody>");
                //        sb.Append(uv.CompanyABN != userView.CompanyABN ? "<tr><td>CompanyABN</td><td>" + uv.CompanyABN + "</td><td>" + userView.CompanyABN + "</td></tr>" : string.Empty);
                //        sb.Append(uv.CompanyName != userView.CompanyName ? "<tr><td>Company Name</td><td>" + uv.CompanyName + "</td><td>" + userView.CompanyName + "</td></tr>" : string.Empty);
                //        sb.Append(uv.BSB != userView.BSB ? "<tr><td>BSB</td><td>" + uv.BSB + "</td><td>" + userView.BSB + "</td></tr>" : string.Empty);
                //        sb.Append(uv.AccountNumber != userView.AccountNumber ? "<tr><td>Account Number</td><td>" + uv.AccountNumber + "</td><td>" + userView.AccountNumber + "</td></tr>" : string.Empty);
                //        sb.Append(uv.AccountName != userView.AccountName ? "<tr><td>Account Name</td><td>" + uv.AccountName + "</td><td>" + userView.AccountName + "</td></tr>" : string.Empty);
                //        sb.Append("</tbody>");
                //        sb.Append("</table>");
                //        emailInfo.Details = sb.ToString();
                //        string mailAddresses = userView.Email + ";" + _userBAL.GetRARAMEmailAddress(ProjectSession.SolarCompanyId);
                //        //Below Changes by BANSI KANERIYA - For Client Requirment- @notication mail id(live id) in from address.
                //        //FormBot.Email.Account acct = System.Web.HttpContext.Current.Session[FormBot.Email.Constants.sessionAccount] as FormBot.Email.Account;
                //        //if (acct != null)
                //        //{
                //        //    _emailBAL.ComposeAndSendEmail(emailInfo, mailAddresses);
                //        //}
                //        //else
                //        //{
                //        EmailTemplate emailTempalte = _emailBAL.GetEmailTemplateByID(emailInfo.TemplateID);
                //        if (emailTempalte != null && !string.IsNullOrEmpty(emailTempalte.Content))
                //        {
                //            string FailReason = string.Empty;
                //            SMTPDetails smtpDetail = new SMTPDetails();
                //            smtpDetail.MailFrom = ProjectSession.MailFrom;
                //            smtpDetail.SMTPUserName = ProjectSession.SMTPUserName;
                //            smtpDetail.SMTPPassword = ProjectSession.SMTPPassword;
                //            smtpDetail.SMTPHost = ProjectSession.SMTPHost;
                //            smtpDetail.SMTPPort = Convert.ToInt32(ProjectSession.SMTPPort);
                //            smtpDetail.IsSMTPEnableSsl = ProjectSession.IsSMTPEnableSsl;
                //            string body = _emailBAL.GetEmailBody(emailInfo, emailTempalte);
                //            ComposeEmail composeEmail = new Email.ComposeEmail();
                //            composeEmail.Subject = emailTempalte.Subject;
                //            composeEmail.Body = new innerBody();
                //            composeEmail.Body.body = body;
                //            bool status1 = MailHelper.SendMail(smtpDetail, mailAddresses, null, null, emailTempalte.Subject, composeEmail.Body.body, null, true, ref FailReason, false);
                //        }
                //        //}
                //    }
                //}
                if (userView.UserTypeID == 4 || userView.UserTypeID == 6 || userView.UserTypeID == 9)
                {
                    if (userView.ProofFileNamesCreate != null)
                    {
                        foreach (var files in userView.ProofFileNamesCreate)
                        {
                            //userView.FileName = files;
                            if (files.ProofDocumentType > 0)
                            {
                                _userBAL.InsertUserDocument(userView.UserId, files.FileName, files.ProofDocumentType);
                            }
                        }
                    }
                }
                else
                {
                    if (userView.FileNamesCreate != null)
                    {
                        foreach (var files in userView.FileNamesCreate)
                        {
                            userView.FileName = files;
                            _userBAL.InsertUserDocument(ProjectSession.LoggedInUserId, files);
                        }
                    }
                    if (userView.ProofFileNamesCreate != null)
                    {
                        foreach (var files in userView.ProofFileNamesCreate)
                        {
                            if (files.ProofDocumentType != 5)
                            {
                                userView.FileName = files.FileName;
                                if (userView.UserTypeID == 7)
                                {
                                    _userBAL.InsertUserDocument(userView.UserId, userView.FileName, files.ProofDocumentType, files.DocLoc);
                                }
                                else
                                {
                                    _userBAL.InsertUserDocument(userView.UserId, userView.FileName, files.ProofDocumentType);
                                }
                            }
                        }
                    }
                    if (userView.UserTypeID == 7 && (userView.Flag != null && userView.Flag.ToString() == "DocUpload"))
                    {
                        SendMailForDocumentsFSA(userView);
                    }
                }
                // Login SCA wants to join SE
                if (ProjectSession.UserTypeId == 4 && userView.IsSTC == true && chkSTC.Equals(0))
                {
                    int seID = _userBAL.InsertSCAasSE(userView);
                    userView.lstUserDocument = _userBAL.GetUserUsersDocumentByUserId(ProjectSession.LoggedInUserId);
                    for (int i = 0; i < userView.lstUserDocument.Count; i++)
                    {
                        _userBAL.InsertUserDocument(seID, userView.lstUserDocument[i].DocumentPath);
                    }

                    string destinationDirectory = ProjectSession.ProofDocumentsURL + "\\" + "UserDocuments" + "\\" + ProjectSession.LoggedInUserId;

                    // SCA want join as SE userID
                    string seDestinationDirectory = ProjectSession.ProofDocumentsURL + "\\" + "UserDocuments" + "\\" + seID;

                    if (userView.FileNamesCreate != null || userView.ProofFileNamesCreate != null || userView.Signature != null)
                    {
                        try
                        {
                            if (userView.IsSTC == true && ProjectSession.UserTypeId == 4)
                            {
                                // SCA want join as SE
                                Directory.CreateDirectory(seDestinationDirectory);
                                DirectoryInfo dir = new DirectoryInfo(destinationDirectory);
                                FileInfo[] files = dir.GetFiles();
                                foreach (FileInfo file in files)
                                {
                                    // Create the path to the new copy of the file.
                                    string temppath = Path.Combine(seDestinationDirectory, file.Name);

                                    // Copy the file.
                                    file.CopyTo(temppath, false);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                }

                this.ShowMessage(SystemEnums.MessageType.Success, "User profile details has been saved successfully.", true);
                //TempData["Flag"] = null;
                ProjectSession.Theme = ((SystemEnums.Theme)userView.Theme).ToString();
                ProjectSession.LoggedInName = userView.FirstName + " " + userView.LastName;
                if (userView.Flag != null && userView.Flag.ToString().ToLower() == "docupload")
                {
                    return RedirectToAction("Logout", "Account");
                }
                else
                {
                    return RedirectToAction("Profile", "User");
                }

                //} Email configuration not required (remove } from here and append it into above)

                //return RedirectToAction("Profile", "User");
            }
            else
            {
                //var designRole = from SystemEnums.PostalAddressType s in Enum.GetValues(typeof(SystemEnums.PostalAddressType))
                //                 select new { ID = s.GetHashCode(), Name = s.ToString().Replace('_', ' ') };
                ViewBag.PostalAddressType = new SelectList(EnumExtensions.GetPostalAddressEnumList(), "ID", "Name");
                userView.lstUserDocument = _userBAL.GetUserUsersDocumentByUserId(userView.UserId);

                userView.RecEmailSignup = new Entity.Email.RecEmailSignup();
                RecEmailConfiguration(userView);

                if (userView.RecEmailSignup.RecConfigurationPassword != null)
                {
                    userView.RecEmailSignup.RecConfigurationPassword = Utils.DecodePassword(userView.RecEmailSignup.RecConfigurationPassword);
                }

                userView.EmailSignup = new Entity.Email.EmailSignup();
                EmailConfiguration(userView);

                if (userView.EmailSignup.ConfigurationPassword != null)
                {
                    userView.EmailSignup.ConfigurationPassword = Utils.DecodePassword(userView.EmailSignup.ConfigurationPassword);
                }

                DateTime fromDate = Convert.ToDateTime(userView.FromDate);
                DateTime toDate = Convert.ToDateTime(userView.ToDate);
                userView.strFromDate = fromDate.ToString("yyyy-MM-dd");
                userView.strToDate = toDate.ToString("yyyy-MM-dd");
                this.ShowMessage(SystemEnums.MessageType.Error, String.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage + " " + v.Exception)), true);

                if (userView.UserTypeID == 10)
                {
                    userView.installerDesignerView = new InstallerDesignerView();
                }

                return this.View("Profile", userView);
            }
        }

        public FormBot.Entity.User GetUserEntityForSASS(DataTable dtUser)
        {
            FormBot.Entity.User userView;
            try
            {
                userView = dtUser.AsEnumerable().Select(m => new FormBot.Entity.User()
                {
                     UserId = m.Field<int>("UserId")
                    ,FirstName = m.Field<string>("FirstName")
                    , LastName = m.Field<string>("LastName")
                    , Phone = m.Field<string>("Phone")
                    , IsPostalAddress = m.Field<bool?>("IsPostalAddress") == null ? false : m.Field<bool>("IsPostalAddress")
                    , UnitTypeID = m.Field<int?>("UnitTypeID") == null ? 0 : m.Field<int>("UnitTypeID")
                    , UnitNumber = m.Field<string>("UnitNumber")
                    , StreetNumber = m.Field<string>("StreetNumber")
                    , StreetName = m.Field<string>("StreetName")
                    , StreetTypeID = m.Field<int?>("StreetTypeID") == null ? 0 : m.Field<int>("StreetTypeID")
                    , Town = m.Field<string>("Town")
                    , State = m.Field<string>("State")
                    , PostCode = m.Field<string>("PostCode")
                    , PostalAddressID = (m.Field<int?>("PostalAddressID") == null ? 0 : m.Field<int>("PostalAddressID"))
                    , PostalDeliveryNumber = m.Field<string>("PostalDeliveryNumber")
                    , ContractPath = m.Field<string>("ContractPath")
                    , UniqueCompanyNumber = m.Field<string>("UniqueCompanyNumber")
                }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Helper.Log.WriteError(ex, "Error for UserId : " + dtUser.Rows[0]["UserId"].ToString() + " " + DateTime.Now.ToString("dd/MM/yyyy") + " " + ex.Message);
                List<FormBot.Entity.User> user = DBClient.DataTableToList<FormBot.Entity.User>(dtUser);
                userView = user.FirstOrDefault();
            }
            return userView;
        }

        public FormBot.Entity.User GetUserEntity(DataTable dtUser)
        {
            FormBot.Entity.User userView = new Entity.User();
            try
            {
                userView = dtUser.AsEnumerable().Select(m => new Entity.User()
                {
                    WholesalerIsPostalAddress = ((m.Field<bool?>("WholesalerIsPostalAddress") == true || m.Field<bool?>("WholesalerIsPostalAddress") == null) ? 2 : 1)
                    ,
                    UserTypeID = m.Field<byte?>("UserTypeID") == null ? 0 : m.Field<byte>("UserTypeID")
                    ,
                    IsWholeSaler = ((m.Field<int?>("IsWholeSaler") == null || m.Field<int?>("IsWholeSaler") == 0) ? false : true)
                    ,
                    SEDesigner = m.Field<byte?>("SEDesigner") == null ? 0 : m.Field<byte>("SEDesigner")
                ,
                    PostalAddressID = (m.Field<int?>("PostalAddressID") == null ? 0 : m.Field<int>("PostalAddressID"))
                ,
                    UserId = m.Field<int>("UserId")
                ,
                    AspNetUserId = m.Field<string>("AspNetUserId")
                ,
                    FirstName = m.Field<string>("FirstName")
                ,
                    LastName = m.Field<string>("LastName")
                ,
                    Phone = m.Field<string>("Phone")
                ,
                    Mobile = m.Field<string>("Mobile")
                ,
                    Email = m.Field<string>("Email")
                ,
                    UserName = m.Field<string>("UserName")
                ,
                    PasswordHash = m.Field<string>("PasswordHash")
                ,
                    CompanyName = m.Field<string>("CompanyName")
                ,
                    CompanyABN = m.Field<string>("CompanyABN")
                ,
                    UnitTypeID = m.Field<int?>("UnitTypeID") == null ? 0 : m.Field<int>("UnitTypeID")
                ,
                    UnitNumber = m.Field<string>("UnitNumber")
                ,
                    StreetNumber = m.Field<string>("StreetNumber")
                ,
                    StreetName = m.Field<string>("StreetName")
                ,
                    StreetTypeID = m.Field<int?>("StreetTypeID") == null ? 0 : m.Field<int>("StreetTypeID")
                ,
                    Town = m.Field<string>("Town")
                ,
                    State = m.Field<string>("State")
                ,
                    PostCode = m.Field<string>("PostCode")
                ,
                    CompanyWebsite = m.Field<string>("CompanyWebsite")
                ,
                    RecUserName = m.Field<string>("RecUserName")
                ,
                    RecPassword = m.Field<string>("RecPassword")
                ,
                    BSB = m.Field<string>("BSB")
                ,
                    AccountNumber = m.Field<string>("AccountNumber")
                ,
                    AccountName = m.Field<string>("AccountName")
                ,
                    ElectricalContractorsLicenseNumber = m.Field<string>("ElectricalContractorsLicenseNumber")
                ,
                    CECAccreditationNumber = m.Field<string>("CECAccreditationNumber")
                ,
                    CECDesignerNumber = m.Field<string>("CECDesignerNumber")
                ,
                    Signature = m.Field<string>("Signature")
                ,
                    IsActive = m.Field<bool?>("IsActive") == null ? false : m.Field<bool>("IsActive")
                ,
                    CreatedDate = m.Field<DateTime?>("CreatedDate") == null ? DateTime.Now : m.Field<DateTime>("CreatedDate")
                ,
                    CreatedBy = m.Field<int?>("CreatedBy") == null ? ProjectSession.LoggedInUserId : m.Field<int>("CreatedBy")
                ,
                    Created = m.Field<string>("Created")
                ,
                    ModifiedDate = m.Field<DateTime?>("ModifiedDate") == null ? DateTime.Now : m.Field<DateTime>("ModifiedDate")
                ,
                    ModifiedBy = m.Field<int?>("ModifiedBy") == null ? ProjectSession.LoggedInUserId : m.Field<int>("ModifiedBy")
                ,
                    IsDeleted = m.Field<bool?>("IsDeleted") == null ? false : m.Field<bool>("IsDeleted")
                ,
                    SolarCompanyId = m.Field<int?>("SolarCompanyId")
                ,
                    ResellerID = m.Field<int?>("ResellerID")
                ,
                    FromDate = m.Field<DateTime?>("FromDate")
                ,
                    ToDate = m.Field<DateTime?>("ToDate")
                ,
                    Theme = m.Field<int?>("Theme") == null ? 0 : m.Field<int>("Theme")
                ,
                    Logo = m.Field<string>("Logo")
                ,
                    isAllowedMasking = m.Field<bool?>("isAllowedMasking") == null ? false : m.Field<bool>("isAllowedMasking")
                ,
                    Note = m.Field<string>("Note")
                ,
                    Status = m.Field<byte?>("Status")
                ,
                    ComplainBy = m.Field<int?>("ComplainBy") == null ? 0 : m.Field<int>("ComplainBy")
                ,
                    RoleID = m.Field<int?>("RoleID") == null ? 0 : m.Field<int>("RoleID")
                ,
                    LoginCompanyName = m.Field<string>("LoginCompanyName")
                ,
                    UnitTypeName = m.Field<string>("UnitTypeName")
                ,
                    StreetTypeName = m.Field<string>("StreetTypeName")
                ,
                    PostalDeliveryNumber = m.Field<string>("PostalDeliveryNumber")
                ,
                    IsPostalAddress = m.Field<bool?>("IsPostalAddress") == null ? false : m.Field<bool>("IsPostalAddress")
                ,
                    Code = m.Field<string>("Code")
                ,
                    PostalDeliveryType = m.Field<string>("PostalDeliveryType")
                ,
                    IsFirstLogin = m.Field<bool?>("IsFirstLogin") == null ? false : m.Field<bool>("IsFirstLogin")
                ,
                    IsSTC = m.Field<bool?>("IsSTC") == null ? false : m.Field<bool>("IsSTC")
                ,
                    IsSCDetailConfirm = m.Field<bool?>("IsSCDetailConfirm") == null ? false : m.Field<bool>("IsSTC")
                ,
                    IsInstaller = m.Field<bool?>("IsInstaller") == null ? false : m.Field<bool>("IsInstaller")
                ,
                    IsSEDetailConfirm = m.Field<bool?>("IsSEDetailConfirm") == null ? false : m.Field<bool>("IsSEDetailConfirm")
                ,
                    IsGSTSetByAdminUser = m.Field<int?>("IsGSTSetByAdminUser") == null ? 0 : m.Field<int>("IsGSTSetByAdminUser")
                ,
                    CustomCompanyName = m.Field<string>("CustomCompanyName")
                ,
                    ApprovedDate = m.Field<DateTime?>("ApprovedDate") == null ? DateTime.Now : m.Field<DateTime>("ApprovedDate")
                ,
                    ApprovedBy = m.Field<string>("ApprovedBy")
                ,
                    ClientNumber = m.Field<string>("ClientNumber")
                ,
                    RAMId = m.Field<int?>("RAMId") == null ? string.Empty : Convert.ToString(m.Field<int>("RAMId"))
                ,
                    RAMName = m.Field<string>("RAMName")
                ,
                    ClientCodePrefix = m.Field<string>("ClientCodePrefix")
                ,
                    GSTText = m.Field<string>("GSTText")
                ,
                    GB_RACode = m.Field<string>("GB_RACode")
                ,
                    GB_SCACode = m.Field<string>("GB_SCACode")
                ,
                    IsPVDUser = m.Field<bool?>("IsPVDUser")
                ,
                    IsSWHUser = m.Field<bool?>("IsSWHUser")
                ,
                    IsAutoRequest = m.Field<bool?>("IsAutoRequest") == null ? false : m.Field<bool>("IsAutoRequest")
                ,
                    UniqueCompanyNumber = m.Field<string>("UniqueCompanyNumber")
                ,
                    WholesalerFirstName = m.Field<string>("WholesalerFirstName")
                ,
                    WholesalerLastName = m.Field<string>("WholesalerLastName")
                ,
                    WholesalerEmail = m.Field<string>("WholesalerEmail")
                ,
                    WholesalerPhone = m.Field<string>("WholesalerPhone")
                ,
                    WholesalerMobile = m.Field<string>("WholesalerMobile")
                ,
                    WholesalerCompanyABN = m.Field<string>("WholesalerCompanyABN")
                ,
                    WholesalerCompanyName = m.Field<string>("WholesalerCompanyName")
                ,
                    WholesalerUnitTypeID = m.Field<int?>("WholesalerUnitTypeID") == null ? 0 : m.Field<int>("WholesalerUnitTypeID")
                ,
                    WholesalerUnitNumber = m.Field<string>("WholesalerUnitNumber")
                ,
                    WholesalerStreetNumber = m.Field<string>("WholesalerStreetNumber")
                ,
                    WholesalerStreetName = m.Field<string>("WholesalerStreetName")
                ,
                    WholesalerStreetTypeID = m.Field<int?>("WholesalerStreetTypeID") == null ? 0 : m.Field<int>("WholesalerStreetTypeID")
                ,
                    WholesalerPostalAddressID = m.Field<int?>("WholesalerPostalAddressID") == null ? 0 : m.Field<int>("WholesalerPostalAddressID")
                ,
                    WholesalerPostalDeliveryNumber = m.Field<string>("WholesalerPostalDeliveryNumber")
                ,
                    WholesalerTown = m.Field<string>("WholesalerTown")
                ,
                    WholesalerState = m.Field<string>("WholesalerState")
                ,
                    WholesalerPostCode = m.Field<string>("WholesalerPostCode")
                ,
                    WholesalerBSB = m.Field<string>("WholesalerBSB")
                ,
                    WholesalerAccountNumber = m.Field<string>("WholesalerAccountNumber")
                ,
                    WholesalerAccountName = m.Field<string>("WholesalerAccountName")
                ,
                    UniqueWholesalerNumber = m.Field<string>("UniqueWholesalerNumber")
                ,
                    WholesalerUnitTypeName = m.Field<string>("WholesalerUnitTypeName")
                ,
                    WholesalerStreetTypeName = m.Field<string>("WholesalerStreetTypeName")
                ,
                    WholesalerCode = m.Field<string>("WholesalerCode")
                ,
                    WholesalerPostalDeliveryType = m.Field<string>("WholesalerPostalDeliveryType")
                ,
                    SCisAllowedSPV = m.Field<bool?>("SCisAllowedSPV") == null ? false : m.Field<bool>("SCisAllowedSPV")
                ,
                    SESelfie = m.Field<string>("SESelfie")
                ,
                    IsVerified = m.Field<int?>("IsVerified")
                ,
                    CERLoginId = m.Field<string>("CERLoginId")
                    
                ,   hdnCERLoginId=m.Field<string>("hdnCERLoginId")
                
                ,
                    CERPassword = m.Field<string>("CERPassword")
                ,
                    RecCompUserName = m.Field<string>("RecCompUserName")
                ,
                    RECName = m.Field<string>("RECName")
                ,
                    RECCompName = m.Field<string>("RECCompName")
                ,
                    SuperAdminCERLoginId = m.Field<string>("SuperAdminCERLoginId")
                   , hdnSuperAdminCERLoginId = m.Field<string>("hdnSuperAdminCERLoginId")
                ,
                    SuperAdminCERPassword = m.Field<string>("SuperAdminCERPassword")
                ,
                    RecSuperAdminUserName = m.Field<string>("RecSuperAdminUserName")
                ,
                    SuperAdminRECName = m.Field<string>("SuperAdminRECName")
                ,
                    SuperAdminRECCompName = m.Field<string>("SuperAdminRECCompName")
                ,
                    Reason = m.Field<string>("Reason")
                ,
                    IsSignatureVerified = m.Field<bool>("IsSignatureVerified")
                ,
                    IsSelfieVerified = m.Field<bool>("IsSelfieVerified")
                ,
                    IsDriverLicVerified = m.Field<bool>("IsDriverLicVerified")
                ,
                    IsOtherDocVerified = m.Field<bool>("IsOtherDocVerified"),
                    EntityName = m.Field<string>("EntityName")
, Invoicer = m.Field<int?>("Invoicer")
                , InvoicerFirstName = m.Field<string>("InvoicerFirstName")
                , InvoicerLastName = m.Field<string>("InvoicerLastName")
                , InvoicerPhone = m.Field<string>("InvoicerPhone")
                , UniqueContactId = m.Field<string>("InvoicerUniqueContactId")
                , InvoicerIsPostalAddress = m.Field<bool?>("InvoicerIsPostalAddress") == null ? false : m.Field<bool>("InvoicerIsPostalAddress")
                , InvoicerUnitTypeID = m.Field<int?>("InvoicerUnitTypeID") == null ? 0 : m.Field<int>("InvoicerUnitTypeID")
                , InvoicerUnitNumber = m.Field<string>("InvoicerUnitNumber")
                , InvoicerStreetNumber = m.Field<string>("InvoicerStreetNumber")
                , InvoicerStreetName = m.Field<string>("InvoicerStreetName")
                , InvoicerStreetTypeID = m.Field<int?>("InvoicerStreetTypeID") == null ? 0 : m.Field<int>("InvoicerStreetTypeID")
                , InvoicerTown = m.Field<string>("InvoicerTown")
                , InvoicerState = m.Field<string>("InvoicerState")
                , InvoicerPostCode = m.Field<string>("InvoicerPostCode")
                , InvoicerPostalAddressID = m.Field<int?>("InvoicerPostalAddressID") == null ? 0 : m.Field<int>("InvoicerPostalAddressID")
                , InvoicerPostalDeliveryNumber = m.Field<string>("InvoicerPostalDeliveryNumber")
                , UsageType = m.Field<int>("UsageType")
                , IsSAASUser = m.Field<bool>("IsSAASUser")
                , ContractPath = m.Field<string>("ContractPath")
                , AccountCode = m.Field<string>("AccountCode")
                }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                //Helper.Helper.Common.WriteErrorLog("Error for UserId : " + dtUser.Rows[0]["UserId"].ToString() + " " + DateTime.Now.ToString("dd/MM/yyyy") +" " + ex.Message);
                Helper.Log.WriteError(ex, "Error for UserId : " + dtUser.Rows[0]["UserId"].ToString() + " " + DateTime.Now.ToString("dd/MM/yyyy") + " " + ex.Message);
                List<FormBot.Entity.User> user = DBClient.DataTableToList<FormBot.Entity.User>(dtUser);
                userView = user.FirstOrDefault();
            }

            return userView;
        }

        /// <summary>
        /// View User
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>View Details</returns>
        [HttpGet]
        [UserAuthorization]
        [GZipOrDeflate]
        public ActionResult ViewDetail(string id, string flg = null)
        {
            int userId = 0;
            if (!string.IsNullOrWhiteSpace(id))
                int.TryParse(QueryString.GetValueFromQueryString(id, "id"), out userId);
            var dsUsers = _userBAL.GetUserById(userId);

            FormBot.Entity.User userView = GetUserEntity(dsUsers.Tables[0]);

            //List<FormBot.Entity.User> user = DBClient.DataTableToList<FormBot.Entity.User>(dsUsers.Tables[0]);
            //FormBot.Entity.User userView = user.FirstOrDefault();
            //userView.solarElectricianView = new SolarElectricianView();
            //userView.solarElectricianView.IsSendRequest = true;
            userView.PreviousFirstName = userView.FirstName;
            userView.PreviousLastName = userView.LastName;
            userView.PreviousEmail = userView.Email;
            userView.PreviousPhone = userView.Phone;
            userView.PreviousMobile = userView.Mobile;
            userView.PreviousUserName = userView.UserName;
            userView.PreviousPassword = userView.Password;
            userView.PreviousCompanyABN = userView.CompanyABN;
            userView.PreviousGB_SCACode = userView.GB_SCACode;
            userView.PreviousCECAccreditationNumber = userView.CECAccreditationNumber;
            userView.PreviousElectricalContractorsLicenseNumber = userView.ElectricalContractorsLicenseNumber;
            userView.PreviousCECDesignerNumber = userView.CECDesignerNumber;
            userView.PreviousSEDesigner = userView.SEDesigner;
            userView.PreviousCompanyName = userView.CompanyName;
            userView.PreviousSignature = userView.Signature;
            userView.PreviousSESelfie = userView.SESelfie;
            userView.PreviousIsPVDUser = userView.IsPVDUser;
            userView.PreviousIsSWHUser = userView.IsSWHUser;
            userView.PreviousIsAutoRequest = userView.IsAutoRequest;
            userView.PreviousCompanyWebsite = userView.CompanyWebsite;
            userView.PreviousStatus = userView.Status;
            userView.PreviousRoleID = userView.RoleID;
            userView.PreviousIsVerified = userView.IsVerified;
            userView.PreviousIsSEDetailConfirm = userView.IsSEDetailConfirm;

           

            userView.EmailSignup = new Entity.Email.EmailSignup();
            //Rec email changes
            userView.RecEmailSignup = new Entity.Email.RecEmailSignup();
            if (userView.Status == null)
            {
                userView.Status = Convert.ToByte(FormBot.Helper.SystemEnums.UserStatus.Pending);
            }

            if (userView.IsPostalAddress == true)
            {
                userView.AddressID = 2;
            }
            else
            {
                userView.AddressID = 1;
            }

            //if (userView.WholesalerIsPostalAddress == 1)
            //{
            //    userView.WholesalerIsPostalAddress = 2;
            //}
            //else
            //{
            //    userView.WholesalerIsPostalAddress = 1;
            //}

            if (userView.IsSTC == true)
            {
                userView.chkSTC = 1;
            }
            else
            {
                userView.chkSTC = 0;
            }

            userView.installerDesignerView = new InstallerDesignerView();
            if (ProjectSession.UserTypeId == 1 || ProjectSession.UserTypeId == 2 || ProjectSession.UserTypeId == 3 || ProjectSession.UserTypeId == 4 || ProjectSession.UserTypeId == 5 || ProjectSession.UserTypeId == 6 || ProjectSession.UserTypeId == 8)
            {
                var statuses = from SystemEnums.SEDesignRole s in Enum.GetValues(typeof(SystemEnums.SEDesignRole))
                               select new { ID = s, Name = s.ToString().Replace('_', ' ') };
                ViewData["SEDesignRole"] = new SelectList(statuses, "ID", "Name");
            }

            var NotesType = from SystemEnums.NotesType n in Enum.GetValues(typeof(SystemEnums.NotesType))
                            select new { Text = (int)n, Value = n.ToString() };
            NotesType = NotesType.Where(x => x.Text == 3).ToList();
            List<SelectListItem> NotestypeDropdown = new SelectList(NotesType, "Text", "Value").ToList();
            NotestypeDropdown.Add(new SelectListItem { Text = "Warning", Value = "5" });
            ViewBag.NotesType = NotestypeDropdown;

            ViewBag.PostalAddressType = new SelectList(EnumExtensions.GetPostalAddressEnumList(), "ID", "Name");
            userView.lstUserDocument = _userBAL.GetUserUsersDocumentByUserId(userId);
            if (userView.IsSTC == true && userView.UserTypeID == 4)
            {
                List<AccreditedInstallers> accreditedInstallersList = _userBAL.CheckAccreditationNumberExistsInAccreditedInstallers(userView.CECAccreditationNumber, userView.ElectricalContractorsLicenseNumber, true);
                userView.isActiveDiv = true;
                ViewDetailComplianceCheck(accreditedInstallersList, userView);
            }
            if (userView.IsPVDUser == true)
            {
                List<AccreditedInstallers> accreditedInstallersList = _userBAL.CheckAccreditationNumberExistsInAccreditedInstallers(userView.CECAccreditationNumber, userView.ElectricalContractorsLicenseNumber, true);
                userView.isActiveDiv = true;
                ViewDetailComplianceCheck(accreditedInstallersList, userView);
            }

            DateTime fromDate = Convert.ToDateTime(userView.FromDate);
            DateTime toDate = Convert.ToDateTime(userView.ToDate);
            userView.strFromDate = fromDate.ToString("yyyy-MM-dd");
            userView.strToDate = toDate.ToString("yyyy-MM-dd");
            string cdate = userView.CreatedDate.Hour >= 12 ? "PM AEST" : "AM AEST";
            userView.DisplayDate = userView.CreatedDate.ToShortDateString() + " " + userView.CreatedDate.Hour + ":" + userView.CreatedDate.Minute + cdate;
            string date = userView.ApprovedDate.Hour >= 12 ? "PM AEST" : "AM AEST";
            userView.ApprovedDisplayDate = userView.ApprovedDate.ToShortDateString() + " " + userView.ApprovedDate.Hour + ":" + userView.ApprovedDate.Minute + date;
            TempData[userView.UserId.ToString()] = userView;
            if (!string.IsNullOrWhiteSpace(flg))
                TempData["DocFlag"] = flg;

 //ViewBag.UsageType = Common.GetUsageTypeList();
            ViewBag.Invoicer = Common.GetInvoicer();
            //Adress
            userView.PreviousAddressID = userView.AddressID;
            userView.PreviousUnitTypeID = userView.UnitTypeID;
            userView.PreviousUnitNumber = userView.UnitNumber;
            userView.PreviousStreetName = userView.StreetName;
            userView.PreviousStreetNumber = userView.StreetNumber;
            userView.PreviousStreetTypeID = userView.StreetTypeID;
            userView.PreviousTown = userView.Town;
            userView.PreviousState = userView.State;
            userView.PreviousPostCode = userView.PostCode;

            userView.PreviousPostalAddressID = userView.PostalAddressID;
            userView.PreviousPostalDeliveryNumber = userView.PostalDeliveryNumber;

            //SCA
            if (userView.UserTypeID == 4)
            {
                userView.PreviousSCisAllowedSPV = userView.SCisAllowedSPV;
                userView.PreviousBSB = userView.BSB;
                userView.PreviousAccountNumber = userView.AccountNumber;
                userView.PreviousAccountName = userView.AccountName;
                userView.PreviousRAMId = userView.RAMId;
                userView.PreviousClientNumber = userView.ClientNumber;
                userView.PreviousUniqueCompanyNumber = userView.UniqueCompanyNumber;
                userView.PreviousIsGSTSetByAdminUser = userView.IsGSTSetByAdminUser;
                userView.PreviousCustomCompanyName = userView.CustomCompanyName;
                userView.PreviousIsSCDetailConfirm = userView.IsSCDetailConfirm;
                userView.PreviousIsInstaller = userView.IsInstaller;
            }
            userView.Note = null;
            return View(userView);
        }

        /// <summary>
        /// Post method for View user
        /// </summary>
        /// <param name="Id">The identifier.</param>
        /// <param name="Status">The status.</param>
        /// <param name="Note">The note.</param>
        /// <returns>InsertStatusNote</returns>
        [HttpPost]
        public ActionResult InsertStatusNote(int Id, byte Status, string Note)
        {
            int complainBy = ProjectSession.LoggedInUserId;
            // _userBAL.InsertStatusNote(Id, Status, Note, complainBy);
            TempData["msg"] = "User status has been saved successfully.";
            return this.Json(new { success = true });
        }

        /// <summary>
        /// Create page for Send Request for SE
        /// </summary>
        /// <param name="SolarCompanyId">SolarCompanyId</param>
        /// <param name="SendBy">SendBy</param>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [UserAuthorization]
        public ActionResult SendRequest(string SolarCompanyId = "", string SendBy = "1")
        {
            //var designRole = from SystemEnums.PostalAddressType s in Enum.GetValues(typeof(SystemEnums.PostalAddressType))
            //                 select new { ID = s.GetHashCode(), Name = s.ToString().Replace('_', ' ') };
            ViewBag.PostalAddressType = new SelectList(EnumExtensions.GetPostalAddressEnumList(), "ID", "Name");
            SolarElectricianView solarElectricianView = new SolarElectricianView();
            solarElectricianView.IsActiveDiv = false;
            solarElectricianView.IsSendRequest = true;
            solarElectricianView.SolarCompanyId = !string.IsNullOrWhiteSpace(SolarCompanyId) ? Convert.ToInt32(SolarCompanyId) : 0;
            solarElectricianView.SendBy = !string.IsNullOrWhiteSpace(SendBy) ? Convert.ToInt32(SendBy) : 0;

            //var usertypes = from SystemEnums.UserType s in Enum.GetValues(typeof(SystemEnums.UserType))
            //                where ((int)s == 7 || (int)s == 10)
            //                select new { ID = (int)s, Name = GetDescription(s) };

            IEnumerable usertypes = from SystemEnums.UserType s in Enum.GetValues(typeof(SystemEnums.UserType))
                                    where (s.GetHashCode() == 7 || s.GetHashCode() == 10)
                                    select new { ID = s.GetHashCode(), Name = GetDescription(s) };

            ViewBag.UserTypeId = new SelectList(usertypes, "ID", "Name");

            return View(solarElectricianView);
        }

        /// <summary>
        /// post method for send request.
        /// </summary>
        /// <param name="solarElectricianView">The solar electrician view.</param>
        /// <returns>SendRequest</returns>
        [HttpPost]
        [UserAuthorization]
        public async Task<ActionResult> SendRequest(FormBot.Entity.SolarElectricianView solarElectricianView, bool IsBulkSendRequestToSE = false, int ElectricianStatusId = 1)
        {
            solarElectricianView.ElectricianStatusId = ElectricianStatusId;
            IEnumerable usertypes = from SystemEnums.UserType s in Enum.GetValues(typeof(SystemEnums.UserType))
                                    where (s.GetHashCode() == 7 || s.GetHashCode() == 10)
                                    select new { ID = s.GetHashCode(), Name = GetDescription(s) };
            ViewBag.UserTypeId = new SelectList(usertypes, "ID", "Name");
            ModelState.Remove("CECAccreditationNumber");
            ModelState.Remove("SWHLicenseNumber");
            //if (string.IsNullOrEmpty(solarElectricianView.CECAccreditationNumber) && string.IsNullOrEmpty(solarElectricianView.SWHLicenseNumber))
            //{
            //    this.ShowMessage(SystemEnums.MessageType.Error, GetErrorMessageForSolarElectrician(5), true);
            //    return this.View("SendRequest", solarElectricianView);
            //}
            if (solarElectricianView.SolarCompanyId == 0)
            {
                this.ShowMessage(SystemEnums.MessageType.Error, "Please select any Solar company first.", true);
                return RedirectToAction("RequestedSE", "User");
            }
            if (!string.IsNullOrWhiteSpace(solarElectricianView.CECAccreditationNumber))
            {
                solarElectricianView.CECAccreditationNumber = solarElectricianView.CECAccreditationNumber.Trim();
            }
            if (!solarElectricianView.IsSendRequest)
            {
                if (solarElectricianView.UnitNumber != null && solarElectricianView.UnitTypeID != 0)
                {
                    ModelState.Remove("StreetNumber");
                }
                if (solarElectricianView.StreetNumber != null)
                {
                    ModelState.Remove("UnitNumber");
                    ModelState.Remove("UnitTypeID");
                }
                if (solarElectricianView.AddressID == 2)
                {
                    ModelState.Remove("UnitNumber");
                    ModelState.Remove("UnitTypeID");
                    ModelState.Remove("StreetNumber");
                    ModelState.Remove("StreetName");
                    ModelState.Remove("StreetTypeID");
                }
                if (solarElectricianView.AddressID == 1)
                {
                    ModelState.Remove("PostalAddressID");
                    ModelState.Remove("PostalDeliveryNumber");
                }
            }
            if (ModelState.IsValid)
            {
                ////Request is already sent or not
                //if (!_userBAL.SolarElectricianRequestIsExists(solarElectricianView.SolarCompanyId, solarElectricianView.SWHLicenseNumber))
                //{
                if (solarElectricianView.UserTypeID == 7)
                {
                    ViewBag.PostalAddressType = new SelectList(EnumExtensions.GetPostalAddressEnumList(), "ID", "Name");

                    //User is exist or not
                    var userExists = _userBAL.CheckAccreditationNumber(solarElectricianView.CECAccreditationNumber, solarElectricianView.SWHLicenseNumber, solarElectricianView.Email, false, false, true);
                    if (userExists.UserId > 0)
                    {
                        //Request is already sent or not
                        if (!_userBAL.SolarElectricianRequestIsExists(solarElectricianView.SolarCompanyId, userExists.UserId))
                        {
                            solarElectricianView.UserId = userExists.UserId;
                            solarElectricianView.CreatedBy = ProjectSession.LoggedInUserId;
                            solarElectricianView.IsDeleted = false;
                            if (!solarElectricianView.IsSendRequest)
                            {
                                if (solarElectricianView.AddressID == 2)
                                {
                                    solarElectricianView.IsPostalAddress = true;
                                }
                                else
                                {
                                    solarElectricianView.IsPostalAddress = false;
                                }
                            }

                            List<AccreditedInstallers> accreditedInstallersList = new List<AccreditedInstallers>();
                            if (!string.IsNullOrWhiteSpace(solarElectricianView.CECAccreditationNumber))
                            {
                                //Get records from master file based on accreditation number
                                accreditedInstallersList = _userBAL.CheckAccreditationNumberExistsInAccreditedInstallers(solarElectricianView.CECAccreditationNumber, null, true);
                            }

                            if (!solarElectricianView.IsSendRequest && solarElectricianView.PostalAddressID > 0)
                            {
                                List<PostalAddressView> postalAddressView = _userBAL.GetPostalAddressDetail(solarElectricianView.PostalAddressID);
                                solarElectricianView.Code = postalAddressView[0].Code;
                            }

                            bool IsCreateUser = false;
                            if (!string.IsNullOrWhiteSpace(solarElectricianView.CECAccreditationNumber))
                            {
                                if (accreditedInstallersList.Count > 0)
                                {
                                    ViewBag.FirstName = ViewBag.CECAccreditationNumber = ViewBag.SEDesignRole = ViewBag.Email = ViewBag.LastName = null;

                                    //Compare FirstName, LastName, Accreditation Number, License Number, Solar Electrician role, With master file Record
                                    SendRequestComplianceCheck(accreditedInstallersList, solarElectricianView, false);

                                    if (ViewBag.FirstName == null && ViewBag.CECAccreditationNumber == null && ViewBag.SEDesignRole == null && ViewBag.Email == null && ViewBag.LastName == null)
                                    {
                                        IsCreateUser = true;
                                    }
                                    else if (ProjectSession.UserTypeId == 1 && IsBulkSendRequestToSE && ViewBag.CECAccreditationNumber == null)
                                    {
                                        IsCreateUser = true;
                                    }
                                }
                                else
                                {
                                    //Accreditation or License Number is not exist in Master file
                                    if (IsBulkSendRequestToSE)
                                    {
                                        TempData["IsBulkSendRequestToSE"] = "CECAccreditationNumber:" + solarElectricianView.CECAccreditationNumber + " Name:" + solarElectricianView.FirstName + " " + solarElectricianView.LastName + " Error:Solar Electrician with given accreditation number doesn't exist.\n";
                                        return Json("");
                                    }
                                    else
                                    {
                                        this.ShowMessage(SystemEnums.MessageType.Error, GetErrorMessageForSolarElectrician(4), true);
                                        return this.View("SendRequest", solarElectricianView);
                                    }
                                }
                            }
                            else
                            {
                                // for SWH user
                                IsCreateUser = true;
                            }

                            if (IsCreateUser)
                            {
                                //Entry in Solar Electrician table and return user id which UserType is electrician
                                string emailMsg = SendSERequest(solarElectricianView);
                                this.ShowMessage(SystemEnums.MessageType.Success, "User send request saved successfully. " + emailMsg, true);
                                return RedirectToAction("RequestedSE", "User");

                            }
                            if (IsBulkSendRequestToSE)
                            {
                                TempData["IsBulkSendRequestToSE"] = "CECAccreditationNumber:" + solarElectricianView.CECAccreditationNumber + " Name:" + solarElectricianView.FirstName + " " + solarElectricianView.LastName + " Error:Inserted AccreditationNumber doesn't match with AccreditedInstallersList.\n";
                                return Json("");
                            }
                            else
                            {
                                return this.View("SendRequest", solarElectricianView);
                            }
                            ////////
                        }
                        else
                        {
                            //Request is already sent.
                            this.ShowMessage(SystemEnums.MessageType.Error, GetErrorMessageForSolarElectrician(6), true);
                            return this.View("SendRequest", solarElectricianView);
                        }
                    }
                    else
                    {
                        //Accreditation or License Number is not exist in System
                        if (IsBulkSendRequestToSE)
                        {
                            TempData["IsBulkSendRequestToSE"] = "CECAccreditationNumber:" + solarElectricianView.CECAccreditationNumber + " Name:" + solarElectricianView.FirstName + " " + solarElectricianView.LastName + " Error:Solar Electrician with given accreditation number doesn't exist.\n";
                            return Json("");
                        }
                        else
                        {
                            this.ShowMessage(SystemEnums.MessageType.Error, GetErrorMessageForSolarElectrician(0, userExists), true);
                            return this.View("SendRequest", solarElectricianView);
                        }
                    }
                }
                //}
                //else
                //{
                //    //Request is already sent.
                //    this.ShowMessage(SystemEnums.MessageType.Error, GetErrorMessageForSolarElectrician(6), true);
                //    return this.View("SendRequest", solarElectricianView);
                //}
            }
            else
            {
                if (IsBulkSendRequestToSE)
                {
                    TempData["IsBulkSendRequestToSE"] = String.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage + " " + v.Exception));
                    return Json("");
                }
                else
                {
                    ViewBag.PostalAddressType = new SelectList(EnumExtensions.GetPostalAddressEnumList(), "ID", "Name");
                    this.ShowMessage(SystemEnums.MessageType.Error, String.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage + " " + v.Exception)), true);
                    return this.View("SendRequest", solarElectricianView);
                }
            }
            return this.View("SendRequest", solarElectricianView);
        }

        [HttpGet]
        [UserAuthorization]
        public ActionResult BulkSendRequest()
        {
            FormBot.Entity.BulkSendRequest bulkSendRequest = new FormBot.Entity.BulkSendRequest();

            var statuses = from SystemEnums.ElectricianStatus s in Enum.GetValues(typeof(SystemEnums.ElectricianStatus))
                           select new { ID = s.GetHashCode(), Name = s.ToString().Replace('_', ' ') };
            ViewData["ElectricianStatusId"] = new SelectList(statuses, "ID", "Name", TempData["SERequestPending"]);

            bulkSendRequest.UserTypeID = ProjectSession.UserTypeId;
            if (bulkSendRequest.UserTypeID == 4)
                bulkSendRequest.SolarCompanyId = ProjectSession.SolarCompanyId;
            //else if (bulkSendRequest.UserTypeID == 2)
            //    bulkSendRequest.ResellerID = ProjectSession.ResellerId;
            //else if (bulkSendRequest.UserTypeID == 5)
            //    bulkSendRequest.ResellerID = ProjectSession.LoggedInUserId;

            return View("BulkSendRequest", bulkSendRequest);
        }

        [HttpPost]
        [UserAuthorization]
        public async Task<ActionResult> BulkSendRequest(BulkSendRequest model, HttpPostedFileBase fileUpload)
        {
            if (model.UserTypeID == 2 || model.UserTypeID == 4 || model.UserTypeID == 5)
            {
                ModelState.Remove("ResellerID");
            }

            if (model.UserTypeID == 4)
            {
                ModelState.Remove("SolarCompanyId");
            }

            if (ModelState.IsValid)
            {
                if (fileUpload != null && fileUpload.ContentLength > 0)
                {
                    string filePath = string.Empty;
                    filePath = Path.Combine(ProjectSession.ProofDocumentsURL + "\\CERFiles\\" + fileUpload.FileName);
                    fileUpload.SaveAs(filePath);

                    List<string> listErrors = new List<string>();
                    List<SolarElectricianView> listSolarElectricianView = new List<SolarElectricianView>();

                    FileStream stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read);
                    IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                    if (!excelReader.IsValid)
                    {
                        stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read);
                        excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    }

                    excelReader.IsFirstRowAsColumnNames = true;
                    DataSet result = excelReader.AsDataSet();

                    if (result != null && result.Tables.Count > 0)
                    {
                        foreach (DataRow excelRow in result.Tables[0].Rows)
                        {
                            SolarElectricianView solarElectricianView = new SolarElectricianView();
                            solarElectricianView.IsActiveDiv = false;
                            solarElectricianView.IsSendRequest = true;
                            solarElectricianView.SolarCompanyId = model.SolarCompanyId;
                            solarElectricianView.SendBy = 1;

                            solarElectricianView.CECAccreditationNumber = Convert.ToString(excelRow[0]).Trim();
                            solarElectricianView.FirstName = Convert.ToString(excelRow[1]).Trim();
                            solarElectricianView.LastName = Convert.ToString(excelRow[2]).Trim();
                            bool isInstaller = Convert.ToBoolean(excelRow[3]);
                            bool isDesigner = Convert.ToBoolean(excelRow[4]);
                            if (isInstaller && isDesigner)
                                solarElectricianView.SEDesignRoleId = 3;
                            else if (isInstaller)
                                solarElectricianView.SEDesignRoleId = 1;
                            else if (isDesigner)
                                solarElectricianView.SEDesignRoleId = 2;
                            solarElectricianView.UserTypeID = 7;
                            listSolarElectricianView.Add(solarElectricianView);
                        }
                    }

                    if (listSolarElectricianView.Count > 0)
                    {
                        foreach (SolarElectricianView solarElectricianView in listSolarElectricianView)
                        {
                            TempData["IsBulkSendRequestToSE"] = "";
                            await SendRequest(solarElectricianView, true, model.ElectricianStatusId);
                            if (!string.IsNullOrWhiteSpace(Convert.ToString(TempData["IsBulkSendRequestToSE"])))
                            {
                                listErrors.Add(Convert.ToString(TempData["IsBulkSendRequestToSE"]));
                            }
                        }
                    }

                    if (listErrors.Count() > 0)
                    {
                        this.ShowMessage(SystemEnums.MessageType.Error, String.Join("<br/>", listErrors.Select(X => X)), true);
                    }
                    else
                    {
                        this.ShowMessage(SystemEnums.MessageType.Success, "Success", true);
                    }
                }
                else
                {
                    this.ShowMessage(SystemEnums.MessageType.Error, "Please upload file.", true);
                }
            }

            var statuses = from SystemEnums.ElectricianStatus s in Enum.GetValues(typeof(SystemEnums.ElectricianStatus))
                           select new { ID = s.GetHashCode(), Name = s.ToString().Replace('_', ' ') };
            ViewData["ElectricianStatusId"] = new SelectList(statuses, "ID", "Name", TempData["SERequestPending"]);
            return View(model);
        }

        /// <summary>
        /// Forms the bot sign up.
        /// </summary>
        /// <param name="userView">The user view.</param>
        /// <returns>FormBotSignUp</returns>
        [HttpPost]
        //public async Task<ActionResult> FormBotSignUp(FormBot.Entity.User userView)
        //public JsonResult FormBotSignUp(FormBot.Entity.User userView)
        public async Task<JsonResult> FormBotSignUp(FormBot.Entity.User userView)
        {
            if (!string.IsNullOrWhiteSpace(userView.CECAccreditationNumber))
            {
                userView.CECAccreditationNumber = userView.CECAccreditationNumber.Trim();
            }
            string guid = userView.Guid;
            string sourceDirectory = ProjectSession.ProofDocumentsURL + "\\" + "UserDocuments" + "\\" + guid;
            RequiredValidationField(userView);
            RemoveWholeSalerValidation();
            RemoveSAASUserValidation();
            if (ModelState.IsValid)
            {
                //var designRole = from SystemEnums.PostalAddressType s in Enum.GetValues(typeof(SystemEnums.PostalAddressType))
                //                 select new { ID = s.GetHashCode(), Name = s.ToString().Replace('_', ' ') };
                ViewBag.PostalAddressType = new SelectList(EnumExtensions.GetPostalAddressEnumList(), "ID", "Name");
                if (userView.UserId > 0)
                {
                    UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                    UserManager.UserValidator = new UserValidator<ApplicationUser>(UserManager) { AllowOnlyAlphanumericUserNames = false };

                    var userDetails = UserManager.FindById(userView.AspNetUserId);

                    if (userDetails != null)
                    {
                        userDetails.Email = userView.Email;
                        userDetails.UserName = userView.UserName.Replace(" ", "");
                        PasswordHasher hasher = new PasswordHasher();
                        if (userView.Password != null)
                        {
                            userDetails.PasswordHash = hasher.HashPassword(userView.Password);
                        }
                    }

                    await UserManager.UpdateAsync(userDetails);
                    userView.ModifiedBy = ProjectSession.LoggedInUserId;
                    userView.ModifiedDate = DateTime.Today;
                    if (userView.strFromDate != null)
                    {
                        userView.FromDate = Convert.ToDateTime(userView.strFromDate);
                    }

                    if (userView.strToDate != null)
                    {
                        userView.ToDate = Convert.ToDateTime(userView.strToDate);
                    }

                    if (userView.AddressID == 2)
                    {
                        userView.IsPostalAddress = true;
                    }
                    else
                    {
                        userView.IsPostalAddress = false;
                    }

                    if (userView.WholesalerIsPostalAddress == 2)
                    {
                        userView.WholesalerIsPostalAddress = 1;
                    }
                    else
                    {
                        userView.WholesalerIsPostalAddress = 0;
                    }


                    if (userView.IsSTC == false && userView.UserTypeID == 4)
                    {
                        userView.CECAccreditationNumber = null;
                        userView.ElectricalContractorsLicenseNumber = null;
                        userView.CECDesignerNumber = null;
                    }

                    if (userView.CompanyABN != null)
                    {
                        userView.CompanyABN = userView.CompanyABN.Replace(" ", "");
                    }

                    int userID = _userBAL.Create(userView);
                   
                    _userBAL.InsertStatusNote(userView.UserId, 1, userView.Note, userView.ComplainBy, userView.IsSCDetailConfirm, userView.IsInstaller, userView.IsSEDetailConfirm);
                    
                    if (userView.FileNamesCreate != null)
                    {
                        foreach (var files in userView.FileNamesCreate)
                        {
                            var lstfile = JsonConvert.DeserializeObject<List<Rootobject>>(files);
                            foreach (var file in lstfile)
                            {
                                userView.FileName = file.Name;
                                _userBAL.InsertUserDocument(userView.UserId, userView.FileName);
                            }
                        }
                    }

                    if (userView.ProofFileNamesCreate != null)
                    {
                        foreach (var files in userView.ProofFileNamesCreate)
                        {
                            userView.FileName = files.FileName;
                            if (userView.UserTypeID == 7 || userView.UserTypeID == 10)
                            {
                                _userBAL.InsertUserDocument(userView.UserId, userView.FileName, files.ProofDocumentType, files.DocLoc);
                            }
                            else
                            {
                                _userBAL.InsertUserDocument(userView.UserId, userView.FileName, files.ProofDocumentType);
                            }
                        }
                    }

                    this.ShowMessage(SystemEnums.MessageType.Success, "User has been saved successfully.", true);
                    if (userView.UserTypeID == 7 && (TempData["Flag"] != null && TempData["Flag"].ToString() == "DocUpload"))
                    {
                        SendMailForDocumentsFSA(userView);
                        //TempData["Flag"] = null;
                    }
                    //return RedirectToAction("Login", "Account");
                    return Json("true", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //if (userView.UserJobType == 1)
                    //    userView.UserTypeID = 7;
                    //else if (userView.UserJobType == 2)
                    //    userView.UserTypeID = 10;
                    //int isUserExists = 0;
                    if (userView.UserTypeID == 7)
                    {
                        string sErrorMessage = string.Empty;
                        var isUserExists = _userBAL.CheckAccreditationNumber(userView.CECAccreditationNumber, userView.ElectricalContractorsLicenseNumber, userView.Email, userView.IsPVDUser == null ? false : userView.IsPVDUser.Value, userView.IsSWHUser == null ? false : userView.IsSWHUser.Value);
                        if (isUserExists.UserId > 0)
                        {
                            if (userView.IsPVDUser == true)
                            {
                                sErrorMessage = GetErrorMessageForSolarElectrician(1, isUserExists);
                            }
                            if (userView.IsSWHUser == true)
                            {
                                if (!string.IsNullOrWhiteSpace(sErrorMessage))
                                    sErrorMessage = GetErrorMessageForSolarElectrician(2, isUserExists);
                                else
                                    sErrorMessage = GetErrorMessageForSolarElectrician(3, isUserExists);
                            }
                            return Json(sErrorMessage, JsonRequestBehavior.AllowGet);
                        }
                    }

                    //if (userView.UserTypeID == 10)
                    //{
                    //    isUserExists = _userBAL.CheckAccreditationNumber(userView.ElectricalContractorsLicenseNumber, userView.UserTypeID);
                    //    if (!isUserExists.Equals(0))
                    //        return Json("SWH User with given license number already exists.", JsonRequestBehavior.AllowGet);
                    //}

                    UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                    UserManager.UserValidator = new UserValidator<ApplicationUser>(UserManager) { AllowOnlyAlphanumericUserNames = false };
                    var user = new ApplicationUser() { Email = userView.Email, UserName = userView.UserName.Replace(" ", ""), PasswordHash = userView.Password };
                    IdentityResult result = await UserManager.CreateAsync(user, userView.Password);
                    if (result.Succeeded)
                    {
                        userView.AspNetUserId = user.Id;
                        userView.CreatedDate = DateTime.Today;
                        userView.ModifiedDate = DateTime.Today;



                        userView.IsActive = true;
                        userView.Logo = "Images/logo.png";
                        userView.Theme = 1;
                        userView.IsFirstLogin = true;
                        if (userView.strFromDate != null)
                        {
                            userView.FromDate = Convert.ToDateTime(userView.strFromDate);
                        }

                        if (userView.strToDate != null)
                        {
                            userView.ToDate = Convert.ToDateTime(userView.strToDate);
                        }

                        if (userView.AddressID == 2)
                        {
                            userView.IsPostalAddress = true;
                        }
                        else
                        {
                            userView.IsPostalAddress = false;
                        }

                        if (userView.WholesalerIsPostalAddress == 2)
                        {
                            userView.WholesalerIsPostalAddress = 1;
                        }
                        else
                        {
                            userView.WholesalerIsPostalAddress = 0;
                        }


                        if (userView.CompanyABN != null)
                        {
                            userView.CompanyABN = userView.CompanyABN.Replace(" ", "");
                        }

                        int userID = _userBAL.FormBotSignUp(userView);
                        string destinationDirectory = ProjectSession.ProofDocumentsURL + "\\" + "UserDocuments" + "\\" + userID;


                        if (userView.FileNamesCreate != null)
                        {
                            foreach (var files in userView.FileNamesCreate)
                            {
                                var lstfile = JsonConvert.DeserializeObject<List<Rootobject>>(files);
                                foreach (var file in lstfile)
                                {
                                    userView.FileName = file.Name;
                                    _userBAL.InsertUserDocument(userID, userView.FileName);
                                }
                            }
                        }

                        if (userView.UserTypeID == 7 || userView.UserTypeID == 10)
                        {
                            if (userView.ProofFileNamesCreate != null)
                            {
                                foreach (var files in userView.ProofFileNamesCreate)
                                {
                                    //userView.FileName = files;
                                    if (files.ProofDocumentType > 0)
                                    {
                                        try
                                        {
                                            _userBAL.InsertUserDocument(userID, files.FileName, files.ProofDocumentType,files.DocLoc);
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine(ex.Message);
                                        }
                                    }
                                }
                            }
                        }

                        if (userView.FileNamesCreate != null || userView.Signature != null)
                        {
                            try
                            {
                                Directory.Move(sourceDirectory, destinationDirectory);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                        }

                        this.SendEmailOnSignUp(userID, userView.Password);
                        this.ShowMessage(SystemEnums.MessageType.Success, "User has been signup successfully.", true);
                        //return RedirectToAction("Login", "Account");
                        return Json("true", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        string errorValue = string.Empty;
                        foreach (var item in result.Errors)
                        {
                            if (!string.IsNullOrWhiteSpace(errorValue))
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
            }
            else
            {
                ViewBag.PostalAddressType = new SelectList(EnumExtensions.GetPostalAddressEnumList(), "ID", "Name");
                return this.Json(String.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage + " " + v.Exception)));
            }
        }

        /// <summary>
        /// Post method for create user.
        /// </summary>
        /// <param name="userView">The user view.</param>
        /// <returns>Reseller SignUp</returns>
        [HttpPost]
        public async Task<JsonResult> ResellerSignUp(FormBot.Entity.User userView)
        {
            string guid = userView.Guid;
            string sourceDirectory = ProjectSession.ProofDocumentsURL + "\\" + "UserDocuments" + "\\" + guid;
            RequiredValidationField(userView);
            ModelState.Remove("CustomCompanyName");
            RemoveWholeSalerValidation();
            RemoveSAASUserValidation();
            if (ModelState.IsValid)
            {
                ViewBag.PostalAddressType = new SelectList(EnumExtensions.GetPostalAddressEnumList(), "ID", "Name");
                if (userView.UserId > 0)
                {
                    UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

                    UserManager.UserValidator = new UserValidator<ApplicationUser>(UserManager) { AllowOnlyAlphanumericUserNames = false };

                    var userDetails = UserManager.FindById(userView.AspNetUserId);
                    if (userDetails != null)
                    {
                        userDetails.Email = userView.Email;
                        userDetails.UserName = userView.UserName.Replace(" ", "");
                        PasswordHasher hasher = new PasswordHasher();
                        if (userView.Password != null)
                        {
                            userDetails.PasswordHash = hasher.HashPassword(userView.Password);
                        }
                    }

                    await UserManager.UpdateAsync(userDetails);
                    userView.ModifiedBy = ProjectSession.LoggedInUserId;
                    userView.ModifiedDate = DateTime.Today;
                    if (userView.strFromDate != null)
                    {
                        userView.FromDate = Convert.ToDateTime(userView.strFromDate);
                    }

                    if (userView.strToDate != null)
                    {
                        userView.ToDate = Convert.ToDateTime(userView.strToDate);
                    }

                    if (userView.AddressID == 2)
                    {
                        userView.IsPostalAddress = true;
                    }
                    else
                    {
                        userView.IsPostalAddress = false;
                    }

                    if (userView.WholesalerIsPostalAddress == 1)
                    {
                        userView.WholesalerIsPostalAddress = 2;
                    }
                    else
                    {
                        userView.WholesalerIsPostalAddress = 1;
                    }

                    if (userView.IsSTC == false && userView.UserTypeID == 4)
                    {
                        userView.CECAccreditationNumber = null;
                        userView.ElectricalContractorsLicenseNumber = null;
                        userView.CECDesignerNumber = null;
                    }

                    if (userView.CompanyABN != null)
                    {
                        userView.CompanyABN = userView.CompanyABN.Replace(" ", "");
                    }

                    int chkSTC = _userBAL.GetISSTCforSCA(userView.UserId);
                    _userBAL.InsertStatusNote(userView.UserId, 1, userView.Note, userView.ComplainBy, userView.IsSCDetailConfirm, userView.IsInstaller, userView.IsSEDetailConfirm);
                    int userID = _userBAL.Create(userView);
                   
                    if (userView.UserTypeID == 4)
                    {
                        if (userView.ProofFileNamesCreate != null)
                        {
                            foreach (var files in userView.ProofFileNamesCreate)
                            {
                                //userView.FileName = files;
                                if (files.ProofDocumentType > 0)
                                {
                                    _userBAL.InsertUserDocument(userView.UserId, files.FileName, files.ProofDocumentType);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (userView.FileNamesCreate != null)
                        {
                            foreach (var files in userView.FileNamesCreate)
                            {
                                var lstfile = JsonConvert.DeserializeObject<List<Rootobject>>(files);
                                foreach (var file in lstfile)
                                {
                                    userView.FileName = file.Name;
                                    _userBAL.InsertUserDocument(userView.UserId, userView.FileName);
                                }
                            }
                        }
                    }

                    //if (userView.FileNamesCreate != null)
                    //{
                    //    foreach (var files in userView.FileNamesCreate)
                    //    {
                    //        var lstfile = JsonConvert.DeserializeObject<List<Rootobject>>(files);
                    //        foreach (var file in lstfile)
                    //        {
                    //            userView.FileName = file.Name;
                    //            _userBAL.InsertUserDocument(userView.UserId, userView.FileName);
                    //        }
                    //    }
                    //}


                    // Login SCA wants to join SE
                    if (userView.IsSTC == true && chkSTC.Equals(0))
                    {
                        int seID = _userBAL.InsertSCAasSE(userView);
                        userView.lstUserDocument = _userBAL.GetUserUsersDocumentByUserId(userView.UserId);
                        for (int i = 0; i < userView.lstUserDocument.Count; i++)
                        {
                            _userBAL.InsertUserDocument(seID, userView.lstUserDocument[i].DocumentPath);
                        }

                        string destinationDirectory = ProjectSession.ProofDocumentsURL + "\\" + "UserDocuments" + "\\" + userView.UserId;

                        // SCA want join as SE userID
                        string seDestinationDirectory = ProjectSession.ProofDocumentsURL + "\\" + "UserDocuments" + "\\" + seID;

                        if (userView.FileNamesCreate != null || userView.Signature != null)
                        {
                            try
                            {
                                if (userView.IsSTC == true)
                                {
                                    // SCA want join as SE
                                    Directory.CreateDirectory(seDestinationDirectory);
                                    DirectoryInfo dir = new DirectoryInfo(destinationDirectory);
                                    FileInfo[] files = dir.GetFiles();
                                    foreach (FileInfo file in files)
                                    {
                                        // Create the path to the new copy of the file.
                                        string temppath = Path.Combine(seDestinationDirectory, file.Name);

                                        // Copy the file.
                                        file.CopyTo(temppath, false);
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                        }
                    }

                    this.ShowMessage(SystemEnums.MessageType.Success, "User has been saved successfully.", true);
                    //return RedirectPermanent(userView.ResellerUrl);
                    return Json("true", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(userView.Email) && !string.IsNullOrWhiteSpace(userView.UserName) && !string.IsNullOrWhiteSpace(userView.Password))
                    {
                        UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                        UserManager.UserValidator = new UserValidator<ApplicationUser>(UserManager) { AllowOnlyAlphanumericUserNames = false };

                        var user = new ApplicationUser() { Email = userView.Email, UserName = userView.UserName.Replace(" ", ""), PasswordHash = userView.Password };
                        Helper.Log.WriteLog("intialize user: " +userView.UserName);
                        IdentityResult result = await UserManager.CreateAsync(user, userView.Password);
                        if (userView.IsSTC == true)
                        {
                            Helper.Log.WriteLog("user isstc true: " + userView.UserName);
                            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                            object isUserExists;
                            string uName = userView.UserName + "_SE";
                            userView.UserName = userView.UserName + "_SE";
                            isUserExists = _userBAL.CheckEmailExists(userView.UserName);
                            if (!isUserExists.Equals(0))
                            {
                                int i = 0;
                                while (true)
                                {
                                    userView.UserName = uName + i;
                                    isUserExists = _userBAL.CheckEmailExists(userView.UserName);
                                    if (isUserExists.Equals(0))
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        i++;
                                    }
                                }
                            }

                            var userSE = new ApplicationUser() { Email = userView.Email, UserName = userView.UserName.Replace(" ", ""), PasswordHash = userView.Password };
                            IdentityResult resultSE = await UserManager.CreateAsync(userSE, userView.Password);
                            userView.AspNetUserIdSE = userSE.Id;
                        }
                        Helper.Log.WriteLog("after successfully insert in aspnetusers..ASPId: "+user.Id+"--" + result.Succeeded);
                        if (result.Succeeded)
                        {
                            Helper.Log.WriteLog("inside result.succeeded..ASPId: " + user.Id + "--");
                            userView.AspNetUserId = user.Id;
                            userView.CreatedDate = DateTime.Today;
                            userView.ModifiedDate = DateTime.Today;
                            userView.UserTypeID = 4;
                            userView.Logo = "Images/logo.png";
                            userView.Theme = 1;
                            userView.IsFirstLogin = true;
                            userView.IsActive = true;
                            if (userView.strFromDate != null)
                            {
                                userView.FromDate = Convert.ToDateTime(userView.strFromDate);
                            }

                            if (userView.strToDate != null)
                            {
                                userView.ToDate = Convert.ToDateTime(userView.strToDate);
                            }

                            if (userView.AddressID == 2)
                            {
                                userView.IsPostalAddress = true;
                            }
                            else
                            {
                                userView.IsPostalAddress = false;
                            }

                            if (userView.WholesalerIsPostalAddress == 2)
                            {
                                userView.WholesalerIsPostalAddress = 1;
                            }
                            else
                            {
                                userView.WholesalerIsPostalAddress = 0;
                            }

                            if (userView.CompanyABN != null)
                            {
                                userView.CompanyABN = userView.CompanyABN.Replace(" ", "");
                            }
                            Helper.Log.WriteLog("above formbotSIgnup : " + user.Id);
                            int userID = _userBAL.FormBotSignUp(userView);
                            Helper.Log.WriteLog("end formbotsignup..ASPId: " + user.Id + "userID--" + userID);
                            string destinationDirectory = ProjectSession.ProofDocumentsURL + "\\" + "UserDocuments" + "\\" + userID;

                            // SCA want join as SE userID
                            int uID = userID + 1;
                            string seDestinationDirectory = ProjectSession.ProofDocumentsURL + "\\" + "UserDocuments" + "\\" + uID;

                            if (userView.UserTypeID == 4)
                            {
                                if (userView.ProofFileNamesCreate != null)
                                {
                                    foreach (var files in userView.ProofFileNamesCreate)
                                    {
                                        //userView.FileName = files;
                                        if (files.ProofDocumentType > 0)
                                        {
                                            _userBAL.InsertUserDocument(userID, files.FileName, files.ProofDocumentType);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (userView.FileNamesCreate != null)
                                {
                                    foreach (var files in userView.FileNamesCreate)
                                    {
                                        var lstfile = JsonConvert.DeserializeObject<List<Rootobject>>(files);
                                        foreach (var file in lstfile)
                                        {
                                            userView.FileName = file.Name;
                                            _userBAL.InsertUserDocument(userID, userView.FileName);
                                        }
                                    }
                                }
                            }


                            //if (userView.FileNamesCreate != null)
                            //{
                            //    foreach (var files in userView.FileNamesCreate)
                            //    {
                            //        var lstfile = JsonConvert.DeserializeObject<List<Rootobject>>(files);
                            //        foreach (var file in lstfile)
                            //        {
                            //            userView.FileName = file.Name;
                            //            _userBAL.InsertUserDocument(userID, userView.FileName);
                            //        }
                            //        // Insert SCA document when join as a SE
                            //        if (userView.IsSTC == true && userView.UserTypeID == 4)
                            //        {
                            //            _userBAL.InsertUserDocument(uID, files);
                            //        }
                            //    }
                            //}

                            if (userView.FileNamesCreate != null || userView.ProofFileNamesCreate != null || userView.Signature != null)
                            {
                                try
                                {
                                    Directory.Move(sourceDirectory, destinationDirectory);

                                    if (userView.IsSTC == true && userView.UserTypeID == 4)
                                    {
                                        // SCA want join as SE
                                        Directory.CreateDirectory(seDestinationDirectory);
                                        DirectoryInfo dir = new DirectoryInfo(destinationDirectory);
                                        FileInfo[] files = dir.GetFiles();
                                        foreach (FileInfo file in files)
                                        {
                                            // Create the path to the new copy of the file.
                                            string temppath = Path.Combine(seDestinationDirectory, file.Name);

                                            // Copy the file.
                                            file.CopyTo(temppath, false);
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                            }

                            this.SendEmailOnSignUp(userID, userView.Password);
                            if (userView.IsSTC == true && userView.UserTypeID == 4)
                            {
                                this.SendEmailOnSignUp(uID, userView.Password);
                            }

                            this.ShowMessage(SystemEnums.MessageType.Success, "User has been signup successfully.", true);
                            //return RedirectPermanent(userView.ResellerUrl);
                            return Json("true", JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            List<string> errotList = new List<string>();
                            errotList = result.Errors.ToList();
                            string errors = string.Empty;
                            if (errotList.Count > 0)
                            {
                                for (int error = 0; error < errotList.Count; error++)
                                {
                                    errors = !string.IsNullOrWhiteSpace(errors) ? errors + Environment.NewLine + errotList[error] : errotList[error];
                                }
                                return this.Json(errors);
                            }

                            //this.ShowMessage(SystemEnums.MessageType.Error, String.Join(Environment.NewLine, ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage + " " + v.Exception)), true);
                            //return this.View("~/Views/Account/ResellerSignUp.cshtml", userView);
                            return this.Json(String.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage + " " + v.Exception)));
                        }
                    }
                    else
                    {
                        return this.Json("Email or Username or Password is required. Please check.");
                    }
                }
            }
            else
            {
                ProjectSession.Theme = ProjectSession.Theme;
                //var designRole = from SystemEnums.PostalAddressType s in Enum.GetValues(typeof(SystemEnums.PostalAddressType))
                //                 select new { ID = s.GetHashCode(), Name = s.ToString().Replace('_', ' ') };
                ViewBag.PostalAddressType = new SelectList(EnumExtensions.GetPostalAddressEnumList(), "ID", "Name");
                return this.Json(String.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage + " " + v.Exception)));
            }
        }

        /// <summary>
        /// MyProfile for popup
        /// </summary>
        /// <returns>My Profile</returns>
        [HttpGet]
        public ActionResult MyProfile()
        {
            int userId = FormBot.Helper.ProjectSession.LoggedInUserId;
            var dsUsers = _userBAL.GetUserById(userId);
            List<FormBot.Entity.User> user = DBClient.DataTableToList<FormBot.Entity.User>(dsUsers.Tables[0]);
            FormBot.Entity.User userView = user.FirstOrDefault();
            userView.lstUserDocument = _userBAL.GetUserUsersDocumentByUserId(userId);
            if (userView.IsPostalAddress == true)
            {
                userView.AddressID = 2;
            }
            else
            {
                userView.AddressID = 1;
            }

            if (userView.WholesalerIsPostalAddress == 1)
            {
                userView.WholesalerIsPostalAddress = 2;
            }
            else
            {
                userView.WholesalerIsPostalAddress = 1;
            }

            if (userView.IsSTC == true)
            {
                userView.chkSTC = 1;
            }
            else
            {
                userView.chkSTC = 0;
            }

            userView.EmailSignup = new Entity.Email.EmailSignup();
            #region Email configuration not required

            //Email configuration is not required at a time of first login
            userView.EmailSignup.IsRequired = false;

            #endregion

            //var designRole = from SystemEnums.PostalAddressType s in Enum.GetValues(typeof(SystemEnums.PostalAddressType))
            //                 select new { ID = s.GetHashCode(), Name = s.ToString().Replace('_', ' ') };
            ViewBag.PostalAddressType = new SelectList(EnumExtensions.GetPostalAddressEnumList(), "ID", "Name");

            userView.RecEmailSignup = new Entity.Email.RecEmailSignup();
            userView.RecEmailSignup.RecIsRequired = false;
            RecEmailConfiguration(userView);

            if (userView.RecEmailSignup.RecConfigurationPassword != null)
            {
                userView.RecEmailSignup.RecConfigurationPassword = Utils.DecodePassword(userView.RecEmailSignup.RecConfigurationPassword);
            }

            EmailConfiguration(userView);
            if (userView.EmailSignup.ConfigurationPassword != null)
            {
                userView.EmailSignup.ConfigurationPassword = Utils.DecodePassword(userView.EmailSignup.ConfigurationPassword);
            }

            DateTime fromDate = Convert.ToDateTime(userView.FromDate);
            DateTime toDate = Convert.ToDateTime(userView.ToDate);
            userView.strFromDate = fromDate.ToString("yyyy-MM-dd");
            userView.strToDate = toDate.ToString("yyyy-MM-dd");

            //userView.solarElectricianView = new SolarElectricianView();
            //userView.solarElectricianView.IsSendRequest = true;

            //userView.UserTypeID = ProjectSession.UserTypeId;
            userView.installerDesignerView = new InstallerDesignerView();
            if (ProjectSession.UserTypeId == 1 || ProjectSession.UserTypeId == 2 || ProjectSession.UserTypeId == 3 || ProjectSession.UserTypeId == 4 || ProjectSession.UserTypeId == 5 || ProjectSession.UserTypeId == 6 || ProjectSession.UserTypeId == 8)
            {
                var statuses = from SystemEnums.SEDesignRole s in Enum.GetValues(typeof(SystemEnums.SEDesignRole))
                               select new { ID = s, Name = s.ToString().Replace('_', ' ') };
                ViewData["SEDesignRole"] = new SelectList(statuses, "ID", "Name");
            }
            TempData[userView.UserId.ToString()] = userView;
            return View(userView);
        }

        /// <summary>
        /// Post method for view user
        /// </summary>
        /// <param name="userView">The user view.</param>
        /// <returns>View Detail</returns>
        [HttpPost]
        [UserAuthorization]
        public async Task<ActionResult> ViewDetail(FormBot.Entity.User userView, string onoffswitch = "off", string SCisAllowedSPVswitch = "off", string hiddenUserNoteID = null)
        {
            ModelState.Remove("LoginCompanyName");
            ModelState.Remove("Password");
            ModelState.Remove("UserName");
            ModelState.Remove("Note");
            if (onoffswitch.ToLower().Equals("off"))
            {
                ModelState.Remove("CustomCompanyName");
                userView.CustomCompanyName = "";
            }
            RequiredValidationField(userView);
            RemoveInstallerDesignerValidation();
            if (!userView.IsWholeSaler || userView.UserTypeID != 2)
                RemoveWholeSalerValidation();
            if (userView.UserTypeID != 2 || (userView.UserTypeID == 2 && userView.UsageType != 3))
            {
                RemoveSAASUserValidation();
            }
            //if (userView.UserTypeID == 10)
            //    userView.ElectricalContractorsLicenseNumber = userView.SWHLicenseNumber;

            if (!string.IsNullOrWhiteSpace(userView.CompanyName))
            {
                userView.CompanyName = HttpUtility.HtmlDecode(userView.CompanyName);
            }
            if (userView.UserTypeID == 4)
            {
                userView.SCisAllowedSPV = SCisAllowedSPVswitch.ToLower() == "on" ? true : false;
            }

            if (ModelState.IsValid)
            {
                //FormBot.Entity.User uv = Session["UserView"] as FormBot.Entity.User;
                FormBot.Entity.User uv = TempData[userView.UserId.ToString()] as FormBot.Entity.User;

                //var designRole = from SystemEnums.PostalAddressType s in Enum.GetValues(typeof(SystemEnums.PostalAddressType))
                //                 select new { ID = s.GetHashCode(), Name = s.ToString().Replace('_', ' ') };
                ViewBag.PostalAddressType = new SelectList(EnumExtensions.GetPostalAddressEnumList(), "ID", "Name");
                userView.EmailSignup = new Entity.Email.EmailSignup();

                UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                UserManager.UserValidator = new UserValidator<ApplicationUser>(UserManager) { AllowOnlyAlphanumericUserNames = false };

                var userDetails = UserManager.FindById(userView.AspNetUserId);

                if (userDetails != null)
                {
                    userDetails.Email = userView.Email;
                    if (!string.IsNullOrWhiteSpace(userView.UserName))
                        userDetails.UserName = userView.UserName.Replace(" ", "");
                    PasswordHasher hasher = new PasswordHasher();
                    if (userView.Password != null)
                    {
                        userDetails.PasswordHash = hasher.HashPassword(userView.Password);
                    }
                }
                string sErrorMessage = string.Empty;

                if (userView.UserTypeID == 7)
                {
                    // check in user table => user is exist or not with same parameter
                    var isUserExists = _userBAL.CheckAccreditationNumber(userView.CECAccreditationNumber, userView.ElectricalContractorsLicenseNumber, userView.Email, userView.IsPVDUser == null ? false : userView.IsPVDUser.Value, userView.IsSWHUser == null ? false : userView.IsSWHUser.Value, false, userView.UserId);
                    if (isUserExists.UserId > 0)
                    {
                        if (userView.IsPVDUser == true)
                        {
                            sErrorMessage = GetErrorMessageForSolarElectrician(1, isUserExists);
                        }
                        if (userView.IsSWHUser == true)
                        {
                            if (!string.IsNullOrWhiteSpace(sErrorMessage))
                                sErrorMessage = GetErrorMessageForSolarElectrician(2, isUserExists);
                            else
                                sErrorMessage = GetErrorMessageForSolarElectrician(3, isUserExists);
                        }
                    }

                    if (userView.IsPVDUser.Value)
                    {
                        List<AccreditedInstallers> accreditedInstallersList = _userBAL.CheckAccreditationNumberExistsInAccreditedInstallers(userView.CECAccreditationNumber, userView.ElectricalContractorsLicenseNumber, userView.IsPVDUser.Value, userView.IsSWHUser.Value);
                        if (!accreditedInstallersList.Any())
                            sErrorMessage = GetErrorMessageForSolarElectrician(4);
                    }

                }
                if (string.IsNullOrWhiteSpace(sErrorMessage))
                {
                    var result = await UserManager.UpdateAsync(userDetails);
                    if (result.Succeeded)
                    {
                        userView.ModifiedBy = ProjectSession.LoggedInUserId;
                        userView.ModifiedDate = DateTime.Today;

                        //userView.Logo = "Images/logo.png";
                        if (userView.strFromDate != null)
                        {
                            userView.FromDate = Convert.ToDateTime(userView.strFromDate);
                        }

                        if (userView.strToDate != null)
                        {
                            userView.ToDate = Convert.ToDateTime(userView.strToDate);
                        }

                        if (userView.AddressID == 2)
                        {
                            userView.IsPostalAddress = true;
                        }
                        else
                        {
                            userView.IsPostalAddress = false;
                        }

                        if (userView.WholesalerIsPostalAddress == 2)
                        {
                            userView.WholesalerIsPostalAddress = 1;
                        }
                        else
                        {
                            userView.WholesalerIsPostalAddress = 0;
                        }

                        if (userView.IsSTC == false && userView.UserTypeID == 4)
                        {
                            userView.CECAccreditationNumber = null;
                            userView.ElectricalContractorsLicenseNumber = null;
                            userView.CECDesignerNumber = null;
                        }

                        if (userView.CompanyABN != null)
                        {
                            userView.CompanyABN = userView.CompanyABN.Replace(" ", "");
                        }

                        //int? isUserExists = 0;
                        //string sErrorMessage = string.Empty;
                        //if (userView.UserTypeID == 7)
                        //{
                        //    // check in user table => user is exist or not with same parameter
                        //    var isUserExists = _userBAL.CheckAccreditationNumber(userView.CECAccreditationNumber, userView.ElectricalContractorsLicenseNumber, userView.Email, userView.IsPVDUser == null ? false : userView.IsPVDUser.Value, userView.IsSWHUser == null ? false : userView.IsSWHUser.Value, false, userView.UserId);
                        //    if (isUserExists.UserId > 0)
                        //    {
                        //        if (userView.IsPVDUser == true)
                        //        {
                        //            sErrorMessage = GetErrorMessageForSolarElectrician(1, isUserExists);
                        //        }
                        //        if (userView.IsSWHUser == true)
                        //        {
                        //            if (!string.IsNullOrEmpty(sErrorMessage))
                        //                sErrorMessage = GetErrorMessageForSolarElectrician(2, isUserExists);
                        //            else
                        //                sErrorMessage = GetErrorMessageForSolarElectrician(3, isUserExists);
                        //        }
                        //    }

                        //    List<AccreditedInstallers> accreditedInstallersList = _userBAL.CheckAccreditationNumberExistsInAccreditedInstallers(userView.CECAccreditationNumber, userView.ElectricalContractorsLicenseNumber);
                        //    if (!accreditedInstallersList.Any())
                        //        sErrorMessage = GetErrorMessageForSolarElectrician(4);
                        //}
                        //if (string.IsNullOrEmpty(sErrorMessage))
                        //{
                        int userID = _userBAL.Create(userView);
                        if (userView.UserTypeID == 4)
                        {
                            string entityName = strGetEntityName(userView.CompanyABN);
                            _userBAL.UpdateEntityName(userView.UserId, entityName);
                        }
                       

                        if (userView.UserTypeID == 4 || userView.UserTypeID == 6)
                        {
                            if (userView.ProofFileNamesCreate != null)
                            {
                                foreach (var files in userView.ProofFileNamesCreate)
                                {
                                    //userView.FileName = files;
                                    if (files.ProofDocumentType > 0)
                                    {
                                        string UserHistoryMessage = "has uploaded a " + Enum.GetName(typeof(SystemEnums.ProofDocumentType), files.ProofDocumentType) + " file " + files.FileName;
                                        Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                                        _userBAL.InsertUserDocument(userView.UserId, files.FileName, files.ProofDocumentType);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (userView.FileNamesCreate != null)
                            {
                                foreach (var files in userView.FileNamesCreate)
                                {
                                    userView.FileName = files;
                                    string UserHistoryMessage = "uploaded a file " + userView.FileName;
                                    Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                                    _userBAL.InsertUserDocument(userView.UserId, files);
                                }
                            }
                            if (userView.UserTypeID == 7 || userView.UserTypeID == 10)
                            {
                                if (userView.ProofFileNamesCreate != null)
                                {
                                    foreach (var files in userView.ProofFileNamesCreate)
                                    {
                                        //userView.FileName = files;
                                        if (files.ProofDocumentType > 0)
                                        {
                                            string UserHistoryMessage = "has uploaded a "+ Enum.GetName(typeof(SystemEnums.ProofDocumentType), files.ProofDocumentType) +" file " + files.FileName;
                                            Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                                            _userBAL.InsertUserDocument(userView.UserId, files.FileName, files.ProofDocumentType,files.DocLoc);
                                        }
                                    }
                                }
                                _userBAL.InsertUpdateDocVerification(userView.UserId,userView.IsSignatureVerified,userView.IsSelfieVerified,userView.IsDriverLicVerified,userView.IsOtherDocVerified);
                            }
                        }

                        int complainBy = ProjectSession.LoggedInUserId;
                        _userBAL.InsertStatusWithoutNote(userView.UserId, userView.Status,  complainBy, userView.IsSCDetailConfirm, userView.IsInstaller, userView.IsSEDetailConfirm, userView.IsGSTSetByAdminUser);
                        if (!string.IsNullOrEmpty(userView.Note))
                        {
                            SaveUserNotetoXML(userView.UserId, userView.Note, userView.NotesType, userView.UserTypeID, userView.IsImportantNote, hiddenUserNoteID);
                        }
                        JobHistory jobHistory = new JobHistory();
                        jobHistory.Name = ProjectSession.LoggedInName;
                        jobHistory.FunctionalityName = "Updating Solar Company Details(Solar Company:" + userView.CompanyName + "[SolarCompanyId:" + userView.SolarCompanyId + "])";
                        jobHistory.JobID = 0;
                        //bool isHistorySaved = _createJobHistoryBAL.LogJobHistory(jobHistory, HistoryCategory.ModifiedIsGst);
                        string JobHistoryMessage = "modified Gst from " + jobHistory.FunctionalityName;
                        Common.SaveJobHistorytoXML(jobHistory.JobID, JobHistoryMessage, "General", "ModifiedIsGst", ProjectSession.LoggedInName, false);
                        // FCO and FSA can accept request for SCA want join as a SE
                        if (userView.IsSTC == true && userView.UserTypeID == 7 && userView.Status == 2 && (ProjectSession.UserTypeId == 3 || ProjectSession.UserTypeId == 1))
                        {
                            _userBAL.SCASEInsert(userView);
                        }

                        if (userView.Status != 1)
                        {
                            ////EmailTemplate eTemplate = null;
                            EmailInfo emailInfo = new EmailInfo();
                            emailInfo.FirstName = userView.FirstName;
                            emailInfo.LastName = userView.LastName;
                            emailInfo.UserName = userView.UserName;
                            emailInfo.Password = userView.Password;

                            emailInfo.Details = ComplianceCheckDetail(uv, userView);

                            if (userView.Status == 2)
                            {
                                emailInfo.TemplateID = 3;
                                ////eTemplate = _emailBAL.PrepareEmailTemplate(3, uv, userView, null, null);
                            }
                            else if (userView.Status == 3)
                            {
                                emailInfo.TemplateID = 4;
                                ////eTemplate = _emailBAL.PrepareEmailTemplate(4, uv, userView, null, null);
                            }
                            else
                            {
                                emailInfo.TemplateID = userView.UserTypeID == 7 ? 48 : 22;
                                emailInfo.LoginLink = userView.UserTypeID == 7 ? ProjectSession.LoginLink + "Account/Login/?returnUrl=User/Profile&flg=DocUpload" : ProjectSession.LoginLink + "Account/ResellerSignUp/" + userView.LoginCompanyName + "/" + userView.Id;
                                ////eTemplate = _emailBAL.PrepareEmailTemplate(22, uv, userView, null, null);
                                ////_emailBAL.GeneralComposeAndSendMail(eTemplate, userView.Email);
                            }
                            _emailBAL.ComposeAndSendEmail(emailInfo, userView.Email);
                        }
                        if (userView.UserTypeID == 7 && userView.IsVerified == 0 && (TempData["DocFlag"] != null && TempData["DocFlag"].ToString().ToLower() == "docverify"))
                        {
                            EmailInfo emailInfo = new EmailInfo();
                            emailInfo.FirstName = userView.FirstName;
                            emailInfo.LastName = userView.LastName;
                            emailInfo.Details = ComplianceCheckDetail(uv, userView);
                            emailInfo.TemplateID = 47;
                            emailInfo.LoginLink = ProjectSession.LoginLink + "Account/Login/?returnUrl=User/Profile&flg=DocUpload";
                            _emailBAL.ComposeAndSendEmail(emailInfo, userView.Email);
                        }

                        if (userView.UserTypeID == 4 && uv != null)
                        {
                            if (uv.SolarCompanyId != null)
                            {
                                string companyId = Convert.ToString(uv.SolarCompanyId);
                                EmailNotificationForSCAUser(uv, userView, Convert.ToInt32(companyId));
                            }
                        }

                        string strEmailConfigureMsg = string.Empty;
                        strEmailConfigureMsg = !ProjectSession.IsUserEmailAccountConfigured ? "(Can not send mail to user because email account is not configured)" : string.Empty;
                        this.ShowMessage(SystemEnums.MessageType.Success, "User status has been saved successfully.", true);

                        if (userView.FirstName != userView.PreviousFirstName)
                        {
                            string PreviousFirstName = (string.IsNullOrEmpty(userView.PreviousFirstName)) ? "null" : userView.PreviousFirstName;
                            string NewFirstName = (string.IsNullOrEmpty(userView.FirstName)) ? "null" : userView.FirstName;
                            string UserHistoryMessage = "changed FirstName from " + PreviousFirstName + " to " + NewFirstName;
                            Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                        }
                        if(userView.LastName != userView.PreviousLastName)
                        {
                            string PreviousLastName = (string.IsNullOrEmpty(userView.PreviousLastName)) ? "null" : userView.PreviousLastName;
                            string NewLastName = (string.IsNullOrEmpty(userView.LastName)) ? "null" : userView.LastName;
                            string UserHistoryMessage = "changed Lastname from " + PreviousLastName + " to " + NewLastName;
                            Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                        }
                        if(userView.Email != userView.PreviousEmail)
                        {
                            string PreviousEmail = (string.IsNullOrEmpty(userView.PreviousEmail)) ? "null" : userView.PreviousEmail;
                            string NewEmail = (string.IsNullOrEmpty(userView.Email)) ? "null" : userView.Email;
                            string UserHistoryMessage = "changed Email from " + PreviousEmail + " to " + NewEmail;
                            Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                        }
                        if (userView.PreviousPhone != userView.Phone)
                        {
                            string PreviousPhone = (string.IsNullOrEmpty(userView.PreviousPhone)) ? "null" : userView.PreviousPhone;
                            string NewPhone = (string.IsNullOrEmpty(userView.Phone)) ? "null" : userView.Phone;
                            string UserHistoryMessage = "changed Phone from " + PreviousPhone + " to " + NewPhone;
                            Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                        }
                        if(userView.PreviousMobile != userView.Mobile)
                        {
                            string PreviousMobile = (string.IsNullOrEmpty(userView.PreviousMobile)) ? "null" : userView.PreviousMobile;
                            string NewMobile = (string.IsNullOrEmpty(userView.Mobile)) ? "null" : userView.Mobile;
                            string UserHistoryMessage = "changed Mobile from " + PreviousMobile + " to " + NewMobile;
                            Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                        }
                        if(userView.PreviousUserName != userView.UserName)
                        {
                            string PreviousUserName = (string.IsNullOrEmpty(userView.PreviousUserName)) ? "null" : userView.PreviousUserName;
                            string NewUserName = (string.IsNullOrEmpty(userView.UserName)) ? "null" : userView.UserName;
                            string UserHistoryMessage = "changed UserName from " + PreviousUserName + " to " + NewUserName;
                            Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                        }
                        //if(userView.PreviousPassword != userView.Password)
                        //{
                        //    string PreviousPassword = (string.IsNullOrEmpty(userView.PreviousPassword)) ? "null" : userView.PreviousPassword;
                        //    string NewPassword = (string.IsNullOrEmpty(userView.Password)) ? "null" : userView.Password;
                        //    string UserHistoryMessage = "changed Password from " + PreviousPassword + " to "  + NewPassword;
                        //    Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                        //}
                        if(userView.PreviousGB_SCACode != userView.GB_SCACode)
                        {
                            string PreviousGB_SCACode = (string.IsNullOrEmpty(userView.PreviousGB_SCACode)) ? "null" : userView.PreviousGB_SCACode;
                            string NewGB_SCACode = (string.IsNullOrEmpty(userView.GB_SCACode)) ? "null" : userView.GB_SCACode;
                            string UserHistoryMessage = "changed Solar Company Code from " + PreviousGB_SCACode + " to " + NewGB_SCACode;
                            Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                        }
                        if(userView.PreviousCompanyABN != userView.CompanyABN)
                        {
                            string PreviousCompanyABN = (string.IsNullOrEmpty(userView.PreviousCompanyABN)) ? "null" : userView.PreviousCompanyABN;
                            string NewCompanyABN = (string.IsNullOrEmpty(userView.CompanyABN)) ? "null" : userView.CompanyABN;
                            string UserHistoryMessage = "changed Company ABN from " + PreviousCompanyABN + " to " + NewCompanyABN;
                            Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                        }
                        if (userView.PreviousCECAccreditationNumber != userView.CECAccreditationNumber)
                        {
                            string PreviousCECAccrdNo = (string.IsNullOrEmpty(userView.PreviousCECAccreditationNumber)) ? "null" : userView.PreviousCECAccreditationNumber;
                            string NewCECAccrdNo = (string.IsNullOrEmpty(userView.CECAccreditationNumber)) ? "null" : userView.CECAccreditationNumber;
                            string UserHistoryMessage = "changed CECAccrediationNumber from " + PreviousCECAccrdNo + " to " + NewCECAccrdNo;
                            Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                        }
                        if(userView.PreviousElectricalContractorsLicenseNumber != userView.ElectricalContractorsLicenseNumber)
                        {
                            string PreviousLicenseNo = (string.IsNullOrEmpty(userView.PreviousElectricalContractorsLicenseNumber)) ? "null" : userView.PreviousElectricalContractorsLicenseNumber;
                            string NewLicenseNo = (string.IsNullOrEmpty(userView.ElectricalContractorsLicenseNumber)) ? "null" : userView.ElectricalContractorsLicenseNumber;
                            string UserHistoryMessage = "changed ElectricalContractorsLicenseNumber from " + PreviousLicenseNo + " to " + NewLicenseNo;
                            Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                        }
                        if (userView.PreviousCECDesignerNumber != userView.CECDesignerNumber)
                        {
                            string PreviousCECDesignerNo = (string.IsNullOrEmpty(userView.PreviousCECDesignerNumber)) ? "null" : userView.PreviousCECDesignerNumber;
                            string NewCECDesignNo = (string.IsNullOrEmpty(userView.CECDesignerNumber)) ? "null" : userView.CECDesignerNumber;
                            string UserHistoryMessage = "changed CECDesignNumber from " + PreviousCECDesignerNo + " to " + NewCECDesignNo;
                            Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                        }
                        if (userView.PreviousSEDesigner != userView.SEDesigner)
                        {
                            string PreviousSERole = (userView.PreviousSEDesigner == 1) ? "Design" : (userView.PreviousSEDesigner == 2) ? "Install" : "DesignandInstall";
                            string SERole = (userView.SEDesigner == 1) ? "Design" : (userView.SEDesigner == 2) ? "Install" : "DesignandInstall";
                            string UserHistoryMessage = "changed SE role from " + PreviousSERole + " to " + SERole;
                            Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                        }
                        if(userView.PreviousCompanyName != userView.CompanyName)
                        {
                            string PreviousCompanyName = (string.IsNullOrEmpty(userView.PreviousCompanyName)) ? "null" : userView.PreviousCompanyName;
                            string NewCompanyName = (string.IsNullOrEmpty(userView.CompanyName)) ? "null" : userView.CompanyName;
                            string UserHistoryMessage = "changed CompanyName from " + PreviousCompanyName + " to " + NewCompanyName;
                            Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                        }
                        if(userView.PreviousSignature != userView.Signature)
                        {
                            string UserHistoryMessage = "uploaded signature file " + userView.Signature;
                            Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                        }
                        if (userView.PreviousSESelfie != userView.SESelfie)
                        {
                            string UserHistoryMessage = "uploaded Selfie file " + userView.SESelfie;
                            Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                        }
                        if(userView.PreviousIsPVDUser != userView.IsPVDUser || userView.PreviousIsSWHUser!=userView.IsSWHUser)
                        {
                            string PVDUser = (userView.IsPVDUser == true) ? "Yes" : "No";
                            string SWHUser = (userView.IsSWHUser == true) ? "Yes" : "No";
                            string UserHistoryMessage = "changed SE User type IsPVDUser: " + PVDUser + " and IsSWHUser: " + SWHUser;
                            Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                        }
                        if (userView.PreviousIsAutoRequest != userView.IsAutoRequest)
                        {
                            string AutoRequest = (userView.IsAutoRequest == true) ? "Yes" : "No";
                            string UserHistoryMessage = "changed Allow any solar company to auto-approve and send you jobs: " + AutoRequest;
                            Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                        }
                        if(userView.PreviousCompanyWebsite != userView.CompanyWebsite)
                        {
                            string PreviousCompanyWebsite = (string.IsNullOrEmpty(userView.PreviousCompanyWebsite)) ? "null" : userView.PreviousCompanyWebsite;
                            string NewCompanyWebsite = (string.IsNullOrEmpty(userView.CompanyWebsite)) ? "null" : userView.CompanyWebsite;
                            string UserHistoryMessage = "changed Company Website from " + PreviousCompanyWebsite + " to " + NewCompanyWebsite;
                            Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                        }
                        string OldUserAddress = "";
                        string NewUserAddress = "";
                        if(userView.AddressID == 1)
                        {
                            string OldAddress = "";
                            string NewAddress = "";
                            string OldaddressLine3 = "";
                            string OldaddressLine1, OldaddressLine2, OldstreetAddress, OldpostCodeAddress = "";
                            string NewaddressLine1, NewaddressLine2, NewaddressLine3, NewstreetAddress, NewpostCodeAddress = "";
                            if (userView.PreviousAddressID != userView.AddressID)
                            {
                                string PostalAddressType = (userView.PreviousPostalAddressID > 0) ? _userBAL.GetPostalDeliveryNameByID(userView.PreviousPostalAddressID) : "";
                                OldaddressLine1 = (PostalAddressType == "" || PostalAddressType == null) ? "" : PostalAddressType + ((userView.PreviousPostalDeliveryNumber == "" || userView.PreviousPostalDeliveryNumber == null) ? "" : " " + userView.PreviousPostalDeliveryNumber);
                                OldaddressLine2 = userView.PreviousTown + " " + userView.PreviousState + " " + userView.PreviousPostCode;
                                if(!string.IsNullOrWhiteSpace(OldaddressLine3))
                                {
                                    OldaddressLine3 = "," + OldaddressLine3;
                                }
                                else
                                {
                                    OldaddressLine3 = "";
                                }
                                OldAddress =(string.IsNullOrEmpty(OldaddressLine1))?OldaddressLine2+OldaddressLine3: OldaddressLine1 + "," + OldaddressLine2 + OldaddressLine3;
                                OldUserAddress = OldAddress;
                                if(string.IsNullOrEmpty(userView.UnitTypeID.ToString()) && string.IsNullOrEmpty(userView.UnitNumber))
                                {
                                    string StreetTypeName = (userView.StreetTypeID > 0) ? _userBAL.GetStreetTypeNameByID(userView.StreetTypeID) : "";
                                    NewaddressLine1 = (userView.StreetNumber == "" || userView.StreetNumber == null) ? "" : userView.StreetNumber + ((userView.StreetName == "" || userView.StreetName == null) ? "" : " " + userView.StreetName) + ((StreetTypeName == "" || StreetTypeName == null) ? "" : " " + StreetTypeName); 
                                    NewaddressLine2 = userView.Town + " " + userView.State + " " + userView.PostCode; 
                                    NewaddressLine3 = "";
                                }
                                else
                                {
                                    string StreetTypeName = (userView.StreetTypeID > 0) ? _userBAL.GetStreetTypeNameByID(userView.StreetTypeID) : "";
                                    string UnitTypeName = (userView.UnitTypeID > 0) ? _userBAL.GetUnitTypeNameByID(userView.UnitTypeID) : "";
                                    NewaddressLine1 = UnitTypeName + ((userView.UnitNumber == "" || userView.UnitNumber == null) ? "" : " " + userView.UnitNumber);
                                    NewaddressLine2 = (userView.StreetNumber == "" || userView.StreetNumber == null) ? "" : userView.StreetNumber + ((userView.StreetName == "" || userView.StreetName == null) ? "" : " " + userView.StreetName) + ((StreetTypeName == "" || StreetTypeName == null) ? "" : " " + StreetTypeName);
                                    NewaddressLine3 = userView.Town + " " + userView.State + " " + userView.PostCode;
                                }
                                if (!string.IsNullOrWhiteSpace(NewaddressLine3))
                                {
                                    NewaddressLine3 = "," + NewaddressLine3;
                                }
                                else
                                {
                                    NewaddressLine3 = "";
                                }
                                NewAddress = (string.IsNullOrEmpty(NewaddressLine1))? NewaddressLine2+NewaddressLine3 : NewaddressLine1 + "," + NewaddressLine2 + NewaddressLine3;
                                NewUserAddress = NewAddress;
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(userView.PreviousUnitTypeID.ToString()) && string.IsNullOrEmpty(userView.PreviousUnitNumber))
                                {
                                    string StreetTypeName = (userView.PreviousStreetTypeID > 0) ? _userBAL.GetStreetTypeNameByID(userView.PreviousStreetTypeID) : "";
                                    OldaddressLine1 = (userView.PreviousStreetNumber == "" || userView.PreviousStreetNumber == null) ? "" : userView.PreviousStreetNumber + ((userView.PreviousStreetName == "" || userView.PreviousStreetName == null) ? "" : " " + userView.PreviousStreetName) + ((StreetTypeName == "" || StreetTypeName == null) ? "" : " " + StreetTypeName);
                                    OldaddressLine2 = userView.PreviousTown + " " + userView.PreviousState + " " + userView.PreviousPostCode;
                                    OldaddressLine3 = "";
                                }
                                else
                                {
                                    string StreetTypeName = (userView.PreviousStreetTypeID > 0) ? _userBAL.GetStreetTypeNameByID(userView.PreviousStreetTypeID) : "";
                                    string UnitTypeName = (userView.PreviousUnitTypeID > 0) ? _userBAL.GetUnitTypeNameByID(userView.PreviousUnitTypeID) : "";
                                    OldaddressLine1 = UnitTypeName + ((userView.PreviousUnitNumber == "" || userView.PreviousUnitNumber == null) ? "" : " " + userView.PreviousUnitNumber);
                                    OldaddressLine2 = (userView.PreviousStreetNumber == "" || userView.PreviousStreetNumber == null) ? "" : userView.PreviousStreetNumber + ((userView.PreviousStreetName == "" || userView.PreviousStreetName == null) ? "" : " " + userView.PreviousStreetName) + ((StreetTypeName == "" || StreetTypeName == null) ? "" : " " + StreetTypeName);
                                    OldaddressLine3 = userView.PreviousTown + " " + userView.PreviousState + " " + userView.PreviousPostCode;
                                }
                                if (!string.IsNullOrWhiteSpace(OldaddressLine3))
                                {
                                    OldaddressLine3 = "," + OldaddressLine3;
                                }
                                else
                                {
                                    OldaddressLine3 = "";
                                }
                                OldAddress = (string.IsNullOrEmpty(OldaddressLine1))? OldaddressLine2 + OldaddressLine3: OldaddressLine1 + "," + OldaddressLine2 + OldaddressLine3;
                                OldUserAddress = OldAddress;
                                if (string.IsNullOrEmpty(userView.UnitTypeID.ToString()) && string.IsNullOrEmpty(userView.UnitNumber))
                                {
                                    string StreetTypeName = (userView.StreetTypeID > 0) ? _userBAL.GetStreetTypeNameByID(userView.StreetTypeID) : "";
                                    NewaddressLine1 = (userView.StreetNumber == "" || userView.StreetNumber == null) ? "" : userView.StreetNumber + ((userView.StreetName == "" || userView.StreetName == null) ? "" : " " + userView.StreetName) + ((StreetTypeName == "" || StreetTypeName == null) ? "" : " " + StreetTypeName);
                                    NewaddressLine2 = userView.Town + " " + userView.State + " " + userView.PostCode;
                                    NewaddressLine3 = "";
                                }
                                else
                                {
                                    string StreetTypeName = (userView.StreetTypeID > 0) ? _userBAL.GetStreetTypeNameByID(userView.StreetTypeID) : "";
                                    string UnitTypeName = (userView.UnitTypeID > 0) ? _userBAL.GetUnitTypeNameByID(userView.UnitTypeID) : "";
                                    NewaddressLine1 = UnitTypeName + ((userView.UnitNumber == "" || userView.UnitNumber == null) ? "" : " " + userView.UnitNumber);
                                    NewaddressLine2 = (userView.StreetNumber == "" || userView.StreetNumber == null) ? "" : userView.StreetNumber + ((userView.StreetName == "" || userView.StreetName == null) ? "" : " " + userView.StreetName) + ((StreetTypeName == "" || StreetTypeName == null) ? "" : " " + StreetTypeName);
                                    NewaddressLine3 = userView.Town + " " + userView.State + " " + userView.PostCode;
                                }
                                if (!string.IsNullOrWhiteSpace(NewaddressLine3))
                                {
                                    NewaddressLine3 = "," + NewaddressLine3;
                                }
                                else
                                {
                                    NewaddressLine3 = "";
                                }
                                NewAddress = (string.IsNullOrEmpty(NewaddressLine1))? NewaddressLine2+ NewaddressLine3 : NewaddressLine1 + "," + NewaddressLine2 + NewaddressLine3;
                                NewUserAddress = NewAddress;
                            }
                        }
                        if (userView.AddressID == 2)
                        {
                            string OldAddress = "";
                            string NewAddress = "";
                            string OldaddressLine3 = "";
                            string NewaddressLine3 = "";
                            string OldaddressLine1, OldaddressLine2, OldstreetAddress, OldpostCodeAddress = "";
                            string NewaddressLine1, NewaddressLine2, NewstreetAddress, NewpostCodeAddress = "";
                            if (userView.PreviousAddressID != userView.AddressID)
                            {

                                if (string.IsNullOrEmpty(userView.PreviousUnitTypeID.ToString()) && string.IsNullOrEmpty(userView.PreviousUnitNumber))
                                {
                                    string StreetTypeName = (userView.PreviousStreetTypeID > 0) ? _userBAL.GetStreetTypeNameByID(userView.PreviousStreetTypeID) : "";
                                    OldaddressLine1 = (userView.PreviousStreetNumber == "" || userView.PreviousStreetNumber == null) ? "" : userView.PreviousStreetNumber + ((userView.PreviousStreetName == "" || userView.PreviousStreetName == null) ? "" : " " + userView.PreviousStreetName) + ((StreetTypeName == "" || StreetTypeName == null) ? "" : " " + StreetTypeName);
                                    OldaddressLine2 = userView.PreviousTown + " " + userView.PreviousState + " " + userView.PreviousPostCode;
                                    OldaddressLine3 = "";
                                }
                                else
                                {
                                    string StreetTypeName = (userView.PreviousStreetTypeID > 0) ? _userBAL.GetStreetTypeNameByID(userView.PreviousStreetTypeID) : "";
                                    string UnitTypeName = (userView.PreviousUnitTypeID > 0) ? _userBAL.GetUnitTypeNameByID(userView.PreviousUnitTypeID) : "";
                                    OldaddressLine1 = UnitTypeName + ((userView.PreviousUnitNumber == "" || userView.PreviousUnitNumber == null) ? "" : " " + userView.PreviousUnitNumber);
                                    OldaddressLine2 = (userView.PreviousStreetNumber == "" || userView.PreviousStreetNumber == null) ? "" : userView.PreviousStreetNumber + ((userView.PreviousStreetName == "" || userView.PreviousStreetName == null) ? "" : " " + userView.PreviousStreetName) + ((StreetTypeName == "" || StreetTypeName == null) ? "" : " " + StreetTypeName);
                                    OldaddressLine3 = userView.Town + " " + userView.State + " " + userView.PostCode;
                                }
                                if (!string.IsNullOrWhiteSpace(OldaddressLine3))
                                {
                                    OldaddressLine3 = "," + OldaddressLine3;
                                }
                                else
                                {
                                    OldaddressLine3 = "";
                                }
                                OldAddress = (string.IsNullOrEmpty(OldaddressLine1))?OldaddressLine2 + OldaddressLine3 : OldaddressLine1 + "," + OldaddressLine2 + OldaddressLine3;
                                OldUserAddress = OldAddress;

                                string PostalAddressType = (userView.PostalAddressID > 0) ? _userBAL.GetPostalDeliveryNameByID(userView.PostalAddressID) : "";
                                NewaddressLine1 = (PostalAddressType == "" || PostalAddressType == null) ? "" : PostalAddressType + ((userView.PostalDeliveryNumber == "" || userView.PostalDeliveryNumber == null) ? "" : " " + userView.PostalDeliveryNumber);
                                NewaddressLine2 = userView.Town + " " + userView.State + " " + userView.PostCode;
                                if (!string.IsNullOrWhiteSpace(NewaddressLine3))
                                {
                                    NewaddressLine3 = "," + NewaddressLine3;
                                }
                                else
                                {
                                    NewaddressLine3 = "";
                                }
                                NewAddress =(string.IsNullOrEmpty(NewaddressLine1)) ? NewaddressLine2+NewaddressLine3 : NewaddressLine1 + "," + NewaddressLine2 + NewaddressLine3;
                                NewUserAddress = NewAddress;
                            }
                            else
                            {
                                string PostalAddressType = (userView.PreviousPostalAddressID > 0) ? _userBAL.GetPostalDeliveryNameByID(userView.PreviousPostalAddressID) : "";
                                OldaddressLine1 = (PostalAddressType == "" || PostalAddressType == null) ? "" : PostalAddressType + ((userView.PreviousPostalDeliveryNumber == "" || userView.PreviousPostalDeliveryNumber == null) ? "" : " " + userView.PreviousPostalDeliveryNumber);
                                OldaddressLine2 = userView.PreviousTown + " " + userView.PreviousState + " " + userView.PreviousPostCode;
                                if (!string.IsNullOrWhiteSpace(OldaddressLine3))
                                {
                                    OldaddressLine3 = "," + OldaddressLine3;
                                }
                                else
                                {
                                    OldaddressLine3 = "";
                                }
                                OldAddress = (string.IsNullOrEmpty(OldaddressLine1))? OldaddressLine2+OldaddressLine3 : OldaddressLine1 + "," + OldaddressLine2 + OldaddressLine3;
                                OldUserAddress = OldAddress;
                                string NewPostalAddressType = (userView.PostalAddressID > 0) ? _userBAL.GetPostalDeliveryNameByID(userView.PostalAddressID) : "";
                                NewaddressLine1 = (NewPostalAddressType == "" || NewPostalAddressType == null) ? "" : NewPostalAddressType + ((userView.PostalDeliveryNumber == "" || userView.PostalDeliveryNumber == null) ? "" : " " + userView.PostalDeliveryNumber);
                                NewaddressLine2 = userView.Town + " " + userView.State + " " + userView.PostCode;
                                if (!string.IsNullOrWhiteSpace(NewaddressLine3))
                                {
                                    NewaddressLine3 = "," + NewaddressLine3;
                                }
                                else
                                {
                                    NewaddressLine3 = "";
                                }
                                NewAddress = (string.IsNullOrEmpty(NewaddressLine1)) ? NewaddressLine2+NewaddressLine3 : NewaddressLine1 + "," + NewaddressLine2 + NewaddressLine3;
                                NewUserAddress = NewAddress;
                            }
                        }
                        if(OldUserAddress != NewUserAddress)
                        {
                            string OldAdd = (string.IsNullOrEmpty(OldUserAddress)) ? "null" : OldUserAddress;
                            string NewAdd = (string.IsNullOrEmpty(NewUserAddress)) ? "null" : NewUserAddress;
                            string UserHistoryMessage = "changed User Address from " + OldAdd + " to " + NewAdd;
                            Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                        }
                        if(userView.PreviousStatus != userView.Status)
                        {
                            string PreviousStatus = Enum.GetName(typeof(SystemEnums.UserStatus), userView.PreviousStatus);
                            string NewStatus = Enum.GetName(typeof(SystemEnums.UserStatus), userView.Status);
                            String UserHistoryMessage = "changed Status from " + PreviousStatus + " to " + NewStatus;
                            Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                        }
                        if(userView.PreviousRoleID != userView.RoleID)
                        {
                            string PreviousRoleName = (userView.PreviousRoleID > 0) ? _userBAL.GetRoleNameByID(userView.PreviousRoleID) : "null";
                            string NewRoleName = (userView.RoleID > 0) ? _userBAL.GetRoleNameByID(userView.RoleID) : "null";
                            string UserHistoryMessage = "changed Role from " + PreviousRoleName + " to " + NewRoleName;
                            Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                        }
                        //if(userView.PreviousIsVerified != userView.IsVerified)
                        //{
                        //    string PreviousIsVerified = (userView.PreviousIsVerified == true) ? "Verified" : "Unverified";
                        //    string NewIsVerified = (userView.IsVerified == true) ? "Verified" : "Unverified";
                        //    string UserHistoryMessage = "changed Verification Status from " + PreviousIsVerified + " to " + NewIsVerified;
                        //    Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                        //}
                        if(userView.PreviousIsSEDetailConfirm != userView.IsSEDetailConfirm)
                        {
                            string PreviousIsSEDetailConfirm = (userView.PreviousIsSEDetailConfirm == true) ? "Yes" : "No";
                            string IsSEDetailConfirm = (userView.IsSEDetailConfirm == true) ? "Yes" : "No";
                            string UserHistoryMessage = "changed SE detail confirm from " + PreviousIsSEDetailConfirm + " to " + IsSEDetailConfirm;
                            Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                        }
                        if (userView.UserTypeID == 4)
                        {
                            if (userView.PreviousSCisAllowedSPV != userView.SCisAllowedSPV)
                            {
                                string PreviousIsAllowSPV = (userView.PreviousSCisAllowedSPV == true) ? "Yes" : "No";
                                string NewIsAllowSPV = (userView.SCisAllowedSPV == true) ? "Yes" : "No";
                                string UserHistoryMessage = "changed Allowed SPV from " + PreviousIsAllowSPV + " to " + NewIsAllowSPV;
                                Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                            }
                            if(userView.PreviousBSB != userView.BSB)
                            {
                                string PreviousBSB = (string.IsNullOrEmpty(userView.PreviousBSB)) ? "null" : userView.PreviousBSB;
                                string NewBSB = (string.IsNullOrEmpty(userView.BSB)) ? "null" : userView.BSB;
                                string UserHistoryMessage = "changed BSB from " + PreviousBSB + " to " + NewBSB;
                                Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                            }
                            if(userView.PreviousAccountNumber != userView.AccountNumber)
                            {
                                string PreviousAccountNo = (string.IsNullOrEmpty(userView.PreviousAccountNumber)) ? "null" : userView.PreviousAccountNumber;
                                string NewAccountNo = (string.IsNullOrEmpty(userView.AccountNumber)) ? "null" : userView.AccountNumber;
                                string UserHistoryMessage = "changed Account Number from " + PreviousAccountNo + " to " + NewAccountNo;
                                Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                            }
                            if(userView.PreviousAccountName != userView.AccountName)
                            {
                                string PreviousAccountName = (string.IsNullOrEmpty(userView.PreviousAccountName)) ? "null" : userView.PreviousAccountName;
                                string NewAccountName = (string.IsNullOrEmpty(userView.AccountName)) ? "null" : userView.AccountName;
                                string UserHistoryMessage = "changed BSB from " + PreviousAccountName + " to " + NewAccountName;
                                Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                            }
                            if(userView.PreviousRAMId != userView.RAMId)
                            {
                                string PreviousRAM = (Convert.ToInt32(userView.PreviousRAMId) > 0) ? _userBAL.GetRAMNameByID(Convert.ToInt32(userView.PreviousRAMId)) : "null";
                                string NewRAM = (Convert.ToInt32(userView.RAMId) > 0) ? _userBAL.GetRAMNameByID(Convert.ToInt32(userView.RAMId)) : "null";
                                string UserHistoryMessage = "changed RAM from " + PreviousRAM + " to " + NewRAM;
                                Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                            }
                            if(userView.PreviousClientNumber != userView.ClientNumber)
                            {
                                string PreviousClientNumber = (string.IsNullOrEmpty(userView.PreviousClientNumber)) ? "null" : userView.PreviousClientNumber;
                                string NewClientNumber = (string.IsNullOrEmpty(userView.ClientNumber)) ? "null" : userView.ClientNumber;
                                string UserHistoryMessage = "changed ClientNumber from " + PreviousClientNumber + " to " + NewClientNumber;
                                Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                            }
                            if(userView.PreviousUniqueCompanyNumber != userView.UniqueCompanyNumber)
                            {
                                string PreviousUniqueCompanyNo = (string.IsNullOrEmpty(userView.PreviousUniqueCompanyNumber)) ? "null" : userView.PreviousUniqueCompanyNumber;
                                string NewUniqueCompanyNo = (string.IsNullOrEmpty(userView.UniqueCompanyNumber)) ? "null" : userView.UniqueCompanyNumber;
                                string UserHistoryMessage = "changed Unique CompanyNumber from " + PreviousUniqueCompanyNo + " to " + NewUniqueCompanyNo;
                                Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                            }
                            if(userView.PreviousIsGSTSetByAdminUser != userView.IsGSTSetByAdminUser)
                            {
                                string PreviousIsGSTSetByAdminUser = (userView.PreviousIsGSTSetByAdminUser == 1) ? "Apply Normal GST Registered Rules" : "Override & Apply GST to All Jobs";
                                string NewIsGSTSetByAdminUser = (userView.IsGSTSetByAdminUser == 1) ? "Apply Normal GST Registered Rules" : "Override & Apply GST to All Jobs";
                                string UserHistoryMessage = "changed GST rule from " + PreviousIsGSTSetByAdminUser + " to " + NewIsGSTSetByAdminUser;
                                Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                            }
                            if((userView.PreviousCustomCompanyName != userView.CustomCompanyName) && !string.IsNullOrEmpty(userView.CustomCompanyName) || (!string.IsNullOrEmpty(userView.PreviousCustomCompanyName) && string.IsNullOrEmpty(userView.CustomCompanyName)))
                            {
                                string PreviousCustomCompanyName = (!string.IsNullOrEmpty(userView.PreviousCustomCompanyName)) ? userView.PreviousCustomCompanyName : "null";
                                string NewCustomCompanyName = (!string.IsNullOrEmpty(userView.CustomCompanyName)) ? userView.CustomCompanyName : "null";
                                string UserHistoryMessage = "changed Custom CompanyName from " + PreviousCustomCompanyName + " to " + NewCustomCompanyName;
                                Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                            }
                            if(userView.PreviousIsSCDetailConfirm != userView.IsSCDetailConfirm)
                            {
                                string PreviousIsSCDetailConfirm = (userView.PreviousIsSCDetailConfirm == true) ? "Yes" : "No";
                                string NewIsSCDetailConfirm = (userView.IsSCDetailConfirm == true) ? "Yes" : "No";
                                string UserHistoryMessage = "changed I have contacted the solar company and confirmed their details from " + PreviousIsSCDetailConfirm + " to " + NewIsSCDetailConfirm;
                                Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                            }
                            if(userView.PreviousIsInstaller != userView.IsInstaller)
                            {
                                string PreviousIsInstaller =(userView.PreviousIsInstaller == true) ? "Yes" : "No";
                                string NewIsInstaller = (userView.IsInstaller == true) ? "Yes" : "No";
                                string UserHistoryMessage = "changed I have contacted their CEC accredited installer if different from" + PreviousIsInstaller + " to " + NewIsInstaller;
                                Common.SaveUserHistorytoXML(userView.UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                            }
                        }
                        TempData["DocFlag"] = null;
                        if (userView.UserTypeID == 4)
                        {
                            //CommonBAL.SetCacheDataForSTCSubmissionFromSolarCompanyId(Convert.ToInt32(userView.ResellerID), Convert.ToString(uv.SolarCompanyId));
                            return RedirectToAction("SCA", "User");
                        }
                        else
                        {
                            return RedirectToAction("SE", "User");
                        }
                        //else
                        //{
                        //    this.ShowMessage(SystemEnums.MessageType.Error, sErrorMessage, true);
                        //}
                    }
                    else
                    {
                        this.ShowMessage(SystemEnums.MessageType.Error, string.Join(",", result.Errors), true);
                    }
                }
                else
                {
                    this.ShowMessage(SystemEnums.MessageType.Error, sErrorMessage, true);
                }

                return RedirectToAction("ViewDetail", "User", new { id = EncryptionDecryption.GetEncrypt("id=" + userView.UserId) });
                //if (userView.UserTypeID == 4)
                //{
                //    return RedirectToAction("SCA", "User");
                //}
                //else
                //{
                //    return RedirectToAction("SE", "User");
                //}
            }
            else
            {
                userView.lstUserDocument = _userBAL.GetUserUsersDocumentByUserId(userView.UserId);
                this.ShowMessage(SystemEnums.MessageType.Error, String.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage + " " + v.Exception)), true);
                return RedirectToAction("ViewDetail", "User", new { id = EncryptionDecryption.GetEncrypt("id=" + userView.UserId) });
                ////return this.View("ViewDetail", userView);
            }
        }

        /// <summary>
        /// post method for my profile popup
        /// </summary>
        /// <param name="userView">The user view.</param>
        /// <param name="emailModel">The email model.</param>
        /// <param name="recEmailModel">recEmailModel.</param>
        /// <returns>My Profile</returns>
        [HttpPost]
        public async Task<ActionResult> MyProfile(FormBot.Entity.User userView, FormBot.Entity.Email.EmailSignup emailModel, FormBot.Entity.Email.RecEmailSignup recEmailModel)
        {
            userView.UserTypeID = ProjectSession.UserTypeId;
            userView.UserId = FormBot.Helper.ProjectSession.LoggedInUserId;

            RequiredValidationField(userView);
            RemoveInstallerDesignerValidation();
            if (userView.Password == null)
            {
                ModelState.Remove("Password");
            }

            if (!userView.IsWholeSaler)
                RemoveWholeSalerValidation();
            if (userView.UserTypeID != 2 || (userView.UserTypeID == 2 && userView.UsageType != 3))
            {
                RemoveSAASUserValidation();
            }
            ModelState.Remove("CustomCompanyName");
            string xmlString = string.Empty;
            ModelState.Remove("Login");
            //Rec Email Configuration
            ModelState.Remove("RecLogin");
            if (ModelState.IsValid)
            {
                //var designRole = from SystemEnums.PostalAddressType s in Enum.GetValues(typeof(SystemEnums.PostalAddressType))
                //                 select new { ID = s.GetHashCode(), Name = s.ToString().Replace('_', ' ') };
                ViewBag.PostalAddressType = new SelectList(EnumExtensions.GetPostalAddressEnumList(), "ID", "Name");
                userView.lstFCOGroup1 = _userBAL.GetMultipleFCOGroupDropDown();

                userView.EmailSignup = new Entity.Email.EmailSignup();
                userView.RecEmailSignup = new Entity.Email.RecEmailSignup();

                #region Rec Email Configuration
                // Email configuration not required (check if all fields are entered by user then only configure mail account)
                if (recEmailModel != null && !string.IsNullOrWhiteSpace(recEmailModel.RecConfigurationEmail) && !string.IsNullOrWhiteSpace(recEmailModel.RecConfigurationPassword) && !string.IsNullOrWhiteSpace(recEmailModel.RecIncomingMail) && !string.IsNullOrWhiteSpace(recEmailModel.RecOutgoingMail))
                {
                    #region Rec Email configuration code
                    //recEmailModel.RecLogin = recEmailModel.RecConfigurationEmail;
                    //xmlString = "<?xml version='1.0' encoding='UTF-8'?><webmail><param name='action' value='login' /><param name='request' value='' /><param name='email'><![CDATA[" + recEmailModel.RecConfigurationEmail
                    //    + "]]></param><param name='mail_inc_login'><![CDATA[" + recEmailModel.RecLogin + "]]></param><param name='mail_inc_pass'><![CDATA[" + recEmailModel.RecConfigurationPassword + "]]></param><param name='mail_inc_host'><![CDATA[" + recEmailModel.RecIncomingMail
                    //    + "]]></param><param name='mail_inc_port' value='" + recEmailModel.RecIncomingMailPort + "' /><param name='mail_protocol' value='0' /><param name='mail_out_host'><![CDATA[" + recEmailModel.RecOutgoingMail
                    //    + "]]></param><param name='mail_out_port' value='" + recEmailModel.RecOutgoingMailPort + "' /><param name='mail_out_auth' value='1' /><param name='sign_me' value='0' /><param name='language' /><param name='advanced_login' value='1' /></webmail>";
                    //CheckMail checkMail = new CheckMail();
                    //var result = checkMail.GetMessages(xmlString);
                    //XDocument xDocument = XDocument.Parse(result.OuterXml);


                    //var error = xDocument.Descendants("webmail").Elements("error").FirstOrDefault();
                    //if (error != null && !string.IsNullOrEmpty(Convert.ToString(error.Value)))
                    //{
                    //    userView.lstUserDocument = _userBAL.GetUserUsersDocumentByUserId(userView.UserId);
                    //    this.ShowMessage(SystemEnums.MessageType.Error, error.Value, false);
                    //    return View("MyProfile", userView);
                    //}
                    //else
                    //{
                    //    Account acct = Session[FormBot.Email.Constants.sessionAccount] as Account;
                    //    if (acct != null)
                    //    {
                    //        xmlString = "<?xml version='1.0' encoding='utf-8'?><webmail><param name='action' value='update'/><param name='request' value='signature'/><param name='id_acct' value='" + acct.ID
                    //            + "'/><signature type='0' opt='1'><![CDATA[" + recEmailModel.RecEmailSignature + "]]></signature></webmail>";
                    //        result = checkMail.GetMessages(xmlString);
                    //        _userBAL.Rec_EmailMappingInsertUpdate(recEmailModel.RecConfigurationEmail, recEmailModel.RecIncomingMail,recEmailModel.RecConfigurationEmail,
                    //            recEmailModel.RecConfigurationPassword,recEmailModel.RecIncomingMailPort,recEmailModel.RecOutgoingMail,recEmailModel.RecOutgoingMailPort,recEmailModel.RecEmailSignature,"1",0, ProjectSession.LoggedInUserId);

                    //        ////This will Update TimeZone
                    //        xmlString = "<?xml version='1.0' encoding='utf-8'?><webmail><param name='action' value='update'/><param name='request' value='settings'/><param name='id_acct' value='" + acct.ID
                    //            + "'/><settings id_acct='" + acct.ID + "' msgs_per_page='10000' contacts_per_page='20' auto_checkmail_interval='0' allow_dhtml_editor='1' def_charset_out='65001' def_timezone='" + recEmailModel.RecDef_TimeZone + "' time_format='1' view_mode='1'><def_skin><![CDATA[AfterLogic]]></def_skin><def_lang><![CDATA[English]]></def_lang></settings></webmail>";
                    //        checkMail.GetMessages(xmlString);
                    //    }
                    //}

                    ////_userBAL.Rec_EmailMappingInsertUpdate(recEmailModel.RecConfigurationEmail, recEmailModel.RecIncomingMail,recEmailModel.RecConfigurationEmail,, ProjectSession.LoggedInUserId); 
                    #endregion

                    _userBAL.Rec_EmailMappingInsertUpdate(recEmailModel.RecConfigurationEmail, recEmailModel.RecIncomingMail, recEmailModel.RecConfigurationEmail,
                               recEmailModel.RecConfigurationPassword, recEmailModel.RecIncomingMailPort, recEmailModel.RecOutgoingMail, recEmailModel.RecOutgoingMailPort,
                               recEmailModel.RecEmailSignature, "1", 0, ProjectSession.LoggedInUserId, recEmailModel.RecDef_TimeZone);

                }

                #endregion

                Session[FormBot.Email.Constants.sessionAccount] = null;
                emailModel.Login = emailModel.ConfigurationEmail;

                // Email configuration not required (check if all fields are entered by user then only configure mail account)
                if (!string.IsNullOrWhiteSpace(emailModel.ConfigurationEmail) && !string.IsNullOrWhiteSpace(emailModel.Login) && !string.IsNullOrWhiteSpace(emailModel.ConfigurationPassword) && !string.IsNullOrWhiteSpace(emailModel.IncomingMail) && !string.IsNullOrWhiteSpace(emailModel.OutgoingMail))
                {
                    xmlString = "<?xml version='1.0' encoding='UTF-8'?><webmail><param name='action' value='login' /><param name='request' value='' /><param name='email'><![CDATA[" + emailModel.ConfigurationEmail
                        + "]]></param><param name='mail_inc_login'><![CDATA[" + emailModel.Login + "]]></param><param name='mail_inc_pass'><![CDATA[" + emailModel.ConfigurationPassword + "]]></param><param name='mail_inc_host'><![CDATA[" + emailModel.IncomingMail
                        + "]]></param><param name='mail_inc_port' value='" + emailModel.IncomingMailPort + "' /><param name='mail_protocol' value='0' /><param name='mail_out_host'><![CDATA[" + emailModel.OutgoingMail
                        + "]]></param><param name='mail_out_port' value='" + emailModel.OutgoingMailPort + "' /><param name='mail_out_auth' value='1' /><param name='sign_me' value='0' /><param name='language' /><param name='advanced_login' value='1' /></webmail>";
                    CheckMail checkMail = new CheckMail();
                    var result = checkMail.GetMessages(xmlString);
                    XDocument xDocument = XDocument.Parse(result.OuterXml);

                    var error = xDocument.Descendants("webmail").Elements("error").FirstOrDefault();
                    if (error != null && !string.IsNullOrWhiteSpace(Convert.ToString(error.Value)))
                    {
                        userView.lstUserDocument = _userBAL.GetUserUsersDocumentByUserId(userView.UserId);
                        this.ShowMessage(SystemEnums.MessageType.Error, error.Value, false);
                        return View("MyProfile", userView);
                    }
                    else
                    {
                        FormBot.Email.Account acct = Session[FormBot.Email.Constants.sessionAccount] as FormBot.Email.Account;
                        if (acct != null)
                        {
                            xmlString = "<?xml version='1.0' encoding='utf-8'?><webmail><param name='action' value='update'/><param name='request' value='signature'/><param name='id_acct' value='" + acct.ID
                                + "'/><signature type='0' opt='1'><![CDATA[" + emailModel.EmailSignature + "]]></signature></webmail>";
                            result = checkMail.GetMessages(xmlString);
                            _userBAL.EmailMappingInsertUpdate(acct.ID, ProjectSession.LoggedInUserId);
                        }
                    }
                } // Email configuration not required (add } here)                

                UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                UserManager.UserValidator = new UserValidator<ApplicationUser>(UserManager) { AllowOnlyAlphanumericUserNames = false };

                var userDetails = UserManager.FindById(userView.AspNetUserId);

                if (userDetails != null)
                {
                    userDetails.Email = userView.Email;
                    userDetails.UserName = userView.UserName.Replace(" ", "");
                    PasswordHasher hasher = new PasswordHasher();
                    if (userView.Password != null)
                    {
                        userDetails.PasswordHash = hasher.HashPassword(userView.Password);
                    }
                }

                await UserManager.UpdateAsync(userDetails);
                userView.ModifiedBy = ProjectSession.LoggedInUserId;
                userView.ModifiedDate = DateTime.Today;
                if (userView.Signature != null && ProjectSession.UserTypeId == 2)
                {
                    userView.Logo = ProjectSession.LoggedInUserId + "/" + userView.Signature;
                    ProjectSession.Logo = userView.Logo;
                }
                else
                {
                    userView.Logo = "Images/logo.png";
                    ProjectSession.Logo = userView.Logo;
                }

                userView.IsFirstLogin = false;
                if (userView.AddressID == 2)
                {
                    userView.IsPostalAddress = true;
                }
                else
                {
                    userView.IsPostalAddress = false;
                }

                if (userView.WholesalerIsPostalAddress == 2)
                {
                    userView.WholesalerIsPostalAddress = 1;
                }
                else
                {
                    userView.WholesalerIsPostalAddress = 0;
                }

                if (userView.strFromDate != null)
                {
                    userView.FromDate = Convert.ToDateTime(userView.strFromDate);
                }

                if (userView.strToDate != null)
                {
                    userView.ToDate = Convert.ToDateTime(userView.strToDate);
                }

                if (userView.UserTypeID == 2)
                {
                    if (userView.hiddenTheme != 0)
                    {
                        ProjectSession.Theme = ((SystemEnums.Theme)userView.hiddenTheme).ToString();
                        userView.Theme = userView.hiddenTheme;
                    }
                }
                else
                {
                    if (ProjectSession.Theme != "")
                    {
                        userView.Theme = Convert.ToInt32((SystemEnums.UserStatus)Enum.Parse(typeof(SystemEnums.Theme), ProjectSession.Theme).GetHashCode());
                    }
                    else
                    {
                        userView.Theme = 1;
                    }
                }

                if (userView.CompanyABN != null)
                {
                    userView.CompanyABN = userView.CompanyABN.Replace(" ", "");
                }

                int chkSTC = _userBAL.GetISSTCforSCA(ProjectSession.LoggedInUserId);
                FormBot.Entity.User uvuser = TempData[userView.UserId.ToString()] as FormBot.Entity.User;
                userView.RecPassword = userView.RecPassword != null ? userView.RecPassword : uvuser.RecPassword;
                //DataTable dt = _userBAL.GetRecDataFromUserId(userView.UserId);
                //userView.RecUserName = dt.Rows[0]["RecUserName"].ToString();
                //userView.RecPassword = dt.Rows[0]["RecPassword"].ToString();
                _userBAL.Create(userView);
                //if (userView.UserTypeID == 4)
                //{
                //    string entityName = strGetEntityName(userView.CompanyABN);
                //    _userBAL.UpdateEntityName(userView.UserId, entityName);
                //}
                if (userView.FileNamesCreate != null)
                {
                    foreach (var files in userView.FileNamesCreate)
                    {
                        userView.FileName = files;
                        _userBAL.InsertUserDocument(ProjectSession.LoggedInUserId, files);
                    }
                }

                // Login SCA wants to join SE
                if (ProjectSession.UserTypeId == 4 && userView.IsSTC == true && chkSTC.Equals(0))
                {
                    int seID = _userBAL.InsertSCAasSE(userView);
                    userView.lstUserDocument = _userBAL.GetUserUsersDocumentByUserId(ProjectSession.LoggedInUserId);
                    for (int i = 0; i < userView.lstUserDocument.Count; i++)
                    {
                        _userBAL.InsertUserDocument(seID, userView.lstUserDocument[i].DocumentPath);
                    }

                    string destinationDirectory = ProjectSession.ProofDocumentsURL + "\\" + "UserDocuments" + "\\" + ProjectSession.LoggedInUserId;

                    // SCA want join as SE userID
                    string seDestinationDirectory = ProjectSession.ProofDocumentsURL + "\\" + "UserDocuments" + "\\" + seID;

                    if (userView.FileNamesCreate != null || userView.Signature != null)
                    {
                        try
                        {
                            if (userView.IsSTC == true && ProjectSession.UserTypeId == 4)
                            {
                                // SCA want join as SE
                                Directory.CreateDirectory(seDestinationDirectory);
                                DirectoryInfo dir = new DirectoryInfo(destinationDirectory);
                                FileInfo[] files = dir.GetFiles();
                                foreach (FileInfo file in files)
                                {
                                    // Create the path to the new copy of the file.
                                    string temppath = Path.Combine(seDestinationDirectory, file.Name);

                                    // Copy the file.
                                    file.CopyTo(temppath, false);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                }

                if (userView.UserTypeID == 7)
                {
                    if (userView.ProofFileNamesCreate != null)
                    {
                        foreach (var files in userView.ProofFileNamesCreate)
                        {
                            //userView.FileName = files;
                            if (files.ProofDocumentType > 0)
                            {
                                _userBAL.InsertUserDocument(userView.UserId, files.FileName, files.ProofDocumentType, files.DocLoc);
                            }
                        }
                    }
                }

                this.ShowMessage(SystemEnums.MessageType.Success, "User profile details has been saved successfully.", true);
                ProjectSession.Theme = ((SystemEnums.Theme)userView.Theme).ToString();
                ProjectSession.LoggedInName = userView.FirstName + " " + userView.LastName;

                #region Email configuration not required
                //} 
                #endregion

                var dsMenuActions = _login.GetMenuActionByRoleId(ProjectSession.RoleId);
                HttpContext.Response.Cookies.Set(new HttpCookie("menuname") { Value = "createnew" });
                List<MenuActionView> MenuActionList = DBClient.DataTableToList<MenuActionView>(dsMenuActions.Tables[0]);
                MenuActionView menuActionView = MenuActionList.FirstOrDefault();
                if (menuActionView.Controller.ToLower() == "email")
                    return RedirectToAction(menuActionView.Action, menuActionView.Controller, new { area = "Email" });
                else
                    return RedirectToAction(menuActionView.Action, menuActionView.Controller, new { area = "" });
                //return RedirectToAction("Index", "User");
            }
            else
            {
                //var designRole = from SystemEnums.PostalAddressType s in Enum.GetValues(typeof(SystemEnums.PostalAddressType))
                //                 select new { ID = s.GetHashCode(), Name = s.ToString().Replace('_', ' ') };
                ViewBag.PostalAddressType = new SelectList(EnumExtensions.GetPostalAddressEnumList(), "ID", "Name");
                userView.lstUserDocument = _userBAL.GetUserUsersDocumentByUserId(userView.UserId);

                userView.RecEmailSignup = new Entity.Email.RecEmailSignup();
                RecEmailConfiguration(userView);

                if (userView.RecEmailSignup.RecConfigurationPassword != null)
                {
                    userView.RecEmailSignup.RecConfigurationPassword = Utils.DecodePassword(userView.RecEmailSignup.RecConfigurationPassword);
                }

                userView.EmailSignup = new Entity.Email.EmailSignup();
                EmailConfiguration(userView);

                if (userView.EmailSignup.ConfigurationPassword != null)
                {
                    userView.EmailSignup.ConfigurationPassword = Utils.DecodePassword(userView.EmailSignup.ConfigurationPassword);
                }

                DateTime fromDate = Convert.ToDateTime(userView.FromDate);
                DateTime toDate = Convert.ToDateTime(userView.ToDate);
                userView.strFromDate = fromDate.ToString("yyyy-MM-dd");
                userView.strToDate = toDate.ToString("yyyy-MM-dd");
                this.ShowMessage(SystemEnums.MessageType.Error, String.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage + " " + v.Exception)), true);
                return this.View("MyProfile", userView);
            }
        }

        /// <summary>
        /// Sends the request for solar sub contractor.
        /// </summary>
        /// <param name="CompanyABN">The company abn.</param>
        /// <returns>Returns Send Request For Solar Sub Contractor</returns>
        [AllowAnonymous]
        public JsonResult SendRequestForSolarSubContractor(string CompanyABN)
        {
            FormBot.Entity.User userView = new Entity.User();
            userView.SolarCompanyId = ProjectSession.SolarCompanyId;
            userView.UserId = _userBAL.GetUserIdByCompanyABN(CompanyABN);
            userView.CompanyABN = CompanyABN;
            object sscID = _userBAL.SSCIDExistINSCASSCMapping(userView.UserId, ProjectSession.LoggedInUserId);
            if (sscID.Equals(0))
            {
                if (userView.UserId > 0)
                {
                    _userBAL.CreateSendRequestForSolarSubContractor(userView);
                    //SendEmailOnSendRequestToSSC(userView);

                    var usr = _userBAL.GetUserById(ProjectSession.LoggedInUserId);
                    FormBot.Entity.User uv = DBClient.DataTableToList<FormBot.Entity.User>(usr.Tables[0]).FirstOrDefault();
                    SolarCompany solarCompany = _solarCompanyBAL.GetSolarCompanyBySolarCompanyID(uv.SolarCompanyId);
                    //emailInfo.SolarCompanyDetails = "Company Name: " + solarCompany.CompanyName + "<br/>" + "Company ABN: " + solarCompany.CompanyABN + "<br/>" + "Contact: " + uv.Phone;

                    var sscuser = _userBAL.GetUserById(userView.UserId);
                    FormBot.Entity.User objSscUser = DBClient.DataTableToList<FormBot.Entity.User>(sscuser.Tables[0]).FirstOrDefault();

                    EmailInfo emailInfo = new EmailInfo();
                    emailInfo.TemplateID = 11;
                    emailInfo.FirstName = objSscUser.FirstName;
                    emailInfo.LastName = objSscUser.LastName;
                    emailInfo.SolarCompanyDetails = "Company Name: " + solarCompany.CompanyName + "<br/>" + "Company ABN: " + solarCompany.CompanyABN + "<br/>" + "Contact: " + uv.Phone;
                    _emailBAL.ComposeAndSendEmail(emailInfo, objSscUser.Email);
                    //EmailTemplate eTemplate = _emailBAL.PrepareEmailTemplate(11, uv, objSscUser, null, solarCompany);
                    //_emailBAL.GeneralComposeAndSendMail(eTemplate, objSscUser.Email);
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json("Exist", JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Signs up.
        /// </summary>
        /// <param name="userView">The user view.</param>
        /// <returns>REturns Signup</returns>
        [HttpPost]
        public async Task<ActionResult> SignUp(FormBot.Entity.User userView)
        {
            ModelState.Remove("LoginCompanyName");
            ModelState.Remove("Password");
            ModelState.Remove("UserName");

            RequiredValidationField(userView);
            if (!userView.IsWholeSaler)
                RemoveWholeSalerValidation();
            if (userView.UserTypeID != 2 || (userView.UserTypeID == 2 && userView.UsageType != 3))
            {
                RemoveSAASUserValidation();
            }
            if (ModelState.IsValid)
            {
                //FormBot.Entity.User uv = Session["UserView"] as FormBot.Entity.User;

                //var designRole = from SystemEnums.PostalAddressType s in Enum.GetValues(typeof(SystemEnums.PostalAddressType))
                //                 select new { ID = s.GetHashCode(), Name = s.ToString().Replace('_', ' ') };
                ViewBag.PostalAddressType = new SelectList(EnumExtensions.GetPostalAddressEnumList(), "ID", "Name");
                userView.EmailSignup = new Entity.Email.EmailSignup();

                UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                UserManager.UserValidator = new UserValidator<ApplicationUser>(UserManager) { AllowOnlyAlphanumericUserNames = false };

                var userDetails = UserManager.FindById(userView.AspNetUserId);

                if (userDetails != null)
                {
                    userDetails.Email = userView.Email;
                    userDetails.UserName = userView.UserName.Replace(" ", "");
                    PasswordHasher hasher = new PasswordHasher();
                    if (userView.Password != null)
                    {
                        userDetails.PasswordHash = hasher.HashPassword(userView.Password);
                    }
                }

                await UserManager.UpdateAsync(userDetails);
                userView.ModifiedDate = DateTime.Today;
                userView.Logo = "Images/logo.png";
                if (userView.strFromDate != null)
                {
                    userView.FromDate = Convert.ToDateTime(userView.strFromDate);
                }

                if (userView.strToDate != null)
                {
                    userView.ToDate = Convert.ToDateTime(userView.strToDate);
                }

                if (userView.AddressID == 2)
                {
                    userView.IsPostalAddress = true;
                }
                else
                {
                    userView.IsPostalAddress = false;
                }

                if (userView.WholesalerIsPostalAddress == 2)
                {
                    userView.WholesalerIsPostalAddress = 1;
                }
                else
                {
                    userView.WholesalerIsPostalAddress = 0;
                }

                if (userView.IsSTC == false && userView.UserTypeID == 4)
                {
                    userView.CECAccreditationNumber = null;
                    userView.ElectricalContractorsLicenseNumber = null;
                    userView.CECDesignerNumber = null;
                }

                if (userView.CompanyABN != null)
                {
                    userView.CompanyABN = userView.CompanyABN.Replace(" ", "");
                }

                int userID = _userBAL.Create(userView);
                //if (userView.UserTypeID == 4)
                //{
                //    string entityName = strGetEntityName(userView.CompanyABN);
                //    _userBAL.UpdateEntityName(userID, entityName);
                //}
                if (userView.FileNamesCreate != null)
                {
                    foreach (var files in userView.FileNamesCreate)
                    {
                        userView.FileName = files;
                        _userBAL.InsertUserDocument(userView.UserId, files);
                    }
                }

                _userBAL.InsertStatus(userView.UserId);
                this.ShowMessage(SystemEnums.MessageType.Success, "User status has been saved successfully.", true);
                if (userView.UserTypeID == 4)
                {
                    return RedirectPermanent(userView.ResellerUrl);
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            else
            {
                this.ShowMessage(SystemEnums.MessageType.Error, String.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage + " " + v.Exception)), true);
                return RedirectToAction("SignUp", "Account", new { id = EncryptionDecryption.GetEncrypt("id=" + userView.UserId) });
            }
        }

        /// <summary>
        /// Post method for view user
        /// </summary>
        /// <param name="userView">The user view.</param>
        /// <returns>View Detail</returns>
        [HttpPost]
        public async Task<ActionResult> UpdateUserXeroCheck(FormBot.Entity.User userView)
        {
            ModelState.Remove("userView.LoginCompanyName");
            ModelState.Remove("userView.Password");
            ModelState.Remove("userView.UserName");
            ModelState.Remove("userView.ResellerID");
            ModelState.Remove("userView.ClientCodePrefix");
            ModelState.Remove("userView.AccountName");
            ModelState.Remove("userView.SWHLicenseNumber");

            if (userView.IsWholeSaler)
            {
                ModelState.Remove("userView.FirstName");
                ModelState.Remove("userView.LastName");
                ModelState.Remove("userView.Email");
                ModelState.Remove("userView.Phone");
                ModelState.Remove("userView.CompanyABN");
                ModelState.Remove("userView.CompanyName");
                ModelState.Remove("userView.UnitNumber");
                ModelState.Remove("userView.UnitTypeID");
                ModelState.Remove("userView.StreetNumber");
                ModelState.Remove("userView.StreetName");
                ModelState.Remove("userView.StreetTypeID");
                ModelState.Remove("userView.PostalAddressID");
                ModelState.Remove("userView.PostalDeliveryNumber");
                ModelState.Remove("userView.State");
                ModelState.Remove("userView.Town");
                ModelState.Remove("userView.PostCode");
                ModelState.Remove("userView.BSB");
                ModelState.Remove("userView.AccountNumber");
            }
            else
            {
                ModelState.Remove("userView.WholesalerFirstName");
                ModelState.Remove("userView.WholesalerLastName");
                ModelState.Remove("userView.WholesalerCompanyABN");
                ModelState.Remove("userView.WholesalerCompanyName");
                ModelState.Remove("userView.WholesalerUnitNumber");
                ModelState.Remove("userView.WholesalerUnitTypeID");
                ModelState.Remove("userView.WholesalerStreetNumber");
                ModelState.Remove("userView.WholesalerStreetName");
                ModelState.Remove("userView.WholesalerStreetTypeID");
                ModelState.Remove("userView.WholesalerPostalAddressID");
                ModelState.Remove("userView.WholesalerPostalDeliveryNumber");
                ModelState.Remove("userView.WholesalerIsPostalAddress");
                ModelState.Remove("userView.WholesalerState");
                ModelState.Remove("userView.WholesalerTown");
                ModelState.Remove("userView.WholesalerPostCode");
            }

            if (userView.AddressID == 1)
            {
                // physical address
                ModelState.Remove("userView.PostalAddressID");
                ModelState.Remove("userView.PostalDeliveryNumber");
                userView.IsPostalAddress = false;
            }
            else
            {
                //postal address
                ModelState.Remove("userView.UnitNumber");
                ModelState.Remove("userView.UnitTypeID");
                ModelState.Remove("userView.StreetNumber");
                ModelState.Remove("userView.StreetName");
                ModelState.Remove("userView.StreetTypeID");
                userView.IsPostalAddress = true;
            }

            if (userView.WholesalerIsPostalAddress == 1)
            {
                // physical address
                ModelState.Remove("userView.WholesalerPostalAddressID");
                ModelState.Remove("userView.WholesalerPostalDeliveryNumber");
                userView.IsPostalAddress = false;
            }
            else
            {
                //postal address
                ModelState.Remove("userView.WholesalerUnitNumber");
                ModelState.Remove("userView.WholesalerUnitTypeID");
                ModelState.Remove("userView.WholesalerStreetNumber");
                ModelState.Remove("userView.WholesalerStreetName");
                ModelState.Remove("userView.WholesalerStreetTypeID");
                userView.IsPostalAddress = true;
            }

            if (userView.UnitNumber != null && userView.UnitTypeID != 0)
            {
                ModelState.Remove("userView.StreetNumber");
            }
            if (userView.WholesalerUnitNumber != null && userView.WholesalerUnitTypeID != 0)
            {
                ModelState.Remove("userView.WholesalerStreetNumber");
            }

            if (userView.StreetNumber != null)
            {
                ModelState.Remove("userView.UnitNumber");
                ModelState.Remove("userView.UnitTypeID");
            }
            if (userView.WholesalerStreetNumber != null)
            {
                ModelState.Remove("userView.WholesalerUnitNumber");
                ModelState.Remove("userView.WholesalerUnitTypeID");
            }

            if (string.IsNullOrWhiteSpace(userView.CustomCompanyName))
                ModelState.Remove("userView.CustomCompanyName");

            if (userView.UserTypeID == 2)
                ModelState.Remove("userView.SolarCompanyId");

            //RequiredValidationField(userView);
            RemoveInstallerDesignerValidation();

            if (ModelState.IsValid)
            {
                //FormBot.Entity.User uv = Session["UserView"] as FormBot.Entity.User;
                FormBot.Entity.User uv = TempData[userView.UserId.ToString()] as FormBot.Entity.User;

                ViewBag.PostalAddressType = new SelectList(EnumExtensions.GetPostalAddressEnumList(), "ID", "Name");
                userView.EmailSignup = new Entity.Email.EmailSignup();

                if (!userView.IsWholeSaler)
                {
                    UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                    UserManager.UserValidator = new UserValidator<ApplicationUser>(UserManager) { AllowOnlyAlphanumericUserNames = false };
                    var userDetails = UserManager.FindById(userView.AspNetUserId);

                    if (userDetails != null)
                    {
                        userDetails.Email = userView.Email;
                    }

                    await UserManager.UpdateAsync(userDetails);
                }
                userView.ModifiedBy = ProjectSession.LoggedInUserId;
                userView.ModifiedDate = DateTime.Today;

                if (userView.AddressID == 2)
                {
                    userView.IsPostalAddress = true;
                }
                else
                {
                    userView.IsPostalAddress = false;
                }

                if (userView.WholesalerIsPostalAddress == 2)
                {
                    userView.WholesalerIsPostalAddress = 1;
                }
                else
                {
                    userView.WholesalerIsPostalAddress = 0;
                }

                if (userView.CompanyABN != null)
                {
                    userView.CompanyABN = userView.CompanyABN.Replace(" ", "");
                }

                int userID = _userBAL.UpdateSolarCompanyUsersCheckInXero(userView);

                if (userView.Status != 1)
                {
                    ////EmailTemplate eTemplate = null;
                    EmailInfo emailInfo = new EmailInfo();
                    emailInfo.FirstName = userView.FirstName;
                    emailInfo.LastName = userView.LastName;
                    emailInfo.Details = ComplianceCheckDetail(uv, userView);

                    if (userView.Status == 2)
                    {
                        emailInfo.TemplateID = 3;
                    }
                    else if (userView.Status == 3)
                    {
                        emailInfo.TemplateID = 4;
                    }
                    else
                    {
                        emailInfo.TemplateID = 22;
                        emailInfo.LoginLink = userView.UserTypeID == 7 ? ProjectSession.LoginLink + "Account/GreenbotSignUp/" + userView.Id : ProjectSession.LoginLink + "Account/ResellerSignUp/" + userView.LoginCompanyName + "/" + userView.Id;
                    }
                    _emailBAL.ComposeAndSendEmail(emailInfo, userView.Email);
                }

                if (!userView.IsWholeSaler)
                {
                    string strEmailConfigureMsg = string.Empty;
                    strEmailConfigureMsg = !ProjectSession.IsUserEmailAccountConfigured ? "(Can not send mail to user because email account is not configured)" : string.Empty;
                    this.ShowMessage(SystemEnums.MessageType.Success, "User status has been saved successfully.", true);
                    return RedirectToAction("ViewDetail", "User", new { id = EncryptionDecryption.GetEncrypt("id=" + userView.UserId) });
                }
                else
                {
                    this.ShowMessage(SystemEnums.MessageType.Success, "Contact has been checked.", true);
                    return RedirectToAction("Index", "STCInvoice");
                }
            }
            else
            {
                userView.lstUserDocument = _userBAL.GetUserUsersDocumentByUserId(userView.UserId);
                this.ShowMessage(SystemEnums.MessageType.Error, String.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage + " " + v.Exception)), true);
                if (!userView.IsWholeSaler)
                {
                    return RedirectToAction("ViewDetail", "User", new { id = EncryptionDecryption.GetEncrypt("id=" + userView.UserId) });
                }
                else
                {
                    return RedirectToAction("Index", "STCInvoice");
                }
            }
        }

        #endregion

        #region Method
        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="excludePostBoxFlag">if set to <c>true</c> [exclude post box flag].</param>
        /// <param name="q">The q.</param>
        /// <returns>REyturns Process Request</returns>
        [HttpGet]
        public JsonResult ProcessRequest(bool excludePostBoxFlag, string q)
        {
            string ret = string.Empty;
            var webRequest = System.Net.WebRequest.Create(string.Format("https://auspost.com.au/api/postcode/search.json?q=" + q + "&excludePostBoxFlag=flase"));

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

        [HttpGet]
        public JsonResult GetEntityName(string companyABN)
        {
            string entityName = string.Empty;
            string abnURL = ProjectConfiguration.GetCompanyABNSearchLink + companyABN;
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
                            if (cell.InnerText.ToLower() == "entity name:")
                            {
                                var td = cell.ParentNode.SelectNodes("./td");
                                string tdValue = td[0].InnerText;
                                entityName = tdValue.Replace("\r\n", "").Trim();
                                if (entityName.Length > 0)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                return Json(new { success = true, data = entityName }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _log.Log(SystemEnums.Severity.Error, "companyABN = "+ companyABN + " GetEntityName = " + ex.Message.ToString());
                return Json(new { success = false, data = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Gets the company abn.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Returns Company ABN</returns>
        [HttpGet]
        public JsonResult GetCompanyABN(string id)
        {
            try
            {
                string url = ProjectSession.ABNLink + "?searchString=" + id + "&includeHistoricalDetails=Y&authenticationGuid=" + ProjectSession.AuthenticationGuid;
                AddLog(url);
                // Create the web request  
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

                // Get response  
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    AddLog(JsonConvert.SerializeObject(response));
                    // Get the response stream  
                    StreamReader reader = new StreamReader(response.GetResponseStream());

                    // Read the whole contents and return as a string  
                    string result = reader.ReadToEnd();
                    AddLog(result);
                    result = result.Replace("<?xml version='1.0' encoding='UTF-8'?>", string.Empty);
                    AddLog("before parse - " + result);
                    var doc = XDocument.Parse(result);
                    AddLog("1");
                    XmlDocument xDxDoc = new XmlDocument();
                    xDxDoc.LoadXml(result);
                    AddLog("2");
                    XmlNodeList companyNameList = xDxDoc.GetElementsByTagName("businessName");
                    XmlNodeList mainNameList = xDxDoc.GetElementsByTagName("mainName");
                    XmlNodeList mainTradingNameList = xDxDoc.GetElementsByTagName("mainTradingName");
                    XmlNodeList otherTradingNameList = xDxDoc.GetElementsByTagName("otherTradingName");
                    XmlNodeList otherlegalNameList = xDxDoc.GetElementsByTagName("legalName");
                    AddLog("3");
                    if (companyNameList.Count > 0 || mainNameList.Count > 0 || mainTradingNameList.Count > 0 || otherTradingNameList.Count > 0 || otherlegalNameList.Count > 0)
                    {
                        AddLog("4");
                        List<FormBot.Entity.User> nodes = new List<FormBot.Entity.User>();
                        foreach (XmlNode node in companyNameList)
                        {
                            FormBot.Entity.User user = new FormBot.Entity.User();
                            user.CompanyName = Convert.ToString(node.ChildNodes.Item(0).FirstChild.InnerText);
                            user.strFromDate = Convert.ToString(node.ChildNodes.Item(1).FirstChild.InnerText);
                            int Cnt = node.ChildNodes.Count;
                            if (Cnt > 2)
                            {
                                if (node.ChildNodes.Item(2).FirstChild.InnerText != "0001-01-01")
                                {
                                    user.strToDate = Convert.ToString(node.ChildNodes.Item(2).FirstChild.InnerText);
                                }

                            }

                            nodes.Add(user);
                        }
                        AddLog("5");
                        foreach (XmlNode node in mainNameList)
                        {
                            FormBot.Entity.User user = new FormBot.Entity.User();
                            user.CompanyName = Convert.ToString(node.ChildNodes.Item(0).FirstChild.InnerText);
                            user.strFromDate = Convert.ToString(node.ChildNodes.Item(1).FirstChild.InnerText);
                            int Cnt = node.ChildNodes.Count;
                            if (Cnt > 2)
                            {
                                if (node.ChildNodes.Item(2).FirstChild.InnerText != "0001-01-01")
                                {
                                    user.strToDate = Convert.ToString(node.ChildNodes.Item(2).FirstChild.InnerText);
                                }

                            }

                            nodes.Add(user);
                        }
                        AddLog("6");
                        foreach (XmlNode node in mainTradingNameList)
                        {
                            FormBot.Entity.User user = new FormBot.Entity.User();
                            user.CompanyName = Convert.ToString(node.ChildNodes.Item(0).FirstChild.InnerText);
                            user.strFromDate = Convert.ToString(node.ChildNodes.Item(1).FirstChild.InnerText);
                            int Cnt = node.ChildNodes.Count;
                            if (Cnt > 2)
                            {
                                if (node.ChildNodes.Item(2).FirstChild.InnerText != "0001-01-01")
                                {
                                    user.strToDate = Convert.ToString(node.ChildNodes.Item(2).FirstChild.InnerText);
                                }

                            }

                            nodes.Add(user);
                        }
                        AddLog("7");
                        foreach (XmlNode node in otherTradingNameList)
                        {
                            FormBot.Entity.User user = new FormBot.Entity.User();
                            user.CompanyName = Convert.ToString(node.ChildNodes.Item(0).FirstChild.InnerText);
                            user.strFromDate = Convert.ToString(node.ChildNodes.Item(1).FirstChild.InnerText);
                            int Cnt = node.ChildNodes.Count;
                            if (Cnt > 2)
                            {
                                if (node.ChildNodes.Item(2).FirstChild.InnerText != "0001-01-01")
                                {
                                    user.strToDate = Convert.ToString(node.ChildNodes.Item(2).FirstChild.InnerText);
                                }
                            }

                            nodes.Add(user);
                        }
                        AddLog("8");
                        foreach (XmlNode node in otherlegalNameList)
                        {
                            FormBot.Entity.User user = new FormBot.Entity.User();
                            user.CompanyName = node.ChildNodes.Item(0).FirstChild!=null? Convert.ToString(node.ChildNodes.Item(0).FirstChild.InnerText):"";
                            if (node.ChildNodes.Item(1).FirstChild != null)
                            {
                                user.CompanyName = user.CompanyName + " " + Convert.ToString(node.ChildNodes.Item(1).FirstChild.InnerText);
                            }

                            if (node.ChildNodes.Item(2).FirstChild != null)
                            {
                                user.CompanyName = user.CompanyName + " " + Convert.ToString(node.ChildNodes.Item(2).FirstChild.InnerText);
                            }

                            user.strFromDate = Convert.ToString(node.ChildNodes.Item(3).FirstChild.InnerText);
                            int Cnt = node.ChildNodes.Count;
                            if (Cnt > 4)
                            {
                                if (node.ChildNodes.Item(4).FirstChild.InnerText != "0001-01-01")
                                {
                                    user.strToDate = Convert.ToString(node.ChildNodes.Item(4).FirstChild.InnerText);
                                }
                            }

                            nodes.Add(user);
                        }
                        AddLog("9");
                        var jsonResult = Json(nodes, JsonRequestBehavior.AllowGet);
                        jsonResult.MaxJsonLength = int.MaxValue;
                        return jsonResult;

                        // return this.Json((serializedResult), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        int cnt = companyNameList.Count;
                        AddLog("4.1 :: cnt == " + cnt);
                        return this.Json(cnt, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogException(SystemEnums.Severity.Debug.ToString() + "   Error", ex);
                return this.Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        public void AddLog(string log)
        {
            //if (System.Configuration.ConfigurationManager.AppSettings["CustomLogOn"].ToString() == "1")
            //{
            //    _log.LogException(SystemEnums.Severity.Debug, log);

            //}
        }

        /// <summary>
        /// Deletes the user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Delete User</returns>
        public ActionResult DeleteUser(string userId)
        {
            //try
            //{
            int uID = 0;
            if (!string.IsNullOrWhiteSpace(userId))
            {
                int.TryParse(QueryString.GetValueFromQueryString(userId, "id"), out uID);
            }

            var dsUsers = _userBAL.GetUserById(uID);
            FormBot.Entity.User userToDelete = DBClient.DataTableToList<FormBot.Entity.User>(dsUsers.Tables[0]).FirstOrDefault();

            _userBAL.DeleteUser(uID);
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

        /// <summary>
        /// Change status of SE for selected SolarCompany.
        /// </summary>
        /// <param name="solarcompanyId">The solarcompany identifier.</param>
        /// <param name="status">if set to <c>true</c> [status].</param>
        /// <returns>
        /// Returns Status for Request Is Accepted or Rejected.
        /// </returns>
        public ActionResult AcceptRejectSolarCompanyRequest(string solarcompanyId, bool status)
        {
            //try
            //{
            int st = status ? 2 : 3;
            FormBot.Entity.User scauser = _userBAL.GetUserBySolarCompanyId(Convert.ToInt32(solarcompanyId));

            if (ProjectSession.UserTypeId == 7)
            {
                _userBAL.SEAcceptRejectSolarCompanyRequest(ProjectSession.LoggedInUserId, Convert.ToInt32(solarcompanyId), st);

                EmailInfo emailInfo = new EmailInfo();
                emailInfo.TemplateID = status ? 7 : 8;
                emailInfo.FirstName = scauser.FirstName;
                emailInfo.LastName = scauser.LastName;
                emailInfo.SolarElectrician = ProjectSession.LoggedInName;
                _emailBAL.ComposeAndSendEmail(emailInfo, scauser.Email);
                //EmailTemplate eTemplate = _emailBAL.PrepareEmailTemplate(status ? 7 : 8, null, scauser, null, null);
                //_emailBAL.GeneralComposeAndSendMail(eTemplate, scauser.Email);
            }
            else
            {
                _userBAL.SSCAcceptRejectSolarCompanyRequest(ProjectSession.LoggedInUserId, Convert.ToInt32(solarcompanyId), st);
            }

            return this.Json(new { success = true });
            //}
            //catch (Exception ex)
            //{
            //    return this.Json(new { success = false, errormessage = ex.Message });
            //}
        }

        /// <summary>
        /// Requireds the validation field.
        /// </summary>
        /// <param name="userView">The user view.</param>
        public void RequiredValidationField(FormBot.Entity.User userView)
        {
            int UserTypeId = userView.UserTypeID;

            //if (UserTypeId != 10)
            //    ModelState.Remove("SWHLicenseNumber");

            if (UserTypeId != 4)
            {
                ModelState.Remove("CustomCompanyName");
            }
            if (UserTypeId != 2 || UserTypeId != 5)
            {
                ModelState.Remove("ClientCodePrefix");
            }
            if (UserTypeId != 4)
                ModelState.Remove("CustomCompanyName");
            if (UserTypeId == 3 || UserTypeId == 1 || UserTypeId == 5)
            {
                ModelState.Remove("SolarCompanyId");
                ModelState.Remove("ResellerID");
                ModelState.Remove("CompanyABN");
                ModelState.Remove("CompanyName");
                ModelState.Remove("FromDate");
                ModelState.Remove("ToDate");
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
                ModelState.Remove("BSB");
                ModelState.Remove("AccountNumber");
                ModelState.Remove("AccountName");
                ModelState.Remove("LoginCompanyName");
                ModelState.Remove("SubContractorID");
                ModelState.Remove("FCOGroupId");
                ModelState.Remove("SEDesignRoleId");
            }

            if (UserTypeId == 1 || UserTypeId == 5)
            {
                ModelState.Remove("lstFCOGroup1");
            }

            if (UserTypeId == 2)
            {
                ModelState.Remove("SolarCompanyId");
                ModelState.Remove("ResellerID");
                ModelState.Remove("SubContractorID");
                ModelState.Remove("SEDesignRoleId");
            }

            if (UserTypeId == 6)
            {
                ModelState.Remove("SolarCompanyId");
                ModelState.Remove("ResellerID");
                ModelState.Remove("LoginCompanyName");
                ModelState.Remove("SubContractorID");
                ModelState.Remove("FCOGroupId");
                ModelState.Remove("SEDesignRoleId");
                ModelState.Remove("lstFCOGroup1");
            }

            if (UserTypeId == 4)
            {
                ModelState.Remove("SolarCompanyId");
                ModelState.Remove("LoginCompanyName");
                ModelState.Remove("SubContractorID");
                ModelState.Remove("FCOGroupId");
                ModelState.Remove("SEDesignRoleId");
                ModelState.Remove("lstFCOGroup1");
            }

            if (UserTypeId == 8)
            {
                ModelState.Remove("ResellerID");
                ModelState.Remove("UnitTypeID");
                ModelState.Remove("UnitNumber");
                ModelState.Remove("StreetNumber");
                ModelState.Remove("StreetName");
                ModelState.Remove("StreetTypeID");
                ModelState.Remove("Town");
                ModelState.Remove("State");
                ModelState.Remove("PostCode");
                ModelState.Remove("CompanyABN");
                ModelState.Remove("CompanyName");
                ModelState.Remove("FromDate");
                ModelState.Remove("ToDate");
                ModelState.Remove("LoginCompanyName");
                ModelState.Remove("FCOGroupId");
                ModelState.Remove("SEDesignRoleId");
                ModelState.Remove("lstFCOGroup1");
                ModelState.Remove("BSB");
                ModelState.Remove("AccountNumber");
                ModelState.Remove("AccountName");
            }

            if (UserTypeId == 9)
            {
                ModelState.Remove("ResellerID");
                ModelState.Remove("UnitTypeID");
                ModelState.Remove("UnitNumber");
                ModelState.Remove("StreetNumber");
                ModelState.Remove("StreetName");
                ModelState.Remove("StreetTypeID");
                ModelState.Remove("Town");
                ModelState.Remove("State");
                ModelState.Remove("PostCode");
                ModelState.Remove("CompanyABN");
                ModelState.Remove("CompanyName");
                ModelState.Remove("FromDate");
                ModelState.Remove("ToDate");
                ModelState.Remove("LoginCompanyName");
                ModelState.Remove("SubContractorID");
                ModelState.Remove("FCOGroupId");
                ModelState.Remove("SEDesignRoleId");
                ModelState.Remove("lstFCOGroup1");
                ModelState.Remove("SolarCompanyId");
            }

            if (UserTypeId == 7)
            {
                ModelState.Remove("ResellerID");
                ModelState.Remove("LoginCompanyName");
                ModelState.Remove("SolarCompanyId");
                ModelState.Remove("SubContractorID");
                ModelState.Remove("FCOGroupId");
                ModelState.Remove("SEDesignRoleId");
                ModelState.Remove("lstFCOGroup1");
                ModelState.Remove("BSB");
                ModelState.Remove("AccountNumber");
                ModelState.Remove("AccountName");

                if (userView.IsPVDUser == true && userView.IsSWHUser == false)
                {
                    ModelState.Remove("ElectricalContractorsLicenseNumber");
                }
                if (userView.IsPVDUser == false && userView.IsSWHUser == true)
                {
                    ModelState.Remove("CECAccreditationNumber");
                    ModelState.Remove("CECDesignerNumber");
                    ModelState.Remove("SEDesigner");
                }
            }

            if (userView.AddressID == 2)
            {
                ModelState.Remove("UnitNumber");
                ModelState.Remove("UnitTypeID");
                ModelState.Remove("StreetNumber");
                ModelState.Remove("StreetName");
                ModelState.Remove("StreetTypeID");
                ModelState.Remove("lstFCOGroup1");
            }

            if (userView.WholesalerIsPostalAddress == 2)
            {
                ModelState.Remove("WholesalerUnitNumber");
                ModelState.Remove("WholesalerUnitTypeID");
                ModelState.Remove("WholesalerStreetNumber");
                ModelState.Remove("WholesalerStreetName");
                ModelState.Remove("WholesalerStreetTypeID");
            }
            if(userView.InvoicerAddressID == 2)
            {
                ModelState.Remove("InvoicerUnitNumber");
                ModelState.Remove("InvoicerUnitTypeID");
                ModelState.Remove("InvoicerStreetNumber");
                ModelState.Remove("InvoicerStreetName");
                ModelState.Remove("InvoicerStreetTypeID");
            }

            if (userView.AddressID == 1)
            {
                ModelState.Remove("PostalAddressID");
                ModelState.Remove("PostalDeliveryNumber");
                ModelState.Remove("lstFCOGroup1");
            }

            if (userView.WholesalerIsPostalAddress == 1)
            {
                ModelState.Remove("WholesalerPostalAddressID");
                ModelState.Remove("WholesalerPostalDeliveryNumber");
            }
            if (userView.InvoicerAddressID == 1)
            {
                ModelState.Remove("InvoicerPostalAddressID");
                ModelState.Remove("InvoicerPostalDeliveryNumber");
            }
            if (userView.UnitNumber != null && userView.UnitTypeID != 0)
            {
                ModelState.Remove("StreetNumber");
            }
            if (userView.StreetNumber != null)
            {
                ModelState.Remove("UnitNumber");
                ModelState.Remove("UnitTypeID");
            }
            if (userView.WholesalerUnitNumber != null && userView.WholesalerUnitTypeID != 0)
            {
                ModelState.Remove("WholesalerStreetNumber");
            }
            if (userView.WholesalerStreetNumber != null)
            {
                ModelState.Remove("WholesalerUnitNumber");
                ModelState.Remove("WholesalerUnitTypeID");
            }
            if (userView.InvoicerUnitNumber != null && userView.InvoicerUnitTypeID != 0)
            {
                ModelState.Remove("InvoicerStreetNumber");
            }
            if (userView.InvoicerStreetNumber != null)
            {
                ModelState.Remove("InvoicerUnitNumber");
                ModelState.Remove("InvoicerUnitTypeID");
            }
            if (userView.Password == null)
            {
                ModelState.Remove("Password");
            }
            if (userView.UserName != null)
                ModelState.Remove("UserName");
            if (userView.Password != null)
                ModelState.Remove("Password");

            ModelState.Remove("RecUserName");
            ModelState.Remove("RecPassword");
        }

        /// <summary>
        /// List All users base on UserTypeId.
        /// </summary>
        /// <param name="selectedUserTypeId">The selected user type identifier.</param>
        /// <param name="view">The view.</param>
        /// <param name="name">The name.</param>
        /// <param name="username">The username.</param>
        /// <param name="email">The email.</param>
        /// <param name="usertype">The usertype.</param>
        /// <param name="reseller">The reseller.</param>
        /// <param name="solarcompany">The solarcompany.</param>
        /// <param name="active">if set to <c>true</c> [active].</param>
        /// <param name="role">The role.</param>
        /// <param name="companyname">The company name.</param>
        /// <param name="companyabn">The company number.</param>
        /// <param name="status">The status.</param>
        /// <param name="accreditationnumber">The accreditation number.</param>
        /// <param name="designernumber">The designer number.</param>
        /// <param name="licensenumber">The license number.</param>
        /// <param name="electricianstatus">The electrician status.</param>
        /// <param name="RAM">RAM.</param>
        public void GetUserList(string selectedUserTypeId, string view = "", string name = "", string username = "", string email = "", string usertype = "", string reseller = "", string solarcompany = "", bool active = false, string role = "", string companyname = "", string companyabn = "", string status = "", string accreditationnumber = "", string designernumber = "", string licensenumber = "", string electricianstatus = "", string RAM = "", string isAllScaJobView = "", string state = "", int? isallowspv = null, string mobile = "", string IsSaasUser = "")
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            int vw = !string.IsNullOrWhiteSpace(view) ? Convert.ToInt32(view) : 1;
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);
            int userType = !string.IsNullOrWhiteSpace(usertype) ? Convert.ToInt32(usertype) : 0;
            int rid = !string.IsNullOrWhiteSpace(reseller) ? Convert.ToInt32(reseller) : 0;
            int ramId = !string.IsNullOrWhiteSpace(RAM) ? Convert.ToInt32(RAM) : 0;
            int solarCompany = !string.IsNullOrWhiteSpace(solarcompany) ? Convert.ToInt32(solarcompany) : 0;
            int roleID = !string.IsNullOrWhiteSpace(role) ? Convert.ToInt32(role) : 0;
            int userstatus = !string.IsNullOrWhiteSpace(status) ? Convert.ToInt32((SystemEnums.UserStatus)Enum.Parse(typeof(SystemEnums.UserStatus), status).GetHashCode()) : 0;
            int elestatus = !string.IsNullOrWhiteSpace(electricianstatus) ? Convert.ToInt32((SystemEnums.ElectricianStatus)Enum.Parse(typeof(SystemEnums.ElectricianStatus), electricianstatus).GetHashCode()) : 0;

            IList<FormBot.Entity.User> lstUser = _userBAL.GetUserList(ProjectSession.LoggedInUserId, pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, selectedUserTypeId, vw, name, username, email, userType, rid, solarCompany, active, roleID, companyname, companyabn, userstatus, accreditationnumber, designernumber, licensenumber, elestatus, ramId, isAllScaJobView, state, isallowspv, mobile, IsSaasUser);
            if (lstUser.Count > 0)
            {
                gridParam.TotalDisplayRecords = lstUser.FirstOrDefault().TotalRecords;
                gridParam.TotalRecords = lstUser.FirstOrDefault().TotalRecords;
            }
            HttpContext.Response.Write(Grid.PrepareDataSet(lstUser, gridParam));
        }

        /// <summary>
        /// Gets the requested ss cfor sca.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="email">The email.</param>
        /// <param name="companyname">The companyname.</param>
        /// <param name="companyabn">The companyabn.</param>
        /// <param name="sscstatus">The sscstatus.</param>
        public void GetRequestedSSCforSCA(string name = "", string email = "", string companyname = "", string companyabn = "", string sscstatus = "")
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);
            int status = !string.IsNullOrWhiteSpace(sscstatus) ? Convert.ToInt32((SystemEnums.ElectricianStatus)Enum.Parse(typeof(SystemEnums.ElectricianStatus), sscstatus).GetHashCode()) : 0;

            IList<FormBot.Entity.User> lstUser = _userBAL.GetRequestedSSCforSCA(ProjectSession.LoggedInUserId, ProjectSession.SolarCompanyId, pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, name, email, companyname, companyabn, status);
            if (lstUser.Count > 0)
            {
                gridParam.TotalDisplayRecords = lstUser.FirstOrDefault().TotalRecords;
                gridParam.TotalRecords = lstUser.FirstOrDefault().TotalRecords;
            }

            HttpContext.Response.Write(Grid.PrepareDataSet(lstUser, gridParam));
        }

        /// <summary>
        /// Gets the solar electrician list.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="username">The username.</param>
        /// <param name="email">The email.</param>
        /// <param name="status">The status.</param>
        /// <param name="accreditationnumber">The accreditationnumber.</param>
        /// <param name="designernumber">The designernumber.</param>
        /// <param name="licensenumber">The licensenumber.</param>
        /// <param name="electricianstatus">The electricianstatus.</param>
        public void GetSolarElectricianList(string name = "", string username = "", string email = "", string status = "", string accreditationnumber = "", string designernumber = "", string licensenumber = "", string electricianstatus = "", string requestedusertype = "", int RoleId = 0, int? IsVerified = null, string mobile = "", string IsSaasUser = "")
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);
            int elestatus = 0;
            if (ProjectSession.UserTypeId == 1 || ProjectSession.UserTypeId == 3)
            {
                elestatus = !string.IsNullOrWhiteSpace(electricianstatus) ? Convert.ToInt32((SystemEnums.UserStatus)Enum.Parse(typeof(SystemEnums.UserStatus), electricianstatus).GetHashCode()) : 0;
            }
            else
            {
                elestatus = !string.IsNullOrWhiteSpace(electricianstatus) ? Convert.ToInt32((SystemEnums.ElectricianStatus)Enum.Parse(typeof(SystemEnums.ElectricianStatus), electricianstatus).GetHashCode()) : 0;
            }

            IList<FormBot.Entity.User> lstUser = _userBAL.GetSolarElectricianList(ProjectSession.UserTypeId, gridParam.PageSize, pageNumber, gridParam.SortCol, gridParam.SortDir, name, username, email, ProjectSession.SolarCompanyId, accreditationnumber, designernumber, licensenumber, elestatus, requestedusertype, RoleId, IsVerified, mobile, IsSaasUser);

            if (lstUser.Count > 0)
            {
                gridParam.TotalDisplayRecords = lstUser.FirstOrDefault().TotalRecords;
                gridParam.TotalRecords = lstUser.FirstOrDefault().TotalRecords;
            }

            HttpContext.Response.Write(Grid.PrepareDataSet(lstUser, gridParam));
        }


        public void GetRequestedSolarElectricianList(string solarcompanyid = "", string name = "", string accreditationnumber = "", string licensenumber = "", string electricianstatus = "", string requestedusertype = "")
        {
            int SolarCompanyId = 0;
            if (ProjectSession.UserTypeId == 1 || ProjectSession.UserTypeId == 2 || ProjectSession.UserTypeId == 3 || ProjectSession.UserTypeId == 5)
            {
                SolarCompanyId = !string.IsNullOrWhiteSpace(solarcompanyid) ? Convert.ToInt32(solarcompanyid) : 0;
            }
            if (ProjectSession.UserTypeId == 4 || ProjectSession.UserTypeId == 6 || ProjectSession.UserTypeId == 8)
            {
                SolarCompanyId = ProjectSession.SolarCompanyId;
            }
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);

            int elestatus = !string.IsNullOrWhiteSpace(electricianstatus) ? Convert.ToInt32((SystemEnums.ElectricianStatus)Enum.Parse(typeof(SystemEnums.ElectricianStatus), electricianstatus).GetHashCode()) : 0;

            //if (!string.IsNullOrEmpty(requestedusertypeid) && requestedusertypeid == "10")
            //	accreditationnumber = licensenumber;

            IList<FormBot.Entity.User> lstUser = _userBAL.GetRequestedSolarElectricianList(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, gridParam.PageSize, pageNumber, gridParam.SortCol, gridParam.SortDir, SolarCompanyId, name, accreditationnumber, licensenumber, elestatus, requestedusertype);
            if (lstUser.Count > 0)
            {
                gridParam.TotalDisplayRecords = lstUser.FirstOrDefault().TotalRecords;
                gridParam.TotalRecords = lstUser.FirstOrDefault().TotalRecords;

            }

            HttpContext.Response.Write(Grid.PrepareDataSet(lstUser, gridParam));
        }

        /// <summary>
        /// Uploads the specified user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Upload Files For Specific User</returns>
        [HttpPost]
        public JsonResult Upload(string userId, int ProofDocumentType = 0, bool isScaProofDocs = false, string DocLoc = "")
        {
            List<HelperClasses.UploadStatus> uploadStatus = new List<HelperClasses.UploadStatus>();
            if (Request.Files != null && Request.Files.Count != 0)
            {
                for (var i = 0; i < Request.Files.Count; i++)
                {
                    //string UserHistoryMessage = "uploaded a " + Enum.GetName(typeof(SystemEnums.ProofDocumentType), ProofDocumentType) + " file " + Request.Files[i].FileName;
                    //Common.SaveUserHistorytoXML(Convert.ToInt32(userId), UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                    uploadStatus.Add(GetFileUpload(Request.Files[i], userId, ProofDocumentType, isScaProofDocs));
                }
            }

            return Json(uploadStatus);
        }

        /// <summary>
        /// Deletes the file from folder.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="FolderName">Name of the folder.</param>
        /// <returns>Delete File From Folder</returns>
        [AllowAnonymous]
        public JsonResult DeleteFileFromFolder(string fileName, string FolderName)
        {
            //DeleteDirectory(Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + "UserDocuments" + "\\" + FolderName, fileName));
            string sourcePath = Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + "UserDocuments" + "\\" + FolderName + "\\" + fileName);
            _generateStcReportBAL.MoveDeletedDocuments(sourcePath, null, FolderName);
            string UserHistoryMessage = "deleted a file " + fileName;
            Common.SaveUserHistorytoXML(Convert.ToInt32(FolderName), UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
            this.ShowMessage(SystemEnums.MessageType.Success, "File has been deleted successfully.", false);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Deletes the signature file from folder.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="FolderName">Name of the folder.</param>
        /// <param name="OldFileName">Old name of the file.</param>
        /// <returns>Delete Signature File From Folder</returns>
        public JsonResult DeleteSignatureFileFromFolder(string fileName, string FolderName, string OldFileName)
        {
            if (OldFileName != fileName && fileName != null)
            {
                DeleteDirectory(Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + "UserDocuments" + "\\" + FolderName, fileName));
                string UserHistoryMessage = "deleted Signature file " + fileName;
                Common.SaveUserHistorytoXML(Convert.ToInt32(FolderName), UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                this.ShowMessage(SystemEnums.MessageType.Success, "File has been deleted successfully.", false);
            }
            //_userBAL.DeleteSignature(Convert.ToInt32(FolderName));
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Deletes the logo from folder.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="FolderName">Name of the folder.</param>
        /// <param name="OldLogo">The old logo.</param>
        /// <returns>Delete Logo From Folder</returns>
        [AllowAnonymous]
        public JsonResult DeleteLogoFromFolder(string fileName, string FolderName, string OldLogo)
        {
            if (OldLogo != fileName && fileName != null)
            {
                DeleteDirectory(Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + "UserDocuments" + "\\", fileName));
                this.ShowMessage(SystemEnums.MessageType.Success, "File has been deleted successfully.", false);
                //_userBAL.DeleteLogo(Convert.ToInt32(FolderName));
                //ProjectSession.Logo = "Images/logo.png";
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Deletes the file by identifier.
        /// </summary>
        /// <param name="UserDocumentID">The user document identifier.</param>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="Documentpath">The documentpath.</param>
        /// <returns>Delete File By ID</returns>
        [AllowAnonymous]
        public JsonResult DeleteFileByID(int UserDocumentID, int UserId, string Documentpath)
        {
            try
            {
                if(UserDocumentID > 0)
                {
                    _userBAL.DeleteUserDocument(UserDocumentID);
                }
                string sourcePath = Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + "UserDocuments" + "\\" + UserId + "\\" + Documentpath);
                _generateStcReportBAL.MoveDeletedDocuments(sourcePath, null, Convert.ToString(UserId));
                //DeleteDirectory(Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + "UserDocuments" + "\\" + UserId + "\\" + Documentpath));
                string UserHistoryMessage = "deleted a file " + Documentpath;
                Common.SaveUserHistorytoXML(UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                this.ShowMessage(SystemEnums.MessageType.Success, "File has been deleted successfully.", false);
            }
            catch
            {
                this.ShowMessage(SystemEnums.MessageType.Error, "There was an error while File  Business. Please try again later.", true);
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Deletes the selfie file from folder.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="FolderName">Name of the folder.</param>
        /// <param name="OldFileName">Old name of the file.</param>
        /// <returns>Delete Signature File From Folder</returns>
        public JsonResult DeleteSelfieFileFromFolder(string fileName, string FolderName, string OldFileName, int UserId = 0)
        {
            if (OldFileName != fileName && fileName != null)
            {
                DeleteDirectory(Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + "UserDocuments" + "\\" + FolderName, fileName));
                string UserHistoryMessage = "deleted Selfie file " + fileName;
                Common.SaveUserHistorytoXML(Convert.ToInt32(FolderName), UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                this.ShowMessage(SystemEnums.MessageType.Success, "File has been deleted successfully.", false);
            }

            if (UserId > 0)
                _userBAL.DeleteSelfie(Convert.ToInt32(FolderName));
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the file upload.
        /// </summary>
        /// <param name="fileUpload">The file upload.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Gets the file upload.</returns>
        public HelperClasses.UploadStatus GetFileUpload(HttpPostedFileBase fileUpload, string userId, int ProofDocumentType = 0, bool isScaProofDocs = false)
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
                        if (isScaProofDocs)
                        {
                            proofDocumentsFolder = Path.Combine(proofDocumentsFolder + "\\" + "UserDocuments" + "\\" + userId + "\\" + Enum.GetName(typeof(SystemEnums.ProofDocumentType), ProofDocumentType) + "\\");
                            //proofDocumentsFolderURL = proofDocumentsFolderURL + "\\" + "UserDocuments" + "\\" + userId + "\\";
                        }
                        else
                        {
                            proofDocumentsFolder = Path.Combine(proofDocumentsFolder + "\\" + "UserDocuments" + "\\" + userId + "\\");
                            proofDocumentsFolderURL = proofDocumentsFolderURL + "\\" + "UserDocuments" + "\\" + userId + "\\";
                        }
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
                                    i++;
                                else
                                {
                                    path = renameFileName;
                                    break;
                                }
                            }

                            fileName = Path.GetFileName(path);
                        }

                        fileName = fileName.Replace("%", "$");

                        if (fileUpload.FileName.Length > 70)
                        {
                            uploadStatus.Status = false;
                            uploadStatus.Message = "BigName";

                        }
                        else
                        {
                            string mimeType = MimeMapping.GetMimeMapping(fileName);
                            fileUpload.SaveAs(path);
                            uploadStatus.Status = true;
                            uploadStatus.Message = "File Uploaded Successfully.";
                            uploadStatus.FileName = fileName;
                            uploadStatus.MimeType = mimeType;

                            uploadStatus.Path = proofDocumentsFolder + uploadStatus.FileName;
                        }

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
        /// Views the download file.
        /// </summary>
        /// <param name="FileName">Name of the file.</param>
        /// <param name="FolderName">Name of the folder.</param>
        /// <returns>Views the download file.</returns>
        [HttpGet]
        public ActionResult ViewDownloadFile(string FileName, string FolderName)
        {
            var path = Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + "UserDocuments" + "\\" + FolderName, FileName);

            var fileData = System.IO.File.ReadAllBytes(path);

            Response.Clear();
            Response.ClearHeaders();
            Response.ClearContent();
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition", string.Format("attachment; filename=" + FileName));
            Response.BinaryWrite(fileData);

            //Response.ContentType = "application/octet-stream";
            //Response.AddHeader("content-disposition", "attachment;  filename=\"" + FileName + "\"");
            //Response.BinaryWrite(fileData);
            //Response.End();

            return Json(false, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the postal address.
        /// </summary>
        /// <returns>Gets the postal address.</returns>
        [HttpGet]
        public JsonResult GetPostalAddress()
        {
            List<SelectListItem> Items = _userBAL.GetPostalAddress().Select(a => new SelectListItem { Text = a.PostalDeliveryType, Value = a.PostalAddressID.ToString() }).ToList();
            return Json(Items, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Sends the request compliance check.
        /// </summary>
        /// <param name="accreditedInstallersList">The accredited installers list.</param>
        /// <param name="solarElectricianView">The solar electrician view.</param>
        /// <param name="CheckAll">CheckAll.</param>
        public void SendRequestComplianceCheck(List<AccreditedInstallers> accreditedInstallersList, FormBot.Entity.SolarElectricianView solarElectricianView, bool CheckAll = true)
        {
            if (accreditedInstallersList[0].FirstName.ToLower() != solarElectricianView.FirstName.ToLower())
            {
                ViewBag.FirstName = "First Name";
            }

            if (accreditedInstallersList[0].LastName.ToLower() != solarElectricianView.LastName.ToLower())
            {
                ViewBag.LastName = "Last Name";
            }

            if (CheckAll)
            {
                if (accreditedInstallersList[0].Email.ToLower() != solarElectricianView.Email.ToLower())
                {
                    ViewBag.Email = "Email";
                }
            }

            //if (accreditedInstallersList[0].PostalCode != solarElectricianView.PostCode)
            //{

            //    ViewBag.PostCode = "Post Code";
            //}

            if (CheckAll)
            {
                if (accreditedInstallersList[0].Abbreviation.ToLower() != solarElectricianView.State.ToLower())
                {
                    ViewBag.StateName = "State";
                }
            }

            if (accreditedInstallersList[0].GridType == "Design & Supervise" || accreditedInstallersList[0].GridType == "Design & Install")
            {
                if (solarElectricianView.SEDesignRoleId != 3)
                {
                    ViewBag.SEDesignRole = "SE Design Role";
                }

                if (CheckAll)
                {
                    if ((string.IsNullOrWhiteSpace(solarElectricianView.ElectricalContractorsLicenseNumber)) || accreditedInstallersList[0].LicensedElectricianNumber.ToLower() != solarElectricianView.ElectricalContractorsLicenseNumber.ToLower())
                    {
                        ViewBag.LicensedNumber = "Licensed Electrician Number";
                    }

                    if ((string.IsNullOrWhiteSpace(solarElectricianView.CECDesignerNumber)) || ((accreditedInstallersList[0].AccreditationNumber).ToLower() != (solarElectricianView.CECDesignerNumber).ToLower()))
                    {
                        ViewBag.CECDesignerNumber = "CEC Designer Number";
                    }
                }

                if ((string.IsNullOrWhiteSpace(solarElectricianView.CECAccreditationNumber)) || ((accreditedInstallersList[0].AccreditationNumber).ToLower() != (solarElectricianView.CECAccreditationNumber).ToLower()))
                {
                    ViewBag.CECAccreditationNumber = "CEC Accreditation Number";
                }

            }

            if (accreditedInstallersList[0].GridType == "Install only")
            {
                if (solarElectricianView.SEDesignRoleId != 1)
                {
                    ViewBag.SEDesignRole = "SE Design Role";
                }

                if (CheckAll)
                {
                    if ((string.IsNullOrWhiteSpace(solarElectricianView.ElectricalContractorsLicenseNumber)) || ((accreditedInstallersList[0].LicensedElectricianNumber).ToLower() != (solarElectricianView.ElectricalContractorsLicenseNumber).ToLower()))
                    {
                        ViewBag.LicensedNumber = "Licensed Electrician Number";
                    }
                }
            }

            if (accreditedInstallersList[0].GridType == "Design only")
            {
                if (solarElectricianView.SEDesignRoleId != 2)
                {
                    ViewBag.SEDesignRole = "SE Design Role";
                }

                if (CheckAll)
                {
                    if ((string.IsNullOrWhiteSpace(solarElectricianView.CECDesignerNumber)) || ((accreditedInstallersList[0].AccreditationNumber).ToLower() != (solarElectricianView.CECDesignerNumber).ToLower()))
                    {
                        ViewBag.CECDesignerNumber = "CEC Designer Number";
                    }
                }

                if ((string.IsNullOrWhiteSpace(solarElectricianView.CECAccreditationNumber)) || ((accreditedInstallersList[0].AccreditationNumber).ToLower() != (solarElectricianView.CECAccreditationNumber).ToLower()))
                {
                    ViewBag.CECAccreditationNumber = "CEC Accreditation Number";
                }
            }
            if (CheckAll)
            {
                if (solarElectricianView.AddressID == 1)
                {
                    if (accreditedInstallersList[0].MailingAddressUnitType != "")
                    {
                        solarElectricianView.UnitTypeName = _unitTypeBAL.GetUnitnameByUnitId(solarElectricianView.UnitTypeID);
                        if (accreditedInstallersList[0].MailingAddressUnitType.ToLower() != solarElectricianView.UnitTypeName.ToLower())
                        {
                            ViewBag.UnitType = "Unit Type";
                        }
                    }

                    if (accreditedInstallersList[0].MailingAddressUnitNumber != "" && !string.IsNullOrWhiteSpace(solarElectricianView.UnitNumber))
                    {
                        if (accreditedInstallersList[0].MailingAddressUnitNumber.ToLower() != solarElectricianView.UnitNumber.ToLower())
                        {
                            ViewBag.UnitNumber = "Unit Number";
                        }
                    }

                    if (accreditedInstallersList[0].MailingAddressStreetNumber != "" && !string.IsNullOrWhiteSpace(solarElectricianView.StreetNumber))
                    {
                        if (accreditedInstallersList[0].MailingAddressStreetNumber.ToLower() != solarElectricianView.StreetNumber.ToLower())
                        {
                            ViewBag.StreetNumber = "Street Number";
                        }
                    }

                    if (accreditedInstallersList[0].MailingAddressStreetName != "")
                    {
                        if (accreditedInstallersList[0].MailingAddressStreetName.ToLower() != solarElectricianView.StreetName.ToLower())
                        {
                            ViewBag.StreetName = "Street Name";
                        }
                    }

                    if (accreditedInstallersList[0].MailingAddressStreetType != "")
                    {
                        solarElectricianView.StreetTypeName = _streetTypeBAL.GetStreetTypeNameByStreetTypeId(solarElectricianView.StreetTypeID);
                        if (accreditedInstallersList[0].MailingAddressStreetType.ToLower() != solarElectricianView.StreetTypeName.ToLower())
                        {
                            ViewBag.StreetType = "Street Type";
                        }
                    }
                }
                else if (solarElectricianView.AddressID == 2)
                {
                    if (accreditedInstallersList[0].MailingAddressUnitType != "")
                    {
                        if (accreditedInstallersList[0].MailingAddressUnitType.ToLower() != solarElectricianView.Code.ToLower())
                        {
                            ViewBag.PostalType = "Postal Delivery Type";
                        }
                    }

                    if (accreditedInstallersList[0].MailingAddressUnitNumber != "")
                    {
                        if (accreditedInstallersList[0].MailingAddressUnitNumber.ToLower() != solarElectricianView.PostalDeliveryNumber.ToLower())
                        {
                            ViewBag.PostalNumber = "Postal Delivery Number";
                        }
                    }
                }
            }

            if (CheckAll)
            {
                if (ViewBag.FirstName != null || ViewBag.LicensedNumber != null || ViewBag.CECAccreditationNumber != null || ViewBag.CECDesignerNumber != null || ViewBag.Email != null || ViewBag.SEDesignRole != null || ViewBag.LastName != null || ViewBag.UnitNumber != null || ViewBag.StreetNumber != null || ViewBag.StreetName != null || ViewBag.StreetType != null || ViewBag.UnitType != null || ViewBag.PostalType != null || ViewBag.PostalNumber != null || ViewBag.StateName != null)
                {
                    ViewBag.FieldList = "Following field(s) does not pass compliance check:";
                    solarElectricianView.IsActiveDiv = true;
                }
            }
            else
            {
                if (ViewBag.FirstName != null || ViewBag.CECAccreditationNumber != null || ViewBag.SEDesignRole != null || ViewBag.LastName != null)
                {
                    ViewBag.FieldList = "Following field(s) does not pass compliance check:";
                    solarElectricianView.IsActiveDiv = true;
                }
            }
        }

        /// <summary>
        /// Views the detail compliance check.
        /// </summary>
        /// <param name="accreditedInstallersList">The accredited installers list.</param>
        /// <param name="userView">The user view.</param>
        public void ViewDetailComplianceCheck(List<AccreditedInstallers> accreditedInstallersList, FormBot.Entity.User userView)
        {
            if (accreditedInstallersList.Count > 0)
            {
                if (accreditedInstallersList[0].FirstName.ToLower() != (userView.FirstName == null ? null : userView.FirstName.ToLower()))
                {
                    ViewBag.FirstName = "First Name -";
                    ViewBag.AccreditedFirstName = accreditedInstallersList[0].FirstName;
                }

                if (accreditedInstallersList[0].LastName.ToLower() != (userView.LastName == null ? null : userView.LastName.ToLower()))
                {
                    ViewBag.LastName = "Last Name -";
                    ViewBag.AccreditedLastName = accreditedInstallersList[0].LastName;
                }

                if (accreditedInstallersList[0].Email.ToLower() != (userView.Email == null ? null : userView.Email.ToLower()))
                {
                    ViewBag.Email = "Email -";
                    ViewBag.AccreditedEmail = accreditedInstallersList[0].Email;
                }

                if (accreditedInstallersList[0].PostalCode != userView.PostCode)
                {
                    ViewBag.PostCode = "Post Code -";
                    ViewBag.AccreditedPostCode = accreditedInstallersList[0].PostalCode;
                }

                if (accreditedInstallersList[0].Phone.Replace(" ", "") != (userView.Phone == null ? null : userView.Phone.Replace(" ", "")))
                {
                    ViewBag.Phone = "Phone -";
                    ViewBag.AccreditedPhone = accreditedInstallersList[0].Phone;
                }

                if (accreditedInstallersList[0].Mobile.Replace(" ", "") != (userView.Mobile == null ? null : userView.Mobile.Replace(" ", "")))
                {
                    ViewBag.Mobile = "Mobile -";
                    ViewBag.AccreditedMobile = accreditedInstallersList[0].Mobile;
                }

                if (userView.UserTypeID == 7 || userView.IsSTC == true)
                {
                    if (accreditedInstallersList[0].GridType == "Design & Supervise" || accreditedInstallersList[0].GridType == "Design & Install")
                    {
                        if (userView.SEDesigner != 3)
                        {
                            ViewBag.SEDesignRole1 = "SE Design Role -";
                            ViewBag.AccreditedSEDesignRole = accreditedInstallersList[0].GridType;
                        }
                    }

                    if (accreditedInstallersList[0].GridType == "Install only")
                    {
                        if (userView.SEDesigner != 1)
                        {
                            ViewBag.SEDesignRole1 = "SE Design Role -";
                            ViewBag.AccreditedSEDesignRole = accreditedInstallersList[0].GridType;
                        }
                    }

                    if (accreditedInstallersList[0].GridType == "Design only")
                    {
                        if (userView.SEDesigner != 2)
                        {
                            ViewBag.SEDesignRole1 = "SE Design Role -";
                            ViewBag.AccreditedSEDesignRole = accreditedInstallersList[0].GridType;
                        }
                    }
                }

                if (userView.UserTypeID == 7)
                {
                    if (accreditedInstallersList[0].GridType == "Design & Supervise" || accreditedInstallersList[0].GridType == "Design & Install")
                    {
                        if (accreditedInstallersList[0].LicensedElectricianNumber != userView.ElectricalContractorsLicenseNumber)
                        {
                            ViewBag.LicensedNumber = "Licensed Electrician Number -";
                            ViewBag.AccreditedLicensedNumber = accreditedInstallersList[0].LicensedElectricianNumber;
                        }

                        if (accreditedInstallersList[0].AccreditationNumber != userView.CECDesignerNumber)
                        {
                            ViewBag.CECDesignerNumber = "CEC Designer Number -";
                            ViewBag.AccreditedCECDesignerNumber = accreditedInstallersList[0].AccreditationNumber;
                        }

                        if (accreditedInstallersList[0].AccreditationNumber != userView.CECAccreditationNumber)
                        {
                            ViewBag.CECAccreditationNumber = "CEC Accreditation Number -";
                            ViewBag.AccreditedCECAccreditationNumber = accreditedInstallersList[0].AccreditationNumber;
                        }
                    }

                    if (accreditedInstallersList[0].GridType == "Install only")
                    {
                        if (accreditedInstallersList[0].LicensedElectricianNumber != userView.ElectricalContractorsLicenseNumber)
                        {
                            ViewBag.LicensedNumber = "Licensed Electrician Number -";
                            ViewBag.AccreditedLicensedNumber = accreditedInstallersList[0].LicensedElectricianNumber;
                        }
                    }

                    if (accreditedInstallersList[0].GridType == "Design only")
                    {
                        if (accreditedInstallersList[0].AccreditationNumber != userView.CECDesignerNumber)
                        {
                            ViewBag.CECDesignerNumber = "CEC Designer Number -";
                            ViewBag.AccreditedCECDesignerNumber = accreditedInstallersList[0].AccreditationNumber;
                        }

                        if (accreditedInstallersList[0].AccreditationNumber != userView.CECAccreditationNumber)
                        {
                            ViewBag.CECAccreditationNumber = "CEC Accreditation Number -";
                            ViewBag.AccreditedCECAccreditationNumber = accreditedInstallersList[0].AccreditationNumber;
                        }
                    }
                }
                else
                {
                    if (accreditedInstallersList[0].LicensedElectricianNumber != userView.ElectricalContractorsLicenseNumber)
                    {
                        ViewBag.LicensedNumber = "Licensed Electrician Number -";
                        ViewBag.AccreditedLicensedNumber = accreditedInstallersList[0].LicensedElectricianNumber;
                    }

                    if (accreditedInstallersList[0].AccreditationNumber != userView.CECAccreditationNumber)
                    {
                        ViewBag.CECAccreditationNumber = "CEC Accreditation Number -";
                        ViewBag.AccreditedCECAccreditationNumber = accreditedInstallersList[0].AccreditationNumber;
                    }
                }

                //no one is blank
                if (accreditedInstallersList[0].MailingAddressUnitType != "" && accreditedInstallersList[0].MailingAddressUnitNumber != "" && accreditedInstallersList[0].MailingAddressStreetName != "" && accreditedInstallersList[0].MailingAddressStreetNumber != "" && accreditedInstallersList[0].MailingAddressStreetType != "")
                {
                    if (accreditedInstallersList[0].MailingAddressStreetNumber.ToLower() != (userView.StreetNumber == null ? null : userView.StreetNumber.ToLower()))
                    {
                        ViewBag.StreetNumber = "Street Number -";
                        if (accreditedInstallersList[0].MailingAddressStreetNumber != "")
                        {
                            ViewBag.AccreditedStreetNumber = accreditedInstallersList[0].MailingAddressStreetNumber;
                        }

                    }

                    if (accreditedInstallersList[0].MailingAddressStreetName.ToLower() != (userView.StreetName == null ? null : userView.StreetName.ToLower()))
                    {
                        ViewBag.StreetName = "Street Name -";
                        if (accreditedInstallersList[0].MailingAddressStreetName != "")
                        {
                            ViewBag.AccreditedStreetName = accreditedInstallersList[0].MailingAddressStreetName;
                        }

                    }

                    if (accreditedInstallersList[0].MailingAddressStreetType.ToLower() != (userView.StreetTypeName == null ? null : userView.StreetTypeName.ToLower()))
                    {
                        ViewBag.StreetType = "Street Type -";
                        if (accreditedInstallersList[0].MailingAddressStreetType != "")
                        {
                            ViewBag.AccreditedStreetType = accreditedInstallersList[0].MailingAddressStreetType;
                        }

                    }

                    if (accreditedInstallersList[0].MailingAddressUnitType.ToLower() != (userView.UnitTypeName != null ? userView.UnitTypeName.ToLower() : null))
                    {
                        ViewBag.UnitType = "Unit Type -";
                        if (accreditedInstallersList[0].MailingAddressUnitType != "")
                        {
                            ViewBag.AccreditedUnitType = accreditedInstallersList[0].MailingAddressUnitType;
                        }

                    }

                    if (accreditedInstallersList[0].MailingAddressUnitNumber.ToLower() != (userView.UnitNumber == null ? null : userView.UnitNumber.ToLower()))
                    {
                        ViewBag.UnitNumber = "Unit Number -";
                        if (accreditedInstallersList[0].MailingAddressUnitNumber != "")
                        {
                            ViewBag.AccreditedUnitNumber = accreditedInstallersList[0].MailingAddressUnitNumber;
                        }

                    }

                }
                //1st field is blank
                if (accreditedInstallersList[0].MailingAddressUnitNumber != "" && accreditedInstallersList[0].MailingAddressStreetName != "" && accreditedInstallersList[0].MailingAddressStreetNumber != "" && accreditedInstallersList[0].MailingAddressStreetType != "")
                {
                    if (accreditedInstallersList[0].MailingAddressStreetNumber.ToLower() != (userView.StreetNumber == null ? null : userView.StreetNumber.ToLower()))
                    {
                        ViewBag.StreetNumber = "Street Number -";
                        if (accreditedInstallersList[0].MailingAddressStreetNumber != "")
                        {
                            ViewBag.AccreditedStreetNumber = accreditedInstallersList[0].MailingAddressStreetNumber;
                        }
                    }

                    if (accreditedInstallersList[0].MailingAddressStreetName.ToLower() != (userView.StreetName == null ? null : userView.StreetName.ToLower()))
                    {
                        ViewBag.StreetName = "Street Name -";
                        if (accreditedInstallersList[0].MailingAddressStreetName != "")
                        {
                            ViewBag.AccreditedStreetName = accreditedInstallersList[0].MailingAddressStreetName;
                        }

                    }

                    if (accreditedInstallersList[0].MailingAddressStreetType.ToLower() != (userView.StreetTypeName == null ? null : userView.StreetTypeName.ToLower()))
                    {
                        ViewBag.StreetType = "Street Type -";
                        if (accreditedInstallersList[0].MailingAddressStreetType != "")
                        {
                            ViewBag.AccreditedStreetType = accreditedInstallersList[0].MailingAddressStreetType;
                        }

                    }

                    ViewBag.UnitType = "Unit Type -";
                    ViewBag.AccreditedUnitType = "[Blank]";

                    if (accreditedInstallersList[0].MailingAddressUnitNumber.ToLower() != (userView.UnitNumber == null ? null : userView.UnitNumber.ToLower()))
                    {
                        ViewBag.UnitNumber = "Unit Number -";
                        if (accreditedInstallersList[0].MailingAddressUnitNumber != "")
                        {
                            ViewBag.AccreditedUnitNumber = accreditedInstallersList[0].MailingAddressUnitNumber;
                        }
                    }
                }
                //last 3 field blank
                if (accreditedInstallersList[0].MailingAddressStreetName == "" && accreditedInstallersList[0].MailingAddressStreetNumber == "" && accreditedInstallersList[0].MailingAddressStreetType == "")
                {
                    if (userView.IsPostalAddress == true && userView.PostalAddressID > 0)
                    {
                        List<PostalAddressView> postalAddressView = _userBAL.GetPostalAddressDetail(userView.PostalAddressID);
                        if (accreditedInstallersList[0].MailingAddressUnitType.ToLower() != postalAddressView[0].Code.ToLower())
                        {
                            ViewBag.PostalDeliveryType = "Postal Delivery Type -";
                            if (accreditedInstallersList[0].MailingAddressUnitType != "")
                            {
                                ViewBag.AccreditedPostalDeliveryType = accreditedInstallersList[0].MailingAddressUnitType;
                            }
                            else
                            {
                                ViewBag.AccreditedPostalDeliveryType = "[Blank]";
                            }
                        }
                    }
                    else
                    {
                        ViewBag.PostalDeliveryType = "Postal Delivery Type -";
                        ViewBag.AccreditedPostalDeliveryType = accreditedInstallersList[0].MailingAddressUnitType;
                    }

                    if (accreditedInstallersList[0].MailingAddressUnitNumber.ToLower() != (userView.PostalDeliveryNumber == null ? null : userView.PostalDeliveryNumber.ToLower()))
                    {
                        ViewBag.PostalDeliveryNumber = "Postal Delivery Number -";
                        if (accreditedInstallersList[0].MailingAddressUnitNumber != "")
                        {
                            ViewBag.AccreditedPostalDeliveryNumber = accreditedInstallersList[0].MailingAddressUnitNumber;
                        }
                        else
                        {
                            ViewBag.AccreditedPostalDeliveryNumber = "[Blank]";
                        }
                    }

                }
                //1st 2 field blank
                if (accreditedInstallersList[0].MailingAddressUnitType == "" && accreditedInstallersList[0].MailingAddressUnitNumber == "")
                {
                    if (accreditedInstallersList[0].MailingAddressStreetNumber.ToLower() != (userView.StreetNumber == null ? null : userView.StreetNumber.ToLower()))
                    {
                        ViewBag.StreetNumber = "Street Number -";
                        if (accreditedInstallersList[0].MailingAddressStreetNumber != "")
                        {
                            ViewBag.AccreditedStreetNumber = accreditedInstallersList[0].MailingAddressStreetNumber;
                        }
                        else
                        {
                            ViewBag.AccreditedStreetNumber = "[Blank]";
                        }
                    }

                    if (accreditedInstallersList[0].MailingAddressStreetName.ToLower() != (userView.StreetName == null ? null : userView.StreetName.ToLower()))
                    {
                        ViewBag.StreetName = "Street Name -";
                        if (accreditedInstallersList[0].MailingAddressStreetName != "")
                        {
                            ViewBag.AccreditedStreetName = accreditedInstallersList[0].MailingAddressStreetName;
                        }
                        else
                        {
                            ViewBag.AccreditedStreetName = "[Blank]";
                        }
                    }

                    if (accreditedInstallersList[0].MailingAddressStreetType.ToLower() != (userView.StreetTypeName == null ? null : userView.StreetTypeName.ToLower()))
                    {
                        ViewBag.StreetType = "Street Type -";
                        if (accreditedInstallersList[0].MailingAddressStreetType != "")
                        {
                            ViewBag.AccreditedStreetType = accreditedInstallersList[0].MailingAddressStreetType;
                        }
                        else
                        {
                            ViewBag.AccreditedStreetType = "[Blank]";
                        }
                    }

                    ViewBag.UnitType = "Unit Type -";
                    ViewBag.AccreditedUnitType = "[Blank]";
                    ViewBag.UnitNumber = "Unit Number -";
                    ViewBag.AccreditedUnitNumber = "[Blank]";
                }

                if (accreditedInstallersList[0].MailingCity.ToLower() != (userView.Town == null ? null : userView.Town.ToLower()))
                {
                    ViewBag.Town = "Town -";
                    if (accreditedInstallersList[0].MailingCity != "")
                    {
                        ViewBag.AccreditedTown = accreditedInstallersList[0].MailingCity;
                    }
                    else
                    {
                        ViewBag.AccreditedTown = "[Blank]";
                    }
                }

                if ((!string.IsNullOrWhiteSpace(accreditedInstallersList[0].Abbreviation)) && accreditedInstallersList[0].Abbreviation.ToLower() != userView.State.ToLower())
                {
                    ViewBag.StateName = "State -";
                    if (accreditedInstallersList[0].Abbreviation != "")
                    {
                        ViewBag.AccreditedStateName = accreditedInstallersList[0].Abbreviation;
                    }
                    else
                    {
                        ViewBag.AccreditedStateName = "[Blank]";
                    }
                }

                if (ViewBag.FirstName != null || ViewBag.LastName != null || ViewBag.PostCode != null || ViewBag.Phone != null || ViewBag.Mobile != null || ViewBag.LicensedNumber != null || ViewBag.UnitNumber != null || ViewBag.StreetNumber != null || ViewBag.StreetName != null || ViewBag.StreetType != null || ViewBag.UnitType != null || ViewBag.Town != null || ViewBag.StateName != null)
                {
                    ViewBag.FieldList = "Following field(s) does not pass compliance check:";
                }

            }
            else
            {
                if (userView.UserTypeID == 4)
                {
                    ViewBag.AccreditationNumber = "Solar Company with given accreditation number doesn't exist.";
                }
                else
                {
                    //ViewBag.AccreditationNumber = "Solar Electrician with given accreditation number doesn't exist.";
                    ViewBag.AccreditationNumber = GetErrorMessageForSolarElectrician(4);
                }
            }
        }

        /// <summary>
        /// Emails the configuration.
        /// </summary>
        /// <param name="userView">The user view.</param>
        public void EmailConfiguration(FormBot.Entity.User userView)
        {
            FormBot.Entity.Email.EmailSignup emailModel = new Entity.Email.EmailSignup();
            string xmlString = string.Empty;
            DataSet lstEmail = _userBAL.LoginUserEmailDetails(ProjectSession.LoggedInUserId);
            if (lstEmail.Tables[0].Rows.Count > 0)
            {
                //Session[FormBot.Email.Constants.sessionAccount] = null;
                FormBot.Email.Account acct = Session[FormBot.Email.Constants.sessionAccount] as FormBot.Email.Account;
                DataRow dr = (DataRow)lstEmail.Tables[0].Rows[0];

                emailModel.Login = dr["email"].ToString();
                emailModel.ConfigurationEmail = dr["email"].ToString();
                emailModel.ConfigurationPassword = dr["mail_inc_pass"].ToString();
                emailModel.IncomingMail = dr["mail_inc_host"].ToString();
                emailModel.IncomingMailPort = Convert.ToInt32(dr["mail_inc_port"]);
                emailModel.Login = dr["email"].ToString();
                emailModel.OutgoingMail = dr["mail_out_host"].ToString();
                emailModel.OutgoingMailPort = Convert.ToInt32(dr["mail_out_port"]);
                emailModel.Def_TimeZone = Convert.ToInt32(dr["def_timezone"]);
                if (acct != null)
                {
                    emailModel.EmailSignature = acct.Signature;
                    var result1 = new { acct.Signature };
                    ViewBag.Signature = Json(result1);
                    if (acct.UserOfAccount != null && acct.UserOfAccount.Settings != null && acct.UserOfAccount.Settings.DefaultTimeZone != null)
                    {
                        emailModel.Def_TimeZone = acct.UserOfAccount.Settings.DefaultTimeZone;
                    }
                }

                xmlString = "<?xml version='1.0' encoding='UTF-8'?><webmail><param name='action' value='login' /><param name='request' value='' /><param name='email'><![CDATA[" + emailModel.ConfigurationEmail
                    + "]]></param><param name='mail_inc_login'><![CDATA[" + emailModel.Login + "]]></param><param name='mail_inc_pass'><![CDATA[" + emailModel.ConfigurationPassword + "]]></param><param name='mail_inc_host'><![CDATA[" + emailModel.IncomingMail
                    + "]]></param><param name='mail_inc_port' value='" + emailModel.IncomingMailPort + "' /><param name='mail_protocol' value='0' /><param name='mail_out_host'><![CDATA[" + emailModel.OutgoingMail
                    + "]]></param><param name='mail_out_port' value='" + emailModel.OutgoingMailPort + "' /><param name='mail_out_auth' value='1' /><param name='sign_me' value='0' /><param name='language' /><param name='advanced_login' value='1' /></webmail>";
                CheckMail checkMail = new CheckMail();
                var result = checkMail.GetMessages(xmlString);
                userView.EmailSignup.ConfigurationEmail = emailModel.ConfigurationEmail;
                userView.EmailSignup.ConfigurationPassword = emailModel.ConfigurationPassword;
                userView.EmailSignup.IncomingMail = emailModel.IncomingMail;
                userView.EmailSignup.IncomingMailPort = emailModel.IncomingMailPort;
                userView.EmailSignup.Login = emailModel.Login;
                userView.EmailSignup.OutgoingMail = emailModel.OutgoingMail;
                userView.EmailSignup.OutgoingMailPort = emailModel.OutgoingMailPort;
                userView.EmailSignup.Def_TimeZone = emailModel.Def_TimeZone;
            }
        }

        /// <summary>
        /// Configure Rec email account
        /// </summary>
        /// <param name="userView"></param>
        public void RecEmailConfiguration(FormBot.Entity.User userView)
        {
            FormBot.Entity.Email.RecEmailSignup Rec_emailModel = new Entity.Email.RecEmailSignup();
            string xmlString = string.Empty;
            DataSet lstEmail = _userBAL.RecUserEmailDetails(ProjectSession.LoggedInUserId);
            if (lstEmail.Tables[0].Rows.Count > 0)
            {
                //Session[FormBot.Email.Constants.sessionAccount] = null;
                //Account acct = Session[FormBot.Email.Constants.sessionAccount] as Account;
                DataRow dr = (DataRow)lstEmail.Tables[0].Rows[0];

                Rec_emailModel.RecLogin = Convert.ToString(dr["email"]);
                Rec_emailModel.RecConfigurationEmail = Convert.ToString(dr["email"]);
                Rec_emailModel.RecConfigurationPassword = Convert.ToString(dr["mail_inc_pass"]);
                Rec_emailModel.RecIncomingMail = Convert.ToString(dr["mail_inc_host"]);
                Rec_emailModel.RecIncomingMailPort = Convert.ToInt32(dr["mail_inc_port"]);
                Rec_emailModel.RecLogin = Convert.ToString(dr["email"]);
                Rec_emailModel.RecOutgoingMail = Convert.ToString(dr["mail_out_host"]);
                Rec_emailModel.RecOutgoingMailPort = Convert.ToInt32(dr["mail_out_port"]);
                Rec_emailModel.RecDef_TimeZone = Convert.ToInt32(dr["def_timezone"]);

                Rec_emailModel.RecEmailSignature = Convert.ToString(dr["signature"]);
                var result1 = new { Rec_emailModel.RecEmailSignature };
                ViewBag.RecEmailSignature = Json(result1);

                //if (acct != null)
                //{
                //    Rec_emailModel.RecEmailSignature = acct.Signature;
                //    var result1 = new { acct.Signature };
                //    ViewBag.RecEmailSignature = Json(result1);
                //    if (acct.UserOfAccount != null && acct.UserOfAccount.Settings != null && acct.UserOfAccount.Settings.DefaultTimeZone != null)
                //    {
                //        Rec_emailModel.RecDef_TimeZone = acct.UserOfAccount.Settings.DefaultTimeZone;
                //    }
                //}

                //xmlString = "<?xml version='1.0' encoding='UTF-8'?><webmail><param name='action' value='login' /><param name='request' value='' /><param name='email'><![CDATA[" + Rec_emailModel.RecConfigurationEmail
                //    + "]]></param><param name='mail_inc_login'><![CDATA[" + Rec_emailModel.RecLogin + "]]></param><param name='mail_inc_pass'><![CDATA[" + Rec_emailModel.RecConfigurationPassword + "]]></param><param name='mail_inc_host'><![CDATA[" + Rec_emailModel.RecIncomingMail
                //    + "]]></param><param name='mail_inc_port' value='" + Rec_emailModel.RecIncomingMailPort + "' /><param name='mail_protocol' value='0' /><param name='mail_out_host'><![CDATA[" + Rec_emailModel.RecOutgoingMail
                //    + "]]></param><param name='mail_out_port' value='" + Rec_emailModel.RecOutgoingMailPort + "' /><param name='mail_out_auth' value='1' /><param name='sign_me' value='0' /><param name='language' /><param name='advanced_login' value='1' /></webmail>";
                //CheckMail checkMail = new CheckMail();
                //var result = checkMail.GetMessages(xmlString);


                userView.RecEmailSignup.RecConfigurationEmail = Rec_emailModel.RecConfigurationEmail;
                userView.RecEmailSignup.RecConfigurationPassword = Rec_emailModel.RecConfigurationPassword;
                userView.RecEmailSignup.RecIncomingMail = Rec_emailModel.RecIncomingMail;
                userView.RecEmailSignup.RecIncomingMailPort = Rec_emailModel.RecIncomingMailPort;
                userView.RecEmailSignup.RecLogin = Rec_emailModel.RecLogin;
                userView.RecEmailSignup.RecOutgoingMail = Rec_emailModel.RecOutgoingMail;
                userView.RecEmailSignup.RecOutgoingMailPort = Rec_emailModel.RecOutgoingMailPort;
                userView.RecEmailSignup.RecDef_TimeZone = Rec_emailModel.RecDef_TimeZone;
            }
        }

        /// <summary>
        /// Checks the user exist.
        /// </summary>
        /// <param name="UserValue">The user value.</param>
        /// <param name="FieldName">Name of the field.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="resellerID">The reseller identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <param name="solarCompanyId">solarCompanyId.</param>
        /// <returns>Checks the user exist.</returns>
        [AllowAnonymous]
        public JsonResult CheckUserExist(string UserValue, string FieldName, int? userId = null, int? resellerID = null, int? UserTypeId = null, int solarCompanyId = 0)
        {
            object isUserExists;
            if (FieldName == "UserName")
            {
                isUserExists = _userBAL.CheckEmailExists(UserValue, userId);
            }
            else if (FieldName == "CompanyABN" || FieldName == "UserView_CompanyABN" || FieldName == "JobOwnerDetails_CompanyABN")
            {
                string UserValue1 = UserValue.Replace(" ", "");
                isUserExists = _userBAL.CheckCompanyABN(UserValue1, userId, UserTypeId);
            }
            else if (FieldName == "LoginCompanyName")
            {
                isUserExists = _userBAL.CheckLoginCompanyNameExists(UserValue, resellerID);
            }
            else if (FieldName.ToLower().Contains("cecaccreditationnumber"))
            {
                //isUserExists = _userBAL.CheckLoginCompanyNameExists(UserValue, resellerID);
                int accreditationNumberExist = _userBAL.CheckExistAccreditationNumberForInstallerDesigner(UserValue, solarCompanyId);
                if (!accreditationNumberExist.Equals(0))
                {
                    isUserExists = 0;
                    return Json(new { status = false, message = "Installer/Designer already exists in your account." }, JsonRequestBehavior.AllowGet);
                }
                else
                    isUserExists = 1;

                List<AccreditedInstallers> accreditedInstallersList = _userBAL.CheckAccreditationNumberExistsInAccreditedInstallers(UserValue, null, true);

                if (accreditedInstallersList.Count == 0)
                {
                    isUserExists = 0;
                    return Json(new { status = false, message = "Installer/Designer with given accreditation number doesn't exist." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    isUserExists = 1;
                    int roleId = 0;
                    if (!string.IsNullOrWhiteSpace(accreditedInstallersList[0].GridType))
                    {
                        if (accreditedInstallersList[0].GridType.ToLower() == "install only" || accreditedInstallersList[0].GridType.ToLower() == "grid-connect install only")
                            roleId = Convert.ToInt32(FormBot.Helper.SystemEnums.SEDesignRole.Install);
                        else if (accreditedInstallersList[0].GridType.ToLower() == "design only" || accreditedInstallersList[0].GridType.ToLower() == "grid-connect design only")
                            roleId = Convert.ToInt32(FormBot.Helper.SystemEnums.SEDesignRole.Design);
                        else if (accreditedInstallersList[0].GridType.ToLower() == "design & install" || accreditedInstallersList[0].GridType.ToLower() == "grid-connect design & install" || accreditedInstallersList[0].GridType.ToLower() == "design & supervise"
                            || accreditedInstallersList[0].GridType.ToLower() == "grid-connect design/supervise")
                            roleId = Convert.ToInt32(FormBot.Helper.SystemEnums.SEDesignRole.Design_Install);
                        //else if (accreditedInstallersList[0].GridType.ToLower() == "design & install")
                        //    roleId = Convert.ToInt32(FormBot.Helper.SystemEnums.SEDesignRole.Design_Install);
                        else
                            roleId = 0;
                    }
                    accreditedInstallersList[0].RoleId = roleId;
                    accreditedInstallersList[0].Phone = Regex.Replace(accreditedInstallersList[0].Phone, "[^.0-9]", "");
                    accreditedInstallersList[0].Inst_Phone = Regex.Replace(accreditedInstallersList[0].Inst_Phone, "[^.0-9]", "");

                    int scheduleStatus = Convert.ToInt32(FormBot.Helper.SystemEnums.JobSchedulingStatus.Pending);
                    var jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(accreditedInstallersList[0]);
                    return Json(new { status = true, accreditedData = jsonData }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                isUserExists = _userBAL.CheckAccreditationNumberExist(UserValue, userId);
            }

            if (!isUserExists.Equals(0))
            {
                //return Json(false, JsonRequestBehavior.AllowGet);
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //return Json(true, JsonRequestBehavior.AllowGet);
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// Checks the user exist.
        /// </summary>
        /// <param name="UserValue">The user value.</param>
        /// <param name="FieldName">Name of the field.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="resellerID">The reseller identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <param name="solarCompanyId">solarCompanyId.</param>
        /// <returns>Checks the user exist.</returns>
        [AllowAnonymous]
        public JsonResult CheckSWHUserExist(string LicenseNumber, string FieldName, int? userId = null, int? resellerID = null, int? UserTypeId = null, int solarCompanyId = 0)
        {
            //int licenseNumberExist = _userBAL.CheckExistLicenseNumberForSWHInstaller(LicenseNumber, solarCompanyId,null);
            object isUserExists;

            //if (!licenseNumberExist.Equals(0))
            //{
            //    return Json(new { status = false, message = "SWH Installer already exists in your account." }, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            List<AccreditedInstallers> accreditedInstallersList = _userBAL.CheckAccreditationNumberExistsInAccreditedInstallers(null, LicenseNumber, false, true, true);
            List<JobInstallerDetails> InstallerDesignerViewlist = new List<JobInstallerDetails>();
            for (int i = 0; i < accreditedInstallersList.Count; i++)
            {
                InstallerDesignerViewlist.Add(new JobInstallerDetails()
                {
                    AccreditedInstallerId = accreditedInstallersList[i].AccreditedInstallerId,
                    FullName = accreditedInstallersList[i].FullName,
                    AccreditationNumber = accreditedInstallersList[i].AccreditationNumber,
                    InstallerStatus = accreditedInstallersList[i].InstallerStatus,
                    InstallerExpiryDate = accreditedInstallersList[i].InstallerExpiryDate,
                    FullAddress = accreditedInstallersList[i].FullAddress,
                    Phone = accreditedInstallersList[i].Inst_Phone,
                    Email = accreditedInstallersList[i].Email,
                    IsSystemUser = accreditedInstallersList[i].IsSystemUser,
                    IsPVDUser = accreditedInstallersList[i].IsPVDUser,
                    IsSWHUser = accreditedInstallersList[i].IsSWHUser,
                    UserId = accreditedInstallersList[i].UserId,
                    StreetTypeID = accreditedInstallersList[i].StreetTypeID,
                    StreetNumber = accreditedInstallersList[i].MailingAddressStreetNumber,
                    StreetName = accreditedInstallersList[i].MailingAddressStreetName,
                    UnitNumber = accreditedInstallersList[i].MailingAddressUnitNumber,
                    UnitTypeID = accreditedInstallersList[i].Inst_UnitTypeID,
                    State = accreditedInstallersList[i].Abbreviation,
                    Town = accreditedInstallersList[i].MailingCity,
                    PostCode = accreditedInstallersList[i].PostalCode,
                    IsPostalAddress = accreditedInstallersList[i].IsPostalAddress,
                    FirstName = accreditedInstallersList[i].FirstName,
                    Surname = accreditedInstallersList[i].LastName,
                    Mobile = accreditedInstallersList[i].Inst_Mobile,
                    PostalDeliveryNumber = accreditedInstallersList[i].PostalDeliveryNumber,
                    PostalAddressID = accreditedInstallersList[i].PostalAddressID


                });
            }
            if (accreditedInstallersList.Count == 0)
            {
                isUserExists = 0;
                return Json(new { status = false, message = "SWH Installer with given license number doesn't exist." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //isUserExists = 1;                                       
                //accreditedInstallersList[0].RoleId = 0;
                ////int scheduleStatus = Convert.ToInt32(FormBot.Helper.SystemEnums.JobSchedulingStatus.Pending);
                var jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(InstallerDesignerViewlist);
                return Json(new { status = true, accreditedData = jsonData }, JsonRequestBehavior.AllowGet);
                //return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            //}
        }


        public JsonResult CheckVEECUserExist(string LicenseNumber, string FieldName, int? userId = null, int? resellerID = null, int? UserTypeId = null, int solarCompanyId = 0)
        {
            object isUserExists;
            //List<AccreditedInstallers> accreditedInstallersList = _userBAL.CheckAccreditationNumberExistsInAccreditedInstallers(null, LicenseNumber, false, true, true);
            List<VEECInstaller> VEECInstallerViewlist = new List<VEECInstaller>();
            VEECInstallerViewlist = _userBAL.CheckVEECInstallerExists(LicenseNumber);
            if (VEECInstallerViewlist.Count == 0)
            {
                isUserExists = 0;
                return Json(new { status = false, message = "VEEC Installer with given license number doesn't exist." }, JsonRequestBehavior.AllowGet);
            }
            else
            {

                var jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(VEECInstallerViewlist);
                return Json(new { status = true, veecInstallerData = jsonData }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// Deletes the document folder on cancel.
        /// </summary>
        /// <param name="Guid">The unique identifier.</param>
        /// <returns>Deletes the document folder on cancel.</returns>
        [AllowAnonymous]
        public JsonResult DeleteDocumentFolderOnCancel(string Guid)
        {
            if (!string.IsNullOrWhiteSpace(Guid))
            {

                string sourceDirectory = ProjectSession.ProofDocumentsURL + "\\" + "UserDocuments" + "\\" + Guid;
                if (Directory.Exists(sourceDirectory))
                {
                    Directory.Delete(sourceDirectory, true);
                }

            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Sends the email on sign up.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <param name="password">The password.</param>
        public void SendEmailOnSignUp(int userID, string password)
        {
            string FailReason = string.Empty;
            var usr = _userBAL.GetUserById(userID);
            FormBot.Entity.User uv = DBClient.DataTableToList<FormBot.Entity.User>(usr.Tables[0]).FirstOrDefault();
            uv.Password = password;

            //SMTPDetails smtpDetail = new SMTPDetails();
            //smtpDetail.MailFrom = ProjectSession.MailFrom;
            //smtpDetail.SMTPUserName = ProjectSession.SMTPUserName;
            //smtpDetail.SMTPPassword = ProjectSession.SMTPPassword;
            //smtpDetail.SMTPHost = ProjectSession.SMTPHost;
            //smtpDetail.SMTPPort = Convert.ToInt32(ProjectSession.SMTPPort);
            //smtpDetail.IsSMTPEnableSsl = ProjectSession.IsSMTPEnableSsl;

            EmailInfo emailInfo = new EmailInfo();
            emailInfo.TemplateID = 2;
            emailInfo.FirstName = uv.FirstName;
            emailInfo.LastName = uv.LastName;
            emailInfo.LoginLink = GetLoginLinkForUser(uv);
            emailInfo.UserName = uv.UserName;
            emailInfo.Password = uv.Password;

            EmailTemplate eTemplate = _emailBAL.GetEmailTemplateByID(2);
            eTemplate.Content = _emailBAL.GetEmailBody(emailInfo, eTemplate);

            EmailInfo emailInfo1 = new EmailInfo();
            emailInfo1.TemplateID = 31;
            EmailTemplate eTemplate1 = _emailBAL.GetEmailTemplateByID(31);
            string mailAddresses = string.Empty;

            StringBuilder sb = new StringBuilder();
            //string uType = uv.UserTypeID == 4 ? "Solar Company" : "Solar Electrician";
            string uType = string.Empty;
            if (uv.UserTypeID == 4)
                uType = "Solar Company";
            else if (uv.UserTypeID == 7 && uv.IsPVDUser == true)
                uType = "Solar Electrician";
            else if (uv.UserTypeID == 7 && uv.IsSWHUser == true)
                uType = "SWH User";

            sb.Append("New " + uType + " has been signup successfully with following details:<br/><br/>");
            sb.Append("<b>Name: </b>" + uv.FirstName + " " + uv.LastName + "<br/>");
            sb.Append("<b>Email: </b>" + uv.Email + "<br/>");
            sb.Append("<b>Phone: </b>" + uv.Phone + "<br/>");
            sb.Append("<b>CompanyABN: </b>" + uv.CompanyABN + "<br/>");
            sb.Append("<b>Company Name: </b>" + uv.CompanyName + "<br/>");
            if (!uv.IsPostalAddress)
            {
                string unitNameNumber = !string.IsNullOrWhiteSpace((uv.UnitTypeName + " " + uv.UnitNumber)) ? (uv.UnitTypeName + " " + uv.UnitNumber) + "/" : string.Empty;
                sb.Append("<b>Address: </b>" + unitNameNumber + uv.StreetNumber + uv.StreetName + uv.StreetTypeName + uv.Town + uv.State + uv.PostCode + "<br/>");
            }
            else
            {
                sb.Append("<b>Address: </b>" + uv.Code + uv.PostalDeliveryNumber + uv.Town + uv.State + uv.PostCode + "<br/>");
            }

            sb.Append("<b>BSB: </b>" + uv.BSB + "<br/>");
            sb.Append("<b>Account Number: </b>" + uv.AccountNumber + "<br/>");
            sb.Append("<b>Account Name: </b>" + uv.AccountName + "<br/>");


            if (uv.UserTypeID == 4)
            {
                eTemplate.Subject = eTemplate.Subject + " - Solar Company";
                eTemplate1.Subject = "New Solar Company SignUp";
                mailAddresses = _userBAL.GetRAEmailAddress(uv.ResellerID != null ? Convert.ToInt32(uv.ResellerID) : 0);
            }
            else if (uv.UserTypeID == 7 && uv.IsPVDUser == true)
            {
                eTemplate.Subject = eTemplate.Subject + " - Solar Electrician";
                eTemplate1.Subject = "New Solar Electrician SignUp";
                sb.Append("<b>CEC Accreditation Number: </b>" + uv.CECAccreditationNumber + "<br/>");
                sb.Append(!string.IsNullOrWhiteSpace(uv.ElectricalContractorsLicenseNumber) ? "<b>Licensed Electrician Number: </b>" + uv.ElectricalContractorsLicenseNumber + "<br/>" : string.Empty);
                sb.Append(!string.IsNullOrWhiteSpace(uv.CECDesignerNumber) ? "<b>CEC Designer Number: </b>" + uv.CECDesignerNumber + "<br/>" : string.Empty);
                mailAddresses = _userBAL.GetFSAFCOEmailAddresses();
            }
            else if (uv.UserTypeID == 7 && uv.IsSWHUser == true)
            {
                eTemplate.Subject = eTemplate.Subject + " - SWH User";
                eTemplate1.Subject = "New SWH User SignUp";
                sb.Append("<b>Licensed Electrician Number: </b>" + uv.ElectricalContractorsLicenseNumber + "<br/>");
                mailAddresses = _userBAL.GetFSAFCOEmailAddresses();
            }

            emailInfo1.Details = sb.ToString();
            eTemplate1.Content = _emailBAL.GetEmailBody(emailInfo1, eTemplate1);
            bool status1 = false;
            bool status = false;

            try
            {
                if (eTemplate1 != null && !string.IsNullOrWhiteSpace(eTemplate1.Content))
                {
                    QueuedEmail objQueuedEmail = new QueuedEmail();
                    objQueuedEmail.FromEmail = ProjectSession.MailFrom;
                    objQueuedEmail.Body = eTemplate1.Content;
                    objQueuedEmail.Subject = eTemplate1.Subject;
                    objQueuedEmail.ToEmail = mailAddresses;
                    objQueuedEmail.CreatedDate = DateTime.Now;
                    objQueuedEmail.ModifiedDate = DateTime.Now;
                    _emailBAL.InsertUpdateQueuedEmail(objQueuedEmail);
                    status1 = true;
                }
                if (eTemplate != null && !string.IsNullOrWhiteSpace(eTemplate.Content))
                {
                    QueuedEmail objQueuedEmail = new QueuedEmail();
                    objQueuedEmail.FromEmail = ProjectSession.MailFrom;
                    objQueuedEmail.Body = eTemplate.Content;
                    objQueuedEmail.Subject = eTemplate.Subject;
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



            //bool status1 = MailHelper.SendMail(smtpDetail, mailAddresses, null, null, eTemplate1.Subject, eTemplate1.Content, null, true, ref FailReason, false);
            //bool status = MailHelper.SendMail(smtpDetail, uv.Email, null, null, eTemplate.Subject, eTemplate.Content, null, true, ref FailReason, false);
        }

        /// <summary>
        /// Gets the comapny name by abn.
        /// </summary>
        /// <param name="companyABN">The company abn.</param>
        /// <returns>Gets the comapny name by ABN.</returns>
        [AllowAnonymous]
        public JsonResult GetComapnyNameByABN(string companyABN)
        {
            string name = _userBAL.GetComapnyNameByABN(companyABN);
            return Json(name, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Sends the email on new user creation.
        /// </summary>
        /// <param name="UserID">The user identifier.</param>
        /// <param name="password">The password.</param>
        /// <param name="userName">Name of the user.</param>
        public void SendEmailOnNewUserCreation(int UserID, string password, string userName)
        {
            EmailInfo emailInfo = new EmailInfo();
            var usr = _userBAL.GetUserById(UserID);
            FormBot.Entity.User uv = DBClient.DataTableToList<FormBot.Entity.User>(usr.Tables[0]).FirstOrDefault();
            emailInfo.TemplateID = 1;
            emailInfo.FirstName = uv.FirstName;
            emailInfo.LastName = uv.LastName;
            emailInfo.UserName = userName;
            emailInfo.Password = password;

            if (uv.UserTypeID == 7)
            {
                emailInfo.LoginLink = ConfigurationManager.AppSettings["LoginLink"].ToString();
            }

            this.ComposeAndSendEmail(emailInfo, uv.Email);
        }

        /// <summary>
        /// Composes the and send email.
        /// </summary>
        /// <param name="emailInfo">The email information.</param>
        /// <param name="mailTo">The mail to.</param>
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
                    if (body != null && !string.IsNullOrWhiteSpace(body))
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
            catch (Exception)
            {
            }
        }

        [HttpGet]
        public JsonResult GetFCOByResellerId(string rId)
        {
            int resellerId = !string.IsNullOrWhiteSpace(rId) ? Convert.ToInt32(rId) : 0;
            List<SelectListItem> items = _userBAL.GetFCOByResellerId(resellerId).Select(a => new SelectListItem { Text = a.Name, Value = a.UserId.ToString() }).ToList();
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the requesting sc afor SSC.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="email">The email.</param>
        /// <param name="companyname">The companyname.</param>
        /// <param name="companyabn">The companyabn.</param>
        /// <param name="electricianstatus">The electricianstatus.</param>
        public void GetRequestingSCAforSSC(string name = "", string email = "", string companyname = "", string companyabn = "", string electricianstatus = "")
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);
            int status = !string.IsNullOrWhiteSpace(electricianstatus) ? Convert.ToInt32((SystemEnums.ElectricianStatus)Enum.Parse(typeof(SystemEnums.ElectricianStatus), electricianstatus).GetHashCode()) : 0;

            IList<FormBot.Entity.User> lstUser = _userBAL.GetRequestingSCAforSSC(ProjectSession.LoggedInUserId, pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, name, email, companyname, companyabn, status);
            if (lstUser.Count > 0)
            {
                gridParam.TotalDisplayRecords = lstUser.FirstOrDefault().TotalRecords;
                gridParam.TotalRecords = lstUser.FirstOrDefault().TotalRecords;
            }

            HttpContext.Response.Write(Grid.PrepareDataSet(lstUser, gridParam));
        }

        /// <summary>
        /// Deletes the directory.
        /// </summary>
        /// <param name="path">The path.</param>
        private void DeleteDirectory(string path)
        {
            if (System.IO.File.Exists(path))
            {
                ////Delete all files from the Directory
                System.IO.File.Delete(path);
            }
        }

        /// <summary>
        /// Gets the login link for user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>string</returns>
        public string GetLoginLinkForUser(FormBot.Entity.User user)
        {
            if (user.UserTypeID == 1 || user.UserTypeID == 3 || user.UserTypeID == 6 || user.UserTypeID == 7 || user.UserTypeID == 9)
            {
                return ProjectSession.LoginLink;
            }
            else if (user.UserTypeID == 2)
            {
                Reseller reseller = _resellerBAL.GetResellerByResellerID(user.ResellerID);
                return ProjectSession.LoginLink + reseller.LoginCompanyName;
            }
            else if (user.UserTypeID == 4)
            {
                if (user.ResellerID != null && user.ResellerID > 0)
                {
                    Reseller reseller = _resellerBAL.GetResellerByResellerID(user.ResellerID);
                    return ProjectSession.LoginLink + reseller.LoginCompanyName;
                }
                else
                {
                    return ProjectSession.LoginLink;
                }
            }
            else if (user.UserTypeID == 5)
            {
                Reseller reseller = _resellerBAL.GetResellerByResellerID(user.ResellerID);
                return ProjectSession.LoginLink + reseller.LoginCompanyName;
            }
            else if (user.UserTypeID == 8)
            {
                SolarCompany solarConmpany = _solarCompanyBAL.GetSolarCompanyBySolarCompanyID(user.SolarCompanyId);
                switch (solarConmpany.IsSubContractor)
                {
                    case 0:
                        Reseller reseller = _resellerBAL.GetResellerByResellerID(solarConmpany.ResellerID);
                        return ProjectSession.LoginLink + reseller.LoginCompanyName;
                        break;
                    case 1:
                        return ProjectSession.LoginLink;
                        break;
                    default:
                        return ProjectSession.LoginLink;
                        break;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Compliances the check detail.
        /// </summary>
        /// <param name="userView1">The user view1.</param>
        /// <param name="userView2">The user view2.</param>
        /// <returns>string</returns>
        public string ComplianceCheckDetail(FormBot.Entity.User userView1, FormBot.Entity.User userView2)
        {
            if (!userView1.Equals(userView2))
            {
                StringBuilder sb = new StringBuilder();
                var props = userView1.GetType().GetProperties();
                foreach (PropertyInfo pro in props)
                {
                    if (pro.Name == "FirstName" || pro.Name == "LastName" || pro.Name == "Email" || pro.Name == "Phone" || pro.Name == "Mobile" || pro.Name == "Note" || pro.Name == "CompanyABN" || pro.Name == "CompanyName" || pro.Name == "CompanyWebsite"
                          || pro.Name == "AddressID" || pro.Name == "PostalAddressID" || pro.Name == "PostalDeliveryNumber" || pro.Name == "Town" || pro.Name == "State" || pro.Name == "PostCode" || pro.Name == "UnitTypeID" || pro.Name == "UnitNumber"
                          || pro.Name == "StreetNumber" || pro.Name == "StreetName" || pro.Name == "StreetTypeID" || pro.Name == "BSB" || pro.Name == "AccountNumber" || pro.Name == "AccountName" || pro.Name == "IsSTC" || pro.Name == "CECAccreditationNumber"
                          || pro.Name == "CECDesignerNumber" || pro.Name == "ElectricalContractorsLicenseNumber" || pro.Name == "SEDesigner")
                    {
                        string value1 = pro.GetValue(userView1) != null ? pro.GetValue(userView1).ToString() : string.Empty;
                        string value2 = pro.GetValue(userView2) != null ? pro.GetValue(userView2).ToString() : string.Empty;

                        if (value1 != value2)
                        {
                            if (pro.Name == "AddressID")
                            {
                                int value = Convert.ToInt32(value2);
                                //SystemEnums.PostalAddressType enumPostalAddress = (SystemEnums.PostalAddressType)value;
                                string postalAddressType = Enum.GetName(typeof(SystemEnums.PostalAddressType), value);
                                sb.Append("Postal Address Type: " + postalAddressType + "<br/>");
                            }
                            else if (pro.Name == "UnitTypeID")
                            {
                                string unitName = _unitTypeBAL.GetUnitnameByUnitId(Convert.ToInt32(value2));
                                sb.Append("Unit Name: " + unitName + "<br/>");
                            }
                            else if (pro.Name == "StreetTypeID")
                            {
                                string streetTypeName = _streetTypeBAL.GetStreetTypeNameByStreetTypeId(Convert.ToInt32(value2));
                                sb.Append("Street Type: " + streetTypeName + "<br/>");
                            }
                            else if (pro.Name == "PostalAddressID")
                            {
                                string postalDeliveryName = _userBAL.GetPostalDeliveryNameByID(Convert.ToInt32(value2));
                                sb.Append("Postal Delivery Type: " + postalDeliveryName + "<br/>");
                            }
                            else if (pro.Name == "SEDesigner")
                            {
                                int value = Convert.ToInt32(value2);
                                if (value != 0)
                                {
                                    sb.Append("SE Role: " + (Enum.GetName(typeof(SystemEnums.SEDesignRole), value)).Replace('_', ' ') + "<br/>");
                                }
                            }
                            else
                            {
                                sb.Append(pro.Name + ": " + value2 + "<br/>");
                            }
                        }
                    }
                }
                if (sb != null && sb.Length > 4)
                {
                    sb.Remove(sb.Length - 5, 5);
                }
                return sb.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Deletes the request to se.
        /// </summary>
        /// <param name="solarcompanyid">The solarcompanyid.</param>
        /// <param name="electricianid">The electricianid.</param>
        /// <returns>ActionResult</returns>
        [UserAuthorization]
        public ActionResult DeleteRequestToSE(string solarcompanyid, string electricianid)
        {
            int eID = 0;
            int SolarCompanyID = !string.IsNullOrWhiteSpace(solarcompanyid) ? Convert.ToInt32(solarcompanyid) : 0;

            if (!string.IsNullOrWhiteSpace(electricianid))
            {
                int.TryParse(QueryString.GetValueFromQueryString(electricianid, "id"), out eID);
            }

            if (eID > 0 && SolarCompanyID > 0)
            {
                _userBAL.DeleteRequestToSE(eID, SolarCompanyID);
                return this.Json(new { success = true });
            }
            else
            {
                return this.Json(new { success = false });
            }
        }

        /// <summary>
        /// Gets the solar sub contractor.
        /// </summary>
        /// <returns>JsonResult</returns>
        [HttpGet]
        public JsonResult GetSolarSubContractor()
        {
            List<SelectListItem> items = _userBAL.GetSolarSubContractor().Select(a => new SelectListItem { Text = a.SolarSubContractorName, Value = a.SolarSubContractorID.ToString() }).ToList();
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> AddInstallerDesigner(InstallerDesignerView installerDesignerView, int jobId = 0, int profileType = 0, string signPath = null, int accreditedInstallerId = 0, bool isSWHInstaller = false, int userId = 0)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(installerDesignerView.CECAccreditationNumber))
                {
                    installerDesignerView.CECAccreditationNumber = installerDesignerView.CECAccreditationNumber.Trim();
                }


                installerDesignerView.IsPostalAddress = installerDesignerView.AddressID == 2 ? true : false;

                ModelState.Remove("installerDesignerView.FindInstallerDesignerFirstName");
                ModelState.Remove("installerDesignerView.FindInstallerDesignerLastName");
                ModelState.Remove("FindInstallerDesignerFirstName");
                ModelState.Remove("FindInstallerDesignerLastName");
                ModelState.Remove("installerDesignerView.SWHLicenseNumber");
                ModelState.Remove("SWHLicenseNumber");

                if (installerDesignerView.IsSWHUser)
                {
                    ModelState.Remove("installerDesignerView.CECAccreditationNumber");
                    ModelState.Remove("CECAccreditationNumber");
                }


                //AddEdit swh intaller from job detail page
                //if ((accreditedInstallerId > 0 || userId > 0) && !isEditSWHInstaller)
                if ((accreditedInstallerId > 0 || userId > 0) && isSWHInstaller)
                {
                    RemoveInstallerDesignerValidation();
                    installerDesignerView.IsSWHUser = true;
                }
                //if(string.IsNullOrEmpty( installerDesignerView.FirstName))
                //    ModelState.AddModelError("installerDesignerView.FirstName", "FirstName is required.");
                //if (string.IsNullOrEmpty(installerDesignerView.LastName))
                //    ModelState.AddModelError("installerDesignerView.LastName", "LastName/Surname is required.");
                //if (string.IsNullOrEmpty(installerDesignerView.Town))
                //    ModelState.AddModelError("installerDesignerView.Town", "Town is required.");
                //if (string.IsNullOrEmpty(installerDesignerView.State))
                //    ModelState.AddModelError("installerDesignerView.State", "State is required.");
                //if (string.IsNullOrEmpty(installerDesignerView.PostCode))
                //    ModelState.AddModelError("installerDesignerView.PostCode", "PostCode is required.");

                if (installerDesignerView.IsPostalAddress == false)
                {
                    if (installerDesignerView.UnitTypeID > 0 && string.IsNullOrWhiteSpace(installerDesignerView.UnitNumber))
                        ModelState.AddModelError("installerDesignerView.UnitNumber", "UnitNumber is required.");

                    if (!string.IsNullOrWhiteSpace(installerDesignerView.UnitNumber) && installerDesignerView.UnitTypeID == 0)
                        ModelState.AddModelError("installerDesignerView.UnitTypeID", "UnitType is required.");
                    if (string.IsNullOrWhiteSpace(installerDesignerView.UnitNumber) && string.IsNullOrWhiteSpace(installerDesignerView.StreetNumber))
                        ModelState.AddModelError("installerDesignerView.StreetNumber", "StreetNumber is required.");
                    if (string.IsNullOrWhiteSpace(installerDesignerView.StreetName))
                        ModelState.AddModelError("installerDesignerView.StreetName", "StreetName is required.");
                    if (installerDesignerView.StreetTypeID == 0)
                        ModelState.AddModelError("installerDesignerView.StreetTypeID", "StreetType is required.");
                }
                else
                {
                    if (installerDesignerView.PostalAddressID == 0)
                        ModelState.AddModelError("installerDesignerView.PostalAddressID", "Postal Delivery Type is required.");
                    if (string.IsNullOrWhiteSpace(installerDesignerView.PostalDeliveryNumber))
                        ModelState.AddModelError("installerDesignerView.PostalAddressID", "Postal Delivery Number is required.");
                }
                if (ModelState.IsValid)
                {
                    #region Installer / Designer
                    List<AccreditedInstallers> accreditedInstallersList = new List<AccreditedInstallers>();

                    if (installerDesignerView.InstallerDesignerId > 0)
                    {
                        installerDesignerView.ModifiedBy = ProjectSession.LoggedInUserId;
                    }
                    else
                    {
                        if (!installerDesignerView.IsSWHUser)
                        {
                            int accreditationNumberExist = _userBAL.CheckExistAccreditationNumberForInstallerDesigner(installerDesignerView.CECAccreditationNumber, installerDesignerView.SolarCompanyId);
                            if (!accreditationNumberExist.Equals(0))
                            {
                                return Json(new { status = false, message = "Installer/Designer already exists in your account." }, JsonRequestBehavior.AllowGet);
                            }
                            accreditedInstallersList = _userBAL.CheckAccreditationNumberExistsInAccreditedInstallers(installerDesignerView.CECAccreditationNumber, null, true, false);
                        }
                        else
                        {
                            int licenseNumberExist = _userBAL.CheckExistLicenseNumberForSWHInstaller(installerDesignerView.ElectricalContractorsLicenseNumber, installerDesignerView.SolarCompanyId, installerDesignerView.Email);
                            if (!licenseNumberExist.Equals(0))
                            {
                                return Json(new { status = false, message = "SWH Installer already exists in your account." }, JsonRequestBehavior.AllowGet);
                            }
                        }

                        //installerDesignerView.CreatedBy = ProjectSession.LoggedInUserId;
                        installerDesignerView.CreatedBy = installerDesignerView.CreatedBy;
                    }

                    installerDesignerView.IsDeleted = false;

                    signPath = !string.IsNullOrWhiteSpace(signPath) ? signPath.Replace(ProjectSession.ProofDocumentsURL, "").Replace(ProjectSession.UploadedDocumentPath, "") : null;
                    if (installerDesignerView.InstallerDesignerId > 0)
                    {
                        int InstallerDesignerId = _userBAL.AddEditInstallerDesigner(installerDesignerView, jobId, profileType, signPath, accreditedInstallerId, userId);
                        if (InstallerDesignerId > 0)
                        {
                            await CommonBAL.SetCacheDataForInstallerDesignerID(InstallerDesignerId);
                            if (jobId > 0)
                            {
                                await CommonBAL.SetCacheDataForJobID(installerDesignerView.SolarCompanyId, jobId);
                            }
                            return Json(new { status = true, message = "Installer/Designer has been updated successfully.", InstallerDesignerId = InstallerDesignerId, RoleId = installerDesignerView.SEDesignRoleId }, JsonRequestBehavior.AllowGet);
                        }
                        else
                            return Json(new { status = true, message = "Installer/Designer has not been updated." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (accreditedInstallersList.Count > 0)
                        {
                            SolarElectricianView solarElectricianView = new SolarElectricianView();
                            solarElectricianView.FirstName = installerDesignerView.FirstName;
                            solarElectricianView.LastName = installerDesignerView.LastName;
                            solarElectricianView.Email = installerDesignerView.Email;
                            solarElectricianView.State = installerDesignerView.State;
                            solarElectricianView.SEDesignRoleId = installerDesignerView.SEDesignRoleId;
                            solarElectricianView.ElectricalContractorsLicenseNumber = installerDesignerView.ElectricalContractorsLicenseNumber;
                            solarElectricianView.CECDesignerNumber = installerDesignerView.CECDesignerNumber;
                            solarElectricianView.CECAccreditationNumber = installerDesignerView.CECAccreditationNumber;
                            solarElectricianView.AddressID = installerDesignerView.AddressID;
                            solarElectricianView.UnitTypeName = installerDesignerView.UnitTypeName;
                            solarElectricianView.UnitTypeID = installerDesignerView.UnitTypeID;
                            solarElectricianView.UnitNumber = installerDesignerView.UnitNumber;
                            solarElectricianView.StreetNumber = installerDesignerView.StreetNumber;
                            solarElectricianView.StreetName = installerDesignerView.StreetName;
                            solarElectricianView.StreetTypeName = installerDesignerView.StreetTypeName;
                            solarElectricianView.StreetTypeID = installerDesignerView.StreetTypeID;
                            solarElectricianView.Code = installerDesignerView.Code;
                            solarElectricianView.PostalDeliveryNumber = installerDesignerView.PostalDeliveryNumber;
                            solarElectricianView.IsActiveDiv = installerDesignerView.IsActiveDiv;

                            installerDesignerView.IsPVDUser = true;
                            int InstallerDesignerId = _userBAL.AddEditInstallerDesigner(installerDesignerView, jobId, profileType, signPath, accreditedInstallerId, userId);
                            if (InstallerDesignerId > 0)
                            {
                                await CommonBAL.SetCacheDataForInstallerDesignerID(InstallerDesignerId);
                                if (jobId > 0)
                                {
                                    await CommonBAL.SetCacheDataForJobID(ProjectSession.SolarCompanyId, jobId);
                                }
                                return Json(new { status = true, message = "Installer/Designer has been added successfully.", InstallerDesignerId = InstallerDesignerId, RoleId = installerDesignerView.SEDesignRoleId }, JsonRequestBehavior.AllowGet);
                            }
                            else
                                return Json(new { status = true, message = "Installer/Designer has not been added." }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            if (installerDesignerView.IsSWHUser)
                            {
                                installerDesignerView.IsSWHUser = true;
                                int InstallerDesignerId = _userBAL.AddEditInstallerDesigner(installerDesignerView, jobId, profileType, signPath, accreditedInstallerId, userId);
                                if (InstallerDesignerId > 0)
                                {
                                    await CommonBAL.SetCacheDataForInstallerDesignerID(InstallerDesignerId);
                                    if (jobId > 0)
                                    {
                                        await CommonBAL.SetCacheDataForJobID(ProjectSession.SolarCompanyId, jobId);
                                    }
                                    return Json(new { status = true, message = "SWH Installer has been added successfully.", InstallerDesignerId = InstallerDesignerId, RoleId = installerDesignerView.SEDesignRoleId }, JsonRequestBehavior.AllowGet);
                                }
                                else
                                    return Json(new { status = true, message = "SWH Installer has not been added." }, JsonRequestBehavior.AllowGet);
                            }
                            else
                                return Json(new { status = false, message = "Installer/Designer with given accreditation number doesn't exist." }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    #endregion
                }
                else
                {
                    string errorMsg = string.Empty;
                    errorMsg = String.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage + " " + v.Exception));
                    return Json(new { status = false, message = errorMsg }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public void GetInstallerDesignerAddByProfile(string solarcompanyid1 = "", string name = "", string accreditationnumber = "", string SERole = "", string SendBy = "", string LicenseNumber = "", bool IsSWHUser = false)
        {
            //int SolarCompanyId = 0;
            //if (ProjectSession.UserTypeId == 4 || ProjectSession.UserTypeId == 6 || ProjectSession.UserTypeId == 8)
            //{
            //    SolarCompanyId = ProjectSession.SolarCompanyId;
            //}

            int SolarCompanyId = 0;
            if (ProjectSession.UserTypeId == 1 || ProjectSession.UserTypeId == 2 || ProjectSession.UserTypeId == 3 || ProjectSession.UserTypeId == 5)
            {
                SolarCompanyId = !string.IsNullOrWhiteSpace(solarcompanyid1) ? Convert.ToInt32(solarcompanyid1) : 0;
            }
            if (ProjectSession.UserTypeId == 4 || ProjectSession.UserTypeId == 6 || ProjectSession.UserTypeId == 8)
            {
                SolarCompanyId = ProjectSession.SolarCompanyId;
            }

            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);

            int sendByValue = 0;

            if (ProjectSession.UserTypeId == 4 || ProjectSession.UserTypeId == 8)
                sendByValue = 1;
            else if (ProjectSession.UserTypeId == 6)
                sendByValue = 2;
            else
                sendByValue = !string.IsNullOrWhiteSpace(SendBy) ? Convert.ToInt32(SendBy) : 0;

            int role = !string.IsNullOrWhiteSpace(SERole) ? Convert.ToInt32((SystemEnums.SEDesignRole)Enum.Parse(typeof(SystemEnums.SEDesignRole), SERole).GetHashCode()) : 0;

            IList<InstallerDesignerView> lstUser = _userBAL.GetInstallerDesignerAddByProfile(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, gridParam.PageSize, pageNumber, gridParam.SortCol, gridParam.SortDir, SolarCompanyId, name, accreditationnumber, role, sendByValue, LicenseNumber, IsSWHUser);
            if (lstUser.Count > 0)
            {
                gridParam.TotalDisplayRecords = lstUser.FirstOrDefault().TotalRecords;
                gridParam.TotalRecords = lstUser.FirstOrDefault().TotalRecords;
            }
            HttpContext.Response.Write(Grid.PrepareDataSet(lstUser, gridParam));
        }

        [HttpGet]
        public PartialViewResult GetInstallerDesigner()
        {
            FormBot.Entity.User user = new FormBot.Entity.User();
            user.UserTypeID = ProjectSession.UserTypeId;

            if (ProjectSession.UserTypeId == 1 || ProjectSession.UserTypeId == 2 || ProjectSession.UserTypeId == 3 || ProjectSession.UserTypeId == 4 || ProjectSession.UserTypeId == 5 || ProjectSession.UserTypeId == 6 || ProjectSession.UserTypeId == 8)
            {
                var statuses = from SystemEnums.SEDesignRole s in Enum.GetValues(typeof(SystemEnums.SEDesignRole))
                               select new { ID = s, Name = s.ToString().Replace('_', ' ') };
                ViewData["SEDesignRole"] = new SelectList(statuses, "ID", "Name");
            }
            return PartialView("InstallerDesigner", user);
        }

        [HttpGet]
        public ActionResult FillInstallerDesigner(string InstallerDesignerId, int? jobId)
        {
            InstallerDesignerView installerDesignerView = new InstallerDesignerView();
            if (!string.IsNullOrWhiteSpace(InstallerDesignerId) && Convert.ToInt32(InstallerDesignerId) > 0)
            {
                installerDesignerView = _userBAL.GetInstallerDesigner(Convert.ToInt32(InstallerDesignerId), jobId);
                //installerDesignerView.UserId = _userBAL.CheckAccreditationNumber(installerDesignerView.CECAccreditationNumber,SystemEnums.UserType.SolarElectricians.GetHashCode());
                //if (installerDesignerView.IsSystemUser && !installerDesignerView.IsSolarElectrician)
                //{
                //    SolarElectricianView solarElectricianView = new SolarElectricianView();

                //    solarElectricianView.CECAccreditationNumber = installerDesignerView.CECAccreditationNumber;
                //    solarElectricianView.CECDesignerNumber = installerDesignerView.CECDesignerNumber;
                //    solarElectricianView.IsDeleted = false;
                //    solarElectricianView.UserId = installerDesignerView.UserId;
                //    solarElectricianView.ElectricianStatusId = 2;
                //    solarElectricianView.SERole = installerDesignerView.SEDesignRoleId;
                //    solarElectricianView.SolarCompanyId = installerDesignerView.SolarCompanyId;
                //    solarElectricianView.ElectricalContractorsLicenseNumber = installerDesignerView.ElectricalContractorsLicenseNumber; // For SWH user

                //    SendSERequest(solarElectricianView);
                //}

            }
            return Json(installerDesignerView, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Deletes the request to se.
        /// </summary>
        /// <param name="solarcompanyid">The solarcompanyid.</param>
        /// <param name="electricianid">The electricianid.</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult DeleteRequestToSEForInstallerDesigner(string solarcompanyid, string electricianid)
        {
            int SolarCompanyID = !string.IsNullOrWhiteSpace(solarcompanyid) ? Convert.ToInt32(solarcompanyid) : 0;
            int eID = !string.IsNullOrWhiteSpace(electricianid) ? Convert.ToInt32(electricianid) : 0;

            if (eID > 0 && SolarCompanyID > 0)
            {
                _userBAL.DeleteRequestToSEForInstallerDesigner(eID, SolarCompanyID);
                return this.Json(new { success = true });
            }
            else
            {
                return this.Json(new { success = false });
            }
        }

        public void RemoveInstallerDesignerValidation()
        {
            ModelState.Remove("installerDesignerView.FirstName");
            ModelState.Remove("installerDesignerView.LastName");
            ModelState.Remove("installerDesignerView.FindInstallerDesignerFirstName");
            ModelState.Remove("installerDesignerView.FindInstallerDesignerLastName");
            ModelState.Remove("installerDesignerView.CECAccreditationNumber");
            ModelState.Remove("installerDesignerView.UnitTypeID");
            ModelState.Remove("installerDesignerView.PostalAddressID");
            ModelState.Remove("installerDesignerView.StreetTypeID");
            ModelState.Remove("installerDesignerView.StreetNumber");
            ModelState.Remove("installerDesignerView.StreetName");

            ModelState.Remove("installerDesignerView.Town");
            ModelState.Remove("installerDesignerView.State");
            ModelState.Remove("installerDesignerView.PostCode");
            ModelState.Remove("installerDesignerView.Phone");
            ModelState.Remove("installerDesignerView.Mobile");
            ModelState.Remove("installerDesignerView.Email");
            ModelState.Remove("installerDesignerView.SWHLicenseNumber");
        }

        ///// <summary>
        ///// Get all users by userTypeId.
        ///// </summary>
        ///// <param name="userTypeId">userTypeId</param>
        ///// <param name="isTypeChange">isTypeChange</param>
        ///// <returns>JsonResult</returns>
        //[HttpGet]
        //public JsonResult GetUserByUserTypeId(string userTypeId, string isTypeChange)
        //{
        //    if (!string.IsNullOrEmpty(isTypeChange) && Convert.ToInt32(isTypeChange) == 1)
        //        ProjectSession.SystemUsersOfUserTypeTable = null;

        //    if (ProjectSession.SystemUsersOfUserTypeTable != null && ProjectSession.SystemUsersOfUserTypeTable.Rows.Count > 0)
        //    {
        //        DataTable dtUser = ProjectSession.SystemUsersOfUserTypeTable;
        //        List<SelectListItem> items = new List<SelectListItem>();
        //        for (int i = 0; i < dtUser.Rows.Count; i++)
        //        {
        //            items.Add(new SelectListItem()
        //            {
        //                Text = dtUser.Rows[i]["Fullname"].ToString(),
        //                Value = dtUser.Rows[i]["UserId"].ToString()
        //            });
        //        }
        //        return Json(items, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        List<SelectListItem> items = GetAllUsers(userTypeId);
        //        DataTable dtUser = new DataTable();
        //        dtUser.Columns.Add("UserId", typeof(string));
        //        dtUser.Columns.Add("Fullname", typeof(string));
        //        for (int i = 0; i < items.Count; i++)
        //        {
        //            dtUser.Rows.Add(new object[] { items[i].Value, items[i].Text });
        //        }
        //        ProjectSession.SystemUsersOfUserTypeTable = dtUser;
        //        return Json(items, JsonRequestBehavior.AllowGet);
        //    }
        //}

        /// <summary>
        /// Get all users by userTypeId.
        /// </summary>
        /// <param name="userTypeId">userTypeId</param>
        /// <param name="isTypeChange">isTypeChange</param>
        /// <returns>JsonResult</returns>
        [HttpGet]
        public JsonResult GetUserByUserTypeId(string userTypeId, string isTypeChange)
        {
            if (!string.IsNullOrWhiteSpace(isTypeChange) && Convert.ToInt32(isTypeChange) == 1)
                ProjectSession.SystemUsersOfUserType = null;

            if (ProjectSession.SystemUsersOfUserType != null)
                return Json(ProjectSession.SystemUsersOfUserType, JsonRequestBehavior.AllowGet);
            else
            {
                List<SelectListItem> items = GetAllUsers(userTypeId);
                ProjectSession.SystemUsersOfUserType = JsonConvert.SerializeObject(items);
                return Json(items, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Get all user
        /// </summary>
        /// <param name="userTypeId">userTypeId</param>
        /// <returns>List</returns>
        public List<SelectListItem> GetAllUsers(string userTypeId)
        {
            return _userBAL.GetUserByUserType(string.IsNullOrWhiteSpace(userTypeId) ? 0 : Convert.ToInt32(userTypeId)).Select(a => new SelectListItem { Text = a.Fullname, Value = a.UserId.ToString() }).ToList();
        }

        ///// <summary>
        ///// Get all users by resellerId.
        ///// </summary>
        ///// <param name="userTypeId">userTypeId</param>
        ///// <param name="resellerId">resellerId</param>
        ///// <param name="isTypeChange">isTypeChange</param>
        ///// <returns>JsonResult</returns>
        //[HttpGet]
        //public JsonResult GetUserByResellerId(string userTypeId, string resellerId, string isTypeChange)
        //{
        //    if (!string.IsNullOrEmpty(isTypeChange) && Convert.ToInt32(isTypeChange) == 1)
        //        ProjectSession.SystemUsersOfUserTypeTable = null;

        //    if (ProjectSession.SystemUsersOfUserTypeTable != null && ProjectSession.SystemUsersOfUserTypeTable.Rows.Count > 0)
        //    {
        //        DataTable dtUser = ProjectSession.SystemUsersOfUserTypeTable;
        //        List<SelectListItem> items = new List<SelectListItem>();
        //        for (int i = 0; i < dtUser.Rows.Count; i++)
        //        {
        //            items.Add(new SelectListItem()
        //            {
        //                Text = dtUser.Rows[i]["Fullname"].ToString(),
        //                Value = dtUser.Rows[i]["UserId"].ToString()
        //            });
        //        }
        //        return Json(items, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        List<SelectListItem> items = _userBAL.GetUserByResellerId(string.IsNullOrEmpty(userTypeId) ? 0 : Convert.ToInt32(userTypeId), string.IsNullOrEmpty(resellerId) ? 0 : Convert.ToInt32(resellerId)).Select(a => new SelectListItem { Text = a.Fullname, Value = a.UserId.ToString() }).ToList();
        //        DataTable dtUser = new DataTable();
        //        dtUser.Columns.Add("UserId", typeof(string));
        //        dtUser.Columns.Add("Fullname", typeof(string));
        //        for (int i = 0; i < items.Count; i++)
        //        {
        //            dtUser.Rows.Add(new object[] { items[i].Value, items[i].Text });
        //        }
        //        ProjectSession.SystemUsersOfUserTypeTable = dtUser;
        //        return Json(items, JsonRequestBehavior.AllowGet);
        //    }
        //}

        /// <summary>
        /// Get all users by resellerId.
        /// </summary>
        /// <param name="userTypeId">userTypeId</param>
        /// <param name="resellerId">resellerId</param>
        /// <param name="isTypeChange">isTypeChange</param>
        /// <returns>JsonResult</returns>
        [HttpGet]
        public JsonResult GetUserByResellerId(string userTypeId, string resellerId, string isTypeChange)
        {
            if (!string.IsNullOrWhiteSpace(isTypeChange) && Convert.ToInt32(isTypeChange) == 1)
                ProjectSession.SystemUsersOfUserType = null;

            if (ProjectSession.SystemUsersOfUserType != null)
                return Json(ProjectSession.SystemUsersOfUserType, JsonRequestBehavior.AllowGet);
            else
            {
                List<SelectListItem> items = _userBAL.GetUserByResellerId(string.IsNullOrWhiteSpace(userTypeId) ? 0 : Convert.ToInt32(userTypeId), string.IsNullOrWhiteSpace(resellerId) ? 0 : Convert.ToInt32(resellerId)).Select(a => new SelectListItem { Text = a.Fullname, Value = a.UserId.ToString() }).ToList();
                ProjectSession.SystemUsersOfUserType = JsonConvert.SerializeObject(items);
                return Json(items, JsonRequestBehavior.AllowGet);
            }

        }

        ///// <summary>
        ///// Get all users by solar company id.
        ///// </summary>
        ///// <param name="userTypeId">userTypeId</param>
        ///// <param name="solarCompanyId">solarCompanyId</param>
        ///// <param name="isTypeChange">isTypeChange</param>
        ///// <returns>JsonResult</returns>
        //[HttpGet]
        //public JsonResult GetUserBySolarCompanyId(string userTypeId, string solarCompanyId, string isTypeChange)
        //{
        //    if (!string.IsNullOrEmpty(isTypeChange) && Convert.ToInt32(isTypeChange) == 1)
        //        ProjectSession.SystemUsersOfUserTypeTable = null;

        //    if (ProjectSession.SystemUsersOfUserTypeTable != null && ProjectSession.SystemUsersOfUserTypeTable.Rows.Count > 0)
        //    {
        //        DataTable dtUser = ProjectSession.SystemUsersOfUserTypeTable;
        //        List<SelectListItem> items = new List<SelectListItem>();
        //        for (int i = 0; i < dtUser.Rows.Count; i++)
        //        {
        //            items.Add(new SelectListItem()
        //            {
        //                Text = dtUser.Rows[i]["Fullname"].ToString(),
        //                Value = dtUser.Rows[i]["UserId"].ToString()
        //            });
        //        }
        //        return Json(items, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        List<SelectListItem> items = _userBAL.GetUserBySolarCompanyId(string.IsNullOrEmpty(userTypeId) ? 0 : Convert.ToInt32(userTypeId), string.IsNullOrEmpty(solarCompanyId) ? 0 : Convert.ToInt32(solarCompanyId)).Select(a => new SelectListItem { Text = a.Fullname, Value = a.UserId.ToString() }).ToList();
        //        DataTable dtUser = new DataTable();
        //        dtUser.Columns.Add("UserId", typeof(string));
        //        dtUser.Columns.Add("Fullname", typeof(string));
        //        for (int i = 0; i < items.Count; i++)
        //        {
        //            dtUser.Rows.Add(new object[] { items[i].Value, items[i].Text });
        //        }
        //        ProjectSession.SystemUsersOfUserTypeTable = dtUser;
        //        return Json(items, JsonRequestBehavior.AllowGet);
        //    }
        //}

        /// <summary>
        /// Get all users by solar company id.
        /// </summary>
        /// <param name="userTypeId">userTypeId</param>
        /// <param name="solarCompanyId">solarCompanyId</param>
        /// <param name="isTypeChange">isTypeChange</param>
        /// <returns>JsonResult</returns>
        [HttpGet]
        public JsonResult GetUserBySolarCompanyId(string userTypeId, string solarCompanyId, string isTypeChange)
        {

            if (!string.IsNullOrWhiteSpace(isTypeChange) && Convert.ToInt32(isTypeChange) == 1)
                ProjectSession.SystemUsersOfUserType = null;

            if (ProjectSession.SystemUsersOfUserType != null)
                return Json(ProjectSession.SystemUsersOfUserType, JsonRequestBehavior.AllowGet);
            else
            {
                List<SelectListItem> items = _userBAL.GetUserBySolarCompanyId(string.IsNullOrWhiteSpace(userTypeId) ? 0 : Convert.ToInt32(userTypeId), string.IsNullOrWhiteSpace(solarCompanyId) ? 0 : Convert.ToInt32(solarCompanyId)).Select(a => new SelectListItem { Text = a.Fullname, Value = a.UserId.ToString() }).ToList();
                ProjectSession.SystemUsersOfUserType = JsonConvert.SerializeObject(items);
                return Json(items, JsonRequestBehavior.AllowGet);
            }
        }

        public void XeroDetailsCheck(int XeroContactAlreadyExists,
            IEnumerable<Contact> lstContactWithABN,
            FormBot.Entity.User userView,
            out List<Contact> contact)
        {
            contact = lstContactWithABN.ToList();
            //contact = lstContactWithABN.Where(x => x.Name.Equals((!string.IsNullOrEmpty(userView.CustomCompanyName) ? userView.CustomCompanyName : userView.CompanyName))).ToList();
            if (!contact.Any() && !string.IsNullOrWhiteSpace(userView.ClientNumber))
                contact = lstContactWithABN.Where(x => x.AccountNumber == userView.ClientNumber).ToList();
            //if (XeroContactAlreadyExists == 1)
            //	contact = lstContactWithABN.Where(x => x.AccountNumber == userView.ClientNumber).ToList();
            //else
            //{
            //contact = lstContactWithABN.Where(x => x.Name.Equals((!string.IsNullOrEmpty(userView.CustomCompanyName) ? userView.CustomCompanyName : userView.CompanyName))).ToList();
            //if (!contact.Any())
            //{
            //	if (string.IsNullOrEmpty(userView.CustomCompanyName))
            //		contact = lstContactWithABN.Where(x => x.Name.Equals(userView.CompanyName)).ToList();
            //}
            //contact = lstContactWithABN.Where(x => x.AccountNumber == userView.ClientNumber).ToList();
            //}

        }
        /// <summary>
        /// Check contact in xero and system.
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="isAllowInsert">isAllowInsert</param>
        /// <returns>ActionResult</returns>
        public ActionResult CheckInXero(string userId, bool isAllowInsert, int XeroContactAlreadyExists = 0)
        {
            if (ProjectSession.LoggedInUserId == 0)
                return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);

            try
            {
                #region Xero outh1.0
                //if (XeroApiHelper.xeroApiHelperSession != null)
                //{
                //    var dsUsers = _userBAL.GetUserById((!string.IsNullOrEmpty(userId) ? Convert.ToInt32(userId) : 0));
                //    List<FormBot.Entity.User> user = DBClient.DataTableToList<FormBot.Entity.User>(dsUsers.Tables[0]);
                //    FormBot.Entity.User userView = user.FirstOrDefault();
                //    userView.AddressID = userView.IsPostalAddress ? 2 : 1;
                //    if (userView.WholesalerIsPostalAddress == 1)
                //    {
                //        userView.WholesalerIsPostalAddress = 2;
                //    }
                //    else
                //    {
                //        userView.WholesalerIsPostalAddress = 1;
                //    }

                //    FormBot.Entity.XeroContact xeroContact = new Entity.XeroContact();
                //    var api = XeroApiHelper.xeroApiHelperSession.CoreApi();
                //    var expected = userView.IsWholeSaler ? userView.WholesalerCompanyABN : userView.CompanyABN;
                //    var contact = new List<Contact>();
                //    SearchXeroContact(userView, out contact);
                //    if ((contact.ToList().Count == 0 && isAllowInsert == true) || contact.ToList().Count > 0)
                //    {
                //        xeroContact = CheckContactInXero(userView, api, contact.ToList());
                //        UserXeroContact userXeroContact = new UserXeroContact();
                //        userXeroContact.UserView = userView;
                //        userXeroContact.XeroContact = xeroContact;
                //        string serializeContact = Newtonsoft.Json.JsonConvert.SerializeObject(userXeroContact);

                //        if (userView.IsWholeSaler == true && contact.Count() > 0)
                //        {
                //            //List<Contact> contacts = contact.ToList();
                //            bool isMatch = CheckAllContactWithXero(userView, contact.FirstOrDefault());
                //            if (isMatch)
                //            {
                //                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
                //            }
                //        }

                //        if (userView.IsWholeSaler)
                //            TempData[userView.UserId.ToString()] = userView;

                //        return Json(new { status = true, data = serializeContact }, JsonRequestBehavior.AllowGet);
                //    }
                //    else
                //    {
                //        return Json(new { status = false, isAllowInsert = false }, JsonRequestBehavior.AllowGet);
                //    }
                //}
                //else
                //{
                //    return Json(new { status = false, error = "specified method is not supported." }, JsonRequestBehavior.AllowGet);
                //}
                #endregion xero oauth1.0
                //if (XeroApiHelper.xeroApiHelperSession != null)
                //{
                var dsUsers = _userBAL.GetUserById((!string.IsNullOrWhiteSpace(userId) ? Convert.ToInt32(userId) : 0));
                List<FormBot.Entity.User> user = DBClient.DataTableToList<FormBot.Entity.User>(dsUsers.Tables[0]);
                FormBot.Entity.User userView = user.FirstOrDefault();

                /* logic only for saas user to set invoicer values like uniquecompanynumber bc'z every user does not have uniquecompany number 
                 * as it is available only for solar company, so at that time we take that values form invoicer table  */

                if (userView.IsSAASUser)
                {
                    var dsInvoicerDetails = _userBAL.GetinvoicerDetailsForSaasUsers((!string.IsNullOrWhiteSpace(userId) ? Convert.ToInt32(userId) : 0));
                    if (dsInvoicerDetails != null && dsInvoicerDetails.Tables.Count > 0 && dsInvoicerDetails.Tables[0].Rows.Count > 0)
                    {
                        userView.CompanyName = dsInvoicerDetails.Tables[0].Rows[0]["InvoicerName"].ToString();
                        userView.UniqueCompanyNumber = dsInvoicerDetails.Tables[0].Rows[0]["UniqueCompanyNumber"].ToString();
                        userView.WholesalerCompanyName = dsInvoicerDetails.Tables[0].Rows[0]["InvoicerName"].ToString();
                        userView.UniqueWholesalerNumber = dsInvoicerDetails.Tables[0].Rows[0]["UniqueCompanyNumber"].ToString();
                    }
                }
                userView.AddressID = userView.IsPostalAddress ? 2 : 1;
                if (userView.WholesalerIsPostalAddress == 1)
                {
                    userView.WholesalerIsPostalAddress = 2;
                }
                else
                {
                    userView.WholesalerIsPostalAddress = 1;
                }

                FormBot.Entity.XeroContact xeroContact = new Entity.XeroContact();
                //var api = XeroApiHelper.xeroApiHelperSession.CoreApi();
                var expected = userView.IsWholeSaler ? userView.WholesalerCompanyABN : userView.CompanyABN;
                var contact = new List<Contact>();
                //SearchXeroContact(userView, out contact);
                if (!TokenUtilities.TokenExists())
                {
                    return Json(new { status = false, error = "invalid_grant" }, JsonRequestBehavior.AllowGet);
                }
                var token = TokenUtilities.GetStoredToken();
                string accessToken = token.AccessToken;
                string xeroTenantId = token.Tenants[0].TenantId.ToString();
                var cnt = Task.Run(async () => await SearchXeroContact(userView, accessToken, xeroTenantId));
                cnt.Wait();
                contact = cnt.Result;
                if ((contact.ToList().Count == 0 && isAllowInsert == true) || contact.ToList().Count > 0)
                {
                    var xeroCnt = Task.Run(async () => await CheckContactInXero(userView, contact.ToList(), accessToken, xeroTenantId));
                    xeroCnt.Wait();
                    xeroContact = xeroCnt.Result;
                    UserXeroContact userXeroContact = new UserXeroContact();
                    userXeroContact.UserView = userView;
                    userXeroContact.XeroContact = xeroContact;
                    string serializeContact = Newtonsoft.Json.JsonConvert.SerializeObject(userXeroContact);

                    if (userView.IsWholeSaler == true && contact.Count() > 0)
                    {
                        //List<Contact> contacts = contact.ToList();
                        bool isMatch = CheckAllContactWithXero(userView, contact.FirstOrDefault());
                        if (isMatch)
                        {
                            return Json(new { status = false }, JsonRequestBehavior.AllowGet);
                        }
                    }

                    if (userView.IsWholeSaler)
                        TempData[userView.UserId.ToString()] = userView;

                    return Json(new { status = true, data = serializeContact }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = false, isAllowInsert = false }, JsonRequestBehavior.AllowGet);
                }
                //}
                //else
                //{
                //    return Json(new { status = false, error = "specified method is not supported." }, JsonRequestBehavior.AllowGet);
                //}
            }
            catch (RenewTokenException e)
            {
                return Json(new { status = false, error = "RenewTokenException" }, JsonRequestBehavior.AllowGet);
            }
            catch (UnauthorizedException e)
            {
                return Json(new { status = false, error = "UnauthorizedException" }, JsonRequestBehavior.AllowGet);
            }
            catch (XeroApiException e)
            {
                int errorCount = ((Xero.Api.Infrastructure.Exceptions.ValidationException)(e)).ValidationErrors.Count;
                string errorMsg = string.Join(", ", ((Xero.Api.Infrastructure.Exceptions.ValidationException)(e)).ValidationErrors.AsEnumerable().Select(a => a.Message));

                return Json(new { status = false, error = errorMsg }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                if (e.InnerException.Message == "invalid_grant" || e.InnerException.InnerException.Message.ToString().ToLower() == "invalid_grant")
                {
                    return Json(new { status = false, error = "invalid_grant" }, JsonRequestBehavior.AllowGet);
                }

                else
                {
                    return Json(new { status = false, error = e.Message }, JsonRequestBehavior.AllowGet);
                }

            }
        }

        /// <summary>
        /// Check contact in xero
        /// </summary>
        /// <param name="xeroContact">xeroContact</param>
        /// <param name="api">api</param>
        /// <param name="contact">contact</param>
        /// <returns>Entity</returns>
        private async Task<FormBot.Entity.XeroContact> CheckContactInXero(FormBot.Entity.User xeroContact, List<Contact> contact, string accessToken, string xeroTenantId)
        {
            FormBot.Entity.XeroContact xeroContactDetail = new Entity.XeroContact();
            //var api = XeroApiHelper.xeroApiHelperSession.CoreApi();
            //var contact = api.Contacts.Find().Where(a => a.TaxNumber == xeroContact.CompanyABN).ToList();
            if (contact.Count == 0)
            {
                var createContact = (List<Contact>)null;
                //string accountNum = xeroContact.BSB.Length >= 6 ? xeroContact.BSB.ToString().Substring(0, 6) + xeroContact.AccountNumber : xeroContact.BSB + xeroContact.AccountNumber;
                string accountNum = xeroContact.IsWholeSaler ? (xeroContact.WholesalerBSB.Length >= 6 ? xeroContact.WholesalerBSB.ToString().Substring(0, 6) + xeroContact.WholesalerAccountNumber : xeroContact.WholesalerBSB + xeroContact.WholesalerAccountNumber) : (xeroContact.BSB.Length >= 6 ? xeroContact.BSB.ToString().Substring(0, 6) + xeroContact.AccountNumber : xeroContact.BSB + xeroContact.AccountNumber);

                #region Address

                string addressLine1 = string.Empty, addressLine2 = string.Empty, streetAddress = string.Empty;
                xeroContact.PostalDeliveryType = xeroContact.IsWholeSaler ? (string.IsNullOrWhiteSpace(xeroContact.WholesalerPostalDeliveryType) ? "" : xeroContact.WholesalerPostalDeliveryType) : (string.IsNullOrWhiteSpace(xeroContact.PostalDeliveryType) ? "" : xeroContact.PostalDeliveryType);
                xeroContact.UnitTypeName = xeroContact.IsWholeSaler ? (string.IsNullOrWhiteSpace(xeroContact.WholesalerUnitTypeName) ? "" : xeroContact.WholesalerUnitTypeName) : (string.IsNullOrWhiteSpace(xeroContact.UnitTypeName) ? "" : xeroContact.UnitTypeName);
                xeroContact.StreetNumber = xeroContact.IsWholeSaler ? (string.IsNullOrWhiteSpace(xeroContact.WholesalerStreetNumber) ? "" : xeroContact.WholesalerStreetNumber) : (string.IsNullOrWhiteSpace(xeroContact.StreetNumber) ? "" : xeroContact.StreetNumber);
                streetAddress = xeroContact.IsWholeSaler ? (xeroContact.WholesalerStreetNumber + (string.IsNullOrWhiteSpace(xeroContact.WholesalerStreetName) ? "" : " " + xeroContact.WholesalerStreetName) + (string.IsNullOrWhiteSpace(xeroContact.WholesalerStreetTypeName) ? "" : " " + xeroContact.WholesalerStreetTypeName)) : (xeroContact.StreetNumber + (string.IsNullOrWhiteSpace(xeroContact.StreetName) ? "" : " " + xeroContact.StreetName) + (string.IsNullOrWhiteSpace(xeroContact.StreetTypeName) ? "" : " " + xeroContact.StreetTypeName));
                List<Address> listAddress = null;
                if (xeroContact.IsWholeSaler)
                {
                    if (xeroContact.WholesalerIsPostalAddress == 2)
                    {
                        addressLine1 = xeroContact.WholesalerPostalDeliveryType + (string.IsNullOrWhiteSpace(xeroContact.WholesalerPostalDeliveryNumber) ? "" : " " + xeroContact.WholesalerPostalDeliveryNumber);
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(xeroContact.WholesalerUnitTypeName) && string.IsNullOrWhiteSpace(xeroContact.WholesalerUnitNumber))
                            addressLine1 = streetAddress;
                        else
                            addressLine1 = xeroContact.WholesalerUnitTypeName + (string.IsNullOrWhiteSpace(xeroContact.WholesalerUnitNumber) ? "" : " " + xeroContact.WholesalerUnitNumber);

                        if (!(string.IsNullOrWhiteSpace(xeroContact.WholesalerUnitTypeName)) && !(string.IsNullOrWhiteSpace(xeroContact.WholesalerUnitNumber)))
                            addressLine2 = streetAddress;
                    }
                    listAddress = addAddressToXeroContact(addressLine1, addressLine2, xeroContact.WholesalerPostCode, xeroContact.WholesalerTown, xeroContact.WholesalerState, xeroContact.WholesalerIsPostalAddress == 2 ? true : false);

                }
                else
                {
                    if (xeroContact.IsPostalAddress)
                    {
                        addressLine1 = xeroContact.PostalDeliveryType + (string.IsNullOrWhiteSpace(xeroContact.PostalDeliveryNumber) ? "" : " " + xeroContact.PostalDeliveryNumber);
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(xeroContact.UnitTypeName) && string.IsNullOrWhiteSpace(xeroContact.UnitNumber))
                            addressLine1 = streetAddress;
                        else
                            addressLine1 = xeroContact.UnitTypeName + (string.IsNullOrWhiteSpace(xeroContact.UnitNumber) ? "" : " " + xeroContact.UnitNumber);

                        if (!(string.IsNullOrWhiteSpace(xeroContact.UnitTypeName)) && !(string.IsNullOrWhiteSpace(xeroContact.UnitNumber)))
                            addressLine2 = streetAddress;
                    }
                    listAddress = addAddressToXeroContact(addressLine1, addressLine2, xeroContact.PostCode, xeroContact.Town, xeroContact.State, xeroContact.IsPostalAddress);
                }
                #endregion

                #region phone

                List<Phone> lstPhone = xeroContact.IsWholeSaler ? addPhoneToXeroContcat(xeroContact.WholesalerMobile, xeroContact.WholesalerPhone) : addPhoneToXeroContcat(xeroContact.Mobile, xeroContact.Phone);

                #endregion phone

                #region trader and option

                List<SalesTrackingCategory> PurchasesTrackingCategories = new List<SalesTrackingCategory>();
                if (!string.IsNullOrWhiteSpace(xeroContact.RAMName))
                {
                    Task<List<SalesTrackingCategory>> trackingCategories = Task.Run(async () => await AddGetTrackingCategory(xeroContact.RAMName, accessToken, xeroTenantId));
                    trackingCategories.Wait();
                    PurchasesTrackingCategories = trackingCategories.Result;
                }

                #endregion
                #region create contact in xero outh1.0
                //createContact = api.Contacts.Create(new[]
                //    {
                //        new Contact
                //        {
                //            AccountNumber = xeroContact.IsWholeSaler?xeroContact.UniqueWholesalerNumber: xeroContact.UniqueCompanyNumber
                //           ,Addresses = listAddress
                //           ,ContactStatus = xeroContact.IsActive ? ContactStatus.Active : ContactStatus.Archived
                //           ,EmailAddress = xeroContact.IsWholeSaler?xeroContact.WholesalerEmail: xeroContact.Email
                //           ,FirstName = xeroContact.IsWholeSaler?xeroContact.WholesalerFirstName:xeroContact.FirstName
                //           ,LastName = xeroContact.IsWholeSaler?xeroContact.WholesalerLastName:xeroContact.LastName
                //           ,Name = xeroContact.IsWholeSaler?xeroContact.WholesalerCompanyName:(!string.IsNullOrEmpty(xeroContact.CustomCompanyName) ? xeroContact.CustomCompanyName : xeroContact.CompanyName)
                //           ,Phones = lstPhone
                //           ,TaxNumber = xeroContact.IsWholeSaler?xeroContact.WholesalerCompanyABN:xeroContact.CompanyABN
                //           ,BankAccountDetails = accountNum
                //           ,PurchasesTrackingCategories = PurchasesTrackingCategories
                //        }
                //    }).ToList();
                #endregion
                var contactObj = new Contact
                {
                    AccountNumber = xeroContact.IsWholeSaler ? xeroContact.UniqueWholesalerNumber : xeroContact.UniqueCompanyNumber
                            ,
                    Addresses = listAddress
                            ,
                    ContactStatus = xeroContact.IsActive ? Contact.ContactStatusEnum.ACTIVE : Contact.ContactStatusEnum.ARCHIVED
                            ,
                    EmailAddress = xeroContact.IsWholeSaler ? xeroContact.WholesalerEmail : xeroContact.Email
                            ,
                    FirstName = xeroContact.IsWholeSaler ? xeroContact.WholesalerFirstName : xeroContact.FirstName
                            ,
                    LastName = xeroContact.IsWholeSaler ? xeroContact.WholesalerLastName : xeroContact.LastName
                            ,
                    Name = xeroContact.IsWholeSaler ? xeroContact.WholesalerCompanyName : (!string.IsNullOrEmpty(xeroContact.CustomCompanyName) ? xeroContact.CustomCompanyName : xeroContact.CompanyName)
                            ,
                    Phones = lstPhone
                            ,
                    TaxNumber = xeroContact.IsWholeSaler ? xeroContact.WholesalerCompanyABN : xeroContact.CompanyABN
                            ,
                    BankAccountDetails = accountNum
                            ,
                    PurchasesTrackingCategories = PurchasesTrackingCategories
                };
                var contactsObj = new Contacts();
                contactsObj._Contacts = new List<Contact>() { contactObj };
                //  createContact = contactsObj._Contacts;
                var AccountingApi = new AccountingApi();
                var response = Task.Run(async () => await AccountingApi.CreateContactsAsync(accessToken, xeroTenantId, contactsObj));
                response.Wait();
                createContact = response.Result._Contacts;
                xeroContactDetail = GetXeroContact(createContact[0], xeroContact.UserId);
                return xeroContactDetail;
            }
            else
            {
                xeroContactDetail = GetXeroContact(contact[0], xeroContact.UserId);
                return xeroContactDetail;
            }
        }

        /// <summary>
        /// open popup with xero contact and system contact detail.
        /// </summary>
        /// <param name="Data">Data</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult _XeroContact(FormBot.Entity.UserXeroContact Data)
        {
            ViewBag.PostalAddressType = new SelectList(EnumExtensions.GetPostalAddressEnumList(), "ID", "Name");
            return PartialView("_XeroContact", Data);
        }

        [HttpGet]
        public JsonResult CheckClientCodePrefix(int userTypeId, int resellerId, int userId, string clientCodePrefix)
        {
            int totalCount = 0;
            if (userTypeId > 0)
            {
                DataSet dsPrefixCount = _userBAL.CheckClientCodePrefix(userTypeId, resellerId, userId, clientCodePrefix);
                if (dsPrefixCount != null && dsPrefixCount.Tables.Count > 0 && dsPrefixCount.Tables[0] != null && dsPrefixCount.Tables[0].Rows.Count > 0)
                {
                    totalCount = Convert.ToInt32(dsPrefixCount.Tables[0].Rows[0]["TotalCount"]);
                }
            }
            return Json(new { count = totalCount }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateContactInXero(Entity.XeroContact xeroContact)
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
                if (xeroContact != null)
                {
                    //var api = XeroApiHelper.xeroApiHelperSession.CoreApi();

                    #region address

                    List<Address> listAddress = addAddressToXeroContact(xeroContact.AddressLine1, xeroContact.AddressLine2, xeroContact.PostCode, xeroContact.Town, xeroContact.State, xeroContact.IsPostalAddress);

                    #endregion

                    #region trader and option

                    List<SalesTrackingCategory> PurchasesTrackingCategories = new List<SalesTrackingCategory>();

                    if (!string.IsNullOrWhiteSpace(xeroContact.purchasesTrackingCategoryOption))
                    {
                        Task<List<SalesTrackingCategory>> trackingCategories = Task.Run(async () => await AddGetTrackingCategory(xeroContact.purchasesTrackingCategoryOption, accessToken, xeroTenantId));
                        trackingCategories.Wait();
                        PurchasesTrackingCategories = trackingCategories.Result;
                    }


                    #endregion

                    #region phone

                    List<Phone> lstPhone = addPhoneToXeroContcat(xeroContact.Mobile, xeroContact.Phone);

                    #endregion phone

                    string accountNumber = xeroContact.BSB + xeroContact.BankAccountDetails;
                    #region update contact xero outh1.0
                    //var createContact = api.Contacts.Update(new[]
                    //    {
                    //        new Contact
                    //        {
                    //            ContactID = new Guid(xeroContact.XeroContactId.ToString())
                    //           ,AccountNumber = xeroContact.ClientNumber
                    //           ,Addresses = listAddress
                    //           ,ContactStatus = xeroContact.IsActive ? Contact.ContactStatusEnum.ACTIVE : Contact.ContactStatusEnum.ARCHIVED
                    //           ,EmailAddress = xeroContact.Email
                    //           ,FirstName = xeroContact.FirstName
                    //           ,LastName = xeroContact.LastName
                    //           ,Name = xeroContact.CompanyName
                    //           ,Phones = lstPhone
                    //           ,TaxNumber = xeroContact.CompanyABN
                    //           ,BankAccountDetails = accountNumber
                    //           ,PurchasesTrackingCategories = PurchasesTrackingCategories
                    //        }
                    //    }).ToList();
                    #endregion
                    var contactObj = new Contact
                    {
                        ContactID = new Guid(xeroContact.XeroContactId.ToString())
                                   ,
                        AccountNumber = xeroContact.ClientNumber
                                   ,
                        Addresses = listAddress
                                   ,
                        ContactStatus = xeroContact.IsActive ? Contact.ContactStatusEnum.ACTIVE : Contact.ContactStatusEnum.ARCHIVED
                                   ,
                        EmailAddress = xeroContact.Email
                                   ,
                        FirstName = xeroContact.FirstName
                                   ,
                        LastName = xeroContact.LastName
                                   ,
                        Name = xeroContact.CompanyName
                                   ,
                        Phones = lstPhone
                                   ,
                        TaxNumber = xeroContact.CompanyABN
                                   ,
                        BankAccountDetails = accountNumber
                                   ,
                        PurchasesTrackingCategories = PurchasesTrackingCategories
                    };
                    var contactsObj = new Contacts();
                    contactsObj._Contacts = new List<Contact>() { contactObj };
                    var createContact = contactsObj._Contacts;
                    var AccountingApi = new AccountingApi();

                    #region for log which details are changed
                    var contactDetails = Task.Run(async () => await AccountingApi.GetContactAsync(accessToken, xeroTenantId, new Guid(xeroContact.XeroContactId.ToString())));
                    contactDetails.Wait();
                    var oldContactDetails = contactDetails.Result._Contacts;
                    var updatedContactDetails = createContact;
                    var OldPhoneDetails = GetMobilePhone(oldContactDetails[0].Phones, false);
                    var UpdatedPhoneDetails = GetMobilePhone(updatedContactDetails[0].Phones, false);
                    var OldMobileDetails = GetMobilePhone(oldContactDetails[0].Phones, true);
                    var UpdatedMobileDetails = GetMobilePhone(updatedContactDetails[0].Phones, true);
                    List<string> historyMessage = new List<string>();
                    for (int i = 0; i < oldContactDetails.ToList().Count; i++)
                    {
                        if (oldContactDetails[i].FirstName != updatedContactDetails[i].FirstName)
                        {
                            historyMessage.Add("Invoicing Contact First Name has been updated in xero from [" + oldContactDetails[i].FirstName + "] to [" + updatedContactDetails[i].FirstName + "] by " + ProjectSession.LoggedInName + " on " + DateTime.Now);
                        }
                        if (oldContactDetails[i].LastName != updatedContactDetails[i].LastName)
                        {
                            historyMessage.Add("Invoicing Contact Last Name has been updated in xero from [" + oldContactDetails[i].LastName + "] to [" + updatedContactDetails[i].LastName + "] by " + ProjectSession.LoggedInName + " on " + DateTime.Now);
                        }
                        if (OldPhoneDetails != UpdatedPhoneDetails)
                        {
                            historyMessage.Add("Invoicing Contact Phone has been updated in xero from [" + OldPhoneDetails + "] to [" + UpdatedPhoneDetails + "] by " + ProjectSession.LoggedInName + " on " + DateTime.Now);
                        }
                        if (OldMobileDetails != UpdatedMobileDetails)
                        {
                            historyMessage.Add("Invoicing Contact Mobile has been updated in xero from [" + OldMobileDetails + "] to [" + UpdatedMobileDetails + "] by " + ProjectSession.LoggedInName + " on " + DateTime.Now);
                        }
                        if (oldContactDetails[i].TaxNumber != updatedContactDetails[i].TaxNumber)
                        {
                            historyMessage.Add("Invoicing Contact ABN has been updated in xero from [" + oldContactDetails[i].TaxNumber + "] to [" + updatedContactDetails[i].TaxNumber + "] by " + ProjectSession.LoggedInName + " on " + DateTime.Now);
                        }
                        if (oldContactDetails[i].Name != updatedContactDetails[i].Name)
                        {
                            historyMessage.Add("Business Name has been updated in xero from [" + oldContactDetails[i].Name + "] to [" + updatedContactDetails[i].Name + "] by " + ProjectSession.LoggedInName + " on " + DateTime.Now);
                        }
                        if (oldContactDetails[i].BankAccountDetails != updatedContactDetails[i].BankAccountDetails)
                        {
                            historyMessage.Add("Banking Details have been updated from[" + oldContactDetails[i].BankAccountDetails.Substring(0, 6) + " - " + oldContactDetails[i].BankAccountDetails.Substring(6) + "] to [" + updatedContactDetails[i].BankAccountDetails.Substring(0, 6) + " - " + updatedContactDetails[i].BankAccountDetails.Substring(6) + "] by " + ProjectSession.LoggedInName + " on " + DateTime.Now);
                        }

                    }
                    _userBAL.InsertLogForUpdateContact(string.Join(",", historyMessage), xeroContact.UserId);
                    #endregion
                    var response = await AccountingApi.UpdateContactAsync(accessToken, xeroTenantId, new Guid(xeroContact.XeroContactId.ToString()), contactsObj);
                    if (!string.IsNullOrEmpty(createContact[0].ContactID.ToString()))
                        return Json(new { status = true }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { status = false }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
            catch (RenewTokenException)
            {
                return Json(new { status = false, error = "RenewTokenException" }, JsonRequestBehavior.AllowGet);
            }
            catch (XeroApiException e)
            {
                int errorCount = ((Xero.Api.Infrastructure.Exceptions.ValidationException)(e)).ValidationErrors.Count;
                string errorMsg = string.Join(", ", ((Xero.Api.Infrastructure.Exceptions.ValidationException)(e)).ValidationErrors.AsEnumerable().Select(a => a.Message));

                return Json(new { status = false, error = errorMsg }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { status = false, error = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private async Task<List<SalesTrackingCategory>> AddGetTrackingCategory(string RAMName, string accessToken, string xeroTenantId)
        {

            var api = new AccountingApi();
            var findTrd = await api.GetTrackingCategoriesAsync(accessToken, xeroTenantId);
            var findTrader = findTrd._TrackingCategories.ToList();
            // var findTrader = api.TrackingCategories.Find().ToList();
            bool isExist = false;
            string traderName = string.Empty;
            if (findTrader.Count > 0)
            {
                for (int i = 0; i < findTrader.Count; i++)
                {
                    if (!isExist)
                    {
                        for (int opt = 0; opt < findTrader[i].Options.Count; opt++)
                        {
                            if (findTrader[i].Options[opt].Name.ToLower().Trim() == RAMName.ToLower().Trim())
                            {
                                isExist = true;
                                traderName = findTrader[i].Name;
                                break;
                            }
                        }
                    }
                    else
                        break;
                }
                if (!isExist)
                {

                    TrackingOption trackingOption = new TrackingOption();
                    trackingOption.Name = RAMName;
                    trackingOption.Status = TrackingOption.StatusEnum.ACTIVE;
                    traderName = findTrader[0].Name;
                    Guid guid = new Guid(findTrader[0].TrackingCategoryID.ToString());
                    await api.CreateTrackingOptionsAsync(accessToken, xeroTenantId, guid, trackingOption);
                    //findTrd._TrackingCategories[findTrader[0].TrackingCategoryID].Add(trackingOption);
                    //api.trackingca[findTrader[0].TrackingCategoryID].Add(trackingOption);
                }
            }
            else
            {
                var trader = (List<TrackingCategory>)null;
                #region oauth1.0
                //trader = api.TrackingCategories.Create(new[] {
                //    new TrackingCategory
                //      {
                //         Name = "Trader"
                //        ,Status = TrackingCategory.StatusEnum.ACTIVE
                //      }
                //    }).ToList();
                #endregion

                var trackingCategory = new TrackingCategory
                {
                    Name = "Trader",
                    Status = TrackingCategory.StatusEnum.ACTIVE
                };
                trader = new List<TrackingCategory>() { trackingCategory };

                var AccountingApi = new AccountingApi();
                var response = await AccountingApi.CreateTrackingCategoryAsync(accessToken, xeroTenantId, trackingCategory);
                TrackingOption trackingOption = new TrackingOption();
                trackingOption.Name = RAMName;
                trackingOption.Status = TrackingOption.StatusEnum.ACTIVE;
                traderName = trader[0].Name;
                Guid guid = new Guid(trader[0].TrackingCategoryID.ToString());
                await api.CreateTrackingOptionsAsync(accessToken, xeroTenantId, guid, trackingOption);
                // api.TrackingCategories[trader[0].Id].Add(trackingOption);
            }

            List<SalesTrackingCategory> PurchasesTrackingCategories = new List<SalesTrackingCategory>();
            SalesTrackingCategory purchasesTrackingCategory = new SalesTrackingCategory();
            purchasesTrackingCategory.TrackingCategoryName = traderName;
            purchasesTrackingCategory.TrackingOptionName = RAMName;
            PurchasesTrackingCategories.Add(purchasesTrackingCategory);

            return PurchasesTrackingCategories;
        }

        private List<Phone> addPhoneToXeroContcat(string xeroMobile, string xeroPhone)
        {
            List<Phone> lstPhone = new List<Phone>();
            if (!string.IsNullOrWhiteSpace(xeroMobile))
            {
                Phone mobile = new Phone();
                mobile.PhoneNumber = xeroMobile;
                mobile.PhoneType = Phone.PhoneTypeEnum.MOBILE;
                lstPhone.Add(mobile);
            }
            if (!string.IsNullOrWhiteSpace(xeroPhone))
            {
                Phone phone = new Phone();
                phone.PhoneNumber = xeroPhone;
                phone.PhoneType = Phone.PhoneTypeEnum.DEFAULT;
                lstPhone.Add(phone);
            }
            return lstPhone;
        }

        private string GetMobilePhone(List<Phone> phoneMobile, bool isMobile)
        {
            string number = string.Empty;
            if (phoneMobile.Count > 0)
            {
                for (int i = 0; i < phoneMobile.Count; i++)
                {
                    if (!isMobile && phoneMobile[i].PhoneType == Phone.PhoneTypeEnum.DEFAULT)
                        number = phoneMobile[i].PhoneNumber;
                    if (isMobile && phoneMobile[i].PhoneType == Phone.PhoneTypeEnum.MOBILE)
                        number = phoneMobile[i].PhoneNumber;
                }
            }
            return number;
        }

        private Address GetAddress(List<Address> Addresses)
        {
            Address address = new Address();

            if (!string.IsNullOrWhiteSpace(Addresses[0].AddressLine1))
                address = Addresses[0];
            else
                address = Addresses[1];

            return address;
        }

        private List<Address> addAddressToXeroContact(string addressLine1, string addressLine2, string postCode, string town, string state, bool IsPostalAddress)
        {
            List<Address> listAddress = new List<Address>();
            Address address = new Address();
            address.AddressLine1 = addressLine1;
            address.AddressLine2 = addressLine2;
            address.PostalCode = postCode;
            address.City = town;
            address.Region = state;
            if (IsPostalAddress)
                address.AddressType = Address.AddressTypeEnum.POBOX;
            else
                address.AddressType = Address.AddressTypeEnum.STREET;
            listAddress.Add(address);
            return listAddress;
        }

        private FormBot.Entity.XeroContact GetXeroContact(Contact contact, int userId)
        {
            FormBot.Entity.XeroContact xeroContactDetail = new Entity.XeroContact();

            xeroContactDetail.XeroContactId = contact.ContactID.ToString();
            xeroContactDetail.UserId = userId;
            xeroContactDetail.Email = contact.EmailAddress;
            xeroContactDetail.FirstName = contact.FirstName;
            xeroContactDetail.LastName = contact.LastName;
            xeroContactDetail.BSB = !string.IsNullOrWhiteSpace(contact.BankAccountDetails) ? contact.BankAccountDetails.Length > 6 ? contact.BankAccountDetails.Substring(0, 6) : contact.BankAccountDetails : string.Empty;
            xeroContactDetail.CompanyABN = contact.TaxNumber;
            xeroContactDetail.CompanyName = contact.Name;
            //xeroContactDetail.BankAccountDetails = !string.IsNullOrEmpty(contact.BankAccountDetails) ? contact.BankAccountDetails.Length > 6 ? contact.BankAccountDetails.Substring(0, 6) : contact.BankAccountDetails : string.Empty;

            xeroContactDetail.BankAccountDetails = (!string.IsNullOrWhiteSpace(contact.BankAccountDetails) && contact.BankAccountDetails.Length > 6) ? contact.BankAccountDetails.Substring(6) : string.Empty;

            xeroContactDetail.IsActive = (contact.ContactStatus == Contact.ContactStatusEnum.ACTIVE) ? true : false;
            xeroContactDetail.XeroContactId = contact.ContactID.ToString();
            //xeroContactDetail.ClientNumber = contact.AccountNumber;
            xeroContactDetail.UniqueCompanyNumber = contact.AccountNumber;

            #region phone/mobile

            //if (contact.Phones.Count > 0)
            //{
            //    for (int i = 0; i < contact.Phones.Count; i++)
            //    {
            //        if (contact.Phones[i].PhoneType == Xero.Api.Core.Model.Types.PhoneType.Default)
            //            xeroContactDetail.Phone = contact.Phones[i].PhoneNumber;
            //        if (contact.Phones[i].PhoneType == Xero.Api.Core.Model.Types.PhoneType.Mobile)
            //            xeroContactDetail.Mobile = contact.Phones[i].PhoneNumber;
            //    }
            //}
            xeroContactDetail.Phone = GetMobilePhone(contact.Phones, false);
            xeroContactDetail.Mobile = GetMobilePhone(contact.Phones, true);

            #endregion

            #region address

            if (contact.Addresses.Count > 0)
            {
                //Address address = new Address();

                //if (!string.IsNullOrEmpty(contact.Addresses[0].AddressLine1))
                //    address = contact.Addresses[0];
                //else
                //    address = contact.Addresses[1];

                Address address = GetAddress(contact.Addresses);
                xeroContactDetail.AddressLine1 = address.AddressLine1;
                xeroContactDetail.AddressLine2 = address.AddressLine2;
                xeroContactDetail.Town = address.City;
                xeroContactDetail.PostCode = address.PostalCode;
                xeroContactDetail.State = address.Region;
                xeroContactDetail.IsPostalAddress = (address.AddressType == Address.AddressTypeEnum.STREET) ? false : true;
            }

            #endregion

            #region purchase category
            List<SalesTrackingCategory> PurchasesTrackingCategories = contact.PurchasesTrackingCategories;
            if (PurchasesTrackingCategories != null && PurchasesTrackingCategories.Count > 0)
            {
                xeroContactDetail.purchasesTrackingCategoryName = PurchasesTrackingCategories[0].TrackingCategoryName;
                xeroContactDetail.purchasesTrackingCategoryOption = PurchasesTrackingCategories[0].TrackingOptionName;
            }

            #endregion

            return xeroContactDetail;
        }
        public async Task<List<Contact>> SearchXeroContact(FormBot.Entity.User userView, string accessToken, string xeroTenantId)
        {
            List<Contact> contactslst = new List<Contact>();
            try
            {

                var AccountingApi = new AccountingApi();
                var response = await AccountingApi.GetContactsAsync(accessToken, xeroTenantId);
                contactslst = response._Contacts.ToList();
                contactslst = contactslst.Where(x => x.AccountNumber == (userView.IsWholeSaler ? userView.UniqueWholesalerNumber : userView.UniqueCompanyNumber)).ToList();
                return contactslst;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            #region getcontact xero oauth1.0
            //var api = XeroApiHelper.xeroApiHelperSession.CoreApi();
            ////var lstContactWithABN = api.Contacts.Where(
            ////                    //Company ABN Number
            ////                    string.Format("TaxNumber == \"{0}\"", userView.CompanyABN)
            ////                    + " And "
            ////                    + //Company Name
            ////                    string.Format("Name == \"{0}\"", (!string.IsNullOrEmpty(userView.CustomCompanyName) ? userView.CustomCompanyName : userView.CompanyName))
            ////                ).Find();
            ////if (lstContactWithABN.Any())
            ////    contact = lstContactWithABN.ToList();
            ////else
            ////{
            //contact = api.Contacts.Where(
            //    //Company Account Number
            //    string.Format("AccountNumber == \"{0}\"", userView.IsWholeSaler ? userView.UniqueWholesalerNumber : userView.UniqueCompanyNumber)
            //).Find().ToList();
            ////}
            #endregion
        }

        [HttpGet]
        public JsonResult CheckAllContactInXero(string solarCompanyIds)
        {
            if (ProjectSession.LoggedInUserId == 0)
                return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);

            List<string> lstSolarCompanyIds = new List<string>();
            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
            List<KeyValuePair<string, string>> lstSolarCompanyName = new List<KeyValuePair<string, string>>();

            try
            {
                //if (XeroApiHelper.xeroApiHelperSession != null)
                //{

                List<string> lstSolarCompany = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(solarCompanyIds);
                DataSet dsUsers = _userBAL.GetAllSolarCompanyDetails(string.Join(",", lstSolarCompany));

                if (dsUsers != null && dsUsers.Tables.Count > 0)
                {
                    for (int i = 0; i < dsUsers.Tables.Count; i++)
                    {
                        List<FormBot.Entity.User> user = DBClient.DataTableToList<FormBot.Entity.User>(dsUsers.Tables[i]);
                        FormBot.Entity.User userView = user.FirstOrDefault();
                        // var contact = new List<Contact>();

                        // SearchXeroContact(userView);
                        if (!TokenUtilities.TokenExists())
                        {
                            return Json(new { status = false, error = "invalid_grant" }, JsonRequestBehavior.AllowGet);
                        }
                        var token = TokenUtilities.GetStoredToken();
                        string accessToken = token.AccessToken;
                        string xeroTenantId = token.Tenants[0].TenantId.ToString();
                        Task<List<Contact>> contact = Task.Run(async () => await SearchXeroContact(userView, accessToken, xeroTenantId));
                        contact.Wait();
                        if (contact.Result.Any())
                        {
                            bool isMatch = true;
                            isMatch = CheckAllContactWithXero(userView, contact.Result.FirstOrDefault());
                            if (!isMatch)
                            {
                                string id = QueryString.QueryStringEncode("id=" + Convert.ToString(userView.UserId));
                                list.Add(new KeyValuePair<string, string>(Convert.ToString(userView.SolarCompanyId), id));
                            }
                            else
                            {
                                lstSolarCompanyName.Add(new KeyValuePair<string, string>(Convert.ToString(userView.SolarCompanyId), userView.CompanyName));
                            }
                        }
                        else
                        {
                            string id = QueryString.QueryStringEncode("id=" + Convert.ToString(userView.UserId));
                            list.Add(new KeyValuePair<string, string>(Convert.ToString(userView.SolarCompanyId), id));
                        }
                    }
                }

                string notMatchSolarCompany = Newtonsoft.Json.JsonConvert.SerializeObject(list);
                return Json(new { status = true, data = notMatchSolarCompany, companyName = Newtonsoft.Json.JsonConvert.SerializeObject(lstSolarCompanyName) }, JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                //    return Json(new { status = false, error = "specified method is not supported." }, JsonRequestBehavior.AllowGet);
                //}
            }
            catch (RenewTokenException e)
            {
                return Json(new { status = false, error = "RenewTokenException" }, JsonRequestBehavior.AllowGet);
            }
            catch (UnauthorizedException e)
            {
                return Json(new { status = false, error = "UnauthorizedException" }, JsonRequestBehavior.AllowGet);
            }
            catch (XeroApiException e)
            {
                int errorCount = ((Xero.Api.Infrastructure.Exceptions.ValidationException)(e)).ValidationErrors.Count;
                string errorMsg = string.Join(", ", ((Xero.Api.Infrastructure.Exceptions.ValidationException)(e)).ValidationErrors.AsEnumerable().Select(a => a.Message));

                return Json(new { status = false, error = errorMsg }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                if (e.InnerException.Message == "invalid_grant" || e.InnerException.InnerException.Message.ToString().ToLower() == "invalid_grant")
                    return Json(new { status = false, error = "invalid_grant" }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { status = false, error = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private bool CheckAllContactWithXero(FormBot.Entity.User userView, Contact contact)
        {
            bool isFirstName, isLastName, isEmail, isPhone, isMobile, isTrader, isClientNumber, isCompanyABN, isCompanyName, isAccountNumber, isTown, isState, isPostCode, isBSB, isUniqueCompanyNumber;
            isFirstName = CheckAllContactWithXeroWithDetail(userView.FirstName, contact.FirstName);
            isLastName = CheckAllContactWithXeroWithDetail(userView.LastName, contact.LastName);
            isEmail = CheckAllContactWithXeroWithDetail(userView.Email, contact.EmailAddress);

            //if (userView.IsWholeSaler == true && string.IsNullOrEmpty(userView.ClientNumber) && string.IsNullOrEmpty(contact.AccountNumber))
            //    isClientNumber = true;
            //else
            //    isClientNumber = CheckAllContactWithXeroWithDetail(userView.ClientNumber, contact.AccountNumber);

            if (userView.IsWholeSaler == true && string.IsNullOrWhiteSpace(userView.UniqueCompanyNumber) && string.IsNullOrWhiteSpace(contact.AccountNumber))
                isUniqueCompanyNumber = true;
            else
                isUniqueCompanyNumber = CheckAllContactWithXeroWithDetail(userView.UniqueCompanyNumber, contact.AccountNumber);

            isCompanyABN = CheckAllContactWithXeroWithDetail(userView.CompanyABN, contact.TaxNumber);
            isCompanyName = CheckAllContactWithXeroWithDetail(userView.CompanyName, contact.Name);
            if (!isCompanyName && !string.IsNullOrWhiteSpace(userView.CustomCompanyName))
            {
                isCompanyName = CheckAllContactWithXeroWithDetail(userView.CustomCompanyName, contact.Name);
            }
            string phone = GetMobilePhone(contact.Phones, false);
            isPhone = CheckAllContactWithXeroWithDetail(userView.Phone, phone);

            string mobile = GetMobilePhone(contact.Phones, true);
            isMobile = CheckAllContactWithXeroWithDetail(userView.Mobile, mobile);

            //string trader = string.Empty;
            //List<SalesTrackingCategory> PurchasesTrackingCategories = contact.PurchasesTrackingCategories;
            //if (PurchasesTrackingCategories != null && PurchasesTrackingCategories.Count > 0)
            //    trader = PurchasesTrackingCategories[0].TrackingOptionName;

            //isTrader = CheckAllContactWithXeroWithDetail(userView.RAMName, trader);
            isTrader = true;

            isAccountNumber = CheckAllContactWithXeroWithDetail(userView.AccountNumber, !string.IsNullOrWhiteSpace(contact.BankAccountDetails) && contact.BankAccountDetails.Length > 6 ? contact.BankAccountDetails.Substring(6) : string.Empty);
            isBSB = CheckAllContactWithXeroWithDetail(userView.BSB, !string.IsNullOrWhiteSpace(contact.BankAccountDetails) ? contact.BankAccountDetails.Length > 6 ? contact.BankAccountDetails.Substring(0, 6) : contact.BankAccountDetails : string.Empty);

            Address address = GetAddress(contact.Addresses);
            isTown = CheckAllContactWithXeroWithDetail(userView.Town, address.City);
            isState = CheckAllContactWithXeroWithDetail(userView.State, address.Region);
            isPostCode = CheckAllContactWithXeroWithDetail(userView.PostCode, address.PostalCode);

            if (isFirstName == true && isLastName == true && isEmail == true && isPhone == true && isMobile == true && isTrader == true && isUniqueCompanyNumber == true && isCompanyABN == true && isCompanyName == true && isAccountNumber == true && isTown == true && isState == true && isPostCode == true && isBSB == true)
                return true;
            else
                return false;
        }

        private bool CheckAllContactWithXeroWithDetail(string sysDetail, string xeroDetail)
        {
            bool isMatch = false;
            //if (!string.IsNullOrEmpty(sysDetail))
            //{
            //    if (!string.IsNullOrEmpty(xeroDetail))
            //    {
            //        if (sysDetail.Trim().ToLower() == xeroDetail.Trim().ToLower())
            //            isMatch = true;
            //    }
            //}
            if (sysDetail != null)
            {
                if (xeroDetail != null)
                {
                    if (sysDetail.Trim().ToLower() == xeroDetail.Trim().ToLower())
                        isMatch = true;
                }
            }
            return isMatch;
        }

        private void EmailNotificationForSCAUser(FormBot.Entity.User uv, FormBot.Entity.User userView, int solarCompanyId)
        {
            EmailInfo emailInfo = new EmailInfo();
            emailInfo.TemplateID = 12;
            emailInfo.FirstName = userView.FirstName;
            emailInfo.LastName = userView.LastName;
            StringBuilder sb = new StringBuilder();

            if (uv.UserName != userView.UserName)
            {
                sb.Append(uv.UserName != userView.UserName ? "UserName: " + userView.UserName : string.Empty);
            }

            //if (uv.PasswordHash != null && userView.Password != null)
            //{
            //    PasswordHasher hasher = new PasswordHasher();
            //    Microsoft.AspNet.Identity.PasswordVerificationResult result = hasher.VerifyHashedPassword(uv.PasswordHash, userView.Password);
            //    if (result.ToString() != "Success")
            //    {
            //        sb.Append(uv.Password != userView.Password ? "Password: " + userView.Password : string.Empty);
            //    }
            //}

            if (uv.CompanyABN != userView.CompanyABN || uv.CompanyName != userView.CompanyName || uv.BSB != userView.BSB || uv.AccountNumber != userView.AccountNumber || uv.AccountName != userView.AccountName)
            {
                sb.Append("<table border=\"1\">");
                sb.Append("<thead><tr><td>Account Details</td><td>Old Value</td><td>New Value</td></tr></thead>");
                sb.Append("<tbody>");
                sb.Append(uv.CompanyABN != userView.CompanyABN ? "<tr><td>CompanyABN</td><td>" + uv.CompanyABN + "</td><td>" + userView.CompanyABN + "</td></tr>" : string.Empty);
                sb.Append(uv.CompanyName != userView.CompanyName ? "<tr><td>Company Name</td><td>" + uv.CompanyName + "</td><td>" + userView.CompanyName + "</td></tr>" : string.Empty);
                sb.Append(uv.BSB != userView.BSB ? "<tr><td>BSB</td><td>" + uv.BSB + "</td><td>" + userView.BSB + "</td></tr>" : string.Empty);
                sb.Append(uv.AccountNumber != userView.AccountNumber ? "<tr><td>Account Number</td><td>" + uv.AccountNumber + "</td><td>" + userView.AccountNumber + "</td></tr>" : string.Empty);
                sb.Append(uv.AccountName != userView.AccountName ? "<tr><td>Account Name</td><td>" + uv.AccountName + "</td><td>" + userView.AccountName + "</td></tr>" : string.Empty);
                sb.Append("</tbody>");
                sb.Append("</table>");
            }

            if (!string.IsNullOrWhiteSpace(sb.ToString()))
            {
                emailInfo.Details = sb.ToString();
                string mailAddresses = userView.Email + ";" + _userBAL.GetRARAMEmailAddress(solarCompanyId);
                //Below Changes by BANSI KANERIYA - For Client Requirment- @notication mail id(live id) in from address.
                //FormBot.Email.Account acct = System.Web.HttpContext.Current.Session[FormBot.Email.Constants.sessionAccount] as FormBot.Email.Account;
                //if (acct != null)
                //{
                //    _emailBAL.ComposeAndSendEmail(emailInfo, mailAddresses);
                //}
                //else
                //{
                EmailTemplate emailTempalte = _emailBAL.GetEmailTemplateByID(emailInfo.TemplateID);
                if (emailTempalte != null && !string.IsNullOrWhiteSpace(emailTempalte.Content))
                {
                    string FailReason = string.Empty;
                    //SMTPDetails smtpDetail = new SMTPDetails();
                    //smtpDetail.MailFrom = ProjectSession.MailFrom;
                    //smtpDetail.SMTPUserName = ProjectSession.SMTPUserName;
                    //smtpDetail.SMTPPassword = ProjectSession.SMTPPassword;
                    //smtpDetail.SMTPHost = ProjectSession.SMTPHost;
                    //smtpDetail.SMTPPort = Convert.ToInt32(ProjectSession.SMTPPort);
                    //smtpDetail.IsSMTPEnableSsl = ProjectSession.IsSMTPEnableSsl;
                    string body = _emailBAL.GetEmailBody(emailInfo, emailTempalte);
                    ComposeEmail composeEmail = new Email.ComposeEmail();
                    composeEmail.Subject = emailTempalte.Subject;
                    composeEmail.Body = new innerBody();
                    composeEmail.Body.body = body;

                    bool status = false;
                    try
                    {
                        if (body != null && !string.IsNullOrWhiteSpace(body))
                        {
                            QueuedEmail objQueuedEmail = new QueuedEmail();
                            objQueuedEmail.FromEmail = ProjectSession.MailFrom;
                            objQueuedEmail.Body = body;
                            objQueuedEmail.Subject = emailTempalte.Subject;
                            objQueuedEmail.ToEmail = mailAddresses;
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

                    //bool status1 = MailHelper.SendMail(smtpDetail, mailAddresses, null, null, emailTempalte.Subject, composeEmail.Body.body, null, true, ref FailReason, false);
                }
            }
        }

        [HttpGet]
        public ActionResult RestoreUser()
        {
            FormBot.Entity.User user = new FormBot.Entity.User();
            user.UserTypeID = ProjectSession.UserTypeId;
            return View(user);
        }

        [HttpPost]
        public JsonResult RestoreUser(string CheckedId)
        {
            string[] CheckedUserId = CheckedId.Split(',').Select(sValue => sValue.Trim()).ToArray();
            string userID = "";
            for (int i = 0; i < CheckedUserId.Length; i++)
            {
                userID += QueryString.GetValueFromQueryString(CheckedUserId[i], "id");
            }
            int result = _userBAL.RestoreUser(userID);
            return Json(new { status = result }, JsonRequestBehavior.AllowGet); ;
        }

        public void GetAllUserForRestore(string name = "", string username = "", string email = "", string resellerId = "", string cecNumber = "", string liecenseNumbe = "", string requestedusertypeid = "", string companyabn = "", string companyName = "", string solarcompanyId = "")
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            IList<FormBot.Entity.User> lstUser;
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);
            if (requestedusertypeid == "")
            {
                lstUser = _userBAL.GetAllUserForRestoreList(1, gridParam.PageSize, pageNumber, gridParam.SortCol, gridParam.SortDir, name, username, email, cecNumber, liecenseNumbe, companyabn, companyName, resellerId, solarcompanyId);
            }
            else
            {
                lstUser = _userBAL.GetAllUserForRestoreList(Convert.ToInt32(requestedusertypeid), gridParam.PageSize, pageNumber, gridParam.SortCol, gridParam.SortDir, name, username, email, cecNumber, liecenseNumbe, companyabn, companyName, resellerId, solarcompanyId);
            }
            if (lstUser.Count > 0)
            {
                gridParam.TotalDisplayRecords = lstUser.FirstOrDefault().TotalRecords;
                gridParam.TotalRecords = lstUser.FirstOrDefault().TotalRecords;
            }

            HttpContext.Response.Write(Grid.PrepareDataSet(lstUser, gridParam));
        }
        public string SendSERequest(SolarElectricianView solarElectricianView, string JobId = null)
        {
            int? userId = _userBAL.CreateSendRequest(solarElectricianView);
            //var usr = _userBAL.GetUserById(userId);
            //FormBot.Entity.User uv = DBClient.DataTableToList<FormBot.Entity.User>(usr.Tables[0]).FirstOrDefault();
            SolarCompany solarCompany = _solarCompanyBAL.GetSolarCompanyBySolarCompanyID(solarElectricianView.SolarCompanyId);

            EmailInfo emailInfo = new EmailInfo();
            emailInfo.FirstName = solarElectricianView.FirstName;
            emailInfo.LastName = solarElectricianView.LastName;
            emailInfo.SolarCompanyDetails = "Company Name: " + solarCompany.CompanyName + "<br/>" + "Company ABN: " + solarCompany.CompanyABN + "<br/>" + "Contact: " + solarCompany.Phone;
            if (userId != null && userId > 0)
            {
                emailInfo.TemplateID = 5;
            }
            else
            {
                emailInfo.TemplateID = 6;
                emailInfo.LoginLink = ProjectSession.LoginLink;
            }
            emailInfo.JobID = JobId == null ? 0 : Convert.ToInt32(JobId);
            _emailBAL.ComposeAndSendEmail(emailInfo, solarElectricianView.Email);
            string strEmailConfigureMsg = string.Empty;
            strEmailConfigureMsg = !ProjectSession.IsUserEmailAccountConfigured ? "(Can not send mail for request because email account is not configured)" : string.Empty;
            return strEmailConfigureMsg;
        }

        public async Task<ActionResult> UpdateJobDetailInstallerDesigner(int installerDesignerId, int jobId, int profileType, bool IsSWHUser, int userId = 0)
        {
            //int JobId = _userBAL.UpdateJob_InstallerDesignerId(installerDesignerId, jobId, profileType, IsSWHUser);
            InstallerDesignerView installerDesignerView = _userBAL.UpdateJob_InstallerDesignerId(installerDesignerId, jobId, profileType, IsSWHUser, userId);
            if (installerDesignerView.IsSystemUser && (!installerDesignerView.IsSolarElectrician || installerDesignerView.SEStatus != 2))
            {
                SolarElectricianView solarElectricianView = new SolarElectricianView();

                solarElectricianView.CECAccreditationNumber = installerDesignerView.CECAccreditationNumber;
                solarElectricianView.CECDesignerNumber = installerDesignerView.CECDesignerNumber;
                solarElectricianView.IsDeleted = false;
                solarElectricianView.UserId = installerDesignerView.UserId;
                solarElectricianView.ElectricianStatusId = installerDesignerView.IsAutoRequest ? 2 : 1;
                solarElectricianView.SERole = installerDesignerView.SEDesignRoleId;
                solarElectricianView.SolarCompanyId = installerDesignerView.SolarCompanyId;
                solarElectricianView.ElectricalContractorsLicenseNumber = installerDesignerView.ElectricalContractorsLicenseNumber; // For SWH user
                solarElectricianView.FirstName = installerDesignerView.FirstName;
                solarElectricianView.LastName = installerDesignerView.LastName;
                solarElectricianView.Email = installerDesignerView.Email;

                SendSERequest(solarElectricianView, Convert.ToString(jobId));
            }
            if (jobId > 0)
            {
                SortedList<string, string> data = new SortedList<string, string>();

                data.Add("InstallerName", installerDesignerView.FirstName + " " + installerDesignerView.LastName);
                await CommonBAL.SetCacheDataForSTCSubmission(null, jobId, data);
                string installerName = installerDesignerId != 0 ? installerDesignerView.FirstName + " " + installerDesignerView.LastName : "";
                string JobHistoryMessage = string.Empty;
                string Type = string.Empty;
                Type = profileType == 2 ? "installer" : profileType == 4 ? "designer" : "";
                if (string.IsNullOrEmpty(installerName))
                {
                    JobHistoryMessage = "has removed "+Type+".";
                }
                else
                {
                    JobHistoryMessage = "has updated " + Type + " to <b style=\"color:black\">" + installerName + "</b>.";
                }
                Common.SaveJobHistorytoXML(jobId, JobHistoryMessage, "General", "InstallerDesignerDetails", ProjectSession.LoggedInName, false);
            }

            return Json(new { status = true, SEStatus = installerDesignerView.IsAutoRequest ? 2 : 1, isVisitScheduled = installerDesignerView.IsVisitScheduled, Mobile = installerDesignerView.Mobile == null ? null : installerDesignerView.Mobile.Replace(" ", ""), Email = installerDesignerView.Email, FirstName = installerDesignerView.FirstName, LastName = installerDesignerView.LastName }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateJobDetailJobElectrician(int jobId, int solarCompanyId, int jobElectricianId, bool isCustomElectrician)
        {
            DataSet ds = _userBAL.UpdateJob_JobElectricianId(jobId, solarCompanyId, jobElectricianId, isCustomElectrician);
            string name = string.Empty;
            string JobHistoryMessage = string.Empty;
            if (ds!=null && ds.Tables.Count>0 && ds.Tables[1].Rows.Count > 0)
            {
                name = ds.Tables[1].Rows[0]["name"].ToString();
            }
            if (string.IsNullOrEmpty(name))
            {
                JobHistoryMessage = "has removed electrician.";
            }
            else
            {
                JobHistoryMessage = "has updated electrician to <b style=\"color:black\">" + name + "</b>.";
            }
            Common.SaveJobHistorytoXML(jobId, JobHistoryMessage, "General", "InstallerDesignerDetails", ProjectSession.LoggedInName, false);
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }

        public string GetErrorMessageForSolarElectrician(int no, SpResponce_CheckAccreditationNumber oSpResponce_CheckAccreditationNumber = null)
        {
            string sErrorMessage = "";
            if (oSpResponce_CheckAccreditationNumber != null)
            {
                if (oSpResponce_CheckAccreditationNumber.IsSameAccreditationNumber && !oSpResponce_CheckAccreditationNumber.IsSameLicenseNumber)
                {
                    sErrorMessage = "Solar Electrician with given accreditation number already exists in system.";
                }
                else if (oSpResponce_CheckAccreditationNumber.IsSameAccreditationNumber && oSpResponce_CheckAccreditationNumber.IsSameLicenseNumber)
                {
                    sErrorMessage = "Solar Electrician with given accreditation number, license number and email already exists in system.";
                }
                else if (!oSpResponce_CheckAccreditationNumber.IsSameAccreditationNumber && oSpResponce_CheckAccreditationNumber.IsSameLicenseNumber)
                {
                    sErrorMessage = "Solar Electrician with given license number and email already exists in system.";
                }
                else if (!oSpResponce_CheckAccreditationNumber.IsSameAccreditationNumber && !oSpResponce_CheckAccreditationNumber.IsSameLicenseNumber && oSpResponce_CheckAccreditationNumber.UserId == 0)
                {
                    sErrorMessage = "Solar Electrician with given accreditation or license number is not system user.";
                }
            }
            else
            {
                if (no == 4)
                {
                    //sErrorMessage = "Solar Electrician with given accreditation number are already exists.";
                    sErrorMessage = "Solar Electrician with given accreditation number or license number doesn't exist in master file.";
                }
                else if (no == 5)
                {
                    sErrorMessage = "Please Enter accreditation number or license number.";
                }
                else if (no == 6)
                {
                    sErrorMessage = "Solar Electrician request is already sent.";
                }
            }
            return sErrorMessage;
        }

        //public void SWHInstallerList(string LicenseNumber)
        //{
        //    GridParam gridParam = Grid.ParseParams(HttpContext.Request);
        //    //int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);
        //    IList<InstallerDesignerView> lstSWHInstaller = new List<InstallerDesignerView>();

        //    lstSWHInstaller = _userBAL.SWHInstallerList_ByLicenseNumber(LicenseNumber);

        //    //if (lstSWHInstaller.Count > 0)
        //    //{
        //    //    gridParam.TotalDisplayRecords = lstSWHInstaller.FirstOrDefault().TotalRecords;
        //    //    gridParam.TotalRecords = lstSWHInstaller.FirstOrDefault().TotalRecords;
        //    //}

        //    HttpContext.Response.Write(Grid.PrepareDataSet(lstSWHInstaller, gridParam));
        //}

        public void RemoveWholeSalerValidation()
        {
            ModelState.Remove("WholesalerFirstName");
            ModelState.Remove("WholesalerLastName");
            ModelState.Remove("WholesalerCompanyABN");
            ModelState.Remove("WholesalerCompanyName");
            ModelState.Remove("WholesalerUnitNumber");
            ModelState.Remove("WholesalerUnitTypeID");
            ModelState.Remove("WholesalerStreetNumber");
            ModelState.Remove("WholesalerStreetName");
            ModelState.Remove("WholesalerStreetTypeID");
            ModelState.Remove("WholesalerPostalAddressID");
            ModelState.Remove("WholesalerPostalDeliveryNumber");
            ModelState.Remove("WholesalerIsPostalAddress");
            ModelState.Remove("WholesalerState");
            ModelState.Remove("WholesalerTown");
            ModelState.Remove("WholesalerPostCode");
        }
        public void RemoveSAASUserValidation()
        {
            ModelState.Remove("Invoicer");
            ModelState.Remove("InvoicerFirstName");
            ModelState.Remove("InvoicerLastName");
            ModelState.Remove("InvoicerPhone");
            ModelState.Remove("UniqueContactId");
            ModelState.Remove("InvoicerUnitNumber");
            ModelState.Remove("InvoicerUnitTypeID");
            ModelState.Remove("InvoicerStreetNumber");
            ModelState.Remove("InvoicerStreetName");
            ModelState.Remove("InvoicerStreetTypeID");
            ModelState.Remove("InvoicerPostalAddressID");
            ModelState.Remove("InvoicerPostalDeliveryNumber");
            ModelState.Remove("InvoicerAddressID");
            ModelState.Remove("InvoicerState");
            ModelState.Remove("InvoicerTown");
            ModelState.Remove("InvoicerPostCode");
        }

        public JsonResult GetInstallerDesignerForSCADashboard()
        {
            //List<InstallerDesignerView> installers = _userBAL.GetInstallerDesignerAddByProfile(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, 100, 1, null, null, ProjectSession.SolarCompanyId, null, null, 0, 1, null, false);
            List<InstallerDesignerView> installers = _userBAL.GetInstallerDesignerWithStatus(true, ProjectSession.SolarCompanyId, 0, false);
            return Json(installers, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UploadProfilePhoto(string userId, int solarCompanyId)
        {
            List<HelperClasses.UploadStatus> uploadStatus = new List<HelperClasses.UploadStatus>();
            if (Request.Files != null && Request.Files.Count != 0)
            {
                for (var i = 0; i < Request.Files.Count; i++)
                {
                    uploadStatus.Add(GetFileUpload(Request.Files[i], userId));
                    if (uploadStatus[i].Status == true)
                    {
                        string filename = uploadStatus[i].FileName.Replace("%", "$");
                        _userBAL.UploadProfilePhoto(solarCompanyId, filename);
                    }
                }
            }

            return Json(uploadStatus);
        }

        public JsonResult GetActivityLog(int UserId, int ActivityTypeId, string StartDate, string EndDate, int page = 0, int pageSize = 10)
        {
            var list = _activityLog.GetActivityLogs(UserId, ActivityTypeId, StartDate, EndDate, page, pageSize);
            return Json(new { total = list.Count > 0 ? list[0].TotalRecords : 0, data = list }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeviceLogout(int id, bool isLogoutAll)
        {
            try
            {
                _userBAL.DeviceLogout(id, isLogoutAll);
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetUserDeviceKendo(int userId)
        {
            List<UserDevice> lstUserDevice = _userBAL.GetUserDeviceInfo(userId);
            return Json(new { total = lstUserDevice.Count, data = lstUserDevice }, JsonRequestBehavior.AllowGet);
        }

        public void ExportSCA(string name = "", string username = "", string state = "", string companyname = "", string email = "", string companyabn = "", string status = "", int Reseller = 0, int RamId = 0, string mobile = "")
        {
            int userstatus = !string.IsNullOrWhiteSpace(status) ? Convert.ToInt32((SystemEnums.UserStatus)Enum.Parse(typeof(SystemEnums.UserStatus), status).GetHashCode()) : 0;
            DataSet ds = _userBAL.ExportSCA(name, username, state, companyname, email, companyabn, userstatus, Reseller, RamId, mobile);
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(ds.Tables[0]);
                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename= GreenBot-SCA.xlsx");

                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }
        }

        /// <summary>
        /// Send Mail to Installer for uploading Id Proofs
        /// </summary>
        /// <returns>Returns Bool value for sending mail</returns>
        [HttpPost]
        public JsonResult SendMailForDocumentsInstaller()
        {
            try
            {
                int emailTemplateId = 47;
                List<FormBot.Entity.User> users = new List<FormBot.Entity.User>();
                users = _userBAL.GetAllApprovedElectricianList();
                foreach (var user in users)
                {
                    SendMail(user);
                }
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        public void SendMail(Entity.User user)
        {
            EmailInfo emailInfo = new EmailInfo();
            emailInfo.TemplateID = 47;
            emailInfo.FirstName = user.FirstName;
            emailInfo.LastName = user.LastName;
            emailInfo.Details = "";
            emailInfo.LoginLink = ProjectSession.LoginLink + "Account/Login/?returnUrl=User/Profile&flg=DocUpload";
            _emailBAL.ComposeAndSendEmail(emailInfo, user.Email);
        }

        /// <summary>
        /// Send Mail to Installer for uploading Id Proofs
        /// </summary>
        /// <returns>Returns Bool value for sending mail</returns>
        [HttpPost]
        public JsonResult SendMailForDocumentsFSA(Entity.User user)
        {
            try
            {
                int emailTemplateId = 49;
                EmailInfo emailInfo = new EmailInfo();
                emailInfo.TemplateID = emailTemplateId;
                emailInfo.FirstName = user.FirstName;
                emailInfo.LastName = user.LastName;
                emailInfo.Details = "";
                emailInfo.LoginLink = ProjectSession.LoginLink + "Account/Login/?returnUrl=User/ViewDetail/" + QueryString.QueryStringEncode("id=" + user.UserId) + "/DocVerify";
                string mailAddresses = _userBAL.GetFSAEmailAddresses();
                _emailBAL.ComposeAndSendEmail(emailInfo, mailAddresses);
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }
        [HttpGet]
        public JsonResult GetLogsForUpdateContact(int UserId)
        {
            try
            {
                List<UpdateContactXeroLog> UpdateContactXeroLogs = _userBAL.GetLogsForUpdateContact(UserId);
                return Json(new { UpdateContactXeroLogs = UpdateContactXeroLogs, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { ex = ex, success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public PartialViewResult InstallerDesigner(int usertypeId, int solarcompanyId)
        {
            FormBot.Entity.User modelData = new Entity.User();
            modelData.UserTypeID = usertypeId;
            modelData.SolarCompanyId = solarcompanyId;
            var statuses = from SystemEnums.SEDesignRole s in Enum.GetValues(typeof(SystemEnums.SEDesignRole))
                           select new { ID = s, Name = s.ToString().Replace('_', ' ') };
            ViewData["SEDesignRole"] = new SelectList(statuses, "ID", "Name");

            return PartialView("InstallerDesigner", modelData);
        }

        
        [HttpGet]
        public JsonResult SaveUserNotetoXML(int UserID, string Notes, int NotesType, int UserTypeID, bool IsImportant = false, string NoteID = null)
        {
            if (string.IsNullOrEmpty(NoteID))
            {
                try
                {
                    if (NotesType == 5)
                    {
                        
                        var NotesXMLPath = Path.Combine(Path.Combine(ProjectConfiguration.ProofDocumentsURL, "StaticTemplate/SPV/UserNote.xml"));
                        string fullDirectoryPath = Path.Combine(ProjectConfiguration.ProofDocumentsURL, "UserDocuments", "WarningNotes");
                        if (!Directory.Exists(fullDirectoryPath))
                            Directory.CreateDirectory(fullDirectoryPath);
                        string fullFilePath = Path.Combine(fullDirectoryPath, "WarningNotes.xml");
                        if (System.IO.File.Exists(fullFilePath))
                        {
                            XDocument olddoc = XDocument.Load(fullFilePath);
                            var count = olddoc.Descendants("Note").Count();
                            XElement root = new XElement("Note");
                            root.Add(new XElement("UserID", Convert.ToString(UserID)));
                            root.Add(new XElement("NoteID", count + 1));
                            root.Add(new XElement("NoteDescription", Notes));
                            root.Add(new XElement("NotesType", "Warning"));
                            root.Add(new XElement("CreatedBy", Convert.ToString(ProjectSession.LoggedInName)));
                            root.Add(new XElement("CreatedDate", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz")));
                            root.Add(new XElement("IsDeleted", Convert.ToString(false)));
                            root.Add(new XElement("IsEdit", Convert.ToString(false)));
                            root.Add(new XElement("ModifiedBy", null));
                            root.Add(new XElement("ModifiedDate", null));
                            root.Add(new XElement("IsImportant", Convert.ToString(IsImportant)));
                            olddoc.Element("UserNotes").Add(root);
                            olddoc.Save(fullFilePath);
                        }
                        else
                        {
                            XmlDocument xmlDoc = new XmlDocument();
                            xmlDoc.PreserveWhitespace = true;
                            xmlDoc.Load(NotesXMLPath);
                            XDocument doc = XDocument.Parse(xmlDoc.InnerXml);
                            xmlDoc.InnerXml = doc.ToString();
                            xmlDoc.InnerXml = xmlDoc.InnerXml.Replace("[[UserID]]", UserID.ToString())
                                .Replace("[[NoteID]]", "1")
                                .Replace("[[NoteDescription]]", HttpUtility.HtmlEncode(Notes))
                                .Replace("[[NotesType]]", "Warning")
                                .Replace("[[CreatedBy]]", ProjectSession.LoggedInName)
                                .Replace("[[CreatedDate]]", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"))
                                .Replace("[[IsDeleted]]", Convert.ToString(false))
                                .Replace("[[IsEdit]]", Convert.ToString(false))
                                .Replace("[[ModifiedBy]]", null)
                                .Replace("[[ModifiedDate]]", null)
                                .Replace("[[IsImportant]]", Convert.ToString(IsImportant));
                            XmlWriterSettings settings = new XmlWriterSettings();
                            settings.Encoding = new UTF8Encoding(false);
                            settings.Indent = true;
                            using (XmlWriter writer = XmlWriter.Create(fullFilePath, settings))
                            {
                                xmlDoc.Save(writer);
                            }
                        }
                    }
                    else
                    {
                        var NotesXMLPath = Path.Combine(Path.Combine(ProjectConfiguration.ProofDocumentsURL, "StaticTemplate/SPV/UserNote.xml"));
                        string fullDirectoryPath = Path.Combine(ProjectConfiguration.ProofDocumentsURL, "UserDocuments", UserID.ToString(), "UserNotes");
                        if (!Directory.Exists(fullDirectoryPath))
                            Directory.CreateDirectory(fullDirectoryPath);
                        string fullFilePath = Path.Combine(fullDirectoryPath, "Notes_" + UserID.ToString() + ".xml");
                        if (System.IO.File.Exists(fullFilePath))
                        {
                            XDocument olddoc = XDocument.Load(fullFilePath);
                            var count = olddoc.Descendants("Note").Count();
                            XElement root = new XElement("Note");
                            root.Add(new XElement("UserID", Convert.ToString(UserID)));
                            root.Add(new XElement("NoteID", count + 1));
                            root.Add(new XElement("NoteDescription", Notes));
                            root.Add(new XElement("NotesType", Enum.GetName(typeof(SystemEnums.NotesType), NotesType)));
                            root.Add(new XElement("CreatedBy", Convert.ToString(ProjectSession.LoggedInName)));
                            root.Add(new XElement("CreatedDate", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz")));
                            root.Add(new XElement("IsDeleted", Convert.ToString(false)));
                            root.Add(new XElement("IsEdit", Convert.ToString(false)));
                            root.Add(new XElement("ModifiedBy", null));
                            root.Add(new XElement("ModifiedDate", null));
                            root.Add(new XElement("IsImportant", Convert.ToString(IsImportant)));
                            olddoc.Element("UserNotes").Add(root);
                            olddoc.Save(fullFilePath);
                        }
                        //System.IO.File.Delete(fullFilePath);
                        else
                        {
                            XmlDocument xmlDoc = new XmlDocument();
                            xmlDoc.PreserveWhitespace = true;
                            xmlDoc.Load(NotesXMLPath);
                            XDocument doc = XDocument.Parse(xmlDoc.InnerXml);
                            xmlDoc.InnerXml = doc.ToString();
                            xmlDoc.InnerXml = xmlDoc.InnerXml.Replace("[[UserID]]", Convert.ToString(UserID))
                                            .Replace("[[NoteID]]", "1")
                                            .Replace("[[NoteDescription]]", HttpUtility.HtmlEncode(Notes))
                                            .Replace("[[NotesType]]", Enum.GetName(typeof(SystemEnums.NotesType), NotesType))
                                            .Replace("[[CreatedBy]]", Convert.ToString(ProjectSession.LoggedInName))
                                            .Replace("[[CreatedDate]]", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"))
                                            .Replace("[[IsDeleted]]", Convert.ToString(false))
                                            .Replace("[[IsEdit]]", Convert.ToString(false))
                                            .Replace("[[ModifiedBy]]", null)
                                            .Replace("[[ModifiedDate]]", null)
                                            .Replace("[[IsImportant]]", Convert.ToString(IsImportant));



                            XmlWriterSettings settings = new XmlWriterSettings();
                            settings.Encoding = new UTF8Encoding(false);
                            settings.Indent = true;
                            using (XmlWriter writer = XmlWriter.Create(fullFilePath, settings))
                            {
                                xmlDoc.Save(writer);
                            }
                        }
                    }
                    HtmlDocument notesdoc = new HtmlDocument();
                    notesdoc.LoadHtml(Notes);
                    var SelectTaggedUsers = notesdoc.DocumentNode.SelectNodes("//a[contains(@class, 'tagged')]");
                    List<string> AllTaggedUsers = new List<string>();
                    if (SelectTaggedUsers != null)
                    {
                        for (int j = 0; j < SelectTaggedUsers.Count; j++)
                        {
                            AllTaggedUsers.Add(SelectTaggedUsers[j].InnerHtml);
                        }
                        AllTaggedUsers = AllTaggedUsers.Select(p => !string.IsNullOrEmpty(p) ? p.Substring(1) : p).ToList();
                        for (int m = 0; m < AllTaggedUsers.Count; m++)
                        {
                            if (AllTaggedUsers[m].Contains('@'))
                            {

                                string[] TaggedList = AllTaggedUsers[m].Split('@');
                                AllTaggedUsers.Remove(AllTaggedUsers[m]);
                                foreach (string tagged in TaggedList)
                                {
                                    AllTaggedUsers.Add(tagged);
                                }
                            }
                        }
                    }
                    List<string> Emailid = new List<string>();
                    //var emails = notesdoc.DocumentNode.SelectNodes("//a[contains(@class,'tagged)']" + a[@data-emaild] ");
                    var emails = notesdoc.DocumentNode.SelectNodes("//a[@data-emailid]");
                    if (emails != null)
                    {
                        foreach (var email in emails)
                        {
                            Emailid.Add(email.GetAttributeValue("data-emailid", ""));
                        }
                    }
                    if (Emailid.Count > 0)
                    {
                        for (int j = 0; j < Emailid.Count; j++)
                        {
                            EmailInfo emailInfo = new EmailInfo();
                            emailInfo.TemplateID = 51;
                            emailInfo.UserName = AllTaggedUsers[j];
                            emailInfo.UserID = UserID;
                            //emailInfo.JobDetailLink = "<a href=" + ProjectSession.LoginLink + Url.Action("Index", "Job") + "?id=" + emailInfo.Id + ">" + ProjectSession.LoginLink + Url.Action("Index", "Job") + "?id=" + emailInfo.Id + "</a>";
                            //emailInfo.JobDetailLink = "<a href=http://localhost:56199/Job/Index?id=" + emailInfo.Id + "> http://localhost:56199/Job/Index?id=" + emailInfo.Id + "</a>";
                            //emailInfo.ReferenceNumber = JobRefNo;
                            emailInfo.Details = Notes;
                            _emailBAL.ComposeAndSendEmail(emailInfo, Emailid[j]);
                        }
                    }
                    //if (UserTypeID == 4 || UserTypeID == 7)
                    //{
                    //    _createJobBAL.InsertUserNote(UserID, Notes);
                    //}
                    return Json(new { status = true, message = "User Note has been saved." }, JsonRequestBehavior.AllowGet);
                }

                catch (Exception e)
                {
                    return Json(new { status = false, message = "User Note has not been saved." }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                try
                {
                    if (NotesType == 5)
                    {
                        string fullDirectoryPath = Path.Combine(ProjectConfiguration.ProofDocumentsURL, "UserDocuments", "WarningNotes");
                        string fullFilePath = Path.Combine(fullDirectoryPath, "WarningNotes.xml");
                        if (System.IO.File.Exists(fullFilePath))
                        {
                            XDocument doc = XDocument.Load(fullFilePath);
                            var Note = doc.Descendants("Note").Single(r => (string)r.Element("NoteID") == NoteID);
                            if (Note != null)
                            {
                                //Note.Element("IsDeleted").Value = Convert.ToString(true);
                                if (Note.Element("IsDeleted").Value == "True")
                                {
                                    return Json(new { stauts = false, message = "User Note does not exist." }, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    HtmlDocument notesdoc = new HtmlDocument();
                                    notesdoc.LoadHtml(Notes);
                                    var SelectTaggedUsers = notesdoc.DocumentNode.SelectNodes("//a[contains(@class, 'tagged')]");
                                    //var tagged3 = doc1.DocumentNode.Descendants("a").Where(d => d.Attributes["class"].Value.Contains("tagged"));
                                    //IEnumerable<HtmlNode> tagged2 =doc.DocumentNode.Descendants(0).Where(n => n.HasClass("class-name")); 
                                    List<string> AllTaggedUsers = new List<string>();
                                    if (SelectTaggedUsers != null)
                                    {
                                        for (int j = 0; j < SelectTaggedUsers.Count; j++)
                                        {
                                            AllTaggedUsers.Add(SelectTaggedUsers[j].InnerHtml);
                                        }
                                        AllTaggedUsers = AllTaggedUsers.Select(p => !string.IsNullOrEmpty(p) ? p.Substring(1) : p).ToList();
                                        for (int m = 0; m < AllTaggedUsers.Count; m++)
                                        {
                                            if (AllTaggedUsers[m].Contains('@'))
                                            {

                                                string[] TaggedList = AllTaggedUsers[m].Split('@');
                                                AllTaggedUsers.Remove(AllTaggedUsers[m]);
                                                foreach (string tagged in TaggedList)
                                                {
                                                    AllTaggedUsers.Add(tagged);
                                                }
                                            }
                                        }
                                    }
                                    string taggeduserlist = string.Join(",", AllTaggedUsers.ToArray());
                                    //string JobHistoryMessage = "edited a " + Note.Element("NotesType").Value + " note - <b class=\"blue-title\">(" + JobId + ") " + JobRefNo + "</b> <a data-noteid=" + NoteID + " style=\"background:url(../Images/delete-icon.png) no-repeat center; text-decoration:none;\" title=\"Delete\"  href=\"javascript: void(0)\" style=\"cursor: pointer\" onclick=\"DeleteNote(this)\">&nbsp;&nbsp;&nbsp;&nbsp;</a> <a data-noteid=" + NoteID + " style=\"background:url(../Images/edit-icon.png) no-repeat center; text-decoration:none;\" title=\"Edit\"  href=\"javascript: void(0)\" style=\"cursor: pointer\" onclick=\"EditNote(this)\">&nbsp;&nbsp;&nbsp;&nbsp;</a>";
                                    bool IsEdit = Convert.ToBoolean(Note.Element("IsEdit").Value);

                                    var NotesTypeEnum = 5;
                                    string NotesTypeString = "Warning";
                                    if (!IsEdit)
                                    {
                                        string UserHistoryMessageAdd = "";
                                        string UserHistoryMessageEdit = "";
                                        string JobHistoryAddedBy = Note.Element("CreatedBy").Value;
                                        if (AllTaggedUsers.Count == 0)
                                        {
                                            UserHistoryMessageAdd = "published a " + Note.Element("NotesType").Value + " note " + "<span style=\"float:right\">NoteID: " + Note.Element("NoteID").Value + "</span>" + Note.Element("NoteDescription").Value;
                                        }
                                        else
                                        {
                                            UserHistoryMessageAdd = "published a " + Note.Element("NotesType").Value + " note " + "<span style=\"float:right\">NoteID: " + Note.Element("NoteID").Value + "</span>" + Note.Element("NoteDescription").Value;
                                        }


                                        UserHistoryMessageEdit = UserHistoryMessageAdd.Replace("added", "edited");
                                        //descriptionedit = "Note edited by <b class=\"blue-title\">" + ProjectSession.LoggedInName + "</b> on <span class=\"blue-title\">" + DateTime.Now.ToString("dd/MM/yyyy hh:mmtt") + "</span><p><span class=\"blue-title\">From: </span></p>" + descriptionadd + "<p><span class=\"blue-title\">To: </span>" + notes;
                                        string CreatedDate = Note.Element("CreatedDate").Value;
                                        Common.SaveUserHistorytoXML(UserID, UserHistoryMessageAdd, JobHistoryAddedBy, CreatedDate, NotesType, true, NoteID);
                                        //Common.SaveJobHistorytoXML(JobId, JobHistoryMessageAdd, "General", "EditNote", JobHistoryAddedBy, false, descriptionadd, NoteID, NotesTypeString, CreatedDate);
                                        //Common.SaveJobHistorytoXML(JobId, JobHistoryMessageEdit, "General", "EditNote", ProjectSession.LoggedInName, false, descriptionedit, NoteID, NotesTypeString);
                                    }
                                    Note.Element("NoteDescription").Value = Notes;
                                    Note.Element("ModifiedBy").Value = ProjectSession.LoggedInName;
                                    Note.Element("ModifiedDate").Value = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");
                                    Note.Element("IsImportant").Value = Convert.ToString(IsImportant);
                                    Note.Element("NotesType").Value = NotesTypeString;
                                    Note.Element("IsEdit").Value = Convert.ToString(true);
                                    doc.Save(fullFilePath);



                                    List<string> Emailid = new List<string>();
                                    //var emails = notesdoc.DocumentNode.SelectNodes("//a[contains(@class,'tagged)']" + a[@data-emaild] ");
                                    var emails = notesdoc.DocumentNode.SelectNodes("//a[@data-emailid]");
                                    if (emails != null)
                                    {
                                        foreach (var email in emails)
                                        {
                                            Emailid.Add(email.GetAttributeValue("data-emailid", ""));
                                        }
                                    }
                                    if (Emailid.Count > 0)
                                    {
                                        for (int j = 0; j < Emailid.Count; j++)
                                        {
                                            EmailInfo emailInfo = new EmailInfo();
                                            emailInfo.TemplateID = 51;
                                            emailInfo.UserName = AllTaggedUsers[j];
                                            emailInfo.UserID = UserID;
                                            //emailInfo.JobDetailLink = "<a href=" + ProjectSession.LoginLink + Url.Action("Index", "Job") + "?id=" + emailInfo.Id + ">" + ProjectSession.LoginLink + Url.Action("Index", "Job") + "?id=" + emailInfo.Id + "</a>";
                                            //emailInfo.JobDetailLink = "<a href=http://localhost:56199/Job/Index?id=" + emailInfo.Id + "> http://localhost:56199/Job/Index?id=" + emailInfo.Id + "</a>";
                                            //emailInfo.ReferenceNumber = insert;
                                            emailInfo.Details = Notes;
                                            _emailBAL.ComposeAndSendEmail(emailInfo, Emailid[j]);
                                        }
                                    }
                                    //if (UserTypeID == 4 || UserTypeID == 7)
                                    //{
                                    //    _createJobBAL.InsertUserNote(UserID, Notes);
                                    //}
                                    return Json(new { status = true, message = "User Note has been edited successfully." }, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                return Json(new { status = false, message = "User Note does not exist." }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json(new { status = false, message = "User Note does not exist." }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        string fullDirectoryPath = Path.Combine(ProjectConfiguration.ProofDocumentsURL, "UserDocuments", UserID.ToString(), "UserNotes");
                        string fullFilePath = Path.Combine(fullDirectoryPath, "Notes_" + UserID + ".xml");
                        if (System.IO.File.Exists(fullFilePath))
                        {
                            XDocument doc = XDocument.Load(fullFilePath);
                            var Note = doc.Descendants("Note").Single(r => (string)r.Element("NoteID") == NoteID);
                            if (Note != null)
                            {
                                //Note.Element("IsDeleted").Value = Convert.ToString(true);
                                if (Note.Element("IsDeleted").Value == "True")
                                {
                                    return Json(new { stauts = false, message = "User Note does not exist." }, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    HtmlDocument notesdoc = new HtmlDocument();
                                    notesdoc.LoadHtml(Notes);
                                    var SelectTaggedUsers = notesdoc.DocumentNode.SelectNodes("//a[contains(@class, 'tagged')]");
                                    //var tagged3 = doc1.DocumentNode.Descendants("a").Where(d => d.Attributes["class"].Value.Contains("tagged"));
                                    //IEnumerable<HtmlNode> tagged2 =doc.DocumentNode.Descendants(0).Where(n => n.HasClass("class-name")); 
                                    List<string> AllTaggedUsers = new List<string>();
                                    if (SelectTaggedUsers != null)
                                    {
                                        for (int j = 0; j < SelectTaggedUsers.Count; j++)
                                        {
                                            AllTaggedUsers.Add(SelectTaggedUsers[j].InnerHtml);
                                        }
                                        AllTaggedUsers = AllTaggedUsers.Select(p => !string.IsNullOrEmpty(p) ? p.Substring(1) : p).ToList();
                                        for (int m = 0; m < AllTaggedUsers.Count; m++)
                                        {
                                            if (AllTaggedUsers[m].Contains('@'))
                                            {

                                                string[] TaggedList = AllTaggedUsers[m].Split('@');
                                                AllTaggedUsers.Remove(AllTaggedUsers[m]);
                                                foreach (string tagged in TaggedList)
                                                {
                                                    AllTaggedUsers.Add(tagged);
                                                }
                                            }
                                        }
                                    }
                                    string taggeduserlist = string.Join(",", AllTaggedUsers.ToArray());
                                    //string JobHistoryMessage = "edited a " + Note.Element("NotesType").Value + " note - <b class=\"blue-title\">(" + JobId + ") " + JobRefNo + "</b> <a data-noteid=" + NoteID + " style=\"background:url(../Images/delete-icon.png) no-repeat center; text-decoration:none;\" title=\"Delete\"  href=\"javascript: void(0)\" style=\"cursor: pointer\" onclick=\"DeleteNote(this)\">&nbsp;&nbsp;&nbsp;&nbsp;</a> <a data-noteid=" + NoteID + " style=\"background:url(../Images/edit-icon.png) no-repeat center; text-decoration:none;\" title=\"Edit\"  href=\"javascript: void(0)\" style=\"cursor: pointer\" onclick=\"EditNote(this)\">&nbsp;&nbsp;&nbsp;&nbsp;</a>";
                                    bool IsEdit = Convert.ToBoolean(Note.Element("IsEdit").Value);

                                    var NotesTypeEnum = (SystemEnums.NotesType)NotesType;
                                    string NotesTypeString = NotesTypeEnum.ToString();
                                    if (!IsEdit)
                                    {
                                        string UserHistoryMessageAdd = "";
                                        string UserHistoryMessageEdit = "";
                                        string JobHistoryAddedBy = Note.Element("CreatedBy").Value;
                                        if (AllTaggedUsers.Count == 0)
                                        {
                                            UserHistoryMessageAdd = "published a " + Note.Element("NotesType").Value + " note " + "<span style=\"float:right\">NoteID: " + Note.Element("NoteID").Value + "</span>" + Note.Element("NoteDescription").Value;
                                        }
                                        else
                                        {
                                            UserHistoryMessageAdd = "published a " + Note.Element("NotesType").Value + " note " + "<span style=\"float:right\">NoteID: " + Note.Element("NoteID").Value + "</span>" + Note.Element("NoteDescription").Value;
                                        }


                                        UserHistoryMessageEdit = UserHistoryMessageAdd.Replace("added", "edited");

                                        //descriptionedit = "Note edited by <b class=\"blue-title\">" + ProjectSession.LoggedInName + "</b> on <span class=\"blue-title\">" + DateTime.Now.ToString("dd/MM/yyyy hh:mmtt") + "</span><p><span class=\"blue-title\">From: </span></p>" + descriptionadd + "<p><span class=\"blue-title\">To: </span>" + notes;
                                        string notestype = Note.Element("NotesType").Value;
                                        int NotesTypeValue = !string.IsNullOrEmpty(notestype) ? Convert.ToInt32((SystemEnums.NotesType)Enum.Parse(typeof(SystemEnums.NotesType), notestype).GetHashCode()) : 0;
                                        string CreatedDate = Note.Element("CreatedDate").Value;
                                        Common.SaveUserHistorytoXML(UserID, UserHistoryMessageAdd, JobHistoryAddedBy, CreatedDate, NotesTypeValue, false, NoteID);
                                        //Common.SaveJobHistorytoXML(JobId, JobHistoryMessageAdd, "General", "EditNote", JobHistoryAddedBy, false, descriptionadd, NoteID, NotesTypeString, CreatedDate);
                                        //Common.SaveJobHistorytoXML(JobId, JobHistoryMessageEdit, "General", "EditNote", ProjectSession.LoggedInName, false, descriptionedit, NoteID, NotesTypeString);
                                    }

                                    Note.Element("NoteDescription").Value = Notes;
                                    Note.Element("ModifiedBy").Value = ProjectSession.LoggedInName;
                                    Note.Element("ModifiedDate").Value = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");
                                    Note.Element("IsImportant").Value = Convert.ToString(IsImportant);
                                    Note.Element("NotesType").Value = NotesTypeString;
                                    Note.Element("IsEdit").Value = Convert.ToString(true);
                                    doc.Save(fullFilePath);



                                    List<string> Emailid = new List<string>();
                                    //var emails = notesdoc.DocumentNode.SelectNodes("//a[contains(@class,'tagged)']" + a[@data-emaild] ");
                                    var emails = notesdoc.DocumentNode.SelectNodes("//a[@data-emailid]");
                                    if (emails != null)
                                    {
                                        foreach (var email in emails)
                                        {
                                            Emailid.Add(email.GetAttributeValue("data-emailid", ""));
                                        }
                                    }
                                    if (Emailid.Count > 0)
                                    {
                                        for (int j = 0; j < Emailid.Count; j++)
                                        {
                                            EmailInfo emailInfo = new EmailInfo();
                                            emailInfo.TemplateID = 51;
                                            emailInfo.UserName = AllTaggedUsers[j];
                                            emailInfo.UserID = UserID;
                                            emailInfo.Details = Notes;
                                            _emailBAL.ComposeAndSendEmail(emailInfo, Emailid[j]);
                                        }
                                    }
                                    //if (UserTypeID == 4 || UserTypeID == 7)
                                    //{
                                    //    _createJobBAL.InsertUserNote(UserID, Notes);
                                    //}
                                    return Json(new { status = true, message = "User Note has been edited successfully." }, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                return Json(new { status = false, message = "User Note does not exist." }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json(new { status = false, message = "User Note does not exist." }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                catch (Exception e)
                {
                    return Json(new { status = false, message = "Failed to save edited User Note." }, JsonRequestBehavior.AllowGet);
                }
            }
        }


        [HttpPost]
        public ActionResult uploadImageCkeditor(int UserID)
        {
            try
            {
                string fullDirectoryPath = Path.Combine(ProjectConfiguration.ProofDocumentsURL, "UserDocuments", UserID.ToString(), "Notes_Images");
                if (!Directory.Exists(fullDirectoryPath))
                    Directory.CreateDirectory(fullDirectoryPath);
                string filepath = string.Empty;

                HttpPostedFile httpPostedFile = System.Web.HttpContext.Current.Request.Files[0];

                string filename = httpPostedFile.FileName;
                filepath = Path.Combine(fullDirectoryPath, filename);
                if (System.IO.File.Exists(filepath))
                {
                    string orignalFileName = Path.GetFileNameWithoutExtension(filepath);
                    string fileExtension = Path.GetExtension(filepath);
                    string fileDirectory = Path.GetDirectoryName(filepath);
                    int j = 1;
                    while (true)
                    {
                        string renameFileName = fileDirectory + @"\" + orignalFileName + "(" + j + ")" + fileExtension;
                        if (System.IO.File.Exists(renameFileName))
                        {
                            j++;
                        }
                        else
                        {
                            filepath = renameFileName;
                            break;
                        }

                    }
                }
                httpPostedFile.SaveAs(filepath);

                string imageurl = Path.Combine(ProjectSession.UploadedDocumentPath, "UserDocuments", UserID.ToString(), "Notes_Images", filename);
                //string imageurl = "https://staging.greenbot.com.au/files/JobDocuments/164494/Notes_Images/1%20(1).jpg";// "https://picsum.photos/200/300";

                return Json(new { url = imageurl, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Helper.Log.WriteError(ex, "Exception in UploadImageInNotes: ");
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public List<UserHistory> GetUserHistory(string UserID, int? categoryID, int IsDeletedUserNote,bool IsImportant=false)
        {
            List<UserHistory> objlstUserHistory = new List<UserHistory>();
            string UserHistoryDirectoryPath = Path.Combine(ProjectConfiguration.ProofDocumentsURL, "UserDocuments", UserID.ToString(), "UserHistory");
            string UserHistoryFilePath = Path.Combine(UserHistoryDirectoryPath, "UserHistory_" + UserID.ToString() + ".xml");
            if (System.IO.File.Exists(UserHistoryFilePath))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(UserHistoryFilePath);
                XmlNodeList History = doc.DocumentElement.SelectNodes("/UserHistory/History");

                foreach (XmlNode node in History)
                {
                    UserHistory objuserhistory = new UserHistory();
                    objuserhistory.CreatedBy = node.SelectSingleNode("ModifiedBy").InnerText;
                    objuserhistory.CreatedDate = Convert.ToDateTime(node.SelectSingleNode("ModifiedDate").InnerText).ToString("dd/MM/yyyy hh:mmtt");
                    objuserhistory.HistoryMessage = node.SelectSingleNode("UserHistoryMessage").InnerText;
                    objuserhistory.CategoryID = Convert.ToInt32(node.SelectSingleNode("HistoryType").InnerText);
                    objuserhistory.CreateDate = Convert.ToDateTime(node.SelectSingleNode("ModifiedDate").InnerText);
                    objuserhistory.IsImportant = false;
                    objuserhistory.Id = 0;
                    bool IsDeleted = Convert.ToBoolean(node.SelectSingleNode("IsDeleted").InnerText);
                    if(IsDeleted)
                    {
                        objuserhistory.IsDeleted = 2;
                    }
                    else
                    {
                        objuserhistory.IsDeleted = 3;
                    }
                    if ((objuserhistory.CategoryID == categoryID||categoryID==0)&& objuserhistory.CategoryID==1)
                    {
                        if ((IsDeletedUserNote == 1) || (IsDeletedUserNote != 1 && IsDeletedUserNote == objuserhistory.IsDeleted))
                        {
                            objlstUserHistory.Add(objuserhistory);
                        }
                    }
                }
            }

            string UserNotesDirectoryPath = Path.Combine(ProjectConfiguration.ProofDocumentsURL, "UserDocuments", UserID.ToString(), "UserNotes");
            string UserNotesFilePath = Path.Combine(UserNotesDirectoryPath, "Notes_" + UserID.ToString() + ".xml");
            if (System.IO.File.Exists(UserNotesFilePath))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(UserNotesFilePath);
                XmlNodeList History = doc.DocumentElement.SelectNodes("/UserNotes/Note");

                foreach (XmlNode node in History)
                {
                    UserHistory objuserhistory = new UserHistory();
                    objuserhistory.CreatedBy = node.SelectSingleNode("CreatedBy").InnerText;
                    objuserhistory.CreatedDate = Convert.ToDateTime(node.SelectSingleNode("CreatedDate").InnerText).ToString("dd/MM/yyyy hh:mmtt");
                    string Category = node.SelectSingleNode("NotesType").InnerText;
                    //objuserhistory.HistoryMessage = "added a " + Category +" Note" + node.SelectSingleNode("NoteDescription").InnerText;    
                    int CategoryID = !string.IsNullOrEmpty(Category) ? Convert.ToInt32((SystemEnums.NotesType)Enum.Parse(typeof(SystemEnums.NotesType), Category).GetHashCode()) : 0;
                    objuserhistory.CategoryID = CategoryID;
                    objuserhistory.CreateDate = Convert.ToDateTime(node.SelectSingleNode("CreatedDate").InnerText);
                    objuserhistory.IsImportant = Convert.ToBoolean(node.SelectSingleNode("IsImportant").InnerText);
                    string noteid = node.SelectSingleNode("NoteID").InnerText;
                    string TaggedUsers = node.SelectSingleNode("NoteDescription").InnerText;
                    bool IsEdit = Convert.ToBoolean(node.SelectSingleNode("IsEdit").InnerText);
                    bool IsDeleted = Convert.ToBoolean(node.SelectSingleNode("IsDeleted").InnerText);
                    objuserhistory.Id =(!string.IsNullOrEmpty(noteid))?Convert.ToInt32(noteid):0;
                    if(IsDeleted)
                    {
                        objuserhistory.IsDeleted = 2;
                    }
                    else
                    {
                        objuserhistory.IsDeleted = 3;
                    }
                    
                    HtmlDocument notesdoc = new HtmlDocument();
                    notesdoc.LoadHtml(TaggedUsers);
                    var SelectTaggedUsers = notesdoc.DocumentNode.SelectNodes("//a[contains(@class, 'tagged')]");
                    List<string> AllTaggedUsers = new List<string>();
                    if (SelectTaggedUsers != null)
                    {
                        for (int j = 0; j < SelectTaggedUsers.Count; j++)
                        {
                            AllTaggedUsers.Add(SelectTaggedUsers[j].InnerHtml);
                        }
                        AllTaggedUsers = AllTaggedUsers.Select(p => !string.IsNullOrEmpty(p) ? p.Substring(1) : p).ToList();
                        for (int m = 0; m < AllTaggedUsers.Count; m++)
                        {
                            if (AllTaggedUsers[m].Contains('@'))
                            {

                                string[] TaggedList = AllTaggedUsers[m].Split('@');
                                AllTaggedUsers.Remove(AllTaggedUsers[m]);
                                foreach (string tagged in TaggedList)
                                {
                                    AllTaggedUsers.Add(tagged);
                                }
                            }
                        }
                    }
                    string taggeduserlist = string.Join(",", AllTaggedUsers.ToArray());
                    string noteDescription = node.SelectSingleNode("NoteDescription").InnerText;
                    if (AllTaggedUsers.Count == 0)
                    {
                        noteDescription = noteDescription.Replace("<a", "<a class=\"disabledLink\"");
                        if (!IsDeleted)
                        {
                            if (!IsEdit)
                            {
                                string ModifiedBy = node.SelectSingleNode("ModifiedBy").InnerText;
                                if(!string.IsNullOrEmpty(ModifiedBy))
                                {
                                    objuserhistory.CreatedDate = Convert.ToDateTime(node.SelectSingleNode("ModifiedDate").InnerText).ToString("dd/MM/yyyy hh:mmtt");
                                    objuserhistory.CreateDate = Convert.ToDateTime(node.SelectSingleNode("ModifiedDate").InnerText);
                                    objuserhistory.CreatedBy = node.SelectSingleNode("ModifiedBy").InnerText;
                                }
                                objuserhistory.HistoryMessage = "published a " + Category + " note " + "<span class='notesfunction'><a data-usernoteid=" + noteid + " data-IsWarningNote=" + Convert.ToString(false) + " class=\"fa-solid fa-pencil\" style=\"font-size: 17px; text-decoration:none;\" title=\"Edit\"  href=\"javascript: void(0)\" style=\"cursor: pointer\" onclick=\"EditUserNote(this)\"></a><a data-usernoteid=" + noteid + " data-IsWarningNote=" + Convert.ToString(false) + " style=\"font-size: 18px;margin-left: 5px;text-decoration:none;\" title=\"Unpublish\" class=\"fas fa-minus-square\"   href=\"javascript: void(0)\" style=\"cursor: pointer\" onclick=\"DeleteUserNote(this)\"></a></span> " + "<span style=\"float:right;display:none\">NoteID: " + noteid + "</span>" + noteDescription;
                            }
                            else
                            {
                                objuserhistory.CreatedDate = Convert.ToDateTime(node.SelectSingleNode("ModifiedDate").InnerText).ToString("dd/MM/yyyy hh:mmtt");
                                objuserhistory.CreateDate = Convert.ToDateTime(node.SelectSingleNode("ModifiedDate").InnerText);
                                objuserhistory.CreatedBy = node.SelectSingleNode("ModifiedBy").InnerText;
                                objuserhistory.HistoryMessage = "edited a published " + Category + " note " + "<span class='notesfunction'><a data-usernoteid=" + noteid + " data-IsWarningNote=" + Convert.ToString(false) + " class=\"fa-solid fa-pencil\" style=\"font-size: 17px; text-decoration:none;\" title=\"Edit\"  href=\"javascript: void(0)\" style=\"cursor: pointer\" onclick=\"EditUserNote(this)\"></a><a data-usernoteid=" + noteid + " data-IsWarningNote=" + Convert.ToString(false) + " style=\"font-size: 18px;margin-left: 5px;text-decoration:none;\" title=\"Unpublish\" class=\"fas fa-minus-square\"   href=\"javascript: void(0)\" style=\"cursor: pointer\" onclick=\"DeleteUserNote(this)\"></a></span> " + "<span style =\"float:right;display:none\">NoteID: " + noteid + "</span>" + noteDescription;
                            }
                        }
                        else
                        {
                            objuserhistory.CreatedDate = Convert.ToDateTime(node.SelectSingleNode("ModifiedDate").InnerText).ToString("dd/MM/yyyy hh:mmtt");
                            objuserhistory.CreateDate = Convert.ToDateTime(node.SelectSingleNode("ModifiedDate").InnerText);
                            objuserhistory.CreatedBy = node.SelectSingleNode("ModifiedBy").InnerText;
                            objuserhistory.HistoryMessage = "unpublished " + Category + " note " + "<span class='notesfunction'><a data-usernoteid=" + noteid + " data-IsWarningNote=" + Convert.ToString(false) + " style=\"font-size: 17px;margin-left: -5px;text-decoration:none;\" title=\"Publish\" class=\"fa fa-upload\" href=\"javascript: void(0)\" style=\"cursor: pointer\" onclick=\"PublishUserNote(this)\"></a></span> " + "<span style=\"float:right;display:none\">NoteID: " + noteid + "</span>" + noteDescription;
                        }
                    }
                    else
                    {
                        if (!IsDeleted)
                        {
                            if (!IsEdit)
                            {
                                string ModifiedBy = node.SelectSingleNode("ModifiedBy").InnerText;
                                if (!string.IsNullOrEmpty(ModifiedBy))
                                {
                                    objuserhistory.CreatedDate = Convert.ToDateTime(node.SelectSingleNode("ModifiedDate").InnerText).ToString("dd/MM/yyyy hh:mmtt");
                                    objuserhistory.CreateDate = Convert.ToDateTime(node.SelectSingleNode("ModifiedDate").InnerText);
                                    objuserhistory.CreatedBy = node.SelectSingleNode("ModifiedBy").InnerText;
                                }
                                objuserhistory.HistoryMessage = "published a " + Category + " note " + "<span class='notesfunction'><a data-usernoteid=" + noteid + " data-IsWarningNote=" + Convert.ToString(false) + " class=\"fa-solid fa-pencil\" style=\"font-size: 17px; text-decoration:none;\" title=\"Edit\"  href=\"javascript: void(0)\" style=\"cursor: pointer\" onclick=\"EditUserNote(this)\"></a><a data-usernoteid=" + noteid + " data-IsWarningNote=" + Convert.ToString(false) + " style=\"font-size: 18px;margin-left: 5px;text-decoration:none;\" title=\"Unpublish\" class=\"fas fa-minus-square\"   href=\"javascript: void(0)\" style=\"cursor: pointer\" onclick=\"DeleteUserNote(this)\"></a> </span>" + "<span style =\"float:right;display:none\">NoteID: " + noteid + "</span>" +noteDescription;
                            }
                            else
                            {
                                objuserhistory.CreatedDate = Convert.ToDateTime(node.SelectSingleNode("ModifiedDate").InnerText).ToString("dd/MM/yyyy hh:mmtt");
                                objuserhistory.CreateDate = Convert.ToDateTime(node.SelectSingleNode("ModifiedDate").InnerText);
                                objuserhistory.CreatedBy = node.SelectSingleNode("ModifiedBy").InnerText;
                                objuserhistory.HistoryMessage = "edited a published " + Category + " note " + "<span class='notesfunction'><a data-usernoteid=" + noteid + " data-IsWarningNote=" + Convert.ToString(false) + " class=\"fa-solid fa-pencil\" style=\"font-size: 17px; text-decoration:none;\" title=\"Edit\"  href=\"javascript: void(0)\" style=\"cursor: pointer\" onclick=\"EditUserNote(this)\"></a><a data-usernoteid=" + noteid + " data-IsWarningNote=" + Convert.ToString(false) + " style=\"font-size: 18px;margin-left: 5px;text-decoration:none;\" title=\"Unpublish\" class=\"fas fa-minus-square\"   href=\"javascript: void(0)\" style=\"cursor: pointer\" onclick=\"DeleteUserNote(this)\"></a></span> " + "<span style =\"float:right;display:none\">NoteID: " + noteid + "</span>" + noteDescription;
                            }
                        }
                        else
                        {
                            objuserhistory.CreatedDate = Convert.ToDateTime(node.SelectSingleNode("ModifiedDate").InnerText).ToString("dd/MM/yyyy hh:mmtt");
                            objuserhistory.CreateDate = Convert.ToDateTime(node.SelectSingleNode("ModifiedDate").InnerText);
                            objuserhistory.CreatedBy = node.SelectSingleNode("ModifiedBy").InnerText;
                            objuserhistory.HistoryMessage = "unpublished " + Category + " note " + "<span class='notesfunction'><a data-usernoteid=" + noteid + " data-IsWarningNote=" + Convert.ToString(false) + " style=\"font-size: 17px;margin-left: -5px;text-decoration:none;\" title=\"Publish\" class=\"fa fa-upload\" href=\"javascript: void(0)\" style=\"cursor: pointer\" onclick=\"PublishUserNote(this)\"></a></span> " + "<span style =\"float:right;display:none\">NoteID: " + noteid + "</span>" + noteDescription;
                        }

                    }

                    if (objuserhistory.CategoryID == categoryID|| categoryID==0)
                    {
                        if ((IsDeletedUserNote == 1) || (IsDeletedUserNote != 1 && IsDeletedUserNote == objuserhistory.IsDeleted))
                        {
                            objlstUserHistory.Add(objuserhistory);
                        }
                    }
                }
            }

            if (categoryID == 5||categoryID==0)
            {
                string UserWarningNotesDirectoryPath = Path.Combine(ProjectConfiguration.ProofDocumentsURL, "UserDocuments", "WarningNotes");
                string UserWarningNotesFilePath = Path.Combine(UserWarningNotesDirectoryPath, "WarningNotes.xml");

                if (System.IO.File.Exists(UserWarningNotesFilePath))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(UserWarningNotesFilePath);
                    XmlNodeList Notes = doc.DocumentElement.SelectNodes("/UserNotes/Note");

                    foreach (XmlNode node in Notes)
                    {
                        UserHistory objuserhistory = new UserHistory();
                        string UserId = node.SelectSingleNode("UserID").InnerText;
                        if (UserId == UserID)
                        {
                            objuserhistory.CreatedBy = node.SelectSingleNode("CreatedBy").InnerText;
                            objuserhistory.CreatedDate = Convert.ToDateTime(node.SelectSingleNode("CreatedDate").InnerText).ToString("dd/MM/yyyy hh:mmtt");
                            //objuserhistory.HistoryMessage = "added a Warning Note" + node.SelectSingleNode("NoteDescription").InnerText;
                            objuserhistory.CategoryID = 5;
                            objuserhistory.CreateDate = Convert.ToDateTime(node.SelectSingleNode("CreatedDate").InnerText);
                            bool IsEdit = Convert.ToBoolean(node.SelectSingleNode("IsEdit").InnerText);
                            string TaggedUsers = node.SelectSingleNode("NoteDescription").InnerText;
                            string noteid = node.SelectSingleNode("NoteID").InnerText;
                            bool IsDeleted = Convert.ToBoolean(node.SelectSingleNode("IsDeleted").InnerText);
                            objuserhistory.Id = (!string.IsNullOrEmpty(noteid)) ? Convert.ToInt32(noteid) : 0;
                            if(IsDeleted)
                            {
                                objuserhistory.IsDeleted = 2;
                            }
                            else
                            {
                                objuserhistory.IsDeleted = 3;
                            }
                            HtmlDocument notesdoc = new HtmlDocument();
                            notesdoc.LoadHtml(TaggedUsers);
                            var SelectTaggedUsers = notesdoc.DocumentNode.SelectNodes("//a[contains(@class, 'tagged')]");
                            List<string> AllTaggedUsers = new List<string>();
                            if (SelectTaggedUsers != null)
                            {
                                for (int j = 0; j < SelectTaggedUsers.Count; j++)
                                {
                                    AllTaggedUsers.Add(SelectTaggedUsers[j].InnerHtml);
                                }
                                AllTaggedUsers = AllTaggedUsers.Select(p => !string.IsNullOrEmpty(p) ? p.Substring(1) : p).ToList();
                                for (int m = 0; m < AllTaggedUsers.Count; m++)
                                {
                                    if (AllTaggedUsers[m].Contains('@'))
                                    {

                                        string[] TaggedList = AllTaggedUsers[m].Split('@');
                                        AllTaggedUsers.Remove(AllTaggedUsers[m]);
                                        foreach (string tagged in TaggedList)
                                        {
                                            AllTaggedUsers.Add(tagged);
                                        }
                                    }
                                }
                            }
                            string taggeduserlist = string.Join(",", AllTaggedUsers.ToArray());
                            if (AllTaggedUsers.Count == 0)
                            {
                                if (!IsDeleted)
                                {
                                    if (!IsEdit)
                                    {
                                        string ModifiedBy = node.SelectSingleNode("ModifiedBy").InnerText;
                                        if (!string.IsNullOrEmpty(ModifiedBy))
                                        {
                                            objuserhistory.CreatedDate = Convert.ToDateTime(node.SelectSingleNode("ModifiedDate").InnerText).ToString("dd/MM/yyyy hh:mmtt");
                                            objuserhistory.CreateDate = Convert.ToDateTime(node.SelectSingleNode("ModifiedDate").InnerText);
                                            objuserhistory.CreatedBy = node.SelectSingleNode("ModifiedBy").InnerText;
                                        }
                                        objuserhistory.HistoryMessage = "published a Warning note " + "<span class='notesfunction'><a data-usernoteid=" + noteid + " data-IsWarningNote=" + Convert.ToString(true) + " class=\"fa-solid fa-pencil\" style=\"font-size: 17px; text-decoration:none;\" title=\"Edit\"  href=\"javascript: void(0)\" style=\"cursor: pointer\" onclick=\"EditUserNote(this)\"></a><a data-usernoteid=" + noteid + " data-IsWarningNote=" + Convert.ToString(true) + " style=\"font-size: 18px;margin-left: 5px;text-decoration:none;\" title=\"Unpublish\" class=\"fas fa-minus-square\"  href=\"javascript: void(0)\" style=\"cursor: pointer\" onclick=\"DeleteUserNote(this)\"></a></span> " + "<span style =\"float:right;display:none\">NoteID: " + noteid + "</span>" + node.SelectSingleNode("NoteDescription").InnerText;
                                    }
                                    else
                                    {
                                        objuserhistory.CreatedDate = Convert.ToDateTime(node.SelectSingleNode("ModifiedDate").InnerText).ToString("dd/MM/yyyy hh:mmtt");
                                        objuserhistory.CreateDate = Convert.ToDateTime(node.SelectSingleNode("ModifiedDate").InnerText);
                                        objuserhistory.CreatedBy = node.SelectSingleNode("ModifiedBy").InnerText;
                                        objuserhistory.HistoryMessage = "edited a published Warning note " + "<span class='notesfunction'><a data-usernoteid=" + noteid + " data-IsWarningNote=" + Convert.ToString(true) + " class=\"fa-solid fa-pencil\" style=\"font-size: 17px; text-decoration:none;\" title=\"Edit\"  href=\"javascript: void(0)\" style=\"cursor: pointer\" onclick=\"EditUserNote(this)\"></a><a data-usernoteid=" + noteid + " data-IsWarningNote=" + Convert.ToString(true) + " style=\"font-size: 18px;margin-left: 5px;text-decoration:none;\" title=\"Unpublish\" class=\"fas fa-minus-square\" href=\"javascript: void(0)\" style=\"cursor: pointer\" onclick=\"DeleteUserNote(this)\"></a></span> " + "<span style =\"float:right;display:none\">NoteID: " + noteid + "</span>" + node.SelectSingleNode("NoteDescription").InnerText;
                                    }
                                }
                                else
                                {
                                    objuserhistory.CreatedDate = Convert.ToDateTime(node.SelectSingleNode("ModifiedDate").InnerText).ToString("dd/MM/yyyy hh:mmtt");
                                    objuserhistory.CreateDate = Convert.ToDateTime(node.SelectSingleNode("ModifiedDate").InnerText);
                                    objuserhistory.CreatedBy = node.SelectSingleNode("ModifiedBy").InnerText;
                                    objuserhistory.HistoryMessage = "unpublished a Warning note " + "<span class='notesfunction'><a data-usernoteid=" + noteid + " data-IsWarningNote=" + Convert.ToString(true) + " style=\"font-size: 17px;margin-left: -5px;text-decoration:none;\" title=\"Publish\" class=\"fa fa-upload\" href=\"javascript: void(0)\" style=\"cursor: pointer\" onclick=\"PublishUserNote(this)\"></a></span> " + "<span style =\"float:right;display:none\">NoteID: " + noteid + "</span>" + node.SelectSingleNode("NoteDescription").InnerText;
                                }

                            }
                            else
                            {
                                if (!IsDeleted)
                                {
                                    if (!IsEdit)
                                    {
                                        string ModifiedBy = node.SelectSingleNode("ModifiedBy").InnerText;
                                        if (!string.IsNullOrEmpty(ModifiedBy))
                                        {
                                            objuserhistory.CreatedDate = Convert.ToDateTime(node.SelectSingleNode("ModifiedDate").InnerText).ToString("dd/MM/yyyy hh:mmtt");
                                            objuserhistory.CreateDate = Convert.ToDateTime(node.SelectSingleNode("ModifiedDate").InnerText);
                                            objuserhistory.CreatedBy = node.SelectSingleNode("ModifiedBy").InnerText;
                                        }
                                        objuserhistory.HistoryMessage = "published a Warning note " + "<span class='notesfunction'><a data-usernoteid=" + noteid + " data-IsWarningNote=" + Convert.ToString(true) + " class=\"fa-solid fa-pencil\" style=\"font-size: 17px; text-decoration:none;\" title=\"Edit\"  href=\"javascript: void(0)\" style=\"cursor: pointer\" onclick=\"EditUserNote(this)\"></a><a data-usernoteid=" + noteid + " data-IsWarningNote=" + Convert.ToString(true) + " style=\"font-size: 18px;margin-left: 5px;text-decoration:none;\" title=\"Unpublish\" class=\"fas fa-minus-square\"   href=\"javascript: void(0)\" style=\"cursor: pointer\" onclick=\"DeleteUserNote(this)\"></a></span> " + "<span style =\"float:right;display:none\">NoteID: " + noteid + "</span>" + node.SelectSingleNode("NoteDescription").InnerText;
                                    }
                                    else
                                    {
                                        objuserhistory.CreatedDate = Convert.ToDateTime(node.SelectSingleNode("ModifiedDate").InnerText).ToString("dd/MM/yyyy hh:mmtt");
                                        objuserhistory.CreateDate = Convert.ToDateTime(node.SelectSingleNode("ModifiedDate").InnerText);
                                        objuserhistory.CreatedBy = node.SelectSingleNode("ModifiedBy").InnerText;
                                        objuserhistory.HistoryMessage = "edited a published Warning note " + "<span class='notesfunction'><a data-usernoteid=" + noteid + " data-IsWarningNote=" + Convert.ToString(true) + " class=\"fa-solid fa-pencil\" style=\"font-size: 17px; text-decoration:none;\" title=\"Edit\"  href=\"javascript: void(0)\" style=\"cursor: pointer\" onclick=\"EditUserNote(this)\"></a><a data-usernoteid=" + noteid + " data-IsWarningNote=" + Convert.ToString(true) + " style=\"font-size: 18px;margin-left: 5px;text-decoration:none;\" title=\"Unpublish\" class=\"fas fa-minus-square\"  href=\"javascript: void(0)\" style=\"cursor: pointer\" onclick=\"DeleteUserNote(this)\"></a></span> " + "<span style =\"float:right;display:none\">NoteID: " + noteid + "</span>" + node.SelectSingleNode("NoteDescription").InnerText;
                                    }
                                }
                                else
                                {
                                    objuserhistory.CreatedDate = Convert.ToDateTime(node.SelectSingleNode("ModifiedDate").InnerText).ToString("dd/MM/yyyy hh:mmtt");
                                    objuserhistory.CreateDate = Convert.ToDateTime(node.SelectSingleNode("ModifiedDate").InnerText);
                                    objuserhistory.CreatedBy = node.SelectSingleNode("ModifiedBy").InnerText;
                                    objuserhistory.HistoryMessage = "unpublished a Warning note " + "<span class='notesfunction'><a data-usernoteid=" + noteid + " data-IsWarningNote=" + Convert.ToString(true) + " style=\"font-size: 17px;margin-left: -5px;text-decoration:none;\" title=\"Publish\" class=\"fa fa-upload\"  href=\"javascript: void(0)\" style=\"cursor: pointer\" onclick=\"PublishUserNote(this)\"></a></span> " + "<span style =\"float:right;display:none\">NoteID: " + noteid + "</span>" + node.SelectSingleNode("NoteDescription").InnerText;
                                }

                            }
                            if (IsDeletedUserNote == 1 || (IsDeletedUserNote != 1 && objuserhistory.IsDeleted == IsDeletedUserNote))
                            {
                                objlstUserHistory.Add(objuserhistory);
                            }
                            
                        }
                    }
                }
            }
            List<UserHistory> importantHistorynotes = new List<UserHistory>();
            List<UserHistory> nonImportantHistorynotes = new List<UserHistory>();
            nonImportantHistorynotes = objlstUserHistory.Where(m => m.IsImportant == false).ToList();
            importantHistorynotes = objlstUserHistory.Where(m => m.IsImportant == true).ToList();
            //objlstUserHistory = objlstUserHistory.OrderByDescending(m => m.CreateDate).ToList();

            if (IsImportant)
            {
                objlstUserHistory = importantHistorynotes.OrderByDescending(m => m.CreateDate).ToList();
            }
            else
            {
                nonImportantHistorynotes = nonImportantHistorynotes.OrderByDescending(m => m.CreateDate).ToList();
                objlstUserHistory = importantHistorynotes.Concat(nonImportantHistorynotes).ToList<UserHistory>();
            }

            return objlstUserHistory;

        }
        public ActionResult ShowFilteredUserHistory(int UserID, int categoryID,bool IsImportant, int IsDeletedUserNote)
        {

            return this.PartialView("~/Views/User/_UserHistoryList.cshtml", GetUserHistory(UserID.ToString(), categoryID, IsDeletedUserNote,IsImportant));
        }

        public JsonResult EditUserNote(string NoteID, int UserID, bool IsWarningNote)
        {
            string fullDirectoryPath = "";
            string fullFilePath = "";
            if (!IsWarningNote)
            {
                fullDirectoryPath = Path.Combine(ProjectConfiguration.ProofDocumentsURL, "UserDocuments", UserID.ToString(), "UserNotes");
                fullFilePath = Path.Combine(fullDirectoryPath, "Notes_" + UserID.ToString() + ".xml");
            }
            else
            {
                fullDirectoryPath = Path.Combine(ProjectConfiguration.ProofDocumentsURL, "UserDocuments", "WarningNotes");
                fullFilePath = Path.Combine(fullDirectoryPath, "WarningNotes.xml");
            }
            try
            {
                if (System.IO.File.Exists(fullFilePath))
                {
                    XDocument doc = XDocument.Load(fullFilePath);
                    var Note = doc.Descendants("Note").Single(r => (string)r.Element("NoteID") == NoteID);
                    if (Note != null)
                    {
                        if (Note.Element("IsDeleted").Value == "True")
                        {
                            return Json(new { status = false, message = "User Note has been deleted." }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            var notesdescription = Note.Element("NoteDescription").Value;
                            var notestype = Note.Element("NotesType").Value;
                            bool IsImportantNote = Convert.ToBoolean(Note.Element("IsImportant").Value);
                            int NotesTypeValue = 0;
                            if(notestype == "Warning")
                            {
                                NotesTypeValue = 5;
                            }
                            else
                            {
                                NotesTypeValue = !string.IsNullOrEmpty(notestype) ? Convert.ToInt32((SystemEnums.NotesType)Enum.Parse(typeof(SystemEnums.NotesType), notestype).GetHashCode()) : 0;
                            }
                            return Json(new { status = true, Notes = notesdescription, NotesType = NotesTypeValue, IsImportantNote = IsImportantNote }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { status = false, message = "User Note does not exist." }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { status = false, message = "User Note does not exist." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { status = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DeleteUserNote(string NoteID, int UserID, bool IsWarningNote)
        {
            string fullDirectoryPath = "";
            string fullFilePath = "";
            if (!IsWarningNote)
            {
                fullDirectoryPath = Path.Combine(ProjectConfiguration.ProofDocumentsURL, "UserDocuments", UserID.ToString(), "UserNotes");
                fullFilePath = Path.Combine(fullDirectoryPath, "Notes_" + UserID.ToString() + ".xml");
            }
            else
            {
                fullDirectoryPath = Path.Combine(ProjectConfiguration.ProofDocumentsURL, "UserDocuments","WarningNotes");
                fullFilePath = Path.Combine(fullDirectoryPath, "WarningNotes.xml");
            }
            try
            {
                if (System.IO.File.Exists(fullFilePath))
                {
                    XDocument doc = XDocument.Load(fullFilePath);
                    var Note = doc.Descendants("Note").Single(r => (string)r.Element("NoteID") == NoteID);
                    if (Note.Element("IsDeleted").Value == "True")
                    {
                        return Json(new { status = false, message = "User Note has been unpublished." }, JsonRequestBehavior.AllowGet);
                    }
                    if(Note.Element("IsEdit").Value == "False")
                    {
                        string Notes = Note.Element("NoteDescription").Value;
                        HtmlDocument notesdoc = new HtmlDocument();
                        notesdoc.LoadHtml(Notes);
                        var SelectTaggedUsers = notesdoc.DocumentNode.SelectNodes("//a[contains(@class, 'tagged')]");
                        //var tagged3 = doc1.DocumentNode.Descendants("a").Where(d => d.Attributes["class"].Value.Contains("tagged"));
                        //IEnumerable<HtmlNode> tagged2 =doc.DocumentNode.Descendants(0).Where(n => n.HasClass("class-name")); 
                        List<string> AllTaggedUsers = new List<string>();
                        if (SelectTaggedUsers != null)
                        {
                            for (int j = 0; j < SelectTaggedUsers.Count; j++)
                            {
                                AllTaggedUsers.Add(SelectTaggedUsers[j].InnerHtml);
                            }
                            AllTaggedUsers = AllTaggedUsers.Select(p => !string.IsNullOrEmpty(p) ? p.Substring(1) : p).ToList();
                            for (int m = 0; m < AllTaggedUsers.Count; m++)
                            {
                                if (AllTaggedUsers[m].Contains('@'))
                                {

                                    string[] TaggedList = AllTaggedUsers[m].Split('@');
                                    AllTaggedUsers.Remove(AllTaggedUsers[m]);
                                    foreach (string tagged in TaggedList)
                                    {
                                        AllTaggedUsers.Add(tagged);
                                    }
                                }
                            }
                        }
                        string taggeduserlist = string.Join(",", AllTaggedUsers.ToArray());
                        string UserHistoryMessageAdd = "";
                        string UserHistoryMessageEdit = "";
                        string JobHistoryAddedBy = Note.Element("CreatedBy").Value;
                        if (AllTaggedUsers.Count == 0)
                        {
                            UserHistoryMessageAdd = "published a " + Note.Element("NotesType").Value + " note " + "<span style=\"float:right\">NoteID: " + Note.Element("NoteID").Value + "</span>" + Note.Element("NoteDescription").Value;
                        }
                        else
                        {
                            UserHistoryMessageAdd = "published a " + Note.Element("NotesType").Value + " note " + "<span style=\"float:right\">NoteID: " + Note.Element("NoteID").Value + "</span>" + Note.Element("NoteDescription").Value;
                        }


                        UserHistoryMessageEdit = UserHistoryMessageAdd.Replace("added", "edited");
                        //descriptionedit = "Note edited by <b class=\"blue-title\">" + ProjectSession.LoggedInName + "</b> on <span class=\"blue-title\">" + DateTime.Now.ToString("dd/MM/yyyy hh:mmtt") + "</span><p><span class=\"blue-title\">From: </span></p>" + descriptionadd + "<p><span class=\"blue-title\">To: </span>" + notes;
                        string CreatedDate = Note.Element("CreatedDate").Value;
                        if(IsWarningNote)
                        {
                            //Common.SaveUserHistorytoXML(UserID, UserHistoryMessageAdd, JobHistoryAddedBy, CreatedDate, 5);
                        }
                        else
                        {
                            string notestype = Note.Element("NotesType").Value;
                            int NotesTypeValue = !string.IsNullOrEmpty(notestype) ? Convert.ToInt32((SystemEnums.NotesType)Enum.Parse(typeof(SystemEnums.NotesType), notestype).GetHashCode()) : 0;
                            //Common.SaveUserHistorytoXML(UserID, UserHistoryMessageAdd, JobHistoryAddedBy, CreatedDate, NotesTypeValue);
                        }
                    }

                    Note.Element("IsDeleted").Value = Convert.ToString(true);
                    Note.Element("ModifiedBy").Value = ProjectSession.LoggedInName;
                    Note.Element("ModifiedDate").Value = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");
                    doc.Save(fullFilePath);
                    string NotesType = Note.Element("NotesType").Value;
                    string UserHistoryMessage = "deleted a " + Note.Element("NotesType").Value + " Note" + "< span style =\"float:right\">NoteID: " + NoteID + "</span>" + Note.Element("NoteDescription").Value;
                    //Common.SaveUserHistorytoXML(UserID, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                    string UserHistoryDirectoryPath = Path.Combine(ProjectConfiguration.ProofDocumentsURL, "UserDocuments", UserID.ToString(), "UserHistory");
                    string UserHistoryFilePath = Path.Combine(UserHistoryDirectoryPath, "UserHistory_" + UserID.ToString() + ".xml");
                    if (System.IO.File.Exists(UserHistoryFilePath))
                    {
                        XmlDocument historydoc = new XmlDocument();
                        historydoc.Load(UserHistoryFilePath);
                        //XmlNodeList nodeList = historydoc.SelectNodes("/UserHistory/History[NoteID=Noteid]");
                        XDocument doc1 = XDocument.Load(UserHistoryFilePath);
                        //var Notes = doc.Descendants("History").Select(r => (string)r.Element("NoteID") == Noteid);
                        var Notes1 = doc1.Descendants("History").Where(r => (string)r.Element("NoteID") == NoteID && (string)r.Element("IsWarning") == Convert.ToString(IsWarningNote));
                        foreach (XElement el in Notes1)
                        {
                            el.Element("IsDeleted").Value = Convert.ToString(true);
                        }
                        doc1.Save(UserHistoryFilePath);
                    }

                    return Json(new { status = true, message = "User Note unpublished successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = false, message = "User Note does not exist." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { status = false, message = "Failed to unpublish User Note" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult PublishUserNote(string NoteID, int UserID, bool IsWarningNote)
        {
            string fullDirectoryPath = "";
            string fullFilePath = "";
            if (!IsWarningNote)
            {
                fullDirectoryPath = Path.Combine(ProjectConfiguration.ProofDocumentsURL, "UserDocuments", UserID.ToString(), "UserNotes");
                fullFilePath = Path.Combine(fullDirectoryPath, "Notes_" + UserID.ToString() + ".xml");
            }
            else
            {
                fullDirectoryPath = Path.Combine(ProjectConfiguration.ProofDocumentsURL, "UserDocuments", "WarningNotes");
                fullFilePath = Path.Combine(fullDirectoryPath, "WarningNotes.xml");
            }

            try
            {
                if(System.IO.File.Exists(fullFilePath))
                {
                    XDocument doc = XDocument.Load(fullFilePath);
                    var Note = doc.Descendants("Note").Single(r => (string)r.Element("NoteID") == NoteID);
                    if (Note.Element("IsDeleted").Value == "False")
                    {
                        return Json(new { status = false, message = "User Note has been already published." }, JsonRequestBehavior.AllowGet);
                    }
                    Note.Element("IsDeleted").Value = Convert.ToString(false);
                    Note.Element("ModifiedBy").Value = ProjectSession.LoggedInName;
                    Note.Element("ModifiedDate").Value = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");
                    doc.Save(fullFilePath);
                    string UserHistoryDirectoryPath = Path.Combine(ProjectConfiguration.ProofDocumentsURL, "UserDocuments", UserID.ToString(), "UserHistory");
                    string UserHistoryFilePath = Path.Combine(UserHistoryDirectoryPath, "UserHistory_" + UserID.ToString() + ".xml");
                    if (System.IO.File.Exists(UserHistoryFilePath))
                    {
                        XmlDocument historydoc = new XmlDocument();
                        historydoc.Load(UserHistoryFilePath);
                        //XmlNodeList nodeList = historydoc.SelectNodes("/UserHistory/History[NoteID=Noteid]");
                        XDocument doc1 = XDocument.Load(UserHistoryFilePath);
                        //var Notes = doc.Descendants("History").Select(r => (string)r.Element("NoteID") == Noteid);
                        var Notes1 = doc1.Descendants("History").Where(r => (string)r.Element("NoteID") == NoteID && (string)r.Element("IsWarning") == Convert.ToString(IsWarningNote));
                        foreach (XElement el in Notes1)
                        {
                            el.Element("IsDeleted").Value = Convert.ToString(false);
                        }
                        doc1.Save(UserHistoryFilePath);
                    }
                    return Json(new { status = true, message = "User Note published successfully." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = false, message = "User Note does not exist." }, JsonRequestBehavior.AllowGet);
                }
                
            }
            catch(Exception e)
            {
                return Json(new { status = false, message = "User Note does not exist." }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        /// <summary>
        /// Save SAAS User notes
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="Notes"></param>
        /// <returns></returns>
        public JsonResult SaveSAASUserNote(int UserID, string Notes)
        {
            try
            {
                int SAASUserID = UserID;
                string Note = Notes;
                int CreatedBy = ProjectSession.LoggedInUserId;
                _userBAL.SaveSAASUserNote(UserID, Notes, CreatedBy, DateTime.Now);
                return Json(new { status = true, message = "Note Saved Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception e)
            {
                return Json(new { status = false, message = "Exception on Saving Note" }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// Get SAAS user details
        /// </summary>
        /// <param name="SAASUserId"></param>
        /// <param name="UserID"></param>
        /// <param name="UserTypeID"></param>
        /// <param name="GB_RACode"></param>
        /// <returns></returns>
        public JsonResult GetSAASUserDetail(int SAASUserId, int UserID, int UserTypeID, string GB_RACode)
        {
            //var dsUsers = _userBAL.GetUserById(SAASUserId);
            FormBot.Entity.User userView = new Entity.User();
            DataSet ds = _userBAL.GetSAASUserDetailsbyInvoicerID(SAASUserId);
            userView = GetUserEntityForSASS(ds.Tables[0]);

            if (userView.IsPostalAddress == true)
            {
                userView.AddressID = 2;
            }
            else
            {
                userView.AddressID = 1;
            }
            return Json(new { status = true, User = userView }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Delete contract file from db
        /// </summary>
        /// <param name="InvoicerID"></param>
        /// <param name="DocumentPath"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult DeleteContractFileByID(int InvoicerID,string DocumentPath)
        {
            try
            {
                string sourcePath = Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + "UserDocuments" + "\\" + InvoicerID + "\\" + "SAAS" + "\\" + DocumentPath);
                if(System.IO.File.Exists(sourcePath))
                {
                    System.IO.File.Delete(sourcePath);
                }
                _userBAL.DeleteContractFile(InvoicerID, DocumentPath);
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception e)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// Delete Contract file from folder
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="FolderName"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult DeleteContractFileFromFolder(string fileName, string FolderName)
        {
            var filePath = Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + "UserDocuments" + "\\" + FolderName, fileName);
            if(System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            this.ShowMessage(SystemEnums.MessageType.Success, "File has been deleted successfully.", false);
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Return Unique SAAS Unique contactid
        /// </summary>
        /// <returns></returns>
        //public string GetUniqueContactIDforSAAS()
        //{
        //    int SAASUserCount = _userBAL.GetSAASUserCount();
        //    string UniqueContactID = "SAAS_" + SAASUserCount.ToString();
        //    return UniqueContactID;
        //}
        public JsonResult GetUserNotesType()
        {
            List<SelectListItem> items = (Enum.GetValues(typeof(SystemEnums.NotesType)).Cast<int>().Select(e => new SelectListItem() { Text = Enum.GetName(typeof(SystemEnums.NotesType), e), Value = e.ToString() })).ToList();
            items = items.Where(x => x.Value == "3" ||x.Value=="0").ToList();
            return Json(items, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetAllUserList(int userId,int UserTypeId,int jobid)
        {
            List<SelectListItem> items = _createJobBAL.GetAllUserList(userId, UserTypeId,jobid).Select(a => new SelectListItem { Text = a.name, Value = a.name.ToString() }).OrderBy(x => x.Text).ToList();
            return Json(items, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// get entityname from companyABN
        /// </summary>
        /// <param name="companyABN"></param>
        /// <returns></returns>
        public string strGetEntityName(string companyABN)
        {
            string entityName = string.Empty;
            string abnURL = ProjectConfiguration.GetCompanyABNSearchLink + companyABN;
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
                    HtmlAgilityPack.HtmlDocument resultat = new HtmlAgilityPack.HtmlDocument();
                    resultat.LoadHtml(strResult);

                    HtmlNode table = resultat.DocumentNode.SelectSingleNode("//table[1]");
                    if (table != null)
                    {
                        foreach (var cell in table.SelectNodes(".//tr/th"))
                        {
                            string someVariable = cell.InnerText;
                            if (cell.InnerText.ToLower() == "entity name:")
                            {
                                var td = cell.ParentNode.SelectNodes("./td");
                                string tdValue = td[0].InnerText;
                                entityName = tdValue.Replace("\r\n", "").Trim();
                                if (entityName.Length > 0)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.Log.WriteError(ex,"exception in strGetEntityname companyABN = " + companyABN + " " + ex.Message.ToString());
            }
            return entityName;
        }

        /// <summary>
        /// Get invoicer list
        /// </summary>
        /// <returns>JsonResult</returns>
        [HttpGet]
        public JsonResult GetInvoicerDetailsList()
        {
            try
            {
                List<FormBot.Entity.User> lstInvoicerDetails = new List<FormBot.Entity.User>();
                lstInvoicerDetails = _userBAL.GetInvoicerDetailsList();

                return Json(lstInvoicerDetails, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                //Response.Write(e.Message);
                return Json(new { status = false, error = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public int GetNotificationCount()
        {
            return _userBAL.GetNotificationCount(ProjectSession.LoggedInUserId);
        }

        public PartialViewResult GetNotificationList()
        {
            List<NotificationViewModel> notificationViewModels = _userBAL.GetNotificationList(ProjectSession.LoggedInUserId);

            return PartialView("_NotificationItem", notificationViewModels);
        }

        public void RemoveNotification(string notificationIds)
        {
            _userBAL.RemoveNotification(notificationIds, ProjectSession.LoggedInUserId);
        }
    }

    public class Rootobject
    {
        public string Name { get; set; }
    }
   


}
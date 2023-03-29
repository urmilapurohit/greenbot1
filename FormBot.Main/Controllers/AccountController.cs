using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using FormBot.Main.Models;
using FormBot.BAL.Service;
using Microsoft.AspNet.Identity.EntityFramework;
using FormBot.Helper;
using System.Web.Security;
using System.Collections.Generic;
using FormBot.Entity;
using FormBot.BAL;
using System.Linq;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.IO;
using FormBot.Entity.Email;
using FormBot.Email;
using System.Data;
using Xero.Api.Infrastructure.Interfaces;
using Xero.Api.Infrastructure.OAuth;
using FormBot.Main.Stores;
using Xero.Api.Example.Applications.Public;
using Xero.Api.Infrastructure.Exceptions;
using FormBot.Main.Infrastructure;
using System.Security.Principal;
using FormBot.Main.App_Start;
using System.Configuration;
using System.Net;
using System.Text;
using FormBot.Main.Helpers;

namespace FormBot.Main.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        #region Properties
        private readonly IUserBAL _user;
        private readonly IResellerBAL _reseller;
        private readonly ILoginBAL _login;
        private readonly IEmailBAL _emailBAL;
        private readonly ILogger _log;
        public UserManager<ApplicationUser> UserManager { get; private set; }
        private readonly ISettingsBAL _settingsBAL;
        private readonly IUserTypeBAL _userTypeService;
        #endregion

        #region Constructor
        public AccountController(IUserBAL user, ILoginBAL login, IResellerBAL reseller, IEmailBAL emailBAL, ISettingsBAL settingsBAL, IUserTypeBAL userTypeService, ILogger log)
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())), user, login, reseller, emailBAL, settingsBAL, userTypeService, log)
        {
        }

        public AccountController(UserManager<ApplicationUser> userManager, IUserBAL user, ILoginBAL login, IResellerBAL reseller, IEmailBAL emailBAL, ISettingsBAL settingsBAL, IUserTypeBAL userTypeService, ILogger log)
        {
            UserManager = userManager;
            this._user = user;
            this._login = login;
            this._reseller = reseller;
            this._emailBAL = emailBAL;
            this._settingsBAL = settingsBAL;
            this._userTypeService = userTypeService;
            this._log = log;
        }
        #endregion
        public ActionResult Test()
        {
            var str = CheckApp.IsStart;
            //CheckApp.IsStart = "TRUE : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return Content(str.ToString());
        }

        #region Events

        /// <summary>
        /// Login Action
        /// </summary>
        /// <param name="returnUrl">Return Url</param>
        /// <param name="id">Id parameter</param>
        /// <returns>Action Result</returns>
        [AllowAnonymous]
        public ActionResult Login(string returnUrl, string id = null)
        {
            if (User.Identity.IsAuthenticated && (ProjectSession.LoggedInUserId != 0))
            {
                /*AuthenticationManager.SignOut();
                //FormsAuthentication.SignOut();
                //Session.Abandon();
                //return RedirectToAction("Login");*/
                UserManager.UserValidator = new UserValidator<ApplicationUser>(UserManager) { AllowOnlyAlphanumericUserNames = false };
                var user = UserManager.FindById(User.Identity.GetUserId());

                //cookie.HttpOnly = true;
                //Response.Cookies.Add(cookie);

                LoginViewModel model = new LoginViewModel();
                ResellerDetailView resellerDetail = new ResellerDetailView();
                if (user != null)
                {
                    var userDetail = _user.GetUserByAspnetUserId(user.Id);

                    if (!userDetail.IsActive)
                    {
                        this.ShowMessage(SystemEnums.MessageType.Error, "Your account is currently inactive.");
                        Logout(returnUrl);
                        if (!string.IsNullOrEmpty(id) && id.ToLower() == ProjectConfiguration.CompanyNameForNewLoginScreen.ToLower())
                        {
                            return View("NewLogin", model);
                        }
                        return View("Login", model);
                    }

                    ProjectSession.LoggedInUserId = userDetail.UserId;
                    ProjectSession.LoggedInName = userDetail.FirstName + " " + userDetail.LastName;
                    ProjectSession.RoleId = userDetail.RoleID;
                    ProjectSession.UserTypeId = userDetail.UserTypeID;
                    ProjectSession.IsNewViewer = userDetail.IsNewViewer;
                    ProjectSession.IsTabularView = userDetail.IsTabularView;
                    ProjectSession.IsSSCReseller = userDetail.IsSSCReseller;
                    if (userDetail.ResellerID != null && userDetail.ResellerID > 0)
                    {
                        ProjectSession.ResellerId = (int)userDetail.ResellerID;
                    }
                    if (userDetail.SolarCompanyId != null && userDetail.SolarCompanyId > 0)
                        ProjectSession.SolarCompanyId = (int)userDetail.SolarCompanyId;

                    ProjectSession.IsSubContractor = userDetail.IsSubContractor;

                    ////if (ProjectSession.LoginCompanyName == null)
                    ////{
                    ////    ProjectSession.LoginCompanyName = id;
                    ////}

                    ProjectSession.LoginCompanyName = id;
                    object resellerDeleted = _login.CheckResellerDelete(ProjectSession.LoginCompanyName);
                    if (resellerDeleted.Equals(false))
                    {
                        var dsReseller = _login.GetResellerThemeLogo(ProjectSession.LoginCompanyName);
                        if (dsReseller != null && dsReseller.Tables[0].Rows.Count > 0)
                        {
                            List<ResellerDetailView> resellerDetail1 = DBClient.DataTableToList<ResellerDetailView>(dsReseller.Tables[0]);
                            ResellerDetailView resellerDetailView = resellerDetail1.FirstOrDefault();
                            ProjectSession.Logo = resellerDetailView.Logo;
                            ProjectSession.Theme = ((SystemEnums.Theme)resellerDetailView.Theme).ToString();
                        }
                        else
                        {
                            ProjectSession.Theme = "green";
                            ProjectSession.Logo = "Images/logo.png";
                        }
                    }
                    else
                    {
                        ProjectSession.Theme = "green";
                        ProjectSession.Logo = "Images/logo.png";
                    }

                    ////Email information store
                    FormBot.Entity.Email.EmailSignup emailModel = new Entity.Email.EmailSignup();
                    string xmlString = string.Empty;
                    DataSet lstEmail = _user.LoginUserEmailDetails(ProjectSession.LoggedInUserId);
                    if (lstEmail.Tables[0].Rows.Count > 0)
                    {
                        Session[FormBot.Email.Constants.sessionAccount] = null;
                        DataRow dr = (DataRow)lstEmail.Tables[0].Rows[0];

                        emailModel.Login = dr["email"].ToString();
                        emailModel.ConfigurationEmail = dr["email"].ToString();
                        emailModel.ConfigurationPassword = Utils.DecodePassword(Convert.ToString(dr["mail_inc_pass"]));
                        emailModel.IncomingMail = dr["mail_inc_host"].ToString();
                        emailModel.IncomingMailPort = Convert.ToInt32(dr["mail_inc_port"]);
                        emailModel.Login = dr["email"].ToString();
                        emailModel.OutgoingMail = dr["mail_out_host"].ToString();
                        emailModel.OutgoingMailPort = Convert.ToInt32(dr["mail_out_port"]);

                        xmlString = "<?xml version='1.0' encoding='UTF-8'?><webmail><param name='action' value='login' /><param name='request' value='' /><param name='email'><![CDATA[" + emailModel.ConfigurationEmail
                            + "]]></param><param name='mail_inc_login'><![CDATA[" + emailModel.Login + "]]></param><param name='mail_inc_pass'><![CDATA[" + emailModel.ConfigurationPassword + "]]></param><param name='mail_inc_host'><![CDATA[" + emailModel.IncomingMail
                            + "]]></param><param name='mail_inc_port' value='" + emailModel.IncomingMailPort + "' /><param name='mail_protocol' value='0' /><param name='mail_out_host'><![CDATA[" + emailModel.OutgoingMail
                            + "]]></param><param name='mail_out_port' value='" + emailModel.OutgoingMailPort + "' /><param name='mail_out_auth' value='1' /><param name='sign_me' value='0' /><param name='language' /><param name='advanced_login' value='1' /></webmail>";
                        CheckMail checkMail = new CheckMail();
                        var result = checkMail.GetMessages(xmlString);
                    }

                    if (ProjectSession.UserTypeId == (int)SystemEnums.UserType.SolarElectricians || ProjectSession.UserTypeId == (int)SystemEnums.UserType.SolarCompanyAdmin)
                    {
                        var status = _login.CheckLoggedInUserStatus(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId);
                        var userStatus = !string.IsNullOrEmpty(status) ? Convert.ToInt32(status) : 0;
                        if (userStatus == 0 || userStatus != (int)SystemEnums.UserStatus.Approved)
                        {
                            this.ShowMessage(SystemEnums.MessageType.Error, "Your account is currently inactive.");
                            Logout(returnUrl);
                            if (!string.IsNullOrEmpty(id) && id.ToLower() == ProjectConfiguration.CompanyNameForNewLoginScreen.ToLower())
                            {
                                return View("NewLogin", model);
                            }
                            return View("Login", model);
                        }
                    }

                    if (ProjectSession.RoleId > 0)
                    {
                        _login.UpdateLatLoginDate(ProjectSession.LoggedInUserId);

                        //if (userDetail.IsFirstLogin)
                        //    return RedirectToAction("MyProfile", "User");

                        if (ProjectSession.UserTypeId == (int)SystemEnums.UserType.SolarElectricians && !string.IsNullOrWhiteSpace(returnUrl))
                            return Redirect(ProjectSession.LoginLink + "User/Profile/?flg=DocUpload#horizontalTab3");
                        else if (ProjectSession.UserTypeId == (int)SystemEnums.UserType.FormBotSuperAdmin && !string.IsNullOrWhiteSpace(returnUrl))
                            return Redirect(ProjectSession.LoginLink + returnUrl + "#horizontalTab3?flg=DocVerify");

                        var dsMenuActions = _login.GetMenuActionByRoleId(ProjectSession.RoleId);
                        if (dsMenuActions != null && dsMenuActions.Tables[0].Rows.Count > 0)
                        {
                            List<MenuActionView> MenuActionList = DBClient.DataTableToList<MenuActionView>(dsMenuActions.Tables[0]);
                            MenuActionView menuActionView = MenuActionList.FirstOrDefault();
                            if (menuActionView.Controller.ToLower().Contains("dashboard"))
                            {
                                HttpContext.Response.Cookies.Set(new HttpCookie("menuname") { Value = "Home" });
                            }
                            else
                            {
                                HttpContext.Response.Cookies.Set(new HttpCookie("menuname") { Value = "createnew" });
                            }
                            //if (menuActionView.Controller.ToLower() == "email")
                            //    return RedirectToAction(menuActionView.Action, menuActionView.Controller, new { area = "Email" });
                            //else
                            //{
                            //if user role does not have access for dashboard then display job listing page 
                            if (!menuActionView.Controller.ToLower().Contains("dashboard"))
                            {
                                MenuActionView _menulist = MenuActionList.FirstOrDefault(x => x.Action.ToLower().Equals("index") && x.Controller.ToLower().Equals("job") && x.MenuId.ToString() == SystemEnums.MenuId.JobView.GetHashCode().ToString());
                                if (_menulist != null && _menulist.Action.ToLower().Equals("index") && _menulist.Controller.ToLower().Equals("job") && _menulist.MenuId.ToString() == SystemEnums.MenuId.JobView.GetHashCode().ToString())
                                {
                                    return RedirectToAction(_menulist.Action, _menulist.Controller, new { area = "" });
                                }
                                else if (menuActionView.Controller.ToLower() == "email")
                                    return RedirectToAction(menuActionView.Action, menuActionView.Controller, new { area = "Email" });
                                else
                                    return RedirectToAction(menuActionView.Action, menuActionView.Controller, new { area = "" });

                            }
                            else
                                return RedirectToAction(menuActionView.Action, menuActionView.Controller, new { area = "" });
                            // }


                        }
                        else
                        {
                            this.ShowMessage(SystemEnums.MessageType.Error, "You don't have system access.");
                            if (!string.IsNullOrEmpty(id) && id.ToLower() == ProjectConfiguration.CompanyNameForNewLoginScreen.ToLower())
                            {
                                return View("NewLogin", model);
                            }
                            return View("Login", model);
                        }
                    }
                    else
                    {
                        return RedirectToLocal(returnUrl);
                    }
                }
                if (!string.IsNullOrEmpty(id) && id.ToLower() == ProjectConfiguration.CompanyNameForNewLoginScreen.ToLower())
                {
                    return View("NewLogin", model);
                }
                return View(model);
            }
            else
            {
                LoginViewModel loginView = new LoginViewModel();
                if (!string.IsNullOrEmpty(id))
                {
                    loginView.CompanyName = id;
                    ProjectSession.LoginCompanyName = loginView.CompanyName;

                    //HttpCookie objCookie = new HttpCookie("LoginCompanyName");
                    //objCookie["LoginCompanyName"] = ProjectSession.LoginCompanyName;
                    //objCookie.Expires = DateTime.MaxValue;
                    //Response.Cookies.Add(objCookie);

                    object resellerDeleted = _login.CheckResellerDelete(loginView.CompanyName);
                    if (resellerDeleted.Equals(false))
                    {
                        var dsReseller = _login.GetResellerThemeLogo(loginView.CompanyName);
                        if (dsReseller != null && dsReseller.Tables[0].Rows.Count > 0)
                        {
                            List<ResellerDetailView> resellerDetail = DBClient.DataTableToList<ResellerDetailView>(dsReseller.Tables[0]);
                            ResellerDetailView resellerDetailView = resellerDetail.FirstOrDefault();
                            loginView.Logo = resellerDetailView.Logo;
                            loginView.Theme = ((SystemEnums.Theme)resellerDetailView.Theme).ToString();
                            ProjectSession.Theme = loginView.Theme;
                        }
                        else
                        {
                            loginView.Theme = "green";
                            ProjectSession.Theme = "green";
                            ProjectSession.Logo = "Images/logo.png";
                            loginView.Logo = "Images/logo.png";
                            loginView.isDisableSignUp = 1;
                        }
                    }
                    else
                    {
                        string defaultLoginCompanyName = _login.GetDefaultLoginCompanyName();
                        loginView.DefaultLoginCompanyName = ProjectSession.SiteUrlBase + defaultLoginCompanyName;
                        loginView.Theme = "green";
                        ProjectSession.Theme = "green";
                        ProjectSession.Logo = "Images/logo.png";
                        loginView.Logo = "Images/logo.png";
                        loginView.isActiveDiv = 1;
                        if (!string.IsNullOrEmpty(id) && id.ToLower() == ProjectConfiguration.CompanyNameForNewLoginScreen.ToLower())
                        {
                            return View("NewLogin", loginView);
                        }
                        return View(loginView);
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(returnUrl))
                    {
                        ViewBag.ReturnUrl = returnUrl;
                    }
                    loginView.Theme = "green";
                    ProjectSession.Theme = "green";
                    ProjectSession.LoginCompanyName = "";
                    ProjectSession.Logo = "Images/logo.png";
                    loginView.Logo = "Images/logo.png";
                }
                loginView.isActiveDiv = 0;
                if (!string.IsNullOrEmpty(id) && id.ToLower() == ProjectConfiguration.CompanyNameForNewLoginScreen.ToLower())
                {
                    return View("NewLogin", loginView);
                }
                return View(loginView);
            }

        }

        /// <summary>
        /// Post - Account login
        /// </summary>
        /// <param name="model">Login Model</param>
        /// <param name="returnUrl">Return URL</param>
        /// <param name="id">String Id</param>
        /// <returns>Action Result</returns>
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl, string id = null)
        {
            if (model.Username != null)
                ModelState.Remove("UserName");
            if (model.Password != null)
                ModelState.Remove("Password");
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser();
                UserManager.UserValidator = new UserValidator<ApplicationUser>(UserManager) { AllowOnlyAlphanumericUserNames = false };
                ResellerDetailView resellerDetail = new ResellerDetailView();

                if (!string.IsNullOrEmpty(id))
                    model.CompanyName = id;

                //if (!string.IsNullOrEmpty(id))
                //{
                //    //get reseller details from company name
                //    var dsReseller = _reseller.GetResellerByLoginCompanyName(id);
                //    List<ResellerDetailView> lstResellerDetail = DBClient.DataTableToList<ResellerDetailView>(dsReseller.Tables[0]);
                //    resellerDetail = lstResellerDetail.FirstOrDefault();

                //    //check reseller username exists in that company
                //    if (resellerDetail != null)
                //    {
                //        //fetch theme and logo of reseller
                //        model.Theme = ((SystemEnums.Theme)resellerDetail.Theme).ToString();
                //        ProjectSession.Theme = model.Theme;
                //        model.Logo = resellerDetail.Logo;
                //        //model.Logo = ProjectSession.UploadedDocumentPath + "UserDocuments/" + model.Logo;

                //        ProjectSession.Logo = model.Logo;
                //        user = await UserManager.FindAsync(model.Username, model.Password);
                //        //comment this section beacuse of now we make login global (from any reseller url anu user can login if username password are valid)

                //        //bool isUserExists = _reseller.CheckUserBelogsToResellerCompany(resellerDetail.ResellerID, model.Username);
                //        //if (isUserExists)
                //        //    user = await UserManager.FindAsync(model.Username, model.Password);
                //        //else
                //        //{
                //        //    this.ShowMessage(SystemEnums.MessageType.Error, "Invalid username or password.");
                //        //    if (!string.IsNullOrEmpty(id) && id.ToLower() == ProjectConfiguration.CompanyNameForNewLoginScreen.ToLower())
                //        //    {
                //        //        return View("NewLogin", model);
                //        //    }
                //        //    return View("Login", model);
                //        //}
                //    }
                //    else
                //    {
                //        this.ShowMessage(SystemEnums.MessageType.Error, "Invalid username or password.");
                //        if (!string.IsNullOrEmpty(id) && id.ToLower() == ProjectConfiguration.CompanyNameForNewLoginScreen.ToLower())
                //        {
                //            return View("NewLogin", model);
                //        }
                //        return View("Login", model);
                //    }
                //}
                //else
                //    ProjectSession.Theme = "green";
                //ProjectSession.Logo = ProjectSession.SiteUrlBase + "Images/logo.png";
                user = await UserManager.FindAsync(model.Username, model.Password);

                if (user != null)
                {

                    //FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, user.Id, DateTime.Now, DateTime.Now.AddMinutes(2), true, user.UserName, FormsAuthentication.FormsCookiePath);
                    //// Encrypt the ticket using the machine key
                    //string encryptedTicket = FormsAuthentication.Encrypt(ticket);
                    //// Add the cookie to the request to save it
                    //HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                    //cookie.HttpOnly = true;
                    //Response.Cookies.Add(cookie);

                    //AuthorizationContext filterContext = new AuthorizationContext();
                    //filterContext.HttpContext.Response.Cookies.Add(cookie);

                    var userDetail = _user.GetUserByAspnetUserId(user.Id);

                    //if from general setting IsAllowAccessToAllUser flag is true
                    if (userDetail != null && userDetail.FirstName != null)
                    {
                        if (!userDetail.IsActive)
                        {
                            this.ShowMessage(SystemEnums.MessageType.Error, "Your account is currently inactive.");
                            Logout(returnUrl);
                            if (!string.IsNullOrEmpty(id) && id.ToLower() == ProjectConfiguration.CompanyNameForNewLoginScreen.ToLower())
                            {
                                return View("NewLogin", model);
                            }
                            return View("Login", model);
                        }

                        if (userDetail.Theme != 0)
                        {
                            ProjectSession.Theme = ((SystemEnums.Theme)userDetail.Theme).ToString();
                            ProjectSession.Logo = userDetail.Logo;
                        }

                        else
                        {
                            ProjectSession.Theme = "green";
                            ProjectSession.Logo = "Images/logo.png";
                        }

                        //if (userDetail != null && userDetail.ResellerID > 0 && resellerDetail.ResellerID == 0 && userDetail.UserTypeID != 9)
                        //{
                        //    this.ShowMessage(SystemEnums.MessageType.Error, "Invalid username or password.");
                        //    if (!string.IsNullOrEmpty(id) && id.ToLower() == ProjectConfiguration.CompanyNameForNewLoginScreen.ToLower())
                        //    {
                        //        return View("NewLogin", model);
                        //    }
                        //    return View("Login", model);
                        //}

                        await SignInAsync(user, model.RememberMe);
                        ProjectSession.IsNewViewer = userDetail.IsNewViewer;
                        StoreSessionInfo(userDetail, id);

                        //ProjectSession.LoggedInUserId = userDetail.UserId;
                        //ProjectSession.LoggedInName = userDetail.FirstName + " " + userDetail.LastName;
                        //ProjectSession.RoleId = userDetail.RoleID;
                        //ProjectSession.UserTypeId = userDetail.UserTypeID;
                        //ProjectSession.IsSSCReseller = userDetail.IsSSCReseller;

                        //if (userDetail.ResellerID != null && userDetail.ResellerID > 0)
                        //{
                        //    ProjectSession.ResellerId = (int)userDetail.ResellerID;
                        //    ProjectSession.LoginCompanyName = id;
                        //}
                        //if (userDetail.SolarCompanyId != null && userDetail.SolarCompanyId > 0)
                        //    ProjectSession.SolarCompanyId = (int)userDetail.SolarCompanyId;

                        //ProjectSession.IsSubContractor = userDetail.IsSubContractor;

                        //Entity.Settings.Settings settings = new Entity.Settings.Settings();
                        //SettingsController objSettings = new SettingsController(_settingsBAL);
                        //settings = objSettings.GetSettingsData();
                        //ProjectSession.PartAccountTax = settings.PartAccountTax;
                        //ProjectSession.IsTaxInclusive = settings.IsTaxInclusive;

                        //Email information store		             
                        //StoreEmailInfo();

                        //Email information store
                        //FormBot.Entity.Email.EmailSignup emailModel = new Entity.Email.EmailSignup();
                        //string xmlString = string.Empty;
                        //DataSet lstEmail = _user.LoginUserEmailDetails(ProjectSession.LoggedInUserId);
                        //if (lstEmail.Tables[0].Rows.Count > 0)
                        //{                       
                        //    Session[FormBot.Email.Constants.sessionAccount] = null;
                        //    DataRow dr = (DataRow)lstEmail.Tables[0].Rows[0];

                        //    emailModel.Login = dr["email"].ToString();
                        //    emailModel.ConfigurationEmail = dr["email"].ToString();
                        //    emailModel.ConfigurationPassword = Utils.DecodePassword(Convert.ToString(dr["mail_inc_pass"]));
                        //    emailModel.IncomingMail = dr["mail_inc_host"].ToString();
                        //    emailModel.IncomingMailPort = Convert.ToInt32(dr["mail_inc_port"]);
                        //    emailModel.Login = dr["email"].ToString();
                        //    emailModel.OutgoingMail = dr["mail_out_host"].ToString();
                        //    emailModel.OutgoingMailPort = Convert.ToInt32(dr["mail_out_port"]);

                        //    xmlString = "<?xml version='1.0' encoding='UTF-8'?><webmail><param name='action' value='login' /><param name='request' value='' /><param name='email'><![CDATA[" + emailModel.ConfigurationEmail
                        //        + "]]></param><param name='mail_inc_login'><![CDATA[" + emailModel.Login + "]]></param><param name='mail_inc_pass'><![CDATA[" + emailModel.ConfigurationPassword + "]]></param><param name='mail_inc_host'><![CDATA[" + emailModel.IncomingMail
                        //        + "]]></param><param name='mail_inc_port' value='" + emailModel.IncomingMailPort + "' /><param name='mail_protocol' value='0' /><param name='mail_out_host'><![CDATA[" + emailModel.OutgoingMail
                        //        + "]]></param><param name='mail_out_port' value='" + emailModel.OutgoingMailPort + "' /><param name='mail_out_auth' value='1' /><param name='sign_me' value='0' /><param name='language' /><param name='advanced_login' value='1' /></webmail>";
                        //    CheckMail checkMail = new CheckMail();
                        //    var result = checkMail.GetMessages(xmlString);

                        //    #region Email configuration not required

                        //    ProjectSession.IsUserEmailAccountConfigured = true;
                        //    #endregion
                        //}
                        //else 
                        //{
                        //    #region Email configuration not required
                        //    Session[FormBot.Email.Constants.sessionAccount] = null;
                        //    ProjectSession.IsUserEmailAccountConfigured = false;

                        //    #endregion
                        //}

                        if (ProjectSession.UserTypeId == (int)SystemEnums.UserType.SolarElectriciansSWH || ProjectSession.UserTypeId == (int)SystemEnums.UserType.SolarElectricians || ProjectSession.UserTypeId == (int)SystemEnums.UserType.SolarCompanyAdmin)
                        {
                            var status = _login.CheckLoggedInUserStatus(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId);
                            var userStatus = !string.IsNullOrEmpty(status) ? Convert.ToInt32(status) : 0;
                            if (userStatus == 0 || userStatus != (int)SystemEnums.UserStatus.Approved)
                            {
                                this.ShowMessage(SystemEnums.MessageType.Error, "Your account is currently inactive.");
                                Logout(returnUrl);
                                if (!string.IsNullOrEmpty(id) && id.ToLower() == ProjectConfiguration.CompanyNameForNewLoginScreen.ToLower())
                                {
                                    return View("NewLogin", model);
                                }
                                return View("Login", model);
                            }
                        }

                        if (ProjectSession.RoleId > 0)
                        {
                            _login.UpdateLatLoginDate(ProjectSession.LoggedInUserId);

                            //if (userDetail.IsFirstLogin)
                            //    return RedirectToAction("MyProfile", "User");

                            if (ProjectSession.UserTypeId == (int)SystemEnums.UserType.SolarElectricians && !string.IsNullOrWhiteSpace(returnUrl))
                                return Redirect("User/Profile/?flg=DocUpload#horizontalTab3");
                            else if (ProjectSession.UserTypeId == (int)SystemEnums.UserType.FormBotSuperAdmin && !string.IsNullOrWhiteSpace(returnUrl))
                                return Redirect(returnUrl + "#horizontalTab3?flg=DocVerify");

                            var dsMenuActions = _login.GetMenuActionByRoleId(ProjectSession.RoleId);
                            if (dsMenuActions != null && dsMenuActions.Tables[0].Rows.Count > 0)
                            {
                                List<MenuActionView> MenuActionList = DBClient.DataTableToList<MenuActionView>(dsMenuActions.Tables[0]);
                                MenuActionView menuActionView = MenuActionList.FirstOrDefault();
                                if (menuActionView.Controller.ToLower().Contains("dashboard"))
                                {
                                    HttpContext.Response.Cookies.Set(new HttpCookie("menuname") { Value = "Home" });
                                }
                                else
                                {
                                    HttpContext.Response.Cookies.Set(new HttpCookie("menuname") { Value = "createnew" });
                                }
                                //if (menuActionView.Controller.ToLower() == "email")
                                //    return RedirectToAction(menuActionView.Action, menuActionView.Controller, new { area = "Email" });
                                //else
                                //{
                                //if user role does not have access for dashboard then display job listing page
                                if (!menuActionView.Controller.ToLower().Contains("dashboard"))
                                {
                                    MenuActionView _menulist = MenuActionList.FirstOrDefault(x => x.Action.ToLower().Equals("index") && x.Controller.ToLower().Equals("job") && x.MenuId.ToString() == SystemEnums.MenuId.JobView.GetHashCode().ToString());
                                    if (_menulist != null && _menulist.Action.ToLower().Equals("index") && _menulist.Controller.ToLower().Equals("job") && _menulist.MenuId.ToString() == SystemEnums.MenuId.JobView.GetHashCode().ToString())
                                    {
                                        return RedirectToAction(_menulist.Action, _menulist.Controller, new { area = "" });
                                    }
                                    else if (menuActionView.Controller.ToLower() == "email")
                                        return RedirectToAction(menuActionView.Action, menuActionView.Controller, new { area = "Email" });

                                    else
                                        return RedirectToAction(menuActionView.Action, menuActionView.Controller, new { area = "" });


                                }
                                else
                                    return RedirectToAction(menuActionView.Action, menuActionView.Controller, new { area = "" });

                                //}

                            }
                            else
                            {
                                this.ShowMessage(SystemEnums.MessageType.Error, "You don't have system access.");
                                if (!string.IsNullOrEmpty(id) && id.ToLower() == ProjectConfiguration.CompanyNameForNewLoginScreen.ToLower())
                                {
                                    return View("NewLogin", model);
                                }
                                return View("Login", model);
                            }
                        }
                        else
                        {
                            return RedirectToLocal(returnUrl);
                        }
                    }
                    else
                    {
                        this.ShowMessage(SystemEnums.MessageType.Error, "System is still undergoing maintenance and is expected to finish 10pm AEST, we appreciate your patience.");
                        if (!string.IsNullOrEmpty(id) && id.ToLower() == ProjectConfiguration.CompanyNameForNewLoginScreen.ToLower())
                        {
                            return View("NewLogin", model);
                        }
                        return View("Login", model);
                    }

                }
                else
                {
                    this.ShowMessage(SystemEnums.MessageType.Error, "Invalid username or password.");
                    if (!string.IsNullOrEmpty(id) && id.ToLower() == ProjectConfiguration.CompanyNameForNewLoginScreen.ToLower())
                    {
                        return View("NewLogin", model);
                    }
                    return View("Login", model);
                }

            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>
        /// Forgot password
        /// </summary>
        /// <returns>Action Result</returns>
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {

            //string url = Request.UrlReferrer.ToString();
            FormBot.Entity.User user = new FormBot.Entity.User();
            //var splitVals = url.Split('/');
            //user.LoginCompanyName = splitVals[splitVals.Length - 1].ToString();
            var dsReseller = _login.GetResellerThemeLogo(ProjectSession.LoginCompanyName);
            if (dsReseller != null && dsReseller.Tables[0].Rows.Count > 0)
            {
                List<ResellerDetailView> resellerDetail = DBClient.DataTableToList<ResellerDetailView>(dsReseller.Tables[0]);
                ResellerDetailView resellerDetailView = resellerDetail.FirstOrDefault();
                ProjectSession.Logo = resellerDetailView.Logo;
                ProjectSession.Theme = ((SystemEnums.Theme)resellerDetailView.Theme).ToString();
            }
            ForgotPasswordViewModel forgotPasswordViewModel = new ForgotPasswordViewModel();
            forgotPasswordViewModel.ResellerUrl = ProjectSession.SiteUrlBase + "" + ProjectSession.LoginCompanyName;
            return View(forgotPasswordViewModel);

            ////  ForgotPasswordViewModel forgotPasswordViewModel = new ForgotPasswordViewModel();
            ////  forgotPasswordViewModel.ResellerUrl = ProjectSession.SiteUrlBase + "" + ProjectSession.LoginCompanyName;
            //  // return View(forgotPasswordViewModel);
            //  OTPBasedResetPassword otpbasedresetpwd = new OTPBasedResetPassword();
            //  return View("OTPSet", otpbasedresetpwd);


        }

        /// <summary>
        /// Forgot UserName
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ForgotUserName()
        {
            FormBot.Entity.User user = new FormBot.Entity.User();
            var dsReseller = _login.GetResellerThemeLogo(ProjectSession.LoginCompanyName);
            if (dsReseller != null && dsReseller.Tables[0].Rows.Count > 0)
            {
                List<ResellerDetailView> resellerDetail = DBClient.DataTableToList<ResellerDetailView>(dsReseller.Tables[0]);
                ResellerDetailView resellerDetailView = resellerDetail.FirstOrDefault();
                ProjectSession.Logo = resellerDetailView.Logo;
                ProjectSession.Theme = ((SystemEnums.Theme)resellerDetailView.Theme).ToString();
            }
            ForgotViewModel forgotViewModel = new ForgotViewModel();
            forgotViewModel.ResellerUrl = ProjectSession.SiteUrlBase + "" + ProjectSession.LoginCompanyName;
            return View(forgotViewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ForgotUserName(ForgotViewModel forgotViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    bool isEmailExist = true;
                    bool status = GetUserNameByEmailId(forgotViewModel.Email, ref isEmailExist);

                    if(!isEmailExist)
                        this.ShowMessage(SystemEnums.MessageType.Error, "This email address does not exist.", true);
                    else
                    {
                        if (status)
                            this.ShowMessage(SystemEnums.MessageType.Success, "The email has been successfully sent.", true);
                        else
                            this.ShowMessage(SystemEnums.MessageType.Error, "The email has not been sent.", true);
                    }
                }
                catch (Exception ex)
                {
                    Helper.Helper.Common.WriteErrorLog(ex.Message);
                    this.ShowMessage(SystemEnums.MessageType.Error, "Something wrong with the forgot username, please contact the administrator.", true);
                }
                return RedirectToAction("ForgotUserName", "Account");
            }
            else
            {
                this.ShowMessage(SystemEnums.MessageType.Error, String.Join(Environment.NewLine, ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage + " " + v.Exception)), true);
                return RedirectToAction("ForgotUserName", "Account");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> OTPSet(OTPBasedResetPassword otpbasedresetpwd)
        {
            if (ModelState.IsValid)
            {
                ResetPasswordViewModel resetPasswordViewModel = new ResetPasswordViewModel();
                //int Otp = _login.GetOTPData(otpbasedresetpwd.UserId,otpbasedresetpwd.Username,otpbasedresetpwd.phone);
                int status = _login.GetOTPData(otpbasedresetpwd.UserId, otpbasedresetpwd.Username, otpbasedresetpwd.phone, otpbasedresetpwd.OtpCode);
                //if(Otp!=otpbasedresetpwd.OtpCode)
                if (status == 1)
                {

                    var dsReseller = _login.GetResellerThemeLogoForgetPassword(Convert.ToString(otpbasedresetpwd.UserId));
                    if (dsReseller != null && dsReseller.Tables[0].Rows.Count > 0)
                    {
                        List<ResellerDetailView> resellerDetail = DBClient.ToListof<ResellerDetailView>(dsReseller.Tables[0]);
                        ResellerDetailView resellerDetailView = resellerDetail.FirstOrDefault();
                        ProjectSession.Logo = resellerDetailView.Logo;
                        ProjectSession.Theme = ((SystemEnums.Theme)resellerDetailView.Theme).ToString();
                        resetPasswordViewModel.ResellerUrl = ProjectSession.SiteUrlBase + "" + resellerDetailView.LoginCompanyName;
                    }
                    else
                    {
                        resetPasswordViewModel.ResellerUrl = ProjectSession.SiteUrlBase;
                        ProjectSession.Theme = "green";
                        ProjectSession.Logo = "Images/logo.png";
                    }
                    string AspnetuserId = _login.GetAspnetUserId(otpbasedresetpwd.UserId);
                    var provider = new MachineKeyProtectionProvider();
                    UserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(
                            provider.Create("EmailConfirmation"));
                    var code = UserManager.GeneratePasswordResetToken(AspnetuserId);
                    resetPasswordViewModel.Code = code;
                    resetPasswordViewModel.ResellerUrl = ProjectSession.SiteUrlBase;
                    return View("ResetPassword", resetPasswordViewModel);

                }
                else
                {
                    this.ShowMessage(SystemEnums.MessageType.Error, "OTP has been invalid,Please try again.", true);
                    otpbasedresetpwd.OtpCode = null;
                    return View("OTPSet", otpbasedresetpwd);

                    //return RedirectToAction("ForgotPassword", "Account");
                }

            }
            else
            {
                return RedirectToAction("ForgotPassword", "Account");
            }



        }
        /// <summary>
        /// POST - Forgot password
        /// </summary>
        /// <param name="model">Forgot password model</param>
        /// <returns>Task List ActionResult</returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {

            if (ModelState.IsValid)
            {
                var users = new ApplicationUser();
                ResellerDetailView resellerDetail = new ResellerDetailView();
                var user = UserManager.FindByName(model.Username);
                if (user == null)
                {
                    this.ShowMessage(SystemEnums.MessageType.Error, "Please check your Username.", true);
                    return View(model);
                }
                #region comment
                //if (!string.IsNullOrEmpty(ProjectSession.LoginCompanyName))
                //{
                //    //get reseller details from company name
                //    var dsReseller = _reseller.GetResellerByLoginCompanyName(ProjectSession.LoginCompanyName);
                //    List<ResellerDetailView> lstResellerDetail = DBClient.DataTableToList<ResellerDetailView>(dsReseller.Tables[0]);
                //    resellerDetail = lstResellerDetail.FirstOrDefault();
                //    //comment this section beacuse now we allow any user to forgot password from any reseller url

                //    //if (resellerDetail != null)
                //    //{
                //    //    bool isUserExists = _reseller.CheckUserBelogsToResellerCompany(resellerDetail.ResellerID, model.Username);
                //    //    if (!isUserExists)
                //    //    {
                //    //        model.ResellerUrl = ProjectSession.SiteUrlBase + "" + ProjectSession.LoginCompanyName;
                //    //        //users = await UserManager.FindAsync(model.Username);
                //    //        this.ShowMessage(SystemEnums.MessageType.Error, "Username is invalid.");
                //    //        return View("ForgotPassword", model);
                //    //    }
                //    //}
                //    if (resellerDetail == null)
                //    {
                //        model.ResellerUrl = ProjectSession.SiteUrlBase + "" + ProjectSession.LoginCompanyName;
                //        this.ShowMessage(SystemEnums.MessageType.Error, "Username is invalid.");
                //        return View("ForgotPassword", model);
                //    }
                //}
                #endregion comment
                var userDetail = _user.GetUserByAspnetUserId(user.Id);
                //if (userDetail != null && userDetail.ResellerID > 0 && resellerDetail.ResellerID == 0)
                //{
                //    model.ResellerUrl = ProjectSession.SiteUrlBase;
                //    this.ShowMessage(SystemEnums.MessageType.Error, "Username is invalid.");
                //    return View("ForgotPassword", model);
                //}

                var usr = _user.GetUserById(userDetail.UserId);
                FormBot.Entity.User uv = DBClient.DataTableToList<FormBot.Entity.User>(usr.Tables[0]).FirstOrDefault();

                if (ConfigurationManager.AppSettings["OTPbasedPwdReset"] == "false")
                {

                    ResetPwdByMail(uv.FirstName, uv.LastName, uv.PasswordLink, uv.Email, uv.AspNetUserId);
                    return RedirectToAction("ForgotPassword", "Account");
                }
                else
                {
                    TempData["OtpDetails"] = uv;
                    TempData["IsRedirect"] = true;
                    return RedirectToAction("OTPSet", "Account");

                    //OTPbasedResetPwd(uv.Phone,uv.UserId,uv.UserName);
                    // OTPBasedResetPassword otpbasedresetpwd = new OTPBasedResetPassword();
                    //otpbasedresetpwd.UserId = uv.UserId;
                    //otpbasedresetpwd.phone = uv.Phone;
                    //otpbasedresetpwd.OtpCode = null;
                    //otpbasedresetpwd.ResellerUrl = ProjectSession.SiteUrlBase;
                    //return View("OTPSet", otpbasedresetpwd);
                }
            }
            else
            {
                this.ShowMessage(SystemEnums.MessageType.Error, String.Join(Environment.NewLine, ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage + " " + v.Exception)), true);
                return RedirectToAction("ForgotPassword", "Account");
            }

        }

        [AllowAnonymous]
        public ActionResult OTPSet()
        {
            var isRedirect = TempData["IsRedirect"] as bool?;
            if (isRedirect != null && (Convert.ToBoolean(isRedirect) || Request.IsAjaxRequest()))
            {
                var uv = TempData["OtpDetails"] as FormBot.Entity.User;
                if (uv != null)
                {
                    TempData.Keep();
                    OTPbasedResetPwd(uv.Phone, uv.UserId, uv.UserName);
                    OTPBasedResetPassword otpbasedresetpwd = new OTPBasedResetPassword();
                    otpbasedresetpwd.UserId = uv.UserId;
                    otpbasedresetpwd.phone = uv.Phone;
                    otpbasedresetpwd.ResendOtpSecond = 120;
                    otpbasedresetpwd.OtpCode = null;
                    otpbasedresetpwd.Username = uv.UserName;
                    otpbasedresetpwd.ResellerUrl = ProjectSession.SiteUrlBase;
                    TempData["IsRedirect"] = null;
                    return View(otpbasedresetpwd);
                }
            }
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public JsonResult GetResetPwdFlag()
        {
            bool resetPwdflag = false;
            if (ProjectSession.IsresetPwd == null)
            {
                resetPwdflag = _login.GetResetFlag(ProjectSession.LoggedInUserId);
                ProjectSession.IsresetPwd = resetPwdflag;
            }
            return Json(new { status = true, resetPwdflag }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Account reset password
        /// </summary>
        /// <param name="UserId">String User Id</param>
        /// <param name="code">String Code</param>
        /// <returns>Action Result</returns>
        [AllowAnonymous]
        public ActionResult ResetPassword(string UserId, string code, DateTime currentDate)
        {
            if ((DateTime.Now - currentDate).TotalMinutes > Convert.ToInt32(ConfigurationManager.AppSettings["ResetPasswordLinkExpirationTimeInMinute"]))
            {
                this.ShowMessage(SystemEnums.MessageType.Error, "Sorry! Your link for reset password has been expired.", true);
                return RedirectToAction("ForgotPassword", "Account");

            }
            else
            {
                ResetPasswordViewModel model = new ResetPasswordViewModel();
                var dsReseller = _login.GetResellerThemeLogoForgetPassword(UserId);
                if (dsReseller != null && dsReseller.Tables[0].Rows.Count > 0)
                {
                    List<ResellerDetailView> resellerDetail = DBClient.ToListof<ResellerDetailView>(dsReseller.Tables[0]);
                    ResellerDetailView resellerDetailView = resellerDetail.FirstOrDefault();
                    ProjectSession.Logo = resellerDetailView.Logo;
                    ProjectSession.Theme = ((SystemEnums.Theme)resellerDetailView.Theme).ToString();
                    model.ResellerUrl = ProjectSession.SiteUrlBase + "" + resellerDetailView.LoginCompanyName;
                }
                else
                {
                    model.ResellerUrl = ProjectSession.SiteUrlBase;
                    ProjectSession.Theme = "green";
                    ProjectSession.Logo = "Images/logo.png";
                }
                model.Code = code;
                model.currentDate = Convert.ToString(currentDate);
                return View(model);
            }

        }

        /// <summary>
        /// Post - Account reset password
        /// </summary>
        /// <param name="model">Reset Password ViewModel</param>
        /// <returns>Task List ActionResult</returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {

                if (ConfigurationManager.AppSettings["OTPbasedPwdReset"] == "true")
                {
                    model.ResellerUrl = Request["r-url"];
                }
                var user = await UserManager.FindByNameAsync(model.Username);
                if (user == null)
                {
                    this.ShowMessage(SystemEnums.MessageType.Error, "Please check your Username.", true);
                    return View(model);
                }

                var provider = new MachineKeyProtectionProvider();

                UserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(
                    provider.Create("EmailConfirmation"));
                user.ModifiedPwdDate = DateTime.Now;
                var data = _login.GetOldPasswordHistory(user.Id);
                for (int i = 0; i < data.Count; i++)
                {
                    PasswordHasher passwordHasher = new PasswordHasher();
                    var resultPasswordMatch = passwordHasher.VerifyHashedPassword(data[i], model.Password);
                    if (resultPasswordMatch.HasFlag(PasswordVerificationResult.Success))
                    {
                        this.ShowMessage(SystemEnums.MessageType.Error, "Sorry! Password has been used in last three password", true);
                        return RedirectToAction("ForgotPassword", "Account");
                    }

                }

                var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);

                if (result != null && result.Errors != null)
                {
                    foreach (var item in result.Errors)
                    {
                        //FormBot.Helper.Helper.Common.WriteErrorLog(item);
                        Helper.Log.WriteLog(item);
                    }
                }


                if (result.Succeeded)
                {
                    user.ModifiedPwdDate = DateTime.Now;
                    user.IsResetPwd = false;
                    ProjectSession.IsresetPwd = false;
                    this.ShowMessage(SystemEnums.MessageType.Success, "Password reset successfully.", true);
                    var UserData = _user.GetUserByUserName(model.Username);
                    await UserManager.UpdateAsync(user);
                    //when user resetpassword then from mobile device their login remove and its neccesaary to login eith new pwd for further access in APP
                    _user.User_UpdateApiTokenFromResetPassword(user.Id);
                    _login.InsertPasswordHistory(user.Id);
                    return Redirect(model.ResellerUrl);
                    //return RedirectToAction("Login", "Account");
                }
                else
                {
                    this.ShowMessage(SystemEnums.MessageType.Error, "Sorry! Token has expired. Do forgot password again.", true);
                    return RedirectToAction("ForgotPassword", "Account");
                }
            }
            else
            {
                this.ShowMessage(SystemEnums.MessageType.Error, String.Join(Environment.NewLine, ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage + " " + v.Exception)), true);
                return RedirectToAction("ResetPassword", "Account");
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public string GetPasswordResetToken(string aspnetuserid)
        {
            try
            {
                if (string.IsNullOrEmpty(aspnetuserid))
                    return "";
                else
                {
                    var provider = new MachineKeyProtectionProvider();
                    UserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(
                        provider.Create("EmailConfirmation"));

                    return UserManager.GeneratePasswordResetToken(aspnetuserid);
                }
            }
            catch (Exception ex)
            {
                //Helper.Helper.Common.WriteErrorLog(ex.Message);
                Helper.Log.WriteError(ex,"GetPasswordResetToken");
                return "";
            }


        }
        public void OTPbasedResetPwd(string phoneNo, int userId, string UserName)
        {
            //Send SMS for OTP 
            var IP = CommonMethods.GetIp();
            //var phone = "91" + phoneNo;
            var phone = phoneNo;

            int otpValue = new Random().Next(100000, 999999);
            string SmsContent = string.Empty;
            var last4digit_phone = "******" + phone.Substring(phone.Length - 4, 4);
            if (phone == null)
            {
                this.ShowMessage(SystemEnums.MessageType.Error, "Your phone number not registered please inform admin..");
            }

            else
            {
                SmsContent = "OTP for reset Password: " + otpValue;
                string api = ProjectConfiguration.SMSGlobalURL + "?action=sendsms&user=" + ConfigurationManager.AppSettings["SMSGlobalUserName"] + "&password=" + ConfigurationManager.AppSettings["SMSGlobalPassword"] + "&&from=" + "Greenbot" + "&to=" + phone + "&text=" + SmsContent;
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(string.Format(api));
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                byte[] postData = Encoding.ASCII.GetBytes(string.Format(api));
                req.ContentLength = postData.Length;
                // Send HTTP request.
                Stream PostStream = req.GetRequestStream();
                PostStream.Write(postData, 0, postData.Length);
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                _login.InsertOtpData(userId, UserName, phoneNo, IP, otpValue);

                if (res.StatusCode == HttpStatusCode.OK)
                {
                    this.ShowMessage(SystemEnums.MessageType.Success, "OTP has been sent to " + last4digit_phone);
                }

            }


        }

        public void ResetPwdByMail(string Firstname, string lastname, string pwdlink, string email, string AspnetUserId)
        {
            //Generate Password Token
            var provider = new MachineKeyProtectionProvider();
            UserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(
                provider.Create("EmailConfirmation"));
            var code = UserManager.GeneratePasswordResetToken(AspnetUserId);

            pwdlink = Url.Action("ResetPassword", "Account", new { UserId = AspnetUserId, code = code, currentDate = DateTime.Now }, protocol: Request.Url.Scheme);

            EmailTemplate eTemplate = _emailBAL.GetEmailTemplateByID(9);
            EmailInfo emailInfo = new EmailInfo();
            emailInfo.FirstName = Firstname;
            emailInfo.LastName = lastname;
            emailInfo.PasswordLink = pwdlink;
            eTemplate.Content = _emailBAL.GetEmailBody(emailInfo, eTemplate);
            //EmailTemplate eTemplate = _emailBAL.PrepareEmailTemplate(9, null, uv, null, null);

            bool status = SendEmailForForgotPassword(eTemplate, email);
            if (status)
                this.ShowMessage(SystemEnums.MessageType.Success, "Email has been sent successfully.", true);
            else
                this.ShowMessage(SystemEnums.MessageType.Error, "Sorry! unable to send email.", true);
        }
        /// <summary>
        /// Account logout
        /// </summary>
        /// <param name="returnUrl">Return Url</param>
        /// <returns>Action Result</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Logout(string returnUrl)
        {
            //CacheConfiguration.Clear();

            string strCompanyName = string.Empty;
            if (!string.IsNullOrEmpty(ProjectSession.LoginCompanyName))
                strCompanyName = ProjectSession.LoginCompanyName;

            AuthenticationManager.SignOut();
            FormsAuthentication.SignOut();
            Session.Abandon();


            System.Web.HttpContext.Current.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
            ApplicationSettingsTest applicationSettingsTest = new ApplicationSettingsTest();
            // Xero token delete
            try
            {
                FormBot.Main.Helpers.XeroApiHelper xeroApiHelper = new Helpers.XeroApiHelper();
                ApiUser _user = xeroApiHelper.User();

                MemoryAccessTokenStore store = new MemoryAccessTokenStore();
                IToken token = store.Find(_user.Name);
                if (token != null)
                    store.Delete(token);
            }
            catch (RenewTokenException)
            {
            }
            catch (XeroApiException)
            {
            }
            catch (Exception)
            {
            }

            //HttpCookie objCookie = Request.Cookies.Get("FormBotCookie");
            //if (objCookie != null)
            //{
            //    objCookie.Expires = DateTime.Now.AddDays(-1);
            //}
            if (Request.Cookies["LoginCompanyName"] != null)
            {
                HttpCookie objCookie = new HttpCookie("LoginCompanyName");
                objCookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(objCookie);
            }

            return RedirectToRoute(new
            {
                id = strCompanyName,
                area = "",
                controller = "Account",
                action = "Login"

            });
            //return RedirectToAction("Login", "Account");
        }

        /// <summary>
        /// Forms the bot sign up.
        /// </summary>
        /// <param name="id">String Id</param>
        /// <returns>Action Result</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult GreenbotSignUp(string id = null, string flg = null)
        {
            FormBot.Entity.User user = new FormBot.Entity.User();
            int userId = 0;
            if (!string.IsNullOrEmpty(id))
                int.TryParse(QueryString.GetValueFromQueryString(id, "id"), out userId);
            var dsUsers = _user.GetUserById(userId);
            List<FormBot.Entity.User> users = DBClient.DataTableToList<FormBot.Entity.User>(dsUsers.Tables[0]);
            FormBot.Entity.User userView = users.FirstOrDefault();
            if (id != null)
            {
                if (userView.IsPostalAddress == true)
                {
                    userView.AddressID = 2;
                }
                else
                {
                    userView.AddressID = 1;
                }
                if (userView.IsSTC == true)
                {
                    userView.chkSTC = 1;
                }
                else
                {
                    userView.chkSTC = 0;
                }
                ProjectSession.LoginCompanyName = userView.LoginCompanyName;
                //HttpCookie objCookie = new HttpCookie("LoginCompanyName");
                //objCookie["LoginCompanyName"] = ProjectSession.LoginCompanyName;
                //objCookie.Expires = DateTime.MaxValue;
                //Response.Cookies.Add(objCookie);
                userView.lstUserDocument = _user.GetUserUsersDocumentByUserId(userId);
                DateTime fromDate = Convert.ToDateTime(userView.FromDate);
                DateTime toDate = Convert.ToDateTime(userView.ToDate);
                userView.strFromDate = fromDate.ToString("yyyy-MM-dd");
                userView.strToDate = toDate.ToString("yyyy-MM-dd");
            }

            var designRole = from SystemEnums.SEDesignRole s in Enum.GetValues(typeof(SystemEnums.SEDesignRole))
                             select new { ID = s.GetHashCode(), Name = s.ToString() };
            ViewBag.SEDesignRole = new SelectList(designRole, "ID", "Name");

            //var designType = from SystemEnums.PostalAddressType s in Enum.GetValues(typeof(SystemEnums.PostalAddressType))
            //                 select new { ID = s.GetHashCode(), Name = s.ToString().Replace('_', ' ') };
            ViewBag.PostalAddressType = new SelectList(EnumExtensions.GetPostalAddressEnumList(), "ID", "Name");
            if (userId > 0)
            {
                userView.StatusName = ((SystemEnums.UserStatus)userView.Status).ToString().ToLower();
            }
            user.Guid = Guid.NewGuid().ToString();
            DateTime dat = Convert.ToDateTime(user.FromDate);
            user.strFromDate = dat.ToString("yyyy-MM-dd");
            ProjectSession.Theme = "green";
            ProjectSession.Logo = "Images/logo.png";
            if (!string.IsNullOrWhiteSpace(flg))
            {
                TempData["Flag"] = flg;
            }
            if (id != null)
            {
                return this.View(userView);
            }
            else
            {
                user.UserTypeID = SystemEnums.UserType.SolarElectricians.GetHashCode();
                return this.View(user);
            }

        }

        /// <summary>
        /// Resellers the sign up.
        /// </summary>
        /// <param name="companyName">The companyName.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>Action Result</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ResellerSignUp(string companyName, string id = null)
        {
            FormBot.Entity.User user = new FormBot.Entity.User();
            user.UserTypeID = 4;
            if (!string.IsNullOrEmpty(companyName))
            {
                ProjectSession.LoginCompanyName = companyName;

                int userId = 0;
                if (!string.IsNullOrEmpty(id))
                    int.TryParse(QueryString.GetValueFromQueryString(id, "id"), out userId);
                var dsUsers = _user.GetUserById(userId);
                List<FormBot.Entity.User> users = DBClient.DataTableToList<FormBot.Entity.User>(dsUsers.Tables[0]);
                FormBot.Entity.User userView = users.FirstOrDefault();
                if (id != null)
                {
                    if (userView.IsPostalAddress == true)
                    {
                        userView.AddressID = 2;
                    }
                    else
                    {
                        userView.AddressID = 1;
                    }
                    if (userView.IsSTC == true)
                    {
                        userView.chkSTC = 1;
                    }
                    else
                    {
                        userView.chkSTC = 0;
                    }
                    ProjectSession.LoginCompanyName = userView.LoginCompanyName;
                    //HttpCookie objCookie = new HttpCookie("LoginCompanyName");
                    //objCookie["LoginCompanyName"] = ProjectSession.LoginCompanyName;
                    //objCookie.Expires = DateTime.MaxValue;
                    //Response.Cookies.Add(objCookie);

                    userView.lstUserDocument = _user.GetUserUsersDocumentByUserId(userId);
                    DateTime fromDate = Convert.ToDateTime(userView.FromDate);
                    DateTime toDate = Convert.ToDateTime(userView.ToDate);
                    userView.strFromDate = fromDate.ToString("yyyy-MM-dd");
                    userView.strToDate = toDate.ToString("yyyy-MM-dd");
                }

                int resellerID = _user.GetResellerIdByLoginCompanyName(ProjectSession.LoginCompanyName);
                string clientNumber = string.Empty;

                if (resellerID > 0)
                {
                    List<KeyValuePair<string, string>> clientNumberUserType = GetClientNumber(2, resellerID, 0);
                    clientNumber = clientNumberUserType.Where(a => a.Key == "ClientNumber").FirstOrDefault().Value;
                }

                user.ClientNumber = clientNumber;
                user.ResellerID = resellerID;
                user.ResellerUrl = ProjectSession.SiteUrlBase + "" + ProjectSession.LoginCompanyName;
                //var designType = from SystemEnums.PostalAddressType s in Enum.GetValues(typeof(SystemEnums.PostalAddressType))
                //                 select new { ID = s.GetHashCode(), Name = s.ToString().Replace('_', ' ') };
                ViewBag.PostalAddressType = new SelectList(EnumExtensions.GetPostalAddressEnumList(), "ID", "Name");
                user.Guid = Guid.NewGuid().ToString();
                DateTime dat = Convert.ToDateTime(user.FromDate);
                user.strFromDate = dat.ToString("yyyy-MM-dd");
                var dsReseller = _login.GetResellerThemeLogo(ProjectSession.LoginCompanyName);
                if (userId > 0)
                {
                    userView.StatusName = ((SystemEnums.UserStatus)userView.Status).ToString().ToLower();
                }
                if (dsReseller != null && dsReseller.Tables[0].Rows.Count > 0)
                {
                    List<ResellerDetailView> resellerDetail = DBClient.DataTableToList<ResellerDetailView>(dsReseller.Tables[0]);
                    ResellerDetailView resellerDetailView = resellerDetail.FirstOrDefault();
                    user.Logo = resellerDetailView.Logo;
                    ProjectSession.Logo = user.Logo;
                    ProjectSession.Theme = ((SystemEnums.Theme)resellerDetailView.Theme).ToString();
                }
                if (id != null)
                {
                    userView.ResellerUrl = user.ResellerUrl;
                    return this.View(userView);
                }
                else
                {
                    return this.View(user);
                }
            }
            else
            {
                //var designType = from SystemEnums.PostalAddressType s in Enum.GetValues(typeof(SystemEnums.PostalAddressType))
                //                 select new { ID = s.GetHashCode(), Name = s.ToString().Replace('_', ' ') };
                ViewBag.PostalAddressType = new SelectList(EnumExtensions.GetPostalAddressEnumList(), "ID", "Name");
                this.ShowMessage(SystemEnums.MessageType.Error, "Please SignUp under any Reseller Company.");
                ProjectSession.Theme = "green";
                ProjectSession.Logo = "Images/logo.png";
                return View(user);
            }
        }

        /// <summary>
        /// Insert Error into LogFile
        /// </summary>
        /// <param name="strErrortype">Error type</param>
        /// <param name="strstatus">String Status</param>
        /// <param name="strexception">String Exception</param>
        /// <returns>Action Result</returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult InsertErrorintoLogFile(string strErrortype, string strstatus, string strexception, string url)
        {
            //RECRegistryHelper.WriteToLogFile("Error Date:" + DateTime.Now.ToString() + " Error from AjaxError method. Type" + strErrortype + " Status:" + strstatus + " Exception:" + strexception + " Error requesting url: " + url);
            _log.LogException("Error Date:" + DateTime.Now.ToString() + " Error from AjaxError method. Type" + strErrortype + " Status:" + strstatus + " Exception:" + strexception + " Error requesting url: " + url, null);
            return Json("1");
        }

        /// <summary>
        /// Redirect FSA To User
        /// </summary>
        /// <param name="userID">userID</param>
        /// <returns>JsonResult</returns>
        public async Task<JsonResult> RedirectFSAToUser(string userID)
        {
            //ProjectSession.DynamicMenuBinding = null;

            bool isUSERFSA = ProjectSession.IsUserFSA;
            int loggedInUserId = ProjectSession.LoggedInUserId;

            if (!string.IsNullOrEmpty(userID) && Convert.ToInt32(userID) == 0)
                userID = Convert.ToString(ProjectSession.FSALoggedInUserId);
            var userDetail = _user.GetUsersByFSA(Convert.ToInt32(userID));

            ApplicationUser user = UserManager.FindById(userDetail.AspNetUserId);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, identity);

            if (!isUSERFSA)
            {
                ProjectSession.FSALoggedInUserId = loggedInUserId;
            }
            ProjectSession.DynamicMenuBinding = null;

            StoreSessionInfo(userDetail);
            ProjectSession.IsUserFSA = true;
            StoreEmailInfo();
            var userDetail1 = _user.GetUserByAspnetUserId(user.Id);
            if (userDetail1.Theme != 0)
            {
                ProjectSession.Theme = ((SystemEnums.Theme)userDetail1.Theme).ToString();
                ProjectSession.Logo = userDetail1.Logo;
            }
            else
            {
                ProjectSession.Theme = "green";
                ProjectSession.Logo = "Images/logo.png";
            }
            if (ProjectSession.RoleId > 0)
            {
                var dsMenuActions = _login.GetMenuActionByRoleId(ProjectSession.RoleId);
                if (dsMenuActions != null && dsMenuActions.Tables[0].Rows.Count > 0)
                {
                    List<MenuActionView> MenuActionList = DBClient.DataTableToList<MenuActionView>(dsMenuActions.Tables[0]);
                    MenuActionView menuActionView = MenuActionList.FirstOrDefault();

                    if (menuActionView.Controller.ToLower().Contains("dashboard"))
                    {
                        HttpContext.Response.Cookies.Set(new HttpCookie("menuname") { Value = "Home" });
                    }
                    else
                    {
                        HttpContext.Response.Cookies.Set(new HttpCookie("menuname") { Value = "createnew" });
                    }


                    //if user role does not have access for dashboard then display job listing page 
                    if (!menuActionView.Controller.ToLower().Contains("dashboard"))
                    {
                        MenuActionView _menulist = MenuActionList.FirstOrDefault(x => x.Action.ToLower().Equals("index") && x.Controller.ToLower().Equals("job") && x.MenuId.ToString() == SystemEnums.MenuId.JobView.GetHashCode().ToString());
                        if (_menulist != null && _menulist.Action.ToLower().Equals("index") && _menulist.Controller.ToLower().Equals("job") && _menulist.MenuId.ToString() == SystemEnums.MenuId.JobView.GetHashCode().ToString())
                        {
                            return Json(new { status = true, action = _menulist.Action, controller = _menulist.Controller, area = "" }, JsonRequestBehavior.AllowGet);
                        }
                        else if (menuActionView.Controller.ToLower() == "email")
                            return Json(new { status = true, action = menuActionView.Action, controller = menuActionView.Controller, area = "Email" }, JsonRequestBehavior.AllowGet);


                        else
                            return Json(new { status = true, action = menuActionView.Action, controller = menuActionView.Controller, area = "" }, JsonRequestBehavior.AllowGet);

                    }
                    else
                        return Json(new { status = true, action = menuActionView.Action, controller = menuActionView.Controller, area = "" }, JsonRequestBehavior.AllowGet);
                }

                else
                {
                    return Json(new { status = false, error = "User don't have system access." }, JsonRequestBehavior.AllowGet);
                }
            }
            else
                return Json(new { status = false, error = "User don't have any role." }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Store Session Info
        /// </summary>
        /// <param name="userDetail"></param>
        /// <param name="id"></param>
        public void StoreSessionInfo(FormBot.Entity.User userDetail, string id = null)
        {
            ProjectSession.LoggedInUserId = userDetail.UserId;
            ProjectSession.LoggedInName = userDetail.FirstName + " " + userDetail.LastName;
            ProjectSession.RoleId = userDetail.RoleID;
            ProjectSession.UserTypeId = userDetail.UserTypeID;
            ProjectSession.IsSSCReseller = userDetail.IsSSCReseller;
            ProjectSession.IsTabularView = userDetail.IsTabularView;
            ProjectSession.IsresetPwd = userDetail.IsResetPwd;
            if (userDetail.ResellerID != null && userDetail.ResellerID > 0)
            {
                ProjectSession.ResellerId = (int)userDetail.ResellerID;
                ProjectSession.LoginCompanyName = id;
            }
            else
            {
                ProjectSession.ResellerId = 0;
                ProjectSession.LoginCompanyName = "";
            }
            if (userDetail.SolarCompanyId != null && userDetail.SolarCompanyId > 0)
            {
                ProjectSession.IsAllowTrade = userDetail.IsAllowTrade;
                ProjectSession.SolarCompanyId = (int)userDetail.SolarCompanyId;
                ProjectSession.IsWholeSaler = userDetail.IsWholeSaler;
            }
            else
            {
                ProjectSession.IsAllowTrade = false;
                ProjectSession.SolarCompanyId = 0;
                ProjectSession.IsWholeSaler = false;
            }

            ProjectSession.IsSubContractor = userDetail.IsSubContractor;
            Entity.Settings.Settings settings = new Entity.Settings.Settings();
            SettingsController objSettings = new SettingsController(_settingsBAL);
            settings = objSettings.GetSettingsData();
            ProjectSession.PartAccountTax = settings.PartAccountTax;
            ProjectSession.IsTaxInclusive = settings.IsTaxInclusive;
            if (settings.XeroPartsCodeId != null && !string.IsNullOrEmpty(settings.XeroPartsCodeId.ToString()))
            {
                ProjectSession.SaleAccountCode = settings.lstXeroPartsCodeId.AsEnumerable().Where(a => a.Id == settings.XeroPartsCodeId).Select(a => a.Code).FirstOrDefault();
            }
            ProjectSession.UserWiseGridConfiguration = _user.GetUserWiseGridConfiguration(userDetail.UserId);
        }

        /// <summary>
        /// Store Email Info
        /// </summary>
        public void StoreEmailInfo()
        {
            FormBot.Entity.Email.EmailSignup emailModel = new Entity.Email.EmailSignup();
            string xmlString = string.Empty;
            DataSet lstEmail = _user.LoginUserEmailDetails(ProjectSession.LoggedInUserId);
            if (lstEmail.Tables[0].Rows.Count > 0)
            {
                Session[FormBot.Email.Constants.sessionAccount] = null;
                DataRow dr = (DataRow)lstEmail.Tables[0].Rows[0];
                emailModel.Login = dr["email"].ToString();
                emailModel.ConfigurationEmail = dr["email"].ToString();
                emailModel.ConfigurationPassword = Utils.DecodePassword(Convert.ToString(dr["mail_inc_pass"]));
                emailModel.IncomingMail = dr["mail_inc_host"].ToString();
                emailModel.IncomingMailPort = Convert.ToInt32(dr["mail_inc_port"]);
                emailModel.Login = dr["email"].ToString();
                emailModel.OutgoingMail = dr["mail_out_host"].ToString();
                emailModel.OutgoingMailPort = Convert.ToInt32(dr["mail_out_port"]);
                xmlString = "<?xml version='1.0' encoding='UTF-8'?><webmail><param name='action' value='login' /><param name='request' value='' /><param name='email'><![CDATA[" + emailModel.ConfigurationEmail
                    + "]]></param><param name='mail_inc_login'><![CDATA[" + emailModel.Login + "]]></param><param name='mail_inc_pass'><![CDATA[" + emailModel.ConfigurationPassword + "]]></param><param name='mail_inc_host'><![CDATA[" + emailModel.IncomingMail
                    + "]]></param><param name='mail_inc_port' value='" + emailModel.IncomingMailPort + "' /><param name='mail_protocol' value='0' /><param name='mail_out_host'><![CDATA[" + emailModel.OutgoingMail
                    + "]]></param><param name='mail_out_port' value='" + emailModel.OutgoingMailPort + "' /><param name='mail_out_auth' value='1' /><param name='sign_me' value='0' /><param name='language' /><param name='advanced_login' value='1' /></webmail>";
                CheckMail checkMail = new CheckMail();
                var result = checkMail.GetMessages(xmlString);
                #region Email configuration not required
                ProjectSession.IsUserEmailAccountConfigured = true;
                #endregion
            }
            else
            {
                #region Email configuration not required
                Session[FormBot.Email.Constants.sessionAccount] = null;
                ProjectSession.IsUserEmailAccountConfigured = false;
                #endregion
            }
        }

        #endregion

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }

                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
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
                _purposes = purposes;
            }

            public byte[] Protect(byte[] userData)
            {
                return MachineKey.Protect(userData, _purposes);
            }

            public byte[] Unprotect(byte[] protectedData)
            {
                return MachineKey.Unprotect(protectedData, _purposes);
            }
        }

        /// <summary>
        /// Method used to set SMTP details for sending email
        /// </summary>
        /// <param name="callBackUrl">Callback Url</param>
        /// <param name="email">String Email</param>
        /// <param name="firstName">First Name</param>
        /// <param name="lastName">Last Name</param>
        /// <returns>True or False (Is mail sent)</returns>
        public bool SendEmail(string callBackUrl, string email, string firstName, string lastName)
        {
            MailReponse obj = new MailReponse();
            string FailReason = string.Empty;
            string messageBody = PopulateBody(callBackUrl, email, firstName, lastName);
            try
            {
                bool status = false;

                //SMTPDetails smtpDetail = new SMTPDetails();
                //            smtpDetail.MailFrom = ProjectSession.MailFrom;
                //            smtpDetail.SMTPUserName = ProjectSession.SMTPUserName;
                //            smtpDetail.SMTPPassword = ProjectSession.SMTPPassword;
                //            smtpDetail.SMTPHost = ProjectSession.SMTPHost;
                //            smtpDetail.SMTPPort = Convert.ToInt32(ProjectSession.SMTPPort);
                //            smtpDetail.IsSMTPEnableSsl = ProjectSession.IsSMTPEnableSsl;
                if (messageBody != null && !string.IsNullOrEmpty(messageBody))
                {
                    QueuedEmail objQueuedEmail = new QueuedEmail();
                    objQueuedEmail.FromEmail = ProjectSession.MailFrom;
                    objQueuedEmail.Body = messageBody;
                    objQueuedEmail.Subject = "Reset Password";
                    objQueuedEmail.ToEmail = email;
                    objQueuedEmail.CreatedDate = DateTime.Now;
                    objQueuedEmail.ModifiedDate = DateTime.Now;
                    _emailBAL.InsertUpdateQueuedEmail(objQueuedEmail);
                    status = true;
                }
                //= MailHelper.SendMail(smtpDetail, email, "", "", "Reset Password", messageBody, "", true, ref FailReason);
                if (status)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FailReason = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Prepare email body
        /// </summary>
        /// <param name="callBackUrl">CallBack URL</param>
        /// <param name="ToMail">To Email</param>
        /// <param name="FirstName">First Name</param>
        /// <param name="LastName">Last Name</param>
        /// <returns>Populated body</returns>
        private string PopulateBody(string callBackUrl, string ToMail, string FirstName, string LastName)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/MailTemplates/ForgotPassword.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{ApplicationUrl}", ProjectSession.SiteUrlBase);
            body = body.Replace("{FirstName}", FirstName);
            body = body.Replace("{LastName}", LastName);
            body = body.Replace("{PasswordLink}", callBackUrl);
            return body;
        }

        //public bool SendEmailForForgotPassword(int userID)
        //{
        //    string FailReason = string.Empty;
        //    EmailInfo emailInfo = new EmailInfo();
        //    var usr = _user.GetUserById(userID);
        //    FormBot.Entity.User uv = DBClient.DataTableToList<FormBot.Entity.User>(usr.Tables[0]).FirstOrDefault();

        //    var provider = new MachineKeyProtectionProvider();
        //    UserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(
        //        provider.Create("EmailConfirmation"));

        //    var code = UserManager.GeneratePasswordResetToken(uv.AspNetUserId);
        //    emailInfo.TemplateID = 9;
        //    emailInfo.FirstName = uv.FirstName;
        //    emailInfo.LastName = uv.LastName;
        //    emailInfo.PasswordLink = Url.Action("ResetPassword", "Account",
        //        new { UserId = uv.AspNetUserId, code = code }, protocol: Request.Url.Scheme);

        //    SMTPDetails smtpDetail = new SMTPDetails();
        //    smtpDetail.MailFrom = ProjectSession.MailFrom;
        //    smtpDetail.SMTPUserName = ProjectSession.SMTPUserName;
        //    smtpDetail.SMTPPassword = ProjectSession.SMTPPassword;
        //    smtpDetail.SMTPHost = ProjectSession.SMTPHost;
        //    smtpDetail.SMTPPort = Convert.ToInt32(ProjectSession.SMTPPort);
        //    smtpDetail.IsSMTPEnableSsl = ProjectSession.IsSMTPEnableSsl;

        //    EmailTemplate emailTempalte = _emailBAL.GetEmailTemplateByID(emailInfo.TemplateID);
        //    string body = _emailBAL.GetEmailBody(emailInfo, emailTempalte);

        //    bool status = MailHelper.SendMail(smtpDetail, uv.Email, null, null, emailTempalte.Subject, body, null, true, ref FailReason, false);
        //    return status;
        //}

        public bool SendEmailForForgotPassword(EmailTemplate eTemplate, string ToMail)
        {
            string FailReason = string.Empty;
            bool status = false;
            //SMTPDetails smtpDetail = new SMTPDetails();
            //smtpDetail.MailFrom = ProjectSession.MailFrom;
            //smtpDetail.SMTPUserName = ProjectSession.SMTPUserName;
            //smtpDetail.SMTPPassword = ProjectSession.SMTPPassword;
            //smtpDetail.SMTPHost = ProjectSession.SMTPHost;
            //smtpDetail.SMTPPort = Convert.ToInt32(ProjectSession.SMTPPort);
            //smtpDetail.IsSMTPEnableSsl = ProjectSession.IsSMTPEnableSsl;
            try
            {
                if (eTemplate != null && !string.IsNullOrEmpty(eTemplate.Content))
                {
                    QueuedEmail objQueuedEmail = new QueuedEmail();
                    objQueuedEmail.FromEmail = ProjectSession.MailFrom;
                    objQueuedEmail.Body = eTemplate.Content;
                    objQueuedEmail.Subject = eTemplate.Subject;
                    objQueuedEmail.ToEmail = ToMail;
                    objQueuedEmail.CreatedDate = DateTime.Now;
                    objQueuedEmail.ModifiedDate = DateTime.Now;
                    _emailBAL.InsertUpdateQueuedEmail(objQueuedEmail);
                    status = true;

                    //SMTPDetails smtpDetail = new SMTPDetails();
                    //smtpDetail.MailFrom = ConfigurationManager.AppSettings["MailFrom"];
                    //smtpDetail.SMTPUserName = ConfigurationManager.AppSettings["SMTPUserName"];
                    //smtpDetail.SMTPPassword = ConfigurationManager.AppSettings["SMTPPassword"];
                    //smtpDetail.SMTPHost = ConfigurationManager.AppSettings["SMTPHost"];
                    //smtpDetail.SMTPPort = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);
                    //smtpDetail.IsSMTPEnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSMTPEnableSsl"]);
                    //status = MailHelper.SendMail(smtpDetail, ToMail, null, null, eTemplate.Subject, eTemplate.Content, null, true, ref FailReason, false);
                }
            }
            catch (Exception ex)
            {
                FailReason = ex.Message;
            }
            //bool status = MailHelper.SendMail(smtpDetail, ToMail, null, null, eTemplate.Subject, eTemplate.Content, null, true, ref FailReason, false);
            return status;

        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (UserManager != null)
                {
                    UserManager.Dispose();
                    UserManager = null;
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Get ClientNumber On RAM Change
        /// </summary>
        /// <param name="userTypeId">userTypeId</param>
        /// <param name="resellerId">resellerId</param>
        /// <param name="userId">userId</param>
        /// <param name="existClientNumber">existClientNumber</param>
        /// <param name="solarCompanyId">solarCompanyId</param>
        /// <returns>JsonResult</returns>
        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetClientNumberOnRAMChange(int userTypeId, int resellerId, int userId, string existClientNumber = null, int solarCompanyId = 0)
        {
            List<KeyValuePair<string, string>> clientNumberUserType = GetClientNumber(userTypeId, resellerId, userId, existClientNumber, solarCompanyId);
            string clientNumber = clientNumberUserType.Where(a => a.Key == "ClientNumber").FirstOrDefault().Value;
            string userTypeIdOfPrefix = clientNumberUserType.Where(a => a.Key == "UserTypeIdOfPrefix").FirstOrDefault().Value;
            return Json(new { clientNumber = clientNumber, userTypeIdOfPrefix = userTypeIdOfPrefix }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GetClientNumber
        /// </summary>
        /// <param name="userTypeId">userTypeId</param>
        /// <param name="resellerId">resellerId</param>
        /// <param name="userId">userId</param>
        /// <param name="existClientNumber">existClientNumber</param>
        /// <param name="solarCompanyId">solarCompanyId</param>
        /// <returns>list</returns>
        public List<KeyValuePair<string, string>> GetClientNumber(int userTypeId, int resellerId, int userId, string existClientNumber = null, int solarCompanyId = 0)
        {
            List<KeyValuePair<string, string>> clientNumberUserType = new List<KeyValuePair<string, string>>();

            if (resellerId > 0)
            {
                DataSet dsClientNumber = _user.GetClientNumber(userTypeId, resellerId, userId, existClientNumber, solarCompanyId);
                if (dsClientNumber != null && dsClientNumber.Tables.Count > 0 && dsClientNumber.Tables[0] != null && dsClientNumber.Tables[0].Rows.Count > 0)
                {
                    string clientNumber = dsClientNumber.Tables[0].Rows[0]["ClientNumber"].ToString();
                    string userTypeIdOfPrefix = dsClientNumber.Tables[0].Rows[0]["UserTypeIdOfPrefix"].ToString();
                    clientNumberUserType.Add(new KeyValuePair<string, string>("ClientNumber", clientNumber));
                    clientNumberUserType.Add(new KeyValuePair<string, string>("UserTypeIdOfPrefix", userTypeIdOfPrefix));
                }
            }
            return clientNumberUserType;
        }

        public bool GetUserNameByEmailId(string email, ref bool isEmailExist)
        {
            string FailReason = string.Empty;
            DataTable dtAccountDetail = _user.GetAcccountDetailByEmail(email);
            string firstName = string.Empty;
            string lastName = string.Empty;
            string body = string.Empty;
            if (dtAccountDetail != null && dtAccountDetail.Rows.Count > 0)
            {
                firstName = dtAccountDetail.Rows[0]["FirstName"].ToString();
                lastName = dtAccountDetail.Rows[0]["LastName"].ToString();

                body = body + "Usertype | <strong>Username</strong><br />";

                for (int i = 0; i < dtAccountDetail.Rows.Count; i++)
                {
                    //body = string.IsNullOrEmpty(body) ? body : body + "<tr><td><br /></td></tr>";
                    body = body + dtAccountDetail.Rows[i]["UserTypeName"] + " | " + "<strong>" + dtAccountDetail.Rows[i]["UserName"] + "</strong><br />";
                    //body = body + "<tr><td><strong>UserType </strong></td><td>" + dtAccountDetail.Rows[i]["UserTypeName"] + "</td></tr><tr><td><strong>UserName </strong></td><td>" + dtAccountDetail.Rows[i]["UserName"] + " </td></tr>";
                }
            }
            else
            {
                isEmailExist = false;
                return false;
            }

            // for local set 50 
            EmailTemplate eTemplate = _emailBAL.GetEmailTemplateByID(55);
            EmailInfo emailInfo = new EmailInfo();
            emailInfo.FirstName = firstName;
            emailInfo.LastName = lastName;
            emailInfo.Details = body;
            eTemplate.Content = _emailBAL.GetEmailBody(emailInfo, eTemplate);

            bool status = _emailBAL.GeneralComposeAndSendMail(eTemplate, email);
            //SMTPDetails smtpDetail = new SMTPDetails();
            //smtpDetail.MailFrom = ConfigurationManager.AppSettings["MailFrom"];
            //smtpDetail.SMTPUserName = ConfigurationManager.AppSettings["SMTPUserName"];
            //smtpDetail.SMTPPassword = ConfigurationManager.AppSettings["SMTPPassword"];
            //smtpDetail.SMTPHost = ConfigurationManager.AppSettings["SMTPHost"];
            //smtpDetail.SMTPPort = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);
            //smtpDetail.IsSMTPEnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSMTPEnableSsl"]);
            //bool status = MailHelper.SendMail(smtpDetail, email, null, null, eTemplate.Subject, eTemplate.Content, null, true, ref FailReason, false);
            return status;
            //if (status)
            //    this.ShowMessage(SystemEnums.MessageType.Success, "Email has been sent successfully.", true);
            //else
            //    this.ShowMessage(SystemEnums.MessageType.Error, "Sorry! unable to send email.", true);
        }

    }
}

using FormBot.BAL;
using FormBot.BAL.Service;
using FormBot.BAL.Service.SpvLogin;
using FormBot.Email;
using FormBot.Entity;
using FormBot.Helper;
using FormBot.SPVMain.Infrastructure;
using FormBot.SPVMain.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FormBot.SPVMain.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        #region Properties
    
        public UserManager<ApplicationUser> UserManager { get; private set; }

        private readonly ISpvLoginBAL _login;

        private readonly ISpvUserBAL _spvUser;

        #endregion Properties

        #region Constructor

        public AccountController(ISpvUserBAL user, ISpvLoginBAL login)
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())), user, login)
        {
        }

        public AccountController(UserManager<ApplicationUser> userManager, ISpvUserBAL spvUser, ISpvLoginBAL login)
        {
            UserManager = userManager;
            this._spvUser = spvUser;
            this._login = login;
        }

        #endregion Constructor

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
                var user = UserManager.FindById(User.Identity.GetUserId());


                LoginViewModel model = new LoginViewModel();
                ResellerDetailView resellerDetail = new ResellerDetailView();
                if (user != null)
                {
                    var userDetail = _spvUser.GetSpvUserByAspnetUserId(user.Id);

                    if (!userDetail.IsActive)
                    {
                        this.ShowMessage(SystemEnums.MessageType.Error, "Your account is currently inactive.");
                        Logout(returnUrl);
                        return View("Login", model);
                    }

                    ProjectSession.LoggedInUserId = userDetail.SpvUserId;
                    ProjectSession.LoggedInName = userDetail.FirstName + " " + userDetail.LastName;
                    ProjectSession.RoleId = userDetail.SpvRoleId;
                    ProjectSession.UserTypeId = userDetail.SpvUserTypeId;
                    ProjectSession.LoginCompanyName = id;

                    
                    ProjectSession.Theme = "green";
                    ProjectSession.Logo = "Images/logo.png";
                   

                    ////Email information store
                    FormBot.Entity.Email.EmailSignup emailModel = new Entity.Email.EmailSignup();
                    string xmlString = string.Empty;
                    DataSet lstEmail = _spvUser.LoginSpvUserEmailDetails(ProjectSession.LoggedInUserId); ///// need to ask
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

                    if (ProjectSession.RoleId > 0)
                    {
                         _login.UpdateLastLoginDate(ProjectSession.LoggedInUserId);

                        //if (userDetail.IsFirstLogin)
                        //    return RedirectToAction("MyProfile", "User");

                        var dsMenuActions = _login.GetSpvMenuActionByRoleId(ProjectSession.RoleId);
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
                            if (menuActionView.Controller.ToLower() == "email")
                                return RedirectToAction(menuActionView.Action, menuActionView.Controller, new { area = "Email" });
                            else
                                return RedirectToAction(menuActionView.Action, menuActionView.Controller, new { area = "" });

                        }
                        else
                        {
                            this.ShowMessage(SystemEnums.MessageType.Error, "You don't have system access.");
                            return View("Login", model);
                        }
                    }
                    else
                    {
                        return RedirectToLocal(returnUrl);
                    }
                }
                return View(model);
            }
            else
            {
                   LoginViewModel loginView = new LoginViewModel();
                
                    loginView.Theme = "green";
                    ProjectSession.Theme = "green";
                    ProjectSession.LoginCompanyName = "";
                    ProjectSession.Logo = "Images/logo.png";
                    loginView.Logo = "Images/logo.png";
                    loginView.isActiveDiv = 0;
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
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser();
                //ResellerDetailView resellerDetail = new ResellerDetailView();

                if (!string.IsNullOrEmpty(id))
                    model.CompanyName = id;

                  ProjectSession.Theme = "green";
                //ProjectSession.Logo = ProjectSession.SiteUrlBase + "Images/logo.png";
                user = await UserManager.FindAsync(model.Username, model.Password);

                if (user != null)
                {
                    var userDetail = _spvUser.GetSpvUserByAspnetUserId(user.Id);

                    if (!userDetail.IsActive)
                    {
                        this.ShowMessage(SystemEnums.MessageType.Error, "Your account is currently inactive.");
                        Logout(returnUrl);
                        return View("Login", model);
                    }


                    await SignInAsync(user, model.RememberMe);
                    //ProjectSession.IsNewViewer = userDetail.IsNewViewer;
                    StoreSessionInfo(userDetail, id);
                   
                    //Email information store		             
                     StoreEmailInfo();

                        if (ProjectSession.RoleId > 0)
                    {
                        _login.UpdateLastLoginDate(ProjectSession.LoggedInUserId);

                        //if (userDetail.IsFirstLogin)
                        //    return RedirectToAction("MyProfile", "User");

                        var dsMenuActions = _login.GetSpvMenuActionByRoleId(ProjectSession.RoleId);
                        if (dsMenuActions != null && dsMenuActions.Tables[0].Rows.Count > 0)
                        {
                            List<MenuActionView> MenuActionList = DBClient.DataTableToList<MenuActionView>(dsMenuActions.Tables[0]);
                            MenuActionView menuActionView = MenuActionList.FirstOrDefault();
                            if (menuActionView.Controller.ToLower().Contains("home"))
                            {
                                HttpContext.Response.Cookies.Set(new HttpCookie("menuname") { Value = "Home" });
                            }
                            else
                            {
                                HttpContext.Response.Cookies.Set(new HttpCookie("menuname") { Value = "createnew" });
                            }
                            if (menuActionView.Controller.ToLower() == "email")
                                return RedirectToAction(menuActionView.Action, menuActionView.Controller, new { area = "Email" });
                            else
                                return RedirectToAction(menuActionView.Action, menuActionView.Controller, new { area = "" });

                        }
                        else
                        {
                            this.ShowMessage(SystemEnums.MessageType.Error, "You don't have system access.");
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
                    this.ShowMessage(SystemEnums.MessageType.Error, "Invalid username or password.");
                    return View("Login", model);
                }

            }
            // If we got this far, something failed, redisplay form
            return View(model);
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
        }

        /// <summary>
        /// Store Email Info
        /// </summary>
        public void StoreEmailInfo()
        {
            FormBot.Entity.Email.EmailSignup emailModel = new Entity.Email.EmailSignup();
            string xmlString = string.Empty;
            DataSet lstEmail = _spvUser.LoginSpvUserEmailDetails(ProjectSession.LoggedInUserId);
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
            var userDetail = _spvUser.GetSPVUsersByFSA(Convert.ToInt32(userID));

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
            if (ProjectSession.RoleId > 0)
            {
                var dsMenuActions = _login.GetSpvMenuActionByRoleId(ProjectSession.RoleId);
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
                    if (menuActionView.Controller.ToLower() == "email")
                        return Json(new { status = true, action = menuActionView.Action, controller = menuActionView.Controller, area = "Email" }, JsonRequestBehavior.AllowGet);

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

        #endregion Events

        #region Helpers

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

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

        /// <summary>
        /// Store Session Info
        /// </summary>
        /// <param name="userDetail"></param>
        /// <param name="id"></param>
        public void StoreSessionInfo(FormBot.Entity.SpvUser userDetail, string id = null)
        {
            ProjectSession.LoggedInUserId = userDetail.SpvUserId;
            ProjectSession.LoggedInName = userDetail.FirstName + " " + userDetail.LastName;

            ProjectSession.RoleId = userDetail.SpvRoleId;
            ProjectSession.UserTypeId = userDetail.SpvUserTypeId;
            ProjectSession.IsresetPwd = userDetail.IsResetPwd;
            ProjectSession.SpvManufacturerName = userDetail.ManufacturerName;
        }


        #endregion Helpers

    }
}
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using FormBot.Helper;
using System.Collections.Generic;
using System.Linq;
using FormBot.BAL.Service;
using FormBot.BAL;
using System.Web.Routing;
using FormBot.Entity;
using System;
using System.ComponentModel;
using System.Collections;
using System.IO.Compression;
using System.Net.Http;
namespace FormBot.Main.Controllers
{
    public class BaseController : Controller
    {
        [HttpPost]
        public JsonResult KeepSessionAlive()
        {
            return new JsonResult { Data = "Success" };
        }

        /// <summary>
        /// To handle session timeout
        /// </summary>
        /// <param name="filterContext">filter Context</param>
        //  protected override void OnAuthorization(AuthorizationContext filterContext)
        public void HandleSessionTimeOut(AuthorizationContext filterContext)
        {
            OnAuthorization(filterContext);
            UrlHelper urlHelper = new UrlHelper(filterContext.RequestContext);
            string rootURL = string.Empty;

            HttpCookie httpCookie = filterContext.HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (httpCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(httpCookie.Value);
                var newTicket = FormsAuthentication.RenewTicketIfOld(authTicket);
                if (!newTicket.Expired)
                {
                    if (ProjectSession.LoggedInUserId <= 0)
                    {
                        rootURL = string.Empty;
                        filterContext.Result = new RedirectResult(urlHelper.Action("Logout", "Account", new { area = string.Empty, id = string.Empty, returnUrl = HttpUtility.UrlEncode(filterContext.HttpContext.Request.RawUrl) }));
                    }
                    else
                    {
                        string encryptedTicket = FormsAuthentication.Encrypt(newTicket);

                        httpCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                        httpCookie.Path = FormsAuthentication.FormsCookiePath;
                        filterContext.HttpContext.Response.Cookies.Add(httpCookie);
                    }
                }
                else
                {
                    rootURL = string.Empty;
                    if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
                    {
                        filterContext.Result = new RedirectResult(urlHelper.Action("Logout", "Account", new { area = string.Empty, id = string.Empty, returnUrl = HttpUtility.UrlEncode(filterContext.HttpContext.Request.RawUrl) }));
                    }
                }
            }
            else
            {
                //Redirects to login page for UnAuthentic Request
                rootURL = string.Empty;
                if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
                {
                    filterContext.Result = new RedirectResult(urlHelper.Action("Logout", "Account", new { area = string.Empty, returnUrl = string.Empty }));
                }
            }
        }
    }

    public class CheckSessionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            UrlHelper urlHelper = new UrlHelper(filterContext.RequestContext);
            string rootURL = string.Empty;
            if (ProjectSession.LoggedInUserId <= 0)
            {
                rootURL = string.Empty;
                filterContext.Result = new RedirectResult(urlHelper.Action("Logout", "Account", new { area = string.Empty, id = string.Empty, returnUrl = HttpUtility.UrlEncode(filterContext.HttpContext.Request.RawUrl) }));
            }
        }

    }

    /// <summary>
    /// Class UserAuthorizationAttribute.
    /// </summary>
    public class UserAuthorizationAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Override OnAuthorization method to check access rights
        /// </summary>
        /// <param name="filterContext">filter Context</param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            UrlHelper urlHelper = new UrlHelper(filterContext.RequestContext);

            //User isn't logged in
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
                //filterContext.Result = new RedirectResult(urlHelper.Action("Login", "Account", new { Area = string.Empty, id = string.Empty }));
                filterContext.Result = new RedirectResult(urlHelper.Action("Logout", "Account", new { area = string.Empty, id = string.Empty, returnUrl = HttpUtility.UrlEncode(filterContext.HttpContext.Request.RawUrl) }));


            //For session time out
            BaseController objBase = new BaseController();
            objBase.HandleSessionTimeOut(filterContext);

            //User is logged in but has no access
            if (!UserAuthorizationAttribute.HasAccessRights(filterContext))
            {
                if (!string.IsNullOrEmpty(ProjectSession.LoginCompanyName))
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { id = ProjectSession.LoginCompanyName, area = string.Empty, controller = "Account", action = "Login" }));
                else
                    //filterContext.Result = new RedirectResult(urlHelper.Action("Login", "Account", new { Area = string.Empty, id = string.Empty }));
                    filterContext.Result = new RedirectResult(urlHelper.Action("Logout", "Account", new { area = string.Empty, id = string.Empty, returnUrl = HttpUtility.UrlEncode(filterContext.HttpContext.Request.RawUrl) }));
            }

            UserBAL obj = new UserBAL();
            var user = obj.GetUserById(ProjectSession.LoggedInUserId);
            var userDetail = DBClient.DataTableToList<User>(user.Tables[0]);
            if (userDetail != null && userDetail.Count > 0)
            {
                if (userDetail[0].IsDeleted)
                    filterContext.Result = new RedirectResult(urlHelper.Action("Logout", "Account", new { Area = string.Empty, id = string.Empty }));
                if (!userDetail[0].IsActive)
                    filterContext.Result = new RedirectResult(urlHelper.Action("Logout", "Account", new { Area = string.Empty, id = string.Empty }));
            }
        }

        /// <summary>
        /// Custom authorization
        /// </summary>
        /// <param name="filterContext">filter context</param>
        /// <returns>static bool</returns>
        public static bool HasAccessRights(AuthorizationContext filterContext)
        {
            bool isRoleExists=false ;
            if (HttpContext.Current != null && HttpContext.Current.Handler is System.Web.Mvc.MvcHandler)
            {
                var handler = HttpContext.Current.Handler as System.Web.Mvc.MvcHandler;
                var controller = handler.RequestContext.RouteData.Values["controller"].ToString();
                var action = handler.RequestContext.RouteData.Values["action"].ToString();

                IRoleBAL role = new RoleBAL();
                if (action == SystemEnums.JobDetails.UploadDocumentCES.ToString()||action==SystemEnums.JobDetails.UploadDocument.ToString()|| action==SystemEnums.JobDetails.UploadDocumentOther.ToString()||action==SystemEnums.JobDetails.UploadSTCDocument.ToString()||
                    action==SystemEnums.JobDetails.UploadReferencePhoto.ToString()||action==SystemEnums.JobDetails.DownloadSTCDocument.ToString()||
                    action==SystemEnums.JobDetails.DownloadJobPhotos.ToString()||action==SystemEnums.JobDetails.DownloadJobDocuments.ToString()||
                    action==SystemEnums.JobDetails.DownloadAllDocumentsNew.ToString()||action==SystemEnums.JobDetails.DeleteDocumentNew.ToString()||
                    action==SystemEnums.JobDetails.DeleteDocument.ToString()||action==SystemEnums.JobDetails.DeleteCheckListPhotos.ToString()
                    ||action==SystemEnums.JobDetails.AddOtherDocuments.ToString() || action==SystemEnums.JobDetails.GenerateFullJobPack.ToString())
                {
                    if (ProjectSession.UserTypeId == 8)
                    {
                        isRoleExists= CheckCustomAuthorization(controller, action,filterContext);
                    }
                    else
                    {
                       isRoleExists=true;
                    }

                }
                else
                {
                    isRoleExists = CheckCustomAuthorization(controller, action, filterContext);
                }
                

               
            }

            return isRoleExists;
        }
        public static bool CheckCustomAuthorization(string controller, string action,AuthorizationContext filterContext)
        {
            bool isRoleExists = false;
            IRoleBAL role = new RoleBAL();
            var dsMenus = role.CustomAuthorization(ProjectSession.RoleId, controller, action);
            List<MenuIdList> menuIds = DBClient.DataTableToList<MenuIdList>(dsMenus.Tables[0]);
            var menus = menuIds.Select(s => s.MenuId).ToList();
            List<CurrentMenu> currentMenu = DBClient.DataTableToList<CurrentMenu>(dsMenus.Tables[1]);
            var currentMenuId = currentMenu.Select(s => s.currentMenuId).FirstOrDefault();
            isRoleExists = menus.Contains(currentMenuId);

            var parentId = menuIds.Where(d => d.MenuId == currentMenuId).Select(x => x.ParentId).FirstOrDefault();
            //TODO : Pankaj - Alter Name for this RoleMenu Enum.
            filterContext.Controller.TempData[SystemEnums.TempDataKey.RoleMenu.ToString()] = menuIds.Where(d => d.ParentId == menuIds.Where(data => data.MenuId == currentMenuId).Select(x => x.ParentId).FirstOrDefault()).ToList();
            filterContext.Controller.TempData[SystemEnums.TempDataKey.IsProfile.ToString()] = menuIds.Where(d => d.MenuId == (int)SystemEnums.MenuId.profile).ToList();
            filterContext.Controller.TempData[SystemEnums.TempDataKey.IsSendRequest.ToString()] = menuIds.Where(d => d.MenuId == (int)SystemEnums.MenuId.SendRequestToElectrician).ToList();
            filterContext.Controller.TempData[SystemEnums.TempDataKey.SCAComplianceCheck.ToString()] = menuIds.Where(d => d.MenuId == (int)SystemEnums.MenuId.SCAComplianceChek).ToList();
            filterContext.Controller.TempData[SystemEnums.TempDataKey.SEComplianceCheck.ToString()] = menuIds.Where(d => d.MenuId == (int)SystemEnums.MenuId.SEComplianceChek).ToList();
            filterContext.Controller.TempData[SystemEnums.TempDataKey.RAMViewAllJob.ToString()] = menuIds.Where(d => d.MenuId == (int)SystemEnums.MenuId.AllScaJobView).ToList();
            filterContext.Controller.TempData[SystemEnums.TempDataKey.SCOViewAssignJob.ToString()] = menuIds.Where(d => d.MenuId == (int)SystemEnums.MenuId.ShowOnlyAssignJobsSCO).ToList();
            filterContext.Controller.TempData[SystemEnums.TempDataKey.AutoRECUpload.ToString()] = menuIds.Where(d => d.MenuId == (int)SystemEnums.MenuId.AutoRECUpload).ToList();

            return isRoleExists;
        }
    }
   
    public class GeneralAuthorization : AuthorizeAttribute
    {
        /// <summary>
        /// Override OnAuthorization method to check access rights
        /// </summary>
        /// <param name="filterContext">filter Context</param>
        public override void OnAuthorization(AuthorizationContext filterContext)

        {
            UrlHelper urlHelper = new UrlHelper(filterContext.RequestContext);
            

            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
                //filterContext.Result = new RedirectResult(urlHelper.Action("Login", "Account", new { Area = string.Empty, id = string.Empty }));
                filterContext.Result = new RedirectResult(urlHelper.Action("Logout", "Account", new { area = string.Empty, id = string.Empty, returnUrl = HttpUtility.UrlEncode(filterContext.HttpContext.Request.RawUrl) }));


            //For session time out
            BaseController objBase = new BaseController();
            objBase.HandleSessionTimeOut(filterContext);
          
            

            UserBAL obj = new UserBAL();
            var user = obj.GetUserById(ProjectSession.LoggedInUserId);
            var userDetail = DBClient.DataTableToList<User>(user.Tables[0]);
            if (userDetail != null && userDetail.Count > 0)
            {
                if (userDetail[0].IsDeleted)
                    filterContext.Result = new RedirectResult(urlHelper.Action("Logout", "Account", new { Area = string.Empty, id = string.Empty }));
                if (!userDetail[0].IsActive)
                    filterContext.Result = new RedirectResult(urlHelper.Action("Logout", "Account", new { Area = string.Empty, id = string.Empty }));
            }
        }

       
    }

    [Serializable]
    public class MenuIdList
    {
        public int MenuId { get; set; }
        public int ParentId { get; set; }
    }

    public class CurrentMenu
    {
        public int currentMenuId { get; set; }
    }

    public static class EnumExtensions
    {

        // This extension method is broken out so you can use a similar pattern with 
        // other MetaData elements in the future. This is your base method for each.
        public static T GetAttribute<T>(this Enum value) where T : Attribute
        {
            var type = value.GetType();
            var memberInfo = type.GetMember(value.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
            return (T)attributes[0];
        }

        // This method creates a specific call to the above method, requesting the
        // Description MetaData attribute.
        public static string ToName(this Enum value)
        {
            var attribute = value.GetAttribute<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }

        /// <summary>
        /// Get Postal Address EnumList for Dropdown bind
        /// </summary>
        /// <returns>IEnumerable</returns>
        public static IEnumerable GetPostalAddressEnumList()
        {
            return from SystemEnums.PostalAddressType s in Enum.GetValues(typeof(SystemEnums.PostalAddressType))
                   select new { ID = s.GetHashCode(), Name = s.ToName() };
        }


    }

    public class GZipOrDeflateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting
             (ActionExecutingContext filterContext)
        {
            string acceptencoding = filterContext.HttpContext.Request.Headers["Accept-Encoding"];

            if (!string.IsNullOrEmpty(acceptencoding))
            {
                acceptencoding = acceptencoding.ToLower();
                var response = filterContext.HttpContext.Response;
                if (acceptencoding.Contains("gzip"))
                {
                    response.AppendHeader("Content-Encoding", "gzip");
                    response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
                }
                else if (acceptencoding.Contains("deflate"))
                {
                    response.AppendHeader("Content-Encoding", "deflate");
                    response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
                }
            }
        }
    }
}
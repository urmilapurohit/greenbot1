using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using System.Linq;
using System.Web;
using System;
using System.EnterpriseServices;
using FormBot.Helper;
using FormBot.Entity;
using System.Data.SqlClient;
using System.Data;
using FormBot.DAL;
using FormBot.BAL;
using FormBot.BAL.Service;
using System.Web.UI;

namespace FormBot.Main
{
    public static class MessageExtensions
    {
        #region Properties
        private static readonly IRoleBAL _role;
        #endregion

        #region Constructor
        static MessageExtensions()
        {
            _role = new RoleBAL();
        }
        #endregion

        /// <summary>
        /// Dynamic menu binding
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <returns>html string</returns>
        public static HtmlString DynamicMenuBinding(this HtmlHelper htmlHelper)
        {
            Page page = new Page();

            var dsMenus = _role.DynamicMenuBinding(ProjectSession.RoleId);
            
            List<MenuView> menus = DBClient.DataTableToList<MenuView>(dsMenus.Tables[0]);
            List<MenuView> menuDeails = DBClient.DataTableToList<MenuView>(dsMenus.Tables[1]);
            List<MenuView> menuDeailsWithoutSubMenu = DBClient.DataTableToList<MenuView>(dsMenus.Tables[2]);
            var menuHtml = DynamicMenuHtml(page, htmlHelper, menus, menuDeails, menuDeailsWithoutSubMenu);
            ProjectSession.DynamicMenuBinding = menuHtml.ToString();
            return MvcHtmlString.Create(menuHtml.ToString());
        }

        /// <summary>
        /// Prepare html for menu
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="menus">The menus.</param>
        /// <param name="menuDetails">The menu details.</param>
        /// <param name="menuDeailsWithoutSubMenu">The menu deails without sub menu.</param>
        /// <returns>builder object</returns>
        public static StringBuilder DynamicMenuHtml(Page page, HtmlHelper htmlHelper, List<MenuView> menus, List<MenuView> menuDetails, List<MenuView> menuDeailsWithoutSubMenu)
        {
            var menuDisplayName = string.Empty;
            var output = new StringBuilder();
            string imagePath = ProjectConfiguration.ProjectImagePath;
            output.Append(@"<ul class='menu'>");
            if (!ProjectSession.IsSSCReseller)
            {
                menuDetails = menuDetails.Where(x => x.MenuId != 90).ToList();
            }

            if (ProjectSession.UserTypeId == 8 && ProjectSession.IsSubContractor)
            {
                menuDetails = menuDetails.Where(x => x.MenuId != 96).ToList();
            }

            foreach (var menu in menus)
            {
                var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
                string url = string.Empty, name = string.Empty;
                if (menu.WithoutSubMenu)
                {
                    var image = "<img src=\"https://cdn.greenbot.com.au/" + menu.Image + "\" alt='Create New' />";
                    MenuView menuView = menuDeailsWithoutSubMenu.Where(x => x.ParentID == menu.MenuId).FirstOrDefault();
                    if (menuView != null)
                    {
                        if (menu.DisplayName.ToLower() == "email")
                        {
                            url = urlHelper.Action(menuView.Action, menuView.Controller, new { area = "email" });
                        }
                        else
                        {
                            url = urlHelper.Action(menuView.Action, menuView.Controller, new { area = "" });
                        }

                        output.Append(@"<li> <a class='navlink' href='" + url + "' id='" + menu.Title + "'> <i class='icon sprite-img " + menu.Image + "'> </i>" + menu.DisplayName + " </a></li>");
                    }
                    
                }
                else
                {
                    var image = "<img src=\"https://cdn.greenbot.com.au/" + menu.Image + "\" alt='Create New' />";

                    if (menu.DisplayName.ToLower() == "users")
                    {
                        output.Append(@"<li id='" + menu.Title + "'> <a class='navlink' href='javascript:void(0)' id='" + menu.Title + "'> <i class='icon sprite-img " + menu.Image + "'> </i>" + menu.DisplayName + " </a>");
                    }
                    else
                    {
                        output.Append(@"<li id='" + menu.Title + "'> <a class='navlink' href='#' id='" + menu.Title + "'> <i class='icon sprite-img " + menu.Image + "'> </i>" + menu.DisplayName + " </a>");
                    }

                    output.Append(@"<ul>");
                    foreach (var menuDetail in menuDetails)
                    {
                        url = urlHelper.Action(menuDetail.Action, menuDetail.Controller, new { id = "", area = "" });
                        if (menu.MenuId == menuDetail.SubMenuParentID)
                        {
                            if (menuDetail.Title == "CreateJob")
                            {
                                output.Append(@"<li id='" + menuDetail.Title + "' class=CreateNewJob><a url='" + url + "' id='" + menu.Title + "'>" + menuDetail.DisplayName + "</a></li>");
                            }
							else if (menuDetail.Title == "CreateVEECs")
							{
								output.Append(@"<li id='" + menuDetail.Title + "'><a url='" + url + "' id='" + menu.Title + "'>" + menuDetail.DisplayName + "</a></li>");
							}
							else
                            {
                                output.Append(@"<li id='" + menuDetail.Title + "'><a href='" + url + "' id='" + menu.Title + "'>" + menuDetail.DisplayName + "</a></li>");
                            }
                            
                        }
                    }
                    output.Append(@"</ul>");
                    output.Append(@"</li>");
                }

            }
            output.Append(@"</ul>");

            output.Append(@"<script>
            if($('.menu').find('li [id=" + ProjectConfiguration.GetMenuName + "]').length>2) { $('.menu').find('li [id=" + ProjectConfiguration.GetMenuName + "]').first().parent('li').addClass('active');} else {{ $('.menu').find('li [id=" + ProjectConfiguration.GetMenuName + "]').parent('li').addClass('active');}}</script>");

            return output;
        }

        /// <summary>
        /// Dynamic role checkbox list binding
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="userType">Type of the user.</param>
        /// <returns>Html String</returns>
        public static HtmlString DynamicRoleBinding(this HtmlHelper htmlHelper, int userType = 1)
        {
            var roleHtml = DynamicRoleHtml(htmlHelper, userType);
            return MvcHtmlString.Create(roleHtml.ToString());
        }

        /// <summary>
        /// Prepare html for role checkboxes list
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="userType">Type of the user.</param>
        /// <returns>String Builder</returns>
        public static StringBuilder DynamicRoleHtml(HtmlHelper htmlHelper, int userType)
        {
            var output = new StringBuilder();
            if (ProjectSession.UserTypeId == 4 || ProjectSession.UserTypeId == 6)
            {
                userType = 8;
            }
            else if (ProjectSession.UserTypeId == 2)
            {
                userType = 5;
            }

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, userType));
            DataSet dsMenus = CommonDAL.ExecuteDataSet("Menu_GetMenuList", sqlParameters.ToArray());
            List<MenuView> menus = DBClient.DataTableToList<MenuView>(dsMenus.Tables[0]);
            List<MenuView> menuActions = DBClient.DataTableToList<MenuView>(dsMenus.Tables[1]);
            if (ProjectSession.UserTypeId == 2)
            {
                List<int> lstMenuID = new List<int>() { 60, 62, 64, 67, 71, 74, 75, 76, 77, 80, 81, 84, 85, 86, 87, 91, 92 };
                menuActions = menuActions.Where(x => !lstMenuID.Contains(x.MenuId)).ToList();
            }
            if (ProjectSession.UserTypeId == 4 || ProjectSession.UserTypeId==6)
            {
                List<int> lstMenuID = new List<int>() { 185,186,188 };
                menus = menus.Where(x => x.MenuId != 187).ToList();
                menuActions = menuActions.Where(x => !lstMenuID.Contains(x.MenuId)).ToList();
            }
            else if (ProjectSession.UserTypeId == 6 && ProjectSession.IsSubContractor)
            {
                List<int> lstMenuID = new List<int>() { 60, 62, 71, 74, 76, 77, 86, 87, 88, 90, 91, 92, 96, 99 };
                menus = menus.Where(x => x.MenuId != 88 && x.MenuId != 99).ToList();
                menuActions = menuActions.Where(x => !lstMenuID.Contains(x.MenuId)).ToList();
            }

            /* Remove saas menu entries for usertype other then 1*/
            if (ProjectSession.UserTypeId != 1)
            {
                List<int> ListToremove = new List<int> { 201, 204, 205, 206, 211, 212, 214 };
                menus = menus.Where(x => !ListToremove.Contains(x.MenuId)).ToList();
                menuActions = menuActions.Where(x => !ListToremove.Contains(x.MenuId)).ToList();
            }

            foreach (var menu in menus)
            {
                output.Append(@"<div  class='col-sm-3 col-md-2'><label class='control-label'>" + menu.DisplayName + ":" + "</label> </div>");
                output.Append(@"<div  class='col-sm-9 col-md-10'> <div class='checkbox-box' style=' margin-top:4px;'>");
                foreach (var menuAction in menuActions)
                {
                    if (menu.MenuId == menuAction.ParentID)
                    {
                        if (menuAction.Name.ToLower() != "invisible")
                        {
                            output.Append(@"<input name='RoleIds' class='clsRights' type='checkbox' value=" + menuAction.MenuId + " cat='" + menuAction.CheckboxId + "' style='margin-right:8px;'/>");
                            output.Append(@"<span style='margin-right:10px;'>" + menuAction.Name + "</span>");
                        }
                    }
                }
                output.Append(@"</div></div>");
                output.Append(@"<div  class='clearfix visible-sm-block visible-md-block'> </div>");
            }
            return output;
        }

        /// <summary>
        /// This method will set TempData or ViewData for notification message.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="messageType"></param>
        /// <param name="message"></param>
        /// <param name="showAfterRedirect"></param>
        public static void ShowMessage(this Controller controller, SystemEnums.MessageType messageType, string message,
            bool showAfterRedirect = false)
        {
            string messageTypeKey = messageType.ToString();
            if (showAfterRedirect)
            {
                controller.TempData[messageTypeKey] = message;
            }
            else
            {
                controller.ViewData[messageTypeKey] = message;
            }
        }

        /// <summary>
        /// Render message in view if any notification message has been set to show after redirect or HttpPost or refresh view.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="clearMessage">if set to <c>true</c> [clear message].</param>
        /// <returns>Html String</returns>
        public static HtmlString RenderMessages(this HtmlHelper htmlHelper, bool clearMessage = true)
        {
            string messages = String.Empty;
            ReturnMessage(htmlHelper, SystemEnums.MessageType.Error.ToString(), ref messages);
            ReturnMessage(htmlHelper, SystemEnums.MessageType.Info.ToString(), ref messages);
            ReturnMessage(htmlHelper, SystemEnums.MessageType.Success.ToString(), ref messages);
            ReturnMessage(htmlHelper, SystemEnums.MessageType.Warning.ToString(), ref messages);
            return MvcHtmlString.Create(messages);
        }

        /// <summary>
        /// If ViewData or TempData contains any message to show and type of that message will match with passed parameters
        /// then this will prepare html to render. It will also clear TempData and ViewData.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="messages">The messages.</param>
        public static void ReturnMessage(HtmlHelper htmlHelper, string messageType, ref string messages)
        {
            object message = htmlHelper.ViewContext.ViewData.ContainsKey(messageType)
                ? htmlHelper.ViewContext.ViewData[messageType]
                : htmlHelper.ViewContext.TempData.ContainsKey(messageType)
                    ? htmlHelper.ViewContext.TempData[messageType]
                    : null;
            if (message != null)
            {
                string _class = string.Empty;
                if (messageType == SystemEnums.MessageType.Success.ToString())
                {
                    _class = "alert-success";
                }
                else if (messageType == SystemEnums.MessageType.Error.ToString())
                {
                    _class = "alert-danger";
                }
                else if (messageType == SystemEnums.MessageType.Info.ToString())
                {
                    _class = "information";
                }
                else if (messageType == SystemEnums.MessageType.Warning.ToString())
                {
                    _class = "alert-warning";
                }
                messages = string.Format("<div class=\"alert {0}\">"
                                         + message.ToString()
                                         + "<button type=\"button\" class=\"close\" onclick=\"$(this).parent().hide();\" aria-hidden=\"true\">&times;</button></div>", _class);
                ClearMessages(htmlHelper, messageType);
            }
        }

        public static string ReturnMessage(this Controller controller, string messageType, string message)
        {
            if (message != null)
            {
                string _class = string.Empty;
                if (messageType == SystemEnums.MessageType.Success.ToString())
                {
                    _class = "alert-success";
                }
                else if (messageType == SystemEnums.MessageType.Error.ToString())
                {
                    _class = "alert-danger";
                }
                else if (messageType == SystemEnums.MessageType.Info.ToString())
                {
                    _class = "information";
                }
                else if (messageType == SystemEnums.MessageType.Warning.ToString())
                {
                    _class = "alert-warning";
                }
                message = string.Format("<div class=\"alert {0}\">"
                                         + message.ToString()
                                         + "<button type=\"button\" class=\"close\" onclick=\"$(this).parent().hide();\" aria-hidden=\"true\">&times;</button>", _class);
            }
            return message;
        }

        /// <summary>
        /// Clear TempData and ViewData if any message has been set to show.
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="messageType"></param>
        private static void ClearMessages(HtmlHelper htmlHelper, string messageType)
        {
            htmlHelper.ViewContext.ViewData[messageType] = htmlHelper.ViewContext.TempData[messageType] = null;
        }
    }
}
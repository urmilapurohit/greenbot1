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
        /// <returns></returns>
        public static HtmlString DynamicMenuBinding(this HtmlHelper htmlHelper)
        {

            var dsMenus = _role.DynamicMenuBinding(ProjectSession.RoleId);
            List<MenuView> menus = DBClient.DataTableToList<MenuView>(dsMenus.Tables[0]);

            List<MenuView> menuDeails = DBClient.DataTableToList<MenuView>(dsMenus.Tables[1]);

            List<MenuView> menuDeailsWithoutSubMenu = DBClient.DataTableToList<MenuView>(dsMenus.Tables[2]);

            var menuHtml = DynamicMenuHtml(htmlHelper, menus, menuDeails, menuDeailsWithoutSubMenu);
            return MvcHtmlString.Create(menuHtml.ToString());
        }

        /// <summary>
        /// Prepare html for menu
        /// </summary>
        /// <param name="menuNames"></param>
        /// <param name="menuHtml"></param>
        /// <returns></returns>
        public static StringBuilder DynamicMenuHtml(HtmlHelper htmlHelper, List<MenuView> menus, List<MenuView> menuDetails, List<MenuView> menuDeailsWithoutSubMenu)
        {
            var output = new StringBuilder();

            output.Append(@"<ul class='menu'>");
            foreach (var menu in menus)
            {
                var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
                string url = string.Empty, name = string.Empty;

                if (menu.WithoutSubMenu)
                {
                    foreach (var menuDetail in menuDeailsWithoutSubMenu)
                    {
                        var image = ProjectSession.SiteUrlBase + menu.Image;
                        url = urlHelper.Action(menuDetail.Action, menuDetail.Controller);
                        output.Append(@"<li> <a class='navlink' href='" + url + "' title='Create New'> <i class='icon'> <img src='" + image + "' alt='Create New'> </i> " + menu.DisplayName + " </a></li>");
                    }
                }
                else
                {
                    var image = ProjectSession.SiteUrlBase + menu.Image;
                    if (menu.DisplayName == "Users")
                        output.Append(@"<li class='active'> <a class='navlink' href='javascript:void(0)' title='Create New'> <i class='icon'> <img src='" + image + "' alt='Create New'> </i> " + menu.DisplayName + " </a>");
                    else
                        output.Append(@"<li> <a class='navlink' href='#' title='Create New'> <i class='icon'> <img src='" + image + "' alt='Create New'> </i> " + menu.DisplayName + " </a>");
                    output.Append(@"<ul>");
                    foreach (var menuDetail in menuDetails)
                    {
                        url = urlHelper.Action(menuDetail.Action, menuDetail.Controller);
                        if (menu.MenuId == menuDetail.SubMenuParentID)
                        {
                            if (menuDetail.Controller == "Role")
                                output.Append(@"<li><a href='" + url + "' title=''>Roles</a></li>");
                            else
                                output.Append(@"<li><a href='" + url + "' title=''>" + menuDetail.DisplayName + "</a></li>");
                        }
                    }
                }
                output.Append(@"</ul>");
                output.Append(@"</li>");
            }
            output.Append(@"</ul>");
            return output;
        }


        /// <summary>
        /// Dynamic role checkbox list binding
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <returns></returns>
        public static HtmlString DynamicRoleBinding(this HtmlHelper htmlHelper)
        {
            var roleHtml = DynamicRoleHtml(htmlHelper);
            DynamicRoleHtml(htmlHelper);
            return MvcHtmlString.Create(roleHtml.ToString());
        }

        /// <summary>
        /// Prepare html for role checkboxes list
        /// </summary>
        /// <param name="menuNames"></param>
        /// <param name="menuHtml"></param>
        /// <returns></returns>
        public static StringBuilder DynamicRoleHtml(HtmlHelper htmlHelper)
        {
            var output = new StringBuilder();

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, ProjectSession.UserTypeId));
            DataSet dsMenus = CommonDAL.ExecuteDataSet("Menu_GetMenuList", sqlParameters.ToArray());

            List<MenuView> menus = DBClient.DataTableToList<MenuView>(dsMenus.Tables[0]);
            List<MenuView> menuActions = DBClient.DataTableToList<MenuView>(dsMenus.Tables[1]);


            foreach (var menu in menus)
            {
                output.Append(@"<div  class='col-sm-1'> <div class='form-group'> <label class='control-label'>" + menu.DisplayName + ":" + "</label> </div> </div>");
                output.Append(@"<div  class='col-sm-11' style='float:left;'>  <div class='form-group'>  <div class='checkbox-box' style=' margin-top:4px;'>");
                foreach (var menuAction in menuActions)
                {
                    if (menu.MenuId == menuAction.ParentID)
                    {
                        output.Append(@"<input name='RoleIds' class='clsRights' type='checkbox' value=" + menuAction.MenuId + " cat='" + menuAction.CheckboxId + "' style='margin-right:8px;'/>");
                        output.Append(@"<span style='margin-right:10px;'>" + menuAction.DisplayName + "</span>");
                    }
                }
                output.Append(@"</div></div></div>");
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
        /// <param name="htmlHelper"></param>
        /// <param name="clearMessage"></param>
        /// <returns></returns>
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
        /// <param name="htmlHelper"></param>
        /// <param name="messageType"></param>
        /// <param name="messages"></param>
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
                messages = string.Format("<div class=\"alert {0}\" onclick=\"$(this).fadeOut(3000)\" >"
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
                message = string.Format("<div class=\"alert {0}\" onclick=\"$(this).fadeOut(3000)\" >"
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
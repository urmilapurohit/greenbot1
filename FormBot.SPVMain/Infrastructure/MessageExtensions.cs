
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

namespace FormBot.SPVMain.Infrastructure
{
    public static class MessageExtensions
    {
        #region Properties
        private static readonly ISpvRoleBAL _spvRole;
        #endregion

        #region Constructor
        static MessageExtensions()
        {
            _spvRole = new SpvRoleBAL();
        }
        #endregion

        /// <summary>
        /// Dynamic spvmenu binding
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <returns>html string</returns>
        public static HtmlString DynamicSpvMenuBinding(this HtmlHelper htmlHelper)
        {
            Page page = new Page();

            var dsSpvMenus = _spvRole.DynamicSpvMenuBinding(2);//ProjectSession.RoleId);

            List<SpvMenuView> spvmenus = DBClient.DataTableToList<SpvMenuView>(dsSpvMenus.Tables[0]);
            List<SpvMenuView> spvmenuDeails = DBClient.DataTableToList<SpvMenuView>(dsSpvMenus.Tables[1]);
            List<SpvMenuView> spvmenuDeailsWithoutSubMenu = DBClient.DataTableToList<SpvMenuView>(dsSpvMenus.Tables[2]);
            var spvmenuHtml = DynamicMenuHtml(page, htmlHelper, spvmenus, spvmenuDeails, spvmenuDeailsWithoutSubMenu);
            ProjectSession.DynamicSpvMenuBinding = spvmenuHtml.ToString();
            return MvcHtmlString.Create(spvmenuHtml.ToString());
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
        public static StringBuilder DynamicMenuHtml(Page page, HtmlHelper htmlHelper, List<SpvMenuView> Spvmenus, List<SpvMenuView> SpvmenuDetails, List<SpvMenuView> SpvmenuDeailsWithoutSubMenu)
        {
            var menuDisplayName = string.Empty;
            var output = new StringBuilder();
            string imagePath = ProjectConfiguration.ProjectImagePath;
            output.Append(@"<ul class='menu'>");

            foreach (var Spvmenu in Spvmenus)
            {
                var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
                string url = string.Empty, name = string.Empty;
                if (Spvmenu.WithoutSubMenu)
                {
                    var image = "<img src=\"https://cdn.greenbot.com.au/" + Spvmenu.Image + "\" alt='Create New' />";
                    SpvMenuView SpvmenuView = SpvmenuDeailsWithoutSubMenu.Where(x => x.ParentID == Spvmenu.SpvMenuId).FirstOrDefault();
                    if (SpvmenuView != null)
                    {
                        
                        url = urlHelper.Action(SpvmenuView.Action, SpvmenuView.Controller, new { area = "" });

                        output.Append(@"<li> <a class='navlink' href='" + url + "' id='" + Spvmenu.Title + "'> <i class='icon sprite-img " + Spvmenu.Image + "'> </i>" + Spvmenu.DisplayName + " </a></li>");
                    }

                }
                else
                {
                    var image = "<img src=\"https://cdn.greenbot.com.au/" + Spvmenu.Image + "\" alt='Create New' />";

                    if (Spvmenu.DisplayName.ToLower() == "users")
                    {
                        output.Append(@"<li id='" + Spvmenu.Title + "'> <a class='navlink' href='javascript:void(0)' id='" + Spvmenu.Title + "'> <i class='icon sprite-img " + Spvmenu.Image + "'> </i>" + Spvmenu.DisplayName + " </a>");
                    }
                    else
                    {
                        output.Append(@"<li id='" + Spvmenu.Title + "'> <a class='navlink' href='#' id='" + Spvmenu.Title + "'> <i class='icon sprite-img " + Spvmenu.Image + "'> </i>" + Spvmenu.DisplayName + " </a>");
                    }

                    output.Append(@"<ul>");
                    foreach (var SpvmenuDetail in SpvmenuDetails)
                    {
                        url = urlHelper.Action(SpvmenuDetail.Action, SpvmenuDetail.Controller, new { id = "", area = "" });
                        if (Spvmenu.SpvMenuId == SpvmenuDetail.SubMenuParentID)
                        {
                            output.Append(@"<li id='" + SpvmenuDetail.Title + "'><a href='" + url + "' id='" + Spvmenu.Title + "'>" + SpvmenuDetail.DisplayName + "</a></li>");
                        }
                    }
                    output.Append(@"</ul>");
                    output.Append(@"</li>");
                }

            }
            output.Append(@"</ul>");

            //output.Append(@"<script>
            //if($('.menu').find('li [id=" + ProjectConfiguration.GetMenuName + "]').length>2) { $('.menu').find('li [id=" + ProjectConfiguration.GetMenuName + "]').first().parent('li').addClass('active');} else {{ $('.menu').find('li [id=" + ProjectConfiguration.GetMenuName + "]').parent('li').addClass('active');}}</script>");

            return output;
        }
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
        private static void ClearMessages(HtmlHelper htmlHelper, string messageType)
        {
            htmlHelper.ViewContext.ViewData[messageType] = htmlHelper.ViewContext.TempData[messageType] = null;
        }
    }
}
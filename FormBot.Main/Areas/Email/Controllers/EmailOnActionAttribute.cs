using System.Web;
using System.Web.Mvc;
using FormBot.Email;

namespace FormBot.Main.Areas.Email.Controllers
{
    public class EmailOnActionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string ActionName = filterContext.ActionDescriptor.ActionName.ToLower();
            string ControllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower();
            if (ControllerName == "email" && ActionName != "emailsettings")
            {
                Account acct = HttpContext.Current.Session[Constants.sessionAccount] as Account;
                if (acct == null)
                {
                    filterContext.HttpContext.Response.Redirect("~/Email/Email/EmailSettings", true);    
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
using System.Web.Mvc;

namespace FormBot.Main.Areas.QueuedEmail
{
    public class QueuedEmailAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "QueuedEmail";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "QueuedEmail_default",
                "QueuedEmail/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FormBot.SPVMain.Controllers
{
    public class HomeController : Controller
    {
        [Route("")]
        [UserAuthorization]
        public ActionResult Index()
        {
            return View();
        }

    }
}
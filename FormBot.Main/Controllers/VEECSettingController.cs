using FormBot.Entity.VEEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FormBot.Main.Controllers
{
    public class VEECSettingController : Controller
    {
        // GET: VEECSetting
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [UserAuthorization]
        public ActionResult VEECSetting()
        {
            VEECSetting veecSetting = new VEECSetting();
            VEECScheduling veecScheduling = new VEECScheduling();
            veecSetting.veecSchedulingTemplate = veecScheduling;
            return View(veecSetting);
        }
    }
}
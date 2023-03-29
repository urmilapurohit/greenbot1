using FormBot.BAL.Service.Job;
using FormBot.BAL.Service.SolarElectrician;
using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FormBot.Main.Controllers
{
    public class SolarElectricianController : BaseController
    {

        #region Properties
        private readonly ISolarElectricianBAL _electrician;
        #endregion

        #region Constructor
        public SolarElectricianController(ISolarElectricianBAL electrician)
        {
            this._electrician = electrician;
        }
        #endregion

        // GET: SolarElectrician
        public ActionResult Index()
        {
            return View();
        }

        //[HttpGet]
        //public JsonResult GetElectricianByUserId()
        //{
        //    List<SelectListItem> items = GetAllElectrician();
        //    return Json(items, JsonRequestBehavior.AllowGet);
        //}

        //public List<SelectListItem> GetAllElectrician()
        //{
        //    int solarCompanyId = ProjectSession.SolarCompanyId;
        //    int userId = ProjectSession.LoggedInUserId;

        //    ////TODO
        //    solarCompanyId = 84;
        //    bool isInstall = true;

        //    return _electrician.GetAllElectrician(isInstall, solarCompanyId).Select(a => new SelectListItem { Text = a.Name, Value = a.ID.ToString() }).ToList();
        //}
    }
}
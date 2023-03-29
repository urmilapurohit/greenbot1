using FormBot.BAL.Service.VEECCheckList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FormBot.Main.Controllers
{
    public class VEECCheckListClassTypeController : Controller
    {
        #region Properties
        private readonly IVECCCheckListClassTypeBAL _veecCheckListClassTypeService;
        #endregion


        #region Constructor
        public VEECCheckListClassTypeController(IVECCCheckListClassTypeBAL veecCheckListClassTypeService)
        {
            this._veecCheckListClassTypeService = veecCheckListClassTypeService;
        }
        #endregion
        // GET: VEECCheckListClassType
        public ActionResult Index()
        {
            return View();
        }


        public JsonResult GetVEECCheckListClassType(bool isSetFromSetting)
        {
            List<SelectListItem> Items = _veecCheckListClassTypeService.GetData(isSetFromSetting).Select(a => new SelectListItem { Text = a.VEECCheckListClassTypeName, Value = a.VEECCheckListClassTypeId.ToString() }).ToList();
            return Json(Items, JsonRequestBehavior.AllowGet);
        }
        
    }
}
using FormBot.BAL.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FormBot.Main.Controllers
{
    public class StreetTypeController : Controller
    {
        #region Properties
        private readonly IStreetTypeBAL _streetTypeService;
        #endregion

        #region Constructor
        public StreetTypeController(IStreetTypeBAL streetTypeService)
        {
            this._streetTypeService = streetTypeService;
        }
        #endregion

        /// <summary>
        /// Get StreetTypeID and StreetTypeName for dropdownlist.
        /// </summary>
        /// <returns>Items</returns>
        [HttpGet]
        public JsonResult GetStreetType()
        {
            List<SelectListItem> Items = _streetTypeService.GetData().Select(a => new SelectListItem { Text = a.StreetTypeName, Value = a.StreetTypeID.ToString() }).ToList();
            return Json(Items, JsonRequestBehavior.AllowGet);
        }
    }
}
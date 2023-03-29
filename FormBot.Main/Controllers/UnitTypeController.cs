using FormBot.BAL.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FormBot.Main.Controllers
{
    public class UnitTypeController : Controller
    {
       #region Properties
        private readonly IUnitTypeBAL _unitTypeService;
        #endregion

        #region Constructor
        public UnitTypeController(IUnitTypeBAL unitTypeService)
        {
            this._unitTypeService = unitTypeService;
        }
        #endregion

        /// <summary>
        /// Get UnitTypeID and UnitTypeName for dropdownlist.
        /// </summary>
        /// <returns>Items</returns>
        [HttpGet]
        public JsonResult GetUnitType()
        {
            List<SelectListItem> Items = _unitTypeService.GetData().Select(a => new SelectListItem { Text = a.UnitTypeName, Value = a.UnitTypeID.ToString() }).ToList();
            return Json(Items, JsonRequestBehavior.AllowGet);
        }
    }
}
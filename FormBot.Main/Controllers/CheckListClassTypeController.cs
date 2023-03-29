using FormBot.BAL.Service.CheckList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FormBot.Main.Controllers
{
    public class CheckListClassTypeController : Controller
    {
        #region Properties
        private readonly  ICheckListClassTypeBAL  _checkListClassTypeService;
        #endregion

        #region Constructor
        public CheckListClassTypeController(ICheckListClassTypeBAL checkListClassTypeService)
        {
            this._checkListClassTypeService = checkListClassTypeService;
        }
        #endregion

        #region Events

        /// <summary>
        /// Get checkList classType dropdownlist.
        /// </summary>
        /// <returns>Items</returns>
        [HttpGet]
        public JsonResult GetCheckListClassType(bool isSetFromSetting)
        {
            List<SelectListItem> Items = _checkListClassTypeService.GetData(isSetFromSetting).Select(a => new SelectListItem { Text = a.CheckListClassTypeName, Value = a.CheckListClassTypeId.ToString() }).ToList();
            return Json(Items, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Get checkList phototype dropdownlist.
        /// </summary>
        /// <returns>Items</returns>
        [HttpGet]
        public JsonResult GetPhotoType()
        {
            List<SelectListItem> Items = _checkListClassTypeService.GetPhototype().Select(a => new SelectListItem { Text = a.PhotoType, Value = a.CheckListPhotoTypeId.ToString() }).ToList();
            return Json(Items, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
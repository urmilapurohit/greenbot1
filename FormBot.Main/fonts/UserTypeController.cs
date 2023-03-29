using FormBot.BAL.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FormBot.Main.Controllers
{
    public class UserTypeController : Controller
    {
        #region Properties
        private readonly IUserTypeBAL _userTypeService;
        #endregion

        #region Constructor
        public UserTypeController(IUserTypeBAL userTypeService)
        {
            this._userTypeService = userTypeService;
        }
        #endregion

        /// <summary>
        /// Get UserTypeId and UserTypeName for dropdownlist.
        /// </summary>
        /// <returns>Items</returns>
        public JsonResult GetUserType()
        {
            List<SelectListItem> Items = _userTypeService.GetData().Select(a => new SelectListItem { Text = a.UserTypeName, Value = a.UserTypeID.ToString() }).ToList();
            return Json(Items, JsonRequestBehavior.AllowGet);
        }
    }
}
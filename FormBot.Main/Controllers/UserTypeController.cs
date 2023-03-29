using FormBot.BAL.Service;
using FormBot.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
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

        ///// <summary>
        ///// Get UserTypeId and UserTypeName for Non FSA user.
        ///// </summary>
        ///// <returns>Returns List Of User Type</returns>
        //[HttpGet]
        //public JsonResult GetUserType()
        //{
        //    if (ProjectSession.SystemUserTypeTable != null && ProjectSession.SystemUserTypeTable.Rows.Count > 0)
        //    {
        //        DataTable dtUserType = ProjectSession.SystemUserTypeTable;
        //        List<SelectListItem> items = new List<SelectListItem>();
        //        for (int i = 0; i < dtUserType.Rows.Count; i++)
        //        {
        //            items.Add(new SelectListItem()
        //            {
        //                Text = dtUserType.Rows[i]["UserTypeName"].ToString(),
        //                Value = dtUserType.Rows[i]["UserTypeID"].ToString()
        //            });
        //        }
        //        return Json(items, JsonRequestBehavior.AllowGet);
        //    }
                
        //    else
        //    {
        //        List<SelectListItem> items = GetAllUserTypes();
        //        DataTable dtUserType = new DataTable();
        //        dtUserType.Columns.Add("UserTypeID", typeof(string));
        //        dtUserType.Columns.Add("UserTypeName", typeof(string));
        //        for (int i = 0; i < items.Count; i++)
        //        {
        //            dtUserType.Rows.Add(new object[] { items[i].Value, items[i].Text });
        //        }
        //        ProjectSession.SystemUserTypeTable = dtUserType;
        //        return Json(items, JsonRequestBehavior.AllowGet);
        //    }
        //}

        [HttpGet]
        public JsonResult GetUserType()
        {
            //List<SelectListItem> items = GetAllUserTypes();
            //return Json(items, JsonRequestBehavior.AllowGet);


            if (ProjectSession.SystemUserType != null)
                return Json(ProjectSession.SystemUserType, JsonRequestBehavior.AllowGet);
            else
            {
                List<SelectListItem> items = new List<SelectListItem>();
                items = GetAllUserTypes();
                ProjectSession.SystemUserType = JsonConvert.SerializeObject(items);
                return Json(items, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Get UserTypeId and UserTypeName for FSA user.
        /// </summary>
        /// <returns>Returns List Of User Type for FSA</returns>
        [HttpGet]
        public JsonResult GetUserTypeforFSA()
        {
            List<SelectListItem> items = GetAllUserTypes();
            List<SelectListItem> itemsToRemove = items.Where(t => t.Value == "4" || t.Value == "7" || t.Value == "10").ToList();
            items = items.Except(itemsToRemove).ToList();
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get UserTypeId and UserTypeName for All user.
        /// </summary>
        /// <returns>Returns List Of All User Type</returns>
        public List<SelectListItem> GetAllUserTypes()
        {
            return _userTypeService.GetData().Select(a => new SelectListItem { Text = a.UserTypeName, Value = a.UserTypeID.ToString() }).ToList();
        }
    }
}
using FormBot.BAL.Service;
using FormBot.Entity;
using System.Collections.Generic;
using FormBot.Helper;
using Microsoft.AspNet.Identity;
using System;
using System.Web.Mvc;
using System.Threading.Tasks;
using FormBot.Main.Models;
using System.Linq;
using FormBot.BAL;
using System.Web;

namespace FormBot.Main.Controllers
{
    public class FCOGroupController : BaseController
    {
        #region Properties
        private readonly IFCOGroupBAL _FCOGroupBAL;
        private readonly IFCOUserGroupBAL _FCOUserGroupBAL;
        public UserManager<ApplicationUser> UserManager { get; private set; }
        #endregion

        #region Constructor
        public FCOGroupController(IFCOGroupBAL FCOGroupBAL, IFCOUserGroupBAL FCOUserGroupBAL)
        {
            this._FCOGroupBAL = FCOGroupBAL;
            this._FCOUserGroupBAL = FCOUserGroupBAL;
        }
        #endregion

        [UserAuthorization]
        public ActionResult Index()
        {
            return View(new FCOGroup());
        }
       
        /// <summary>
        /// Get FCOGroup List
        /// </summary>
        /// <param name="groupName">group Name</param>
        /// <param name="AssignedUser">Assigned User</param>
        public void GetFCOGroupList(string groupName = null, string AssignedUser = null) //, int FCOGroupId = 0
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            int PageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);
            List<FCOGroup> lstFCOGroup = _FCOGroupBAL.FCOGroupList(PageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, groupName, AssignedUser);

            //if (lstFCOGroup.Count > 0)
            //{
            //    var result = new
            //    {
            //        iTotalRecords = lstFCOGroup.FirstOrDefault().TotalRecords,
            //        iTotalDisplayRecords = lstFCOGroup.FirstOrDefault().TotalRecords,
            //        aaData = lstFCOGroup
            //    };

            //    JavaScriptSerializer js = new JavaScriptSerializer();
            //    js.Serialize(result);

            //    HttpContext.Response.Write(js.Serialize(result));
            //}
            //else
            //{
            //    HttpContext.Response.Write("");
            //}
            if (lstFCOGroup.Count > 0)
            {
                gridParam.TotalDisplayRecords = lstFCOGroup.FirstOrDefault().TotalRecords;
                gridParam.TotalRecords = lstFCOGroup.FirstOrDefault().TotalRecords;

            }
            HttpContext.Response.Write(Grid.PrepareDataSet(lstFCOGroup, gridParam));
        }

        [HttpGet]
        [UserAuthorization]
        public ActionResult Create()
        {
            var model = new FCOGroup();
            //List<User> lstUser = _userBAL.GetUserList(UserID, PageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir);
            //List<SelectListItem> Items 
            var lstUserList = _FCOGroupBAL.GetUserList(0);
            model.lstUser = lstUserList;
            model.lstAssignedUser = new List<SelectListItem>();
            return View(model);
            //return View(new FCOGroup());

        }
       
        /// <summary>
        /// Post method for create FCOGroup.
        /// </summary>
        /// <param name="FCOGroupView">FCOGroup View</param>
        /// <returns>list of actionResult</returns>
        [HttpPost]
        [UserAuthorization]
        public async Task<ActionResult> Create(FormBot.Entity.FCOGroup FCOGroupView)
        {
            ModelState.Remove("AssignedUser");
            ModelState.Remove("lstUser");
            if (ModelState.IsValid)
            {
                object groupNameExists = _FCOGroupBAL.CheckGroupNameExists(FCOGroupView.GroupName, FCOGroupView.FCOGroupId);
                if (!groupNameExists.Equals(0))
                {
                    var lstUserListTest = _FCOGroupBAL.GetUserList(0);
                    FCOGroupView.lstUser = lstUserListTest;
                    FCOGroupView.lstAssignedUser = new List<SelectListItem>();
                    this.ShowMessage(SystemEnums.MessageType.Error, "FCO group with same name already exists.", true);
                    return RedirectToAction("Create", "FCOGroup");
                }
                FCOGroupView.CreatedDate = DateTime.Today;
                FCOGroupView.CreatedBy = ProjectSession.LoggedInUserId; // change this value after login page
                FCOGroupView.ModifiedDate = DateTime.Today;
                int FCOGroupId = _FCOGroupBAL.Create(FCOGroupView);
                if (FCOGroupView.AssignedUser != null)
                {
                    for (int i = 0; i < FCOGroupView.AssignedUser.Count(); i++)
                    {
                        int UserID = Convert.ToInt32(FCOGroupView.AssignedUser.ToArray()[i]);
                        var FCOUserGroup = _FCOGroupBAL.CreateFCOGroupUser(UserID, FCOGroupId);
                    }
                }

                var lstUserList = _FCOGroupBAL.GetUserList(0);
                FCOGroupView.lstUser = lstUserList;
                FCOGroupView.lstAssignedUser = new List<SelectListItem>();
                this.ShowMessage(SystemEnums.MessageType.Success, "FCO Group has been saved successfully.", true);
                return RedirectToAction("Index", "FCOGroup");
            }
            else
            {
                //var lstUserList = _FCOGroupBAL.GetUserList(0);
                //FCOGroupView.lstUser = lstUserList;
                //FCOGroupView.lstAssignedUser = new List<SelectListItem>();
                this.ShowMessage(SystemEnums.MessageType.Error, String.Join(Environment.NewLine, ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage + " " + v.Exception)), true);
                return RedirectToAction("Create", "FCOGroup");
                //return this.View("Create", FCOGroupView);
            }
        }
        public ActionResult _Details()
        {
            return View();
        }

        /// <summary>
        /// Edits the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Action result</returns>
        [HttpGet]
        [UserAuthorization]
        public ActionResult Edit(string id)
        {
            var model = new FCOGroup();
            int FCOGroupId = 0;
            if (!string.IsNullOrEmpty(id))
                int.TryParse(QueryString.GetValueFromQueryString(id, "id"), out FCOGroupId);
            var dsFCOGroup = _FCOGroupBAL.GetFCOGroupById(FCOGroupId);
            var lstFCOUserGroup = _FCOGroupBAL.GetUserList(FCOGroupId);
            List<FCOGroup> FCOGroup = DBClient.DataTableToList<FCOGroup>(dsFCOGroup.Tables[0]);
            FCOGroup FCOGroupView = FCOGroup.FirstOrDefault();
            FCOGroupView.lstUser = lstFCOUserGroup;
            var lstAssignedFCOGroupList = _FCOUserGroupBAL.GetFCOUserGroupList(FCOGroupId);
            FCOGroupView.lstAssignedUser = lstAssignedFCOGroupList;
            FCOGroupView.AssignedUser = lstAssignedFCOGroupList.Select(d => d.Value).ToArray();
            return View(FCOGroupView);
        }

        /// <summary>
        /// Edits the specified fco group view.
        /// </summary>
        /// <param name="FCOGroupView">The fco group view.</param>
        /// <returns>list of action result</returns>
        [HttpPost]
        [UserAuthorization]
        public async Task<ActionResult> Edit(FormBot.Entity.FCOGroup FCOGroupView)
        {
            ModelState.Remove("AssignedUser");
            ModelState.Remove("lstUser");
            if (ModelState.IsValid)
            {
                object groupNameExists = _FCOGroupBAL.CheckGroupNameExists(FCOGroupView.GroupName, FCOGroupView.FCOGroupId);
                if (!groupNameExists.Equals(0))
                {
                    var lstUserListTest = _FCOGroupBAL.GetUserList(0);
                    FCOGroupView.lstUser = lstUserListTest;
                    FCOGroupView.lstAssignedUser = new List<SelectListItem>();
                    this.ShowMessage(SystemEnums.MessageType.Error, "FCO group with same name already exists.", true);
                    return RedirectToAction("Edit", FCOGroupView);
                }
                FCOGroupView.ModifiedBy = ProjectSession.LoggedInUserId;
                FCOGroupView.ModifiedDate = DateTime.Today;
                _FCOGroupBAL.Create(FCOGroupView);
                _FCOUserGroupBAL.Delete(FCOGroupView.FCOGroupId);
                if (FCOGroupView.AssignedUser != null)
                {
                    for (int i = 0; i < FCOGroupView.AssignedUser.Count(); i++)
                    {
                        int UserID = Convert.ToInt32(FCOGroupView.AssignedUser.ToArray()[i]);
                        _FCOGroupBAL.CreateFCOGroupUser(UserID, FCOGroupView.FCOGroupId);
                    }
                }
                this.ShowMessage(SystemEnums.MessageType.Success, "FCO Group has been Updated successfully.", true);

                var lstFCOUserGroup = _FCOGroupBAL.GetUserList(FCOGroupView.FCOGroupId);
                var lstAssignedFCOGroupList = _FCOUserGroupBAL.GetFCOUserGroupList(FCOGroupView.FCOGroupId);
                FCOGroupView.lstAssignedUser = lstAssignedFCOGroupList;
                FCOGroupView.lstUser = lstFCOUserGroup;
                return RedirectToAction("Index", "FCOGroup");
            }
            else
            {
                this.ShowMessage(SystemEnums.MessageType.Error, String.Join(Environment.NewLine, ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage + " " + v.Exception)), true);
                //return this.View("Edit", FCOGroupView);
                return RedirectToAction("Edit", "FCOGroup");
            }
        }

        /// <summary>
        /// Gets the fco group.
        /// </summary>
        /// <returns>json result</returns>
        [HttpGet]
        public JsonResult GetFCOGroup()
        {
            List<SelectListItem> Items = _FCOGroupBAL.GetFCOGroupDropDown().Select(a => new SelectListItem { Text = a.GroupName, Value = a.FCOGroupId.ToString() }).ToList();
            return Json(Items, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>action result</returns>
        [UserAuthorization]
        public ActionResult Delete(string id)
        {
            //try
            //{
            int FCOGroupId = 0;
            if (!string.IsNullOrEmpty(id))
            {
                int.TryParse(QueryString.GetValueFromQueryString(id, "id"), out FCOGroupId);
            }
            _FCOGroupBAL.DeleteFCOGroup(FCOGroupId);
            return this.Json(new { success = true });
            //}
            //catch (Exception ex)
            //{
            //    return this.Json(new { success = false, errormessage = ex.Message });
            //}
        }
    }
}

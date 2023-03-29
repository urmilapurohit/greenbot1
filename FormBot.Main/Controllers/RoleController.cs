using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FormBot.Entity;
using System.Data;
using FormBot.DAL;
using FormBot.Helper;
using System.Transactions;
using FormBot.BAL.Service;
using FormBot.BAL;
using System.Web.Script.Serialization;
using System.Text;
using System.Data.SqlClient;
using FormBot.Helper.Helper;

namespace FormBot.Main.Controllers
{
    public class RoleController : BaseController
    {
        #region Properties
        private readonly IRoleBAL _role;
        #endregion

        #region Constructor
        public RoleController(IRoleBAL role)
        {
            this._role = role;
        }
        #endregion

        /// <summary>
        /// Role list
        /// </summary>
        /// <returns>action result</returns>
        [UserAuthorization]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Gets the role list.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="strUserType">Type of the string user.</param>
        public void GetRoleList(string name, string strUserType,string CreatedBy)
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);
            IList<RoleView> lstRole = new List<RoleView>();
            int userType = 0;
            if (!string.IsNullOrEmpty(strUserType))
            {
                int.TryParse(strUserType, out userType);
            }

            if (ProjectSession.UserTypeId == (int)SystemEnums.UserType.FormBotSuperAdmin)
            {
                lstRole = _role.AdminRoleList(pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, name, userType,CreatedBy);
            }
            else
            {
                lstRole = _role.RoleList(pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, name, userType,CreatedBy);
            }

            if (lstRole.Count > 0)
            {
                gridParam.TotalDisplayRecords = lstRole.FirstOrDefault().TotalRecords;
                gridParam.TotalRecords = lstRole.FirstOrDefault().TotalRecords;
            }

            HttpContext.Response.Write(Grid.PrepareDataSet(lstRole, gridParam));
        }

        /// <summary>
        /// Insert role
        /// </summary>
        /// <returns>Action Result</returns>
        [HttpGet]
        [UserAuthorization]
        public ActionResult Create()
        {
            RoleView roleView = new RoleView();
            return View(roleView);
        }

        /// <summary>
        /// Insert Role - Post
        /// </summary>
        /// <param name="roleView">role view</param>
        /// <returns>action result</returns>
        [HttpPost]
        [UserAuthorization]
        public ActionResult Create(RoleView roleView)
        {
            if (ModelState.IsValid)
            {
                bool roleNameExists = _role.CheckRoleNameExists(roleView.Name, roleView.RoleId);
                if (roleNameExists)
                {
                    this.ShowMessage(SystemEnums.MessageType.Error, "Role with same name already exists.", true);
                    return RedirectToAction("Create", "Role");
                }

                roleView.CreatedBy = 1;
                roleView.ModifiedBy = 1;
                roleView.IsDeleted = false;
                roleView.IsSystemRole = false;
                if (ProjectSession.UserTypeId == (int)SystemEnums.UserType.ResellerAdmin)
                {
                    roleView.UserType = (int)SystemEnums.UserType.ResellerAccountManager;
                }

                if (ProjectSession.UserTypeId == (int)SystemEnums.UserType.SolarCompanyAdmin)
                {
                    roleView.UserType = (int)SystemEnums.UserType.SolarConnectionsOfficer;
                }

                if (ProjectSession.UserTypeId == (int)SystemEnums.UserType.SolarSubContractorAdmin)
                {
                    roleView.UserType = (int)SystemEnums.UserType.SolarConnectionsOfficer;
                }

                List<string> temp = roleView.Rights.Split(',').ToList();
                if (temp.Contains(((int)SystemEnums.MenuId.RoelList).ToString()))
                {
                    temp.Add(((int)SystemEnums.MenuId.RoleView).ToString());
                }

                if (temp.Contains(((int)SystemEnums.MenuId.UserList).ToString()))
                {
                    temp.Add(((int)SystemEnums.MenuId.UserView).ToString());
                }

                if (temp.Contains(((int)SystemEnums.MenuId.SCAList).ToString()))
                {
                    temp.Add(((int)SystemEnums.MenuId.SCAView).ToString());
                }

                if (temp.Contains(((int)SystemEnums.MenuId.SEList).ToString()))
                {
                    temp.Add(((int)SystemEnums.MenuId.SEView).ToString());
                }

                if (temp.Contains(((int)SystemEnums.MenuId.EmailTemplateList).ToString()))
                {
                    temp.Add(((int)SystemEnums.MenuId.EmailTemplateView).ToString());
                }

                if (temp.Contains(((int)SystemEnums.MenuId.Email).ToString()))
                {
                    temp.Add(((int)SystemEnums.MenuId.EmailSetting).ToString());
                }

                roleView.Rights = String.Join(",", temp);
                _role.CreateRole(roleView);
                this.ShowMessage(SystemEnums.MessageType.Success, "Role has been saved successfully.", true);
                return RedirectToAction("Index", "Role");
                /*return View();*/
            }
            else
            {
                this.ShowMessage(SystemEnums.MessageType.Error, String.Join(Environment.NewLine, ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage + " " + v.Exception)), true);
                return RedirectToAction("Create", "Role");
            }
        }

        /// <summary>
        /// Views the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>action result</returns>
        [UserAuthorization]
        public ActionResult View(string id)
        {
            int roleId = 0;
            if (!string.IsNullOrEmpty(id))
            {
                int.TryParse(QueryString.GetValueFromQueryString(id, "id"), out roleId);
            }

            var dsRoles = new DataSet();
            if (ProjectSession.UserTypeId == (int)SystemEnums.UserType.FormBotSuperAdmin)
            {
                dsRoles = _role.GetAdminRoleById(roleId);
            }
            else
            {
                dsRoles = _role.GetRoleById(roleId);
            }

            List<RoleView> role = DBClient.DataTableToList<RoleView>(dsRoles.Tables[0]);
            RoleView roleView = role.FirstOrDefault();
            List<RoleMenuView> roleMenus = DBClient.DataTableToList<RoleMenuView>(dsRoles.Tables[1]);
            var roleMenuList = roleMenus.Select(s => s.MenuId).ToList();
            foreach (var roleMenu in roleMenuList)
            {
                roleView.Rights = roleView.Rights + "," + roleMenu;
            }

            roleView.Rights = string.IsNullOrEmpty(roleView.Rights) ? null : roleView.Rights.Trim(',');
            return View(roleView);
        }

        /// <summary>
        /// Edit role
        /// </summary>
        /// <param name="id">the identifier</param>
        /// <returns>Action Result</returns>
        [HttpGet]
        [UserAuthorization]
        public ActionResult Edit(string id)
        {
            int roleId = 0;
            if (!string.IsNullOrEmpty(id))
            {
                int.TryParse(QueryString.GetValueFromQueryString(id, "id"), out roleId);
            }

            var dsRoles = new DataSet();
            if (ProjectSession.UserTypeId == (int)SystemEnums.UserType.FormBotSuperAdmin)
            {
                dsRoles = _role.GetAdminRoleById(roleId);
            }
            else
            {
                dsRoles = _role.GetRoleById(roleId);
            }

            int SelectedInvoicer = _role.GetRoleWiseInvoicer(roleId);

            List<RoleView> role = DBClient.DataTableToList<RoleView>(dsRoles.Tables[0]);
            RoleView roleView = role.Where(x => x.RoleId == roleId).FirstOrDefault();
            List<RoleMenuView> roleMenus = DBClient.DataTableToList<RoleMenuView>(dsRoles.Tables[1]);
            var roleMenuList = roleMenus.Where(s => s.RoleId == roleId).Select(s => s.MenuId).ToList();
            foreach (var roleMenu in roleMenuList)
            {
                roleView.Rights = roleView.Rights + "," + roleMenu;
            }

            roleView.Rights = string.IsNullOrEmpty(roleView.Rights) ? null : roleView.Rights.Trim(',');
            roleView.SelectedInvoicer = SelectedInvoicer;
            return View(roleView);
        }

        /// <summary>
        /// Edit Role - POST
        /// </summary>
        /// <param name="roleView">role view</param>
        /// <returns>action result</returns>
        [HttpPost]
        [UserAuthorization]
        public ActionResult Edit(RoleView roleView)
        {
            if (ModelState.IsValid)
            {
                bool roleNameExists = _role.CheckRoleNameExists(roleView.Name, roleView.RoleId);
                if (roleNameExists)
                {
                    this.ShowMessage(SystemEnums.MessageType.Error, "Role with same name already exists.", true);
                    return RedirectToAction("Edit", "Role", new { id = EncryptionDecryption.GetEncrypt("id=" + roleView.RoleId) });
                }

                if (ProjectSession.UserTypeId == (int)SystemEnums.UserType.ResellerAdmin)
                {
                    roleView.UserType = (int)SystemEnums.UserType.ResellerAccountManager;
                }

                if (ProjectSession.UserTypeId == (int)SystemEnums.UserType.SolarCompanyAdmin)
                {
                    roleView.UserType = (int)SystemEnums.UserType.SolarConnectionsOfficer;
                }

                if (ProjectSession.UserTypeId == (int)SystemEnums.UserType.SolarSubContractorAdmin)
                {
                    roleView.UserType = (int)SystemEnums.UserType.SolarConnectionsOfficer;
                }

                List<string> temp = roleView.Rights.Split(',').ToList();
                if (temp.Contains(((int)SystemEnums.MenuId.RoelList).ToString()))
                {
                    temp.Add(((int)SystemEnums.MenuId.RoleView).ToString());
                }

                if (temp.Contains(((int)SystemEnums.MenuId.UserList).ToString()))
                {
                    temp.Add(((int)SystemEnums.MenuId.UserView).ToString());
                }

                if (temp.Contains(((int)SystemEnums.MenuId.SCAList).ToString()))
                {
                    temp.Add(((int)SystemEnums.MenuId.SCAView).ToString());
                }

                if (temp.Contains(((int)SystemEnums.MenuId.SEList).ToString()))
                {
                    temp.Add(((int)SystemEnums.MenuId.SEView).ToString());
                }

                if (temp.Contains(((int)SystemEnums.MenuId.EmailTemplateList).ToString()))
                {
                    temp.Add(((int)SystemEnums.MenuId.EmailTemplateView).ToString());
                }

                if (temp.Contains(((int)SystemEnums.MenuId.Email).ToString()))
                {
                    temp.Add(((int)SystemEnums.MenuId.EmailSetting).ToString());
                }

                roleView.Rights = String.Join(",", temp);
                _role.EditRole(roleView);
                this.ShowMessage(SystemEnums.MessageType.Success, "Role has been saved successfully.", true);
                return RedirectToAction("Index", "Role");
            }
            else
            {
                this.ShowMessage(SystemEnums.MessageType.Error, String.Join(Environment.NewLine, ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage + " " + v.Exception)), true);
                return RedirectToAction("Edit", "Role", new { id = EncryptionDecryption.GetEncrypt("id=" + roleView.RoleId) });
            }

        }

        /// <summary>
        /// Delete Role
        /// </summary>
        /// <param name="id">the identifier</param>
        /// <returns>Action Result</returns>
        [UserAuthorization]
        public ActionResult Delete(string id)
        {
            int roleId = 0;
            if (!string.IsNullOrEmpty(id))
            {
                int.TryParse(QueryString.GetValueFromQueryString(id, "id"), out roleId);
            }

            var roleView = _role.GetRoleById(roleId);
            bool isExists = _role.DeleteRole(roleId);
            if (!isExists)
            {
                this.ShowMessage(SystemEnums.MessageType.Error, "Role has been assigned to some user(s). You can't delete this role.", true);
            }
            else
            {
                this.ShowMessage(SystemEnums.MessageType.Success, "Role has been deleted successfully.", true);
            }

            return RedirectToAction("Index", "Role");
        }

        /// <summary>
        /// Gets the role.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>object result</returns>
        [HttpGet]
        public JsonResult GetRole(int id = 0)
        {
            List<SelectListItem> Items = _role.GetData(id).Select(a => new SelectListItem { Text = a.Name, Value = a.RoleId.ToString(), Selected = a.IsSystemRole == true }).ToList();
            return Json(Items, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the role menus on dropdown change.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>object result</returns>
        [HttpGet]
        public JsonResult GetRoleMenusOnDropdownChange(int id)
        {
            var result = _role.DynamicRoleBinding(id).ToString();
            return Json(new { result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the role user type wise.
        /// </summary>
        /// <returns>object result</returns>
        [HttpGet]
        public JsonResult GetRoleUserTypewise()
        {
            string usertypes = string.Empty;
            if (ProjectSession.UserTypeId == 1)
            {
                usertypes = "1,2,3,5,6,8,9";
            }
            else if (ProjectSession.UserTypeId == 2)
            {
                usertypes = "5";
            }
            else if (ProjectSession.UserTypeId == 4 || ProjectSession.UserTypeId == 6)
            {
                usertypes = "8";
            }

            List<SelectListItem> Items = _role.GetRoleUserTypewise(usertypes).Select(a => new SelectListItem { Text = a.Name, Value = a.RoleId.ToString(), Selected = a.IsSystemRole == true }).ToList();
            return Json(Items, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Sets users as saas user.
        /// </summary>
        [HttpPost]
        public JsonResult SetUsersAsSaas(string RoleId, bool IsSAASUser, string Invoicer)
        {
            try
            {
                _role.SetUsersAsSaas(RoleId, IsSAASUser, Invoicer);
            }
            catch (Exception ex)
            {
                return this.Json(new { success = false, errormessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            return this.Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the role.
        /// </summary>
        /// <param name="RoleId">The identifier.</param>
        /// <returns>object result</returns>
        [HttpGet]
        public bool CheckRoleHasSaasUser(int RoleId)
        {
            var dsMenus = _role.CheckRoleHasSaasUser(RoleId);
            if (dsMenus.Tables.Count > 0 && dsMenus.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
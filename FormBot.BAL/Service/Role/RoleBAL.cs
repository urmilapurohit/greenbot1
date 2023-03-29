using FormBot.DAL;
using FormBot.Entity;
using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace FormBot.BAL.Service
{
    public class RoleBAL : IRoleBAL
    {
        /// <summary>
        /// Checks the role name exists.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="roleId">The role identifier.</param>
        /// <returns>role identifier</returns>
        public bool CheckRoleNameExists(string name, int roleId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("Name", SqlDbType.NVarChar, name));
            sqlParameters.Add(DBClient.AddParameters("RoleId", SqlDbType.Int, roleId));
            var isRoleExists = CommonDAL.ExecuteScalar("Role_Exists", sqlParameters.ToArray());
            return isRoleExists.ToString() == "1" ? true : false;
        }

        /// <summary>
        /// Gets the role by identifier.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <returns>DataSet</returns>
        public DataSet GetRoleById(int roleId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("RoleId", SqlDbType.Int, roleId));
            sqlParameters.Add(DBClient.AddParameters("SelectedUserType", SqlDbType.Int, ProjectSession.UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, ProjectSession.LoggedInUserId));
            DataSet dsRoles = CommonDAL.ExecuteDataSet("Role_GetRoleById", sqlParameters.ToArray());
            return dsRoles;
        }

        /// <summary>
        /// Gets the admin role by identifier.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <returns>DataSet</returns>
        public DataSet GetAdminRoleById(int roleId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("RoleId", SqlDbType.Int, roleId));
            DataSet dsRoles = CommonDAL.ExecuteDataSet("Role_AdminGetRoleById", sqlParameters.ToArray());
            return dsRoles;
        }

        /// <summary>
        /// Creates the role.
        /// </summary>
        /// <param name="roleView">The role view.</param>
        public void CreateRole(RoleView roleView)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("Name", SqlDbType.NVarChar, roleView.Name));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("IsDeleted", SqlDbType.Bit, roleView.IsDeleted));
            sqlParameters.Add(DBClient.AddParameters("UserType", SqlDbType.TinyInt, roleView.UserType));
            sqlParameters.Add(DBClient.AddParameters("MenuIds", SqlDbType.NVarChar, roleView.Rights));
            CommonDAL.Crud("Role_InsertUpdate", sqlParameters.ToArray());
        }

        /// <summary>
        /// Edits the role.
        /// </summary>
        /// <param name="roleView">The role view.</param>
        public void EditRole(RoleView roleView)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("RoleId", SqlDbType.Int, roleView.RoleId));
            sqlParameters.Add(DBClient.AddParameters("Name", SqlDbType.NVarChar, roleView.Name));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("IsDeleted", SqlDbType.Bit, roleView.IsDeleted));
            sqlParameters.Add(DBClient.AddParameters("UserType", SqlDbType.TinyInt, roleView.UserType));
            sqlParameters.Add(DBClient.AddParameters("MenuIds", SqlDbType.NVarChar, roleView.Rights));
            CommonDAL.Crud("Role_InsertUpdate", sqlParameters.ToArray());
        }

        /// <summary>
        /// Deletes the role menu.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        public void DeleteRoleMenu(int roleId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("RoleId", SqlDbType.Int, roleId));
            CommonDAL.Crud("RoleMenu_Delete", sqlParameters.ToArray());
        }

        /// <summary>
        /// Roles the list.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="sortCol">The sort col.</param>
        /// <param name="sortDir">The sort dir.</param>
        /// <param name="name">The name.</param>
        /// <param name="userType">Type of the user.</param>
        /// <returns>role view</returns>
        public IList<RoleView> RoleList(int pageNumber, int pageSize, string sortCol, string sortDir, string name, int userType, string CreatedBy)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("Name", SqlDbType.NVarChar, string.IsNullOrEmpty(name) ? null : name));
            sqlParameters.Add(DBClient.AddParameters("Createdby", SqlDbType.NVarChar, string.IsNullOrEmpty(CreatedBy) ? null : CreatedBy));
            sqlParameters.Add(DBClient.AddParameters("UserType", SqlDbType.Int, userType));
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, pageNumber));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, pageSize));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, sortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, sortDir));
            sqlParameters.Add(DBClient.AddParameters("SelectedUserType", SqlDbType.Int, ProjectSession.UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, ProjectSession.LoggedInUserId));
            IList<RoleView> lstRole = CommonDAL.ExecuteProcedure<RoleView>("Role_GetRoleById", sqlParameters.ToArray()).ToList();
            return lstRole;
        }

        /// <summary>
        /// Admins the role list.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="sortCol">The sort col.</param>
        /// <param name="sortDir">The sort dir.</param>
        /// <param name="name">The name.</param>
        /// <param name="userType">Type of the user.</param>
        /// <returns>Role View</returns>
        public IList<RoleView> AdminRoleList(int pageNumber, int pageSize, string sortCol, string sortDir, string name, int userType,string CreatedBy)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("Name", SqlDbType.NVarChar, string.IsNullOrEmpty(name) ? null : name));
            sqlParameters.Add(DBClient.AddParameters("Createdby", SqlDbType.NVarChar, string.IsNullOrEmpty(CreatedBy) ? null : CreatedBy));
            sqlParameters.Add(DBClient.AddParameters("UserType", SqlDbType.Int, userType));
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, pageNumber));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, pageSize));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, sortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, sortDir));
            IList<RoleView> lstRole = CommonDAL.ExecuteProcedure<RoleView>("Role_AdminGetRoleById", sqlParameters.ToArray()).ToList();
            return lstRole;
        }

        /// <summary>
        /// Deletes the role.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <returns>bool</returns>
        public bool DeleteRole(int roleId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("RoleId", SqlDbType.Int, roleId));
            var isRoleExists = CommonDAL.ExecuteScalar("Role_Delete", sqlParameters.ToArray());
            return isRoleExists.ToString() == "1" ? true : false;
        }

        /// <summary>
        /// Dynamics the menu binding.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <returns>DataSet</returns>
        public DataSet DynamicMenuBinding(int roleId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("RoleId", SqlDbType.Int, roleId));
            var dsMenus = CommonDAL.ExecuteDataSet("Role_DynamicMenuBinding", sqlParameters.ToArray());
            return dsMenus;
        }

        /// <summary>
        /// Customs the authorization.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <param name="controller">The controller.</param>
        /// <param name="action">The action.</param>
        /// <returns>DataSet</returns>
        public DataSet CustomAuthorization(int roleId, string controller, string action)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("RoleId", SqlDbType.Int, roleId));
            sqlParameters.Add(DBClient.AddParameters("Controller", SqlDbType.NVarChar, controller));
            sqlParameters.Add(DBClient.AddParameters("Action", SqlDbType.NVarChar, action));
            var dsMenus = CommonDAL.ExecuteDataSet("Role_CustomAuthorization", sqlParameters.ToArray());
            return dsMenus;
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="userType">Type of the user.</param>
        /// <returns>role view</returns>
        public List<RoleView> GetData(int? userType = null)
        {
            string spName = "[Role_BindDropDown]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserType", SqlDbType.Int, userType));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            IList<RoleView> roleList = CommonDAL.ExecuteProcedure<RoleView>(spName, sqlParameters.ToArray());
            return roleList.ToList();
        }

        /// <summary>
        /// Dynamics the role binding.
        /// </summary>
        /// <param name="userType">Type of the user.</param>
        /// <returns>html string</returns>
        public HtmlString DynamicRoleBinding(int userType = 1)
        {
            var roleHtml = DynamicRoleHtml(userType);
            return MvcHtmlString.Create(roleHtml.ToString());
        }

        /// <summary>
        /// Dynamics the role HTML.
        /// </summary>
        /// <param name="userType">Type of the user.</param>
        /// <returns>String Builder</returns>
        public StringBuilder DynamicRoleHtml(int userType)
        {
            var output = new StringBuilder();
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, userType));
            DataSet dsMenus = CommonDAL.ExecuteDataSet("Menu_GetMenuList", sqlParameters.ToArray());
            List<MenuView> menus = DBClient.DataTableToList<MenuView>(dsMenus.Tables[0]);
            List<MenuView> menuActions = DBClient.DataTableToList<MenuView>(dsMenus.Tables[1]);
            foreach (var menu in menus)
            {
                output.Append(@"<div  class='col-sm-2'> <div class='form-group'> <label class='control-label'>" + menu.DisplayName + ":" + "</label> </div> </div>");
                output.Append(@"<div  class='col-sm-10' style='float:left;'>  <div class='form-group'>  <div class='checkbox-box' style=' margin-top:4px;'>");
                foreach (var menuAction in menuActions)
                {
                    if (menu.MenuId == menuAction.ParentID)
                    {
                        if (menuAction.Name.ToLower() != "invisible")
                        {
                            output.Append(@"<input name='RoleIds' class='clsRights' type='checkbox' value=" + menuAction.MenuId + " cat='" + menuAction.CheckboxId + "' style='margin-right:8px;'/>");
                            output.Append(@"<span style='margin-right:10px;'>" + menuAction.Name + "</span>");
                        }
                    }
                }
                output.Append(@"</div></div></div>");
            }
            return output;
        }

        /// <summary>
        /// Gets the role user typewise.
        /// </summary>
        /// <param name="usertypes">The usertypes.</param>
        /// <returns>role view</returns>
        public List<RoleView> GetRoleUserTypewise(string usertypes)
        {
            string spName = "[Role_GetRoleUserTypewise]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserTypes", SqlDbType.NVarChar, usertypes));
            IList<RoleView> roleList = CommonDAL.ExecuteProcedure<RoleView>(spName, sqlParameters.ToArray());
            return roleList.ToList();
        }

        /// <summary>
        /// Sets users as saas user.
        /// </summary>
        /// <param name="RoleId">The role identifier.</param>
        /// <param name="IsSAASUser">The saas user flag.</param>
        public void SetUsersAsSaas(string RoleId, bool IsSAASUser, string Invoicer)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("RoleId", SqlDbType.Int, RoleId));
            sqlParameters.Add(DBClient.AddParameters("IsSAASUser", SqlDbType.Bit, IsSAASUser));
            sqlParameters.Add(DBClient.AddParameters("Invoicer", SqlDbType.Int, Invoicer != "" ? Invoicer : (object)DBNull.Value));
            CommonDAL.Crud("UpdateUsersAsSaas", sqlParameters.ToArray());
        }

        /// <summary>
        /// Gets status of role has saas user checked.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <returns>DataSet</returns>
        public DataSet CheckRoleHasSaasUser(int RoleId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("RoleId", SqlDbType.Int, RoleId));
            DataSet dsRoles = CommonDAL.ExecuteDataSet("CheckRoleHasSaasUser", sqlParameters.ToArray());
            return dsRoles;
        }

        /// <summary>
        /// Gets the role user typewise.
        /// </summary>
        /// <param name="usertypes">The usertypes.</param>
        /// <returns>role view</returns>
        public int GetRoleWiseInvoicer(int roleId)
        {
            string spName = "[GetRoleWiseInvoicer]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("RoleId", SqlDbType.Int, roleId));
            object InvoicerList = CommonDAL.ExecuteScalar(spName, sqlParameters.ToArray());
            return Convert.ToInt32(InvoicerList);
        }
    }
}

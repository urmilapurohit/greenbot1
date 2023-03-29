using FormBot.Entity;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace FormBot.BAL.Service
{
    public interface IRoleBAL
    {
        bool CheckRoleNameExists(string name, int roleId);
        void CreateRole(RoleView roleView);
        DataSet GetRoleById(int roleId);
        void EditRole(RoleView roleView);
        void DeleteRoleMenu(int roleId);
        IList<RoleView> RoleList(int pageNumber, int pageSize, string sortCol, string sortDir, string name, int userType, string CreatedBy);
        bool DeleteRole(int roleId);
        DataSet DynamicMenuBinding(int roleId);
        DataSet CustomAuthorization(int roleId, string controller, string action);
        List<RoleView> GetData(int? userType = null);
        HtmlString DynamicRoleBinding(int userType = 1);
        StringBuilder DynamicRoleHtml(int userType);
        IList<RoleView> AdminRoleList(int pageNumber, int pageSize, string sortCol, string sortDir, string name, int userType,string CreatedBy);
        DataSet GetAdminRoleById(int roleId);
        List<RoleView> GetRoleUserTypewise(string usertypes);


        /// <summary>
        /// Sets users as saas user.
        /// </summary>
        /// <param name="RoleId">The role identifier.</param>
        /// <param name="IsSAASUser">The saas user flag.</param>
        void SetUsersAsSaas(string RoleId, bool IsSAASUser, string Invoicer);

        DataSet CheckRoleHasSaasUser(int RoleId);

        int GetRoleWiseInvoicer(int roleId);
    }
}

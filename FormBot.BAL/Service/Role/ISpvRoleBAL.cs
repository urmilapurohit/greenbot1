using FormBot.Entity;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace FormBot.BAL.Service
{
    public interface ISpvRoleBAL
    {
        DataSet DynamicSpvMenuBinding(int spvroleId);

        DataSet CustomAuthorization(int roleId, string controller, string action);
    }
}

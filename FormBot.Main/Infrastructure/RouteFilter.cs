using FormBot.BAL;
using FormBot.BAL.Service;
using FormBot.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace FormBot.Main
{


    public class MustBeFriendlyName : IRouteConstraint
    {
        //#region Properties
        //private readonly ILoginBAL _login;
        //#endregion

        //#region Constructor
        //public MustBeFriendlyName()
        //{
        //}

        ////public MustBeFriendlyName(ILoginBAL login)
        ////{
        ////    this._login = login;
        ////}
        //#endregion

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {

            // return true if this is a valid friendlyName
            // MUST BE CERTAIN friendlyName DOES NOT MATCH ANY
            // CONTROLLER NAMES OR AREA NAMES

             
            //List<SqlParameter> sqlParameters = new List<SqlParameter>();
            //sqlParameters.Add(DBClient.AddParameters("RoleId", SqlDbType.BigInt, roleId));
            //DataSet dsRoles = CommonDAL.ExecuteDataSet("Login_GetMenuActionByRoleId", sqlParameters.ToArray());
            
            //var _db = new DbContext();
            //return _db.FriendlyNames.FirstOrDefault(x => x.FriendlyName.ToLowerInvariant() ==
            //    values[parameterName].ToString().ToLowerInvariant()) != null;
            return true;
        }
    }


}
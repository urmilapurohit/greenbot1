using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FormBot.Entity;
using System.Web.Script.Serialization;

namespace FormBot.WebAPI
{
    public static class HelperMethods
    {
        public static string ReturnResponse(bool status, string data, string error, string exceptionError = "")
        {
            ServiceResponse serviceResp = new ServiceResponse();
            serviceResp.Data = data;
            serviceResp.Status = status;
            serviceResp.Error = error;
            serviceResp.ExceptionMessage = exceptionError;
            return ConvertToJSON(serviceResp);
        }

        public static string ConvertToJSON(dynamic data)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            return json;
        }
       
    }

    public class RoleMenuView
    {
        public int MenuActionId { get; set; }
        public int MenuId { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
    }

    public class RootObject
    {
        public string LoginStatus { get; set; }
        public List<RoleMenuView> RoleMenuViews { get; set; }
    }

}
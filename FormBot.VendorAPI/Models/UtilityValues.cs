using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.VendorAPI.Models
{
    public class UtilityValues
    {
        public static string ProofDocuments
        {
            get
            {
                //string documentuploadpath = HttpContext.Current.Server.MapPath("~" + ConfigurationManager.AppSettings["ProofUploadFolder"] + "/"); 
                string documentuploadpath = ConfigurationManager.AppSettings["ProofUploadFolder"];
                return documentuploadpath;
            }
        }

    }
}
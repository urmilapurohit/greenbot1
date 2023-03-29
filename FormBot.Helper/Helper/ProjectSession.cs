using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FormBot.Helper
{
    /// <summary>
    /// Project Session
    /// </summary>
    public class ProjectSession
    {
        /// <summary>
        /// Gets or sets the name of the logged in.
        /// </summary>
        /// <value>
        /// The name of the logged in.
        /// </value>
        public static string LoggedInName
        {
            get
            {
                if (HttpContext.Current.Session["LoggedInName"] == null)
                {
                    return null;
                }
                else
                {
                    return HttpContext.Current.Session["LoggedInName"].ToString();
                }
            }
            set
            {
                HttpContext.Current.Session["LoggedInName"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the logged in user identifier.
        /// </summary>
        /// <value>
        /// The logged in user identifier.
        /// </value>
        public static int LoggedInUserId
        {
            get
            {
                if (HttpContext.Current.Session["LoggedInUserId"] == null)
                {
                    return 0;
                }
                else
                {
                    return Convert.ToInt32(HttpContext.Current.Session["LoggedInUserId"]);
                }
            }
            set
            {
                HttpContext.Current.Session["LoggedInUserId"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the logged in user's type
        /// </summary>
        public static int UserTypeId
        {
            get
            {
                if (HttpContext.Current.Session["UserTypeId"] == null)
                {
                    return 0;
                }
                else
                {
                    return Convert.ToInt32(HttpContext.Current.Session["UserTypeId"]);
                }
            }
            set
            {
                HttpContext.Current.Session["UserTypeId"] = value;
            }
        }
        public static bool IsNewViewer
        {
            get
            {
                if (HttpContext.Current.Session["IsNewViewer"] == null)
                {
                    return false;
                }
                else
                {
                    return Convert.ToBoolean(HttpContext.Current.Session["IsNewViewer"]);
                }
            }
            set
            {
                HttpContext.Current.Session["IsNewViewer"] = value;
            }
        }

        public static bool? IsresetPwd
        {
            get
            {
                if (HttpContext.Current.Session["IsresetPwd"] == null)
                {
                    return null;


                }
                else
                {
                    return Convert.ToBoolean(HttpContext.Current.Session["IsresetPwd"]);
                }
            }
            set
            {
                HttpContext.Current.Session["IsresetPwd"] = value;
            }
        }

        /// <summary>
        /// Store userwise tabular view show or default
        /// </summary>
        public static bool IsTabularView
        {
            get
            {
                if (HttpContext.Current.Session["IsTabularView"] == null)
                {
                    return false;
                }
                else
                {
                    return Convert.ToBoolean(HttpContext.Current.Session["IsTabularView"]);
                }
            }
            set
            {
                HttpContext.Current.Session["IsTabularView"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the logged in user's role id
        /// </summary>
        public static int RoleId
        {
            get
            {
                if (HttpContext.Current.Session["RoleId"] == null)
                {
                    return 0;
                }
                else
                {
                    return Convert.ToInt32(HttpContext.Current.Session["RoleId"]);
                }
            }
            set
            {
                HttpContext.Current.Session["RoleId"] = value;
            }
        }

        /// <summary>
        /// Gets or sets logged in user's ResellerId if user is reseller
        /// </summary>
        public static int ResellerId
        {
            get
            {
                if (HttpContext.Current.Session["ResellerId"] == null)
                {
                    return 0;
                }
                else
                {
                    return Convert.ToInt32(HttpContext.Current.Session["ResellerId"]);
                }
            }
            set
            {
                HttpContext.Current.Session["ResellerId"] = value;
            }
        }

        /// <summary>
        /// Get or set value for reseller's theme
        /// </summary>
        public static string Theme
        {
            get
            {
                if (HttpContext.Current.Session["Theme"] == null)
                {
                    return string.Empty;
                }
                else
                {
                    return Convert.ToString(HttpContext.Current.Session["Theme"]);
                }
            }
            set
            {
                HttpContext.Current.Session["Theme"] = value;
            }
        }

        /// <summary>
        /// Gets or set value for reseller's logo
        /// </summary>
        public static string Logo
        {
            get
            {
                if (HttpContext.Current.Session["Logo"] == null)
                {
                    return string.Empty;
                }
                else
                {
                    return Convert.ToString(HttpContext.Current.Session["Logo"]);
                }
            }
            set
            {
                HttpContext.Current.Session["Logo"] = value;
            }
        }

        /// <summary>
        /// Gets or sets logged in user's LoginCompanyName if user is reseller
        /// </summary>
        public static string LoginCompanyName
        {
            //get;
            //set;
            get
            {
                HttpCookie objCookie = HttpContext.Current.Request.Cookies.Get("LoginCompanyName");
                if (objCookie != null)
                {
                    return objCookie["LoginCompanyName"];
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                HttpCookie objCookie = new HttpCookie("LoginCompanyName");
                //objCookie.Expires = DateTime.Now.AddDays(-1);
                objCookie["LoginCompanyName"] = value;
                objCookie.Expires = DateTime.MaxValue;
                HttpContext.Current.Response.Cookies.Add(objCookie);
            }
        }

        /// <summary>
        /// Gets or sets logged in user's SolarCompanyId
        /// </summary>
        public static int SolarCompanyId
        {
            get
            {
                if (HttpContext.Current.Session["SolarCompanyId"] == null)
                {
                    return 0;
                }
                else
                {
                    return Convert.ToInt32(HttpContext.Current.Session["SolarCompanyId"]);
                }
            }
            set
            {
                HttpContext.Current.Session["SolarCompanyId"] = value;
            }
        }

        public static bool IsSubContractor
        {
            get
            {
                if (HttpContext.Current.Session["IsSubContractor"] == null)
                {
                    return false;
                }
                else
                {
                    return Convert.ToBoolean(HttpContext.Current.Session["IsSubContractor"]);
                }
            }
            set
            {
                HttpContext.Current.Session["IsSubContractor"] = value;
            }
        }

        public static bool IsSSCReseller
        {
            get
            {
                if (HttpContext.Current.Session["IsSSCReseller"] == null)
                {
                    return false;
                }
                else
                {
                    return Convert.ToBoolean(HttpContext.Current.Session["IsSSCReseller"]);
                }
            }
            set
            {
                HttpContext.Current.Session["IsSSCReseller"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        public static string ConnectionString
        {
            get
            {
                if (HttpContext.Current.Session["ConnectionString"] == null)
                {
                    return string.Empty;
                }
                else
                {
                    return Convert.ToString(HttpContext.Current.Session["ConnectionString"]);
                }
            }
            set
            {
                HttpContext.Current.Session["ConnectionString"] = value;
            }
        }



        /// <summary>
        /// Gets or sets the name of the SMS global user.
        /// </summary>
        /// <value>
        /// The name of the SMS global user.
        /// </value>
        public static string SMSGlobalUserName
        {
            get
            {
                if (HttpContext.Current.Session["SMSGlobalUserName"] == null)
                {
                    return ConfigurationManager.AppSettings["SMSGlobalUserName"].ToString();
                }
                else
                {
                    return Convert.ToString(HttpContext.Current.Session["SMSGlobalUserName"]);
                }
            }
            set
            {
                HttpContext.Current.Session["SMSGlobalUserName"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the SMS global password.
        /// </summary>
        /// <value>
        /// The SMS global password.
        /// </value>
        public static string SMSGlobalPassword
        {
            get
            {
                if (HttpContext.Current.Session["SMSGlobalPassword"] == null)
                {
                    return ConfigurationManager.AppSettings["SMSGlobalPassword"].ToString();
                }
                else
                {
                    return Convert.ToString(HttpContext.Current.Session["SMSGlobalPassword"]);
                }
            }
            set
            {
                HttpContext.Current.Session["SMSGlobalPassword"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the SMS global from.
        /// </summary>
        /// <value>
        /// The SMS global from.
        /// </value>
        public static string SMSGlobalFrom
        {
            get
            {
                if (HttpContext.Current.Session["SMSGlobalFrom"] == null)
                {
                    return ConfigurationManager.AppSettings["SMSGlobalFrom"].ToString();
                }
                else
                {
                    return Convert.ToString(HttpContext.Current.Session["SMSGlobalFrom"]);
                }
            }
            set
            {
                HttpContext.Current.Session["SMSGlobalFrom"] = value;
            }
        }


        /// <summary>
        /// SMTP detail - Mail From key
        /// </summary>
        public static string MailFrom
        {
            get
            {
                return ConfigurationManager.AppSettings["MailFrom"];
            }
        }

        /// <summary>
        /// SMTP detail - Host key
        /// </summary>
        public static string SMTPHost
        {
            get
            {
                return ConfigurationManager.AppSettings["SMTPHost"];
            }
        }

        /// <summary>
        /// SMTP detail - Username key
        /// </summary>
        public static string SMTPUserName
        {
            get
            {
                return ConfigurationManager.AppSettings["SMTPUserName"];
            }
        }

        /// <summary>
        /// SMTP detail - Password key
        /// </summary>
        public static string SMTPPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["SMTPPassword"];
            }
        }

        /// <summary>
        /// SMTP detail - Port key
        /// </summary>
        public static int SMTPPort
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);
            }
        }

        /// <summary>
        /// SMTP detail - SSL key
        /// </summary>
        public static bool IsSMTPEnableSsl
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["IsSMTPEnableSsl"]);
            }
        }

        /// <summary>
        /// Gets Site Url Base
        /// </summary>
        public static string SiteUrlBase
        {
            get
            {
                if (HttpContext.Current.Request.IsSecureConnection)
                {
                    return SecureUrlBase;
                }
                else
                {
                    return UrlBase;
                }
            }
        }

        /// <summary>
        /// Gets Secure User Base
        /// </summary>
        public static string SecureUrlBase
        {
            get
            {
                return "https://" + UrlSuffix;
            }
        }

        /// <summary>
        /// Gets Url Base
        /// </summary>
        public static string UrlBase
        {
            get
            {
                return "http://" + UrlSuffix;
            }
        }

        /// <summary>
        /// Fetch email template
        /// </summary>
        public static string EmailTemplatePath
        {
            get
            {
                string path = System.IO.Path.GetDirectoryName((new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath);
                string culture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
                return string.Format("{0}/MailTemplates/{1}", path, culture);
            }
        }

        /// <summary>
        /// Get the path for Request Documents upload.
        /// </summary>
        public static string ProofDocuments
        {
            get
            {
                //string documentuploadpath = HttpContext.Current.Server.MapPath("~" + ConfigurationManager.AppSettings["ProofUploadFolder"] + "/"); 
                string documentuploadpath = ConfigurationManager.AppSettings["ProofUploadFolder"];
                return documentuploadpath;
            }
        }

        /// <summary>
        /// Get the site path for Uploaded Document Path 
        /// </summary>
        public static string UploadedDocumentPath
        {
            get
            {
                return ConfigurationManager.AppSettings["UploadedDocumentPath"];
            }
        }
        public static string STCInvoiceDocumentPath
        {
            get
            {
                return ConfigurationManager.AppSettings["ProofUploadFolder"];
            }
        }
        /// <summary>
        /// Get the site path for Request Documents
        /// </summary>
        public static string ProofDocumentsURL
        {
            get
            {
                return ConfigurationManager.AppSettings["ProofUploadFolder"] + "/";
            }
        }

        /// <summary>
        /// Gets the maximum size of the image.
        /// </summary>
        /// <value>
        /// The maximum size of the image.
        /// </value>
        public static int MaxImageSize
        {
            get
            {
                if (ConfigurationManager.AppSettings["MaxImageSize"] != null)
                {
                    return Convert.ToInt32(ConfigurationManager.AppSettings["MaxImageSize"]);
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Gets the maximum size of the logo.
        /// </summary>
        /// <value>
        /// The maximum size of the logo.
        /// </value>
        public static int MaxLogoSize
        {
            get
            {
                if (ConfigurationManager.AppSettings["MaxLogoSize"] != null)
                {
                    return Convert.ToInt32(ConfigurationManager.AppSettings["MaxLogoSize"]);
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Get the Url for CalculateStc 
        /// </summary>
        public static string CalculateSTCUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["CalculateSTCUrl"];
            }
        }

        /// <summary>
        /// Get the Url for CalculateSWHStc 
        /// </summary>
        public static string CalculateSWHSTCUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["CalculateSWHSTCUrl"];
            }
        }

        /// <summary>
        /// Get the Url for JobMapKey 
        /// </summary>
        public static string JobMapKeyUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["JobMapKeyUrl"];
            }
        }

        public static string LoginLink
        {
            get
            {
                return ConfigurationManager.AppSettings["LoginLink"].ToString();
            }
        }

        /// <summary>
        /// Gets the abn link.
        /// </summary>
        /// <value>
        /// The abn link.
        /// </value>
        public static string ABNLink
        {
            get
            {
                return ConfigurationManager.AppSettings["ABNLink"];
            }
        }

        /// <summary>
        /// Gets the authentication unique identifier.
        /// </summary>
        /// <value>
        /// The authentication unique identifier.
        /// </value>
        public static string AuthenticationGuid
        {
            get
            {
                return ConfigurationManager.AppSettings["AuthenticationGuid"];
            }
        }

        /// <summary>
        /// Get the path of call back url of xero
        /// </summary>
        public static string XeroAuthorizeURL
        {
            get
            {
                if (HttpContext.Current.Session["XeroAuthorizeURL"] == null)
                {
                    return ConfigurationManager.AppSettings["XeroAuthorizeURL"].ToString();
                }
                else
                {
                    return Convert.ToString(HttpContext.Current.Session["XeroAuthorizeURL"]);
                }
            }
            set
            {
                HttpContext.Current.Session["XeroAuthorizeURL"] = value;
            }
        }

        /// <summary>
        /// Gets Url Suffix
        /// </summary>
        private static string UrlSuffix
        {
            get
            {
                if (HttpContext.Current.Request.ApplicationPath == "/")
                {
                    return HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath;
                }
                else
                {
                    return HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath + "/";
                }
            }
        }

        /// <summary>
        /// Gets or sets the tax rate.
        /// </summary>
        /// <value>
        /// The logged in user identifier.
        /// </value>
        public static decimal? PartAccountTax
        {
            get
            {
                if (HttpContext.Current.Session["PartAccountTax"] == null)
                {
                    return 0;
                }
                else
                {
                    return Convert.ToDecimal(HttpContext.Current.Session["PartAccountTax"]);
                }
            }
            set
            {
                HttpContext.Current.Session["PartAccountTax"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the tax inclusive value.
        /// </summary>
        /// <value>
        /// The logged in user identifier.
        /// </value>
        public static bool IsTaxInclusive
        {
            get
            {
                if (HttpContext.Current.Session["IsTaxInclusive"] == null)
                {
                    return false;
                }
                else
                {
                    return Convert.ToBoolean(HttpContext.Current.Session["IsTaxInclusive"]);
                }
            }
            set
            {
                HttpContext.Current.Session["IsTaxInclusive"] = value;
            }
        }

        /// <summary>		
        /// Gets or sets the tax inclusive value.		
        /// </summary>		
        /// <value>		
        /// The logged in user identifier.		
        /// </value>		
        public static bool IsUserFSA
        {
            get
            {
                if (HttpContext.Current.Session["IsUserFSA"] == null)
                {
                    return false;
                }
                else
                {
                    return Convert.ToBoolean(HttpContext.Current.Session["IsUserFSA"]);
                }
            }
            set
            {
                HttpContext.Current.Session["IsUserFSA"] = value;
            }
        }

        ///// <summary>		
        ///// Gets or sets system user type		
        ///// </summary>		
        ///// <value>		
        ///// The name of the logged in.		
        ///// </value>		
        //public static List<SelectListItem> SystemUserType
        //{
        //    get
        //    {
        //        if (HttpContext.Current.Session["SystemUserType"] == null)
        //        {
        //            return null;
        //        }
        //        else
        //        {
        //            return HttpContext.Current.Session["SystemUserType"] as List<SelectListItem>;
        //        }
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session["SystemUserType"] = value;
        //    }
        //}
        ///// <summary>		
        ///// Gets or sets system users of userType		
        ///// </summary>		
        ///// <value>		
        ///// The name of the logged in.		
        ///// </value>		
        //public static List<SelectListItem> SystemUsersOfUserType
        //{
        //    get
        //    {
        //        if (HttpContext.Current.Session["SystemUsersOfUserType"] == null)
        //        {
        //            return null;
        //        }
        //        else
        //        {
        //            return HttpContext.Current.Session["SystemUsersOfUserType"] as List<SelectListItem>;
        //        }
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session["SystemUsersOfUserType"] = value;
        //    }
        //}

        /// <summary>		
        /// Gets or sets the logged in fsa user identifier.		
        /// </summary>		
        /// <value>		
        /// The logged in user identifier.		
        /// </value>		
        public static int FSALoggedInUserId
        {
            get
            {
                if (HttpContext.Current.Session["FSALoggedInUserId"] == null)
                {
                    return 0;
                }
                else
                {
                    return Convert.ToInt32(HttpContext.Current.Session["FSALoggedInUserId"]);
                }
            }
            set
            {
                HttpContext.Current.Session["FSALoggedInUserId"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the Sale Account Code.
        /// </summary>
        /// <value>
        /// The logged in user identifier.
        /// </value>
        public static string SaleAccountCode
        {
            get
            {
                if (HttpContext.Current.Session["SaleAccountCode"] == null)
                {
                    return null;
                }
                else
                {
                    return Convert.ToString(HttpContext.Current.Session["SaleAccountCode"]);
                }
            }
            set
            {
                HttpContext.Current.Session["SaleAccountCode"] = value;
            }
        }
        public static bool IsAllowTrade
        {
            get
            {
                if (HttpContext.Current.Session["IsAllowTrade"] == null)
                {
                    return true;
                }
                else
                {
                    return Convert.ToBoolean(HttpContext.Current.Session["IsAllowTrade"]);
                }
            }
            set
            {
                HttpContext.Current.Session["IsAllowTrade"] = value;
            }
        }

        #region Email configuration not required
        /// <summary>
        /// Set to true if User email account is cofigured
        /// </summary>
        public static bool IsUserEmailAccountConfigured
        {
            get
            {
                if (HttpContext.Current.Session["IsUserEmailAccountConfigured"] == null)
                {
                    return false;
                }
                else
                {
                    return Convert.ToBoolean(HttpContext.Current.Session["IsUserEmailAccountConfigured"]);
                }
            }
            set
            {
                HttpContext.Current.Session["IsUserEmailAccountConfigured"] = value;
            }
        }

        ///// <summary>  
        ///// Gets or sets system user type  
        ///// </summary>  
        ///// <value>  
        ///// The name of the logged in.  
        ///// </value>  
        public static string SystemUserType
        {
            get
            {
                if (HttpContext.Current.Session["SystemUserType"] == null)
                {
                    return null;
                }
                else
                {
                    return HttpContext.Current.Session["SystemUserType"].ToString();
                }
            }
            set
            {
                HttpContext.Current.Session["SystemUserType"] = value;
            }
        }
        /// <summary>  
        /// Gets or sets system users of userType  
        /// </summary>  
        /// <value>  
        /// The name of the logged in.  
        /// </value>  
        public static string SystemUsersOfUserType
        {
            get
            {
                if (HttpContext.Current.Session["SystemUsersOfUserType"] == null)
                {
                    return null;
                }
                else
                {
                    return HttpContext.Current.Session["SystemUsersOfUserType"].ToString();
                }
            }
            set
            {
                HttpContext.Current.Session["SystemUsersOfUserType"] = value;
            }
        }

        /// <summary>
        /// get system Reseller Table
        /// </summary>
        public static string SystemReseller
        {
            get
            {
                if (HttpContext.Current.Session["SystemReseller"] == null)
                {
                    return null;
                }
                else
                {
                    return HttpContext.Current.Session["SystemReseller"].ToString();
                }
            }
            set
            {
                HttpContext.Current.Session["SystemReseller"] = value;
            }
        }

        public static string SystemSolarCompanyByReseller
        {
            get
            {
                if (HttpContext.Current.Session["SystemSolarCompanyByReseller"] == null)
                {
                    return null;
                }
                else
                {
                    return HttpContext.Current.Session["SystemSolarCompanyByReseller"].ToString();
                }
            }
            set
            {
                HttpContext.Current.Session["SystemSolarCompanyByReseller"] = value;
            }
        }

        ///// <summary>
        ///// get system UserType Table
        ///// </summary>
        //public static DataTable SystemUserTypeTable
        //{
        //    get
        //    {
        //        if (HttpContext.Current.Session["SystemUserTypeTable"] == null)
        //        {
        //            return null;
        //        }
        //        else
        //        {
        //            return (DataTable)(HttpContext.Current.Session["SystemUserTypeTable"]);
        //        }
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session["SystemUserTypeTable"] = value;
        //    }
        //}

        ///// <summary>
        ///// get System Users Of UserType Table
        ///// </summary>
        //public static DataTable SystemUsersOfUserTypeTable
        //{
        //    get
        //    {
        //        if (HttpContext.Current.Session["SystemUsersOfUserTypeTable"] == null)
        //        {
        //            return null;
        //        }
        //        else
        //        {
        //            return (DataTable)HttpContext.Current.Session["SystemUsersOfUserTypeTable"];
        //        }
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session["SystemUsersOfUserTypeTable"] = value;
        //    }
        //}

        ///// <summary>
        ///// get system Reseller Table
        ///// </summary>
        //public static DataTable SystemResellerTable
        //{
        //    get
        //    {
        //        if (HttpContext.Current.Session["SystemResellerTable"] == null)
        //        {
        //            return null;
        //        }
        //        else
        //        {
        //            return (DataTable)HttpContext.Current.Session["SystemResellerTable"];
        //        }
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session["SystemResellerTable"] = value;
        //    }
        //}

        ///// <summary>
        ///// get system solar company Table by reseller 
        ///// </summary>
        //public static DataTable SystemSolarCompanyTableByReseller
        //{
        //    get
        //    {
        //        if (HttpContext.Current.Session["SystemSolarCompanyTableByReseller"] == null)
        //        {
        //            return null;
        //        }
        //        else
        //        {
        //            return (DataTable)HttpContext.Current.Session["SystemSolarCompanyTableByReseller"];
        //        }
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session["SystemSolarCompanyTableByReseller"] = value;
        //    }
        //}

        /// <summary>
        /// Get the Url for JobStaticMapKey 
        /// </summary>
        public static string JobStaticMapKeyUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["JobStaticMapKeyUrl"];
            }
        }


        /// <summary>
        /// Get the Url for JobStaticMapKey 
        /// </summary>
        public static string mapKey
        {
            get
            {
                return ConfigurationManager.AppSettings["mapKey"];
            }
        }


        #endregion

        public static string VendorAPIErrorLogs
        {
            get
            {
                // return ConfigurationManager.AppSettings["VerdorAPIErrorLogs"];
                return AppDomain.CurrentDomain.BaseDirectory + "VendorAPIFormBotLog.txt";
            }
        }

        public static bool IsWholeSaler
        {
            get
            {
                if (HttpContext.Current.Session["IsWholeSaler"] == null)
                {
                    return false;
                }
                else
                {
                    return Convert.ToBoolean(HttpContext.Current.Session["IsWholeSaler"]);
                }
            }
            set
            {
                HttpContext.Current.Session["IsWholeSaler"] = value;
            }
        }

        public static string DynamicMenuBinding
        {
            get
            {
                if (HttpContext.Current.Session["DynamicMenuBinding"] == null)
                {
                    return null;
                }
                else
                {
                    return HttpContext.Current.Session["DynamicMenuBinding"].ToString();
                }
            }
            set
            {
                HttpContext.Current.Session["DynamicMenuBinding"] = value;
            }
        }

        public static string DynamicSpvMenuBinding
        {
            get
            {
                if (HttpContext.Current.Session["DynamicSpvMenuBinding"] == null)
                {
                    return null;
                }
                else
                {
                    return HttpContext.Current.Session["DynamicSpvMenuBinding"].ToString();
                }
            }
            set
            {
                HttpContext.Current.Session["DynamicSpvMenuBinding"] = value;
            }
        }
        public static string LogFilePath
        {
            get
            {
                //return ConfigurationManager.AppSettings["LogFilePath"];
                return AppDomain.CurrentDomain.BaseDirectory + "FormBotLog.txt";
            }
        }
        public static string EmailErrorLogFilePath
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory + "EmailErrorLog.txt";
            }
        }

        public static string LiveWebUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["LiveWebUrl"];
            }
        }

        public static object UserWiseGridConfiguration
        {
            get
            {
                if (HttpContext.Current.Session["UserWiseGridConfiguration"] == null)
                    return new object();
                else
                    return HttpContext.Current.Session["UserWiseGridConfiguration"];
            }
            set
            {
                HttpContext.Current.Session["UserWiseGridConfiguration"] = value;
            }
        }
        public static string SnackbarId
        {
            get
            {
                if (HttpContext.Current.Session["SnackbarId"] == null)
                {
                    return null;
                }
                else
                {
                    return HttpContext.Current.Session["SnackbarId"].ToString();

                }
            }
            set
            {
                HttpContext.Current.Session["SnackbarId"] = value;
            }
        }
        public static int SpvUserTypeId
        {
            get
            {
                if (HttpContext.Current.Session["SpvUserTypeId"] == null)
                {
                    return 0;
                }
                else
                {
                    return Convert.ToInt32(HttpContext.Current.Session["SpvUserTypeId"]);

                }
            }
            set
            {
                HttpContext.Current.Session["SpvUserTypeId"] = value;
            }
        }

        public static string SpvManufacturerName
        {
            get
            {
                if (HttpContext.Current.Session["SpvManufacturerName"] == null)
                {
                    return "";
                }
                else
                {
                    return HttpContext.Current.Session["SpvManufacturerName"].ToString();

                }
            }
            set
            {
                HttpContext.Current.Session["SpvManufacturerName"] = value;
            }
        }

    }

    /// <summary>
    /// Invoice Payment Details
    /// </summary>
    public class InvoicePaymentDetails
    {
        public decimal? SubTotal { get; set; }
        public decimal? Cost { get; set; }
        public decimal? Profit { get; set; }
        public decimal? Margin { get; set; }
        public decimal? Tax { get; set; }
        public decimal? SummaryTotal { get; set; }
        public decimal? Payments { get; set; }
        public decimal? Ramaning { get; set; }
    }
}

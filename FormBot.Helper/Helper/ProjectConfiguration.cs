using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FormBot.Helper
{
    public class ProjectConfiguration
    {
        public static string GetPageSize
        {
            get
            {
                return ConfigurationManager.AppSettings["GridPageSize"].ToString();
            }
        }

        public static string ProjectImagePath
        {
            get
            {
                return ConfigurationManager.AppSettings["ProjectImagePath"].ToString();
            }
        }

        public static string JobDocumentPath
        {
            get
            {
                return ProjectSession.ProofDocuments + ConfigurationManager.AppSettings["JobDocumentPath"].ToString();
            }
        }

        public static string JobDocumentsToSavePath
        {
            get
            {
                return ConfigurationManager.AppSettings["JobDocumentsToSavePath"].ToString();
            }
        }

        public static string JobDocumentsToSaveFullPath
        {
            get
            {
                return ProjectSession.ProofDocuments + JobDocumentsToSavePath;
            }
        }

        public static string GetPageSizeForMail
        {
            get
            {
                return ConfigurationManager.AppSettings["PageSize"].ToString();
            }
        }

        /// <summary>
        /// Get selected menu name using cookie
        /// </summary>
        public static string GetMenuName
        {
            get
            {
                if (HttpContext.Current.Request.Cookies["menuname"] != null)
                {
                    HttpCookie cookie = HttpContext.Current.Request.Cookies.Get("menuname");
                    return cookie.Value;
                }
                else
                {
                    return "";
                }
            }
            set
            {
                if (HttpContext.Current.Request.Cookies["menuname"] != null)
                {
                    HttpCookie cookie = HttpContext.Current.Request.Cookies.Get("menuname");
                    cookie.Value = value;
                }
            }
        }

        public static string GetDateFormat
        {
            get
            {
                return ConfigurationManager.AppSettings["DateFormat"].ToString();
            }
        }

        public static string JobInvoiceComponentUpload
        {
            get
            {
                return ConfigurationManager.AppSettings["JobInvoiceComponentUpload"].ToString();
            }
        }

        public static string XeroAuthorizeURL
        {
            get
            {
                return ConfigurationManager.AppSettings["XeroAuthorizeURL"].ToString();
            }
        }

        public static string XeroPublicAppConsumerKey
        {
            get
            {
                return ConfigurationManager.AppSettings["XeroPublicAppConsumerKey"].ToString();
            }
        }
        public static string XeroPublicAppConsumerSecret
        {
            get
            {
                return ConfigurationManager.AppSettings["XeroPublicAppConsumerSecret"].ToString();
            }
        }

        /// <summary>
        /// Gets the sessiontimeout.
        /// </summary>
        /// <value>
        /// The sessiontimeout.
        /// </value>
        public static int Sessiontimeout
        {
            get
            {
                return Convert.ToInt16(ConfigurationManager.AppSettings["Sessiontimeout"].ToString());
            }
        }

        /// <summary>
        /// Receive email from this email address at a time of REC Paperwork submission from window service
        /// </summary>
        public static string REC_PaperWork_FromEmail
        {
            get
            {
                return ConfigurationManager.AppSettings["REC_PaperWork_FromEmail"].ToString();
            }
        }

        /// <summary>
        /// Send email to this email address at a time of REC Paperwork submission from window service
        /// </summary>
        public static string REC_PaperWork_ToEmail
        {
            get
            {
                return ConfigurationManager.AppSettings["REC_PaperWork_ToEmail"].ToString();
            }
        }

        /// <summary>
        /// Get subject line for PVD Job Email
        /// </summary>
        public static string REC_PaperWork_From_SubjectLine_PVD
        {
            get
            {
                return ConfigurationManager.AppSettings["REC_PaperWork_From_SubjectLine_PVD"].ToString();
            }
        }

        /// <summary>
        /// Get subject line for SWH Job Email
        /// </summary>
        public static string REC_Site_audit_request_for_From_SubjectLine_SWH
        {
            get
            {
                return ConfigurationManager.AppSettings["REC_Site_audit_request_for_From_SubjectLine_SWH"].ToString();
            }
        }

        public static int XeroDraftsDueDate
        {
            get
            {
                return ConfigurationManager.AppSettings["XeroDraftsDueDate"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["XeroDraftsDueDate"]) : 0;
            }
        }

        /// <summary>
        /// Converts the date and time.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="format">The format.</param>
        /// <returns>Date Time</returns>
        public DateTime? ConvertDateAndTime(string date, string format)
        {
            if (!string.IsNullOrEmpty(date))
            {
                DateTime dateTime;
                if (DateTime.TryParseExact(date, format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dateTime))
                {
                    return dateTime;
                }
                return null;
            }
            else
            {
                return null;
            }
        }

        public static string SettlementCutOffTime
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["SettlementCutOffTime"]);
            }
        }
        public static string OnApprovalSettlementCutOffTime
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["OnApprovalSettlementCutOffTime"]);
            }
        }

        public static string ScriptVersion
        {
            get
            {
                return ConfigurationManager.AppSettings["ScriptVersion"].ToString();
            }
        }
        public static string JobSizeForOptiPay
        {
            get
            {
                return ConfigurationManager.AppSettings["JobSizeForOptiPay"].ToString();
            }
        }

        public static Int32 GetCacheTime
        {
            get
            {
                if (ConfigurationManager.AppSettings["CacheTimeInMins"] != null && Convert.ToInt32(ConfigurationManager.AppSettings["CacheTimeInMins"].ToString()) > 0)
                    return Convert.ToInt32(ConfigurationManager.AppSettings["CacheTimeInMins"].ToString());
                else
                    return 300;
            }
        }

        public static string SMSGlobalURL
        {
            get
            {
                return ConfigurationManager.AppSettings["SMSGlobalURL"].ToString();
            }
        }

        public static string VEECUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["VEECUrl"].ToString();
            }
        }

        public static string VEECHost
        {
            get
            {
                return ConfigurationManager.AppSettings["VEECHost"].ToString();
            }
        }

        public static string TemplateForBulkUploadSolarJobsPath
        {
            get
            {
                return ProjectSession.ProofDocuments + ConfigurationManager.AppSettings["TemplateForBulkUploadSolarJobsPath"].ToString();
            }
        }

        public static string TemplateForUploadSpvPanelDetailsPath
        {
            get
            {
                return System.Web.HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["TemplateForUploadSpvPanelDetailsPath"].ToString());
                //return Path.Combine(, ConfigurationManager.AppSettings["TemplateForUploadSpvPanelDetailsPath"].ToString());
            }
        }

        public static string GetCompanyABNSearchLink
        {
            get
            {
                return ConfigurationManager.AppSettings["ABNSearch"].ToString();
            }
        }

        public static string BatchRecUploadCount
        {
            get
            {
                return ConfigurationManager.AppSettings["BatchRecUploadCount"].ToString();
            }
        }
        public static string BatchRecUploadCountForCsv
        {
            get
            {
                return ConfigurationManager.AppSettings["BatchRecUploadCountForCsv"].ToString();
            }
        }

        public static string RecTimeCheck
        {
            get
            {
                return ConfigurationManager.AppSettings["RecTimeCheck"].ToString();
            }
        }

        public static string ProofDocumentsURL
        {
            get
            {
                return ConfigurationManager.AppSettings["ProofUploadFolder"] + "/";
            }
        }

        public static string UploadedDocumentPath
        {
            get
            {
                return ConfigurationManager.AppSettings["UploadedDocumentPath"];
            }
        }

        public static string RECUploadUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["RECUploadUrl"].ToString();
            }
        }

        public static string RECHost
        {
            get
            {
                return ConfigurationManager.AppSettings["RECHost"].ToString();
            }
        }

        public static string SPVReferenceJsonUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["SPVReferenceJsonUrl"];
            }
        }

        public static string InstallationVerificationXMLPath
        {
            get
            {
                return ConfigurationManager.AppSettings["InstallationVerificationXMLPath"];
            }
        }

        public static string NotesXMLPath
        {
            get
            {
                return ConfigurationManager.AppSettings["NotesXMLPath"];
            }
        }
        public static string ProductVerificationXMLPath
        {
            get
            {
                return ConfigurationManager.AppSettings["ProductVerificationXMLPath"];
            }
        }

        public static string ServerCertificate
        {
            get
            {
                return ConfigurationManager.AppSettings["ServerCertificate"];
            }
        }
        public static string RECXmlPath
        {
            get
            {
                return ConfigurationManager.AppSettings["RECXmlPath"];
            }
        }
        public static string ServerCertificatePassword
        {
            get
            {
                return ConfigurationManager.AppSettings["ServerCertificatePassword"];
            }
        }
        public static string GlobalLevelSpvRequiredKeyName
        {
            get
            {
                return ConfigurationManager.AppSettings["GlobalLevelSpvRequiredKeyName"];
            }
        }
        public static string ServiceAdministrator
        {
            get
            {
                return ConfigurationManager.AppSettings["ServiceAdministrator"].ToString();
            }
        }

        public static double scaleFactor
        {
            get
            {
                return Convert.ToDouble(ConfigurationManager.AppSettings["ScaleFactor"]);
            }
        }

        public static string ShortNameOfSerialNumberPhoto
        {
            get
            {
                return ConfigurationManager.AppSettings["ShortNameOfSerialNumberPhoto"].ToString();
            }
        }
        public static bool IsSpvRequestResponseResultSaveInTextFile
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["IsSpvRequestResponseResultSaveInTextFile"].ToString());
            }
        }
        public static string SyncAccreditedInstallerListCurrentURL
        {
            get
            {
                return ConfigurationManager.AppSettings["SyncAccreditedInstallerListCurrentURL"].ToString();
            }
        }
        public static string SyncAccreditedInstallerListHistoricalURL
        {
            get
            {
                return ConfigurationManager.AppSettings["SyncAccreditedInstallerListHistoricalURL"].ToString();
            }
        }
        public static string SyncAccreditedInstallerListUsername
        {
            get
            {
                return ConfigurationManager.AppSettings["SyncAccreditedInstallerListUsername"].ToString();
            }
        }
        public static string SyncAccreditedInstallerListPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["SyncAccreditedInstallerListPassword"].ToString();
            }
        }

        public static string CompanyNameForNewLoginScreen
        {
            get
            {
                return ConfigurationManager.AppSettings["CompanyNameForNewLoginScreen"].ToString();
            }
        }

        /// <summary>
        /// Rec Registry UserName
        /// </summary>
        public static string RECUserName
        {
            get
            {
                return ConfigurationManager.AppSettings["RECUserName"].ToString();
            }
        }

        /// <summary>
        /// Rec Registry Password
        /// </summary>
        public static string RECPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["RECPassword"].ToString();
            }
        }

        /// <summary>
        /// Rec Registry Auth URL
        /// </summary>
        public static string RECAuthURL
        {
            get
            {
                return ConfigurationManager.AppSettings["RECAuthURL"].ToString();
            }
        }

        /// <summary>
        /// Rec Registry Register URL
        /// </summary>
        public static string RECRegisterURL
        {
            get
            {
                return ConfigurationManager.AppSettings["RECRegisterURL"].ToString();
            }
        }

        /// <summary>
        /// Rec Registry Search URL
        /// </summary>
        public static string RECSearchURL
        {
            get
            {
                return ConfigurationManager.AppSettings["RECSearchURL"].ToString();
            }
        }

        public static string RECDataURL
        {
            get
            {
                return ConfigurationManager.AppSettings["RECDataURL"].ToString();
            }
        }

        public static double SPVTimeOut
        {
            get
            {
                return Convert.ToDouble(ConfigurationManager.AppSettings["SPVTimeOut"]);
            }
        }
        public static string IsAllowedAccessToAllUsersKeyName
        {
            get
            {
                return ConfigurationManager.AppSettings["IsAllowedAccessToAllUsersKeyName"];
            }
        }
        public static bool AllowAccessForCreateInRec
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["AllowAccessForCreateInRec"].ToString
                    ());
            }
        }
        public static string GroupId
        {
            get
            {
                return ConfigurationManager.AppSettings["GroupId"].ToString();
            }
        }

        public static string Domain
        {
            get
            {
                return ConfigurationManager.AppSettings["Domain"].ToString();
            }
        }
        public static string BitLyToken
        {
            get
            {
                return ConfigurationManager.AppSettings["BitLyToken"].ToString();
            }
        }
        public static string BitLyURL
        {
            get
            {
                return ConfigurationManager.AppSettings["BitLyURL"].ToString();
            }
        }
        public static string CERApprovedPVModulesUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["CERApprovedPVModulesUrl"];
            }
        }
        public static int UrgentSTCStatusJobDay
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["SetUrgentSTCStatusJobDay"].ToString
                    ());
            }
        }
        public static string RECSearchByPVDCode2
        {
            get
            {
                return ConfigurationManager.AppSettings["RECSearchByPVDCode2"];
            }
        }

        public static int DistributedCacheTimeOutInMinutes
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["DistributedCacheTimeOutInMinutes"].ToString());
            }
        }

        public static int JobsToGroupPerRedisCache
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["JobsToGroupPerRedisCache"].ToString());
            }
        }


        public static int RedisCacheBatchCommandCount
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["RedisCacheBatchCommandCount"].ToString());
            }
        }

        public static int ArchiveMinYear
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["ArchiveMinYear"].ToString());
            }
        }
        public static string RECDataLimitDate
        {
            get
            {
                return ConfigurationManager.AppSettings["RECDataLimitDate"].ToString();
            }
        }
        /// <summary>
        /// Rec Registry Search URL
        /// </summary>
        public static string RECSearchURLForSWH
        {
            get
            {
                return ConfigurationManager.AppSettings["RECSearchURLForSWH"].ToString();
            }
        }
        public static string RECSearchByPVDCode2ForSWH
        {
            get
            {
                return ConfigurationManager.AppSettings["RECSearchByPVDCode2ForSWH"];
            }
        }

    }
}








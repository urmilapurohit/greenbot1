using FormBot.BAL.Service;
using FormBot.Helper;
using FormBot.Helper.Helper;
using Ionic.Zlib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Collections.ObjectModel;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using System.Xml.Linq;
using System.Xml;
using System.Configuration;
using System.Threading.Tasks;
using OpenQA.Selenium.Interactions;

namespace FormBot.Main.Infrastructure
{
    public class RECRegistryHelper
    {

        private readonly ISTCInvoiceBAL _stcInvoiceServiceBAL;
        private readonly ICreateJobBAL _createJobBAL;
        private static readonly ILogger _log;

        static RECRegistryHelper()
        {
            _log = new Logger();
        }

        public RECRegistryHelper(ISTCInvoiceBAL stcInvoice, ICreateJobBAL createJobBAL)
        {
            this._stcInvoiceServiceBAL = stcInvoice;
            this._createJobBAL = createJobBAL;
        }

        /// <summary>
        /// Authenticate use for REC and upload file into REC
        /// </summary>
        /// <param name="FilePath">The file path.</param>
        /// <param name="JobsSerialNumbers">The jobs serial numbers.</param>
        /// <param name="JsonResponseObj">The json response object.</param>
        /// <param name="dtCSV_JobID">The dt cs v_ job identifier.</param>
        /// <param name="UploadURL">The upload URL.</param>
        /// <param name="referer">The referer.</param>
        /// <param name="paramname">The paramname.</param>
        /// <param name="IsPVDJob">if set to <c>true</c> [is PVD job].</param>
        /// <param name="objResellerUser">The object reseller user.</param>
        //public static void AuthenticateUser_UploadFileForREC(string FilePath, string[] JobsSerialNumbers, ref clsUploadedFileJsonResponseObject JsonResponseObj, ref DataTable dtCSV_JobID, string UploadURL, string referer, string paramname, bool IsPVDJob, FormBot.Entity.User objResellerUser, string STcJobDetailsId, int UserId, string SerialNumber = "", string RecBulkUploadId = "", string RefNumber = "", string OwnerLastName = "", string FromDate = "", string spvParamName = "", string spvFilePath = "", string sguBulkUploadDocumentsParamName = "", string sguBulkUploadDocumentsFilePath = "")
        public static void AuthenticateUser_UploadFileForREC(ref clsUploadedFileJsonResponseObject JsonResponseObj, ref DataSet dsCSV_JobID, string referer, string paramname, bool IsPVDJob, FormBot.Entity.RECAccount objResellerUser, string STcJobDetailsId, int UserId, string SerialNumber = "", string RecBulkUploadId = "", string RefNumber = "", string OwnerLastName = "", string FromDate = "")
        {
            #region Login
            CookieContainer cookies = new CookieContainer();
            System.Net.ServicePointManager.Expect100Continue = false;

            ChromeOptions options = new ChromeOptions();
            options.AddArgument("no-sandbox");
            options.AddAdditionalCapability("useAutomationExtension", false);
            options.AddExcludedArgument("enable-automation");
            //options.AddArguments("headless");
            ChromeDriver driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), options, TimeSpan.FromMinutes(3));
            driver.Manage().Window.Minimize();
            driver.Manage().Timeouts().PageLoad.Add(System.TimeSpan.FromSeconds(30));

            try
            {
                driver.Navigate().GoToUrl(ProjectConfiguration.RECAuthURL);
                Thread.Sleep(5000);
                IWebElement ele = driver.FindElement(By.Id("signInName"));
                IWebElement ele2 = driver.FindElement(By.Id("password"));
                ele.SendKeys(objResellerUser.CERLoginId);
                ele2.SendKeys(objResellerUser.CERPassword);
                IWebElement ele1 = driver.FindElement(By.Id("next"));
                ele1.Click();
                Thread.Sleep(5000);

                ReadOnlyCollection<IWebElement> lstEle = driver.FindElements(By.ClassName("btn-primary"));
                IWebElement eleAccount = lstEle.Where(a => a.GetAttribute("onclick") == "submitUser('" + objResellerUser.RECAccName + "')").FirstOrDefault();

                if (eleAccount != null)
                {
                    eleAccount.Click();
                    bool IsTimeOut = false;
                    SearchFileResult BulkUploadIdResult = new SearchFileResult();

                    //Search by BulkUploadId
                    if (!string.IsNullOrEmpty(RecBulkUploadId))
                    {
                        _log.Log(SystemEnums.Severity.Info, "Method: AuthenticateUser_UploadFileForREC  Activity: Search by RecBulkUploadId FilePAth:");
                        BulkUploadIdResult = SearchByUploadID(IsPVDJob, RecBulkUploadId, "", "", "", "", driver);
                        if (BulkUploadIdResult.status.ToLower() == "completed")
                        {
                            _log.Log(SystemEnums.Severity.Info, "Method: AuthenticateUser_UploadFileForREC  Activity: UpdatePVDCode from RecBulkUploadId FilePAth:");
                            JsonResponseObj.IsPVDCodeUpdated = UpdatePVDCode(BulkUploadIdResult, dsCSV_JobID, IsPVDJob, UserId);
                            _log.Log(SystemEnums.Severity.Info, "Method: AuthenticateUser_UploadFileForREC  Activity: UpdatePVDCode IsPVDCodeUpdated:" + JsonResponseObj.IsPVDCodeUpdated);

                        }
                    }

                    //Search by SerialNumber
                    if (BulkUploadIdResult.status?.ToLower() != "completed" && !string.IsNullOrEmpty(SerialNumber))
                    {
                        _log.Log(SystemEnums.Severity.Info, "Method: AuthenticateUser_UploadFileForREC  Activity: Search by SerialNumber FilePAth:");
                        BulkUploadIdResult = SearchByUploadID(IsPVDJob, string.Empty, SerialNumber, RefNumber, OwnerLastName, FromDate, driver);
                        if (BulkUploadIdResult.status?.ToLower() == "completed")
                        {
                            if (BulkUploadIdResult.result.results.Length > 0)
                            {
                                STCInvoiceBAL obj = new STCInvoiceBAL();
                                obj.UpdateRECUploadId(STcJobDetailsId, BulkUploadIdResult.result.results[0].bulkUploadId);
                                //for cache update rec bulk upload id

                                SearchFileResult PVDCodeResult = SearchByUploadID(IsPVDJob, Convert.ToString(BulkUploadIdResult.result.results[0].bulkUploadId),driver:driver);
                                if (PVDCodeResult.status.ToLower() == "completed")
                                {
                                    _log.Log(SystemEnums.Severity.Info, "Method: AuthenticateUser_UploadFileForREC  Activity: UpdatePVDCode from search by serial number FilePAth:");
                                    JsonResponseObj.IsPVDCodeUpdated = UpdatePVDCode(PVDCodeResult, dsCSV_JobID, IsPVDJob, UserId);
                                    CreateJobBAL createJob = new CreateJobBAL();
                                    DataTable dt = createJob.GetSTCDetailsAndJobDataForCache(STcJobDetailsId, null);
                                    if (dt != null && dt.Rows.Count > 0)
                                    {
                                        for (int i = 0; i < dt.Rows.Count; i++)
                                        {
                                            SortedList<string, string> data = new SortedList<string, string>();
                                            string gbBatchRECUploadId = dt.Rows[i]["GBBatchRECUploadId"].ToString();
                                            data.Add("GBBatchRECUploadId", gbBatchRECUploadId);
                                            BAL.Service.CommonRules.CommonBAL.SetCacheDataForSTCSubmission(Convert.ToInt32(dt.Rows[i]["STCJobDetailsID"].ToString()), null, data).Wait(); ;
                                            Helper.Log.WriteLog(DateTime.Now + " Update cache for stcid from AUthenticateUser_UploadFIleFroRec from main: " + dt.Rows[i]["STCJobDetailsID"].ToString() + " BulkUploadId: " + gbBatchRECUploadId);
                                        }
                                    }
                                    _log.Log(SystemEnums.Severity.Info, "Method: AuthenticateUser_UploadFileForREC  Activity: UpdatePVDCode IsPVDCodeUpdated:" + JsonResponseObj.IsPVDCodeUpdated);

                                }

                                JsonResponseObj.status = "Completed";
                            }
                        }
                    }
                }
                else
                {
                    JsonResponseObj.status = "Failed";
                    JsonResponseObj.strErrors = "<ul><li>Username passsword is invalid for this reseller.</li></ul>";
                }
                #endregion
            }
            catch (Exception ex)
            {
                _log.LogException(SystemEnums.Severity.Error, "AuthenticateUser_UploadFileForREC Error", ex);
                JsonResponseObj.status = "Failed";
                JsonResponseObj.strErrors = "<ul><li>" + ex.Message + "</li></ul>";
            }
            finally
            {
                driver.Quit();
            }
        }

        /// <summary>
        /// Request to REC Registry to upload File
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="file">The file.</param>
        /// <param name="referer">The referer.</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="nvc">The NVC.</param>
        /// <param name="cookies">The cookies.</param>
        /// <param name="CSRFToken">The CSRF token.</param>
        /// <param name="JsonResponseObj">The json response object.</param>
        /// <param name="dtCSV_JobID">The dt cs v_ job identifier.</param>
        /// <param name="IsPVDJob">if set to <c>true</c> [is PVD job].</param>
        public static void HttpUploadFile_Old(string url, string file, string referer, string paramName, string contentType, NameValueCollection nvc, CookieContainer cookies, string CSRFToken,
            ref clsUploadedFileJsonResponseObject JsonResponseObj, ref DataSet dsCSV_JobID, bool IsPVDJob, string StcJobDetailsId, ref bool IsTimeOut, int UserId, string spvParamName = "",
            string spvContentType = "", string spvFile = "", string sguBulkUploadDocumentsParamName = "", string sguBulkUploadDocumentsContentType = "", string sguBulkUploadDocumentsFile = "")
        {
            _log.Log(SystemEnums.Severity.Info, "Method: HttpUploadFile  Activity: HttpUploadFile start for file,StcJobDetailsId,spvFile,sguBulkUploadDocumentsFile:" + file + "," + StcJobDetailsId + "," + spvFile + "," + sguBulkUploadDocumentsFile);

            //if (!string.IsNullOrEmpty(spvParamName))
            //    wr.Host = "test.rec-registry.gov.au";
            //else
            //    wr.Host = "www.rec-registry.gov.au";

            //string boundary = "----WebKitFormBoundary" + DateTime.Now.Ticks.ToString("x");
            string boundary = "----WebKitFormBoundary" + "VhaTQYKo4QEzANTB";
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.Method = "POST";
            wr.Host = ProjectConfiguration.RECHost;
            wr.Accept = "application/json, text/javascript, */*; q=0.01";
            wr.KeepAlive = true;
            wr.Headers.Add("X-Requested-With", "XMLHttpRequest");
            wr.Headers.Add("ADRUM", "isAjax:true");
            wr.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.121 Safari/537.36";
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Headers.Add("Origin", "https://www.rec-registry.gov.au");
            wr.Referer = "https://www.rec-registry.gov.au/rec-registry/app/smallunits/register-bulk-small-generation-unit";
            wr.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            wr.Headers.Add("Accept-Language", "en-US,en;q=0.9,fa;q=0.8");

            //DateTime dtStart = DateTime.Now.ToUniversalTime();
            //Int64 i = Convert.ToInt64((dtStart - new DateTime(1970, 1, 1)).TotalMilliseconds);
            //cookies.Add(new System.Net.Cookie { Name = "fileDownload", Value = "false", Domain = "www.rec-registry.gov.au" });
            //cookies.Add(new System.Net.Cookie { Name = "SESSION", Value = "805fef4f-13a9-44fb-82d9-9dd87b696b58", Domain = "www.rec-registry.gov.au" });
            //cookies.Add(new System.Net.Cookie { Name = "JSESSIONID", Value = "3233C42D0E1509EA51BFD9EF24829F93", Domain = "www.rec-registry.gov.au" });
            //cookies.Add(new System.Net.Cookie { Name = "ADRUM", Value = "s=" + i + "&r=https://www.rec-registry.gov.au/rec-registry/app/smallunits/small-generation-unit-search?0", Domain = "www.rec-registry.gov.au" });
            wr.CookieContainer = cookies;
            wr.UseDefaultCredentials = true;
            wr.PreAuthenticate = true;
            wr.Credentials = System.Net.CredentialCache.DefaultCredentials;
            wr.Timeout = -1;
            //wr.Headers.Add("X-CSRF-TOKEN", CSRFToken);

            Stream rs = wr.GetRequestStream();



            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (string key in nvc.Keys)
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, key, nvc[key]);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }
            rs.Write(boundarybytes, 0, boundarybytes.Length);

            /***************** REC csv file *****************/
            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string header = string.Format(headerTemplate, paramName, Path.GetFileName(file), contentType);
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[fileStream.Length];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                rs.Write(buffer, 0, bytesRead);
            }
            fileStream.Close();

            if (!string.IsNullOrEmpty(spvFile) || !string.IsNullOrEmpty(sguBulkUploadDocumentsFile))
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
            }

            /***************** SPV signed data package *****************/
            if (!string.IsNullOrEmpty(spvFile))
            {
                _log.Log(SystemEnums.Severity.Info, "Method: HttpUploadFile  Activity: SPVfile:" + spvFile);

                string spvHeaderTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                //headerTemplate += "-----WebKitFormBoundary" + DateTime.Now.Ticks.ToString("x");
                string spvHeader = string.Format(spvHeaderTemplate, spvParamName, Path.GetFileName(spvFile), spvContentType);
                byte[] spvHeaderBytes = System.Text.Encoding.UTF8.GetBytes(spvHeader);
                rs.Write(spvHeaderBytes, 0, spvHeaderBytes.Length);

                FileStream zipFileStream = new FileStream(spvFile, FileMode.Open, FileAccess.Read);
                byte[] zipBuffer = new byte[zipFileStream.Length];
                int zipBytesRead = 0;
                while ((zipBytesRead = zipFileStream.Read(zipBuffer, 0, zipBuffer.Length)) != 0)
                {
                    rs.Write(zipBuffer, 0, zipBytesRead);
                }
                zipFileStream.Close();
            }

            /***************** Bulk Upload Document Zip file *****************/
            if (!string.IsNullOrEmpty(sguBulkUploadDocumentsFile))
            {
                _log.Log(SystemEnums.Severity.Info, "Method: HttpUploadFile  Activity: sguBulkUploadDocumentsFile:" + sguBulkUploadDocumentsFile);

                string sguBulkUpploadDocumentHeaderTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                string sguBulkUpploadDocumentHeader = string.Format(sguBulkUpploadDocumentHeaderTemplate, sguBulkUploadDocumentsParamName, Path.GetFileName(sguBulkUploadDocumentsFile), sguBulkUploadDocumentsContentType);
                byte[] sguBulkUpploadDocumentHeaderbytes = System.Text.Encoding.UTF8.GetBytes(sguBulkUpploadDocumentHeader);
                rs.Write(sguBulkUpploadDocumentHeaderbytes, 0, sguBulkUpploadDocumentHeaderbytes.Length);

                FileStream zipBulkUpploadDocumentStream = new FileStream(sguBulkUploadDocumentsFile, FileMode.Open, FileAccess.Read);
                byte[] zipBulkUpploadDocumentBuffer = new byte[zipBulkUpploadDocumentStream.Length];
                int zipBulkUpploadDocumentBytesRead = 0;
                while ((zipBulkUpploadDocumentBytesRead = zipBulkUpploadDocumentStream.Read(zipBulkUpploadDocumentBuffer, 0, zipBulkUpploadDocumentBuffer.Length)) != 0)
                {
                    rs.Write(zipBulkUpploadDocumentBuffer, 0, zipBulkUpploadDocumentBytesRead);
                }
                zipBulkUpploadDocumentStream.Close();
            }


            /***************** CSRF token **************************/
            string csrfHeader = "Content-Disposition: form-data; name=\"_csrf\"\r\n\r\n";
            //string token = string.Format(csrfHeader, CSRFToken);
            byte[] csrfbytes = System.Text.Encoding.UTF8.GetBytes(csrfHeader);
            rs.Write(csrfbytes, 0, csrfbytes.Length);
            byte[] tokenbytes = System.Text.Encoding.UTF8.GetBytes(CSRFToken);
            rs.Write(tokenbytes, 0, tokenbytes.Length);

            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);

            using (StreamWriter sw = new StreamWriter(@"D:\Projects\FormBot\SourceCode\FormBot01082017\FormBot.Main\UserDocuments\data\test\test.txt"))
            {
                sw.WriteLine(rs);
                sw.Close();
            }

            rs.Close();



            //WebResponse wresp = null;
            HttpWebResponse wresp = null;
            try
            {

                wresp = (HttpWebResponse)wr.GetResponse();
                Stream responseStream = responseStream = wresp.GetResponseStream();
                if (wresp.ContentEncoding.ToLower().Contains("gzip"))
                    responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
                else if (wresp.ContentEncoding.ToLower().Contains("deflate"))
                    responseStream = new DeflateStream(responseStream, CompressionMode.Decompress);

                StreamReader Reader = new StreamReader(responseStream, Encoding.Default);
                string json = Reader.ReadToEnd();

                //wresp = wr.GetResponse();
                //Stream stream2 = wresp.GetResponseStream();
                //StreamReader reader2 = new StreamReader(stream2);

                JsonResponseObj = JsonConvert.DeserializeObject<clsUploadedFileJsonResponseObject>(json);
                if (JsonResponseObj != null)
                {
                    if (JsonResponseObj.status == "Completed")
                    {
                        _log.Log(SystemEnums.Severity.Info, "Method: HttpUploadFile  Activity: Sucessfully Uploaded File In REC FilePath:" + file);
                        if ((IsPVDJob == true && JsonResponseObj.result.uploadId != 0) || (IsPVDJob == false && JsonResponseObj.result.bulkUploadId != 0))
                        {
                            //Generate datatable from CSV                            
                            //SearchFileResult model = SearchByUploadID(cookies, CSRFToken, IsPVDJob, 318723);//,106611,308546);
                            STCInvoiceBAL obj = new STCInvoiceBAL();
                            obj.UpdateRECUploadId(StcJobDetailsId, IsPVDJob == true ? JsonResponseObj.result.uploadId : JsonResponseObj.result.bulkUploadId);
                            //for cache update rec bulk upload id
                            CreateJobBAL createJob = new CreateJobBAL();
                            DataTable dt = createJob.GetSTCDetailsAndJobDataForCache(StcJobDetailsId, null);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    SortedList<string, string> data = new SortedList<string, string>();
                                    string gbBatchRECUploadId = dt.Rows[i]["GBBatchRECUploadId"].ToString();
                                    data.Add("GBBatchRECUploadId", gbBatchRECUploadId);
                                    BAL.Service.CommonRules.CommonBAL.SetCacheDataForSTCSubmission(Convert.ToInt32(dt.Rows[i]["STCJobDetailsID"].ToString()), null, data).Wait(); 
                                    Helper.Log.WriteLog(DateTime.Now + " Update cache for stcid from HttpUploadFIle_Old: " + (dt.Rows[i]["STCJobDetailsID"].ToString()) + " BulkUploadId: " + gbBatchRECUploadId);
                                }
                            }

                            SearchFileResult model = SearchByUploadID(IsPVDJob, IsPVDJob == true ? Convert.ToString(JsonResponseObj.result.uploadId) : Convert.ToString(JsonResponseObj.result.bulkUploadId));
                            if (model.status.ToLower() == "completed")
                            {
                                _log.Log(SystemEnums.Severity.Info, "Method: HttpUploadFile  Activity: UpdatePVDCode FilePath:" + file);
                                JsonResponseObj.IsPVDCodeUpdated = UpdatePVDCode(model, dsCSV_JobID, IsPVDJob, UserId);
                                _log.Log(SystemEnums.Severity.Info, "Method: HttpUlpoadFile  Activity: UpdatePVDCode IsPVDCodeUpdated:" + JsonResponseObj.IsPVDCodeUpdated);

                            }
                        }
                        // Successfully uploaded file into REC Regsitry
                    }
                    else if (JsonResponseObj.status == "Failed")
                    {
                        #region Failed Response
                        _log.Log(SystemEnums.Severity.Info, "Method: HttpUploadFile  Activity: Failed Status in uploadingFileInRec FilePath:" + file);
                        //Update RECUploadId as null
                        STCInvoiceBAL obj = new STCInvoiceBAL();

                        DataTable dtCSVRecords = dsCSV_JobID.Tables[0];
                        Int16 intLineNo = 0;
                        string strReplaceTerm = string.Empty;
                        StringBuilder sb = new StringBuilder();
                        sb.Append("<ul>");
                        _log.Log(SystemEnums.Severity.Info, "Method: HttpUploadFile  Activity: Insert Failure Reasons FilePath:" + file);
                        foreach (var item in JsonResponseObj.errors)
                        {
                            if (!string.IsNullOrEmpty(item) && item.Contains(":"))
                            {
                                string OrgItem = item;
                                if (!item.Contains("Invalid CSV file header"))
                                {
                                    string[] arrItem = item.Split(':');
                                    if (arrItem != null && arrItem.Count() > 0)
                                    {
                                        // Find Line No  (Example "Line 1: Column \"Type of system\")
                                        intLineNo = Convert.ToInt16(arrItem[0].Split(' ')[1]);
                                        if (Convert.ToString(dtCSVRecords.Rows[intLineNo - 1]["Reference"]) != string.Empty && dtCSVRecords.Rows.Count >= intLineNo)
                                        {
                                            int jobId = Convert.ToInt32(dtCSVRecords.Rows[intLineNo - 1]["JobId"]);
                                            //string JobUrl = ProjectSession.LoginLink + "Job/Index?id=" + QueryString.QueryStringEncode("id=" + Convert.ToString(jobId));
                                            //strReplaceTerm = "<a href = '" + JobUrl + "'>" + Convert.ToString(dtCSVRecords.Rows[0]["Reference"]) + "</a>";
                                            strReplaceTerm = Convert.ToString(dtCSVRecords.Rows[intLineNo - 1]["Reference"]);
                                            sb.Append("<li>" + OrgItem.Replace(arrItem[0], strReplaceTerm) + "</li>");
                                            obj.InsertRECEntryFailureReason(jobId, string.Join(" ", arrItem.Skip(1).ToArray()), UserId);
                                        }
                                        else
                                        {
                                            sb.Append("<li>" + item + "</li>");
                                        }

                                        //if (dtCSVRecords != null && dtCSVRecords.Rows.Count > 0)
                                        //{
                                        //    // Same Line No Do not need to call again
                                        //    if (intLineNo != intLineNo_Temp && intLineNo != 0)
                                        //    {
                                        //        string Installation_streetnumber = Convert.ToString(dtCSVRecords.Rows[intLineNo - 1]["Installation street number"]);
                                        //        string Installation_streetname = Convert.ToString(dtCSVRecords.Rows[intLineNo - 1]["Installation street name"]);
                                        //        string Installation_streettype = Convert.ToString(dtCSVRecords.Rows[intLineNo - 1]["Installation street type"]);
                                        //        string Installation_town_suburb = Convert.ToString(dtCSVRecords.Rows[intLineNo - 1]["Installation town/suburb"]);
                                        //        string Installation_state = Convert.ToString(dtCSVRecords.Rows[intLineNo - 1]["Installation state"]);
                                        //        string Installation_postcode = Convert.ToString(dtCSVRecords.Rows[intLineNo - 1]["Installation postcode"]);
                                        //        //Installation postcode
                                        //        string Installation_date = Convert.ToString(dtCSVRecords.Rows[intLineNo - 1]["Installation date"]);
                                        //        //strFindTerm = strFindTerm.Replace(";", "\n");
                                        //        if (!string.IsNullOrEmpty(Installation_streetnumber) && string.IsNullOrEmpty(Installation_date))
                                        //        {
                                        //            STCInvoiceBAL obj = new STCInvoiceBAL();
                                        //            DataTable dt = obj.GetJobRefNumberByAddress_InstallationDate(Installation_streetnumber, Installation_streetname, Installation_streettype, Installation_town_suburb, Installation_state, Installation_postcode, Convert.ToDateTime(Installation_date)).Tables[0];
                                        //            if (dt != null && dt.Rows.Count > 0)
                                        //            {
                                        //                strReplaceTerm = Convert.ToString(dt.Rows[0]["RefNumber"]);
                                        //                //OrgItem = OrgItem.Replace(arrItem[0], strReplaceTerm);
                                        //                sb.Append("<li>" + OrgItem.Replace(arrItem[0], strReplaceTerm) + "</li>");
                                        //                intLineNo_Temp = intLineNo;
                                        //            }
                                        //        }
                                        //        else
                                        //        {
                                        //            if (!string.IsNullOrEmpty(strReplaceTerm))
                                        //                sb.Append("<li>" + OrgItem.Replace(arrItem[0], strReplaceTerm) + "</li>");
                                        //            else
                                        //                sb.Append("<li>" + item + "</li>");
                                        //        }

                                        //    }
                                        //    else
                                        //    {
                                        //        if (!string.IsNullOrEmpty(strReplaceTerm))
                                        //            sb.Append("<li>" + OrgItem.Replace(arrItem[0], strReplaceTerm) + "</li>");
                                        //        else
                                        //            sb.Append("<li>" + item + "</li>");
                                        //    }
                                        //}

                                    }
                                }
                                else
                                    sb.Append("<li>" + item + "</li>");
                            }
                            else
                                sb.Append("<li>" + item + "</li>");
                        }
                        //obj.UpdateRECUploadId(StcJobDetailsId, 0, true);
                        obj.UpdateRECFailedRecord(StcJobDetailsId, true);
                        _log.Log(SystemEnums.Severity.Info, "Method: HttpUploadFile  Activity: UpdateRECFailedRecord for STcJobDetailsId:" + StcJobDetailsId);

                        sb.Append("</ul>");
                        JsonResponseObj.strErrors = sb.ToString();
                        #endregion
                    }
                }
                //WriteToLogFile(string.Format("File uploaded, server response is: {0}", reader2.ReadToEnd()));
            }
            catch (Exception ex)
            {
                _log.LogException(SystemEnums.Severity.Error, "HttpUploadFile Error", ex);
                if (ex.Message.ToLower() == "the remote server returned an error: (401) unauthorized.")
                {
                    JsonResponseObj.status = "Failed";
                    JsonResponseObj.strErrors = "<ul><li>" + "Please make sure username or password is correct or password should not be expired." + "</li></ul>";
                }
                else if (ex.Message.ToLower().Contains("timeout") || ex.Message.Contains("The underlying connection was closed: A connection that was expected to be kept alive was closed by the server."))
                {
                    IsTimeOut = true;
                    JsonResponseObj.status = "Failed";
                    JsonResponseObj.strErrors = "<ul><li>" + ex.Message + "</li></ul>";
                }
                else
                {
                    //WriteToLogFile("Error uploading file" + ex);
                    JsonResponseObj = new clsUploadedFileJsonResponseObject();
                    JsonResponseObj.status = "Failed";
                    //JsonResponseObj.strErrors = "<ul><li>" + ex.Message + "</li></ul>";
                    JsonResponseObj.strErrors = "<ul><li>" + ex.Message + "</li></ul>";
                }

                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
            }
            finally
            {
                wr = null;
            }
        }

        /// <summary>
        /// Request to REC Registry to upload File
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="file">The file.</param>
        /// <param name="referer">The referer.</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="nvc">The NVC.</param>
        /// <param name="cookies">The cookies.</param>
        /// <param name="CSRFToken">The CSRF token.</param>
        /// <param name="JsonResponseObj">The json response object.</param>
        /// <param name="dtCSV_JobID">The dt cs v_ job identifier.</param>
        /// <param name="IsPVDJob">if set to <c>true</c> [is PVD job].</param>
        public static void HttpUploadFile(string url, string file, string referer, string paramName, string contentType, NameValueCollection nvc, CookieContainer cookies, string CSRFToken,
            ref clsUploadedFileJsonResponseObject JsonResponseObj, ref DataSet dsCSV_JobID, bool IsPVDJob, string StcJobDetailsId, ref bool IsTimeOut, int UserId, string spvParamName = "",
            string spvContentType = "", string spvFile = "", string sguBulkUploadDocumentsParamName = "", string sguBulkUploadDocumentsContentType = "", string sguBulkUploadDocumentsFile = "",
            ChromeDriver driver = null)
        {
            _log.Log(SystemEnums.Severity.Info, "Method: HttpUploadFile  Activity: HttpUploadFile start for file,StcJobDetailsId,spvFile,sguBulkUploadDocumentsFile:" + file + "," + StcJobDetailsId + "," + spvFile + "," + sguBulkUploadDocumentsFile);
            if (driver != null)
            {
                driver.Navigate().GoToUrl(ProjectConfiguration.RECRegisterURL);
                Thread.Sleep(2000);
                IWebElement uploadsguBtn = driver.FindElement(By.Id("sguBulkUploadFile"));
                if (uploadsguBtn != null)
                {
                    uploadsguBtn.SendKeys(file);
                    Thread.Sleep(2000);
                }
                if (!string.IsNullOrEmpty(spvFile))
                {
                    IWebElement uploadsdpBtn = driver.FindElement(By.Id("sguBulkUploadSdpZip"));
                    if (uploadsguBtn != null)
                    {
                        uploadsdpBtn.SendKeys(spvFile);
                        Thread.Sleep(2000);
                    }
                }
                if (!string.IsNullOrEmpty(sguBulkUploadDocumentsFile))
                {
                    IWebElement uploaddocBtn = driver.FindElement(By.Id("sguBulkUploadDocumentsZip"));
                    if (uploadsguBtn != null)
                    {
                        uploaddocBtn.SendKeys(sguBulkUploadDocumentsFile);
                        Thread.Sleep(2000);
                    }
                }
                IWebElement uploadBtn = driver.FindElement(By.Id("sgu-file-upload-submit"));
                if (uploadBtn != null)
                {
                    uploadBtn.Click();
                    Thread.Sleep(2000);
                }
                IWebElement divError = driver.FindElement(By.Id("error"));
                if (divError != null && divError.Displayed)
                {
                    JsonResponseObj = new clsUploadedFileJsonResponseObject();
                    JsonResponseObj.status = "Failed";
                    JsonResponseObj.result = null;
                    JsonResponseObj.errors = new List<string>();
                    IWebElement ulElement = divError.FindElement(By.TagName("ul"));
                    List<IWebElement> lis = ulElement.FindElements(By.TagName("li")).ToList();
                    if (lis.Count > 0)
                    {
                        for (int i = 0; i < lis.Count; i++)
                        {
                            JsonResponseObj.errors.Add(lis[i].Text);
                        }
                    }
                }

                IWebElement divSuccess = driver.FindElement(By.Id("success"));
                if (divSuccess != null && divSuccess.Displayed)
                {
                    if (divSuccess.GetAttribute("innerHTML").Contains("Register bulk small generation units succeeded."))
                    {
                        string divHtml = divSuccess.GetAttribute("innerHTML");
                        string bulkUploadId = "";
                        //bulkUploadId = divHtml.
                        string[] stringSeparators = new string[] { "<br>" };
                        List<string> txtSuccess = divHtml.Split(stringSeparators, StringSplitOptions.None).ToList();
                        bulkUploadId = txtSuccess[1].Split(':')[1].Trim();
                        JsonResponseObj = new clsUploadedFileJsonResponseObject();
                        JsonResponseObj.status = "Completed";
                        JsonResponseObj.result = new UploadedFileStatusResult();
                        JsonResponseObj.result.bulkUploadId = Convert.ToInt32(bulkUploadId);
                        JsonResponseObj.result.uploadId = Convert.ToInt32(bulkUploadId);

                        // Successfully uploaded file into REC Regsitry
                    }
                }
                if (JsonResponseObj.status == "Completed")
                {
                    _log.Log(SystemEnums.Severity.Info, "Method: HttpUploadFile  Activity: Sucessfully Uploaded File In REC FilePath:" + file);
                    if ((IsPVDJob == true && JsonResponseObj.result.uploadId != 0) || (IsPVDJob == false && JsonResponseObj.result.bulkUploadId != 0))
                    {
                        //Generate datatable from CSV                            
                        STCInvoiceBAL obj = new STCInvoiceBAL();
                        obj.UpdateRECUploadId(StcJobDetailsId, IsPVDJob == true ? JsonResponseObj.result.uploadId : JsonResponseObj.result.bulkUploadId);
                        //for cache update rec bulk upload id
                        CreateJobBAL createJob = new CreateJobBAL();
                        DataTable dt = createJob.GetSTCDetailsAndJobDataForCache(StcJobDetailsId, null);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                SortedList<string, string> data = new SortedList<string, string>();
                                string gbBatchRECUploadId = dt.Rows[i]["GBBatchRECUploadId"].ToString();
                                data.Add("GBBatchRECUploadId", gbBatchRECUploadId);
                                BAL.Service.CommonRules.CommonBAL.SetCacheDataForSTCSubmission(Convert.ToInt32(dt.Rows[i]["STCJobDetailsID"].ToString()), null, data).Wait(); 
                                Helper.Log.WriteLog(DateTime.Now + " Update cache for stcid from HttpUploadFIle of Main: " + (dt.Rows[i]["STCJobDetailsID"].ToString()) + " BulkUploadId: " + gbBatchRECUploadId);
                            }
                        }

                        SearchFileResult model = SearchByUploadID(IsPVDJob, IsPVDJob == true ? Convert.ToString(JsonResponseObj.result.uploadId) : Convert.ToString(JsonResponseObj.result.bulkUploadId), driver: driver);
                        if (model.status.ToLower() == "completed")
                        {
                            _log.Log(SystemEnums.Severity.Info, "Method: HttpUploadFile  Activity: UpdatePVDCode FilePath:" + file);
                            JsonResponseObj.IsPVDCodeUpdated = UpdatePVDCode(model, dsCSV_JobID, IsPVDJob, UserId);
                            _log.Log(SystemEnums.Severity.Info, "Method: HttpUlpoadFile  Activity: UpdatePVDCode IsPVDCodeUpdated:" + JsonResponseObj.IsPVDCodeUpdated);

                        }
                    }
                }
                else if (JsonResponseObj.status == "Failed")
                {
                    #region Failed Response
                    _log.Log(SystemEnums.Severity.Info, "Method: HttpUploadFile  Activity: Failed Status in uploadingFileInRec FilePath:" + file);
                    //Update RECUploadId as null
                    STCInvoiceBAL obj = new STCInvoiceBAL();

                    DataTable dtCSVRecords = dsCSV_JobID.Tables[0];
                    Int16 intLineNo = 0;
                    string strReplaceTerm = string.Empty;
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<ul>");
                    _log.Log(SystemEnums.Severity.Info, "Method: HttpUploadFile  Activity: Insert Failure Reasons FilePath:" + file);
                    foreach (var item in JsonResponseObj.errors)
                    {
                        if (!string.IsNullOrEmpty(item) && item.Contains(":"))
                        {
                            string OrgItem = item;
                            if (!item.Contains("Invalid CSV file header"))
                            {
                                string[] arrItem = item.Split(':');
                                if (arrItem != null && arrItem.Count() > 0)
                                {
                                    // Find Line No  (Example "Line 1: Column \"Type of system\")
                                    intLineNo = Convert.ToInt16(arrItem[0].Split(' ')[1]);
                                    if (Convert.ToString(dtCSVRecords.Rows[intLineNo - 1]["Reference"]) != string.Empty && dtCSVRecords.Rows.Count >= intLineNo)
                                    {
                                        int jobId = Convert.ToInt32(dtCSVRecords.Rows[intLineNo - 1]["JobId"]);
                                        strReplaceTerm = Convert.ToString(dtCSVRecords.Rows[intLineNo - 1]["Reference"]);
                                        sb.Append("<li>" + OrgItem.Replace(arrItem[0], strReplaceTerm) + "</li>");
                                        obj.InsertRECEntryFailureReason(jobId, string.Join(" ", arrItem.Skip(1).ToArray()), UserId);
                                    }
                                    else
                                    {
                                        sb.Append("<li>" + item + "</li>");
                                    }
                                }
                            }
                            else
                                sb.Append("<li>" + item + "</li>");
                        }
                        else
                            sb.Append("<li>" + item + "</li>");
                    }
                    //obj.UpdateRECUploadId(StcJobDetailsId, 0, true);
                    obj.UpdateRECFailedRecord(StcJobDetailsId, true);
                    _log.Log(SystemEnums.Severity.Info, "Method: HttpUploadFile  Activity: UpdateRECFailedRecord for STcJobDetailsId:" + StcJobDetailsId);

                    sb.Append("</ul>");
                    JsonResponseObj.strErrors = sb.ToString();
                    #endregion
                }
            }
        }
        /// <summary>
        /// GetDate
        /// </summary>
        /// <param name="strDate">The string date.</param>
        /// <returns>DateTime</returns>
        public static DateTime GetDate(string strDate)
        {
            string[] formats = new string[] { "yyyy-MM-ddTHH:mm:s:ss.fffZ" };
            DateTime retValue;
            if (false == DateTime.TryParseExact(strDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out retValue))
            {
                retValue = DateTime.MinValue;
            }
            return retValue;
        }

        /// <summary>
        /// Search By Upload ID
        /// </summary>
        /// <param name="cookies">The cookies.</param>
        /// <param name="CSRFToken">The CSRF token.</param>
        /// <param name="IsPVDJob">IsPVDJob.</param>
        /// <param name="intBulkUploadID">The int bulk upload identifier.</param>
        /// <returns>search file result</returns>
        public static SearchFileResult SearchByUploadID_Old(CookieContainer cookies, string CSRFToken, bool IsPVDJob, string BulkUploadID = "", string SerialNumber = "", string refNumber = "", string ownersSurname = "", string fromDate = "")
        {

            _log.Log(SystemEnums.Severity.Info, "Method: SearchByUploadID  Activity: search from bulk upload id:" + BulkUploadID);

            HttpWebRequest wreq = null;
            if (IsPVDJob)
                wreq = (HttpWebRequest)WebRequest.Create(ProjectConfiguration.RECUploadUrl + "rec-registry/app/smallunits/sgu/search");
            else
                wreq = (HttpWebRequest)WebRequest.Create(ProjectConfiguration.RECUploadUrl + "rec-registry/app/smallunits/swh/search");
            wreq.Method = "POST";
            wreq.Timeout = -1;
            wreq.CookieContainer = cookies;
            wreq.ContentType = "application/json; charset=UTF-8";

            string[] stringSeparators = new string[] { "\r\n" };
            SerialNumber = SerialNumber.Split(stringSeparators, StringSplitOptions.None).ToList()[0];

            string toDate = DateTime.Now.ToString("yyyy-MM-ddT00:00:00.000Z");
            string frmDate = string.IsNullOrEmpty(fromDate) ? string.Empty : Convert.ToDateTime(fromDate).ToString("yyyy-MM-ddT00:00:00.000Z");
            //string frmDate = string.IsNullOrEmpty(fromDate) ? toDate : Convert.ToDateTime(fromDate).ToString("yyyy-MM-ddT00:00:00.000Z");


            string paramContent = string.Empty;
            //In REC-Registry max length = 100 for search result
            //if (!string.IsNullOrEmpty(SerialNumber)) {
            //    string[] stringSeparators = new string[] { "\r\n" };
            //    SerialNumber = SerialNumber.Split(stringSeparators, StringSplitOptions.None).ToList()[0];
            //    paramContent = "{\"displayStart\":0,\"displayLength\":100,\"sortColumn\":\"Accreditation code\",\"sortDirection\":\"ASC\",\"serialNumber\":\"" + SerialNumber.Split('\\r\\n') + "\"}";
            //}
            //else
            //    paramContent = "{\"displayStart\":0,\"displayLength\":100,\"sortColumn\":\"Accreditation code\",\"sortDirection\":\"ASC\",\"bulkUploadId\":" + intBulkUploadID + "}";

            // Removing owner surname from search parameters since commercial job considers Company details as owner name
            //paramContent = "{\"displayStart\":0,\"displayLength\":100,\"sortColumn\":\"Accreditation code\",\"sortDirection\":\"DESC\",\"serialNumber\":\"" + SerialNumber + "\",\"reference\":\"" + refNumber + "\",\"ownersSurname\":\"" + ownersSurname + "\",\"bulkUploadId\":\"" + BulkUploadID + "\",\"certificatesCreatedFromInclusive\":\"" + frmDate + "\",\"certificateCreatedTillInclusive\":\"" + toDate + "\"}";
            paramContent = "{\"displayStart\":0,\"displayLength\":10,\"sortColumn\":\"Accreditation code\",\"sortDirection\":\"ASC\",\"reference\":\"HS201239\"}";//"{\"displayStart\":0,\"displayLength\":100,\"sortColumn\":\"Accreditation code\",\"sortDirection\":\"DESC\",\"serialNumber\":\"" + SerialNumber + "\",\"reference\":\"" + refNumber + "\",\"bulkUploadId\":\"" + BulkUploadID + "\",\"certificatesCreatedFromInclusive\":\"" + frmDate + "\",\"certificateCreatedTillInclusive\":\"" + toDate + "\"}";

            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(paramContent);
            wreq.ContentLength = buffer.Length;
            wreq.Headers.Add("X-CSRF-TOKEN", CSRFToken);
            using (var request = wreq.GetRequestStream())
            {
                request.Write(buffer, 0, buffer.Length);
                request.Close();
            }
            var myHttpWebResponse = (HttpWebResponse)wreq.GetResponse();
            string strResult;
            using (var reader = new StreamReader(myHttpWebResponse.GetResponseStream(), System.Text.Encoding.UTF8))
            {
                strResult = reader.ReadToEnd();
                myHttpWebResponse.Close();
            }
            //Match m1 = Regex.Match(strResult, "name=\"registrationCode\" content=\"(.*)\"/>");
            if (strResult != null)
            {
                var model = JsonConvert.DeserializeObject<SearchFileResult>(strResult);
                return model;
            }
            return new SearchFileResult();
        }

        /// <summary>
        /// Search By Upload ID
        /// </summary>
        /// <param name="cookies">The cookies.</param>
        /// <param name="CSRFToken">The CSRF token.</param>
        /// <param name="IsPVDJob">IsPVDJob.</param>
        /// <param name="intBulkUploadID">The int bulk upload identifier.</param>
        /// <returns>search file result</returns>
        public static SearchFileResult SearchByUploadID(bool IsPVDJob, string BulkUploadID = "", string SerialNumber = "", string refNumber = "", string ownersSurname = "", string fromDate = "", ChromeDriver driver = null)
        {
            try
            {
                driver.Navigate().GoToUrl(ProjectConfiguration.RECSearchURL);
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                WebDriverWait waitLoad = new WebDriverWait(driver, TimeSpan.FromMilliseconds(300000));
                Func<IWebDriver, bool> isResponseReceivedLoad =
                    d =>
                    {
                        IWebElement eError = d.FindElement(By.Id("progress-indicator"));
                        return !eError.Displayed;
                    };
                waitLoad.Until(isResponseReceivedLoad);
                {
                    IWebElement reference = driver.FindElement(By.Id("reference"));
                    IWebElement serialnumber = driver.FindElement(By.Id("serial-number"));
                    IWebElement bulkuploadid = driver.FindElement(By.Id("bulk-upload-id"));
                    IWebElement createddateafter = driver.FindElement(By.Id("created-date-after"));
                    IWebElement createddatebefore = driver.FindElement(By.Id("created-date-before"));

                    string[] stringSeparators = new string[] { "\r\n" };
                    SerialNumber = SerialNumber.Split(stringSeparators, StringSplitOptions.None).ToList()[0];

                    string toDate = DateTime.Now.ToString("dd/MM/yyyy");
                    string frmDate = string.IsNullOrEmpty(fromDate) ? string.Empty : Convert.ToDateTime(fromDate).ToString("dd/MM/yyyy");

                    IWebElement clearBtn = driver.FindElement(By.Id("clear-button"));
                    if (clearBtn != null)
                    {
                        clearBtn.Click();
                    }

                    reference.SendKeys(refNumber);
                    serialnumber.SendKeys(SerialNumber);
                    bulkuploadid.SendKeys(BulkUploadID);
                    createddateafter.SendKeys(frmDate);
                    createddatebefore.SendKeys(toDate);

                    IWebElement searchBtn = driver.FindElement(By.Id("search-button"));
                    if (searchBtn != null)
                    {
                        js.ExecuteScript("$('#search-results').show()");
                        searchBtn.Click();
                        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(300000));
                        Func<IWebDriver, bool> isResponseReceived =
                            d =>
                            {
                                IWebElement eError = d.FindElement(By.Id("progress-indicator"));
                                return !eError.Displayed;
                            };
                        wait.Until(isResponseReceived);
                        {
                            IWebElement searchDiv = driver.FindElement(By.Name("search-sgu-table_length"));
                            searchDiv.SendKeys("50");
                            wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(300000));
                            Func<IWebDriver, bool> isResponseReceived1 =
                                d =>
                                {
                                    IWebElement eError = d.FindElement(By.Id("progress-indicator"));
                                    return !eError.Displayed;
                                };
                            wait.Until(isResponseReceived1);
                            {

                                SearchFileResult searchRes = new SearchFileResult();
                                IWebElement searchTable = driver.FindElement(By.Id("search-sgu-table"));
                                if (searchTable != null)
                                {
                                    searchRes.status = "Completed";
                                    searchRes.objectErrors = null;
                                    searchRes.result = new Result();
                                    IWebElement tBody = searchTable.FindElement(By.TagName("tbody"));
                                    List<IWebElement> rows = tBody.FindElements(By.TagName("tr")).ToList();
                                    if (rows.Count > 0)
                                    {
                                        if (Exists(FindElementSafe(driver, By.ClassName("dataTables_empty"))))
                                        {
                                            IWebElement dataEmptyRow = driver.FindElement(By.ClassName("dataTables_empty"));
                                            if (dataEmptyRow != null)
                                            {
                                                Helper.Log.WriteLog("Empty Row");
                                                searchRes.result.totalCount = 0;
                                                searchRes.result.results = new ResultDetails[0];
                                                return searchRes;
                                            }
                                        }
                                        else
                                        {
                                            rows = tBody.FindElements(By.TagName("tr")).ToList();
                                            searchRes.result.results = new ResultDetails[rows.Count];
                                            for (int i = 0; i < rows.Count; i++)
                                            {
                                                List<IWebElement> colData = rows[i].FindElements(By.TagName("td")).ToList();
                                                Helper.Log.WriteLog("ACC Code: " + colData[0].Text + "InstallerNum: " + colData[7].Text + "bulkId: " + colData[8].Text + "serialNum: " + colData[6].Text + "installationdate: " + colData[1].Text + "State: " + colData[3].Text + "OwnerName:" + colData[2].Text);
                                                ResultDetails det = new ResultDetails();
                                                det.accreditationCode = colData[0].Text;
                                                det.accreditedInstallerNumber = colData[7].Text;
                                                det.bulkUploadId = Convert.ToInt32(colData[8].Text);
                                                det.commaSeparatedSerialNumbers = colData[6].Text;

                                                det.installationDate = Convert.ToDateTime(colData[1].Text.Split('/')[2] + "-" + colData[1].Text.Split('/')[1] + "-" + colData[1].Text.Split('/')[0]);
                                                det.installationState = new Installationstate { displayName = colData[3].Text, name = colData[3].Text };
                                                det.ownerName = colData[2].Text;
                                                //det.registrationCode = colData[]
                                                searchRes.result.results[i] = det;
                                            }
                                            return searchRes;

                                        }
                                    }
                                    else
                                    {
                                    }
                                }
                            }
                        }
                    }
                }
                //menu-renewable-energy-systems
                _log.Log(SystemEnums.Severity.Info, "Method: SearchByUploadID  Activity: search from bulk upload id:" + BulkUploadID);
            }
            catch (WebDriverTimeoutException ex)
            {
                STCInvoiceBAL obj = new STCInvoiceBAL();
                obj.UpdateInternalIssue(BulkUploadID, ex.Message);
            }
            catch (Exception ex)
            {
                STCInvoiceBAL obj = new STCInvoiceBAL();
            }
            return new SearchFileResult();
        }

        public static void SearchByPVDCode(ref List<string> lstFailureReasons, ref DateTime? dateLastAudited, string STCPVDCode, int jobType, string strBulkUploadID = "", ChromeDriver driver = null)
        {

            driver.Navigate().GoToUrl(ProjectConfiguration.RECSearchURL);
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(300000));
            Func<IWebDriver, bool> isResponseReceivedLoad =
                d =>
                {
                    IWebElement eError = d.FindElement(By.Id("progress-indicator"));
                    return !eError.Displayed;
                };
            wait.Until(isResponseReceivedLoad);
            {
                IWebElement reference = driver.FindElement(By.Id("accreditation-code"));
                IWebElement bulkuploadid = driver.FindElement(By.Id("bulk-upload-id"));

                reference.Clear();
                reference.SendKeys(STCPVDCode);
                bulkuploadid.SendKeys(strBulkUploadID);

                IWebElement searchBtn = driver.FindElement(By.Id("search-button"));
                if (searchBtn != null)
                {
                    js.ExecuteScript("$('#search-results').show()");
                    searchBtn.Click();
                    SearchFileResult searchRes = new SearchFileResult();
                    wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(300000));
                    Func<IWebDriver, bool> isResponseReceived =
                        d =>
                        {
                            IWebElement eError = d.FindElement(By.Id("progress-indicator"));
                            return !eError.Displayed;
                        };
                    wait.Until(isResponseReceived);
                    {
                        IWebElement searchDiv = driver.FindElement(By.Name("search-sgu-table_length"));
                        IWebElement searchTable = driver.FindElement(By.Id("search-sgu-table"));
                        if (searchTable != null)
                        {
                            searchRes.status = "Completed";
                            searchRes.objectErrors = null;
                            searchRes.result = new Result();
                            IWebElement tBody = searchTable.FindElement(By.TagName("tbody"));
                            List<IWebElement> rows = tBody.FindElements(By.TagName("tr")).ToList();
                            if (rows.Count > 0)
                            {
                                if (Exists(FindElementSafe(driver, By.ClassName("dataTables_empty"))))
                                {
                                    IWebElement dataEmptyRow = driver.FindElement(By.ClassName("dataTables_empty"));
                                    if (dataEmptyRow != null)
                                    {
                                        Helper.Log.WriteLog("Empty Row");
                                        searchRes.result.totalCount = 0;
                                        searchRes.result.results = new ResultDetails[0];
                                    }
                                }
                                else
                                {
                                    searchRes.result.results = new ResultDetails[rows.Count];
                                    rows = tBody.FindElements(By.TagName("tr")).ToList();
                                    for (int i = 0; i < rows.Count; i++)
                                    {
                                        List<IWebElement> colData = rows[i].FindElements(By.TagName("td")).ToList();

                                        ResultDetails det = new ResultDetails();
                                        det.accreditationCode = colData[0].Text;
                                        det.accreditedInstallerNumber = colData[7].Text;
                                        det.bulkUploadId = Convert.ToInt32(colData[8].Text);
                                        string[] dateLastAuditarray = colData[15].Text.Split('/');
                                        det.LastAuditedDate = Convert.ToDateTime(dateLastAuditarray[0] + "/" + dateLastAuditarray[1] + "/" + dateLastAuditarray[2]);
                                        dateLastAudited = det.LastAuditedDate;
                                        IWebElement aRegCode = driver.FindElement(By.LinkText(det.accreditationCode));
                                        string ahref = aRegCode.GetAttribute("href");
                                       // string regcode = ahref.Replace("https://www.rec-registry.gov.au/rec-registry/app/smallunits/view-my-small-generation-unit-details?&registrationCode=", "");
                                        string regcode = ahref.Replace(ProjectConfiguration.RECSearchByPVDCode2 + "&registrationCode=", "");
                                        det.registrationCode = regcode;
                                        searchRes.result.results[i] = det;
                                    }
                                }
                            }
                        }
                    }

                    if (jobType == 1)
                    {
                        Helper.Log.WriteLog("Search Result Count:" + searchRes.result.results.Count().ToString());
                        if (searchRes.result.results != null && searchRes.result.results.Count() > 0)
                        {
                            for (int i = 0; i < searchRes.result.results.Count(); i++)
                            {
                                GetFailureReasonForREC(ref lstFailureReasons, searchRes.result.results[i].registrationCode, jobType, driver);
                            }
                        }
                    }
                    else
                    {
                        //lstFailureReasons = GetFailureReasonListSWH(searchRes);
                    }
                }
            }
            //menu-renewable-energy-systems
            Helper.Log.WriteLog("Method: SearchByUploadID  Activity: search from bulk upload id:" + strBulkUploadID);
        }

        public static void GetFailureReasonForREC(ref List<string> lstFailureReasons, string RegistrationCode, int jobType, ChromeDriver driver)
        {
            Helper.Log.WriteLog("Finding Failure Reason For Registration Code:" + RegistrationCode);
            string url = ProjectConfiguration.RECSearchByPVDCode2 + "&registrationCode=" + RegistrationCode;
           // string url = "https://www.rec-registry.gov.au/rec-registry/app/smallunits/view-my-small-generation-unit-details?&registrationCode=" + RegistrationCode;
            driver.Navigate().GoToUrl(url);
            Thread.Sleep(2000);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(300000));
            Func<IWebDriver, bool> isResponseReceived =
                d =>
                {
                    IWebElement eError = d.FindElement(By.Id("view-failure-notice"));
                    return eError.Displayed;
                };
            wait.Until(isResponseReceived);
            {
                IWebElement failurebtn = driver.FindElement(By.Id("view-failure-notice"));
                string strResult = "";
                if (failurebtn != null)
                {
                    failurebtn.Click();
                    Thread.Sleep(2000);
                    IWebElement iframeErrors = driver.FindElement(By.XPath(".//div[@id='audit-failure-note-dialog']/div['detail modal-body']/div['row-fluid']/iframe['failure-note-container']"));
                    IWebDriver subDriver = driver.SwitchTo().Frame(iframeErrors);
                    iframeErrors = subDriver.FindElement(By.TagName("html"));
                    strResult = iframeErrors.Text;
                }

                Helper.Log.WriteLog("strResult: " + strResult);
                lstFailureReasons.Add(strResult);
            }
        }

        public static IWebElement FindElementSafe(IWebDriver driver, By by)
        {
            try
            {
                return driver.FindElement(by);
            }
            catch (NoSuchElementException)
            {
                return null;
            }
        }

        public static bool Exists(IWebElement element)
        {
            if (element == null)
            { return false; }
            return true;
        }

        /// <summary>
        /// Write To Log File
        /// </summary>
        /// <param name="content"></param>
        //public static void WriteToLogFile(string content)
        //{
        //    FileStream fs = null;
        //    try
        //    {
        //        content = Environment.NewLine + content;
        //        //set up a filestream
        //        fs = new FileStream(FormBot.Helper.ProjectSession.LogFilePath, FileMode.OpenOrCreate, FileAccess.Write);
        //        //set up a streamwriter for adding text
        //        using (StreamWriter sw = new StreamWriter(fs))
        //        {
        //            sw.BaseStream.Seek(0, SeekOrigin.End);
        //            //add the text
        //            sw.WriteLine(content);
        //            //add the text to the underlying filestream
        //            sw.Flush();
        //            //close the writer
        //            sw.Close();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        if (fs != null)
        //        {
        //            fs.Close();
        //            fs.Dispose();
        //        }
        //        //throw;
        //    }
        //    //StreamWriter sw = new StreamWriter(fs);
        //    //find the end of the underlying filestream            
        //}

        /// <summary>
        /// Convert CSV File records into Datatable
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="isFirstRowHeader">if set to <c>true</c> [is first row header].</param>
        /// <returns>DataTable</returns>
        static DataTable GetDataTableFromCsv(string path, bool isFirstRowHeader)
        {
            //path = @"C:\Users\pci36\Downloads\Jobs636082159896331926.csv";
            //path = @"D:\Projects\FormBot\SourceCode\FormBot\FormBot.Main\UserDocuments\Jobs636080761571188707.csv";
            string header = isFirstRowHeader ? "Yes" : "No";
            string pathOnly = Path.GetDirectoryName(path);
            string fileName = Path.GetFileName(path);
            string sql = @"SELECT * FROM [" + fileName + "]";
            using (OleDbConnection connection = new OleDbConnection(
                      @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathOnly +
                      ";Extended Properties=\"Text;HDR=" + header + "\""))
            using (OleDbCommand command = new OleDbCommand(sql, connection))
            using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
            {
                DataTable dataTable = new DataTable();
                dataTable.Locale = CultureInfo.CurrentCulture;
                adapter.Fill(dataTable);
                return dataTable;
            }
        }

        public static bool UpdatePVDCode(SearchFileResult model, DataSet dsCSV_JobID, bool IsPVDJob, int UserId)
        {
            try
            {

                if (model.result.results != null && model.result.results.Count() > 0)
                {
                    //file = @"C:\Users\pci36\Desktop\FormBotScripts\Server CSV\636143901514692778.csv";
                    //dtCSV_JobID = GetDataTableFromCsv(file, true);
                    //_log.Log(SystemEnums.Severity.Info, "Method: UpdatePVDCode  Activity:Datatable for match data in db and rec data for without spv job:" + dsCSV_JobID.Tables[0]);
                    //_log.Log(SystemEnums.Severity.Info, "Method: UpdatePVDCode  Activity:Datatable for match data in db and rec data for spv job:" + dsCSV_JobID.Tables[2]);
                    if (dsCSV_JobID != null && dsCSV_JobID.Tables.Count > 0 && dsCSV_JobID.Tables[0] != null && dsCSV_JobID.Tables[0].Rows.Count > 0)
                    {
                        DataTable tbSearchResult = new DataTable();
                        tbSearchResult.Columns.Add("PVDCode", typeof(string));
                        tbSearchResult.Columns.Add("JobID", typeof(string));

                        for (int i = 0; i < model.result.results.Count(); i++)
                        {

                            string csvfileFormattedSerialNumbers = model.result.results[i].commaSeparatedSerialNumbers.Contains(",") ?
                                model.result.results[i].commaSeparatedSerialNumbers.Split(',')[0] :
                                model.result.results[i].commaSeparatedSerialNumbers;

                            string ownerFirstName = string.Empty;
                            string ownerLastName = string.Empty;
                            string ownerName = model.result.results[i].ownerName;

                            if (model.result.results[i].ownerName.Contains(" "))
                            {
                                ownerFirstName = model.result.results[i].ownerName.Split(' ')[0];
                                ownerLastName = model.result.results[i].ownerName.Split(' ')[1];
                            }

                            string installationdate = model.result.results[i].installationDate.ToString("dd/MM/yyyy");
                            string ownerType = model.result.results[i].ownerType != null ? (string.IsNullOrEmpty(model.result.results[i].ownerType.name) == true ? string.Empty : model.result.results[i].ownerType.name.ToLower()) : string.Empty;
                            string accreditedInstallerNumber = model.result.results[i].accreditedInstallerNumber;

                            var foundRow = (from DataRow dr in dsCSV_JobID.Tables[0].Rows
                                            where (IsPVDJob == true ? Convert.ToString(dr["Equipment model serial number(s)"]).Contains(csvfileFormattedSerialNumbers) : Convert.ToString(dr["Tank serial number(s)"]).Contains(csvfileFormattedSerialNumbers) &&
                                                Convert.ToDateTime(dr["Installation date"]).ToShortDateString() == installationdate &&
                                                IsPVDJob == true ? Convert.ToString(dr["CEC accredited installer number"]) == accreditedInstallerNumber : 1 == 1 &&
                                               (IsPVDJob == true || Convert.ToString(dr["Owner type"]).ToLower() == "individual") ? ((string.IsNullOrEmpty(ownerFirstName) == false ? Convert.ToString(dr["Owner first name"]).Contains(ownerFirstName) : 1 == 1)) : (Convert.ToString(dr["Owner organisation name"]).Contains(ownerName)) &&
                                               (IsPVDJob == true || Convert.ToString(dr["Owner type"]).ToLower() == "individual") ? ((string.IsNullOrEmpty(ownerLastName) == false ? Convert.ToString(dr["Owner surname"]).Contains(ownerLastName) : 1 == 1)) : (Convert.ToString(dr["Owner organisation name"]).Contains(ownerName)) &&
                                               (string.IsNullOrEmpty(ownerType) == false ? Convert.ToString(dr["Owner type"]).ToLower() == ownerType : 1 == 1))
                                            select dr).ToList();
                            _log.Log(SystemEnums.Severity.Info, "Method: UpdatePVDCode  Activity:Find in database as uploaded data in rec without spv row count:" + foundRow.Count());
                            if (foundRow != null && foundRow.Count() > 0)
                            {
                                tbSearchResult.Rows.Add(model.result.results[i].accreditationCode,
                                                        Convert.ToString(foundRow[0]["JobId"]));
                                _log.Log(SystemEnums.Severity.Info, "Method: UpdatePVDCode  Activity:Find in database as uploaded data in rec without spv successfully for:" + Convert.ToString(foundRow[0]["JobId"]));
                            }
                            else
                            {
                                var spvRow = (from DataRow dr in dsCSV_JobID.Tables[2].Rows
                                              where (IsPVDJob == true ? Convert.ToString(dr["Equipment model serial number(s)"]).Contains(csvfileFormattedSerialNumbers) : Convert.ToString(dr["Tank serial number(s)"]).Contains(csvfileFormattedSerialNumbers) &&
                                                  Convert.ToDateTime(dr["Installation date"]).ToShortDateString() == installationdate &&
                                                  IsPVDJob == true ? Convert.ToString(dr["CEC accredited installer number"]) == accreditedInstallerNumber : 1 == 1 &&
                                                 (IsPVDJob == true || Convert.ToString(dr["Owner type"]).ToLower() == "individual") ? ((string.IsNullOrEmpty(ownerFirstName) == false ? Convert.ToString(dr["Owner first name"]).Contains(ownerFirstName) : 1 == 1)) : (Convert.ToString(dr["Owner organisation name"]).Contains(ownerName)) &&
                                                 (IsPVDJob == true || Convert.ToString(dr["Owner type"]).ToLower() == "individual") ? ((string.IsNullOrEmpty(ownerLastName) == false ? Convert.ToString(dr["Owner surname"]).Contains(ownerLastName) : 1 == 1)) : (Convert.ToString(dr["Owner organisation name"]).Contains(ownerName)) &&
                                                 (string.IsNullOrEmpty(ownerType) == false ? Convert.ToString(dr["Owner type"]).ToLower() == ownerType : 1 == 1))
                                              select dr).ToList();
                                _log.Log(SystemEnums.Severity.Info, "Method: UpdatePVDCode  Activity:Find in database as uploaded data in rec for spv job row count:" + spvRow.Count());

                                if (spvRow != null && spvRow.Count() > 0)
                                {
                                    tbSearchResult.Rows.Add(model.result.results[i].accreditationCode,
                                                      Convert.ToString(spvRow[0]["JobId"]));
                                    _log.Log(SystemEnums.Severity.Info, "Method: UpdatePVDCode  Activity:Find in database as uploaded data in rec for spv job successfully for:" + Convert.ToString(spvRow[0]["JobId"]));
                                }

                            }
                        }

                        if (tbSearchResult != null && tbSearchResult.Rows.Count > 0)
                        {
                            _log.Log(SystemEnums.Severity.Info, "Method: UpdatePVDCode  Activity:match data from db and rec getting successfully");
                            CreateJobBAL createJob = new CreateJobBAL();
                            STCInvoiceBAL obj = new STCInvoiceBAL();
                            DataSet ds = obj.UpdatePVDCodeByJobID(tbSearchResult, UserId);
                            #region save stcJobHistory into xml
                            if (ds != null && ds.Tables.Count > 0 && ds.Tables[1].Rows.Count > 0)
                            {
                                foreach (DataRow drUpdatePVDCodeByJobID in ds.Tables[1].Rows)
                                {
                                    int JobID = Convert.ToInt32(drUpdatePVDCodeByJobID["JobID"]);
                                    int STCStatusID = Convert.ToInt32(drUpdatePVDCodeByJobID["STCStatusID"]);
                                    string Description = drUpdatePVDCodeByJobID["Description"].ToString();
                                    string CreatedByID = drUpdatePVDCodeByJobID["CreatedBy"].ToString();
                                    string CreatedBy = "";
                                    if (CreatedByID.ToString() == "-1")
                                    {
                                        CreatedBy = "System";
                                    }
                                    else
                                    {
                                        CreatedBy = createJob.GetUsernameByUserID(Convert.ToInt32(CreatedByID));
                                    }
                                    string JobHistoryMessage = "changed STC Status to " + createJob.GetSTCStausNameBySTCStatusID(STCStatusID) + " <b class=\"blue-title\"> (" + JobID + ") JobRefNo </b> - ";
                                    Common.SaveSTCJobHistorytoXML(JobID, JobHistoryMessage, Description, STCStatusID, "Statuses", "STCSubmission", CreatedBy, false);
                                }
                            }
                            #endregion
                            //DataTable dt = createJob.GetSTCDetailsAndJobDataForCache(afterRecStcJobIds, null);
                            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                            {
                                DataTable dt = ds.Tables[0];
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    SortedList<string, string> data = new SortedList<string, string>();
                                    string pvdswhcode = dt.Rows[i]["PVDCode"].ToString();
                                    string stcstatus = dt.Rows[i]["STCStatus"].ToString();
                                    string strStcStatus = (stcstatus != null || stcstatus != "") ? Common.GetDescription((SystemEnums.STCJobStatus)Convert.ToInt32(stcstatus), "") : "";
                                    string colorCode = (stcstatus != null || stcstatus != "") ? Common.GetSubDescription((SystemEnums.STCJobStatus)Convert.ToInt32(stcstatus), "") : "";

                                    data.Add("ColorCode", colorCode);
                                    data.Add("PVDSWHCode", pvdswhcode);
                                    data.Add("STCStatus", strStcStatus);
                                    data.Add("STCStatusId", stcstatus);
                                    BAL.Service.CommonRules.CommonBAL.SetCacheDataForSTCSubmission(Convert.ToInt32(dt.Rows[i]["STCjobdetailsid"].ToString()), null, data).Wait();
                                }
                                _log.Log(SystemEnums.Severity.Info, "Method: UpdatePVDCode  Activity:UpdatePVDCodeByJobID cache updated successfully. ");
                            }
                            _log.Log(SystemEnums.Severity.Info, "Method: UpdatePVDCode  Activity:UpdatePVDCodeByJobID successfully ");
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                _log.Log(SystemEnums.Severity.Error, "Error in Updating PVD Code: " + ex.Message.ToString());
                return false;
            }
        }

        public static bool UpdatePVDCodeFromFile(SearchFileResult model, DataSet dsCSV_JobID, bool IsPVDJob, int UserId)
        {


            if (model.result.results != null && model.result.results.Count() > 0)
            {
                if (dsCSV_JobID != null && dsCSV_JobID.Tables.Count > 0 && dsCSV_JobID.Tables[0] != null && dsCSV_JobID.Tables[0].Rows.Count > 0)
                {
                    DataTable tbSearchResult = new DataTable();
                    tbSearchResult.Columns.Add("PVDCode", typeof(string));
                    tbSearchResult.Columns.Add("JobID", typeof(string));

                    for (int i = 0; i < model.result.results.Count(); i++)
                    {

                        string csvfileFormattedSerialNumbers = model.result.results[i].commaSeparatedSerialNumbers.Contains(",") ?
                            model.result.results[i].commaSeparatedSerialNumbers.Split(',')[0] :
                            model.result.results[i].commaSeparatedSerialNumbers;

                        string ownerFirstName = string.Empty;
                        string ownerLastName = string.Empty;
                        string ownerName = model.result.results[i].ownerName;

                        if (model.result.results[i].ownerName.Contains(" "))
                        {
                            ownerFirstName = model.result.results[i].ownerName.Split(' ')[0];
                            ownerLastName = model.result.results[i].ownerName.Split(' ')[1];
                        }

                        string installationdate = model.result.results[i].installationDate.ToString("dd/MM/yyyy");
                        string ownerType = model.result.results[i].ownerType != null ? (string.IsNullOrEmpty(model.result.results[i].ownerType.name) == true ? string.Empty : model.result.results[i].ownerType.name.ToLower()) : string.Empty;
                        string accreditedInstallerNumber = model.result.results[i].accreditedInstallerNumber;

                        var foundRow = (from DataRow dr in dsCSV_JobID.Tables[0].Rows
                                        where (IsPVDJob == true ? Convert.ToString(dr["Equipment model serial number(s)"]).Contains(csvfileFormattedSerialNumbers) : Convert.ToString(dr["Tank serial number(s)"]).Contains(csvfileFormattedSerialNumbers) &&
                                            Convert.ToDateTime(dr["Installation date"]).ToShortDateString() == installationdate &&
                                            IsPVDJob == true ? Convert.ToString(dr["CEC accredited installer number"]) == accreditedInstallerNumber : 1 == 1 &&
                                           (IsPVDJob == true || Convert.ToString(dr["Owner type"]).ToLower() == "individual") ? ((string.IsNullOrEmpty(ownerFirstName) == false ? Convert.ToString(dr["Owner first name"]).Contains(ownerFirstName) : 1 == 1)) : (Convert.ToString(dr["Owner organisation name"]).Contains(ownerName)) &&
                                           (IsPVDJob == true || Convert.ToString(dr["Owner type"]).ToLower() == "individual") ? ((string.IsNullOrEmpty(ownerLastName) == false ? Convert.ToString(dr["Owner surname"]).Contains(ownerLastName) : 1 == 1)) : (Convert.ToString(dr["Owner organisation name"]).Contains(ownerName)) &&
                                           (string.IsNullOrEmpty(ownerType) == false ? Convert.ToString(dr["Owner type"]).ToLower() == ownerType : 1 == 1))
                                        select dr).ToList();
                        _log.Log(SystemEnums.Severity.Info, "Method: UpdatePVDCode  Activity:Find in database as uploaded data in rec without spv row count:" + foundRow.Count());
                        if (foundRow != null && foundRow.Count() > 0)
                        {
                            tbSearchResult.Rows.Add(model.result.results[i].accreditationCode,
                                                    Convert.ToString(foundRow[0]["JobId"]));
                            _log.Log(SystemEnums.Severity.Info, "Method: UpdatePVDCode  Activity:Find in database as uploaded data in rec without spv successfully for:" + Convert.ToString(foundRow[0]["JobId"]));
                        }
                        else
                        {
                            var spvRow = (from DataRow dr in dsCSV_JobID.Tables[2].Rows
                                          where (IsPVDJob == true ? Convert.ToString(dr["Equipment model serial number(s)"]).Contains(csvfileFormattedSerialNumbers) : Convert.ToString(dr["Tank serial number(s)"]).Contains(csvfileFormattedSerialNumbers) &&
                                              Convert.ToDateTime(dr["Installation date"]).ToShortDateString() == installationdate &&
                                              IsPVDJob == true ? Convert.ToString(dr["CEC accredited installer number"]) == accreditedInstallerNumber : 1 == 1 &&
                                             (IsPVDJob == true || Convert.ToString(dr["Owner type"]).ToLower() == "individual") ? ((string.IsNullOrEmpty(ownerFirstName) == false ? Convert.ToString(dr["Owner first name"]).Contains(ownerFirstName) : 1 == 1)) : (Convert.ToString(dr["Owner organisation name"]).Contains(ownerName)) &&
                                             (IsPVDJob == true || Convert.ToString(dr["Owner type"]).ToLower() == "individual") ? ((string.IsNullOrEmpty(ownerLastName) == false ? Convert.ToString(dr["Owner surname"]).Contains(ownerLastName) : 1 == 1)) : (Convert.ToString(dr["Owner organisation name"]).Contains(ownerName)) &&
                                             (string.IsNullOrEmpty(ownerType) == false ? Convert.ToString(dr["Owner type"]).ToLower() == ownerType : 1 == 1))
                                          select dr).ToList();
                            _log.Log(SystemEnums.Severity.Info, "Method: UpdatePVDCode  Activity:Find in database as uploaded data in rec for spv job row count:" + spvRow.Count());

                            if (spvRow != null && spvRow.Count() > 0)
                            {
                                tbSearchResult.Rows.Add(model.result.results[i].accreditationCode,
                                                  Convert.ToString(spvRow[0]["JobId"]));
                                _log.Log(SystemEnums.Severity.Info, "Method: UpdatePVDCode  Activity:Find in database as uploaded data in rec for spv job successfully for:" + Convert.ToString(spvRow[0]["JobId"]));
                            }

                        }
                    }

                    if (tbSearchResult != null && tbSearchResult.Rows.Count > 0)
                    {
                        _log.Log(SystemEnums.Severity.Info, "Method: UpdatePVDCode  Activity:match data from db and rec getting successfully");
                        CreateJobBAL createJob = new CreateJobBAL();
                        STCInvoiceBAL obj = new STCInvoiceBAL();
                        DataSet ds = obj.UpdatePVDCodeByJobID(tbSearchResult, UserId);
                        #region save stcJobHistory into xml
                        if (ds != null && ds.Tables.Count > 0 && ds.Tables[1].Rows.Count > 0)
                        {
                            foreach (DataRow drUpdatePVDCodeByJobID in ds.Tables[1].Rows)
                            {
                                int JobID = Convert.ToInt32(drUpdatePVDCodeByJobID["JobID"]);
                                int STCStatusID = Convert.ToInt32(drUpdatePVDCodeByJobID["STCStatusID"]);
                                string Description = drUpdatePVDCodeByJobID["Description"].ToString();
                                string CreatedByID = drUpdatePVDCodeByJobID["CreatedBy"].ToString();
                                string CreatedBy = "";
                                if (CreatedByID.ToString() == "-1")
                                {
                                    CreatedBy = "System";
                                }
                                else
                                {
                                    CreatedBy = createJob.GetUsernameByUserID(Convert.ToInt32(CreatedByID));
                                }
                                string JobHistoryMessage = "changed STC Status to " + createJob.GetSTCStausNameBySTCStatusID(STCStatusID) + " <b class=\"blue-title\"> (" + JobID + ") JobRefNo </b> - ";
                                Common.SaveSTCJobHistorytoXML(JobID, JobHistoryMessage, Description, STCStatusID, "Statuses", "STCSubmission", CreatedBy, false);
                            }
                        }
                        #endregion
                        //DataTable dt = createJob.GetSTCDetailsAndJobDataForCache(afterRecStcJobIds, null);
                        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            DataTable dt = ds.Tables[0];
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                SortedList<string, string> data = new SortedList<string, string>();
                                string pvdswhcode = dt.Rows[i]["PVDCode"].ToString();
                                string stcstatus = dt.Rows[i]["STCStatus"].ToString();
                                string strStcStatus = (stcstatus != null || stcstatus != "") ? Common.GetDescription((SystemEnums.STCJobStatus)Convert.ToInt32(stcstatus), "") : "";
                                string colorCode = (stcstatus != null || stcstatus != "") ? Common.GetSubDescription((SystemEnums.STCJobStatus)Convert.ToInt32(stcstatus), "") : "";

                                data.Add("ColorCode", colorCode);
                                data.Add("PVDSWHCode", pvdswhcode);
                                data.Add("STCStatus", strStcStatus);
                                data.Add("STCStatusId", stcstatus);
                                BAL.Service.CommonRules.CommonBAL.SetCacheDataForSTCSubmission(Convert.ToInt32(dt.Rows[i]["STCjobdetailsid"].ToString()), null, data).Wait();
                            }
                            _log.Log(SystemEnums.Severity.Info, "Method: UpdatePVDCode  Activity:UpdatePVDCodeByJobID cache updated successfully. ");
                        }
                        _log.Log(SystemEnums.Severity.Info, "Method: UpdatePVDCode  Activity:UpdatePVDCodeByJobID successfully ");
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static void AuthenticateUser_UploadFileForREC(ref List<string> lstFailureReasons,ref DateTime?  lastAuditedDate, FormBot.Entity.RECAccount objAdminUser, string STCPVDCode, int jobType, FormBot.Entity.RECAccount objResellerUser)
        {
            Relogin:
            // CookieContainer cookies = new CookieContainer();
            ChromeDriver driver = null;
            System.Net.ServicePointManager.Expect100Continue = false;

            //ChromeOptions options = new ChromeOptions();
            //options.AddArgument("no-sandbox");
            //options.AddAdditionalCapability("useAutomationExtension", false);
            //options.AddExcludedArgument("enable-automation");
           
            //driver.Manage().Window.Minimize();
            //driver.Manage().Timeouts().PageLoad.Add(System.TimeSpan.FromSeconds(30));
            try
            {
                if (!string.IsNullOrWhiteSpace(objAdminUser.CERLoginId) && !string.IsNullOrWhiteSpace(objAdminUser.CERPassword) && !string.IsNullOrWhiteSpace(objAdminUser.RECAccName))
                {
                    driver = LoginAndValidateOTP().Result;
                    Thread.Sleep(50000);


                    //driver.Navigate().GoToUrl(ProjectConfiguration.RECAuthURL);
                    //Thread.Sleep(5000);
                    //IWebElement ele = driver.FindElement(By.Id("signInName"));
                    //IWebElement ele2 = driver.FindElement(By.Id("password"));
                    //ele.SendKeys(objAdminUser.CERLoginId);
                    //ele2.SendKeys(objAdminUser.CERPassword);
                    //IWebElement ele1 = driver.FindElement(By.Id("next"));
                    //ele1.Click();
                    //Thread.Sleep(5000);

                    //if (Exists(FindElementSafe(driver, By.ClassName("error"))))
                    //{
                    //    IWebElement eleError = driver.FindElement(By.ClassName("error"));
                    //    if (eleError != null)
                    //    {
                    //        if (eleError.Displayed)
                    //        {
                    //            Helper.Log.WriteLog("Invalid UserName or Password For Job: " + STCPVDCode);
                    //        }
                    //    }
                    //}
                    //else
                    //{
                        Helper.Log.WriteLog("Login Successful");
                        ReadOnlyCollection<IWebElement> lstEle = driver.FindElements(By.ClassName("btn-primary"));
                    Common.Log("Login Successful lstEleCnt: " + lstEle.Count);
                    if (lstEle != null & lstEle.Count > 5)
                    {
                        if (lstEle != null && lstEle.Count > 0)
                        {

                            IWebElement eleAccount = lstEle.Where(a => a.GetAttribute("onclick") == "submitUser('" + objAdminUser.RECAccName + "')").FirstOrDefault();

                            if (eleAccount != null)
                            {
                                eleAccount.Click();
                                Helper.Log.WriteLog("Search Started For PVDCode: " + STCPVDCode);
                                SearchByPVDCode(ref lstFailureReasons,ref lastAuditedDate, STCPVDCode, jobType, "", driver);
                                {
                                    driver.Navigate().GoToUrl(ConfigurationManager.AppSettings["RECHomeAllElementURL"]);
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        driver?.FindElement(By.XPath("//*[@id=\"j_logout\"]")).Click();
                        Thread.Sleep(5000);
                        driver?.Close();
                        driver?.Quit();
                        Common.Log("AuthenticateUser_UploadFileForREC For Batch: Relogin Started now");

                        goto Relogin;
                    }
                    //}
                }

                if (!string.IsNullOrWhiteSpace(objResellerUser.CERLoginId) && !string.IsNullOrWhiteSpace(objResellerUser.CERPassword) && !string.IsNullOrWhiteSpace(objResellerUser.RECAccName))
                {

                    driver = LoginAndValidateOTP().Result;
                    Thread.Sleep(50000);
                    //driver.Navigate().GoToUrl(ProjectConfiguration.RECAuthURL);
                    //Thread.Sleep(5000);
                    //IWebElement ele = driver.FindElement(By.Id("signInName"));
                    //IWebElement ele2 = driver.FindElement(By.Id("password"));
                    //ele.SendKeys(objResellerUser.CERLoginId);
                    //ele2.SendKeys(objResellerUser.CERPassword);
                    //IWebElement ele1 = driver.FindElement(By.Id("next"));
                    //ele1.Click();
                    //Thread.Sleep(5000);

                    //if (Exists(FindElementSafe(driver, By.ClassName("error"))))
                    //{
                    //    IWebElement eleError = driver.FindElement(By.ClassName("error"));
                    //    if (eleError != null)
                    //    {
                    //        if (eleError.Displayed)
                    //        {
                    //            Helper.Log.WriteLog("Invalid UserName or Password For Job: " + STCPVDCode);
                    //        }
                    //    }
                    //}
                    //else
                    //{
                        Helper.Log.WriteLog("Login Successful");
                        ReadOnlyCollection<IWebElement> lstEle = driver.FindElements(By.ClassName("btn-primary"));

                    if (lstEle != null & lstEle.Count > 5)
                    {
                        if (lstEle != null && lstEle.Count > 0)
                        {
                            IWebElement eleAccount = lstEle.Where(a => a.GetAttribute("onclick") == "submitUser('" + objResellerUser.RECAccName + "')").FirstOrDefault();

                            if (eleAccount != null)
                            {
                                eleAccount.Click();
                                Helper.Log.WriteLog("Search Started For PVDCode: " + STCPVDCode);
                                SearchByPVDCode(ref lstFailureReasons, ref lastAuditedDate, STCPVDCode, jobType, "", driver);
                                if (lstFailureReasons != null && lstFailureReasons.Count > 0)
                                {
                                    driver.Navigate().GoToUrl(ConfigurationManager.AppSettings["RECHomeAllElementURL"]);
                                    return;
                                }
                                else
                                {
                                    Helper.Log.WriteLog("No REC Login Credentials found for the PVDCode: " + STCPVDCode);
                                    throw new Exception("No REC Login Credentials found for the PVDCode: " + STCPVDCode);
                                }
                            }
                        }
                    }
                    else
                    {
                        driver?.FindElement(By.XPath("//*[@id=\"j_logout\"]")).Click();
                        Thread.Sleep(5000);
                        driver?.Close();
                        driver?.Quit();
                        Common.Log("AuthenticateUser_UploadFileForREC For Batch: Relogin Started now");

                        goto Relogin;
                    }

                          
                    //}
                }
                else
                {
                    Helper.Log.WriteLog("No REC Login Credentials found for the PVDCode: " + STCPVDCode);
                }
            }
            catch (Exception ex)
            {
                Helper.Log.WriteLog("Error in Authenticate User: " + ex.ToString());
            }
            finally
            {
                driver.Quit();
            }
        }

        #region New login
        public static async Task<ChromeDriver> LoginAndValidateOTP(string tempRecBulkUploadId = "")
        {
            ChromeDriver driver = null;
            Common.Log("LoginAndValidateOTP start For Batch: " + tempRecBulkUploadId);
            #region Login
            CookieContainer cookies = new CookieContainer();
            System.Net.ServicePointManager.Expect100Continue = false;
            STCInvoiceBAL obj = new STCInvoiceBAL();

            ChromeOptions options = new ChromeOptions();
            options.AddArgument("no-sandbox");
            options.AddAdditionalCapability("useAutomationExtension", false);
            options.AddExcludedArgument("enable-automation");
            //options.AddArguments("headless");


            try
            {
               
                driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), options, TimeSpan.FromMinutes(3));
                driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().PageLoad.Add(System.TimeSpan.FromSeconds(30));
            Common.Log("LoginANDValidateOTP For Batch: " + tempRecBulkUploadId);
            
                driver.Navigate().GoToUrl(ConfigurationManager.AppSettings["RECAuthURL"]);
                Thread.Sleep(TimeSpan.FromSeconds(5));
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(300000));
                Func<IWebDriver, bool> isResponseReceivedLoad =
                    d =>
                    {
                        IWebElement eError = d.FindElement(By.Id("signInName"));
                        return eError.Displayed;
                    };
                wait.Until(isResponseReceivedLoad);

                {
                    Common.Log("Login Page Loaded For Batch 2: " + tempRecBulkUploadId);
                    if (Exists(FindElementSafe(driver, By.ClassName("btn-primary"))))
                    {
                        ReadOnlyCollection<IWebElement> lstEle = driver.FindElements(By.ClassName("btn-primary"));
                        if (lstEle != null && lstEle.Count > 0)
                            return driver;
                    }
                    else
                    {
                        Common.Log("enter in else of find elements of list");
                        IWebElement ele = driver.FindElement(By.Id("signInName"));
                        Common.Log("enter in else of find elements of list 2");
                        IWebElement ele2 = driver.FindElement(By.Id("password"));
                        Common.Log("enter in else of find elements of list 3");
                        ele.SendKeys(ConfigurationManager.AppSettings["RECAuthUserName"]);
                        //ele2.SendKeys("Tstpw2124!");
                        ele2.SendKeys(ConfigurationManager.AppSettings["RECAuthPwd"]);
                        Common.Log("enter in else of find elements of list 4 ");
                        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5));
                        Common.Log("enter in else of find elements of list 5 ");
                        IWebElement ele1 = driver.FindElement(By.Id("next"));
                        ele1.Click();
                        Common.Log("enter in else of find elements of list 6");

                        //WebDriverWait wait2 = new WebDriverWait(driver, TimeSpan.FromMilliseconds(300000));
                        //Func<IWebDriver, bool> isResponseReceivedLoad2 =
                        //    d =>
                        //    {
                        //        IWebElement eError = d.FindElement(By.Id("extension_mfaByPhoneOrEmail_email"));
                        //        return eError.Displayed;
                        //    };
                        //wait2.Until(isResponseReceivedLoad2);ImplicitWait
                        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(120);

                        if (Exists(FindElementSafe(driver, By.XPath("//*[@id=\"api\"]/div/div[2]/p"))) && !string.IsNullOrEmpty(driver.FindElement(By.XPath("//*[@id=\"api\"]/div/div[2]/p")).Text))
                        {
                            IWebElement eleError = driver.FindElement(By.XPath("//*[@id=\"api\"]/div/div[2]/p"));
                            if (eleError != null)
                            {
                                obj.UpdateInternalIssue(tempRecBulkUploadId, eleError.Text);
                            }
                        }
                        else
                        {
                            if (Exists(FindElementSafe(driver, By.XPath("//*[@id=\"extension_mfaByPhoneOrEmail_email\"]"))))
                            // if (Exists(FindElementSafe(driver, By.Id("extension_mfaByPhoneOrEmail_email"))))
                            {
                                Common.Log("enter in check email");
                                //clickemail checkbox and submit 
                                driver.FindElement(By.Id("extension_mfaByPhoneOrEmail_email")).Click();
                                driver.FindElement(By.Id("continue")).Click();

                                //wait for page to load
                                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
                                //submit email for otp
                                driver.FindElement(By.Id("readOnlyEmail_ver_but_send")).Click();

                                int otpretrycount = 0;
                            RECHECKEMAIL:
                                //open mail to check emails otp
                                string otp = await LoginCpannelEmail(tempRecBulkUploadId);//callmail for otp
                                Common.Log("otp after login:" + otp);
                                if (!string.IsNullOrEmpty(otp))
                                {
                                    Common.Log("otp not null");
                                    //insert OTp and verify account
                                    driver.FindElement(By.Id("readOnlyEmail_ver_input")).Clear();//delete old otp if any

                                    driver.FindElement(By.Id("readOnlyEmail_ver_input")).SendKeys(otp);
                                    Common.Log("after otp 1");
                                    // click verify
                                    driver.FindElement(By.Id("readOnlyEmail_ver_but_verify")).Click();
                                    Common.Log("after otp 2");
                                    //Error shown for incorect otp not correct // not tested if otp is correctfkmjgdg
                                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5));
                                    IWebElement error = driver.FindElement(By.Id("readOnlyEmail_fail_retry"));
                                    Common.Log("after otp 3");
                                    if (error.Text.Contains("That code is incorrect. Please try again"))
                                    {
                                        Common.Log("after otp 4");
                                        if (otpretrycount < 10)
                                        {
                                            Common.Log("after otp 5--" + otpretrycount);
                                            otpretrycount++;
                                            //if incorrect otp resend otp 
                                            // driver.FindElement(By.Id("readOnlyEmail_ver_but_resend")).Click(); otp can be incorrect but it can be old so no need for resend otp
                                            // recheck if new otp received 
                                            goto RECHECKEMAIL;
                                        }
                                        else
                                        {
                                            Common.Log("after otp 6");
                                            obj.UpdateInternalIssue(tempRecBulkUploadId, "OTP retry ended,That OTP code is incorrect. Please try again");
                                            throw new ArgumentOutOfRangeException("OTP retry ended", "That code is incorrect. Please try again later you have maximum otp requested attepted");
                                        }

                                    }
                                }
                                else

                                {
                                    Common.Log("after otp 7");
                                    if (otpretrycount < 10)
                                    {
                                        Common.Log("after otp 8");
                                        otpretrycount++;
                                        Common.Log("after otp 9--" + otpretrycount);
                                        //if otp not received resend otp
                                        driver.FindElement(By.Id("readOnlyEmail_ver_but_resend")).Click();
                                        Common.Log("after otp 10");
                                        // recheck if new otp received 
                                        goto RECHECKEMAIL;
                                    }
                                    else
                                    {
                                        Common.Log("after otp 11");
                                        obj.UpdateInternalIssue(tempRecBulkUploadId, "OTP retry ended,That OTP code is incorrect. Please try again");
                                        throw new ArgumentOutOfRangeException("OTP retry ended", "That code is incorrect. Please try again later you have maximum otp requested attepted");
                                    }
                                }
                                Common.Log("after otp 12");
                                driver.FindElement(By.Id("continue")).Click();
                                Common.Log("after otp 13");
                                Thread.Sleep(15000);
                                //driver.FindElement(By.XPath("//*[@id=\"applicationContainer\"]/div/div[2]/div[4]/div[2]/ul/li[5]/a")).Click();
                                driver.Navigate().GoToUrl(ConfigurationManager.AppSettings["RECHomeAllElementURL"]);
                                Common.Log("after otp 14");
                                return driver;



                                #endregion
                            }
                            else
                            {
                                Common.Log("enter in else of mail check");
                                if (Exists(FindElementSafe(driver, By.ClassName("btn-primary"))))
                                {
                                    ReadOnlyCollection<IWebElement> lstEle = driver.FindElements(By.ClassName("btn-primary"));
                                    if (lstEle != null && lstEle.Count > 0)
                                        return driver;
                                }
                                else
                                {
                                    Common.Log("enter in else of else mail check");
                                    driver.Navigate().GoToUrl(ConfigurationManager.AppSettings["RECHomeAllElementURL"]);
                                    return driver;
                                }

                            }
                        }

                        // }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.Log("after otp 15 exception:" + ex.InnerException.Message.ToString());
                //driver.FindElement(By.XPath("//*[@id=\"applicationContainer\"]/div/div[2]/div[4]/div[2]/ul/li[5]/a")).Click();
                driver.Navigate().GoToUrl(ConfigurationManager.AppSettings["RECHomeAllElementURL"]);
                return driver;

            }
            return driver;
        }
        static IWebDriver driverMail = null;
        public static async Task<string> LoginCpannelEmail(string tempRECBulkUploadId = "")
        {
            STCInvoiceBAL obj = new STCInvoiceBAL();
            try
            {
                Common.Log("start open mail acc");
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(20));
                //LoginGmail();
                var DeviceDriver = ChromeDriverService.CreateDefaultService();
                DeviceDriver.HideCommandPromptWindow = true;
                ChromeOptions options = new ChromeOptions();
                //options.AddArguments("--disable-infobars");
                options.AddArgument("no-sandbox");
                options.AddAdditionalCapability("useAutomationExtension", false);
                options.AddExcludedArgument("enable-automation");
                driverMail = new ChromeDriver(DeviceDriver, options);
                driverMail.Manage().Window.Maximize();
                driverMail.Navigate().GoToUrl(ConfigurationManager.AppSettings["cpanelLoginUrl"]);
                driverMail.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
                Common.Log("start open mail acc 2");
                IWebElement EmailTextBox = driverMail.FindElement(By.Id("user"));
                Common.Log("start open mail acc 3");
                EmailTextBox.Clear();
                EmailTextBox.SendKeys(ConfigurationManager.AppSettings["cpanelUserName"]);
                Common.Log("start open mail acc 4");
                IWebElement PasswordTextBox = driverMail.FindElement(By.Id("pass"));
                PasswordTextBox.Clear();
                PasswordTextBox.SendKeys(ConfigurationManager.AppSettings["cpanelPwd"]);
                driverMail.FindElement(By.Id("login_submit")).Click();
                Common.Log("start open mail acc 5");
                int retryCount = 0;
            waitformail:
                Common.Log("start open mail acc 6");

                //WebDriverWait wait2 = new WebDriverWait(driverMail, TimeSpan.FromMilliseconds(300000));
                //Func<IWebDriver, bool> isResponseReceivedLoad2 =
                //    d =>
                //    {
                //        IWebElement eError = d.FindElement(By.Id("messagelist"));
                //        return eError.Displayed;
                //    };
                //wait2.Until(isResponseReceivedLoad2);
                driverMail.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
                //System.Threading.Thread.Sleep(TimeSpan.FromSeconds(10));
                Common.Log("start open mail acc 6.1");
                IWebElement tbl = driverMail.FindElement(By.XPath("//*[@id=\"messagelist\"]"));
                Common.Log("start open mail acc 7");
                IReadOnlyCollection<IWebElement> rows = tbl.FindElements(By.TagName("tr"));
                bool MailFound = false;
                foreach (var item in rows)
                {
                    Common.Log("start open mail acc for loop 8");
                    //if (item.Text.Contains("verification code") && item.Text.ToLower().Contains("today"))
                    if (item.Text.Contains("verification code") && item.Text.ToLower().Contains("today"))
                    {
                        MailFound = true;
                        //item.FindElement(By.TagName("a")).Click();
                        Actions actions = new Actions(driverMail);
                        actions.DoubleClick(item.FindElement(By.TagName("a"))).Perform();
                        break;
                    }
                }
                Common.Log("start open mail acc 9");
                if (!MailFound)
                {
                    Common.Log("start open mail acc 10");
                    driverMail.FindElement(By.Id("rcmbtn110")).Click();//refresh button
                    if (retryCount < 10)
                    {
                        Common.Log("start open mail acc 11");
                        retryCount++;
                        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(20));
                        goto waitformail;

                    }
                    else
                    {
                        Common.Log("start open mail acc 12");
                        return null;
                    }
                }
                Common.Log("start open mail acc 13");
                //driverMail.SwitchTo().Frame(driverMail.FindElement(By.Id("messagecontframe")));

                //messagebody
                IWebElement bmsgOTP = driverMail.FindElement(By.XPath("//*[@id=\"v1PageBody\"]/table/tbody/tr/td/table/tbody/tr/td/div[2]"));
                Common.Log("start open mail acc 14");
                IWebElement msgOTPText = bmsgOTP.FindElement(By.TagName("Span"));
                Common.Log("start open mail acc 15");
                string OTP = Regex.Match(msgOTPText.Text, @"\d+").Value;
                Common.Log("start open mail acc 16");
                driverMail.Close();
                driverMail.Quit();
                return OTP;

            }

            catch (Exception ex)
            {
                Common.Log("Exception in login Cpanel: " + tempRECBulkUploadId + "exception message: " + ex.InnerException.Message + " --" + ex.InnerException.ToString());
                obj.UpdateInternalIssue(tempRECBulkUploadId, "Exception in cPanel Login");
                return null;
            }



        }

        #endregion
    }


    public class clsUploadedFileJsonResponseObject
    {
        public string status { get; set; }
        public UploadedFileStatusResult result { get; set; }
        public List<string> errors { get; set; }
        public List<string> firstErrorOrDefault { get; set; }
        public List<string> fieldErrors { get; set; }
        public List<string> objectErrors { get; set; }
        public string strErrors { get; set; }
        public bool IsPVDCodeUpdated { get; set; }
    }

    public class UploadedFileStatusResult
    {
        public int uploadId { get; set; }
        public int numberOfStc { get; set; }
        public int bulkUploadId { get; set; }
        public int numberOfStcs { get; set; }
    }

    public class SearchFileResult
    {
        public string status { get; set; }
        public Result result { get; set; }
        public object errors { get; set; }
        public object firstErrorOrDefault { get; set; }
        public object fieldErrors { get; set; }
        public object objectErrors { get; set; }
    }

    public class SearchPVDCodeResult
    {
        public string status { get; set; }
        public PVDCodeResult result { get; set; }
        public object errors { get; set; }
        public object firstErrorOrDefault { get; set; }
        public object fieldErrors { get; set; }
        public object objectErrors { get; set; }
    }

    public class PVDCodeResult
    {
        public InstallationDetails installationDetails { get; set; }
        public string reference { get; set; }
    }

    public class InstallationDetails
    {
        public string retailerName { get; set; }
        public InverterDetail[] inverters { get; set; }
    }

    public class InverterDetail
    {
        public string modelNumber { get; set; }
    }


    public class Result
    {
        public int totalCount { get; set; }
        public ResultDetails[] results { get; set; }
    }




    public class ResultDetails
    {
        public string accreditationCode { get; set; }
        public int registrationId { get; set; }
        public int bulkUploadId { get; set; }
        public string registrationCode { get; set; }
        public string commaSeparatedSerialNumbers { get; set; }
        public DateTime installationDate { get; set; }
        public DateTime? LastAuditedDate { get; set; }
        public Ownertype ownerType { get; set; }
        public string ownerName { get; set; }
        public string accreditedInstallerNumber { get; set; }
        public Installationstate installationState { get; set; }
        //public Systemdetail[] systemDetails { get; set; }
        //public string systemBrand { get; set; }
        //public string systemModel { get; set; }
        //public DateTime certificatesCreatedDate { get; set; }
        //public int numberOfCertificatesCreated { get; set; }
        //public int numberOfRegisteredCertificates { get; set; }
        //public string fuelSource { get; set; }
        //public float ratedPowerOutput { get; set; }
        //public int deemingPeriod { get; set; }
        //public object resourceAvailability { get; set; }
        //public int numberOfPanels { get; set; }
        //public string inverterManufacturer { get; set; }
        //public string inverterSeries { get; set; }
        //public string reference { get; set; }
        //public object additionalSystemInformation { get; set; }
        //public int postCodeZone { get; set; }
        //public int recMultiplierRate { get; set; }
        //public Assessment assessment { get; set; }        
        //public string inverterModelNumber { get; set; }
    }

    public class Ownertype
    {
        public string displayName { get; set; }
        public string name { get; set; }
    }

    public class Installationstate
    {
        public string displayName { get; set; }
        public string name { get; set; }
    }

    public class Assessment
    {
        public object failureEmailContent { get; set; }
        public object latestAuditFailureNote { get; set; }
        public DateTime lastAuditedDate { get; set; }
        public int numberOfPassedStcs { get; set; }
        public int numberOfPendingStcs { get; set; }
        public int numberOfFailedStcs { get; set; }
    }

    public class Systemdetail
    {
        public string brand { get; set; }
        public string model { get; set; }
    }

}

using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using FormBot.Helper;
using FormBot.BAL.Service;
using FormBot.Helper.Helper;
using System.Globalization;
using OpenQA.Selenium.Support.UI;
using System.Threading.Tasks;
using OpenQA.Selenium.Interactions;

namespace PriorityWindowService
{
    public class RECRegistry
    {
        private static readonly Logger _logger = new Logger();
        private static readonly STCInvoiceBAL sTCInvoiceBAL = new STCInvoiceBAL();

        /// <summary>
        /// Authenticates the user_ upload file for record.
        /// </summary>
        /// <param name="lstFailureReasons">The LST failure reasons.</param>
        /// <param name="RecUsername">The record username.</param>
        /// <param name="RecPassword">The record password.</param>
        /// <param name="STCPVDCode">The STCPVD code.</param>
        public static void AuthenticateUser_UploadFileForREC(ref List<string> lstFailureReasons, ref DateTime? dateLastAudited, FormBot.Entity.RECAccount objAdminUser, string STCPVDCode, int jobType, FormBot.Entity.RECAccount objResellerUser)
        {
        Relogin:
            ChromeDriver driver = null;
            CookieContainer cookies = new CookieContainer();
            System.Net.ServicePointManager.Expect100Continue = false;
            try
            {
                if (!string.IsNullOrWhiteSpace(objAdminUser.CERLoginId) && !string.IsNullOrWhiteSpace(objAdminUser.CERPassword) && !string.IsNullOrWhiteSpace(objAdminUser.RECAccName))
                {
                    //  driver.Navigate().GoToUrl(ConfigurationManager.AppSettings["RECAuthURL"]);
                    driver = LoginAndValidateOTP().Result;
                    Thread.Sleep(50000);

                    //if (Exists(FindElementSafe(driver, By.ClassName("error"))))
                    //{
                    //    IWebElement eleError = driver.FindElement(By.ClassName("error"));
                    //    if (eleError != null)
                    //    {
                    //        if (eleError.Displayed)
                    //        {
                    //            Common.Log("Invalid UserName or Password For Job: " + STCPVDCode);
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(30000));
                    //Func<IWebDriver, bool> isResponseReceivedLoad =
                    //    d =>
                    //    {
                    //        IWebElement eError = d.FindElement(By.ClassName("btn-primary"));
                    //        return eError.Displayed;
                    //    };
                    //wait.Until(isResponseReceivedLoad);
                    Common.Log("Login Successful");
                    // ReadOnlyCollection<IWebElement> lstEle = driver.FindElements(By.TagName("input"));
                    ReadOnlyCollection<IWebElement> lstEle = driver.FindElements(By.ClassName("btn-primary"));

                    Common.Log("Login Successful lstEleCnt: " + lstEle.Count);
                    if (lstEle != null & lstEle.Count > 5)
                    {
                        if (lstEle != null && lstEle.Count > 0)
                        {
                            //IWebElement eleAccount = lstEle.Where(a => a.GetAttribute("onclick") == "submitUser('" + "LAMH54891" + "')").FirstOrDefault();
                            IWebElement eleAccount = lstEle.Where(a => a.GetAttribute("onclick") == "submitUser('" + objAdminUser.RECAccName + "')").FirstOrDefault();

                            if (eleAccount != null)
                            {
                                eleAccount.Click();
                                Common.Log("Search Started For PVDCode: " + STCPVDCode);
                                SearchByPVDCode(ref lstFailureReasons, ref dateLastAudited, cookies, "", STCPVDCode, jobType, "", driver);
                                if (lstFailureReasons != null && lstFailureReasons.Count > 0)
                                {
                                    driver.Navigate().GoToUrl(ConfigurationManager.AppSettings["RECHomeAllElementURL"]);
                                    return;
                                }

                            }
                        }
                    }
                    else
                    {
                        // this means we are on a wrong page we need to logout and re try
                        driver?.FindElement(By.XPath("//*[@id=\"j_logout\"]")).Click();
                        Thread.Sleep(5000);
                        driver?.Close();
                        driver?.Quit();
                        Common.Log("AuthenticateUser_UploadFileForREC For Batch: Relogin Started now");

                        goto Relogin;
                    }


                    //}
                    // }
                }

                if (!string.IsNullOrWhiteSpace(objResellerUser.CERLoginId) && !string.IsNullOrWhiteSpace(objResellerUser.CERPassword) && !string.IsNullOrWhiteSpace(objResellerUser.RECAccName))
                {
                    driver = LoginAndValidateOTP().Result;
                    Thread.Sleep(50000);
                    {
                        //if (Exists(FindElementSafe(driver, By.ClassName("error"))))
                        //{
                        //    IWebElement eleError = driver.FindElement(By.ClassName("error"));
                        //    if (eleError != null)
                        //    {
                        //        if (eleError.Displayed)
                        //        {
                        //            Common.Log("Invalid UserName or Password For Job: " + STCPVDCode);
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        //WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(30000));
                        //Func<IWebDriver, bool> isResponseReceivedLoad =
                        //    d =>
                        //    {
                        //        IWebElement eError = d.FindElement(By.ClassName("btn-primary"));
                        //        return eError.Displayed;
                        //    };
                        //wait.Until(isResponseReceivedLoad);
                        Common.Log("Login Successful");
                        ReadOnlyCollection<IWebElement> lstEle = driver.FindElements(By.ClassName("btn-primary"));
                        //ReadOnlyCollection<IWebElement> lstEle = driver.FindElements(By.TagName("input"));
                        if (lstEle != null & lstEle.Count > 5)
                        {
                            if (lstEle != null && lstEle.Count > 0)
                            {
                                IWebElement eleAccount = lstEle.Where(a => a.GetAttribute("onclick") == "submitUser('" + objResellerUser.RECAccName + "')").FirstOrDefault();

                                if (eleAccount != null)
                                {
                                    eleAccount.Click();
                                    Common.Log("Search Started For PVDCode: " + STCPVDCode);
                                    SearchByPVDCode(ref lstFailureReasons, ref dateLastAudited, cookies, "", STCPVDCode, jobType, "", driver);
                                    if (lstFailureReasons != null && lstFailureReasons.Count > 0)
                                    {
                                        driver.Navigate().GoToUrl(ConfigurationManager.AppSettings["RECHomeAllElementURL"]);
                                        return;
                                    }
                                    else
                                        Common.Log("No REC Login Credentials found for the PVDCode: " + STCPVDCode);
                                }
                            }
                        }
                        else
                        {
                            // this means we are on a wrong page we need to logout and re try
                            driver?.FindElement(By.XPath("//*[@id=\"j_logout\"]")).Click();
                            Thread.Sleep(5000);
                            driver?.Close();
                            driver?.Quit();
                            Common.Log("AuthenticateUser_UploadFileForREC For Batch: Relogin Started now");

                            goto Relogin;
                        }

                        //}
                    }
                }
                else
                {
                    Common.Log("No REC Login Credentials found for the PVDCode: " + STCPVDCode);
                }
            }
            catch (Exception ex)
            {
                Common.Log("Error in Fetching Failure Data: " + ex.Message);
            }
            finally
            {
                driver?.FindElement(By.XPath("//*[@id=\"j_logout\"]")).Click();
                Thread.Sleep(5000);
                driver?.Close();
                driver?.Quit();
            }
        }

        /// <summary>
        /// Search By Upload ID
        /// </summary>
        /// <param name="cookies">The cookies.</param>
        /// <param name="CSRFToken">The CSRF token.</param>
        /// <param name="IsPVDJob">IsPVDJob.</param>
        /// <param name="intBulkUploadID">The int bulk upload identifier.</param>
        /// <returns>search file result</returns>
        public static void SearchByPVDCode(ref List<string> lstFailureReasons, ref DateTime? dateLastAudited, CookieContainer cookies, string CSRFToken, string STCPVDCode, int jobType, string strBulkUploadID = "", ChromeDriver driver = null)
        {
            IWebElement eleAccount = driver.FindElement(By.Id("menu-renewable-energy-systems"));

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
                bulkuploadid.Clear();

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
                                        Common.Log("Empty Row");
                                        searchRes.result.totalCount = 0;
                                        searchRes.result.results = new ResultDetails[0];
                                    }
                                }
                                else
                                {
                                    rows = tBody.FindElements(By.TagName("tr")).ToList();
                                    searchRes.result.results = new ResultDetails[rows.Count];
                                    for (int i = 0; i < rows.Count; i++)
                                    {
                                        List<IWebElement> colData = rows[i].FindElements(By.TagName("td")).ToList();

                                        ResultDetails det = new ResultDetails();
                                        det.accreditationCode = colData[0].Text;
                                        det.accreditedInstallerNumber = colData[7].Text;
                                        if (!string.IsNullOrWhiteSpace(colData[8].Text))
                                        {
                                            det.bulkUploadId = Convert.ToInt32(colData[8].Text);
                                        }
                                        det.commaSeparatedSerialNumbers = colData[6].Text;
                                        string[] dateString = colData[1].Text.Split('/');
                                        //Common.Log("installationDate:" + det.installationDate);
                                        det.installationDate = Convert.ToDateTime(dateString[1] + "/" + dateString[0] + "/" + dateString[2]);
                                        // Common.Log("installationDate 11---:" + det.installationDate);
                                        det.installationState = new Installationstate { displayName = colData[3].Text, name = colData[3].Text };
                                        det.ownerName = colData[2].Text;
                                        string[] dateLastAuditarray = colData[15].Text.Split('/');
                                        //Common.Log("dateLastAuditarray start:" );
                                        det.LastAuditedDate = Convert.ToDateTime(dateLastAuditarray[1] + "/" + dateLastAuditarray[0] + "/" + dateLastAuditarray[2]);
                                        dateLastAudited = det.LastAuditedDate;
                                        //Common.Log("dateLastAuditarray start 11:"+dateLastAudited);
                                        IWebElement aRegCode = driver.FindElement(By.LinkText(det.accreditationCode));
                                        string ahref = aRegCode.GetAttribute("href");
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
                        Common.Log("Search Result Count:" + searchRes.result.results.Count().ToString());
                        if (searchRes.result.results != null && searchRes.result.results.Count() > 0)
                        {
                            for (int i = 0; i < searchRes.result.results.Count(); i++)
                            {
                                GetFailureReasonForREC(ref lstFailureReasons, cookies, CSRFToken, searchRes.result.results[i].registrationCode, jobType, driver);
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
            Common.Log("Method: SearchByUploadID  Activity: search from bulk upload id:" + strBulkUploadID);
        }

        /// <summary>
        /// Gets the failure reason for record.
        /// </summary>
        /// <param name="lstFailureReasons">The LST failure reasons.</param>
        /// <param name="cookies">The cookies.</param>
        /// <param name="CSRFToken">The CSRF token.</param>
        /// <param name="RegistrationCode">The registration code.</param>
        public static void GetFailureReasonForREC(ref List<string> lstFailureReasons, CookieContainer cookies, string CSRFToken, string RegistrationCode, int jobType, ChromeDriver driver)
        {
            Common.Log("Finding Failure Reason For Registration Code:" + RegistrationCode);
            string url = ProjectConfiguration.RECSearchByPVDCode2 + "&registrationCode=" + RegistrationCode;

            //string url = RegistrationCode;
            driver.Navigate().GoToUrl(url);
            Thread.Sleep(5000);
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
                Common.Log("strResult: " + strResult);
                lstFailureReasons.Add(strResult);

                //IWebElement closebtn = driver.FindElement(By.Id("failure-note-close"));
                //closebtn.Click();
            }
        }

        /// <summary>
        /// Writes Log for REC Registry response
        /// </summary>
        /// <param name="content">write log</param>
        private static void WriteToLogFile(string content)
        {
            ////set up a filestream
            //FileStream fs = new FileStream(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6) + "\\RECRegsitryLog.txt", FileMode.OpenOrCreate, FileAccess.Write);
            ////set up a streamwriter for adding text
            //using (StreamWriter sw = new StreamWriter(fs))
            //{
            //    sw.BaseStream.Seek(0, SeekOrigin.End);
            //    //add the text
            //    sw.WriteLine(content);
            //    //add the text to the underlying filestream
            //    sw.Flush();
            //    //close the writer
            //    sw.Close();
            //}
            //StreamWriter sw = new StreamWriter(fs);
            //find the end of the underlying filestream            
        }

        public static List<string> GetFailureReasonListPVD(string strResult)
        {
            List<string> lstFailureReasons = new List<string>();
            if (!string.IsNullOrEmpty(strResult))
            {
                Rootobject model = JsonConvert.DeserializeObject<Rootobject>(strResult);
                if (!string.IsNullOrEmpty(Convert.ToString(model.result.assessment.latestAuditFailureNote)))
                {
                    var startindex = model.result.assessment.latestAuditFailureNote.ToString().IndexOf("<ul>");
                    var lastindex = model.result.assessment.latestAuditFailureNote.ToString().IndexOf("</ul>");
                    string strFailureReasons = model.result.assessment.latestAuditFailureNote.ToString().Substring(startindex + 4, lastindex - startindex - 4);
                    if (!string.IsNullOrEmpty(strFailureReasons))
                    {
                        MatchCollection m_Li = Regex.Matches(strFailureReasons, @"<li>(.*?)<\/li>", RegexOptions.Multiline);
                        foreach (Match match_LI in m_Li)
                        {
                            string strFailureReason_LI = Convert.ToString(match_LI.Groups[1]);
                            if (!string.IsNullOrEmpty(strFailureReason_LI))
                            {
                                lstFailureReasons.Add(strFailureReason_LI);
                            }
                        }
                    }
                }
            }
            return lstFailureReasons;
        }
        public static List<string> GetFailureReasonListSWH(RootobjectSWH model)
        {
            List<string> lstFailureReasons = new List<string>();
            for (int i = 0; i < model.result.results.Length; i++)
            {
                if (!string.IsNullOrEmpty(Convert.ToString(model.result.results[i].assessment.latestAuditFailureNote)))
                {
                    var startindex = model.result.results[i].assessment.latestAuditFailureNote.ToString().IndexOf("<ul>");
                    var lastindex = model.result.results[i].assessment.latestAuditFailureNote.ToString().IndexOf("</ul>");
                    string strFailureReasons = model.result.results[i].assessment.latestAuditFailureNote.ToString().Substring(startindex + 4, lastindex - startindex - 4);
                    if (!string.IsNullOrEmpty(strFailureReasons))
                    {
                        MatchCollection m_Li = Regex.Matches(strFailureReasons, @"<li>(.*?)<\/li>", RegexOptions.Multiline);
                        foreach (Match match_LI in m_Li)
                        {
                            string strFailureReason_LI = Convert.ToString(match_LI.Groups[1]);
                            if (!string.IsNullOrEmpty(strFailureReason_LI))
                            {
                                lstFailureReasons.Add(strFailureReason_LI);
                            }
                        }
                    }
                }
            }
            return lstFailureReasons;
        }
        #region New login
        public static async Task<ChromeDriver> LoginAndValidateOTP(string tempRecBulkUploadId = "")
        {
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


            ChromeDriver driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), options, TimeSpan.FromMinutes(3));
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().PageLoad.Add(System.TimeSpan.FromSeconds(30));
            Common.Log("LoginANDValidateOTP For Batch: " + tempRecBulkUploadId);
            try
            {
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
                        Actions actions = new Actions(driverMail);
                        actions.DoubleClick(item.FindElement(By.TagName("a"))).Perform();
                        //item.FindElement(By.TagName("a")).Click();
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
        public static void AuthenticateUser_UploadFileForREC(string FilePath, ref clsUploadedFileJsonResponseObject JsonResponseObj, ref DataSet dsCSV_JobID, string UploadURL, string referer, string paramname, bool IsPVDJob, FormBot.Entity.RECAccount objResellerUser, string STcJobDetailsId, int UserId, string SerialNumber = "", string RecBulkUploadId = "", string RefNumber = "", string OwnerLastName = "", string FromDate = "", string spvParamName = "", string spvFilePath = "", string sguBulkUploadDocumentsParamName = "", string sguBulkUploadDocumentsFilePath = "", string tempRECBulkUploadId = "")
        {
            //LoginCpannelEmail();
            #region Login
            Common.Log("AuthenticateUser_UploadFileForREC For Batch start: " + tempRECBulkUploadId);
            CookieContainer cookies = new CookieContainer();
            System.Net.ServicePointManager.Expect100Continue = false;
            STCInvoiceBAL obj = new STCInvoiceBAL();

            // ChromeOptions options = new ChromeOptions();
            //options.AddArgument("no-sandbox");
            //options.AddAdditionalCapability("useAutomationExtension", false);
            //options.AddExcludedArgument("enable-automation");
            //options.AddArguments("headless");
            // ChromeDriver driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), options, TimeSpan.FromMinutes(3));
            //driver.Manage().Window.Minimize();
            //driver.Manage().Timeouts().PageLoad.Add(System.TimeSpan.FromSeconds(30));
            Common.Log("AuthenticateUser_UploadFileForREC For Batch: " + tempRECBulkUploadId);
        Relogin:
            ChromeDriver driver = null;
            try
            {

                {
                    Common.Log("Login Page Loaded For Batch: " + tempRECBulkUploadId);

                    driver = LoginAndValidateOTP(tempRECBulkUploadId).Result;
                    Common.Log("successfully login done..");


                    Thread.Sleep(50000);
                    Common.Log("Login Successful");
                    obj.UpdateQueuedSubmissionStatus(tempRECBulkUploadId, "Login");

                    ReadOnlyCollection<IWebElement> lstEle = driver.FindElements(By.TagName("input"));
                    Common.Log("Login Successful lstEleCnt: " + lstEle.Count);
                    if (lstEle != null & lstEle.Count > 5)
                    {
                        if (lstEle != null && lstEle.Count > 0)
                        {
                            IWebElement eleAccount = lstEle.Where(a => a.GetAttribute("onclick") == "submitUser('" + objResellerUser.RECAccName + "')").FirstOrDefault();
                            Common.Log("Logged IN into : " + objResellerUser.RECAccName);
                            if (eleAccount != null)
                            {
                                eleAccount.Click();
                            }
                            else
                            {
                                JsonResponseObj.status = "Failed";
                                JsonResponseObj.strErrors = "<ul><li>Username passsword is invalid for this reseller.</li></ul>";
                            }
                        }

                        Match m = Regex.Match(driver.PageSource, "name=\"_csrf\" content=\"(.*)\">");
                        string CSRFToken = string.Empty;

                        //CSRF
                        bool IsUnknownError = false;
                        if (m.Success)
                        {
                            CSRFToken = m.Groups[1].Value;
                            NameValueCollection nvc = new NameValueCollection();
                            bool IsTimeOut = false;
                            bool IsError = false;


                            SearchFileResult BulkUploadIdResult = new SearchFileResult();

                            Common.Log("RecBulkUploadId" + tempRECBulkUploadId);
                            obj.UpdateQueuedSubmissionStatus(tempRECBulkUploadId, "Search");
                            Common.Log("Update REC Status Completed " + tempRECBulkUploadId);
                            //Search by BulkUploadId
                            if (!string.IsNullOrWhiteSpace(RecBulkUploadId))
                            {
                                Common.Log("Method: AuthenticateUser_UploadFileForREC  Activity: Search by RecBulkUploadId FilePath:" + FilePath);
                                BulkUploadIdResult = SearchByUploadID(cookies, CSRFToken, IsPVDJob, RecBulkUploadId, "", "", "", "", driver, tempRECBulkUploadId);
                                if (BulkUploadIdResult.status?.ToLower() == "completed")
                                {
                                    Common.Log("Method: AuthenticateUser_UploadFileForREC  Activity: UpdatePVDCode from RecBulkUploadId FilePAth:" + FilePath);
                                    JsonResponseObj.IsPVDCodeUpdated = UpdatePVDCode(BulkUploadIdResult, dsCSV_JobID, IsPVDJob, UserId);
                                    Common.Log("Method: AuthenticateUser_UploadFileForREC  Activity: UpdatePVDCode IsPVDCodeUpdated:" + JsonResponseObj.IsPVDCodeUpdated);

                                }
                            }

                            //Search by SerialNumber
                            if (BulkUploadIdResult.status?.ToLower() != "completed" && !string.IsNullOrEmpty(SerialNumber))
                            {
                                Common.Log("2");
                                Common.Log("Method: AuthenticateUser_UploadFileForREC  Activity: Search by SerialNumber FilePAth:" + FilePath);
                                BulkUploadIdResult = SearchByUploadID(cookies, CSRFToken, IsPVDJob, string.Empty, SerialNumber, RefNumber, OwnerLastName, FromDate, driver, tempRECBulkUploadId);
                                if (BulkUploadIdResult.status?.ToLower() == "completed")
                                {
                                    if (BulkUploadIdResult.result.results.Length > 0)
                                    {

                                        obj.UpdateRECUploadId(STcJobDetailsId, BulkUploadIdResult.result.results[0].bulkUploadId);
                                        //for cache update rec bulk upload id
                                        //CreateJobBAL createJob = new CreateJobBAL();
                                        //DataTable dt = createJob.GetSTCDetailsAndJobDataForCache(STcJobDetailsId, null);
                                        //if (dt != null && dt.Rows.Count > 0)
                                        //{
                                        //    for (int i = 0; i < dt.Rows.Count; i++)
                                        //    {
                                        //        SortedList<string, string> data = new SortedList<string, string>();
                                        //        string gbBatchRECUploadId = dt.Rows[i]["GBBatchRECUploadId"].ToString();
                                        //        data.Add("GBBatchRECUploadId", gbBatchRECUploadId);
                                        //        //FormBot.BAL.Service.CommonRules.CommonBAL.SetCacheDataForSTCSubmission(Convert.ToInt32(dt.Rows[i]["STCJobDetailsID"].ToString()), null, data);
                                        //        SetCacheDataForStcJobIdFromService(Convert.ToInt32(dt.Rows[i]["STCJobDetailsID"].ToString()), null, data);

                                        //        Common.Log(DateTime.Now + " Update cache for stcid from WindowService AuthenticateUser_UploadFileFOrREc: " + ((dt.Rows[i]["STCJobDetailsID"].ToString()) + " BulkUploadId: " + gbBatchRECUploadId));
                                        //    }

                                        //}


                                        SearchFileResult PVDCodeResult = SearchByUploadID(cookies, CSRFToken, IsPVDJob, Convert.ToString(BulkUploadIdResult.result.results[0].bulkUploadId), "", "", "", "", driver, tempRECBulkUploadId);
                                        if (PVDCodeResult.status?.ToLower() == "completed")
                                        {
                                            Common.Log("Method: AuthenticateUser_UploadFileForREC  Activity: UpdatePVDCode from search by serial number FilePAth:" + FilePath);
                                            JsonResponseObj.IsPVDCodeUpdated = UpdatePVDCode(PVDCodeResult, dsCSV_JobID, IsPVDJob, UserId);

                                            Common.Log("Method: AuthenticateUser_UploadFileForREC  Activity: UpdatePVDCode IsPVDCodeUpdated:" + JsonResponseObj.IsPVDCodeUpdated);

                                        }

                                        JsonResponseObj.status = "Completed";
                                    }
                                    else
                                    {
                                        Common.Log("Method: AuthenticateUser_UploadFileForREC  Activity: UpdateRecSearchFailedRecord for STcJobDetailsId:" + STcJobDetailsId);
                                        obj.UpdateRecSearchFailedRecord(STcJobDetailsId, true);
                                        JsonResponseObj.status = "Failed";
                                    }
                                }
                                else
                                {
                                    Common.Log("Method: AuthenticateUser_UploadFileForREC  Activity: UpdateRecSearchFailedRecord for STcJobDetailsId:" + STcJobDetailsId);
                                    obj.UpdateRecSearchFailedRecord(STcJobDetailsId, true);
                                    JsonResponseObj.status = "Failed";
                                }
                            }

                            //Send to REC
                            Common.Log("Send To Rec");
                            obj.UpdateQueuedSubmissionStatus(tempRECBulkUploadId, "Send to REC");
                            Common.Log("BulkUploadIdResultStatus: " + SerialNumber);
                            if (string.IsNullOrWhiteSpace(SerialNumber) || BulkUploadIdResult.status?.ToLower() != "completed")
                            {
                                Common.Log("Method: AuthenticateUser_UploadFileForREC  Activity: UploadFileInREC FilePAth:" + FilePath);

                                obj.UpdateRECFailedRecord(STcJobDetailsId, false);
                                Common.Log("Method: AuthenticateUser_UploadFileForREC  Activity: UpdateRECFailedRecord for STcJobDetailsId:" + STcJobDetailsId);

                                obj.UpdateQueuedSubmissionStatus(tempRECBulkUploadId, "Uploading Files");
                                HttpUploadFile(UploadURL, FilePath, referer, paramname, "application/vnd.ms-excel", nvc, cookies, CSRFToken, ref JsonResponseObj, ref dsCSV_JobID, IsPVDJob, STcJobDetailsId, ref IsTimeOut, ref IsError, ref IsUnknownError, UserId, spvParamName, "application/zip", spvFilePath, sguBulkUploadDocumentsParamName, "application/zip", sguBulkUploadDocumentsFilePath, driver, tempRECBulkUploadId);

                                if (IsTimeOut)
                                {
                                    string TimeoutSerialNumber = string.Empty;
                                    RefNumber = Convert.ToString(dsCSV_JobID.Tables[0].Rows[0]["Reference"]);
                                    OwnerLastName = Convert.ToString(dsCSV_JobID.Tables[0].Rows[0]["Owner surname"]);
                                    if (IsPVDJob)
                                        TimeoutSerialNumber = Convert.ToString(dsCSV_JobID.Tables[0].Rows[0]["Equipment model serial number(s)"]).Split(';')[0];
                                    else
                                        TimeoutSerialNumber = Convert.ToString(dsCSV_JobID.Tables[0].Rows[0]["Tank serial number(s)"]).Split(';')[0];
                                    SearchFileResult BulkUploadIdResult1 = SearchByUploadID(cookies, CSRFToken, IsPVDJob, string.Empty, TimeoutSerialNumber, "", "", FromDate, driver, tempRECBulkUploadId);
                                    if (BulkUploadIdResult1.status?.ToLower() == "completed")
                                    {
                                        if (BulkUploadIdResult1.result.results.Length > 0)
                                        {
                                            obj.UpdateQueuedSubmissionStatus(tempRECBulkUploadId, "Updating PVD Code");
                                            SearchFileResult PVDCodeResult = SearchByUploadID(cookies, CSRFToken, IsPVDJob, Convert.ToString(BulkUploadIdResult1.result.results[0].bulkUploadId), "", "", "", "", driver, tempRECBulkUploadId);
                                            if (PVDCodeResult.status?.ToLower() == "completed")
                                            {
                                                JsonResponseObj.IsPVDCodeUpdated = UpdatePVDCode(PVDCodeResult, dsCSV_JobID, IsPVDJob, UserId);
                                                if (JsonResponseObj.IsPVDCodeUpdated)
                                                {
                                                    obj.UpdateQueuedSubmissionStatus(tempRECBulkUploadId, "Completed");
                                                    IsError = false;
                                                }
                                                else
                                                {
                                                    obj.UpdateInternalIssue(tempRECBulkUploadId, "Error while Updating PVD Code");
                                                }
                                                Common.Log("Method: AuthenticateUser_UploadFileForREC  Activity: UpdatePVDCode IsPVDCodeUpdated:" + JsonResponseObj.IsPVDCodeUpdated);
                                            }
                                            else
                                            {
                                                obj.UpdateInternalIssue(tempRECBulkUploadId, "Error while Updating PVD Code");
                                            }
                                        }
                                        else
                                        {
                                            //obj.UpdateQueuedSubmissionStatus(tempRECBulkUploadId, "Uploading Failure reason");
                                            Common.Log("Method: AuthenticateUser_UploadFileForREC  Activity: UpdateRecSearchFailedRecord for stcjobdetailId:" + STcJobDetailsId);
                                            obj.UpdateRecSearchFailedRecord(STcJobDetailsId, true);
                                        }
                                    }
                                }
                                if (IsError)
                                {
                                    JsonResponseObj.status = "Failed";
                                    JsonResponseObj.strErrors = "Timed out after 300 Seconds";
                                    return;
                                }
                            }
                        }
                        else
                        {
                            JsonResponseObj.status = "Failed";
                            JsonResponseObj.strErrors = "<ul><li>Username passsword is invalid for this reseller.</li></ul>";
                        }
                        #endregion
                        if (!IsUnknownError)
                            obj.UpdateQueuedSubmissionStatus(tempRECBulkUploadId, "Completed");
                    }
                    else
                    {
                        // this means we are on a wrong page we need to logout and re try
                        driver?.FindElement(By.XPath("//*[@id=\"j_logout\"]")).Click();
                        Thread.Sleep(5000);
                        driver?.Close();
                        driver?.Quit();
                        Common.Log("AuthenticateUser_UploadFileForREC For Batch: Relogin Started now");

                        goto Relogin;
                    }

                }
                //}
            }
            catch (Exception ex)
            {
                obj.UpdateInternalIssue(tempRECBulkUploadId, ex.Message.ToString());
                _logger.LogException(SystemEnums.Severity.Error, "AuthenticateUser_UploadFileForREC Error", ex);
                Common.Log("AuthenticateUser_UploadFileForREC Error: " + ex.Message);
                JsonResponseObj.status = "Failed";
                JsonResponseObj.strErrors = "<ul><li>" + ex.Message + "</li></ul>";
            }
            finally
            {
                driver?.Quit();
            }
        }
        public static void AuthenticateUser_UploadFileForREC_Old(string FilePath, ref clsUploadedFileJsonResponseObject JsonResponseObj, ref DataSet dsCSV_JobID, string UploadURL, string referer, string paramname, bool IsPVDJob, FormBot.Entity.RECAccount objResellerUser, string STcJobDetailsId, int UserId, string SerialNumber = "", string RecBulkUploadId = "", string RefNumber = "", string OwnerLastName = "", string FromDate = "", string spvParamName = "", string spvFilePath = "", string sguBulkUploadDocumentsParamName = "", string sguBulkUploadDocumentsFilePath = "", string tempRECBulkUploadId = "")
        {
            #region Login
            CookieContainer cookies = new CookieContainer();
            System.Net.ServicePointManager.Expect100Continue = false;
            STCInvoiceBAL obj = new STCInvoiceBAL();

            ChromeOptions options = new ChromeOptions();
            options.AddArgument("no-sandbox");
            options.AddAdditionalCapability("useAutomationExtension", false);
            options.AddExcludedArgument("enable-automation");
            //options.AddArguments("headless");
            ChromeDriver driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), options, TimeSpan.FromMinutes(3));
            driver.Manage().Window.Minimize();
            driver.Manage().Timeouts().PageLoad.Add(System.TimeSpan.FromSeconds(30));
            Common.Log("AuthenticateUser_UploadFileForREC For Batch: " + tempRECBulkUploadId);
            try
            {
                driver.Navigate().GoToUrl(ConfigurationManager.AppSettings["RECAuthURL"]);
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(300000));
                Func<IWebDriver, bool> isResponseReceivedLoad =
                    d =>
                    {
                        IWebElement eError = d.FindElement(By.Id("signInName"));
                        return eError.Displayed;
                    };
                wait.Until(isResponseReceivedLoad);
                {
                    Common.Log("Login Page Loaded For Batch: " + tempRECBulkUploadId);
                    IWebElement ele = driver.FindElement(By.Id("signInName"));
                    IWebElement ele2 = driver.FindElement(By.Id("password"));
                    ele.SendKeys(objResellerUser.CERLoginId);
                    ele2.SendKeys(objResellerUser.CERPassword);
                    IWebElement ele1 = driver.FindElement(By.Id("next"));
                    ele1.Click();
                    Thread.Sleep(5000);

                    if (Exists(FindElementSafe(driver, By.ClassName("error"))))
                    {
                        IWebElement eleError = driver.FindElement(By.ClassName("error"));
                        if (eleError != null)
                        {
                            if (eleError.Displayed)
                            {
                                obj.UpdateInternalIssue(tempRECBulkUploadId, "Invalid UserName or Password");
                            }
                        }
                    }
                    else
                    {
                        Common.Log("Login Successful");
                        obj.UpdateQueuedSubmissionStatus(tempRECBulkUploadId, "Login");
                        ReadOnlyCollection<IWebElement> lstEle = driver.FindElements(By.ClassName("btn-primary"));

                        if (lstEle != null && lstEle.Count > 0)
                        {
                            IWebElement eleAccount = lstEle.Where(a => a.GetAttribute("onclick") == "submitUser('" + objResellerUser.RECAccName + "')").FirstOrDefault();
                            Common.Log("Logged IN into : " + objResellerUser.RECAccName);
                            if (eleAccount != null)
                            {
                                eleAccount.Click();
                            }
                            else
                            {
                                JsonResponseObj.status = "Failed";
                                JsonResponseObj.strErrors = "<ul><li>Username passsword is invalid for this reseller.</li></ul>";
                            }
                        }

                        Match m = Regex.Match(driver.PageSource, "name=\"_csrf\" content=\"(.*)\">");
                        string CSRFToken = string.Empty;

                        //CSRF
                        bool IsUnknownError = false;
                        if (m.Success)
                        {
                            CSRFToken = m.Groups[1].Value;
                            NameValueCollection nvc = new NameValueCollection();
                            bool IsTimeOut = false;
                            bool IsError = false;


                            SearchFileResult BulkUploadIdResult = new SearchFileResult();

                            Common.Log("RecBulkUploadId" + tempRECBulkUploadId);
                            obj.UpdateQueuedSubmissionStatus(tempRECBulkUploadId, "Search");
                            Common.Log("Update REC Status Completed " + tempRECBulkUploadId);
                            //Search by BulkUploadId
                            if (!string.IsNullOrWhiteSpace(RecBulkUploadId))
                            {
                                Common.Log("Method: AuthenticateUser_UploadFileForREC  Activity: Search by RecBulkUploadId FilePath:" + FilePath);
                                BulkUploadIdResult = SearchByUploadID(cookies, CSRFToken, IsPVDJob, RecBulkUploadId, "", "", "", "", driver, tempRECBulkUploadId);
                                if (BulkUploadIdResult.status?.ToLower() == "completed")
                                {
                                    Common.Log("Method: AuthenticateUser_UploadFileForREC  Activity: UpdatePVDCode from RecBulkUploadId FilePAth:" + FilePath);
                                    JsonResponseObj.IsPVDCodeUpdated = UpdatePVDCode(BulkUploadIdResult, dsCSV_JobID, IsPVDJob, UserId);
                                    Common.Log("Method: AuthenticateUser_UploadFileForREC  Activity: UpdatePVDCode IsPVDCodeUpdated:" + JsonResponseObj.IsPVDCodeUpdated);

                                }
                            }

                            //Search by SerialNumber
                            if (BulkUploadIdResult.status?.ToLower() != "completed" && !string.IsNullOrEmpty(SerialNumber))
                            {
                                Common.Log("2");
                                Common.Log("Method: AuthenticateUser_UploadFileForREC  Activity: Search by SerialNumber FilePAth:" + FilePath);
                                BulkUploadIdResult = SearchByUploadID(cookies, CSRFToken, IsPVDJob, string.Empty, SerialNumber, RefNumber, OwnerLastName, FromDate, driver, tempRECBulkUploadId);
                                if (BulkUploadIdResult.status?.ToLower() == "completed")
                                {
                                    if (BulkUploadIdResult.result.results.Length > 0)
                                    {

                                        obj.UpdateRECUploadId(STcJobDetailsId, BulkUploadIdResult.result.results[0].bulkUploadId);
                                        //for cache update rec bulk upload id
                                        //CreateJobBAL createJob = new CreateJobBAL();
                                        //DataTable dt = createJob.GetSTCDetailsAndJobDataForCache(STcJobDetailsId, null);
                                        //if (dt != null && dt.Rows.Count > 0)
                                        //{
                                        //    for (int i = 0; i < dt.Rows.Count; i++)
                                        //    {
                                        //        SortedList<string, string> data = new SortedList<string, string>();
                                        //        string gbBatchRECUploadId = dt.Rows[i]["GBBatchRECUploadId"].ToString();
                                        //        data.Add("GBBatchRECUploadId", gbBatchRECUploadId);
                                        //        //FormBot.BAL.Service.CommonRules.CommonBAL.SetCacheDataForSTCSubmission(Convert.ToInt32(dt.Rows[i]["STCJobDetailsID"].ToString()), null, data);
                                        //        SetCacheDataForStcJobIdFromService(Convert.ToInt32(dt.Rows[i]["STCJobDetailsID"].ToString()), null, data);

                                        //        Common.Log(DateTime.Now + " Update cache for stcid from WindowService AuthenticateUser_UploadFileFOrREc: " + ((dt.Rows[i]["STCJobDetailsID"].ToString()) + " BulkUploadId: " + gbBatchRECUploadId));
                                        //    }

                                        //}


                                        SearchFileResult PVDCodeResult = SearchByUploadID(cookies, CSRFToken, IsPVDJob, Convert.ToString(BulkUploadIdResult.result.results[0].bulkUploadId), "", "", "", "", driver, tempRECBulkUploadId);
                                        if (PVDCodeResult.status?.ToLower() == "completed")
                                        {
                                            Common.Log("Method: AuthenticateUser_UploadFileForREC  Activity: UpdatePVDCode from search by serial number FilePAth:" + FilePath);
                                            JsonResponseObj.IsPVDCodeUpdated = UpdatePVDCode(PVDCodeResult, dsCSV_JobID, IsPVDJob, UserId);

                                            Common.Log("Method: AuthenticateUser_UploadFileForREC  Activity: UpdatePVDCode IsPVDCodeUpdated:" + JsonResponseObj.IsPVDCodeUpdated);

                                        }

                                        JsonResponseObj.status = "Completed";
                                    }
                                    else
                                    {
                                        Common.Log("Method: AuthenticateUser_UploadFileForREC  Activity: UpdateRecSearchFailedRecord for STcJobDetailsId:" + STcJobDetailsId);
                                        obj.UpdateRecSearchFailedRecord(STcJobDetailsId, true);
                                        JsonResponseObj.status = "Failed";
                                    }
                                }
                                else
                                {
                                    Common.Log("Method: AuthenticateUser_UploadFileForREC  Activity: UpdateRecSearchFailedRecord for STcJobDetailsId:" + STcJobDetailsId);
                                    obj.UpdateRecSearchFailedRecord(STcJobDetailsId, true);
                                    JsonResponseObj.status = "Failed";
                                }
                            }

                            //Send to REC
                            Common.Log("Send To Rec");
                            obj.UpdateQueuedSubmissionStatus(tempRECBulkUploadId, "Send to REC");
                            Common.Log("BulkUploadIdResultStatus: " + SerialNumber);
                            if (string.IsNullOrWhiteSpace(SerialNumber) || BulkUploadIdResult.status?.ToLower() != "completed")
                            {
                                Common.Log("Method: AuthenticateUser_UploadFileForREC  Activity: UploadFileInREC FilePAth:" + FilePath);

                                obj.UpdateRECFailedRecord(STcJobDetailsId, false);
                                Common.Log("Method: AuthenticateUser_UploadFileForREC  Activity: UpdateRECFailedRecord for STcJobDetailsId:" + STcJobDetailsId);

                                obj.UpdateQueuedSubmissionStatus(tempRECBulkUploadId, "Uploading Files");
                                HttpUploadFile(UploadURL, FilePath, referer, paramname, "application/vnd.ms-excel", nvc, cookies, CSRFToken, ref JsonResponseObj, ref dsCSV_JobID, IsPVDJob, STcJobDetailsId, ref IsTimeOut, ref IsError, ref IsUnknownError, UserId, spvParamName, "application/zip", spvFilePath, sguBulkUploadDocumentsParamName, "application/zip", sguBulkUploadDocumentsFilePath, driver, tempRECBulkUploadId);

                                if (IsTimeOut)
                                {
                                    string TimeoutSerialNumber = string.Empty;
                                    RefNumber = Convert.ToString(dsCSV_JobID.Tables[0].Rows[0]["Reference"]);
                                    OwnerLastName = Convert.ToString(dsCSV_JobID.Tables[0].Rows[0]["Owner surname"]);
                                    if (IsPVDJob)
                                        TimeoutSerialNumber = Convert.ToString(dsCSV_JobID.Tables[0].Rows[0]["Equipment model serial number(s)"]).Split(';')[0];
                                    else
                                        TimeoutSerialNumber = Convert.ToString(dsCSV_JobID.Tables[0].Rows[0]["Tank serial number(s)"]).Split(';')[0];
                                    SearchFileResult BulkUploadIdResult1 = SearchByUploadID(cookies, CSRFToken, IsPVDJob, string.Empty, TimeoutSerialNumber, "", "", FromDate, driver, tempRECBulkUploadId);
                                    if (BulkUploadIdResult1.status?.ToLower() == "completed")
                                    {
                                        if (BulkUploadIdResult1.result.results.Length > 0)
                                        {
                                            obj.UpdateQueuedSubmissionStatus(tempRECBulkUploadId, "Updating PVD Code");
                                            SearchFileResult PVDCodeResult = SearchByUploadID(cookies, CSRFToken, IsPVDJob, Convert.ToString(BulkUploadIdResult1.result.results[0].bulkUploadId), "", "", "", "", driver, tempRECBulkUploadId);
                                            if (PVDCodeResult.status?.ToLower() == "completed")
                                            {
                                                JsonResponseObj.IsPVDCodeUpdated = UpdatePVDCode(PVDCodeResult, dsCSV_JobID, IsPVDJob, UserId);
                                                if (JsonResponseObj.IsPVDCodeUpdated)
                                                {
                                                    obj.UpdateQueuedSubmissionStatus(tempRECBulkUploadId, "Completed");
                                                    IsError = false;
                                                }
                                                else
                                                {
                                                    obj.UpdateInternalIssue(tempRECBulkUploadId, "Error while Updating PVD Code");
                                                }
                                                Common.Log("Method: AuthenticateUser_UploadFileForREC  Activity: UpdatePVDCode IsPVDCodeUpdated:" + JsonResponseObj.IsPVDCodeUpdated);
                                            }
                                            else
                                            {
                                                obj.UpdateInternalIssue(tempRECBulkUploadId, "Error while Updating PVD Code");
                                            }
                                        }
                                        else
                                        {
                                            //obj.UpdateQueuedSubmissionStatus(tempRECBulkUploadId, "Uploading Failure reason");
                                            Common.Log("Method: AuthenticateUser_UploadFileForREC  Activity: UpdateRecSearchFailedRecord for stcjobdetailId:" + STcJobDetailsId);
                                            obj.UpdateRecSearchFailedRecord(STcJobDetailsId, true);
                                        }
                                    }
                                }
                                if (IsError)
                                {
                                    JsonResponseObj.status = "Failed";
                                    JsonResponseObj.strErrors = "Timed out after 300 Seconds";
                                    return;
                                }
                            }
                        }
                        else
                        {
                            JsonResponseObj.status = "Failed";
                            JsonResponseObj.strErrors = "<ul><li>Username passsword is invalid for this reseller.</li></ul>";
                        }
                        #endregion
                        if (!IsUnknownError)
                            obj.UpdateQueuedSubmissionStatus(tempRECBulkUploadId, "Completed");
                    }
                }
            }
            catch (Exception ex)
            {
                obj.UpdateInternalIssue(tempRECBulkUploadId, ex.Message.ToString());
                _logger.LogException(SystemEnums.Severity.Error, "AuthenticateUser_UploadFileForREC Error", ex);
                Common.Log("AuthenticateUser_UploadFileForREC Error: " + ex.Message);
                JsonResponseObj.status = "Failed";
                JsonResponseObj.strErrors = "<ul><li>" + ex.Message + "</li></ul>";
            }
            finally
            {
                driver?.Quit();
            }
        }

        /// <summary>
        /// Search By Upload ID
        /// </summary>
        /// <param name="cookies">The cookies.</param>
        /// <param name="CSRFToken">The CSRF token.</param>
        /// <param name="IsPVDJob">IsPVDJob.</param>
        /// <param name="intBulkUploadID">The int bulk upload identifier.</param>
        /// <returns>search file result</returns>
        public static SearchFileResult SearchByUploadID(CookieContainer cookies, string CSRFToken, bool IsPVDJob, string BulkUploadID = "", string SerialNumber = "", string refNumber = "", string ownersSurname = "", string fromDate = "", ChromeDriver driver = null, string tempBatchRecUploadId = null)
        {
        refresh:
            try
            {

                Common.Log("Search Start");
                IWebElement eleAccount = driver.FindElement(By.Id("menu-renewable-energy-systems"));
                driver.Navigate().GoToUrl(ProjectConfiguration.RECSearchURL);
                Common.Log("Navigated to : " + ProjectConfiguration.RECSearchURL);

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
                    IWebElement reference = driver.FindElement(By.Id("reference"));
                    IWebElement serialnumber = driver.FindElement(By.Id("serial-number"));
                    IWebElement bulkuploadid = driver.FindElement(By.Id("bulk-upload-id"));
                    IWebElement createddateafter = driver.FindElement(By.Id("created-date-after"));
                    IWebElement createddatebefore = driver.FindElement(By.Id("created-date-before"));

                    string[] stringSeparators = new string[] { "\r\n" };
                    SerialNumber = SerialNumber.Split(stringSeparators, StringSplitOptions.None).ToList()[0];

                    string toDate = DateTime.Now.ToString("dd/MM/yyyy").Replace("-", "/");
                    string frmDate = string.IsNullOrEmpty(fromDate) ? string.Empty : Convert.ToDateTime(fromDate).ToString("dd/MM/yyyy").Replace("-", "/");

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

                    Common.Log("Activity Search:" + refNumber);

                    IWebElement searchBtn = driver.FindElement(By.Id("search-button"));
                    if (searchBtn != null)
                    {
                        js.ExecuteScript("$('#search-results').show()");
                        searchBtn.Click();
                        Common.Log("Search Button Clicked");
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
                                if (searchTable != null && !searchTable.Text.Contains("No data available in table"))
                                {
                                    searchRes.status = "Completed";
                                    searchRes.objectErrors = null;
                                    searchRes.result = new Result();
                                    IWebElement tBody = searchTable.FindElement(By.TagName("tbody"));
                                    List<IWebElement> rows = tBody.FindElements(By.TagName("tr")).ToList();
                                    Common.Log("Search Counts: " + rows.Count);
                                    if (rows.Count > 0)
                                    {
                                        Common.Log("start if");
                                        if (Exists(FindElementSafe(driver, By.ClassName("dataTables_empty"))))
                                        {
                                            IWebElement dataEmptyRow = driver.FindElement(By.ClassName("dataTables_empty"));
                                            if (dataEmptyRow != null)
                                            {
                                                Common.Log("Empty Row");
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
                                                Common.Log("ACC Code: " + colData[0].Text + "InstallerNum: " + colData[7].Text + "bulkId: " + colData[8].Text + "serialNum: " + colData[6].Text + "installationdate: " + colData[1].Text + "State: " + colData[3].Text + "OwnerName:" + colData[2].Text);
                                                ResultDetails det = new ResultDetails();
                                                det.accreditationCode = colData[0].Text;
                                                det.accreditedInstallerNumber = colData[7].Text;
                                                if (!string.IsNullOrWhiteSpace(colData[8].Text))
                                                {
                                                    det.bulkUploadId = Convert.ToInt32(colData[8].Text);
                                                }
                                                det.commaSeparatedSerialNumbers = colData[6].Text;
                                                if (!string.IsNullOrWhiteSpace(colData[1].Text))
                                                {
                                                    det.installationDate = Convert.ToDateTime(colData[1].Text.Split('/')[2] + "-" + colData[1].Text.Split('/')[1] + "-" + colData[1].Text.Split('/')[0]);
                                                }
                                                det.installationState = new Installationstate { displayName = colData[3].Text, name = colData[3].Text };
                                                det.ownerName = colData[2].Text;

                                                //det.registrationCode = colData[]
                                                searchRes.result.results[i] = det;
                                                Common.Log("end fill resultDetails:");
                                            }
                                            Common.Log("Final Done.");
                                            return searchRes;

                                        }
                                    }
                                    else
                                    {
                                        Common.Log("else start");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (WebDriverTimeoutException ex)
            {
                STCInvoiceBAL obj = new STCInvoiceBAL();
                obj.UpdateInternalIssue(tempBatchRecUploadId, ex.Message);
                Common.Log("Timeout" + ex.Message);
                driver.Navigate().Refresh();
                goto refresh;
            }
            catch (Exception ex)
            {
                STCInvoiceBAL obj = new STCInvoiceBAL();
                obj.UpdateInternalIssue(tempBatchRecUploadId, ex.Message);
                Common.Log(DateTime.Now + ex.Message);
            }
            //menu-renewable-energy-systems
            Common.Log("Method: SearchByUploadID  Activity: search from reference:");

            return new SearchFileResult();

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
            ref clsUploadedFileJsonResponseObject JsonResponseObj, ref DataSet dsCSV_JobID, bool IsPVDJob, string StcJobDetailsId, ref bool IsTimeOut, ref bool IsError, ref bool IsUnknownError, int UserId, string spvParamName = "",
            string spvContentType = "", string spvFile = "", string sguBulkUploadDocumentsParamName = "", string sguBulkUploadDocumentsContentType = "", string sguBulkUploadDocumentsFile = "",
            ChromeDriver driver = null, string tempRecBulkUploadId = null)
        {
            try
            {
                Common.Log("Method: HttpUploadFile  Activity: HttpUploadFile start for file,StcJobDetailsId,spvFile,sguBulkUploadDocumentsFile:" + file + "," + StcJobDetailsId + "," + spvFile + "," + sguBulkUploadDocumentsFile);
                DateTime april2022Date = Convert.ToDateTime("2022-04-01");
                DateTime? installationDate = null;
                if (dsCSV_JobID != null && dsCSV_JobID.Tables[0] != null && dsCSV_JobID.Tables[0].Rows.Count > 0)
                {
                    if (dsCSV_JobID.Tables[2] != null && dsCSV_JobID.Tables[2].Rows.Count > 0)
                    {
                        installationDate = Convert.ToDateTime(dsCSV_JobID.Tables[2].Rows[0]["Installation date"]);
                    }
                    else
                    {
                        installationDate = DateTime.ParseExact(dsCSV_JobID.Tables[0].Rows[0]["Installation date"].ToString(), "dd/MM/yyyy", null);
                    }
                }
                if (driver != null)
                {
                    driver.Navigate().GoToUrl(ProjectConfiguration.RECRegisterURL);
                    WebDriverWait waitLoad = new WebDriverWait(driver, TimeSpan.FromMilliseconds(300000));
                    Func<IWebDriver, bool> isResponseReceivedLoad =
                        d =>
                        {
                            IWebElement eError = d.FindElement(By.Id("after01042022"));
                            return eError.Displayed;
                        };
                    waitLoad.Until(isResponseReceivedLoad);
                    {
                        Common.Log("Navigated to : " + ProjectConfiguration.RECRegisterURL);

                        IWebElement installtionDateYearOptions = driver.FindElement(By.Id("after01042022"));
                        if (installtionDateYearOptions != null)
                        {
                            var selectElement = new SelectElement(installtionDateYearOptions);
                            if (selectElement != null)
                            {
                                // select by text
                                if (installationDate < april2022Date)
                                    selectElement.SelectByText("Yes");
                                else
                                    selectElement.SelectByText("No");
                            }
                        }

                        IWebElement uploadsguBtn;

                        if (installationDate < april2022Date)
                            uploadsguBtn = driver.FindElement(By.Id("sguBulkUploadFileBefore"));
                        else
                            uploadsguBtn = driver.FindElement(By.Id("sguBulkUploadFileAfter"));

                        if (uploadsguBtn != null)
                        {
                            uploadsguBtn.SendKeys(file);
                            Thread.Sleep(2000);
                        }
                        Common.Log("Zip File Uploading " + tempRecBulkUploadId);

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
                            Common.Log("Upload Button Click " + tempRecBulkUploadId);
                            WebDriverWait waitClick = new WebDriverWait(driver, TimeSpan.FromMilliseconds(300000));
                            Func<IWebDriver, bool> isResponseReceivedClick =
                                d =>
                                {
                                    IWebElement eError = d.FindElement(By.Id("sgu-file-upload-submit"));
                                    return eError.Displayed;
                                };
                            waitClick.Until(isResponseReceivedClick);
                            {
                                uploadBtn.Click();
                            }
                        }

                        IWebElement termsAndConditionCheck = driver.FindElement(By.Id("confirm-information"));
                        if (termsAndConditionCheck != null)
                        {
                            termsAndConditionCheck.Click();
                        }

                        IWebElement confirmbtn = driver.FindElement(By.Id("legal-declaration-confirm"));
                        if (confirmbtn != null)
                        {
                            confirmbtn.Click();
                        }

                        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(600000));
                        Func<IWebDriver, bool> isResponseReceived =
                            d =>
                            {
                                IWebElement eError = d.FindElement(By.Id("error"));
                                IWebElement eSuccess = d.FindElement(By.Id("success"));
                                return eError.Displayed || eSuccess.Displayed;
                            };
                        //wait until the condition is true
                        wait.Until(isResponseReceived);
                        {
                            IWebElement divError = driver.FindElement(By.Id("error"));
                            Common.Log("Error Div: " + divError.Displayed + " Batch ID:" + tempRecBulkUploadId);
                            if (divError != null && divError.Displayed)
                            {
                                Common.Log("Error: " + divError.Text);
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
                                else
                                {
                                    JsonResponseObj.errors.Add("Unknown error return from REC " + tempRecBulkUploadId);
                                }
                            }

                            IWebElement divSuccess = driver.FindElement(By.Id("success"));
                            Common.Log("Success Div: " + divSuccess.Displayed + " Batch ID: " + tempRecBulkUploadId);
                            if (divSuccess != null && divSuccess.Displayed)
                            {
                                Common.Log("Success: " + divSuccess.Text);
                                if (divSuccess.GetAttribute("innerHTML").Contains("Register bulk small generation units succeeded."))
                                {
                                    string divHtml = divSuccess.GetAttribute("innerHTML");
                                    string bulkUploadId = "";
                                    string[] stringSeparators = new string[] { "<br>" };
                                    List<string> txtSuccess = divHtml.Split(stringSeparators, StringSplitOptions.None).ToList();
                                    bulkUploadId = txtSuccess[1].Split(':')[1].Trim();
                                    JsonResponseObj = new clsUploadedFileJsonResponseObject();
                                    JsonResponseObj.status = "Completed";
                                    JsonResponseObj.result = new UploadedFileStatusResult();
                                    if (!string.IsNullOrWhiteSpace(bulkUploadId))
                                    {
                                        JsonResponseObj.result.bulkUploadId = Convert.ToInt32(bulkUploadId);
                                        JsonResponseObj.result.uploadId = Convert.ToInt32(bulkUploadId);
                                    }
                                    // Successfully uploaded file into REC Regsitry
                                }
                            }
                        }
                        if (JsonResponseObj.status == "Completed")
                        {
                            Common.Log("Method: HttpUploadFile  Activity: Sucessfully Uploaded File In REC FilePath:" + file);
                            if ((IsPVDJob == true && JsonResponseObj.result.uploadId != 0) || (IsPVDJob == false && JsonResponseObj.result.bulkUploadId != 0))
                            {
                                //Generate datatable from CSV                            
                                STCInvoiceBAL obj = new STCInvoiceBAL();
                                obj.UpdateRECUploadId(StcJobDetailsId, IsPVDJob == true ? JsonResponseObj.result.uploadId : JsonResponseObj.result.bulkUploadId);
                                //for cache update rec bulk upload id
                                //CreateJobBAL createJob = new CreateJobBAL();
                                //DataTable dt = createJob.GetSTCDetailsAndJobDataForCache(StcJobDetailsId, null);
                                //if (dt != null && dt.Rows.Count > 0)
                                //{
                                //    for (int i = 0; i < dt.Rows.Count; i++)
                                //    {
                                //        Common.Log(DateTime.Now + " Enter in for loop of update cache: " + dt.Rows[i]["GBBatchRECUploadId"].ToString());
                                //        SortedList<string, string> data = new SortedList<string, string>();
                                //        string gbBatchRECUploadId = dt.Rows[i]["GBBatchRECUploadId"].ToString();
                                //        data.Add("GBBatchRECUploadId", gbBatchRECUploadId);
                                //        //FormBot.BAL.Service.CommonRules.CommonBAL.SetCacheDataForSTCSubmission(Convert.ToInt32(dt.Rows[i]["STCJobDetailsID"].ToString()), null, data);
                                //        SetCacheDataForStcJobIdFromService(Convert.ToInt32(dt.Rows[i]["STCJobDetailsID"].ToString()), null, data);

                                //        Common.Log(DateTime.Now + " Update cache for stcid from WindowService HttpUploadFile: " + ((dt.Rows[i]["STCJobDetailsID"].ToString()) + " BulkUploadId: " + gbBatchRECUploadId));
                                //    }

                                //}

                                SearchFileResult model = SearchByUploadID(cookies, CSRFToken, IsPVDJob, IsPVDJob == true ? Convert.ToString(JsonResponseObj.result.uploadId) : Convert.ToString(JsonResponseObj.result.bulkUploadId), driver: driver, tempBatchRecUploadId: tempRecBulkUploadId);
                                Common.Log(DateTime.Now + " SerachByUploadID: status:" + model.status?.ToLower());
                                if (model.status?.ToLower() == "completed")
                                {
                                    Common.Log("Method: HttpUploadFile  Activity: UpdatePVDCode FilePath:" + file);
                                    JsonResponseObj.IsPVDCodeUpdated = UpdatePVDCode(model, dsCSV_JobID, IsPVDJob, UserId);
                                    Common.Log("Method: HttpUlpoadFile  Activity: UpdatePVDCode IsPVDCodeUpdated:" + JsonResponseObj.IsPVDCodeUpdated);

                                }

                                //if (dt != null && dt.Rows.Count > 0)
                                //{
                                //    for (int i = 0; i < dt.Rows.Count; i++)
                                //    {
                                //        Common.Log(DateTime.Now + " Enter in for loop of update cache: " + dt.Rows[i]["GBBatchRECUploadId"].ToString());
                                //        SortedList<string, string> data = new SortedList<string, string>();
                                //        string gbBatchRECUploadId = dt.Rows[i]["GBBatchRECUploadId"].ToString();
                                //        data.Add("GBBatchRECUploadId", gbBatchRECUploadId);
                                //        //FormBot.BAL.Service.CommonRules.CommonBAL.SetCacheDataForSTCSubmission(Convert.ToInt32(dt.Rows[i]["STCJobDetailsID"].ToString()), null, data);
                                //        SetCacheDataForStcJobIdFromService(Convert.ToInt32(dt.Rows[i]["STCJobDetailsID"].ToString()), null, data);

                                //        Common.Log(DateTime.Now + " Update cache for stcid from WindowService HttpUploadFile: " + ((dt.Rows[i]["STCJobDetailsID"].ToString()) + " BulkUploadId: " + gbBatchRECUploadId));
                                //    }

                                //}

                            }
                        }
                        else if (JsonResponseObj.status == "Failed")
                        {
                            #region Failed Response
                            Common.Log("Method: HttpUploadFile  Activity: Failed Status in uploadingFileInRec FilePath:" + file);
                            //Update RECUploadId as null
                            STCInvoiceBAL obj = new STCInvoiceBAL();

                            DataTable dtCSVRecords = dsCSV_JobID.Tables[0];
                            Int16 intLineNo = 0;
                            string strReplaceTerm = string.Empty;
                            StringBuilder sb = new StringBuilder();
                            sb.Append("<ul>");
                            Common.Log("Method: HttpUploadFile  Activity: Insert Failure Reasons FilePath:" + file);
                            foreach (var item in JsonResponseObj.errors)
                            {
                                if (!string.IsNullOrEmpty(item) && item.Contains(":"))
                                {
                                    Common.Log("Method: HttpUploadFile  Activity: Insert Failure Reasons inside for loop:" + item);
                                    string OrgItem = item;
                                    if (!item.Contains("Invalid CSV file header") && !item.Contains("Signed Data Package did not validate"))
                                    {
                                        Common.Log("Method: HttpUploadFile  Activity: Insert Failure Reasons inside if condiotion csv header 1:");
                                        string[] arrItem = item.Split(':');
                                        bool isXMLIssue = false;
                                        Common.Log("Method: HttpUploadFile  Activity: Insert Failure Reasons inside if condiotion csv header 2:");
                                        if (arrItem != null && arrItem.Count() > 0)
                                        {
                                            // Find Line No  (Example "Line 1: Column \"Type of system\")
                                            Common.Log("Method: HttpUploadFile  Activity: Insert Failure Reasons inside if condiotion csv header 3 arrItemCount: " + arrItem.Count() + "Arrayitems :" + arrItem[0] + "ArrayItems1: ");

                                            if (!item.Contains("Line") && item.Contains(".xml:") && arrItem[0].Contains(".xml"))
                                            {
                                                intLineNo = 0;
                                                isXMLIssue = true;
                                                Common.Log("Method: HttpUploadFile  Activity: Insert Failure Reasons inside if condiotion csv header 4 arrItem: " + (arrItem[0].Split('_')[1]).ToString());
                                                int Errjobid = Convert.ToInt32((arrItem[0].Split('_')[1].Split('.')[0].ToString()));
                                                int jobId = Errjobid;

                                                sb.Append("<li>" + OrgItem + "</li>");
                                                obj.InsertRECEntryFailureReason(jobId, string.Join(" ", arrItem.Skip(1).ToArray()), UserId);
                                            }

                                            else
                                                intLineNo = Convert.ToInt16(arrItem[0].Split(' ')[1]);
                                            Common.Log("Method: HttpUploadFile  Activity: Insert Failure Reasons inside if condiotion csv header 4 intLineNO: " + intLineNo);
                                            if (intLineNo != 0 && Convert.ToString(dtCSVRecords.Rows[intLineNo - 1]["Reference"]) != string.Empty && dtCSVRecords.Rows.Count >= intLineNo)
                                            {
                                                Common.Log("Method: HttpUploadFile  Activity: Insert Failure Reasons inside if condiotion csv header 5 intLineNO: " + intLineNo);
                                                int jobId = Convert.ToInt32(dtCSVRecords.Rows[intLineNo - 1]["JobId"]);
                                                strReplaceTerm = Convert.ToString(dtCSVRecords.Rows[intLineNo - 1]["Reference"]);
                                                sb.Append("<li>" + OrgItem.Replace(arrItem[0], strReplaceTerm) + "</li>");
                                                obj.InsertRECEntryFailureReason(jobId, string.Join(" ", arrItem.Skip(1).ToArray()), UserId);
                                            }


                                            else if (isXMLIssue == false)
                                            {
                                                Common.Log("Method: HttpUploadFile  Activity: Insert Failure Reasons inside if condiotion csv header 6 intLineNO: " + intLineNo);
                                                sb.Append("<li>" + item + "</li>");
                                                Common.Log("Method: HttpUploadFile  Activity: Insert Failure Reasons inside if condiotion csv header 7 intLineNO: " + intLineNo);
                                                obj.UpdateInternalIssue(tempRecBulkUploadId, item);
                                                IsUnknownError = true;
                                                Common.Log("Method: HttpUploadFile  Activity: Insert Failure Reasons inside if condiotion csv header 8 intLineNO: " + intLineNo);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Common.Log("Method: HttpUploadFile  Activity: Insert Failure Reasons inside else condiotion csv header 1: ");
                                        sb.Append("<li>" + item + "</li>");
                                        Common.Log("Method: HttpUploadFile  Activity: Insert Failure Reasons inside else condiotion csv header 2: ");
                                        obj.UpdateInternalIssue(tempRecBulkUploadId, item);
                                        IsUnknownError = true;
                                        Common.Log("Method: HttpUploadFile  Activity: Insert Failure Reasons inside else condiotion csv header 3: ");
                                    }
                                }
                                else
                                {
                                    Common.Log("Method: HttpUploadFile  Activity: Insert Failure Reasons inside else condiotion 1: ");
                                    sb.Append("<li>" + item + "</li>");
                                    Common.Log("Method: HttpUploadFile  Activity: Insert Failure Reasons inside else condiotion 2: ");
                                    obj.UpdateInternalIssue(tempRecBulkUploadId, item);
                                    IsUnknownError = true;
                                    Common.Log("Method: HttpUploadFile  Activity: Insert Failure Reasons inside else condiotion 3: ");
                                }
                            }
                            obj.UpdateRECFailedRecord(StcJobDetailsId, true);
                            Common.Log("Method: HttpUploadFile  Activity: UpdateRECFailedRecord for STcJobDetailsId:" + StcJobDetailsId);

                            sb.Append("</ul>");
                            JsonResponseObj.strErrors = sb.ToString();
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.Log("HttpUploadFile Error:" + ex.Message);
                STCInvoiceBAL obj = new STCInvoiceBAL();
                obj.UpdateInternalIssue(tempRecBulkUploadId, ex.Message.ToString());
                IsError = true;
                if (ex.Message.ToLower() == "the remote server returned an error: (401) unauthorized.")
                {
                    IsTimeOut = false;
                }
                else if (ex.Message.ToLower().Contains("timeout") || ex.Message.ToLower().Contains("timed out") || ex.Message.Contains("The underlying connection was closed: A connection that was expected to be kept alive was closed by the server."))
                {
                    IsTimeOut = true;
                }
                else
                {
                    IsTimeOut = false;
                }
            }
        }

        public static bool UpdatePVDCode(SearchFileResult model, DataSet dsCSV_JobID, bool IsPVDJob, int UserId)
        {
            if (model.result.results != null && model.result.results.Count() > 0)
            {
                //file = @"C:\Users\pci36\Desktop\FormBotScripts\Server CSV\636143901514692778.csv";
                //dtCSV_JobID = GetDataTableFromCsv(file, true);
                //_logger.Log(SystemEnums.Severity.Info, "Method: UpdatePVDCode  Activity:Datatable for match data in db and rec data for without spv job:" + dsCSV_JobID.Tables[0]);
                //_logger.Log(SystemEnums.Severity.Info, "Method: UpdatePVDCode  Activity:Datatable for match data in db and rec data for spv job:" + dsCSV_JobID.Tables[2]);
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
                        Common.Log("Method: UpdatePVDCode  Activity:Find in database as uploaded data in rec without spv row count:" + foundRow.Count());
                        if (foundRow != null && foundRow.Count() > 0)
                        {
                            tbSearchResult.Rows.Add(model.result.results[i].accreditationCode,
                                                    Convert.ToString(foundRow[0]["JobId"]));
                            Common.Log("Method: UpdatePVDCode  Activity:Find in database as uploaded data in rec without spv successfully for:" + Convert.ToString(foundRow[0]["JobId"]));
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
                            Common.Log("Method: UpdatePVDCode  Activity:Find in database as uploaded data in rec for spv job row count:" + spvRow.Count());

                            if (spvRow != null && spvRow.Count() > 0)
                            {
                                tbSearchResult.Rows.Add(model.result.results[i].accreditationCode,
                                                  Convert.ToString(spvRow[0]["JobId"]));
                                Common.Log("Method: UpdatePVDCode  Activity:Find in database as uploaded data in rec for spv job successfully for:" + Convert.ToString(spvRow[0]["JobId"]));
                            }

                        }
                    }

                    if (tbSearchResult != null && tbSearchResult.Rows.Count > 0)
                    {
                        Common.Log("Method: UpdatePVDCode  Activity:match data from db and rec getting successfully");
                        CreateJobBAL createJob = new CreateJobBAL();
                        STCInvoiceBAL obj = new STCInvoiceBAL();
                        DataSet ds = obj.UpdatePVDCodeByJobID(tbSearchResult, UserId);

                        //DataTable dt = createJob.GetSTCDetailsAndJobDataForCache(afterRecStcJobIds, null);
                        //if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        //{
                        //    DataTable dt = ds.Tables[0];
                        //    for (int i = 0; i < dt.Rows.Count; i++)
                        //    {
                        //        SortedList<string, string> data = new SortedList<string, string>();
                        //        string pvdswhcode = dt.Rows[i]["PVDCode"].ToString();
                        //        string stcstatus = dt.Rows[i]["STCStatus"].ToString();
                        //        string strStcStatus = (stcstatus != null || stcstatus != "") ? Common.GetDescription((SystemEnums.STCJobStatus)Convert.ToInt32(stcstatus), "") : "";
                        //        string colorCode = (stcstatus != null || stcstatus != "") ? Common.GetSubDescription((SystemEnums.STCJobStatus)Convert.ToInt32(stcstatus), "") : "";

                        //        data.Add("ColorCode", colorCode);
                        //        data.Add("PVDSWHCode", pvdswhcode);
                        //        data.Add("STCStatus", strStcStatus);
                        //        data.Add("STCStatusId", stcstatus);
                        //        //FormBot.BAL.Service.CommonRules.CommonBAL.SetCacheDataForSTCSubmission(Convert.ToInt32(dt.Rows[i]["STCjobdetailsid"].ToString()), null, data);
                        //        SetCacheDataForStcJobIdFromService(Convert.ToInt32(dt.Rows[i]["STCjobdetailsid"].ToString()), null, data);
                        //        Common.Log(DateTime.Now + " Update cache for stcid from WindowService UpdatePvDCode: " + ((dt.Rows[i]["STCJobDetailsID"].ToString()) + " StcSTatus: " + strStcStatus + " PVDSWHCOde:" + pvdswhcode));
                        //    }
                        //    Common.Log("Method: UpdatePVDCode  Activity:UpdatePVDCodeByJobID cache updated successfully. ");
                        //}
                        Common.Log("Method: UpdatePVDCode  Activity:UpdatePVDCodeByJobID successfully ");
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
        public static void SetCacheDataForStcJobIdFromService(int? StcJobDetailsId, int? JobId = null, SortedList<string, string> data = null)
        {
            try
            {
                Common.Log(DateTime.Now + "start update cache: " + StcJobDetailsId.ToString());
                string serviceUrl = ConfigurationManager.AppSettings["urlCreateCacheForStcJobIdFromRecRegistry"].ToString();
                string UpdateCacheURL = string.Format(serviceUrl, StcJobDetailsId);
                HttpWebRequest requestUpdateCache = (HttpWebRequest)WebRequest.Create(UpdateCacheURL);
                Common.Log(DateTime.Now + "start update cache 1 updatecacheURL: " + UpdateCacheURL);
                //foreach (var item in data)
                //{
                //    Common.Log(DateTime.Now + "start update cache 2 key: " + item.Key +" Value: "+item.Value);
                //    requestUpdateCache.Headers.Add(item.Key, item.Value);
                //}
                requestUpdateCache.AutomaticDecompression = DecompressionMethods.GZip;
                requestUpdateCache.Timeout = 12000000;
                using (HttpWebResponse responseUpdateCache = (HttpWebResponse)requestUpdateCache.GetResponse())
                {
                    Common.Log("cache updated for stcjobId from rec registry: " + StcJobDetailsId + "andresponsestatus:" + responseUpdateCache.StatusCode);
                }


            }
            catch (Exception ex)
            {
                Common.Log(DateTime.Now + " something wrong in cache updating for stcjobId: " + StcJobDetailsId + "exception: " + ex.InnerException.Message.ToString());
            }

        }
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

    public class Result
    {
        public int totalCount { get; set; }
        public ResultDetails[] results { get; set; }
    }

    public class ResultDetails
    {
        public string accreditationCode { get; set; }
        public int bulkUploadId { get; set; }
        public string registrationCode { get; set; }
        public int registrationId { get; set; }
        public string commaSeparatedSerialNumbers { get; set; }
        public DateTime installationDate { get; set; }
        public Ownertype ownerType { get; set; }
        public string ownerName { get; set; }
        public string accreditedInstallerNumber { get; set; }
        public Installationstate installationState { get; set; }
        public DateTime? LastAuditedDate { get; set; }
    }

    public class Installationstate
    {
        public string displayName { get; set; }
        public string name { get; set; }
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

    public class Rootobject
    {
        public string status { get; set; }
        public Result_SearchRegistrationCode result { get; set; }
        public object errors { get; set; }
        public object firstErrorOrDefault { get; set; }
        public object fieldErrors { get; set; }
        public object objectErrors { get; set; }
    }

    public class RootobjectSWH
    {
        public string status { get; set; }
        public Result_SWHJob result { get; set; }
        public object errors { get; set; }
        public object firstErrorOrDefault { get; set; }
        public object fieldErrors { get; set; }
        public object objectErrors { get; set; }
    }

    public class Result_SWHJob
    {
        public Result_SWHJobResult[] results { get; set; }
    }

    public class Result_SWHJobResult
    {
        public string accreditationCode { get; set; }
        public DateTime installationDate { get; set; }
        public string ownerFirstName { get; set; }
        public string ownerSurname { get; set; }
        public InstallationState installationState { get; set; }
        public string systemBrand { get; set; }
        public string systemModel { get; set; }
        public string commaSeparatedSerialNumbers { get; set; }
        public DateTime certificatesCreatedDate { get; set; }
        public int numberOfCertificatesCreated { get; set; }
        public int numberOfRegisteredCertificates { get; set; }
        public Assessment assessment { get; set; }
        public int bulkUploadId { get; set; }
        public string ownerName { get; set; }
    }

    public class InstallationState
    {
        public string displayName { get; set; }
        public string name { get; set; }
    }

    public class Result_SearchRegistrationCode
    {
        public Installationdetails installationDetails { get; set; }
        public Installationaddress installationAddress { get; set; }
        public Owner owner { get; set; }
        public Installer installer { get; set; }
        public Designer designer { get; set; }
        public Electrician electrician { get; set; }
        public DateTime registrationDate { get; set; }
        public DateTime registrationEndDate { get; set; }
        public DateTime sguCreatedOn { get; set; }
        public string accreditationCode { get; set; }
        public DateTime createdOn { get; set; }
        public Fuelsource fuelSource { get; set; }
        public int id { get; set; }
        public int postcodeZone { get; set; }
        public int numberOfCertificates { get; set; }
        public int numberOfRegisteredCertificates { get; set; }
        public int bulkUploadId { get; set; }
        public string registrationCode { get; set; }
        public string reference { get; set; }
        public object recMultiplierRate { get; set; }
        public Questionanswer[] questionAnswers { get; set; }
        public Assessment assessment { get; set; }
        public string performedByUser { get; set; }
        public string accountName { get; set; }
        public bool legacyEligibleForSolarCreditsMultiplier { get; set; }
    }

    public class Installationdetails
    {
        public Sgutype sguType { get; set; }
        public DateTime installDate { get; set; }
        public Deemingperiod deemingPeriod { get; set; }
        public float ratedPowerOutputInKw { get; set; }
        public int panels { get; set; }
        public Systemdetail[] systemDetails { get; set; }
        public string systemBrands { get; set; }
        public string systemModels { get; set; }
        public object availability { get; set; }
        public object siteAudit { get; set; }
        public object defaultAvailability { get; set; }
        public Systemlocationtype systemLocationType { get; set; }
        public Electricitygridconnectivity electricityGridConnectivity { get; set; }
        public string[] panelSerialNumbers { get; set; }
        public string commaSeparatedSerialNumbers { get; set; }
        public bool completeUnitInstalled { get; set; }
        public Inverter[] inverters { get; set; }
        public string inverterManufacturers { get; set; }
        public string inverterSeries { get; set; }
        public string inverterModelNumbers { get; set; }
        public bool creatingCertificatesForSystemHasFailedBefore { get; set; }
        public object accreditationCodeOfPreviouslyFailedSystem { get; set; }
        public object explanatoryNotesForRecreatingCertificates { get; set; }
        public object additionalCapacityDetails { get; set; }
        public object legacyDisconnectionDate { get; set; }
        public object legacyOutOfPocketExpense { get; set; }
        public object legacyTransitionalMultiplierFlag { get; set; }
        public object legacyPreviousRecsMultiplierFlag { get; set; }
        public object legacyRebateApproved { get; set; }
        public bool theSystemFailedPreviously { get; set; }
        public bool completeUnitNotInstalled { get; set; }
        public bool theSystemNotFailedPreviously { get; set; }
        public bool solarDeemed { get; set; }
        public bool windHydroDeemed { get; set; }
    }

    public class Sgutype
    {
        public Sgudeemingperiodsstrategy[] sguDeemingPeriodsStrategies { get; set; }
        public string displayName { get; set; }
        public string name { get; set; }
    }

    public class Sgudeemingperiodsstrategy
    {
        public int[] years { get; set; }
        public Sgudeemingperiod[] sguDeemingPeriods { get; set; }
    }

    public class Sgudeemingperiod
    {
        public string displayName { get; set; }
        public string name { get; set; }
    }

    public class Deemingperiod
    {
        public string displayName { get; set; }
        public string name { get; set; }
    }

    public class Systemlocationtype
    {
        public string displayName { get; set; }
        public string name { get; set; }
    }

    public class Electricitygridconnectivity
    {
        public string displayName { get; set; }
        public string name { get; set; }
    }

    public class Systemdetail
    {
        public string brand { get; set; }
        public string model { get; set; }
    }

    public class Inverter
    {
        public string manufacturer { get; set; }
        public string series { get; set; }
        public string modelNumber { get; set; }
    }

    public class Installationaddress
    {
        public Address address { get; set; }
        public object propertyName { get; set; }
        public Propertysize propertySize { get; set; }
        public Propertytype propertyType { get; set; }
        public Gislocation gisLocation { get; set; }
        public bool moreThanOneSguAtSameAddress { get; set; }
        public object additionalSystemInformation { get; set; }
        public object legacyGisLocation { get; set; }
        public object latitude { get; set; }
        public object longitude { get; set; }
    }

    public class Address
    {
        public object poBoxType { get; set; }
        public object poBoxNumber { get; set; }
        public object unitType { get; set; }
        public object unitNumber { get; set; }
        public string streetNumber { get; set; }
        public string streetName { get; set; }
        public Streettype streetType { get; set; }
        public string suburb { get; set; }
        public State state { get; set; }
        public string postcode { get; set; }
        public Country country { get; set; }
        public bool ignoreValidation { get; set; }
        public bool postalAddress { get; set; }
        public object specialAddress { get; set; }
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string addressLine3 { get; set; }
        public bool locationAddress { get; set; }
        public bool specialAddressMoreThanSevenChar { get; set; }
    }

    public class Streettype
    {
        public string code { get; set; }
        public string label { get; set; }
        public string titleCaseLabel { get; set; }
    }

    public class State
    {
        public string displayName { get; set; }
        public string name { get; set; }
    }

    public class Country
    {
        public string code { get; set; }
        public string name { get; set; }
    }

    public class Propertysize
    {
        public string displayName { get; set; }
        public string name { get; set; }
    }

    public class Propertytype
    {
        public string displayName { get; set; }
        public string name { get; set; }
    }

    public class Gislocation
    {
        public object latitude { get; set; }
        public object longitude { get; set; }
    }

    public class Owner
    {
        public Ownertype ownerType { get; set; }
        public string firstName { get; set; }
        public string surname { get; set; }
        public object organisationName { get; set; }
        public Contact contact { get; set; }
        public object addressType { get; set; }
        public bool individual { get; set; }
        public bool organisation { get; set; }
    }

    public class Ownertype
    {
        public string displayName { get; set; }
        public string name { get; set; }
    }

    public class Contact
    {
        public string phone { get; set; }
        public object mobile { get; set; }
        public object fax { get; set; }
        public object email { get; set; }
        public Address1 address { get; set; }
    }

    public class Address1
    {
        public object poBoxType { get; set; }
        public object poBoxNumber { get; set; }
        public object unitType { get; set; }
        public object unitNumber { get; set; }
        public string streetNumber { get; set; }
        public string streetName { get; set; }
        public Streettype1 streetType { get; set; }
        public string suburb { get; set; }
        public State1 state { get; set; }
        public string postcode { get; set; }
        public Country1 country { get; set; }
        public bool ignoreValidation { get; set; }
        public bool postalAddress { get; set; }
        public object specialAddress { get; set; }
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string addressLine3 { get; set; }
        public bool locationAddress { get; set; }
        public bool specialAddressMoreThanSevenChar { get; set; }
    }

    public class Streettype1
    {
        public string code { get; set; }
        public string label { get; set; }
        public string titleCaseLabel { get; set; }
    }

    public class State1
    {
        public string displayName { get; set; }
        public string name { get; set; }
    }

    public class Country1
    {
        public string code { get; set; }
        public string name { get; set; }
    }

    public class Installer
    {
        public string firstName { get; set; }
        public string surname { get; set; }
        public string accreditedNumber { get; set; }
        public object electricianNumber { get; set; }
        public Contact1 contact { get; set; }
    }

    public class Contact1
    {
        public string phone { get; set; }
        public object mobile { get; set; }
        public object fax { get; set; }
        public string email { get; set; }
        public Address2 address { get; set; }
    }

    public class Address2
    {
        public object poBoxType { get; set; }
        public object poBoxNumber { get; set; }
        public object unitType { get; set; }
        public object unitNumber { get; set; }
        public string streetNumber { get; set; }
        public string streetName { get; set; }
        public Streettype2 streetType { get; set; }
        public string suburb { get; set; }
        public State2 state { get; set; }
        public string postcode { get; set; }
        public Country2 country { get; set; }
        public bool ignoreValidation { get; set; }
        public bool postalAddress { get; set; }
        public object specialAddress { get; set; }
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string addressLine3 { get; set; }
        public bool locationAddress { get; set; }
        public bool specialAddressMoreThanSevenChar { get; set; }
    }

    public class Streettype2
    {
        public string code { get; set; }
        public string label { get; set; }
        public string titleCaseLabel { get; set; }
    }

    public class State2
    {
        public string displayName { get; set; }
        public string name { get; set; }
    }

    public class Country2
    {
        public string code { get; set; }
        public string name { get; set; }
    }

    public class Designer
    {
        public string firstName { get; set; }
        public string surname { get; set; }
        public string accreditedNumber { get; set; }
        public object electricianNumber { get; set; }
        public Contact2 contact { get; set; }
    }

    public class Contact2
    {
        public string phone { get; set; }
        public object mobile { get; set; }
        public object fax { get; set; }
        public string email { get; set; }
        public Address3 address { get; set; }
    }

    public class Address3
    {
        public object poBoxType { get; set; }
        public object poBoxNumber { get; set; }
        public object unitType { get; set; }
        public object unitNumber { get; set; }
        public string streetNumber { get; set; }
        public string streetName { get; set; }
        public Streettype3 streetType { get; set; }
        public string suburb { get; set; }
        public State3 state { get; set; }
        public string postcode { get; set; }
        public Country3 country { get; set; }
        public bool ignoreValidation { get; set; }
        public bool postalAddress { get; set; }
        public object specialAddress { get; set; }
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string addressLine3 { get; set; }
        public bool locationAddress { get; set; }
        public bool specialAddressMoreThanSevenChar { get; set; }
    }

    public class Streettype3
    {
        public string code { get; set; }
        public string label { get; set; }
        public string titleCaseLabel { get; set; }
    }

    public class State3
    {
        public string displayName { get; set; }
        public string name { get; set; }
    }

    public class Country3
    {
        public string code { get; set; }
        public string name { get; set; }
    }

    public class Electrician
    {
        public string firstName { get; set; }
        public string surname { get; set; }
        public object accreditedNumber { get; set; }
        public string electricianNumber { get; set; }
        public Contact3 contact { get; set; }
    }

    public class Contact3
    {
        public string phone { get; set; }
        public object mobile { get; set; }
        public object fax { get; set; }
        public string email { get; set; }
        public Address4 address { get; set; }
    }

    public class Address4
    {
        public object poBoxType { get; set; }
        public object poBoxNumber { get; set; }
        public object unitType { get; set; }
        public object unitNumber { get; set; }
        public string streetNumber { get; set; }
        public string streetName { get; set; }
        public Streettype4 streetType { get; set; }
        public string suburb { get; set; }
        public State4 state { get; set; }
        public string postcode { get; set; }
        public Country4 country { get; set; }
        public bool ignoreValidation { get; set; }
        public bool postalAddress { get; set; }
        public object specialAddress { get; set; }
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string addressLine3 { get; set; }
        public bool locationAddress { get; set; }
        public bool specialAddressMoreThanSevenChar { get; set; }
    }

    public class Streettype4
    {
        public string code { get; set; }
        public string label { get; set; }
        public string titleCaseLabel { get; set; }
    }

    public class State4
    {
        public string displayName { get; set; }
        public string name { get; set; }
    }

    public class Country4
    {
        public string code { get; set; }
        public string name { get; set; }
    }

    public class Fuelsource
    {
        public bool active { get; set; }
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

    public class Questionanswer
    {
        public bool answer { get; set; }
        public string question { get; set; }
    }

}

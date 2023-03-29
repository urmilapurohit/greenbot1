using System;
using FormBot.Entity;
using FormBot.BAL.Service;
using System.Web.Mvc;
using FormBot.Helper;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Data;
using FormBot.Entity.Email;
using FormBot.Helper.Helper;
using FormBot.Main.Models;
using FormBot.Main.Infrastructure;
using System.Collections.Specialized;
using System.Net;
using System.Linq;
using Ionic.Zip;
using OpenQA.Selenium.Chrome;
using System.Threading;
using OpenQA.Selenium;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using HtmlAgilityPack;

namespace FormBot.Main.Controllers
{
    public class HomeController : Controller
    {
        #region Properties
        private readonly IUserBAL _userBAL;
        private readonly IEmailBAL _emailBAL;
        private readonly ISTCInvoiceBAL _stcInvoiceBAL;
        private readonly ICreateJobBAL _createJob;
        private static readonly Logger _logger = new Logger();
        #endregion

        public HomeController(IUserBAL _userBAL,
            IEmailBAL _emailBAL, ISTCInvoiceBAL _stcInvoiceBAL, ICreateJobBAL createJob)
        {
            this._userBAL = _userBAL;
            this._emailBAL = _emailBAL;
            this._stcInvoiceBAL = _stcInvoiceBAL;
            this._createJob = createJob;
        }

        public ActionResult Index()
        {
            //NewRECLogin();
            //CombinedLogin();
            GetAllUsers();
            return View();
        }

        public void CombinedLogin()           
        {
            ChromeDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://www.rec-registry.gov.au/rec-registry/app/auth");

            Thread.Sleep(2000);
            IWebElement ele = driver.FindElement(By.Id("signInName"));
            IWebElement ele2 = driver.FindElement(By.Id("password"));
            ele.SendKeys("hus@emergingenergy.com.au");
            ele2.SendKeys("Gow4ybk2!!");
            Thread.Sleep(2000);
            IWebElement ele1 = driver.FindElement(By.Id("next"));
            ele1.Click();
            Thread.Sleep(3000);

            ReadOnlyCollection<OpenQA.Selenium.Cookie> cookie = driver.Manage().Cookies.AllCookies;

            ReadOnlyCollection<IWebElement> lstEle = driver.FindElements(By.ClassName("btn-primary"));
            IWebElement eleAccount = lstEle.Where(a => a.GetAttribute("onclick") == "submitUser('LAMH54891')").FirstOrDefault();

            eleAccount.Click();

            Match m = Regex.Match(driver.PageSource, "name=\"_csrf\" content=\"(.*)\">");
            string CSRFToken = string.Empty;
            if (m.Success)
            {
                CSRFToken = m.Groups[1].Value;
            }

        }

        

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult ThankYou()
        {
            return View();
        }

        public ActionResult SendEmailToAndroidAppUsers()
        {
            try
            {
                DataTable dtUsers = _userBAL.GetAllAndroidMobileAppUsers();
                if(dtUsers != null)
                {
                    if(dtUsers.Rows.Count > 0)
                    {
                        foreach(DataRow dr  in dtUsers.Rows)
                        {
                            if(dr["Email"] != DBNull.Value)
                            {
                                _emailBAL.ComposeAndSendEmailForAndroidAppUsers(new EmailInfo() { TemplateID = 46}, dr["Email"].ToString());
                            }
                        }
                    }
                }
                return Content("Completed");
            }
            catch (Exception ex)
            {
                return Content(JsonConvert.SerializeObject(ex));
            }
        }
        ///// <summary>
        ///// This action is use for move job document folder to another location.
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //public ActionResult MoveJobDocFolderForAudit()
        //{
        //    List<string> lstMovedFolder = new List<string>();
        //    string ResponceMessage = "";
        //    try
        //    {
        //        string jobIdsFromConfig = System.Configuration.ConfigurationManager.AppSettings["JobIds"].ToString();
        //        string sourceDirPath = ProjectSession.ProofDocumentsURL + "JobDocuments/";
        //        string destinationDirPath = ProjectSession.ProofDocumentsURL + System.Configuration.ConfigurationManager.AppSettings["AuditFolderName"].ToString() + "/";

        //        string[] jobids = jobIdsFromConfig.Split(',');
        //        foreach (var jobid in jobids)
        //        {
        //            sourceDirPath += jobid;
        //            destinationDirPath += jobid;
        //            DirectoryInfo dir = new DirectoryInfo(sourceDirPath);
        //            dir.MoveTo(destinationDirPath);
        //            lstMovedFolder.Add(jobid);
        //        }
        //        ResponceMessage =$"Completed.";
        //    }
        //    catch (Exception ex)
        //    {
        //        ResponceMessage = $"Exception = {JsonConvert.SerializeObject(ex)}";
        //    }
        //    return Content($"{ResponceMessage}  {Environment.NewLine}Moved JobIds = {string.Join(",", lstMovedFolder)}");
        //}

        public void GetAllUsers()
        {
            DataSet ds = _userBAL.GetAllUsersWithABN();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                string entityName = GetEntityName(Convert.ToString(ds.Tables[0].Rows[i]["CompanyABN"]));
                _userBAL.UpdateEntityName(Convert.ToInt32(ds.Tables[0].Rows[i]["userid"]), entityName);
            }
            bool isDone = true;
        }

        public string GetEntityName(string companyABN)
        {
            string entityName = string.Empty;
            string abnURL = ProjectConfiguration.GetCompanyABNSearchLink + companyABN;
            try
            {
                HttpWebRequest wreq = (HttpWebRequest)WebRequest.Create(abnURL);
                wreq.Method = "GET";
                wreq.Timeout = -1;
                wreq.ContentType = "application/json; charset=UTF-8";
                var myHttpWebResponse = (HttpWebResponse)wreq.GetResponse();
                string strResult;
                using (var reader = new StreamReader(myHttpWebResponse.GetResponseStream(), System.Text.Encoding.UTF8))
                {
                    strResult = reader.ReadToEnd();
                    myHttpWebResponse.Close();
                }

                if (strResult != null)
                {
                    strResult = WebUtility.HtmlDecode(strResult);
                    HtmlAgilityPack.HtmlDocument resultat = new HtmlAgilityPack.HtmlDocument();
                    resultat.LoadHtml(strResult);

                    HtmlNode table = resultat.DocumentNode.SelectSingleNode("//table[1]");
                    if (table != null)
                    {
                        foreach (var cell in table.SelectNodes(".//tr/th"))
                        {
                            string someVariable = cell.InnerText;
                            if (cell.InnerText.ToLower() == "entity name:")
                            {
                                var td = cell.ParentNode.SelectNodes("./td");
                                string tdValue = td[0].InnerText;
                                entityName = tdValue.Replace("\r\n", "").Trim();
                                if (entityName.Length > 0)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(SystemEnums.Severity.Error, "companyABN = " + companyABN + " " + ex.Message.ToString());
            }
            return entityName;
        }

    }
}
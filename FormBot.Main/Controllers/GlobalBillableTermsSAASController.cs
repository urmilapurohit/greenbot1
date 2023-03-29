using FormBot.BAL.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FormBot.Entity;
using FormBot.Helper;
using FormBot.BAL.Service.GlobalBillableTermsSAAS;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using System.Text;

namespace FormBot.Main.Controllers
{
    public class GlobalBillableTermsSAASController : Controller
    {
        #region Properties
        private readonly GlobalBillableTermsSAAS _GlobalBillableTermsSAASBAL = new GlobalBillableTermsSAAS();
        #endregion

        #region Constructor
        //public GlobalBillableTermsSAASController(IGlobalBillableTermsSAAS GlobalBillableTerms)
        //{
        //    this._GlobalBillableTermsSAASBAL = GlobalBillableTerms;
        //}
        #endregion

        // GET: GlobalBillableTermsSAAS
        [UserAuthorization]
        public ActionResult Index()
        {
            FormBot.Entity.GlobalBillableTerms GlobalBillableTerms = new Entity.GlobalBillableTerms();
            GlobalBillableTerms.UserTypeID = ProjectSession.UserTypeId;

            return View("Index", GlobalBillableTerms);
        }

        [HttpPost]
        public JsonResult SaveGlobalBillableTermSAAS(GlobalBillableTerms GlobalBillableTerms)
        {
            string HistoryMessage = string.Empty;
            string Category = string.Empty;
            //string OldBillerCode = GlobalBillableTerms.OldBillerCode;
            decimal OldPrice = GlobalBillableTerms.OldGlobalPrice;
            IList<FormBot.Entity.GlobalBillableTerms> lstGlobalPrice = null;
            try
            {
                lstGlobalPrice = _GlobalBillableTermsSAASBAL.SaveGlobalBillableTermSAAS(GlobalBillableTerms);

                if (lstGlobalPrice[0].OutPutValue != 3 && lstGlobalPrice[0].OutPutValue != 4)
                {
                    if (GlobalBillableTerms.Id == 0)
                    {
                        HistoryMessage = "has added a new billable term (" + lstGlobalPrice[0].BillerCode + ") " + lstGlobalPrice[0].TermName + " having term code " + lstGlobalPrice[0].TermCode + " at global price " + lstGlobalPrice[0].GlobalPrice + "";
                        Category = "New Billable Term";
                    }
                    else
                    {
                        HistoryMessage = "has edited price from " + OldPrice + " to " + lstGlobalPrice[0].GlobalPrice + " having term code " + lstGlobalPrice[0].TermCode + " with biller code (" + lstGlobalPrice[0].BillerCode + ")";
                        Category = "Edit Billable Term";
                    }
                    SaveGlobalTermsHistorytoXML(lstGlobalPrice[0].BillerCode, HistoryMessage, "BillableTerm", Category, ProjectSession.LoggedInName);
                }
            }
            catch (Exception ex)
            {
                return this.Json(new { success = false, errormessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            return this.Json(new { success = true, OutPutValue = lstGlobalPrice[0].OutPutValue }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get SAAS global pricing list.
        /// </summary>
        /// <param name="TermName"></param>
        /// <param name="BillerCode"></param>
        /// <param name="TermDescription"></param>
        /// <param name="TermCode"></param>
        public void GetGlobalPricingList(string TermName, string BillerCode, string TermDescription, string TermCode)
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);

            IList<FormBot.Entity.GlobalBillableTerms> lstGlobalPrice = _GlobalBillableTermsSAASBAL.GetGlobalPricingList(TermName, BillerCode, TermDescription, TermCode, pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir);

            if (lstGlobalPrice.Count > 0)
            {
                gridParam.TotalDisplayRecords = lstGlobalPrice.FirstOrDefault().TotalRecords;
                gridParam.TotalRecords = lstGlobalPrice.FirstOrDefault().TotalRecords;
            }

            HttpContext.Response.Write(Grid.PrepareDataSet(lstGlobalPrice, gridParam));
        }

        /// <summary>
        /// Show the billing term history.
        /// </summary>
        /// <returns>partial view</returns>
        [HttpGet]
        public PartialViewResult ShowBillingTermHistory(string BillerCode)
        {
            return this.PartialView("~/Views/GlobalBillableTermsSAAS/_BillingTermHistory.cshtml", GetGlobalTermsHistory(BillerCode));
        }

        public static void SaveGlobalTermsHistorytoXML(string BillerCode, string BillableTermHistoryMessage, string Filter, string Category, string CreatedBy, string description = null, string NoteID = null, string HistoryType = "Public", string CreatedDate = null)
        {
            try
            {
                var BillableTermHistoryXMLPath = Path.Combine(Path.Combine(ProjectConfiguration.ProofDocumentsURL, "StaticTemplate/SPV/BillableTerm.xml"));
                string fullDirectoryPath = Path.Combine(ProjectConfiguration.ProofDocumentsURL, "BillableTermDocuments", "BillableTermHistory");
                if (!Directory.Exists(fullDirectoryPath))
                    Directory.CreateDirectory(fullDirectoryPath);
                string fullFilePath = Path.Combine(fullDirectoryPath, "BillableTermHistory_" + BillerCode.ToString() + ".xml");

                if (!string.IsNullOrEmpty(description))
                {
                    if (description.ToLower().Contains(" from ") && description.ToLower().Contains(" to  "))
                    {

                        int indexFrom = description.ToLower().LastIndexOf(" from ");
                        int startIndexTo = description.ToLower().IndexOf(" to ");
                        int LastindexTO = description.ToLower().LastIndexOf(" to ");
                        string substrFrom = description.Substring(indexFrom + 5, startIndexTo - (indexFrom + 5));
                        string substrTo = description.Substring(LastindexTO + 3);
                        description = description.Replace(substrFrom, "<b style=\"color: black\">" + substrFrom + "</b>");

                        description = description.Replace(substrTo, "<b style=\"color: black\">" + substrTo + "</b>");

                    }
                    else if (description.ToLower().Contains(" to "))
                    {
                        int LastindexTO = description.ToLower().LastIndexOf(" to ");
                        string substrTo = description.Substring(LastindexTO + 3);
                        description = description.Replace(substrTo, "<b style=\"color: black\">" + substrTo + "</b>");
                    }
                }

                if (!string.IsNullOrEmpty(BillableTermHistoryMessage))
                {
                    if (BillableTermHistoryMessage.ToLower().Contains(" from ") && BillableTermHistoryMessage.ToLower().Contains(" to "))
                    {
                        int indexFrom = BillableTermHistoryMessage.ToLower().LastIndexOf(" from ");
                        int startIndexTo = BillableTermHistoryMessage.ToLower().IndexOf(" to ");
                        int LastindexTO = BillableTermHistoryMessage.ToLower().LastIndexOf(" to ");
                        string substrFrom = BillableTermHistoryMessage.Substring(indexFrom + 5, startIndexTo - (indexFrom + 5));
                        string substrTo = BillableTermHistoryMessage.Substring(LastindexTO + 3, startIndexTo - (indexFrom + 5));
                        BillableTermHistoryMessage = BillableTermHistoryMessage.Replace(substrFrom, "<b style=\"color: black\">" + substrFrom + "</b>");
                        BillableTermHistoryMessage = BillableTermHistoryMessage.Replace(substrTo, "<b style=\"color: black\">" + substrTo + "</b>");
                    }
                    else if (BillableTermHistoryMessage.ToLower().Contains(" to "))
                    {
                        int LastindexTO = BillableTermHistoryMessage.ToLower().LastIndexOf(" to ");
                        string substrTo = BillableTermHistoryMessage.Substring(LastindexTO + 3);
                        BillableTermHistoryMessage = BillableTermHistoryMessage.Replace(substrTo, "<b style=\"color: black\">" + substrTo + "</b>");
                    }
                }

                string Date = "";
                if (string.IsNullOrEmpty(CreatedDate))
                {
                    Date = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");
                }
                else
                {
                    Date = CreatedDate;
                }
                if (System.IO.File.Exists(fullFilePath))
                {
                    XDocument olddoc = new XDocument();
                    using (XmlReader xml = XmlReader.Create(fullFilePath))
                    {
                        olddoc = XDocument.Load(xml);
                        XElement root = new XElement("History");

                        root.Add(new XElement("BillerCode", Convert.ToString(BillerCode)));
                        root.Add(new XElement("BillableTermHistoryMessage", BillableTermHistoryMessage));
                        root.Add(new XElement("Filter", Filter));
                        root.Add(new XElement("Category", Category));
                        root.Add(new XElement("Description", description));
                        root.Add(new XElement("CreatedBy", CreatedBy));
                        root.Add(new XElement("CreatedDate", Date));
                        root.Add(new XElement("HistoryType", HistoryType));
                        root.Add(new XElement("NoteID", NoteID));
                        olddoc.Element("GlobalBillableTermHistory").Add(root);
                        xml.Dispose();
                        xml.Close();
                        olddoc.Save(fullFilePath);

                    }
                }
                else
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.PreserveWhitespace = true;
                    xmlDoc.Load(BillableTermHistoryXMLPath);
                    XDocument doc = XDocument.Parse(xmlDoc.InnerXml);
                    xmlDoc.InnerXml = doc.ToString();
                    xmlDoc.InnerXml = xmlDoc.InnerXml.Replace("[[BillerCode]]", BillerCode.ToString())
                                   .Replace("[[BillableTermHistoryMessage]]", HttpUtility.HtmlEncode(BillableTermHistoryMessage))
                                   .Replace("[[Filter]]", Filter)
                                   .Replace("[[Category]]", Category)
                                   .Replace("[[Description]]", HttpUtility.HtmlEncode(description))
                                    .Replace("[[CreatedBy]]", CreatedBy)
                                    .Replace("[[CreatedDate]]", Date)
                                    .Replace("[[HistoryType]]", HistoryType)
                                    .Replace("[[NoteID]]", NoteID);

                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Encoding = new UTF8Encoding(false);
                    settings.Indent = true;
                    using (XmlWriter writer = XmlWriter.Create(fullFilePath, settings))
                    {
                        xmlDoc.Save(writer);
                        writer.Close();
                        writer.Dispose();
                    }
                }
            }
            catch (Exception e)
            {
                FormBot.Helper.Log.WriteError(e, "Exception in SaveGlobalTermsHistorytoXML.." + DateTime.Now.ToString());
                Console.WriteLine(e.Message);
            }
        }

        public List<GlobalBillableTerms> GetGlobalTermsHistory(string BillerCode)
        {
            List<GlobalBillableTerms> billabletermHistory = new List<GlobalBillableTerms>();

            #region Add History from xml
            string BillableTermHistoryDirectoryPath = Path.Combine(ProjectConfiguration.ProofDocumentsURL, "BillableTermDocuments", "BillableTermHistory");
            string BillableTermHistoryFilePath = Path.Combine(BillableTermHistoryDirectoryPath, "BillableTermHistory_" + BillerCode.ToString() + ".xml");

            if (System.IO.File.Exists(BillableTermHistoryFilePath))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(BillableTermHistoryFilePath);
                XmlNodeList History = doc.DocumentElement.SelectNodes("/GlobalBillableTermHistory/History");

                foreach (XmlNode node in History)
                {
                    string Category = node.SelectSingleNode("Category").InnerText;
                    string Filter = node.SelectSingleNode("Filter").InnerText;
                    string Historytype = node.SelectSingleNode("HistoryType").InnerText;
                    int HistoryTypeValue = !string.IsNullOrEmpty(Historytype) ? Convert.ToInt32((SystemEnums.NotesType)Enum.Parse(typeof(SystemEnums.NotesType), Historytype).GetHashCode()) : 0;
                    GlobalBillableTerms objbillabletermhistory = new GlobalBillableTerms();
                    objbillabletermhistory.HistoryCategory = Category;
                    string HistoryMessage = node.SelectSingleNode("BillableTermHistoryMessage").InnerText;
                    string Description = "<p>" + node.SelectSingleNode("Description").InnerText + "</p>";
                    string BillableTermHistoryMessage = HistoryMessage + Description;
                    objbillabletermhistory.HistoryMessage = BillableTermHistoryMessage;
                    objbillabletermhistory.CreateDate = Convert.ToDateTime(node.SelectSingleNode("CreatedDate").InnerText).ToString("dd/MM/yyyy hh:mmtt");
                    objbillabletermhistory.ModifiedDate = Convert.ToDateTime(node.SelectSingleNode("CreatedDate").InnerText);
                    objbillabletermhistory.Modifier = node.SelectSingleNode("CreatedBy").InnerText;
                    objbillabletermhistory.NoteId = 0;

                    billabletermHistory.Add(objbillabletermhistory);
                }
            }
            #endregion
            return billabletermHistory.OrderByDescending(x => x.CreateDate).ToList();
        }


        [HttpPost]
        public JsonResult DeleteBillableTermByID(GlobalBillableTerms GlobalBillableTerms)
        {
            string HistoryMessage = string.Empty;
            string Category = string.Empty;
            IList<FormBot.Entity.GlobalBillableTerms> lstGlobalBillableTerms = null;
            try
            {
                lstGlobalBillableTerms = _GlobalBillableTermsSAASBAL.DeleteBillableTermByID(Convert.ToInt32(GlobalBillableTerms.Id));

                HistoryMessage = "has deleted billing term having term code " + lstGlobalBillableTerms[0].TermCode + " and biller code (" + lstGlobalBillableTerms[0].BillerCode + ")";
                Category = "Delete Billable Term";
                SaveGlobalTermsHistorytoXML(GlobalBillableTerms.BillerCode, HistoryMessage, "BillableTerm", Category, ProjectSession.LoggedInName);
            }
            catch (Exception ex)
            {
                return this.Json(new { success = false, errormessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }

            return this.Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RestoreBillableTerm(GlobalBillableTerms GlobalBillableTerms)
        {
            string HistoryMessage = string.Empty;
            string Category = string.Empty;
            IList<FormBot.Entity.GlobalBillableTerms> lstGlobalBillableTerms = null;
            try
            {
                lstGlobalBillableTerms = _GlobalBillableTermsSAASBAL.RetoreBillingTermByID(Convert.ToInt32(GlobalBillableTerms.Id));

                HistoryMessage = "has retrieved billing term having term code " + lstGlobalBillableTerms[0].TermCode + " and biller code (" + lstGlobalBillableTerms[0].BillerCode + ")";
                Category = "Retrieve Billable Term";
                SaveGlobalTermsHistorytoXML(GlobalBillableTerms.BillerCode, HistoryMessage, "BillableTerm", Category, ProjectSession.LoggedInName);
            }
            catch (Exception ex)
            {
                return this.Json(new { success = false, errormessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            return this.Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

    }
}
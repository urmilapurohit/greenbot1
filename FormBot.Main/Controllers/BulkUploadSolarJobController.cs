using Excel;
using FormBot.BAL.Service;
using FormBot.BAL.Service.CommonRules;
using FormBot.Entity;
using FormBot.Entity.Job;
using FormBot.Helper;
using FormBot.Helper.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;

namespace FormBot.Main.Controllers
{
    public class BulkUploadSolarJobController : Controller
    {
        #region Properties       
        private readonly ICreateJobBAL _job;
        private readonly IJobRulesBAL _jobRules;
        private readonly UserController _userController;
        private readonly ICreateJobHistoryBAL _jobHistory;
        #endregion

        #region Constructor
        public BulkUploadSolarJobController(ICreateJobBAL job, IJobRulesBAL jobRules, UserController userController, ICreateJobHistoryBAL jobHistory)
        {
            this._job = job;
            this._jobRules = jobRules;
            this._userController = userController;
            this._jobHistory = jobHistory;
        }
        #endregion


        // GET: BulkUploadSolarJob        
        [UserAuthorization]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Download Template For BulkUploadSolarJobs
        /// </summary>
        /// <param name="JobType"></param>
        /// <returns></returns>
        public FileResult DownloadTemplateForBulkUploadSolarJobs(int JobType)
        {
            string documentName = JobType == 1 ? "TemplateForBulkUploadPVDSolarJobs.xlsx" : "TemplateForBulkUploadSWHSolarJobs.xlsx";
            string path = ProjectConfiguration.TemplateForBulkUploadSolarJobsPath + documentName;
            string documentFullPath = Path.Combine(ProjectSession.ProofDocuments, path);
            if (System.IO.File.Exists(documentFullPath))
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(documentFullPath);
                string fileName = path.Split('\\').Last();
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "PVDBulkUploadSample.xlsx");
            }
            else
                Response.Write("File Doesn't exists!");
            return null;
        }

        /// <summary>
        /// Bulk Upload Solar Jobs
        /// </summary>
        /// <param name="solarCompanyId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> BulkUploadSolarJobs(int solarCompanyId)
        {
            string filePath = string.Empty;

            if (ModelState.IsValid)
            {
                HttpPostedFileBase fileUpload = Request.Files[0];
                if (fileUpload != null && fileUpload.ContentLength > 0)
                {
                    string filename = Path.GetFileName(fileUpload.FileName);
                    string dirPath = Path.Combine(ProjectSession.ProofDocumentsURL + ProjectConfiguration.JobDocumentsToSavePath + "\\BulkUploadSolarJobs\\" + solarCompanyId + "\\");
                    filePath = Path.Combine(dirPath + Path.GetFileNameWithoutExtension(filename) + "_" + DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss") + Path.GetExtension(filename));

                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }
                    fileUpload.SaveAs(filePath);

                    //DataTable dtSpreadsheetSolarJobs = new DataTable();
                    //dtSpreadsheetSolarJobs = GetDataTableFromSpreadsheetSolarJobs(filePath, solarCompanyId);

                    //DataTable dtResult = new DataTable();
                    //if (!dtSpreadsheetSolarJobs.Columns.Contains("ErrorMsg"))
                    //{
                    //    dtResult = _job.Job_InsertBulkUploadSolarJobs(dtSpreadsheetSolarJobs, solarCompanyId);
                    //    return Json(new { status = dtResult.Rows.Count > 0 ? false : true, dtResult = true, msg = Newtonsoft.Json.JsonConvert.SerializeObject(dtResult) }, JsonRequestBehavior.AllowGet);
                    //}
                    //else
                    //{
                    //    return Json(new { status = false, dtSpreadsheetSolarJobs = true, msg = Newtonsoft.Json.JsonConvert.SerializeObject(dtSpreadsheetSolarJobs) }, JsonRequestBehavior.AllowGet);
                    //}

                    //DataTable dtResult = new DataTable();
                    //dtResult = GetDataTableFromSpreadsheetSolarJobs(filePath, solarCompanyId);

                    DataSet dsResult = new DataSet();
                    dsResult = GetDataTableFromSpreadsheetSolarJobs(filePath, solarCompanyId);

                    if (dsResult.Tables[0].Rows.Count > 0)
                    {
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                    else
                    {
                        foreach (DataRow row in dsResult.Tables[1].Rows)
                        {
                            int jid = Convert.ToInt32(row["CacheJobId"]);
                            await CommonBAL.SetCacheDataForJobID(ProjectSession.SolarCompanyId, jid);
                        }
                    }
                    return Json(new { status = dsResult.Tables[0].Rows.Count > 0 ? false : true, msg = Newtonsoft.Json.JsonConvert.SerializeObject(dsResult.Tables[0]) }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Get DataTable From Spreadsheet of SolarJobs
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public DataSet GetDataTableFromSpreadsheetSolarJobs(string filePath, int solarCompanyId)
        {
            int colIndex = 1;
            DataTable dataTable = new DataTable();
            DataSet errorDataTable = new DataSet();
            try
            {
                // OleDb not supporting on server
                //string conString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";" + "Extended Properties=" + "\"" + "Excel 12.0;HDR=YES;" + "\"";
                //OleDbConnection oleCon = new OleDbConnection(conString);
                //OleDbCommand oleCmd = new OleDbCommand("SELECT * FROM [Sheet1$]", oleCon);
                //DataTable dtExcel = new DataTable();

                //oleCon.Open();
                //dtExcel.Load(oleCmd.ExecuteReader());
                //oleCon.Close();

                FileStream stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                //if (!excelReader.IsValid)
                //{
                //    stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read);
                //    excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                //}

                excelReader.IsFirstRowAsColumnNames = true;
                DataSet result = excelReader.AsDataSet();
                DataTable dtExcel = new DataTable();

                for (int i = 0; i < result.Tables.Count; i++)
                {
                    if (result.Tables[i].Rows[0].ItemArray[0].ToString().ToLower() == "pvd")
                    {
                        dtExcel = result.Tables[i];
                        break;
                    }
                }

                var rowCount = dtExcel.AsEnumerable().Where(myRow => myRow.Field<string>("Job Type") != null).Count();

                if (rowCount >= 51)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("ErrorMsg", typeof(string));
                    DataRow dr = dt.NewRow();
                    dr["ErrorMsg"] = "Please upload file with less than 50 records.";
                    dt.Rows.Add(dr);
                    errorDataTable.Tables.Add(dt);
                    return errorDataTable;
                }
                else
                {
                    //dtExcel = result.Tables["Template"];
                    if (dtExcel.Rows.Count > 0)
                    {
                        var jobType = 0;
                        string STCUrl = string.Empty;

                        if (dtExcel.Rows[0].ItemArray[0].ToString().ToLower() == "pvd")
                        {
                            dataTable = DtBulkUploadPVDSolarJobs();
                            jobType = 1;
                            //STCUrl = ProjectSession.CalculateSTCUrl;
                        }
                        else
                        {
                            dataTable = DtBulkUploadSWHSolarJobs();
                            jobType = 2;
                            //STCUrl = ProjectSession.CalculateSWHSTCUrl;
                        }

                        if (jobType == 1)
                        {
                            dtExcel.Columns.Add("STCDeemingPeriod", typeof(string)).SetOrdinal(53);
                            dtExcel.Columns.Add("STCInstallingCompleteUnit", typeof(string)).SetOrdinal(54);
                        }
                        dtExcel.Columns.Add("STCStandAloneGridSelected", typeof(string)).SetOrdinal(jobType == 1 ? 55 : 41);
                        dtExcel.Columns.Add("STCCECAccreditationStatement", typeof(string)).SetOrdinal(jobType == 1 ? 56 : 42);
                        dtExcel.Columns.Add("STCGovernmentSitingApproval", typeof(string)).SetOrdinal(jobType == 1 ? 57 : 43);
                        dtExcel.Columns.Add("STCElectricalSafetyDocumentation", typeof(string)).SetOrdinal(jobType == 1 ? 58 : 44);
                        dtExcel.Columns.Add("STCAustralianNewZealandStandardStatement", typeof(string)).SetOrdinal(jobType == 1 ? 59 : 45);

                        //dtExcel.Columns.Add("CalculatedSTC", typeof(string)).SetOrdinal(62);

                        if (dtExcel != null && dtExcel.Rows.Count > 0)
                        {
                            foreach (DataRow excelRow in dtExcel.Rows)
                            {
                                if (!string.IsNullOrEmpty(excelRow[0].ToString()))
                                {
                                    DataRow dataRow = dataTable.NewRow();
                                    colIndex = 1;
                                    for (int i = 0; i < dataRow.ItemArray.Length; i++)
                                    //for (int i = 0; i < 72; i++)
                                    {
                                        if (excelRow[i].ToString().ToLower() == "pvd" || excelRow[i].ToString().ToLower() == "swh")
                                        {
                                            dataRow[i] = (int)Enum.Parse(typeof(SystemEnums.JobType), excelRow[i].ToString());
                                        }
                                        else if (excelRow[i].ToString().ToLower() == "physical address" || excelRow[i].ToString().ToLower() == "p.o.box")
                                        {
                                            dataRow[i] = excelRow[i].ToString().ToLower() == "physical address" ? Convert.ToBoolean(false) : Convert.ToBoolean(true);
                                        }
                                        else
                                        {
                                            //dataRow[i] = !string.IsNullOrEmpty(excelRow[i].ToString())?excelRow[i].ToString():null;
                                            if (string.IsNullOrEmpty(excelRow[i].ToString()))
                                            {
                                                dataRow[i] = DBNull.Value;
                                            }
                                            else
                                            {
                                                dataRow[i] = excelRow[i].ToString().TrimEnd();
                                            }
                                        }
                                        colIndex++;
                                    }

                                    //SqlConnection conn = new SqlConnection(Convert.ToString(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString));
                                    //SqlCommand sqlCmd = new SqlCommand("Select [dbo].[GetDeemingPeriodYear](" + DateTime.Now.Year + ")", conn);
                                    //conn.Open();
                                    //int deemingPeriod = (int)sqlCmd.ExecuteScalar();
                                    //conn.Close();

                                    JobSTCDetails objJobSTCDetails = new JobSTCDetails();
                                    if (jobType == 1)
                                    {
                                        //dataRow["STCDeemingPeriod"] = _job.GetYearInWords(DateTime.Parse(excelRow[3].ToString()).Year);
                                        dataRow["STCDeemingPeriod"] = !string.IsNullOrEmpty(excelRow[3].ToString()) ? _job.GetYearInWords(DateTime.Parse(excelRow[3].ToString()).Year) : null;
                                        objJobSTCDetails.AdditionalCapacityNotes = excelRow[46].ToString();
                                        objJobSTCDetails.TypeOfConnection = excelRow[47].ToString();
                                        objJobSTCDetails.MultipleSGUAddress = excelRow[49].ToString();
                                    }
                                    objJobSTCDetails = _job.JobSTCDetailsBusinessRules(excelRow["Multiple SGU Address"].ToString(), objJobSTCDetails);

                                    //dataRow["STCDeemingPeriod"] = _job.GetYearInWords(DateTime.Now.Year);

                                    //JobSTCDetails objJobSTCDetails = new JobSTCDetails();
                                    //objJobSTCDetails.AdditionalCapacityNotes = excelRow[46].ToString();
                                    //objJobSTCDetails.TypeOfConnection = excelRow[47].ToString();
                                    //objJobSTCDetails.MultipleSGUAddress = excelRow[49].ToString();
                                    //objJobSTCDetails = _job.JobSTCDetailsBusinessRules(excelRow[49].ToString(), objJobSTCDetails);

                                    //dataRow[51] = "DeemingPeriod";
                                    //dataRow[46] = objJobSTCDetails.AdditionalCapacityNotes;
                                    //dataRow[53] = objJobSTCDetails.InstallingCompleteUnit;
                                    //dataRow[54] = objJobSTCDetails.StandAloneGridSelected;
                                    //dataRow[55] = objJobSTCDetails.CECAccreditationStatement;
                                    //dataRow[56] = objJobSTCDetails.GovernmentSitingApproval;
                                    //dataRow[57] = objJobSTCDetails.ElectricalSafetyDocumentation;
                                    //dataRow[58] = objJobSTCDetails.AustralianNewZealandStandardStatement;

                                    if (jobType == 1)
                                    {
                                        dataRow["STCAdditionalCapacityNotes"] = objJobSTCDetails.AdditionalCapacityNotes;
                                        dataRow["STCInstallingCompleteUnit"] = objJobSTCDetails.InstallingCompleteUnit;
                                    }
                                    dataRow["STCStandAloneGridSelected"] = objJobSTCDetails.StandAloneGridSelected;
                                    dataRow["STCCECAccreditationStatement"] = objJobSTCDetails.CECAccreditationStatement;
                                    dataRow["STCGovernmentSitingApproval"] = objJobSTCDetails.GovernmentSitingApproval;
                                    dataRow["STCElectricalSafetyDocumentation"] = objJobSTCDetails.ElectricalSafetyDocumentation;
                                    dataRow["STCAustralianNewZealandStandardStatement"] = objJobSTCDetails.AustralianNewZealandStandardStatement;

                                    //if (!string.IsNullOrEmpty(dataRow["InstallationDate"].ToString()) && !string.IsNullOrEmpty(dataRow["STCDeemingPeriod"].ToString()) && !string.IsNullOrEmpty(dataRow["InsPostCode"].ToString()) && !string.IsNullOrEmpty(dataRow["SystemSize"].ToString()))
                                    //    dataRow["CalculatedSTC"] = _jobRules.GetSTCValue(jobType, dataRow["InstallationDate"].ToString(), dataRow["STCDeemingPeriod"].ToString(), dataRow["InsPostCode"].ToString(), Convert.ToDecimal(dataRow["SystemSize"]), null, null, STCUrl);


                                    dataTable.Rows.Add(dataRow);
                                }
                            }
                        }

                        errorDataTable = ErrorDatatable(dataTable);
                        DataSet dsResult = new DataSet();
                        if (errorDataTable.Tables[0].Rows.Count == 0)
                        {
                            // Get Job History template for auto schedule installer
                            HistoryTemplate historyTemplate = new HistoryTemplate(HistoryCategory.CreateSchedule);
                            string JobScheduleHistoryMsg = historyTemplate.HTMLTemplate;

                            dsResult = jobType == 1 ? _job.Job_InsertBulkUploadSolarJobs(dataTable, null, solarCompanyId, JobScheduleHistoryMsg) : _job.Job_InsertBulkUploadSolarJobs(null, dataTable, solarCompanyId, JobScheduleHistoryMsg);
                            //if(dsResult.Tables[2].Rows.Count > 0)
                            //{
                            //    int JobID;
                            //    int AssignTo;
                            //    string HistoryMessage = "";
                            //    string assignname = "";
                            //    string JobHistoryMessage = "";
                            //    foreach (DataRow drResult in dsResult.Tables[2].Rows)
                            //    {
                            //        JobID = Convert.ToInt32(drResult["JobID"]);
                            //        AssignTo = Convert.ToInt32(drResult["AssignTo"]);
                            //        HistoryMessage = drResult["HistoryMessage"].ToString();
                            //        assignname = _job.GetUsernameByUserID(AssignTo);
                            //        JobHistoryMessage = HistoryMessage.Replace("#UserName", assignname);
                            //       Common.SaveJobHistorytoXML(JobID, JobHistoryMessage, "Scheduling", "CreateSchedule", ProjectSession.LoggedInName, false);
                            //    }
                                
                            //}
                            // Maintain panel history 
                            if (dsResult.Tables[0].Rows.Count == 0)
                            {
                                for (int k = 0; k < dataTable.Rows.Count; k++)
                                {
                                    List<string> panelBrand = dataTable.Rows[k]["PanelBrand"].ToString().Split(',').ToList();
                                    List<string> panelModel = dataTable.Rows[k]["PanelModel"].ToString().Split(',').ToList();
                                    List<string> noOfPanel = dataTable.Rows[k]["NoOfPanel"].ToString().Split(',').ToList();
                                    for (int l = 0; l < panelBrand.Count; l++)
                                    {
                                        PanelCompare objPanelCompare = new PanelCompare();
                                        objPanelCompare.Brand = panelBrand[l];
                                        objPanelCompare.Model = panelModel[l];
                                        objPanelCompare.Count = noOfPanel[l];
                                        objPanelCompare.JobID = Convert.ToInt32(dsResult.Tables[1].Rows[k]["CacheJobId"]);
                                        //_jobHistory.LogJobHistory<PanelCompare>(objPanelCompare, HistoryCategory.PanelAdded);
                                        string JobHistoryMessage = "has added Panel <b style =\"color: black\"> {Brand : " + objPanelCompare.Brand+" , Model : "+objPanelCompare.Model+" , No. of Panel : "+objPanelCompare.Count+ "} " + "</b>"+"for Job";
                                      Common.SaveJobHistorytoXML(objPanelCompare.JobID, JobHistoryMessage, "General", "PanelAdded", ProjectSession.LoggedInName, false);   
                                    }
                                }
                            }
                            // Maintain panel history 
                        }
                        return errorDataTable.Tables[0].Rows.Count > 0 ? errorDataTable : dsResult;
                    }
                    else
                    {
                        DataTable dt = new DataTable();
                        dt.Columns.Add("ErrorMsg", typeof(string));
                        DataRow dr = dt.NewRow();
                        dr["ErrorMsg"] = "Please upload file with proper format.";
                        dt.Rows.Add(dr);
                        errorDataTable.Tables.Add(dt);
                        return errorDataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                dt.Columns.Add("ErrorMsg", typeof(string));
                DataRow dr = dt.NewRow();
                dr["ErrorMsg"] = dataTable != null ? ex.Message + " at Row: " + (dataTable.Rows.Count + 2) + " and Column: " + colIndex : ex.Message;
                dt.Rows.Add(dr);
                ds.Tables.Add(dt);
                return ds;
            }
        }

        /// <summary>
        /// DataTable for BulkUpload PVD SolarJobs
        /// </summary>
        /// <returns></returns>
        public DataTable DtBulkUploadPVDSolarJobs()
        {
            DataTable dtBulkUploadPVDSolarJobs = new DataTable();

            dtBulkUploadPVDSolarJobs.Columns.Add("JobType", typeof(int));
            dtBulkUploadPVDSolarJobs.Columns.Add("RefNumber", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("Title", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("InstallationDate", typeof(DateTime));
            dtBulkUploadPVDSolarJobs.Columns.Add("Description", typeof(string));

            dtBulkUploadPVDSolarJobs.Columns.Add("OwnerType", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("OwnerCompanyABN", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("OwnerCompanyName", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("OwnerFirstName", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("OwnerLastName", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("OwnerEmail", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("OwnerPhone", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("OwnerMobile", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("OwnerIsPostalAddress", typeof(bool));
            dtBulkUploadPVDSolarJobs.Columns.Add("OwnerUnitTypeName", typeof(string));
            //dtBulkUploadPVDSolarJobs.Columns.Add("OwnerUnitTypeID", typeof(Int32));
            dtBulkUploadPVDSolarJobs.Columns.Add("OwnerUnitNumber", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("OwnerStreetNumber", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("OwnerStreetName", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("OwnerStreetTypeName", typeof(string));
            //dtBulkUploadPVDSolarJobs.Columns.Add("OwnerStreetTypeID", typeof(int));
            dtBulkUploadPVDSolarJobs.Columns.Add("OwnerPostalAddressName", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("OwnerPostalDeliveryNumber", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("OwnerTown", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("OwnerState", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("OwnerPostCode", typeof(string));

            dtBulkUploadPVDSolarJobs.Columns.Add("InsUnitTypeName", typeof(string));
            //dtBulkUploadPVDSolarJobs.Columns.Add("InstUnitTypeID", typeof(int));
            dtBulkUploadPVDSolarJobs.Columns.Add("InsUnitNumber", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("InsStreetNumber", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("InsStreetName", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("InsStreetTypeName", typeof(string));
            //dtBulkUploadPVDSolarJobs.Columns.Add("InsStreetTypeID", typeof(int));
            dtBulkUploadPVDSolarJobs.Columns.Add("InsTown", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("InsState", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("InsPostCode", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("InsNMI", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("InsDistributorName", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("InsElectricityProvider", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("InstMeterNumber", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("InstPhaseProperty", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("InsExistingSystem", typeof(bool));
            dtBulkUploadPVDSolarJobs.Columns.Add("InsExistingSystemSize", typeof(decimal));
            dtBulkUploadPVDSolarJobs.Columns.Add("InsNoOfPanels", typeof(int));
            dtBulkUploadPVDSolarJobs.Columns.Add("InsSystemLocation", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("InstAdditionalInstallationInformation", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("InstPropertyType", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("InstSingleMultipleStory", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("InstInstallingNewPanel", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("InstLocation", typeof(string));

            dtBulkUploadPVDSolarJobs.Columns.Add("STCAdditionalCapacityNotes", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("STCTypeOfConnection", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("STCSystemMountingType", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("STCMultipleSGUAddress", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("STCLocation", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("AdditionalLocationInformation", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("AdditionalSystemInformation", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("STCDeemingPeriod", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("STCInstallingCompleteUnit", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("STCStandAloneGridSelected", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("STCCECAccreditationStatement", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("STCGovernmentSitingApproval", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("STCElectricalSafetyDocumentation", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("STCAustralianNewZealandStandardStatement", typeof(string));

            dtBulkUploadPVDSolarJobs.Columns.Add("SystemSize", typeof(decimal));
            dtBulkUploadPVDSolarJobs.Columns.Add("SerialNumbers", typeof(string));
            //dtBulkUploadPVDSolarJobs.Columns.Add("CalculatedSTC", typeof(decimal));

            dtBulkUploadPVDSolarJobs.Columns.Add("PanelBrand", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("PanelModel", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("NoOfPanel", typeof(string));

            dtBulkUploadPVDSolarJobs.Columns.Add("InverterBrand", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("InverterSeries", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("InverterModel", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("NoOfInverter", typeof(string));

            dtBulkUploadPVDSolarJobs.Columns.Add("InstallerCECAccreditationNumber", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("DesignerCECAccreditationNumber", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("ElectricianCECAccreditationNumber", typeof(string));
            dtBulkUploadPVDSolarJobs.Columns.Add("IsAutoScheduleInstaller", typeof(string));

            return dtBulkUploadPVDSolarJobs;
        }

        /// <summary>
        /// DataTable for BulkUpload SWH SolarJobs
        /// </summary>
        /// <returns></returns>
        public DataTable DtBulkUploadSWHSolarJobs()
        {
            DataTable dtBulkUploadSWHSolarJobs = new DataTable();

            dtBulkUploadSWHSolarJobs.Columns.Add("JobType", typeof(int));
            dtBulkUploadSWHSolarJobs.Columns.Add("RefNumber", typeof(string));
            dtBulkUploadSWHSolarJobs.Columns.Add("Title", typeof(string));
            dtBulkUploadSWHSolarJobs.Columns.Add("InstallationDate", typeof(DateTime));
            dtBulkUploadSWHSolarJobs.Columns.Add("Description", typeof(string));

            dtBulkUploadSWHSolarJobs.Columns.Add("OwnerType", typeof(string));
            dtBulkUploadSWHSolarJobs.Columns.Add("OwnerCompanyABN", typeof(string));
            dtBulkUploadSWHSolarJobs.Columns.Add("OwnerCompanyName", typeof(string));
            dtBulkUploadSWHSolarJobs.Columns.Add("OwnerFirstName", typeof(string));
            dtBulkUploadSWHSolarJobs.Columns.Add("OwnerLastName", typeof(string));
            dtBulkUploadSWHSolarJobs.Columns.Add("OwnerEmail", typeof(string));
            dtBulkUploadSWHSolarJobs.Columns.Add("OwnerPhone", typeof(string));
            dtBulkUploadSWHSolarJobs.Columns.Add("OwnerMobile", typeof(string));
            dtBulkUploadSWHSolarJobs.Columns.Add("OwnerIsPostalAddress", typeof(bool));
            dtBulkUploadSWHSolarJobs.Columns.Add("OwnerUnitTypeName", typeof(string));
            //dtBulkUploadSWHSolarJobs.Columns.Add("OwnerUnitTypeID", typeof(Int32));
            dtBulkUploadSWHSolarJobs.Columns.Add("OwnerUnitNumber", typeof(string));
            dtBulkUploadSWHSolarJobs.Columns.Add("OwnerStreetNumber", typeof(string));
            dtBulkUploadSWHSolarJobs.Columns.Add("OwnerStreetName", typeof(string));
            dtBulkUploadSWHSolarJobs.Columns.Add("OwnerStreetTypeName", typeof(string));
            //dtBulkUploadSWHSolarJobs.Columns.Add("OwnerStreetTypeID", typeof(int));
            dtBulkUploadSWHSolarJobs.Columns.Add("OwnerPostalAddressName", typeof(string));
            dtBulkUploadSWHSolarJobs.Columns.Add("OwnerPostalDeliveryNumber", typeof(string));
            dtBulkUploadSWHSolarJobs.Columns.Add("OwnerTown", typeof(string));
            dtBulkUploadSWHSolarJobs.Columns.Add("OwnerState", typeof(string));
            dtBulkUploadSWHSolarJobs.Columns.Add("OwnerPostCode", typeof(string));

            dtBulkUploadSWHSolarJobs.Columns.Add("InsUnitTypeName", typeof(string));
            //dtBulkUploadSWHSolarJobs.Columns.Add("InstUnitTypeID", typeof(int));
            dtBulkUploadSWHSolarJobs.Columns.Add("InsUnitNumber", typeof(string));
            dtBulkUploadSWHSolarJobs.Columns.Add("InsStreetNumber", typeof(string));
            dtBulkUploadSWHSolarJobs.Columns.Add("InsStreetName", typeof(string));
            dtBulkUploadSWHSolarJobs.Columns.Add("InsStreetTypeName", typeof(string));
            //dtBulkUploadSWHSolarJobs.Columns.Add("InsStreetTypeID", typeof(int));
            dtBulkUploadSWHSolarJobs.Columns.Add("InsTown", typeof(string));
            dtBulkUploadSWHSolarJobs.Columns.Add("InsState", typeof(string));
            dtBulkUploadSWHSolarJobs.Columns.Add("InsPostCode", typeof(string));
            dtBulkUploadSWHSolarJobs.Columns.Add("InsDistributorName", typeof(string));
            dtBulkUploadSWHSolarJobs.Columns.Add("InstAdditionalInstallationInformation", typeof(string));
            dtBulkUploadSWHSolarJobs.Columns.Add("InstPropertyType", typeof(string));
            dtBulkUploadSWHSolarJobs.Columns.Add("InstSingleMultipleStory", typeof(string));

            //dtBulkUploadSWHSolarJobs.Columns.Add("STCSystemMountingType", typeof(string));
            dtBulkUploadSWHSolarJobs.Columns.Add("STCMultipleSGUAddress", typeof(string));
            dtBulkUploadSWHSolarJobs.Columns.Add("STCVolumetricCapacity", typeof(string));
            dtBulkUploadSWHSolarJobs.Columns.Add("STCSecondhandWaterHeater", typeof(string));
            dtBulkUploadSWHSolarJobs.Columns.Add("STCStatutoryDeclarations", typeof(string));
            dtBulkUploadSWHSolarJobs.Columns.Add("AdditionalSystemInformation", typeof(string));
            dtBulkUploadSWHSolarJobs.Columns.Add("STCStandAloneGridSelected", typeof(string));
            dtBulkUploadSWHSolarJobs.Columns.Add("STCCECAccreditationStatement", typeof(string));
            dtBulkUploadSWHSolarJobs.Columns.Add("STCGovernmentSitingApproval", typeof(string));
            dtBulkUploadSWHSolarJobs.Columns.Add("STCElectricalSafetyDocumentation", typeof(string));
            dtBulkUploadSWHSolarJobs.Columns.Add("STCAustralianNewZealandStandardStatement", typeof(string));

            //dtBulkUploadSWHSolarJobs.Columns.Add("SerialNumbers", typeof(string));
            //dtBulkUploadSWHSolarJobs.Columns.Add("CalculatedSTC", typeof(decimal));
            dtBulkUploadSWHSolarJobs.Columns.Add("InstallationType", typeof(string));
            dtBulkUploadSWHSolarJobs.Columns.Add("SystemBrand", typeof(string));
            dtBulkUploadSWHSolarJobs.Columns.Add("SystemModel", typeof(string));
            dtBulkUploadSWHSolarJobs.Columns.Add("NoOfSystemBrand", typeof(string));

            //dtBulkUploadSWHSolarJobs.Columns.Add("", typeof());
            //dtBulkUploadSWHSolarJobs.Columns.Add("", typeof());
            //dtBulkUploadSWHSolarJobs.Columns.Add("", typeof());
            //dtBulkUploadSWHSolarJobs.Columns.Add("", typeof());

            return dtBulkUploadSWHSolarJobs;
        }

        /// <summary>
        /// Error DataTable
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public DataSet ErrorDatatable(DataTable dataTable)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            dt.Columns.Add("ErrorMsg", typeof(string));
            string msg = string.Empty;
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                if (string.IsNullOrEmpty(dataTable.Rows[i]["JobType"].ToString()) || string.IsNullOrEmpty(dataTable.Rows[i]["RefNumber"].ToString()))
                {
                    msg = "JobType and RefNumber is required";
                    InsertErrorMsgInDatatable(dt, msg, i);
                }
                if (string.IsNullOrEmpty(dataTable.Rows[i]["OwnerType"].ToString()) || string.IsNullOrEmpty(dataTable.Rows[i]["OwnerFirstName"].ToString()) || string.IsNullOrEmpty(dataTable.Rows[i]["OwnerLastName"].ToString()) || string.IsNullOrEmpty(dataTable.Rows[i]["OwnerPhone"].ToString()) || string.IsNullOrEmpty(dataTable.Rows[i]["OwnerIsPostalAddress"].ToString()))
                {
                    msg = "Job Owner Details are required";
                    InsertErrorMsgInDatatable(dt, msg, i);
                }
                if (dataTable.Rows[i]["OwnerType"].ToString().ToLower() != "individual" && (string.IsNullOrEmpty(dataTable.Rows[i]["OwnerCompanyABN"].ToString()) || string.IsNullOrEmpty(dataTable.Rows[i]["OwnerCompanyName"].ToString())))
                {
                    msg = "Owner Company ABN and Owner Company Name is required.";
                    InsertErrorMsgInDatatable(dt, msg, i);
                }

                // check companyABN and companyName from ABNLink
                if (dataTable.Rows[i]["OwnerType"].ToString().ToLower() != "individual" && (!string.IsNullOrEmpty(dataTable.Rows[i]["OwnerCompanyABN"].ToString()) && !string.IsNullOrEmpty(dataTable.Rows[i]["OwnerCompanyName"].ToString())))
                {
                    var jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(_userController.GetCompanyABN(dataTable.Rows[i]["OwnerCompanyABN"].ToString()));
                    var list = JObject.Parse(jsonData)["Data"].Select(el => new { CompanyName = (string)el["CompanyName"] }).ToList();
                    bool validCompanyName = false;
                    foreach (var item in list)
                    {
                        if (item.CompanyName == dataTable.Rows[i]["OwnerCompanyName"].ToString())
                        {
                            validCompanyName = true;
                            break;
                        }
                    }
                    if (!validCompanyName)
                    {
                        msg = "Invalid Owner Company ABN or Owner Company Name.";
                        InsertErrorMsgInDatatable(dt, msg, i);
                    }
                }

                if (dataTable.Rows[i]["OwnerType"].ToString().ToLower() == "individual" && (!string.IsNullOrEmpty(dataTable.Rows[i]["OwnerCompanyABN"].ToString()) || !string.IsNullOrEmpty(dataTable.Rows[i]["OwnerCompanyName"].ToString())))
                {
                    msg = "Owner Company ABN and Owner Company Name is not required when OwnerType is Individual.";
                    InsertErrorMsgInDatatable(dt, msg, i);
                }
                if (!string.IsNullOrEmpty(dataTable.Rows[i]["OwnerIsPostalAddress"].ToString()))
                {
                    DatatableAddressValidation("Owner", dataTable, i, dt);
                }

                // For Installation address validation(will always have physical address only as per Hus)
                DatatableAddressValidation("Ins", dataTable, i, dt, true);
                decimal Nmi;
                if (dataTable.Rows[i]["JobType"].ToString() == "1")
                {
                    //if (string.IsNullOrEmpty(dataTable.Rows[i]["InsNMI"].ToString()))
                    //{
                    //    msg = "NMI is required.";
                    //    InsertErrorMsgInDatatable(dt, msg, i);
                    //}
                    if(!string.IsNullOrEmpty(dataTable.Rows[i]["InsNMI"].ToString()) && dataTable.Rows[i]["InsNMI"].ToString().Length < 10)
                    {
                        msg = "Minimum 10 characters required for NMI.";
                        InsertErrorMsgInDatatable(dt, msg, i);
                    }
                    if (!string.IsNullOrEmpty(dataTable.Rows[i]["InsNMI"].ToString()) && decimal.TryParse(dataTable.Rows[i]["InsNMI"].ToString(), out Nmi) && Nmi == 0)
                    {
                        msg = "NMI value is not correct.";
                        InsertErrorMsgInDatatable(dt, msg, i);
                    }
                        if ((string.IsNullOrEmpty(dataTable.Rows[i]["InstInstallingNewPanel"].ToString()) || dataTable.Rows[i]["InstInstallingNewPanel"].ToString().ToLower() == "new") && (!string.IsNullOrEmpty(dataTable.Rows[i]["InstLocation"].ToString()) || !string.IsNullOrEmpty(dataTable.Rows[i]["STCAdditionalCapacityNotes"].ToString())))
                    {
                        dataTable.Rows[i]["InstLocation"] = DBNull.Value;
                        dataTable.Rows[i]["STCAdditionalCapacityNotes"] = DBNull.Value;
                    }
                    if (string.IsNullOrEmpty(dataTable.Rows[i]["InstLocation"].ToString()) && !string.IsNullOrEmpty(dataTable.Rows[i]["STCAdditionalCapacityNotes"].ToString()))
                    {
                        dataTable.Rows[i]["STCAdditionalCapacityNotes"] = DBNull.Value;
                    }
                    if ((string.IsNullOrEmpty(dataTable.Rows[i]["STCMultipleSGUAddress"].ToString()) || dataTable.Rows[i]["STCMultipleSGUAddress"].ToString().ToLower() != "yes") && (!string.IsNullOrEmpty(dataTable.Rows[i]["STCLocation"].ToString()) || !string.IsNullOrEmpty(dataTable.Rows[i]["AdditionalLocationInformation"].ToString())))
                    {
                        dataTable.Rows[i]["STCLocation"] = DBNull.Value;
                        dataTable.Rows[i]["AdditionalLocationInformation"] = DBNull.Value;
                    }
                    if (string.IsNullOrEmpty(dataTable.Rows[i]["STCLocation"].ToString()) && !string.IsNullOrEmpty(dataTable.Rows[i]["AdditionalLocationInformation"].ToString()))
                    {
                        dataTable.Rows[i]["AdditionalLocationInformation"] = DBNull.Value;
                    }

                    int panelBrand = (!string.IsNullOrEmpty(dataTable.Rows[i]["PanelBrand"].ToString())) ? dataTable.Rows[i]["PanelBrand"].ToString().Split(',').Count() : 0;
                    int panelModel = (!string.IsNullOrEmpty(dataTable.Rows[i]["PanelModel"].ToString())) ? dataTable.Rows[i]["PanelModel"].ToString().Split(',').Count() : 0;
                    int noOfPanel = (!string.IsNullOrEmpty(dataTable.Rows[i]["NoOfPanel"].ToString())) ? dataTable.Rows[i]["NoOfPanel"].ToString().Split(',').Count() : 0;

                    if (panelBrand != panelModel || panelModel != noOfPanel)
                    {
                        msg = "Panel detail is Invalid";
                        InsertErrorMsgInDatatable(dt, msg, i);
                    }
                    if(noOfPanel == 0)
                    {
                        msg = "No of panel should be between 1 to 10000";
                        InsertErrorMsgInDatatable(dt, msg, i);
                    }

                    int inverterBrand = (!string.IsNullOrEmpty(dataTable.Rows[i]["InverterBrand"].ToString())) ? dataTable.Rows[i]["InverterBrand"].ToString().Split(',').Count() : 0;
                    int inverterSeries = (!string.IsNullOrEmpty(dataTable.Rows[i]["InverterSeries"].ToString())) ? dataTable.Rows[i]["InverterSeries"].ToString().Split(',').Count() : 0;
                    int inverterModel = (!string.IsNullOrEmpty(dataTable.Rows[i]["InverterModel"].ToString())) ? dataTable.Rows[i]["InverterModel"].ToString().Split(',').Count() : 0;
                    int noOfInverter = (!string.IsNullOrEmpty(dataTable.Rows[i]["NoOfInverter"].ToString())) ? dataTable.Rows[i]["NoOfInverter"].ToString().Split(',').Count() : 0;

                    if (inverterBrand != inverterSeries || inverterSeries != inverterModel || inverterModel != noOfInverter)
                    {
                        msg = "Inverter detail is Invalid";
                        InsertErrorMsgInDatatable(dt, msg, i);
                    }
                    if (noOfInverter == 0)
                    {
                        msg = "No of inverter should be between 1 to 10000";
                        InsertErrorMsgInDatatable(dt, msg, i);
                    }
                }
                else
                {
                    if (dataTable.Rows[i]["STCVolumetricCapacity"].ToString().ToLower() != "yes" && !string.IsNullOrEmpty(dataTable.Rows[i]["STCStatutoryDeclarations"].ToString()))
                    {
                        dataTable.Rows[i]["STCStatutoryDeclarations"] = DBNull.Value;
                    }

                    int systemBrand = (!string.IsNullOrEmpty(dataTable.Rows[i]["SystemBrand"].ToString())) ? dataTable.Rows[i]["SystemBrand"].ToString().Split(',').Count() : 0;
                    int systemModel = (!string.IsNullOrEmpty(dataTable.Rows[i]["SystemModel"].ToString())) ? dataTable.Rows[i]["SystemModel"].ToString().Split(',').Count() : 0;
                    int noOfSystemBrand = (!string.IsNullOrEmpty(dataTable.Rows[i]["NoOfSystemBrand"].ToString())) ? dataTable.Rows[i]["NoOfSystemBrand"].ToString().Split(',').Count() : 0;

                    if (systemBrand != systemModel || systemModel != noOfSystemBrand)
                    {
                        msg = "System Brand detail is Invalid";
                        InsertErrorMsgInDatatable(dt, msg, i);
                    }
                }
            }
            ds.Tables.Add(dt);
            return ds;
        }

        /// <summary>
        /// Insert ErrorMsg In Error Datatable
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="msg"></param>
        /// <param name="rowNo"></param>
        public void InsertErrorMsgInDatatable(DataTable dt, string msg, int rowNo)
        {
            rowNo = rowNo + 2;
            DataRow dr = dt.NewRow();
            dr["ErrorMsg"] = "Row:" + rowNo + " " + msg;
            dt.Rows.Add(dr);
        }

        /// <summary>
        /// Address Validation of Datatable records
        /// </summary>
        /// <param name="jobClass"></param>
        /// <param name="dataTable"></param>
        /// <param name="rowNo"></param>
        /// <param name="ErrorDatatable"></param>
        /// <param name="isInstallationAddress"></param>
        public void DatatableAddressValidation(string jobClass, DataTable dataTable, int rowNo, DataTable ErrorDatatable, bool isInstallationAddress = false)
        {
            string msg = string.Empty;
            string userAddress = isInstallationAddress ? "Installation" : jobClass;

            if (isInstallationAddress || dataTable.Rows[rowNo][jobClass + "IsPostalAddress"].ToString().ToLower() == "false")
            {
                if (!string.IsNullOrEmpty(dataTable.Rows[rowNo][jobClass + "UnitTypeName"].ToString()) && string.IsNullOrEmpty(dataTable.Rows[rowNo][jobClass + "UnitNumber"].ToString()))
                {
                    msg = userAddress + " Unit Number is required.";
                    InsertErrorMsgInDatatable(ErrorDatatable, msg, rowNo);
                }
                if (string.IsNullOrEmpty(dataTable.Rows[rowNo][jobClass + "StreetNumber"].ToString()))
                {
                    msg = userAddress + " Street Number is required.";
                    InsertErrorMsgInDatatable(ErrorDatatable, msg, rowNo);
                }
                if (string.IsNullOrEmpty(dataTable.Rows[rowNo][jobClass + "StreetName"].ToString()))
                {
                    msg = userAddress + " Street Name is required.";
                    InsertErrorMsgInDatatable(ErrorDatatable, msg, rowNo);
                }
                if (string.IsNullOrEmpty(dataTable.Rows[rowNo][jobClass + "StreetTypeName"].ToString()))
                {
                    msg = userAddress + " Street Type Name is required.";
                    InsertErrorMsgInDatatable(ErrorDatatable, msg, rowNo);
                }
                if (!isInstallationAddress)
                {
                    dataTable.Rows[rowNo][jobClass + "PostalAddressName"] = DBNull.Value;
                    dataTable.Rows[rowNo][jobClass + "PostalDeliveryNumber"] = DBNull.Value;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(dataTable.Rows[rowNo][jobClass + "PostalAddressName"].ToString()))
                {
                    msg = userAddress + " Postal Address is required.";
                    InsertErrorMsgInDatatable(ErrorDatatable, msg, rowNo);
                }
                if (string.IsNullOrEmpty(dataTable.Rows[rowNo][jobClass + "PostalDeliveryNumber"].ToString()))
                {
                    msg = userAddress + " Postal Delivery Number is required.";
                    InsertErrorMsgInDatatable(ErrorDatatable, msg, rowNo);
                }

                dataTable.Rows[rowNo][jobClass + "UnitTypeName"] = DBNull.Value;
                dataTable.Rows[rowNo][jobClass + "UnitNumber"] = DBNull.Value;
                dataTable.Rows[rowNo][jobClass + "StreetNumber"] = DBNull.Value;
                dataTable.Rows[rowNo][jobClass + "StreetName"] = DBNull.Value;
                dataTable.Rows[rowNo][jobClass + "StreetTypeName"] = DBNull.Value;
            }

            if (!string.IsNullOrEmpty(dataTable.Rows[rowNo][jobClass + "PostCode"].ToString()) && !string.IsNullOrEmpty(dataTable.Rows[rowNo][jobClass + "Town"].ToString()) && !string.IsNullOrEmpty(dataTable.Rows[rowNo][jobClass + "State"].ToString()))
            {
                dataTable.Rows[rowNo][jobClass + "Town"] = dataTable.Rows[rowNo][jobClass + "Town"].ToString().ToUpper();
                string postCodeValidationMsg = PostCodeValidation(dataTable.Rows[rowNo][jobClass + "PostCode"].ToString(), dataTable.Rows[rowNo][jobClass + "Town"].ToString(), dataTable.Rows[rowNo][jobClass + "State"].ToString());
                if (!string.IsNullOrEmpty(postCodeValidationMsg))
                {
                    msg = userAddress + postCodeValidationMsg;
                    InsertErrorMsgInDatatable(ErrorDatatable, msg, rowNo);
                }
            }
            else
            {
                msg = userAddress + " Town, State and Postal Code is required.";
                InsertErrorMsgInDatatable(ErrorDatatable, msg, rowNo);
            }
        }

        /// <summary>
        /// PostCode Validation
        /// </summary>
        /// <param name="PostCode"></param>
        /// <param name="Town"></param>
        /// <param name="State"></param>
        /// <returns></returns>
        public string PostCodeValidation(string PostCode, string Town, string State)
        {
            string msg = string.Empty;
            string q = PostCode;
            string ret = string.Empty;
            var webRequest = System.Net.WebRequest.Create(string.Format("https://auspost.com.au/api/postcode/search.json?q=" + q + "&excludePostBoxFlag=flase"));

            if (webRequest != null)
            {
                webRequest.Headers.Add("AUTH-KEY", "0344e02f-843b-49a7-8fd6-d35acd471480");
                webRequest.Method = "GET";
                webRequest.Timeout = 20000;
                webRequest.ContentType = "application/json";
            }

            HttpWebResponse resp = (HttpWebResponse)webRequest.GetResponse();
            Stream resStream = resp.GetResponseStream();
            StreamReader reader = new StreamReader(resStream);
            ret = reader.ReadToEnd();
            ret = ret.Replace(@"\", "");
            if (!ret.Contains("["))
            {
                ret = ret.Replace("{\"c", "[{\"c");
                ret = ret.Replace("}}}", "}]}}");
            }

            if (!ret.Contains("locality"))
            {
                msg = " PostCode is invalid.";
            }
            else
            {
                if (!ret.Contains(Town) || !ret.Contains(State))
                {
                    msg = " PostCode/Town/State is invalid.";
                }
            }
            return msg;
        }
    }
}
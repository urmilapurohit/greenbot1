using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FormBot.BAL;
using FormBot.Helper;
using System.IO;
using System.Data;
using FormBot.Entity;
using OfficeOpenXml;
using FormBot.Entity.KendoGrid;

namespace FormBot.SPVMain.Controllers
{
    [RoutePrefix("SpvVerification")]
    public class SpvVerificationController : Controller
    {
        #region Properties
        private readonly ISpvVerificationBAL _spvVerificationBAL;
        private readonly Logger _log;
        private readonly BAL.Service.ICERImportBAL _cerImportBAL;
        #endregion
        #region Constructor
        public SpvVerificationController(ISpvVerificationBAL spvVerificationBAL, BAL.Service.ICERImportBAL cerImportBAL)
        {
            this._spvVerificationBAL = spvVerificationBAL;
            this._cerImportBAL = cerImportBAL;
            this._log = new Logger();
        }
        #endregion
        #region Methods
        #region Actions
        // GET: SpvVerification
        [UserAuthorization]
        public ActionResult Index()
        {
            _log.Log(SystemEnums.Severity.Debug, "Index Method Call");
            return View();
        }

        public ActionResult DownloadSpvData(int SpvUserId)
        {
            try
            {
                if (SpvUserId > 0)
                {
                    List<SpvPanelDetailsModel> lstSpvPanelDetails = _spvVerificationBAL.GetSpvPanelDetailsSearchByManufacturer(SpvUserId, "", "",true);
                    using (ExcelPackage pckExport = new ExcelPackage())
                    {
                        //SerialNumber  ModelNumber Wattage VOC ISC PM VM  IM FF  N

                        ExcelWorksheet worksheetExport = pckExport.Workbook.Worksheets.Add("SpvVerificationData");
                        worksheetExport.Cells[1, 1].Value = "SerialNumber";
                        worksheetExport.Cells[1, 2].Value = "ModelNumber";
                        worksheetExport.Cells[1, 3].Value = "Wattage";
                        worksheetExport.Cells[1, 4].Value = "VOC";
                        worksheetExport.Cells[1, 5].Value = "ISC";
                        worksheetExport.Cells[1, 6].Value = "PM";
                        worksheetExport.Cells[1, 7].Value = "VM";
                        worksheetExport.Cells[1, 8].Value = "IM";
                        worksheetExport.Cells[1, 9].Value = "FF";
                        //worksheetExport.Cells[1, 10].Value = "N";
                        int lastRowsInserted = 2;
                        for (int rowNo = 0; rowNo < lstSpvPanelDetails.Count; rowNo++)
                        {
                            worksheetExport.Cells[lastRowsInserted, 1].Value = lstSpvPanelDetails[rowNo].SerialNumber;
                            worksheetExport.Cells[lastRowsInserted, 2].Value = lstSpvPanelDetails[rowNo].ModelNumber;
                            worksheetExport.Cells[lastRowsInserted, 3].Value = lstSpvPanelDetails[rowNo].Wattage;
                            worksheetExport.Cells[lastRowsInserted, 4].Value = lstSpvPanelDetails[rowNo].VOC;
                            worksheetExport.Cells[lastRowsInserted, 5].Value = lstSpvPanelDetails[rowNo].ISC;
                            worksheetExport.Cells[lastRowsInserted, 6].Value = lstSpvPanelDetails[rowNo].PM;
                            worksheetExport.Cells[lastRowsInserted, 7].Value = lstSpvPanelDetails[rowNo].VM;
                            worksheetExport.Cells[lastRowsInserted, 8].Value = lstSpvPanelDetails[rowNo].IM;
                            worksheetExport.Cells[lastRowsInserted, 9].Value = lstSpvPanelDetails[rowNo].FF;
                            //worksheetExport.Cells[lastRowsInserted, 6].Value = lstSpvPanelDetails[rowNo].N;
                            lastRowsInserted++;
                        }

                        string filename = "SpvPanelDetails_" + DateTime.Now.ToString("dd-MMM-yyyy") + ".xlsx";
                        Response.Clear();
                        Response.ClearHeaders();
                        Response.ClearContent();
                        Response.ContentType = "application/octet-stream";
                        Response.AddHeader("content-disposition", "attachment;  filename=\"" + filename + "\"");
                        Response.BinaryWrite(pckExport.GetAsByteArray());
                        Response.End();
                    }
                }
                return RedirectToAction("Index", "SpvVerification");
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return RedirectToAction("Index", "SpvVerification");
            }

        }
        [HttpPost]
        [SpvUserAuthorization]
        public JsonResult GetSpvPanelDetailsSearchByManufacturer(int SpvUserId, string SerialNumber, string ModelNumber, KendoGridData filter = null)
        {
            List<SpvPanelDetailsModel> lstSpvPanelDetails = _spvVerificationBAL.GetSpvPanelDetailsSearchByManufacturer(SpvUserId, SerialNumber, ModelNumber);
           
            if (lstSpvPanelDetails !=null && lstSpvPanelDetails.Count>0 && filter != null && filter.Filters != null && filter.Filters.Count > 0)
            {
                if (filter.Filters[0].Field == "IsInstallationVerified")
                lstSpvPanelDetails=   lstSpvPanelDetails.Where(x => x.IsInstallationVerified == Convert.ToBoolean(filter.Filters[0].Value)).ToList();
                
            }
            return Json(new { total = lstSpvPanelDetails.Count, data = lstSpvPanelDetails }, JsonRequestBehavior.AllowGet);
        }

        //public void SpvPanelDetailsFilter(KendoGridData filter, List<SpvPanelDetailsModel> lstSpvPanelDetails)
        //{
        //    string query = "";
        //    var op = "LIKE";
        //    if (filter != null && filter.Filters != null && filter.Filters.Count > 0)
        //    {
        //        query += "(";

        //        for (int i = 0; i < filter.Filters.Count; i++)
        //        {
        //            if (filter.Filters[i] != null)
        //            {
        //                if (i > 0)
        //                {
        //                    query += " " + filter.Logic + " ";
        //                }
        //                if (filter.Filters[i].Filters != null)
        //                {
        //                    query = query + "(";
        //                    for (int j = 0; j < filter.Filters[i].Filters.Count; j++)
        //                    {
        //                        if (j > 0)
        //                        {
        //                            query += " " + filter.Filters[i].Logic + " ";
        //                        }
        //                        query += filter.Filters[j].Field + " " + op + " '%" + filter.Filters[j].Value.Trim() + "%'";
        //                    }
        //                    query = query + ")";
        //                    //query = query.Substring(0, query.LastIndexOf(filter.Filters[i].Logic));
        //                }
        //                else
        //                {
        //                    query += filter.Filters[i].Field + " " + op + " '%" + filter.Filters[i].Value.Trim() + "%'";
        //                }
        //            }

        //        }

        //        query = query + ")";
        //        //query = query.Substring(0,query.LastIndexOf(filter.Logic));
        //        //dv.RowFilter = query;
        //        //string expression = "Modul =" + value;

        //    }
        //    if (query.Length > 0)
        //    {
        //        DataTable dt = new DataTable();
        //        dt = dt.Select(query).Length > 0 ? dt.Select(query).CopyToDataTable() : new DataTable();

        //        //lstSpvPanelDetails = lstSpvPanelDetails.Select(query).Length > 0 ? lstSpvPanelDetails.Select(query).CopyToDataTable() : new List<SpvPanelDetailsModel>();
        //    }
          
        //}
        [HttpGet]
        [SpvUserAuthorization]
        public JsonResult GetSpvPanelManufacturer()
        {
            List<SelectListItem> items = _spvVerificationBAL.GetData().AsEnumerable().Select(a => new SelectListItem { Text = a.Manufacturer, Value = a.SpvPanelManufacturerId.ToString() }).ToList();
            return Json(items, JsonRequestBehavior.AllowGet);
        }
        public FileResult DownloadTemplateForUploadSpvPanelDetails()
        {
            string path = ProjectConfiguration.TemplateForUploadSpvPanelDetailsPath;
            //string documentFullPath = System.Web.HttpContext.Current.Server.MapPath(path);
            //_log.Log(SystemEnums.Severity.Info, documentFullPath);
            if (System.IO.File.Exists(path))
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                string fileName = path.Split('\\').Last();
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "SpvPanelDetailsTemplate.xlsx");
            }
            else
                Response.Write("File Doesn't exists!");
            return null;
        }
        [HttpPost]
        [SpvUserAuthorization]
        public JsonResult UploadSpvPanelDetails(int SpvUserId)
        {
            string filePath = string.Empty;

            if (ModelState.IsValid)
            {
                HttpPostedFileBase fileUpload = Request.Files[0];
                //int ManufacturerId = Convert.ToInt32(Request["ManufacturerId"]);
                if (fileUpload != null && fileUpload.ContentLength > 0)
                {
                    string OriginalFileName = Path.GetFileName(fileUpload.FileName);
                    string extension = Path.GetExtension(OriginalFileName);
                    string filename = SpvUserId + "_" + Guid.NewGuid().ToString() + extension;
                    string dirPath = System.Web.HttpContext.Current.Server.MapPath("~/SpvPanelDetails/");
                    //string dirPath = Path.Combine(ProjectConfiguration.UploadedDocumentPath + "\\SpvPanelDetails\\");
                    filePath = Path.Combine(dirPath + filename);

                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }


                    fileUpload.SaveAs(filePath);
                    DataSet dsResult = new DataSet();
                    dsResult = GetDataTableFromSpreadsheet(filePath);

                    //if (dsResult.Tables[0].Rows.Count > 0)
                    //{
                    //    if (System.IO.File.Exists(filePath))
                    //    {
                    //        System.IO.File.Delete(filePath);
                    //    }
                    //}
                    if (dsResult != null)
                    {
                        if (dsResult.Tables.Count > 0)
                        {
                            if (dsResult.Tables[0].Columns.Count > 1)
                            {
                                _spvVerificationBAL.BulkInsertSpvPanelManufacturer(dsResult.Tables[0], SpvUserId);
                                _spvVerificationBAL.SpvPanelDetailsUploadHistoryInsert(filePath.Replace(ProjectConfiguration.ProofDocumentsURL, ""), filename, SpvUserId);
                            }
                        }
                    }
                    return Json(new { status = dsResult.Tables[0].Rows.Count > 1 ? true : false, msg = Newtonsoft.Json.JsonConvert.SerializeObject(dsResult.Tables[0]) }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [SpvUserAuthorization]
        [HttpPost]
        public JsonResult GetSpvPanelDetailsUploadedHistory(int manufactureID)
        {
            var data= _spvVerificationBAL.GetSpvPanelDetailsUploadedHistory(manufactureID);
            return Json(new { total = data.Count, data = data }, JsonRequestBehavior.AllowGet);
        }
        [SpvUserAuthorization]
        [HttpPost]
        public JsonResult ReleaseSelectedSerialNumber(List<string> serialNumbers)
        {
            string strSerialNumbers = string.Empty;
            if (serialNumbers != null && serialNumbers.Count > 0)
            {
                strSerialNumbers = serialNumbers.Count() > 0 ? string.Join(",", serialNumbers) : string.Empty;
                if (strSerialNumbers != string.Empty)
                {
                    _spvVerificationBAL.ReleaseSerailNumbers(strSerialNumbers);
                    return Json(new { status = true }, JsonRequestBehavior.AllowGet);
                }
            }
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region NonAction
        [NonAction]
        DataSet GetDataTableFromSpreadsheet(string filePath)
        {
            DataTable dtError = new DataTable();
            dtError.Columns.Add("ErrorMsg", typeof(string));

            DataTable dtExcel = CreateDataTableForSpvPanelManufacturerDetails();
            DataSet errorDataTable = new DataSet();
            try
            {
                FileStream stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                Excel.IExcelDataReader excelReader = Excel.ExcelReaderFactory.CreateOpenXmlReader(stream);

                excelReader.IsFirstRowAsColumnNames = true;
                DataSet result = excelReader.AsDataSet();

                foreach (DataTable dt in result.Tables)
                {
                    foreach (DataColumn dtcolumn in dt.Columns)
                    {
                        if (dtcolumn.ColumnName.ToString().ToLower().Equals("serialnumber"))
                        {
                            //dtExcel = dt;
                            SetBankValueInColumn(ref dtExcel, dt);
                            goto getSheet;
                        }
                    }
                }
            getSheet:
                var rowCount = dtExcel.AsEnumerable().Count();
                if (rowCount >= 51)
                {
                    setErrorMessageInDatatable(ref dtError, "Please upload file with less than 50 records.");
                    errorDataTable.Tables.Add(dtError);
                }
                else
                {
                    if (dtExcel.Rows.Count > 0)
                        errorDataTable.Tables.Add(dtExcel.Copy());
                    else
                    {
                        setErrorMessageInDatatable(ref dtError, "Please upload file with proper format.");
                        errorDataTable.Tables.Add(dtError);
                    }
                }
            }
            catch (Exception ex)
            {
                setErrorMessageInDatatable(ref dtError, ex.Message);
                errorDataTable.Tables.Add(dtError);
                _log.LogException(SystemEnums.Severity.Error, "", ex);
            }
            return errorDataTable;
        }
        [NonAction]
        void SetBankValueInColumn(ref DataTable dt, DataTable Result)
        {
            for (int i = 0; i < Result.Rows.Count; i++)
            {
                DataRow dr = dt.NewRow();
                for (int j = 0; j < Result.Columns.Count; j++)
                {
                    if (string.IsNullOrEmpty(Result.Rows[i][j].ToString()))
                        dr[j] = DBNull.Value;
                    else
                    {
                        if (dt.Columns[j].DataType == typeof(DateTime))
                            dr[j] = Convert.ToDateTime(Result.Rows[i][j].ToString().Trim());
                        if (dt.Columns[j].DataType == typeof(int))
                            dr[j] = Convert.ToInt32(Result.Rows[i][j].ToString().Trim());
                        if (dt.Columns[j].DataType == typeof(decimal))
                            dr[j] = Convert.ToDouble(Result.Rows[i][j].ToString().Trim());
                        else
                            dr[j] = Result.Rows[i][j].ToString().Trim();
                    }
                }
                dt.Rows.Add(dr);
            }
        }
        [NonAction]
        DataTable CreateDataTableForSpvPanelManufacturerDetails()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("SerialNumber", typeof(string));
            dt.Columns.Add("ModelNumber", typeof(string));
            dt.Columns.Add("Wattage", typeof(int));
            dt.Columns.Add("VOC", typeof(double));
            dt.Columns.Add("ISC", typeof(double));
            dt.Columns.Add("PM", typeof(double));
            dt.Columns.Add("VM", typeof(double));
            dt.Columns.Add("IM", typeof(double));
            dt.Columns.Add("FF", typeof(double));
            dt.Columns.Add("N", typeof(double));
            return dt;
        }
        [NonAction]
        void setErrorMessageInDatatable(ref DataTable dtError, string errorMessage)
        {
            DataRow dr = dtError.NewRow();
            dr["ErrorMsg"] = errorMessage;
            dtError.Rows.Add(dr);
        }
        #endregion
        #endregion
    }
}
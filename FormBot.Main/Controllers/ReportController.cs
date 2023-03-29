using FormBot.BAL;
using FormBot.BAL.Service;
using FormBot.Entity;
using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.HPSF;
using System.IO;
using System.Data;

namespace FormBot.Main.Controllers
{
    public class ReportController : Controller
    {
        #region Properties
        private readonly IReportBAL _reportService;
        #endregion

        #region Constructor
        public ReportController(IReportBAL reportService)
        {
            this._reportService = reportService;
        }
        #endregion

        #region Event

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>ActionResult</returns>
        [UserAuthorization]
        public ActionResult Index()
        {
            Report model = new Report();
            var designRole = from SystemEnums.DateGrouping s in Enum.GetValues(typeof(SystemEnums.DateGrouping))
                             select new { ID = s.GetHashCode(), Name = s.ToString().Replace('_', ' ') };
            ViewBag.DateGrouping = new SelectList(designRole, "ID", "Name");
            return View(model);
        }

        /// <summary>
        /// the owner account.
        /// </summary>
        /// <returns>ActionResult</returns>
        public ActionResult _OwnerAccount()
        {
            Report model = new Report();
            var lstOwnerList = _reportService.GetOwnerAccount();
            model.LstOwnerAccountUser = lstOwnerList;
            model.LstOwnerAccountAssignedUser = new List<SelectListItem>();
            return View(model);
        }

        /// <summary>
        /// the type of the action.
        /// </summary>
        /// <returns>ActionResult</returns>
        public ActionResult _ActionType()
        {
            Report model = new Report();
            var lstActionType = _reportService.GetActionType();
            model.LstActionTypeUser = lstActionType;
            model.LstActionTypeAssignedUser = new List<SelectListItem>();
            return View(model);
        }

        /// <summary>
        /// the type of the status.
        /// </summary>
        /// <returns>ActionResult</returns>
        public ActionResult _StatusType()
        {
            Report model = new Report();
            var lstStatusType = _reportService.GetStatusType();
            model.LstStatusTypeUser = lstStatusType;
            model.LstStatusTypeAssignedUser = new List<SelectListItem>();
            return View(model);
        }

        /// <summary>
        /// bind data of the fuel source.
        /// </summary>
        /// <returns>ActionResult</returns>
        public ActionResult _FuelSource()
        {
            Report model = new Report();
            var lstFuelSource = _reportService.GetFuelSource();
            model.LstFuelSourceUser = lstFuelSource;
            model.LstFuelSourceAssignedUser = new List<SelectListItem>();
            return View(model);
        }
        #endregion

        #region Method

        /// <summary>
        /// Gets the report list.
        /// </summary>
        /// <param name="reportModel">The report model.</param>
        /// <returns>JsonResult</returns>
        public JsonResult GetReportList(Report reportModel)
        {
            IList<Report> lstUser = _reportService.GetReportList(reportModel);
            return new JsonResult()
            {
                Data = lstUser,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
            //return Json(lstUser, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the report.
        /// </summary>
        /// <param name="reportID">The report identifier.</param>
        /// <returns>JsonResult</returns>
        public JsonResult GetReport(int? reportID)
        {
            Report model = new Report();
            var dsReportList = _reportService.GetReport(reportID);
            model.reportList = DBClient.ToListof<Report>(dsReportList.Tables[0]);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the report DRP.
        /// </summary>
        /// <returns>JsonResult</returns>
        public JsonResult GetReportDrp()
        {
            List<SelectListItem> items = _reportService.GetReportDrp().Select(a => new SelectListItem { Text = a.ReportName, Value = a.ReportID.ToString() }).ToList();
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// BTNs the export_ click.
        /// </summary>
        /// <param name="reportID">The report identifier.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="DateGroup">The date group.</param>
        /// <param name="actionType">Type of the action.</param>
        /// <param name="ownerAccount">The owner account.</param>
        /// <param name="fuelSource">The fuel source.</param>
        /// <param name="status">The status.</param>
        [HttpGet]
        public void btnExport_Click(string reportID, string startDate, string endDate, string fileName, string DateGroup, string actionType, string ownerAccount, string fuelSource, string status)
        {
            string filename = "ReportExcel.xls";
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
            Response.Clear();

            InitializeWorkbook();
            Report reportModel = new Report();
            reportModel.ReportID = Convert.ToInt32(reportID);
            reportModel.StartDate = Convert.ToDateTime(startDate);
            reportModel.EndDate = Convert.ToDateTime(endDate);
            reportModel.DateGrouping = Convert.ToInt32(DateGroup);
            reportModel.hdnRECActionTypeAssignedUser = actionType;
            reportModel.hdnRECOwnerAccountAssignedUser = ownerAccount;
            reportModel.hdnRECStatusTypeAssignedUser = fuelSource;
            reportModel.hdnRECFuelSourceAssignedUser = status;
            GenerateData(reportModel);
            GetExcelStream().WriteTo(Response.OutputStream);
            Response.End();
        }

        HSSFWorkbook hssfworkbook;

        /// <summary>
        /// Initializes the workbook.
        /// </summary>
        void InitializeWorkbook()
        {
            hssfworkbook = new HSSFWorkbook();

            ////create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hssfworkbook.DocumentSummaryInformation = dsi;

            ////create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            hssfworkbook.SummaryInformation = si;
        }

        /// <summary>
        /// Generates the data.
        /// </summary>
        /// <param name="reportModel">The report model.</param>
        void GenerateData(Report reportModel)
        {
            DataSet ds = _reportService.GetExcelData(reportModel);
            DataTable dtUsers = ds.Tables[0];

            ISheet sheet1 = hssfworkbook.CreateSheet("CheckIn Dashboard");
            ICellStyle cellStyle = GetDateCellStyle(hssfworkbook);

            // IWorkbook doc
            IFont font = hssfworkbook.CreateFont();
            font.FontHeightInPoints = 10;
            font.FontName = "Callibri";
            font.Boldweight = (short)FontBoldWeight.BOLD;

            IRow Headerrow1 = sheet1.CreateRow(0);

            var cell = Headerrow1.CreateCell(0);
            cell.CellStyle = hssfworkbook.CreateCellStyle();
            cell.CellStyle.SetFont(font);
            if (reportModel.ReportID == 1 || reportModel.ReportID == 2 || reportModel.ReportID == 3 || reportModel.ReportID == 4 || reportModel.ReportID == 5 || reportModel.ReportID == 7 || reportModel.ReportID == 8 || reportModel.ReportID == 9 || reportModel.ReportID == 11)
            {
                cell.SetCellValue("Date");
            }
            if (reportModel.ReportID == 10 || reportModel.ReportID == 12 || reportModel.ReportID == 6)
            {
                cell.SetCellValue("Owner Account");
            }

            var cell1 = Headerrow1.CreateCell(1);
            if (reportModel.ReportID == 1)
            {
                cell1.SetCellValue("Action Type");
            }
            if (reportModel.ReportID == 4)
            {
                cell1.SetCellValue("Selling Account");
            }
            if (reportModel.ReportID == 8)
            {
                cell1.SetCellValue("Buying Account");
            }
            if (reportModel.ReportID == 5 || reportModel.ReportID == 6)
            {
                cell1.SetCellValue("State");
            }
            if (reportModel.ReportID == 9)
            {
                cell1.SetCellValue("Status");
            }
            if (reportModel.ReportID == 2 || reportModel.ReportID == 3 || reportModel.ReportID == 7 || reportModel.ReportID == 11)
            {
                cell1.SetCellValue("Owner Account");
            }
            if (reportModel.ReportID == 10 || reportModel.ReportID == 12)
            {
                cell1.SetCellValue("STCs Created");
            }
            cell1.CellStyle = hssfworkbook.CreateCellStyle();
            cell1.CellStyle.SetFont(font);

            var cell2 = Headerrow1.CreateCell(2);
            if (reportModel.ReportID == 1 || reportModel.ReportID == 2 || reportModel.ReportID == 6)
            {
                cell2.SetCellValue("REC Count");
            }
            if (reportModel.ReportID == 3)
            {
                cell2.SetCellValue("RECsBought");
            }
            if (reportModel.ReportID == 4)
            {
                cell2.SetCellValue("Buying Account");
            }
            if (reportModel.ReportID == 8)
            {
                cell2.SetCellValue("Selling Account");
            }
            if (reportModel.ReportID == 7)
            {
                cell2.SetCellValue("RECs Surrendered");
            }
            if (reportModel.ReportID == 5 || reportModel.ReportID == 11)
            {
                cell2.SetCellValue("STCs Created");
            }
            if (reportModel.ReportID == 9)
            {
                cell2.SetCellValue("STCs Count");
            }
            cell2.CellStyle = hssfworkbook.CreateCellStyle();
            cell2.CellStyle.SetFont(font);
            var cell3 = Headerrow1.CreateCell(3);
            cell3.CellStyle = hssfworkbook.CreateCellStyle();
            cell3.CellStyle.SetFont(font);
            if (reportModel.ReportID == 4 || reportModel.ReportID == 8)
            {
                cell3.SetCellValue("REC Count");
            }
            sheet1.SetDefaultColumnStyle(4, cellStyle);

            for (int i = 1; i <= dtUsers.Rows.Count; i++)
            {
                IRow row = sheet1.CreateRow(i);

                if (reportModel.ReportID == 1 || reportModel.ReportID == 2 || reportModel.ReportID == 3 || reportModel.ReportID == 4 || reportModel.ReportID == 5 || reportModel.ReportID == 7 || reportModel.ReportID == 8 || reportModel.ReportID == 9 || reportModel.ReportID == 11)
                {
                    row.CreateCell(0).SetCellValue(Convert.ToDateTime(dtUsers.Rows[i - 1]["Date"].ToString()).ToString("dd/MM/yyyy"));
                }
                if (reportModel.ReportID == 10 || reportModel.ReportID == 12)
                {
                    row.CreateCell(0).SetCellValue(dtUsers.Rows[i - 1]["OwnerAccount"].ToString());
                }

                if (reportModel.ReportID == 1)
                {
                    row.CreateCell(1).SetCellValue(dtUsers.Rows[i - 1]["ActionType"].ToString());
                }
                if (reportModel.ReportID == 5)
                {
                    row.CreateCell(1).SetCellValue(dtUsers.Rows[i - 1]["State"].ToString());
                }
                if (reportModel.ReportID == 2 || reportModel.ReportID == 3 || reportModel.ReportID == 7 || reportModel.ReportID == 11)
                {
                    row.CreateCell(1).SetCellValue(dtUsers.Rows[i - 1]["OwnerAccount"].ToString());
                }
                if (reportModel.ReportID == 9)
                {
                    row.CreateCell(1).SetCellValue(dtUsers.Rows[i - 1]["Status"].ToString());
                }
                if (reportModel.ReportID == 10 || reportModel.ReportID == 12)
                {
                    row.CreateCell(1).SetCellValue(dtUsers.Rows[i - 1]["RECCOUNT"].ToString());
                }

                if (reportModel.ReportID == 1 || reportModel.ReportID == 2 || reportModel.ReportID == 3 || reportModel.ReportID == 5 || reportModel.ReportID == 7 || reportModel.ReportID == 9 || reportModel.ReportID == 11)
                {
                    row.CreateCell(2).SetCellValue(dtUsers.Rows[i - 1]["RECCOUNT"].ToString());
                }

                if (reportModel.ReportID == 4)
                {
                    row.CreateCell(1).SetCellValue(dtUsers.Rows[i - 1]["SellingAccount"].ToString());
                    row.CreateCell(2).SetCellValue(dtUsers.Rows[i - 1]["BuyingAccount"].ToString());
                    row.CreateCell(3).SetCellValue(dtUsers.Rows[i - 1]["RECCOUNT"].ToString());
                }
                if (reportModel.ReportID == 8)
                {
                    row.CreateCell(1).SetCellValue(dtUsers.Rows[i - 1]["BuyingAccount"].ToString());
                    row.CreateCell(2).SetCellValue(dtUsers.Rows[i - 1]["SellingAccount"].ToString());
                    row.CreateCell(3).SetCellValue(dtUsers.Rows[i - 1]["RECCOUNT"].ToString());
                }
                if (reportModel.ReportID == 6)
                {
                    row.CreateCell(0).SetCellValue(dtUsers.Rows[i - 1]["OwnerAccount"].ToString());
                    row.CreateCell(1).SetCellValue(dtUsers.Rows[i - 1]["Status"].ToString());
                    row.CreateCell(2).SetCellValue(dtUsers.Rows[i - 1]["RECCOUNT"].ToString());
                }
            }
        }

        /// <summary>
        /// Gets the excel stream.
        /// </summary>
        /// <returns>MemoryStream</returns>
        MemoryStream GetExcelStream()
        {
            //Write the stream data of workbook to the root directory
            MemoryStream file = new MemoryStream();
            hssfworkbook.Write(file);
            return file;
        }

        /// <summary>
        /// Gets the date cell style.
        /// </summary>
        /// <param name="hssfworkbook">The hssfworkbook.</param>
        /// <returns>ICellStyle</returns>
        public static ICellStyle GetDateCellStyle(HSSFWorkbook hssfworkbook)
        {
            ICellStyle cellStyle = hssfworkbook.CreateCellStyle();
            cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("m/d/yy");
            return cellStyle;
        }

        #endregion
    }
}
using Excel;
using FormBot.DAL;
using FormBot.Entity;
using FormBot.Helper;
using FormBot.Helper.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace FormBot.BAL.Service
{
    public class CERImportBAL : ICERImportBAL
    {
        #region Declaration

        bool isExcel = false;

        #endregion

        /// <summary>
        /// Read excel file from given path and execute procedure
        /// </summary>
        /// <param name="excelStream">The excel stream.</param>
        /// <param name="fileType">file type</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="version">The version.</param> 
        /// <param name="subtype">The subtype.</param> 
        /// <returns>if error not generate than return true</returns>
        public string MergeDataTable(Stream excelStream, SystemEnums.CERType fileType, string filePath, string version, SystemEnums.CERSubType subtype = 0)
        {
            try
            {
                DataTable mergeDataTable = new DataTable();

                string procedureName = string.Empty;
                switch (fileType)
                {
                    case SystemEnums.CERType.PhotovoltaicModules:
                        mergeDataTable = this.GetDataTableFromSpreadsheetModules(filePath);
                        procedureName = "MergePVModules";
                        break;

                    case SystemEnums.CERType.BatteryStorage:
                        //mergeDataTable = this.GetDataTableFromPdfBatteryStorage(filePath);
                        mergeDataTable = this.GetDataTableFromSpreadsheetBatteryStorage(filePath);
                        procedureName = "MergeBatteryStorage";
                        break;

                    case SystemEnums.CERType.ApprovedInverters:
                        mergeDataTable = this.GetDataTableFromSpreadsheetInverter(filePath);
                        procedureName = "MergeInverters";
                        break;

                    case SystemEnums.CERType.AccreditedInstallers:
                        mergeDataTable = this.GetDataTableFromSpreadsheetAccreditedInstallers(filePath);
                        procedureName = "MergeAccreditedInstallers";
                        break;

                    case SystemEnums.CERType.HWBrandModel:
                        mergeDataTable = this.GetDataTableFromSpreadsheetHWBrandModel(filePath);
                        procedureName = "MergeHWBrandModel";
                        break;

                    case SystemEnums.CERType.ElectricityProvider:
                        mergeDataTable = this.GetDataTableFromSpreadsheetElectricityProvider(filePath);
                        procedureName = "MergeElectricityProvider";
                        break;
                    case SystemEnums.CERType.SerialNumbers:
                        this.GetDataTableFromSpreadsheetSerialNumber(filePath);
                        return "true";
                    default:
                        break;
                }

                if (mergeDataTable.Columns.Count > 1 && mergeDataTable.Rows.Count > 0)
                {
                    object result = CommonDAL.MergeDataTable(mergeDataTable, procedureName, version, (int)fileType, ProjectSession.LoggedInUserId, isExcel, (int)subtype);
                    int num;
                    bool isNum = int.TryParse(result.ToString(), out num);
                    return (isNum && (int)result > 0) ? "true" : result.ToString();
                    //return (int)result > 0 ? "true" : Convert.ToString(result);
                }
                else
                {
                    return mergeDataTable.Rows[0][0].ToString();
                }
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return ex.Message;
            }
        }

        /// <summary>
        /// Gets the data table from spreadsheet inverter.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns> read excel file in DataTable </returns>
        public DataTable GetDataTableFromSpreadsheetInverter(string filePath)
        {
            DataTable dataTable = new DataTable();
            int colIndex = 1;
            try
            {
                FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
                IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                if (!excelReader.IsValid)
                {
                    stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
                    excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }

                excelReader.IsFirstRowAsColumnNames = false;
                DataSet result = excelReader.AsDataSet();

                dataTable.Columns.Add("Manufacturer", typeof(string));
                dataTable.Columns.Add("Series", typeof(string));
                dataTable.Columns.Add("ModelNumber", typeof(string));
                dataTable.Columns.Add("AcPowerKW", typeof(string));
                dataTable.Columns.Add("ApprovalDate", typeof(string));
                dataTable.Columns.Add("ExpiryDate", typeof(string));
                dataTable.Columns.Add("CreatedBy", typeof(Int32));
                if (result != null && result.Tables.Count > 0)
                {
                    foreach (DataRow excelRow in result.Tables[0].Rows)
                    {
                        if (!string.IsNullOrEmpty(Convert.ToString(excelRow[0])) && !string.IsNullOrEmpty(Convert.ToString(excelRow[5])))
                        {
                            DataRow dataRow = dataTable.NewRow();
                            colIndex = 1;
                            if (!string.IsNullOrEmpty(excelRow[0].ToString()))
                                dataRow[0] = excelRow[0];
                            else
                                throw new System.InvalidOperationException("Value is null");

                            colIndex++;
                            if (!string.IsNullOrEmpty(excelRow[1].ToString()))
                                dataRow[1] = excelRow[1];
                            else
                                throw new System.InvalidOperationException("Value is null");

                            colIndex++;
                            if (!string.IsNullOrEmpty(excelRow[2].ToString()))
                                dataRow[2] = excelRow[2];
                            else
                                throw new System.InvalidOperationException("Value is null");

                            colIndex++;
                            if (!string.IsNullOrEmpty(excelRow[3].ToString()))
                                dataRow[3] = excelRow[3];
                            else
                                throw new System.InvalidOperationException("Value is null");

                            colIndex++;
                            double value;
                            DateTime date;
                            if (!string.IsNullOrEmpty(Convert.ToString(excelRow[4])))
                            {
                                if (DateTime.TryParse(excelRow[4].ToString(), out date))
                                {
                                    dataRow[4] = date.ToString("yyyyMMdd");
                                }
                                else
                                {
                                    if (double.TryParse(excelRow[4].ToString(), out value))
                                    {
                                        dataRow[4] = DateTime.FromOADate(double.Parse(Convert.ToString(excelRow[4]))).ToString("yyyyMMdd");
                                    }
                                    else
                                    {
                                        //for error creating
                                        dataRow[4] = DBNull.Value;
                                    }
                                }
                                //bool isDouble = Double.TryParse(excelRow[4].ToString(), out value);
                                //if (isDouble)
                                //{
                                //    dataRow[4] = DateTime.FromOADate(double.Parse(Convert.ToString(excelRow[4]))).ToString("yyyyMMdd");
                                //}
                                //else
                                //{
                                //    dataRow[4] = Convert.ToDateTime(excelRow[4]).ToString("yyyyMMdd");
                                //}
                            }
                            else
                                throw new System.InvalidOperationException("Value is null");

                            colIndex++;
                            if (!string.IsNullOrEmpty(Convert.ToString(excelRow[5])))
                            {
                                if (DateTime.TryParse(excelRow[5].ToString(), out date))
                                {
                                    dataRow[5] = date.ToString("yyyyMMdd"); ;
                                }
                                else
                                {
                                    if (double.TryParse(excelRow[5].ToString(), out value))
                                    {
                                        dataRow[5] = DateTime.FromOADate(double.Parse(Convert.ToString(excelRow[5]))).ToString("yyyyMMdd");
                                    }
                                    else
                                    {
                                        //for error creating
                                        dataRow[5] = DBNull.Value;
                                    }
                                }
                                //bool isDouble = Double.TryParse(excelRow[5].ToString(), out value);
                                //if (isDouble)
                                //{
                                //    dataRow[5] = DateTime.FromOADate(double.Parse(Convert.ToString(excelRow[5]))).ToString("yyyyMMdd");
                                //}
                                //else
                                //{
                                //    dataRow[5] = Convert.ToDateTime(excelRow[5]).ToString("yyyyMMdd");
                                //}
                            }
                            else
                                throw new System.InvalidOperationException("Value is null");

                            colIndex++;
                            dataRow[6] = ProjectSession.LoggedInUserId;
                            dataTable.Rows.Add(dataRow);
                        }
                    }
                    if (dataTable.Rows.Count > 0)
                    {
                        dataTable.Rows.RemoveAt(0);
                    }
                }

                return dataTable;
            }
            catch (Exception ex)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("ErrorMsg", typeof(string));
                DataRow dr = dt.NewRow();
                dr["ErrorMsg"] = dataTable != null ? ex.Message + " at Row: " + (dataTable.Rows.Count + 2) + " and Column: " + colIndex : ex.Message;
                dt.Rows.Add(dr);
                return dt;
            }
        }

        /// <summary>
        /// Gets the data table from spreadsheet modules.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>read excel file in DataTable</returns>
        public DataTable GetDataTableFromSpreadsheetModules(string filePath)
        {
            int colIndex = 1;
            DataTable dataTable = new DataTable();
            try
            {
                FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
                IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);

                if (!excelReader.IsValid)
                {
                    stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
                    excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }

                excelReader.IsFirstRowAsColumnNames = false;
                DataSet result = excelReader.AsDataSet();

                dataTable.Columns.Add("CertificateHolder", typeof(string));
                dataTable.Columns.Add("ModelNumber", typeof(string));
                dataTable.Columns.Add("Wattage", typeof(string));
                dataTable.Columns.Add("CECApprovedDate", typeof(string));
                dataTable.Columns.Add("ExpiryDate", typeof(string));
                dataTable.Columns.Add("FireTested", typeof(string));
                dataTable.Columns.Add("CreatedBy", typeof(Int32));
                double decimalNumber = 0;
                if (result != null && result.Tables.Count > 0)
                {
                    if (result.Tables[0].Columns.Count == 5)
                    {
                        foreach (DataRow excelRow in result.Tables[0].Rows)
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(excelRow[0])) && !string.IsNullOrEmpty(Convert.ToString(excelRow[4])))
                            {
                                DataRow dataRow = dataTable.NewRow();

                                colIndex = 1;
                                if (!string.IsNullOrEmpty(excelRow[0].ToString()))
                                    dataRow[0] = excelRow[0];
                                else
                                    throw new System.InvalidOperationException("Value is null");

                                colIndex++;
                                dataRow[1] = excelRow[1];
                                dataRow[2] = Common.GetWattageFromModelNumber(Convert.ToString(excelRow[1]));

                                colIndex++;
                                double value;
                                DateTime date;
                                if (DateTime.TryParse(excelRow[2].ToString(), out date))
                                {
                                    dataRow[3] = date.ToString("yyyyMMdd");
                                }
                                else
                                {
                                    if (double.TryParse(excelRow[2].ToString(), out value))
                                    {
                                        dataRow[3] = DateTime.FromOADate(double.Parse(Convert.ToString(excelRow[2]))).ToString("yyyyMMdd");
                                    }
                                    else
                                    {
                                        //for error creating
                                        dataRow[3] = DBNull.Value;
                                    }
                                }
                                colIndex++;
                                if (DateTime.TryParse(excelRow[3].ToString(), out date))
                                {
                                    dataRow[4] = date.ToString("yyyyMMdd");
                                }
                                else
                                {
                                    if (double.TryParse(excelRow[3].ToString(), out value))
                                    {
                                        dataRow[4] = DateTime.FromOADate(double.Parse(Convert.ToString(excelRow[3]))).ToString("yyyyMMdd");
                                    }
                                    else
                                    {
                                        //for error creating
                                        dataRow[4] = DBNull.Value;
                                    }
                                }
                                colIndex++;
                                dataRow[5] = excelRow[4];

                                colIndex++;
                                dataRow[6] = ProjectSession.LoggedInUserId;
                                dataTable.Rows.Add(dataRow);
                            }
                        }
                        if (dataTable.Rows.Count > 0)
                        {
                            dataTable.Rows.RemoveAt(0);
                        }
                    }
                    else if (result.Tables[0].Columns.Count == 6)
                    {
                        isExcel = true;
                        foreach (DataRow excelRow in result.Tables[0].Rows)
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(excelRow[0])) && !string.IsNullOrEmpty(Convert.ToString(excelRow[5])))
                            {
                                DataRow dataRow = dataTable.NewRow();

                                colIndex = 1;
                                if (!string.IsNullOrEmpty(excelRow[0].ToString()))
                                    dataRow[0] = excelRow[0];
                                else
                                    throw new System.InvalidOperationException("Value is null");

                                colIndex++;
                                dataRow[1] = excelRow[1];

                                colIndex++;
                                dataRow[2] = Common.GetWattageFromModelNumber(Convert.ToString(excelRow[1]));

                                colIndex++;
                                DateTime approvedDate;
                                if (DateTime.TryParseExact(excelRow[3].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out approvedDate))
                                {
                                    dataRow[3] = approvedDate.ToString("yyyyMMdd");
                                }
                                else
                                {
                                    if (double.TryParse(excelRow[3].ToString(), out decimalNumber))
                                    {
                                        dataRow[3] = DateTime.FromOADate(double.Parse(Convert.ToString(excelRow[3])));
                                    }
                                    else
                                    {
                                        //for error creating
                                        dataRow[3] = DBNull.Value;
                                    }
                                }

                                colIndex++;
                                DateTime expiryDate;
                                if (DateTime.TryParseExact(excelRow[4].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out expiryDate))
                                {
                                    dataRow[4] = expiryDate.ToString("yyyyMMdd");
                                }
                                else
                                {
                                    if (double.TryParse(excelRow[4].ToString(), out decimalNumber))
                                    {
                                        dataRow[4] = DateTime.FromOADate(double.Parse(Convert.ToString(excelRow[4])));
                                    }
                                    else
                                    {
                                        //for error creating
                                        dataRow[4] = DBNull.Value;
                                    }
                                }

                                colIndex++;
                                dataRow[5] = excelRow[5];

                                colIndex++;
                                dataRow[6] = ProjectSession.LoggedInUserId;
                                dataTable.Rows.Add(dataRow);
                            }
                        }
                        if (dataTable.Rows.Count > 0)
                        {
                            dataTable.Rows.RemoveAt(0);
                        }
                    }
                }
                return dataTable;
            }
            catch (Exception ex)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("ErrorMsg", typeof(string));
                DataRow dr = dt.NewRow();
                dr["ErrorMsg"] = dataTable != null ? ex.Message + " at Row: " + (dataTable.Rows.Count + 2) + " and Column: " + colIndex : ex.Message;
                dt.Rows.Add(dr);
                return dt;
            }
        }

        /// <summary>
        /// Gets the data table from spreadsheet battery storage.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        public DataTable GetDataTableFromPdfBatteryStorage(string Filename)
        {
            DataTable dt = dtBatteryStorage();
            string pathToPdf = Filename;
            string pathToExcel = System.IO.Path.ChangeExtension(pathToPdf, ".xls");

            // Convert PDF file to Excel file
            SautinSoft.PdfFocus f = new SautinSoft.PdfFocus();
            f.ExcelOptions.ConvertNonTabularDataToSpreadsheet = true;
            f.ExcelOptions.SingleSheet = false;
            f.ExcelOptions.PreservePageLayout = true;

            // The information includes the names for the culture, the writing system, 
            // the calendar used, the sort order of strings, and formatting for dates and numbers.
            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");
            ci.NumberFormat.NumberDecimalSeparator = ",";
            ci.NumberFormat.NumberGroupSeparator = ".";
            f.ExcelOptions.CultureInfo = ci;

            f.OpenPdf(pathToPdf);

            if (f.PageCount > 0)
            {
                for (var i = 1; i <= f.PageCount; i++)
                {
                    //Converting single pdf page to xls
                    f.ToExcel(pathToExcel, i, i);
                    DataTable dtExcel = GetDataTableFromSpreadsheetBatteryStorage(pathToExcel);
                    foreach (DataRow dr in dtExcel.Rows)
                    {
                        dt.Rows.Add(dr.ItemArray);
                    }
                    if (System.IO.File.Exists(pathToExcel))
                    {
                        System.IO.File.Delete(pathToExcel);
                    }
                }
            }
            f.ClosePdf();
            return dt;
        }

        /// <summary>
        /// Gets the data table from spreadsheet battery storage.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        public DataTable GetDataTableFromSpreadsheetBatteryStorage(string filePathOriginal)
        {
            string filePath = "D:\\Projects\\FormBot\\SourceCode\\FormBot01082017\\FormBot.Main\\CERFiles\\CEC_AccreditedBatteries_220912.xlsx";
            DataTable dataTable = new DataTable();
            //DataTable dataTable = dtBattery();
            int colIndex = 1;
            try
            {

                FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
                IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                if (!excelReader.IsValid)
                {
                    stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
                    excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }

                excelReader.IsFirstRowAsColumnNames = false;
                DataSet result = excelReader.AsDataSet();

                foreach (DataTable dt in result.Tables)
                {
                    if (dt != null && dt.Rows.Count > 0 && dt.Columns.Count > 2)
                    {
                        DataColumn col = new DataColumn();
                        col.DefaultValue = DateTime.Now;
                        dt.Columns.Add(col);
                        col = new DataColumn();
                        col.DefaultValue = ProjectSession.LoggedInUserId;
                        dt.Columns.Add(col);

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            DateTime date;
                            double decimalNumber = 0;

                            ////Removing empty rows
                            //if (dt.Rows[i][0] == DBNull.Value || dt.Rows[i][1] == DBNull.Value || dt.Rows[i][0].ToString().ToLower() == "equipment category")
                            if (dt.Rows[i][0] == DBNull.Value || dt.Rows[i][1] == DBNull.Value)
                            {
                                dt.Rows[i].Delete();
                            }
                            else
                            {
                                for (int j = 0; j < dt.Columns.Count; j++)
                                {
                                    if (dt.Rows[i][0].ToString().ToLower() == "equipment category")
                                    {
                                        dt.Rows[i][j] = Convert.ToString(dt.Rows[i][j]);
                                    }
                                    else
                                    {
                                        if (j == 8)
                                        {
                                            if (string.IsNullOrEmpty(dt.Rows[i][j].ToString()) || dt.Rows[i][j].ToString() == "-")
                                            {
                                                dt.Rows[i][j] = Convert.ToDecimal(0);
                                            }
                                            else
                                            {
                                                dt.Rows[i][j] = Convert.ToDecimal(dt.Rows[i][j]);
                                            }
                                        }
                                        else if (j == 9)
                                        {
                                            dt.Rows[i][j] = Convert.ToDecimal(dt.Rows[i][j]);
                                        }
                                        else if (j == 10)
                                        {
                                            dt.Rows[i][j] = Convert.ToInt16(dt.Rows[i][j]);
                                        }
                                        else if (j == 11)
                                        {
                                            dt.Rows[i][j] = Convert.ToDecimal(dt.Rows[i][j]);
                                        }
                                        else if (j == 13)
                                        {
                                            dt.Rows[i][j] = Convert.ToInt16(dt.Rows[i][j]);
                                        }
                                        else if (j == 14)
                                        {
                                            dt.Rows[i][j] = Convert.ToInt16(dt.Rows[i][j]);
                                        }
                                        else if (j == 16)
                                        {
                                            if (DateTime.TryParse(dt.Rows[i][j].ToString(), out date))
                                            {
                                                dt.Rows[i][j] = date;
                                            }
                                            else
                                            {
                                                if (double.TryParse(dt.Rows[i][j].ToString(), out decimalNumber))
                                                {
                                                    dt.Rows[i][j] = DateTime.FromOADate(double.Parse(Convert.ToString(dt.Rows[i][j])));
                                                }
                                                else
                                                {
                                                    //for error creating
                                                    dt.Rows[i][j] = string.Empty;
                                                }
                                            }
                                            //dt.Rows[i][j] = Convert.ToDateTime(dt.Rows[i][j]);
                                        }
                                        else if (j == 17)
                                        {
                                            if (DateTime.TryParse(dt.Rows[i][j].ToString(), out date))
                                            {
                                                dt.Rows[i][j] = date;
                                            }
                                            else
                                            {
                                                if (double.TryParse(dt.Rows[i][j].ToString(), out decimalNumber))
                                                {
                                                    dt.Rows[i][j] = DateTime.FromOADate(double.Parse(Convert.ToString(dt.Rows[i][j])));
                                                }
                                                else
                                                {
                                                    //for error creating
                                                    dt.Rows[i][j] = string.Empty;
                                                }
                                            }
                                            //dt.Rows[i][j] = Convert.ToDateTime(dt.Rows[i][j]);
                                        }
                                        else if (j == 20)
                                        {
                                            dt.Rows[i][j] = Convert.ToDateTime(dt.Rows[i][j]);
                                        }
                                        else if (j == 21)
                                        {
                                            dt.Rows[i][j] = Convert.ToInt16(dt.Rows[i][j]);
                                        }
                                        else
                                        {
                                            dt.Rows[i][j] = Convert.ToString(dt.Rows[i][j]);
                                        }
                                    }
                                }
                                //dt.Rows[i][dt.Columns.Count] = DateTime.Now;
                                //dt.Rows[i][dt.Columns.Count + 1] = ProjectSession.LoggedInUserId;
                            }
                        }
                        dt.AcceptChanges();

                        //Removing empty columns
                        foreach (var column in dt.Columns.Cast<DataColumn>().ToArray())
                        {
                            if (string.IsNullOrEmpty(dt.Rows[0][column.ColumnName].ToString()))
                            {
                                dt.Columns.Remove(column);
                            }
                        }
                        dt.AcceptChanges();

                        //Removing first row(header)
                        dt.Rows[0].Delete();
                        dt.AcceptChanges();

                        dataTable.Merge(dt);
                    }
                }
                return dataTable;
            }
            catch (Exception ex)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("ErrorMsg", typeof(string));
                DataRow dr = dt.NewRow();
                dr["ErrorMsg"] = dataTable != null ? ex.Message + " at Row: " + (dataTable.Rows.Count + 2) + " and Column: " + colIndex : ex.Message;
                dt.Rows.Add(dr);
                return dt;
            }
        }


        ///// <summary>
        ///// Gets the data table from spreadsheet battery storage1 (xls file).
        ///// </summary>
        ///// <param name="filePath">The file path.</param>
        ///// <returns></returns>
        //public DataTable GetDataTableFromSpreadsheetBatteryStorage1(string filePath)
        //{
        //    DataTable dataTable = new DataTable();
        //    int colIndex = 1;
        //    try
        //    {

        //        FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
        //        IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
        //        if (!excelReader.IsValid)
        //        {
        //            stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
        //            excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        //        }

        //        excelReader.IsFirstRowAsColumnNames = false;
        //        DataSet result = excelReader.AsDataSet();


        //        dataTable.Columns.Add("Manufacturer", typeof(string));
        //        dataTable.Columns.Add("ModelNumber", typeof(string));
        //        dataTable.Columns.Add("CreatedDate", typeof(DateTime));
        //        dataTable.Columns.Add("CreatedBy", typeof(Int32));
        //        if (result != null && result.Tables.Count > 0)
        //        {
        //            if (result.Tables[0].Columns.Count == 2)
        //            {
        //                foreach (DataRow excelRow in result.Tables[0].Rows)
        //                {
        //                    if (!string.IsNullOrEmpty(excelRow[0].ToString()) && !string.IsNullOrEmpty(Convert.ToString(excelRow[1])))
        //                    {
        //                        DataRow dataRow = dataTable.NewRow();
        //                        colIndex = 1;
        //                        dataRow[0] = excelRow[0];
        //                        colIndex++;
        //                        dataRow[1] = excelRow[1];
        //                        colIndex++;
        //                        dataRow[2] = DateTime.Now;
        //                        colIndex++;
        //                        dataRow[3] = ProjectSession.LoggedInUserId;
        //                        dataTable.Rows.Add(dataRow);
        //                    }
        //                }
        //                if (dataTable.Rows.Count > 0)
        //                {
        //                    dataTable.Rows.RemoveAt(0);
        //                }

        //            }
        //        }

        //        return dataTable;
        //    }
        //    catch (Exception ex)
        //    {
        //        DataTable dt = new DataTable();
        //        dt.Columns.Add("ErrorMsg", typeof(string));
        //        DataRow dr = dt.NewRow();
        //        dr["ErrorMsg"] = dataTable != null ? ex.Message + " at Row: " + (dataTable.Rows.Count + 2) + " and Column: " + colIndex : ex.Message;
        //        dt.Rows.Add(dr);
        //        return dt;
        //    }
        //}

        /// <summary>
        /// Gets the data table from spreadsheet brand model.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>read excel file in DataTable</returns>
        public DataTable GetDataTableFromSpreadsheetHWBrandModel(string filePath)
        {
            DataTable dataTable = new DataTable();
            int colIndex = 1;
            try
            {
                FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
                IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                if (!excelReader.IsValid)
                {
                    stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
                    excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }

                excelReader.IsFirstRowAsColumnNames = false;
                DataSet result = excelReader.AsDataSet();

                dataTable.Columns.Add("Item", typeof(string));
                dataTable.Columns.Add("Brand", typeof(string));
                dataTable.Columns.Add("Model", typeof(string));
                dataTable.Columns.Add("EligibleFrom", typeof(DateTime));
                dataTable.Columns.Add("EligibleTo", typeof(DateTime));

                dataTable.Columns.Add("Zone1Certificates", typeof(string));
                dataTable.Columns.Add("Zone2Certificates", typeof(string));
                dataTable.Columns.Add("Zone3Certificates", typeof(string));
                dataTable.Columns.Add("Zone4Certificates", typeof(string));
                dataTable.Columns.Add("Zone5Certificates", typeof(string));
                dataTable.Columns.Add("CreatedBy", typeof(Int32));
                double decimalNumber = 0;
                if (result != null && result.Tables.Count > 0)
                {
                    foreach (DataRow excelRow in result.Tables[0].Rows)
                    {
                        DataRow dataRow = dataTable.NewRow();
                        DateTime date;
                        if (excelRow.ItemArray.Length > 9 && !string.IsNullOrEmpty(Convert.ToString(excelRow[0])) && !string.IsNullOrEmpty(Convert.ToString(excelRow[1])) && !string.IsNullOrEmpty(Convert.ToString(excelRow[9])))
                        {
                            for (int i = 0; i < excelRow.ItemArray.Length; i++)
                            {
                                colIndex = i + 1;
                                if (i == 3 || i == 4)
                                {
                                    if (DateTime.TryParse(excelRow[i].ToString(), out date))
                                    {
                                        dataRow[i] = date;
                                    }
                                    else
                                    {
                                        if (double.TryParse(excelRow[i].ToString(), out decimalNumber))
                                        {
                                            dataRow[i] = DateTime.FromOADate(double.Parse(Convert.ToString(excelRow[i])));
                                        }
                                        else
                                        {
                                            //for error creating
                                            dataRow[i] = DBNull.Value;
                                        }
                                    }
                                }
                                else
                                {
                                    //dataRow[i] = excelRow[i].ToString().Trim();
                                    if (!string.IsNullOrEmpty(excelRow[i].ToString()))
                                        dataRow[i] = excelRow[i];
                                    else
                                        throw new System.InvalidOperationException("Value is null");
                                }
                            }
                            //dataRow[0] = excelRow[0].ToString().Trim();
                            //dataRow[1] = excelRow[1].ToString().Trim();
                            //dataRow[2] = excelRow[2].ToString().Trim();

                            //if (DateTime.TryParse(excelRow[3].ToString(), out date))
                            //{
                            //    dataRow[3] = date;
                            //}
                            //else
                            //{
                            //    if (double.TryParse(excelRow[3].ToString(), out decimalNumber))
                            //    {
                            //        dataRow[3] = DateTime.FromOADate(double.Parse(Convert.ToString(excelRow[3])));
                            //    }
                            //    else
                            //    {
                            //        //for error creating
                            //        dataRow[3] = DBNull.Value;
                            //    }
                            //}

                            //if (DateTime.TryParse(excelRow[4].ToString(), out date))
                            //{
                            //    dataRow[4] = date;
                            //}
                            //else
                            //{
                            //    if (double.TryParse(excelRow[4].ToString(), out decimalNumber))
                            //    {
                            //        dataRow[4] = DateTime.FromOADate(double.Parse(Convert.ToString(excelRow[4])));
                            //    }
                            //    else
                            //    {
                            //        //for error creating
                            //        dataRow[4] = DBNull.Value;
                            //    }
                            //}

                            //dataRow[5] = excelRow[5];
                            //dataRow[6] = excelRow[6];
                            //dataRow[7] = excelRow[7];
                            //dataRow[8] = excelRow[8];
                            //dataRow[9] = excelRow[9];
                            dataRow[10] = ProjectSession.LoggedInUserId;
                            dataTable.Rows.Add(dataRow);
                        }
                        else if (excelRow.ItemArray.Length > 8 && !string.IsNullOrEmpty(Convert.ToString(excelRow[0])) && !string.IsNullOrEmpty(Convert.ToString(excelRow[1])) && !string.IsNullOrEmpty(Convert.ToString(excelRow[8])))
                        {
                            for (int i = 0; i < excelRow.ItemArray.Length; i++)
                            {
                                colIndex = i + 1;
                                if (i == 3 || i == 4)
                                {
                                    if (DateTime.TryParse(excelRow[i].ToString(), out date))
                                    {
                                        dataRow[i] = date;
                                    }
                                    else
                                    {
                                        if (double.TryParse(excelRow[i].ToString(), out decimalNumber))
                                        {
                                            dataRow[i] = DateTime.FromOADate(double.Parse(Convert.ToString(excelRow[i])));
                                        }
                                        else
                                        {
                                            //for error creating
                                            dataRow[i] = DBNull.Value;
                                        }
                                    }
                                }
                                else
                                {
                                    //dataRow[i] = excelRow[i].ToString().Trim();
                                    if (!string.IsNullOrEmpty(excelRow[i].ToString()))
                                        dataRow[i] = excelRow[i];
                                    else
                                        throw new System.InvalidOperationException("Value is null");

                                }
                            }
                            //dataRow[0] = excelRow[0];
                            //dataRow[1] = excelRow[1];
                            //dataRow[2] = excelRow[2];
                            //if (DateTime.TryParse(excelRow[3].ToString(), out date))
                            //{
                            //    dataRow[3] = date;
                            //}
                            //else
                            //{
                            //    if (double.TryParse(excelRow[3].ToString(), out decimalNumber))
                            //    {
                            //        dataRow[3] = DateTime.FromOADate(double.Parse(Convert.ToString(excelRow[3])));
                            //    }
                            //    else
                            //    {
                            //        //for error creating
                            //        dataRow[3] = DBNull.Value;
                            //    }
                            //}

                            //if (DateTime.TryParse(excelRow[4].ToString(), out date))
                            //{
                            //    dataRow[4] = date;
                            //}
                            //else
                            //{
                            //    if (double.TryParse(excelRow[4].ToString(), out decimalNumber))
                            //    {
                            //        dataRow[4] = DateTime.FromOADate(double.Parse(Convert.ToString(excelRow[4])));
                            //    }
                            //    else
                            //    {
                            //        //for error creating
                            //        dataRow[4] = DBNull.Value;
                            //    }
                            //}

                            //dataRow[5] = excelRow[5];
                            //dataRow[6] = excelRow[6];
                            //dataRow[7] = excelRow[7];
                            //dataRow[8] = excelRow[8];
                            dataRow[10] = ProjectSession.LoggedInUserId;
                            dataTable.Rows.Add(dataRow);
                        }
                    }

                    if (dataTable.Rows.Count > 0)
                    {
                        dataTable.Rows.RemoveAt(0);
                    }
                }

                return dataTable;
            }
            catch (Exception ex)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("ErrorMsg", typeof(string));
                DataRow dr = dt.NewRow();
                dr["ErrorMsg"] = dataTable != null ? ex.Message + " at Row: " + (dataTable.Rows.Count) + " and Column: " + colIndex : ex.Message;
                dt.Rows.Add(dr);
                return dt;
            }
        }

        /// <summary>
        /// Gets the data table from spreadsheet accredited installers.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>read excel file in DataTable</returns>
        public DataTable GetDataTableFromSpreadsheetAccreditedInstallers(string filePath)
        {
            DataTable dataTable = new DataTable();
            int colIndex = 1;
            try
            {
                FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);

                IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);

                if (!excelReader.IsValid)
                {
                    stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
                    excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }

                excelReader.IsFirstRowAsColumnNames = false;
                DataSet result = excelReader.AsDataSet();
                dataTable.Columns.Add("SyncAccreditedInstallerId", typeof(string));
                dataTable.Columns.Add("ContactSNo", typeof(string));
                dataTable.Columns.Add("InstallerStatus", typeof(string));
                dataTable.Columns.Add("AccreditationNumber", typeof(string));
                dataTable.Columns.Add("FirstName", typeof(string));
                dataTable.Columns.Add("LastName", typeof(string));
                dataTable.Columns.Add("ConcatenatedMailAddress", typeof(string));
                dataTable.Columns.Add("MailingAddressUnitType", typeof(string));
                dataTable.Columns.Add("MailingAddressUnitNumber", typeof(string));
                dataTable.Columns.Add("MailingAddressStreetNumber", typeof(string));
                dataTable.Columns.Add("MailingAddressStreetName", typeof(string));
                dataTable.Columns.Add("MailingAddressStreetType", typeof(string));
                dataTable.Columns.Add("MailingCity", typeof(string));
                dataTable.Columns.Add("MailingState", typeof(string));
                dataTable.Columns.Add("PostalCode", typeof(string));
                dataTable.Columns.Add("MailingCountry", typeof(string));
                dataTable.Columns.Add("Phone", typeof(string));
                dataTable.Columns.Add("Fax", typeof(string));
                dataTable.Columns.Add("Mobile", typeof(string));
                dataTable.Columns.Add("Email", typeof(string));
                dataTable.Columns.Add("GridType", typeof(string));
                dataTable.Columns.Add("SPS", typeof(string));
                dataTable.Columns.Add("InstallerFullAwardDate", typeof(string));
                dataTable.Columns.Add("InstallerProvisionalAwardDate", typeof(string));
                dataTable.Columns.Add("InstallerExpiryDate", typeof(string));
                dataTable.Columns.Add("SuspensionStartDate", typeof(string));
                dataTable.Columns.Add("SuspensionEndDate", typeof(string));
                dataTable.Columns.Add("LicensedElectricianNumber", typeof(string));
                dataTable.Columns.Add("Endorsements", typeof(string));
                dataTable.Columns.Add("CreatedBy", typeof(Int32));
                dataTable.Columns.Add("AccountName", typeof(string));



                if (result != null && result.Tables.Count > 0)
                {
                    foreach (DataRow excelRow in result.Tables[0].Rows)
                    {
                        if (!string.IsNullOrEmpty(excelRow[1].ToString()))
                        {
                            DataRow dataRow = dataTable.NewRow();
                            DateTime date;
                            double decimalNumber;
                            for (int i = 0; i < 27; i++)
                            {
                                colIndex = i + 1;
                                if (i == 22 || i == 23 || i == 24 || i == 25 || i == 26)
                                {
                                    if (!string.IsNullOrEmpty(Convert.ToString(excelRow[i]).Trim()))
                                    {
                                        if (DateTime.TryParse(excelRow[i].ToString(), out date))
                                        {
                                            dataRow[i] = (date.ToString("yyyyMMdd"));
                                        }
                                        else
                                        {
                                            if (double.TryParse(excelRow[i].ToString(), out decimalNumber))
                                            {
                                                dataRow[i] = DateTime.FromOADate(double.Parse(Convert.ToString(excelRow[i]))).ToString("yyyyMMdd");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        dataRow[i] = DBNull.Value;
                                    }
                                }
                                else
                                {
                                    dataRow[i] = Convert.ToString(excelRow[i]).Trim();
                                }
                            }
                            dataRow[27] = ProjectSession.LoggedInUserId;
                            dataTable.Rows.Add(dataRow);
                        }
                        //if (!string.IsNullOrEmpty(Convert.ToString(excelRow[1]).Trim()))
                        //{
                        //    DataRow dataRow = dataTable.NewRow();
                        //    dataRow[0] = Convert.ToString(excelRow[0]).Trim();
                        //    dataRow[1] = Convert.ToString(excelRow[1]).Trim();
                        //    dataRow[2] = Convert.ToString(excelRow[2]).Trim();
                        //    dataRow[3] = Convert.ToString(excelRow[3]).Trim();
                        //    dataRow[4] = Convert.ToString(excelRow[4]).Trim();
                        //    dataRow[5] = Convert.ToString(excelRow[5]).Trim();
                        //    dataRow[6] = Convert.ToString(excelRow[6]).Trim();
                        //    dataRow[7] = Convert.ToString(excelRow[7]).Trim();
                        //    dataRow[8] = Convert.ToString(excelRow[8]).Trim();
                        //    dataRow[9] = Convert.ToString(excelRow[9]).Trim();
                        //    dataRow[10] = Convert.ToString(excelRow[10]).Trim();
                        //    dataRow[11] = Convert.ToString(excelRow[11]).Trim();
                        //    dataRow[12] = Convert.ToString(excelRow[12]).Trim();
                        //    dataRow[13] = Convert.ToString(excelRow[13]).Trim();
                        //    dataRow[14] = Convert.ToString(excelRow[14]).Trim();
                        //    dataRow[15] = Convert.ToString(excelRow[15]).Trim();
                        //    dataRow[16] = Convert.ToString(excelRow[16]).Trim();
                        //    dataRow[17] = Convert.ToString(excelRow[17]).Trim();
                        //    dataRow[18] = Convert.ToString(excelRow[18]).Trim();
                        //    dataRow[19] = Convert.ToString(excelRow[19]).Trim();

                        //    DateTime date;
                        //    double decimalNumber;
                        //    if (!string.IsNullOrEmpty(Convert.ToString(excelRow[20]).Trim()))
                        //    {
                        //        if (DateTime.TryParse(excelRow[20].ToString(), out date))
                        //        {
                        //            dataRow[20] = date.ToString("yyyyMMdd");
                        //        }
                        //        else
                        //        {
                        //            if (double.TryParse(excelRow[20].ToString(), out decimalNumber))
                        //            {
                        //                dataRow[20] = DateTime.FromOADate(double.Parse(Convert.ToString(excelRow[20]))).ToString("yyyyMMdd");
                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        dataRow[20] = DBNull.Value;
                        //    }

                        //    if (!string.IsNullOrEmpty(Convert.ToString(excelRow[21]).Trim()))
                        //    {
                        //        if (DateTime.TryParse(excelRow[21].ToString(), out date))
                        //        {
                        //            dataRow[21] = date.ToString("yyyyMMdd");
                        //        }
                        //        else
                        //        {
                        //            if (double.TryParse(excelRow[21].ToString(), out decimalNumber))
                        //            {
                        //                dataRow[21] = DateTime.FromOADate(double.Parse(Convert.ToString(excelRow[21]))).ToString("yyyyMMdd");
                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        dataRow[21] = DBNull.Value;
                        //    }

                        //    if (!string.IsNullOrEmpty(Convert.ToString(excelRow[22]).Trim()))
                        //    {
                        //        if (DateTime.TryParse(excelRow[22].ToString(), out date))
                        //        {
                        //            dataRow[22] = date.ToString("yyyyMMdd");
                        //        }
                        //        else
                        //        {
                        //            if (double.TryParse(excelRow[22].ToString(), out decimalNumber))
                        //            {
                        //                dataRow[22] = DateTime.FromOADate(double.Parse(Convert.ToString(excelRow[22]))).ToString("yyyyMMdd");
                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        dataRow[22] = DBNull.Value;
                        //    }

                        //    if (!string.IsNullOrEmpty(Convert.ToString(excelRow[23]).Trim()))
                        //    {
                        //        if (DateTime.TryParse(excelRow[23].ToString(), out date))
                        //        {
                        //            dataRow[23] = date.ToString("yyyyMMdd");
                        //        }
                        //        else
                        //        {
                        //            if (double.TryParse(excelRow[23].ToString(), out decimalNumber))
                        //            {
                        //                dataRow[23] = DateTime.FromOADate(double.Parse(Convert.ToString(excelRow[23]))).ToString("yyyyMMdd");
                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        dataRow[23] = DBNull.Value;
                        //    }

                        //    if (!string.IsNullOrEmpty(Convert.ToString(excelRow[24]).Trim()))
                        //    {
                        //        if (DateTime.TryParse(excelRow[24].ToString(), out date))
                        //        {
                        //            dataRow[24] = date.ToString("yyyyMMdd");
                        //        }
                        //        else
                        //        {
                        //            if (double.TryParse(excelRow[24].ToString(), out decimalNumber))
                        //            {
                        //                dataRow[24] = DateTime.FromOADate(double.Parse(Convert.ToString(excelRow[24]))).ToString("yyyyMMdd");
                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        dataRow[24] = DBNull.Value;
                        //    }

                        //    dataRow[25] = Convert.ToString(excelRow[25]).Trim();
                        //    dataRow[26] = Convert.ToString(excelRow[26]).Trim();
                        //    dataRow[27] = ProjectSession.LoggedInUserId;
                        //    dataTable.Rows.Add(dataRow);
                        //}
                    }
                    if (dataTable.Rows.Count > 0)
                    {
                        dataTable.Rows.RemoveAt(0);
                    }
                }
                //            DataView view = new DataView(dataTable);
                //            DataTable distinctValues = view.ToTable(true,"AccreditationNumber","AccountName");

                //            List<DataTable> tables = dataTable.AsEnumerable().GroupBy(row => new
                //                            {
                //                                Email = row.Field<string>("AccreditationNumber"),
                //                                Name = row.Field<string>("AccountName")
                //                            })
                //                      .Where(x => x.Count() > 1)
                //                //.Select(a => a.Select(p => p.Field<string>("AccreditationNumber")).ToString()).ToList();
                //.Select(g => g.CopyToDataTable()).ToList();

                return dataTable;
            }
            catch (Exception ex)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("ErrorMsg", typeof(string));
                DataRow dr = dt.NewRow();
                dr["ErrorMsg"] = dataTable != null ? ex.Message + " at Row: " + (dataTable.Rows.Count + 2) + " and Column: " + colIndex : ex.Message;
                dt.Rows.Add(dr);
                return dt;
            }
        }

        public DataTable GetDataTableFromSpreadsheetElectricityProvider(string filePath)
        {
            DataTable dataTable = new DataTable();
            int colIndex = 1;
            try
            {
                FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);

                IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);

                if (!excelReader.IsValid)
                {
                    stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
                    excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }

                excelReader.IsFirstRowAsColumnNames = false;
                DataSet result = excelReader.AsDataSet();

                dataTable.Columns.Add("Provider", typeof(string));
                dataTable.Columns.Add("Type", typeof(string));
                dataTable.Columns.Add("State", typeof(string));
                dataTable.Columns.Add("Preapprovals", typeof(string));
                dataTable.Columns.Add("Connections", typeof(string));
                dataTable.Columns.Add("CreatedBy", typeof(Int32));
                if (result != null && result.Tables.Count > 0)
                {

                    foreach (DataRow excelRow in result.Tables[0].Rows)
                    {
                        if (!string.IsNullOrEmpty(excelRow[0].ToString()) && !string.IsNullOrEmpty(Convert.ToString(excelRow[4])))
                        {
                            DataRow dataRow = dataTable.NewRow();
                            for (int i = 0; i < 5; i++)
                            {
                                colIndex = i + 1;
                                if (!string.IsNullOrEmpty(Convert.ToString(excelRow[i])))
                                    dataRow[i] = Convert.ToString(excelRow[i]).Trim();
                                else
                                    throw new System.InvalidOperationException("Value is null");
                            }
                            //dataRow[0] = Convert.ToString(excelRow[0]).Trim();
                            //dataRow[1] = Convert.ToString(excelRow[1]).Trim();
                            //dataRow[2] = Convert.ToString(excelRow[2]).Trim();
                            //dataRow[3] = Convert.ToString(excelRow[3]).Trim();
                            //dataRow[4] = Convert.ToString(excelRow[4]).Trim();
                            dataRow[5] = ProjectSession.LoggedInUserId;
                            dataTable.Rows.Add(dataRow);
                        }
                    }
                    if (dataTable.Rows.Count > 0)
                    {
                        dataTable.Rows.RemoveAt(0);
                    }
                }
                return dataTable;
            }
            catch (Exception ex)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("ErrorMsg", typeof(string));
                DataRow dr = dt.NewRow();
                dr["ErrorMsg"] = dataTable != null ? ex.Message + " at Row: " + (dataTable.Rows.Count + 2) + " and Column: " + colIndex : ex.Message;
                dt.Rows.Add(dr);
                return dt;
            }
        }

        /// <summary>
        /// Gets the data table from spreadsheet serial number.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public void GetDataTableFromSpreadsheetSerialNumber(string filePath)
        {
            FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
            if (!excelReader.IsValid)
            {
                stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
                excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            }

            excelReader.IsFirstRowAsColumnNames = true;
            DataSet result = excelReader.AsDataSet();
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("SerialNumber", typeof(string));
            if (result != null && result.Tables.Count > 0)
            {
                foreach (DataRow excelRow in result.Tables[0].Rows)
                {
                    foreach (string stringItem in excelRow[0].ToString().Split(','))
                    {
                        DataRow dataRow = dataTable.NewRow();
                        dataRow[0] = stringItem.Trim();
                        dataTable.Rows.Add(dataRow);
                    }
                }
            }

            string[] column = new string[1];
            column[0] = "SerialNumber";
            CommonDAL.SaveBulk(dataTable, "[dbo].[SerialNumbers]", column, column);
        }

        /// <summary>
        /// Updates the modules.
        /// </summary>
        /// <param name="moduleId">The module identifier.</param>
        /// <param name="wattage">The wattage.</param>
        public void UpdatePVModules(int moduleId, int wattage)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("PVModuleId", SqlDbType.Int, moduleId));
            sqlParameters.Add(DBClient.AddParameters("Wattage", SqlDbType.Int, wattage));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
            CommonDAL.ExecuteScalar("CER_UpdatePVModules", sqlParameters.ToArray());
        }

        /// <summary>
        /// Gets the modules data into list.
        /// </summary>
        /// <param name="pageNumber">page number</param>
        /// <param name="pageSize">page size</param>
        /// <param name="sortColumn">sort column</param>
        /// <param name="sortDirection">sort direction</param>
        /// <param name="certificateHolder">certificate holder</param>
        /// <param name="modelNumber">model number</param>
        /// <param name="wattage">wattage</param>
        /// <param name="cecApprovedDate">approved date</param>
        /// <param name="expiryDate">expiry date</param>
        /// <returns>list of modules</returns>
        public IList<PVModules> ModulesList(int pageNumber, int pageSize, string sortColumn, string sortDirection, string certificateHolder, string modelNumber, string wattage, string cecApprovedDate, string expiryDate)
        {
            try
            {
                int? innerWattage = null;
                if (!string.IsNullOrEmpty(wattage))
                {
                    innerWattage = Convert.ToInt32(wattage);
                }

                DateTime dtApproval, dtExpiry;
                DateTime? dtApprovalDate, dtExpiryDate;
                if (DateTime.TryParseExact(cecApprovedDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dtApproval))
                {
                    dtApprovalDate = dtApproval;
                }
                else
                {
                    dtApprovalDate = null;
                }

                if (DateTime.TryParseExact(expiryDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dtExpiry))
                {
                    dtExpiryDate = dtExpiry;
                }
                else
                {
                    dtExpiryDate = null;
                }

                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, pageNumber));
                sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, pageSize));
                sqlParameters.Add(DBClient.AddParameters("SortColumn", SqlDbType.NVarChar, sortColumn));
                sqlParameters.Add(DBClient.AddParameters("SortDirection", SqlDbType.VarChar, sortDirection));
                sqlParameters.Add(DBClient.AddParameters("CertificateHolder", SqlDbType.NVarChar, certificateHolder));
                sqlParameters.Add(DBClient.AddParameters("ModelNumber", SqlDbType.NVarChar, modelNumber));
                sqlParameters.Add(DBClient.AddParameters("Wattage", SqlDbType.Int, innerWattage));
                sqlParameters.Add(DBClient.AddParameters("CECApprovedDate", SqlDbType.Date, dtApprovalDate));
                sqlParameters.Add(DBClient.AddParameters("ExpiryDate", SqlDbType.Date, dtExpiryDate));
                IList<PVModules> lstPVModules = CommonDAL.ExecuteProcedure<PVModules>("CER_GetPVModules", sqlParameters.ToArray()).ToList();
                return lstPVModules;
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return null;
            }
        }

        public IList<BatteryStorage> BatteryStorageList(int pageNumber, int pageSize, string sortColumn, string sortDirection, string manufacturer, string modelNumber)
        {
            try
            {
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, pageNumber));
                sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, pageSize));
                sqlParameters.Add(DBClient.AddParameters("SortColumn", SqlDbType.NVarChar, sortColumn));
                sqlParameters.Add(DBClient.AddParameters("SortDirection", SqlDbType.VarChar, sortDirection));
                sqlParameters.Add(DBClient.AddParameters("Manufacturer", SqlDbType.NVarChar, manufacturer));
                sqlParameters.Add(DBClient.AddParameters("ModelNumber", SqlDbType.NVarChar, modelNumber));
                IList<BatteryStorage> lstBatteryStorage = CommonDAL.ExecuteProcedure<BatteryStorage>("GetBatteryStorage", sqlParameters.ToArray()).ToList();
                return lstBatteryStorage;
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return null;
            }
        }

        /// <summary>
        /// Gets the inverters data into list.
        /// </summary>
        /// <param name="pageNumber">page number</param>
        /// <param name="pageSize">page size</param>
        /// <param name="sortColumn">sort column</param>
        /// <param name="sortDirection">sort direction</param>
        /// <param name="manufacturer">manu facture</param>
        /// <param name="series">series</param>
        /// <param name="modelNumber">model number</param>
        /// <param name="acPowerKW">power</param>
        /// <param name="approvalDate">approval date</param>
        /// <param name="expiryDate">expiry date</param>
        /// <returns>list of inverters</returns>
        public IList<Inverters> InvertersList(int pageNumber, int pageSize, string sortColumn, string sortDirection, string manufacturer, string series, string modelNumber, string acPowerKW, string approvalDate, string expiryDate)
        {

            try
            {
                int? powerKW = null;
                if (!string.IsNullOrEmpty(acPowerKW))
                {
                    powerKW = Convert.ToInt32(acPowerKW);
                }

                DateTime dtApproval, dtExpiry;
                DateTime? dtApprovalDate, dtExpiryDate;
                if (DateTime.TryParseExact(approvalDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dtApproval))
                {
                    dtApprovalDate = dtApproval;
                }
                else
                {
                    dtApprovalDate = null;
                }

                if (DateTime.TryParseExact(expiryDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dtExpiry))
                {
                    dtExpiryDate = dtExpiry;
                }
                else
                {
                    dtExpiryDate = null;
                }

                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, pageNumber));
                sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, pageSize));
                sqlParameters.Add(DBClient.AddParameters("SortColumn", SqlDbType.NVarChar, sortColumn));
                sqlParameters.Add(DBClient.AddParameters("SortDirection", SqlDbType.VarChar, sortDirection));
                sqlParameters.Add(DBClient.AddParameters("Manufacturer", SqlDbType.NVarChar, manufacturer));
                sqlParameters.Add(DBClient.AddParameters("Series", SqlDbType.NVarChar, series));
                sqlParameters.Add(DBClient.AddParameters("ModelNumber", SqlDbType.NVarChar, modelNumber));
                sqlParameters.Add(DBClient.AddParameters("AcPowerKW", SqlDbType.Int, powerKW));
                sqlParameters.Add(DBClient.AddParameters("ApprovalDate", SqlDbType.Date, dtApprovalDate));
                sqlParameters.Add(DBClient.AddParameters("ExpiryDate", SqlDbType.Date, dtExpiryDate));
                IList<Inverters> lstInverters = CommonDAL.ExecuteProcedure<Inverters>("CER_GetInverters", sqlParameters.ToArray()).ToList();
                return lstInverters;
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return null;
            }
        }

        /// <summary>
        /// Gets the brand model data into list.
        /// </summary>
        /// <param name="pageNumber">page number</param>
        /// <param name="pageSize">page size</param>
        /// <param name="sortColumn">sort column</param>
        /// <param name="sortDirection">sort direction</param>
        /// <param name="item">item</param>
        /// <param name="brand">brand</param>
        /// <param name="model">model</param>
        /// <param name="eligibleFrom">eligible from</param>
        /// <param name="eligibleTo">eligible to</param>
        /// <returns>list of brand model</returns>
        public IList<HWBrandModel> HWBrandModelList(int pageNumber, int pageSize, string sortColumn, string sortDirection, string item, string brand, string model, string eligibleFrom, string eligibleTo)
        {
            try
            {
                DateTime dtFrom, dtTo;
                DateTime? dtEligibleFrom, dtEligibleTo;
                if (DateTime.TryParseExact(eligibleFrom, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dtFrom))
                {
                    dtEligibleFrom = dtFrom;
                }
                else
                {
                    dtEligibleFrom = null;
                }

                if (DateTime.TryParseExact(eligibleTo, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dtTo))
                {
                    dtEligibleTo = dtTo;
                }
                else
                {
                    dtEligibleTo = null;
                }

                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, pageNumber));
                sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, pageSize));
                sqlParameters.Add(DBClient.AddParameters("SortColumn", SqlDbType.NVarChar, sortColumn));
                sqlParameters.Add(DBClient.AddParameters("SortDirection", SqlDbType.VarChar, sortDirection));
                sqlParameters.Add(DBClient.AddParameters("Item", SqlDbType.NVarChar, string.IsNullOrEmpty(item) ? null : item));
                sqlParameters.Add(DBClient.AddParameters("Brand", SqlDbType.NVarChar, string.IsNullOrEmpty(brand) ? null : brand));
                sqlParameters.Add(DBClient.AddParameters("Model", SqlDbType.NVarChar, string.IsNullOrEmpty(model) ? null : model));
                sqlParameters.Add(DBClient.AddParameters("EligibleFrom", SqlDbType.Date, dtEligibleFrom));
                sqlParameters.Add(DBClient.AddParameters("EligibleTo", SqlDbType.Date, dtEligibleTo));
                IList<HWBrandModel> lstHWBrandModel = CommonDAL.ExecuteProcedure<HWBrandModel>("CER_GetHWBrandModel", sqlParameters.ToArray()).ToList();
                return lstHWBrandModel;
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return null;
            }
        }

        /// <summary>
        /// Accredited the installers list.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <param name="accreditationNumber">The accreditation number.</param>
        /// <param name="name">The name.</param>
        /// <param name="accountName">Name of the account.</param>
        /// <param name="licensedElectricianNumber">The licensed electrician number.</param>
        /// <param name="gridType">Type of the grid.</param>
        /// <returns>list of Accredited installers</returns>
        public IList<AccreditedInstallers> AccreditedInstallersList(int pageNumber, int pageSize, string sortColumn, string sortDirection, string accreditationNumber, string name, string accountName, string licensedElectricianNumber, string gridType)
        {
            try
            {
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, pageNumber));
                sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, pageSize));
                sqlParameters.Add(DBClient.AddParameters("SortColumn", SqlDbType.NVarChar, sortColumn));
                sqlParameters.Add(DBClient.AddParameters("SortDirection", SqlDbType.VarChar, sortDirection));
                sqlParameters.Add(DBClient.AddParameters("AccreditationNumber", SqlDbType.NVarChar, accreditationNumber));
                string firstName = null, lastName = null;
                if (!string.IsNullOrEmpty(name) && name.Trim().Contains(" "))
                {
                    string[] array = name.Split(' ');
                    firstName = array[0];
                    lastName = array[1];
                }
                else
                {
                    firstName = name;
                }

                sqlParameters.Add(DBClient.AddParameters("FirstName", SqlDbType.NVarChar, firstName));
                sqlParameters.Add(DBClient.AddParameters("LastName", SqlDbType.NVarChar, lastName));
                sqlParameters.Add(DBClient.AddParameters("AccountName", SqlDbType.NVarChar, accountName));
                sqlParameters.Add(DBClient.AddParameters("LicensedElectricianNumber", SqlDbType.NVarChar, licensedElectricianNumber));
                sqlParameters.Add(DBClient.AddParameters("GridType", SqlDbType.NVarChar, gridType));
                IList<AccreditedInstallers> lstAccreditedInstallers = CommonDAL.ExecuteProcedure<AccreditedInstallers>("CER_GetAccreditedInstallers", sqlParameters.ToArray()).ToList();
                return lstAccreditedInstallers;
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return null;
            }
        }

        public IList<ElectricityProvider> ElectricityProviderList(int pageNumber, int pageSize, string sortColumn, string sortDirection, string provider, string type, string state, string preapprovals, string connections)
        {
            try
            {
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, pageNumber));
                sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, pageSize));
                sqlParameters.Add(DBClient.AddParameters("SortColumn", SqlDbType.NVarChar, sortColumn));
                sqlParameters.Add(DBClient.AddParameters("SortDirection", SqlDbType.VarChar, sortDirection));
                sqlParameters.Add(DBClient.AddParameters("Provider", SqlDbType.NVarChar, provider));
                sqlParameters.Add(DBClient.AddParameters("Type", SqlDbType.NVarChar, type));
                sqlParameters.Add(DBClient.AddParameters("State", SqlDbType.NVarChar, state));
                sqlParameters.Add(DBClient.AddParameters("Preapprovals", SqlDbType.NVarChar, preapprovals));
                sqlParameters.Add(DBClient.AddParameters("Connections", SqlDbType.NVarChar, connections));
                IList<ElectricityProvider> lstElectricityProvider = CommonDAL.ExecuteProcedure<ElectricityProvider>("CER_GetElectricityProvider", sqlParameters.ToArray()).ToList();
                return lstElectricityProvider;
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return null;
            }
        }

        public void DeleteElectricityProvider(int electricityProviderId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ElectricityProviderId", SqlDbType.Int, electricityProviderId));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("Mode", SqlDbType.NVarChar, "DELETE"));
            sqlParameters.Add(DBClient.AddParameters("UpdatedDate", SqlDbType.DateTime, DateTime.Now));
            CommonDAL.Crud("CER_UpdateElectricityProviders", sqlParameters.ToArray());
        }

        public void UpdateElectricityProvider(string electricityProviderId, string provider, string type, string state, string preapproval, string connection)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ElectricityProviderId", SqlDbType.Int, electricityProviderId));
            sqlParameters.Add(DBClient.AddParameters("Provider", SqlDbType.NVarChar, provider));
            sqlParameters.Add(DBClient.AddParameters("Type", SqlDbType.NVarChar, type));
            sqlParameters.Add(DBClient.AddParameters("State", SqlDbType.NVarChar, state));
            sqlParameters.Add(DBClient.AddParameters("Preapprovals", SqlDbType.NVarChar, preapproval));
            sqlParameters.Add(DBClient.AddParameters("Connections", SqlDbType.NVarChar, connection));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("Mode", SqlDbType.NVarChar, "UPDATE"));
            sqlParameters.Add(DBClient.AddParameters("UpdatedDate", SqlDbType.DateTime, DateTime.Now));
            CommonDAL.Crud("CER_UpdateElectricityProviders", sqlParameters.ToArray());
        }

        /// <summary>
        /// Gets all modules.
        /// </summary>
        /// <returns>Return  dataSet</returns>
        public DataSet GetAllModules()
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, -1));
            return CommonDAL.ExecuteDataSet("CER_GetPVModules", sqlParameters.ToArray());
        }

        public string GetCERLog(SystemEnums.CERType fileType, SystemEnums.CERSubType subType)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("CERType", SqlDbType.TinyInt, (int)fileType));
            sqlParameters.Add(DBClient.AddParameters("SubType", SqlDbType.TinyInt, (int)subType));
            IList<CERLog> lstCERLog = CommonDAL.ExecuteProcedure<CERLog>("GetCERLog", sqlParameters.ToArray()).ToList();
            return lstCERLog.Count > 0 ? lstCERLog[0].CERText : "";

        }

        public AccreditedInstallers GetAccreditedInstallerDetailByAccreditedInstallerId(int AccreditedInstallerId)
        {
            try
            {
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(DBClient.AddParameters("AccreditedInstallerId", SqlDbType.Int, AccreditedInstallerId));
                AccreditedInstallers objAccreditedInstallers = CommonDAL.SelectObject<AccreditedInstallers>("GetAccreditedInstallerDetailByAccreditedInstallerId", sqlParameters.ToArray());
                return objAccreditedInstallers;
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return null;
            }
        }

        public DataTable dtBatteryStorage()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("EquipmentCategory", typeof(string));
            dt.Columns.Add("CompliancePathway", typeof(string));
            dt.Columns.Add("ManufacturerCertificateHolder", typeof(string));
            dt.Columns.Add("BrandName", typeof(string));
            dt.Columns.Add("Series", typeof(string));
            dt.Columns.Add("ModelNumber", typeof(string));
            dt.Columns.Add("RatedApparentACPowerkVA", typeof(decimal));
            dt.Columns.Add("NominalBatteryCapacitykWh", typeof(decimal));
            dt.Columns.Add("DepthOfDischarge", typeof(int));
            dt.Columns.Add("UsableCapacitykWh", typeof(decimal));
            dt.Columns.Add("MinOperatingTemp", typeof(int));
            dt.Columns.Add("MaxOperatingTemp", typeof(int));
            dt.Columns.Add("OutdoorUsage", typeof(string));
            dt.Columns.Add("CECApprovalDate", typeof(DateTime));
            dt.Columns.Add("CECExpiryDate", typeof(DateTime));
            dt.Columns.Add("CreatedDate", typeof(DateTime));
            dt.Columns.Add("CreatedBy", typeof(int));
            return dt;
        }


        /// <summary>
        /// Insert Spv manufacture and its verification url
        /// </summary>
        /// <param name="SPVDatatable"></param>
        public void SyncSPVJson(DataTable SPVDatatable, bool isFromSyncJson = false, bool isFromUploadJson = false,string fileName=null)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SPVDatatable", SqlDbType.Structured,SPVDatatable));
            sqlParameters.Add(DBClient.AddParameters("isFromSyncJson", SqlDbType.Bit, isFromSyncJson));
            sqlParameters.Add(DBClient.AddParameters("isFromUploadJson", SqlDbType.Bit, isFromUploadJson));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("CERType", SqlDbType.Int, SystemEnums.CERType.SPVManufacturer.GetHashCode()));
            sqlParameters.Add(DBClient.AddParameters("Version", SqlDbType.VarChar, fileName));

            CommonDAL.ExecuteScalar("SyncSPVJson", sqlParameters.ToArray());
        }
        /// <summary>
        /// Get Manufacturer for set the isspv flag  manufactuer wise
        /// </summary>
        
        public IList<Spvmanufacturer> GetManufacturerForSetSpvByManufacturer()
        {
            IList<Spvmanufacturer> lstSpvmanufacturer = CommonDAL.ExecuteProcedure<Spvmanufacturer>("GetManufacturerForSetSpvByManufacturer").ToList();
            return lstSpvmanufacturer;
           
        }
        public void SaveSpvSetByManufacturerPopUp(string Spvmanufacturerid)
        {
            string spName = "[SaveSpvSetByManufacturer]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("Spvmanufacturerid", SqlDbType.VarChar, Spvmanufacturerid));
            CommonDAL.ExecuteScalar(spName, sqlParameters.ToArray());
        }
        public void SyncAccreditedInstallerList(DataTable SyncAccreditedInstallerListDatatable)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("DataTable", SqlDbType.Structured, SyncAccreditedInstallerListDatatable));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("CERType", SqlDbType.Int, SystemEnums.CERType.AccreditedInstallers.GetHashCode()));
            sqlParameters.Add(DBClient.AddParameters("IsFromSyncAccreditedInstaller", SqlDbType.Bit, true));
            CommonDAL.ExecuteScalar("MergeAccreditedInstallers", sqlParameters.ToArray());
        }
    }
}

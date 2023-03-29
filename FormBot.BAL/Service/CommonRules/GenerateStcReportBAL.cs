using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormBot.Entity;
using System.Data;
using FormBot.Helper.Helper;
using System.IO;
using System.Net;
using FormBot.Helper;
using Newtonsoft.Json;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using System.Web;
using System.Configuration;


namespace FormBot.BAL.Service.CommonRules
{
    public class GenerateStcReportBAL : IGenerateStcReportBAL
    {
        private readonly ISTCInvoiceBAL _stcInvoiceBal;

        public GenerateStcReportBAL(ISTCInvoiceBAL stcInvoiceBal)
        {
            this._stcInvoiceBal = stcInvoiceBal;
        }

        //public String CreateStcReport(string Filename, string ExportType, int STCJobDetailsID, string InvoiceNo, string solarCompanyId, string userTypeId, int userId, int rId, bool IsWindowService, bool RegenerateRemittanceFile = false, bool IsBackgroundRecProcess = false)
        //{

        //    Microsoft.Reporting.WebForms.Warning[] warnings;
        //    string[] streamIds;
        //    string mimeType = string.Empty;
        //    string encoding = string.Empty;
        //    string extension = string.Empty;
        //    ReportViewer viewer = new ReportViewer();
        //    //XmlDocument oXD = new XmlDocument();
        //    //oXD.Load(Server.MapPath("/Reports/InvoiceSTC.rdlc"));
        //    STCInvoice stcinvoice = new STCInvoice();
        //    DataSet ds = new DataSet();
        //    int report = 1;
        //    string RefNumber = string.Empty;
        //    string CompanyABN = string.Empty;
        //    string CompanyABNReseller = string.Empty;
        //    string InoviceDate = string.Empty;
        //    string InvoiceNumber = string.Empty;
        //    string AmountDue = string.Empty;
        //    string Total = string.Empty;
        //    string DueDate = string.Empty;
        //    string FromAddressLine1 = string.Empty;
        //    string FromAddressLine2 = string.Empty;
        //    string FromAddressLine3 = string.Empty;
        //    string ToAddressLine1 = string.Empty;
        //    string ToAddressLine2 = string.Empty;
        //    string ToAddressLine3 = string.Empty;
        //    string LogoPath = string.Empty;
        //    string InvoiceFooter = string.Empty;
        //    string JobDescription = string.Empty;
        //    string JobDate = string.Empty;
        //    string JobAddress = string.Empty;
        //    string JobTitle = string.Empty;
        //    string Logo = string.Empty;
        //    string ItemCode = string.Empty;
        //    string jobid = string.Empty;
        //    string IsStcInvoice = string.Empty;
        //    string ToName = string.Empty;
        //    string FromName = string.Empty;
        //    string FromCompanyName = string.Empty;
        //    string ToCompanyName = string.Empty;
        //    string ToOwnerCompanyName = string.Empty;
        //    string OwnerName = string.Empty;
        //    string lblABN = string.Empty;

        //    bool IsJobDescription = false;
        //    bool IsJobAddress = false;
        //    bool IsJobDate = false;
        //    bool IsTitle = false;
        //    bool IsName = false;
        //    bool IsTaxInclusive = false;
        //    decimal? TaxRate = 0;
        //    int SettingUserId = 0;
        //    string path = string.Empty;

        //    string AccountName = string.Empty;
        //    string BSB = string.Empty;
        //    string AccountNumber = string.Empty;
        //    string Reference_WholeSaler = string.Empty;
        //    string Gst = string.Empty;
        //    decimal NoOfStc = 0;
        //    decimal StcPrice = 0;
        //    string AccountNameReseller = string.Empty;
        //    string BSBReseller = string.Empty;
        //    string AccountNumberReseller = string.Empty;
        //    string STCPVDCode = string.Empty;
        //    bool IsWholeSaler = false;
        //    string resellerId = string.Empty;
        //    bool IsPeakPay = false;
        //    decimal PeakPayFee = 0;
        //    decimal PeakPayGst = 0;
        //    string SolarCompanyName = string.Empty;

        //    Entity.Settings.Settings settings = new Entity.Settings.Settings();

        //    settings = GetSettingsData(solarCompanyId, userTypeId, userId, rId);
        //    IsJobDescription = settings.IsJobDescription;
        //    IsJobAddress = settings.IsJobAddress;
        //    IsJobDate = settings.IsJobDate;
        //    IsTitle = settings.IsTitle;
        //    IsName = settings.IsName;
        //    IsTaxInclusive = settings.IsTaxInclusive;
        //    TaxRate = settings.TaxRate;
        //    //SettingUserId = settings.UserId;

        //    //SettingUserId = 544;
        //    //Logo = "gogreen.jpg";

        //    //var invoiceSetting = _jobInvoiceDetail.getInvoiceSetting();
        //    //if (invoiceSetting.Item1 != null && invoiceSetting.Item1 != DateTime.MinValue)
        //    //{
        //    //    DueDate = invoiceSetting.Item1.ToString("dd MMMM yyyy");
        //    //}


        //    ds = _stcInvoiceBal.GetStcInvoice(STCJobDetailsID, IsJobAddress, IsJobDate, IsJobDescription, IsTitle, IsName, DateTime.Now, InvoiceNo);
        //    DataTable dt = new DataTable();
        //    if (ds != null && ds.Tables.Count > 0)
        //    {

        //        if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
        //        {
        //            RefNumber = !string.IsNullOrEmpty(ds.Tables[0].Rows[0]["RefNumber"].ToString()) ? ds.Tables[0].Rows[0]["RefNumber"].ToString() : string.Empty;
        //        }
        //        if (ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
        //        {
        //            CompanyABN = !string.IsNullOrEmpty(ds.Tables[1].Rows[0]["CompanyABN"].ToString()) ? ds.Tables[1].Rows[0]["CompanyABN"].ToString() : string.Empty;
        //            if (!string.IsNullOrEmpty(CompanyABN))
        //            {
        //                lblABN = "ABN:";
        //            }

        //            CompanyABNReseller = !string.IsNullOrEmpty(ds.Tables[1].Rows[0]["CompanyABNReseller"].ToString()) ? ds.Tables[1].Rows[0]["CompanyABNReseller"].ToString() : string.Empty;
        //            Logo = !string.IsNullOrEmpty(ds.Tables[1].Rows[0]["ResellerLogo"].ToString()) ? ds.Tables[1].Rows[0]["ResellerLogo"].ToString() : string.Empty;
        //            SettingUserId = !string.IsNullOrEmpty(ds.Tables[1].Rows[0]["SettingUserId"].ToString()) ? Convert.ToInt32(ds.Tables[1].Rows[0]["SettingUserId"]) : 0;
        //            InvoiceFooter = !string.IsNullOrEmpty(ds.Tables[1].Rows[0]["InvoiceFooter"].ToString()) ? ds.Tables[1].Rows[0]["InvoiceFooter"].ToString() : string.Empty;

        //            AccountName = !string.IsNullOrEmpty(ds.Tables[1].Rows[0]["AccountName"].ToString()) ? ds.Tables[1].Rows[0]["AccountName"].ToString() : string.Empty;
        //            BSB = !string.IsNullOrEmpty(ds.Tables[1].Rows[0]["BSB"].ToString()) ? Convert.ToString(ds.Tables[1].Rows[0]["BSB"]) : string.Empty;
        //            AccountNumber = !string.IsNullOrEmpty(ds.Tables[1].Rows[0]["AccountNumber"].ToString()) ? ds.Tables[1].Rows[0]["AccountNumber"].ToString() : string.Empty;

        //            AccountNameReseller = !string.IsNullOrEmpty(ds.Tables[1].Rows[0]["AccountNameReseller"].ToString()) ? ds.Tables[1].Rows[0]["AccountNameReseller"].ToString() : string.Empty;
        //            BSBReseller = !string.IsNullOrEmpty(ds.Tables[1].Rows[0]["BSBReseller"].ToString()) ? Convert.ToString(ds.Tables[1].Rows[0]["BSBReseller"]) : string.Empty;
        //            AccountNumberReseller = !string.IsNullOrEmpty(ds.Tables[1].Rows[0]["AccountNumberReseller"].ToString()) ? ds.Tables[1].Rows[0]["AccountNumberReseller"].ToString() : string.Empty;
        //            IsWholeSaler = Convert.ToBoolean(ds.Tables[1].Rows[0]["IsWholeSaler"]);
        //            resellerId = !string.IsNullOrEmpty(ds.Tables[1].Rows[0]["ResellerID"].ToString()) ? ds.Tables[1].Rows[0]["ResellerID"].ToString() : string.Empty;
        //            SolarCompanyName = !string.IsNullOrEmpty(ds.Tables[1].Rows[0]["SolarCompanyName"].ToString()) ? ds.Tables[1].Rows[0]["SolarCompanyName"].ToString() : string.Empty;
        //            if (Logo != "" && Logo != null)
        //            {
        //                string LogoP = string.Empty;
        //                if (IsWindowService)
        //                    LogoP = Convert.ToString(ConfigurationManager.AppSettings["ProofUploadFolder"]) + "\\UserDocuments" + "\\" + SettingUserId + "\\" + Logo;
        //                else
        //                    LogoP = Path.Combine(ProjectConfiguration.UploadedDocumentPath + "\\UserDocuments" + "\\" + SettingUserId, Logo);
        //                //LogoP = Path.Combine(ProjectSession.UploadedDocumentPath + "\\UserDocuments" + "\\" + SettingUserId, Logo);

        //                LogoPath = new Uri(LogoP).AbsoluteUri;
        //            }
        //            else
        //            {
        //                LogoPath = "";
        //            }
        //        }
        //        if (ds.Tables[2] != null && ds.Tables[2].Rows.Count > 0)
        //        {
        //            InoviceDate = !string.IsNullOrEmpty(ds.Tables[2].Rows[0]["InoviceDate"].ToString()) ? Convert.ToDateTime(ds.Tables[2].Rows[0]["InoviceDate"]).ToString("dd MMMM yyyy") : string.Empty;
        //            InvoiceNumber = InvoiceNo;
        //            jobid = !string.IsNullOrEmpty(ds.Tables[2].Rows[0]["jobid"].ToString()) ? ds.Tables[2].Rows[0]["jobid"].ToString() : string.Empty;
        //            DueDate = !string.IsNullOrEmpty(ds.Tables[2].Rows[0]["DueDate"].ToString()) ? Convert.ToDateTime(ds.Tables[2].Rows[0]["DueDate"]).ToString("dd MMMM yyyy") : string.Empty;
        //        }

        //        if (ds.Tables[3] != null && ds.Tables[3].Rows.Count > 0)
        //        {
        //            Gst = ds.Tables[3].Rows[0]["IsTaxInclusive"].ToString() == "True" ? "+Gst" : "";
        //            NoOfStc = !string.IsNullOrEmpty(ds.Tables[3].Rows[0]["Quantity"].ToString()) ? Convert.ToDecimal(ds.Tables[3].Rows[0]["Quantity"]) : 0;
        //            StcPrice = !string.IsNullOrEmpty(ds.Tables[3].Rows[0]["Sale"].ToString()) ? Convert.ToDecimal(ds.Tables[3].Rows[0]["Sale"]) : 0;
        //            STCPVDCode = !string.IsNullOrEmpty(ds.Tables[3].Rows[0]["STCPVDCode"].ToString()) ? ds.Tables[3].Rows[0]["STCPVDCode"].ToString() : string.Empty;
        //            dt = ds.Tables[3];
        //        }
        //        if (ds.Tables[4] != null && ds.Tables[4].Rows.Count > 0)
        //        {
        //            ToAddressLine1 = !string.IsNullOrEmpty(ds.Tables[4].Rows[0]["ToAddressLine1"].ToString()) ? ds.Tables[4].Rows[0]["ToAddressLine1"].ToString() : string.Empty;
        //            ToAddressLine2 = !string.IsNullOrEmpty(ds.Tables[4].Rows[0]["ToAddressLine2"].ToString()) ? ds.Tables[4].Rows[0]["ToAddressLine2"].ToString() : string.Empty;
        //            ToAddressLine3 = !string.IsNullOrEmpty(ds.Tables[4].Rows[0]["ToAddressLine3"].ToString()) ? ds.Tables[4].Rows[0]["ToAddressLine3"].ToString() : string.Empty;
        //            //ToCompanyName = !string.IsNullOrEmpty(ds.Tables[4].Rows[0]["ToCompanyName"].ToString()) ? ds.Tables[4].Rows[0]["ToCompanyName"].ToString() : string.Empty;
        //            if (IsWholeSaler)
        //            {
        //                OwnerName = !string.IsNullOrEmpty(ds.Tables[4].Rows[0]["OwnerName"].ToString()) ? ds.Tables[4].Rows[0]["OwnerName"].ToString() : string.Empty;
        //                ToOwnerCompanyName = !string.IsNullOrEmpty(ds.Tables[4].Rows[0]["ToOwnerCompanyName"].ToString()) ? ds.Tables[4].Rows[0]["ToOwnerCompanyName"].ToString() : string.Empty;
        //            }
        //            else
        //            {
        //                ToCompanyName = !string.IsNullOrEmpty(ds.Tables[4].Rows[0]["ToCompanyName"].ToString()) ? ds.Tables[4].Rows[0]["ToCompanyName"].ToString() : string.Empty;
        //            }
        //        }
        //        if (ds.Tables[5] != null && ds.Tables[5].Rows.Count > 0)
        //        {
        //            FromAddressLine1 = (ds.Tables[5].Rows[0]["FormAddressline1"].ToString() == "") ? "" : ds.Tables[5].Rows[0]["FormAddressline1"].ToString();
        //            FromAddressLine2 = (ds.Tables[5].Rows[0]["FormAddressline2"].ToString() == "") ? "" : ds.Tables[5].Rows[0]["FormAddressline2"].ToString();
        //            FromAddressLine3 = (ds.Tables[5].Rows[0]["FormAddressline3"].ToString() == "") ? "" : ds.Tables[5].Rows[0]["FormAddressline3"].ToString();
        //            //FromCompanyName = (ds.Tables[5].Rows[0]["FromCompanyName"].ToString() == "") ? "" : ds.Tables[5].Rows[0]["FromCompanyName"].ToString();
        //            if (IsWholeSaler)
        //            {
        //                FromCompanyName = (ds.Tables[5].Rows[0]["FromCompanyName"].ToString() == "") ? "" : ds.Tables[5].Rows[0]["FromCompanyName"].ToString();
        //            }
        //            else
        //            {
        //                OwnerName = (ds.Tables[5].Rows[0]["OwnerName"].ToString() == "") ? "" : ds.Tables[5].Rows[0]["OwnerName"].ToString();
        //                ToOwnerCompanyName = !string.IsNullOrEmpty(ds.Tables[5].Rows[0]["ToOwnerCompanyName"].ToString()) ? ds.Tables[5].Rows[0]["ToOwnerCompanyName"].ToString() : string.Empty;
        //            }
        //        }
        //        if (ds.Tables[6] != null && ds.Tables[6].Rows.Count > 0)
        //        {
        //            JobDate = !string.IsNullOrEmpty(ds.Tables[6].Rows[0][0].ToString()) ? Convert.ToDateTime(ds.Tables[6].Rows[0][0]).ToString("dd MMMM yyyy") : string.Empty;
        //        }
        //        if (ds.Tables[7] != null && ds.Tables[7].Rows.Count > 0)
        //        {
        //            JobDescription = !string.IsNullOrEmpty(ds.Tables[7].Rows[0][0].ToString()) ? ds.Tables[7].Rows[0][0].ToString() : string.Empty;
        //        }

        //        if (ds.Tables[8] != null && ds.Tables[8].Rows.Count > 0)
        //        {
        //            JobTitle = !string.IsNullOrEmpty(ds.Tables[8].Rows[0][0].ToString()) ? ds.Tables[8].Rows[0][0].ToString() : string.Empty;
        //        }
        //        if (ds.Tables[9] != null && ds.Tables[9].Rows.Count > 0)
        //        {
        //            JobAddress = !string.IsNullOrEmpty(ds.Tables[9].Rows[0][0].ToString()) ? ds.Tables[9].Rows[0][0].ToString() : string.Empty;
        //        }

        //        if (ds.Tables.Count > 10 && ds.Tables[10] != null && ds.Tables[10].Rows.Count > 0)
        //        {
        //            IsPeakPay = true;
        //            PeakPayFee = !string.IsNullOrEmpty(ds.Tables[10].Rows[0]["PeakPayFee"].ToString()) ? Convert.ToDecimal(ds.Tables[10].Rows[0]["PeakPayFee"]) : 0;
        //            PeakPayGst = !string.IsNullOrEmpty(ds.Tables[10].Rows[0]["PeakPayGst"].ToString()) ? Convert.ToDecimal(ds.Tables[10].Rows[0]["PeakPayGst"]) : 0;
        //        }
        //        //Reference_WholeSaler = ToCompanyName + " - " + RefNumber + " " + STCPVDCode + " " + NoOfStc + "@" + StcPrice + Gst;
        //        Reference_WholeSaler = SolarCompanyName + " - " + RefNumber + " " + STCPVDCode + " " + NoOfStc + "@" + StcPrice + Gst;

        //        if (IsPeakPay)
        //        {
        //            if (IsWindowService || IsBackgroundRecProcess)
        //                viewer.LocalReport.ReportPath = AppDomain.CurrentDomain.BaseDirectory + "//Reports//InvoiceSTC_PeakPay.rdlc";
        //            else
        //                viewer.LocalReport.ReportPath = @"Reports//InvoiceSTC_PeakPay.rdlc";
        //        }
        //        else if (IsWholeSaler)
        //        {
        //            if (IsWindowService || IsBackgroundRecProcess)
        //                viewer.LocalReport.ReportPath = AppDomain.CurrentDomain.BaseDirectory + "//Reports//InvoiceSTC_WholeSaler.rdlc";
        //            else
        //                viewer.LocalReport.ReportPath = @"Reports//InvoiceSTC_WholeSaler.rdlc";
        //        }
        //        else
        //        {
        //            if (IsWindowService || IsBackgroundRecProcess)
        //                viewer.LocalReport.ReportPath = AppDomain.CurrentDomain.BaseDirectory + "//Reports//InvoiceSTC.rdlc";
        //            else
        //                viewer.LocalReport.ReportPath = @"Reports//InvoiceSTC.rdlc";
        //        }

        //        viewer.LocalReport.EnableExternalImages = true;
        //        ReportDataSource rds1 = new ReportDataSource("dt", dt);
        //        //viewer.ProcessingMode = ProcessingMode.Local;
        //        //using (StreamReader rdlcSR = new StreamReader(viewer.LocalReport.ReportPath))
        //        //{
        //        //    viewer.LocalReport.LoadReportDefinition(rdlcSR);
        //        //    viewer.LocalReport.Refresh();
        //        //}
        //        //rv.LocalReport.DataSources.Add(rds);


        //        viewer.LocalReport.DataSources.Add(rds1);

        //viewer.LocalReport.SetParameters(new ReportParameter("OwnerName", OwnerName));
        //        viewer.LocalReport.SetParameters(new ReportParameter("ToOwnerCompanyName", ToOwnerCompanyName));
        //        viewer.LocalReport.SetParameters(new ReportParameter("RefNumber", RefNumber));
        //        viewer.LocalReport.SetParameters(new ReportParameter("CompanyABN", CompanyABN));
        //        viewer.LocalReport.SetParameters(new ReportParameter("CompanyABNReseller", CompanyABNReseller));
        //        viewer.LocalReport.SetParameters(new ReportParameter("InoviceDate", InoviceDate));
        //        viewer.LocalReport.SetParameters(new ReportParameter("InvoiceNumber", InvoiceNumber));
        //        viewer.LocalReport.SetParameters(new ReportParameter("AmountDue", AmountDue));
        //        viewer.LocalReport.SetParameters(new ReportParameter("Total", Total));
        //        viewer.LocalReport.SetParameters(new ReportParameter("DueDate", DueDate));
        //        viewer.LocalReport.SetParameters(new ReportParameter("ToAddressLine1", ToAddressLine1));
        //        viewer.LocalReport.SetParameters(new ReportParameter("ToAddressLine2", ToAddressLine2));
        //        viewer.LocalReport.SetParameters(new ReportParameter("ToAddressLine3", ToAddressLine3));
        //        viewer.LocalReport.SetParameters(new ReportParameter("FromAddressLine1", FromAddressLine1));
        //        viewer.LocalReport.SetParameters(new ReportParameter("FromAddressLine2", FromAddressLine2));
        //        viewer.LocalReport.SetParameters(new ReportParameter("FromAddressLine3", FromAddressLine3));
        //        viewer.LocalReport.SetParameters(new ReportParameter("JobDate", JobDate));
        //        viewer.LocalReport.SetParameters(new ReportParameter("JobDescription", JobDescription));
        //        viewer.LocalReport.SetParameters(new ReportParameter("JobTitle", JobTitle));
        //        viewer.LocalReport.SetParameters(new ReportParameter("JobAddress", JobAddress));
        //        viewer.LocalReport.SetParameters(new ReportParameter("LogoPath", LogoPath));
        //        viewer.LocalReport.SetParameters(new ReportParameter("InvoiceFooter", InvoiceFooter));
        //        viewer.LocalReport.SetParameters(new ReportParameter("ToName", ToName));
        //        viewer.LocalReport.SetParameters(new ReportParameter("FromName", FromName));
        //        viewer.LocalReport.SetParameters(new ReportParameter("IsStcInvoice", IsStcInvoice));
        //        viewer.LocalReport.SetParameters(new ReportParameter("ABN", lblABN));
        //        //viewer.LocalReport.SetParameters(new ReportParameter("FromCompanyName", FromCompanyName));
        //        //viewer.LocalReport.SetParameters(new ReportParameter("ToCompanyName", ToCompanyName));

        //        viewer.LocalReport.SetParameters(new ReportParameter("AccountName", AccountName));
        //        viewer.LocalReport.SetParameters(new ReportParameter("BSB", BSB));
        //        viewer.LocalReport.SetParameters(new ReportParameter("AccountNumber", AccountNumber));
        //        if (IsWholeSaler)
        //        {
        //            viewer.LocalReport.SetParameters(new ReportParameter("AccountNameReseller", AccountNameReseller));
        //            viewer.LocalReport.SetParameters(new ReportParameter("BSBReseller", BSBReseller));
        //            viewer.LocalReport.SetParameters(new ReportParameter("AccountNumberReseller", AccountNumberReseller));
        //            viewer.LocalReport.SetParameters(new ReportParameter("Reference_WholeSaler", Reference_WholeSaler));
        //            viewer.LocalReport.SetParameters(new ReportParameter("CompanyABNReseller", CompanyABN));
        //            viewer.LocalReport.SetParameters(new ReportParameter("FromCompanyName", FromCompanyName));
        //        }
        //        else
        //        {
        //            viewer.LocalReport.SetParameters(new ReportParameter("ToCompanyName", ToCompanyName));
        //        }

        //        if (IsPeakPay)
        //        {
        //            viewer.LocalReport.SetParameters(new ReportParameter("PeakPayFee", Convert.ToString(PeakPayFee)));
        //            viewer.LocalReport.SetParameters(new ReportParameter("PeakPayGst", Convert.ToString(PeakPayGst)));
        //        }
        //        //viewer.LocalReport.GetParameters();
        //        //ReportParameterInfoCollection reportParameterInfo = viewer.LocalReport.GetParameters();
        //        //viewer.LocalReport.Refresh();
        //        //try
        //        //{
        //        //    //Warning[] warnings1;
        //        //    //string[] streamids1;
        //        //    //string mimeType1 = string.Empty;
        //        //    //string encoding1 = string.Empty;
        //        //    //string extension1 = string.Empty;
        //        //    //byte[] bytes = viewer.LocalReport.Render("PDF", null, out mimeType1, out encoding1, out extension1, out streamids1, out warnings1);
        //        //    byte[] bytes = viewer.LocalReport.Render(ExportType, null, out mimeType, out encoding, out extension, out streamIds, out warnings);
        //        //    path = ByteArrayToFile(InvoiceNo, bytes, jobid, IsWindowService);
        //        //}
        //        //catch(Exception ex)
        //        //{

        //        //}
        //        viewer.LocalReport.Refresh();
        //        byte[] bytes = viewer.LocalReport.Render(ExportType, null, out mimeType, out encoding, out extension, out streamIds, out warnings);
        //        path = ByteArrayToFile(InvoiceNo, bytes, jobid, IsWindowService);


        //        if (RegenerateRemittanceFile)
        //        {
        //            DataSet remittanceData = _stcInvoiceBal.RegenerateRemittanceFile(Convert.ToInt32(resellerId), InvoiceNo);
        //            //DeleteDirectory(Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + "JobDocuments" + "\\" + remittanceData.Tables[0].Rows[0]["JobId"] + "\\" + "Invoice" + "\\" + "Report" + "\\" + "Remittance_" + remittanceData.Tables[0].Rows[0]["STCInvoicePaymentID"] + ".pdf"));
        //            if (remittanceData.Tables[0].Rows.Count > 0)
        //                for (int i = 0; i < remittanceData.Tables[0].Rows.Count; i++)
        //                {
        //                    MoveDeletedDocuments(Path.Combine(ProjectConfiguration.ProofDocumentsURL + "\\" + "JobDocuments" + "\\" + remittanceData.Tables[0].Rows[i]["JobId"] + "\\" + "Invoice" + "\\" + "Report" + "\\" + "Remittance_" + remittanceData.Tables[0].Rows[i]["STCInvoicePaymentID"] + ".pdf"), remittanceData.Tables[0].Rows[i]["JobId"].ToString());
        //                    //MoveDeletedDocuments(Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + "JobDocuments" + "\\" + remittanceData.Tables[0].Rows[i]["JobId"] + "\\" + "Invoice" + "\\" + "Report" + "\\" + "Remittance_" + remittanceData.Tables[0].Rows[i]["STCInvoicePaymentID"] + ".pdf"), remittanceData.Tables[0].Rows[i]["JobId"].ToString());
        //                }

        //            GenerateRemittance(remittanceData, resellerId);
        //        }

        //        // Now that you have all the bytes representing the PDF report, buffer it and send it to the client.
        //        //Response.Buffer = true;
        //        //Response.Clear();
        //        //Response.ClearHeaders();
        //        //Response.ContentType = mimeType;
        //        //Response.AddHeader("content-disposition", "attachment; filename=" + Filename + "." + extension);
        //        //Response.BinaryWrite(bytes); // create the file
        //        //Response.End(); // send it to the client to download
        //    }

        //    return path;
        //}

        public String CreateStcReportNew(string Filename, string ExportType, int STCJobDetailsID, string InvoiceNo, string solarCompanyId, string userTypeId, int userId, int rId, bool IsWindowService, bool RegenerateRemittanceFile = false, bool IsBackgroundRecProcess = false)
        {
            try
            {
                Common.Log("start create stc report new:STCJobDetailsID --" + STCJobDetailsID);
                Entity.STCInvoiceReport sTCInvoiceReport = new STCInvoiceReport();
                bool IsJobDescription = false;
                bool IsJobAddress = false;
                bool IsJobDate = false;
                bool IsTitle = false;
                bool IsName = false;
                bool IsTaxInclusive = false;
                decimal? TaxRate = 0;
                string LogoPath = string.Empty;

                Entity.Settings.Settings settings = new Entity.Settings.Settings();
                settings = GetSettingsData(solarCompanyId, userTypeId, userId, rId);
                Common.Log("AFter GetSettingData:");
                IsJobDescription = settings.IsJobDescription;
                IsJobAddress = settings.IsJobAddress;
                IsJobDate = settings.IsJobDate;
                IsTitle = settings.IsTitle;
                IsName = settings.IsName;
                IsTaxInclusive = settings.IsTaxInclusive;
                TaxRate = settings.TaxRate;

                sTCInvoiceReport = _stcInvoiceBal.GetStcInvoice(STCJobDetailsID, IsJobAddress, IsJobDate, IsJobDescription, IsTitle, IsName, DateTime.Now, InvoiceNo);

                Microsoft.Reporting.WebForms.Warning[] warnings;
                string[] streamIds;
                string mimeType = string.Empty;
                string encoding = string.Empty;
                string extension = string.Empty;
                ReportViewer viewer = new ReportViewer();
                string path = string.Empty;
                string lblABN = string.Empty;

                string Gst = sTCInvoiceReport.IsTaxInclusive.ToString() == "True" ? "+Gst" : "";

                if (!string.IsNullOrEmpty(sTCInvoiceReport.CompanyABN))
                {
                    lblABN = "ABN:";
                }
                if (sTCInvoiceReport.ResellerLogo != "" && sTCInvoiceReport.ResellerLogo != null)
                {
                    string LogoP = string.Empty;
                    if (IsWindowService)
                        LogoP = Convert.ToString(ConfigurationManager.AppSettings["ProofUploadFolder"]) + "\\UserDocuments" + "\\" + sTCInvoiceReport.SettingUserId + "\\" + sTCInvoiceReport.ResellerLogo;
                    else
                        LogoP = Path.Combine(ProjectConfiguration.UploadedDocumentPath + "\\UserDocuments" + "\\" + sTCInvoiceReport.SettingUserId, sTCInvoiceReport.ResellerLogo);
                    LogoPath = new Uri(LogoP).AbsoluteUri;
                }
                else
                {
                    LogoPath = "";
                }


                if (sTCInvoiceReport.IsPeakPay)
                {
                    if (IsWindowService || IsBackgroundRecProcess)
                        viewer.LocalReport.ReportPath = AppDomain.CurrentDomain.BaseDirectory + "//Reports//InvoiceSTC_PeakPay.rdlc";
                    else
                        viewer.LocalReport.ReportPath = @"Reports//InvoiceSTC_PeakPay.rdlc";
                }
                else if (sTCInvoiceReport.IsWholeSaler)
                {
                    if (IsWindowService || IsBackgroundRecProcess)
                        viewer.LocalReport.ReportPath = AppDomain.CurrentDomain.BaseDirectory + "//Reports//InvoiceSTC_WholeSaler.rdlc";
                    else
                        viewer.LocalReport.ReportPath = @"Reports//InvoiceSTC_WholeSaler.rdlc";
                }
                else
                {
                    if (IsWindowService || IsBackgroundRecProcess)
                        viewer.LocalReport.ReportPath = AppDomain.CurrentDomain.BaseDirectory + "//Reports//InvoiceSTC.rdlc";
                    else
                        viewer.LocalReport.ReportPath = @"Reports//InvoiceSTC.rdlc";
                }
                Common.Log("path for report creation: " + viewer.LocalReport.ReportPath);
                DataTable dt = new DataTable();
                dt.Columns.Add("Description", typeof(string));
                dt.Columns.Add("Quantity", typeof(int));
                dt.Columns.Add("Sale", typeof(decimal));
                dt.Columns.Add("PaymentAmount", typeof(decimal));
                dt.Columns.Add("IsTaxInclusive", typeof(bool));
                dt.Columns.Add("Tax", typeof(decimal));
                dt.Columns.Add("TotalAmount", typeof(decimal));
                int quntity = Convert.ToInt32(Convert.ToDouble(sTCInvoiceReport.Quantity));
                dt.Rows.Add(sTCInvoiceReport.Description, quntity, sTCInvoiceReport.Sale, 0, sTCInvoiceReport.IsTaxInclusive, sTCInvoiceReport.Tax, sTCInvoiceReport.TotalAmount);

                viewer.LocalReport.EnableExternalImages = true;
                ReportDataSource rds1 = new ReportDataSource("dt", dt);
                viewer.LocalReport.DataSources.Add(rds1);
                viewer.LocalReport.SetParameters(new ReportParameter("OwnerName", sTCInvoiceReport.OwnerName));
                viewer.LocalReport.SetParameters(new ReportParameter("ToOwnerCompanyName", sTCInvoiceReport.ToOwnerCompanyName));
                viewer.LocalReport.SetParameters(new ReportParameter("RefNumber", sTCInvoiceReport.RefNumber));
                viewer.LocalReport.SetParameters(new ReportParameter("CompanyABN", sTCInvoiceReport.CompanyABN));
                viewer.LocalReport.SetParameters(new ReportParameter("CompanyABNReseller", sTCInvoiceReport.CompanyABNReseller));
                viewer.LocalReport.SetParameters(new ReportParameter("InoviceDate", sTCInvoiceReport.InvoiceDate.ToString("dd MMMM yyyy")));
                viewer.LocalReport.SetParameters(new ReportParameter("InvoiceNumber", sTCInvoiceReport.STCInvoiceNumber));
                //viewer.LocalReport.SetParameters(new ReportParameter("AmountDue", sTCInvoiceReport.AmountDue));
                //viewer.LocalReport.SetParameters(new ReportParameter("Total", Total));
                viewer.LocalReport.SetParameters(new ReportParameter("DueDate", !string.IsNullOrEmpty(sTCInvoiceReport.DueDate.ToString()) ? sTCInvoiceReport.DueDate.ToString("dd MMMM yyyy") : string.Empty));
                viewer.LocalReport.SetParameters(new ReportParameter("ToAddressLine1", sTCInvoiceReport.ToAddressLine1));
                viewer.LocalReport.SetParameters(new ReportParameter("ToAddressLine2", sTCInvoiceReport.ToAddressLine2));
                viewer.LocalReport.SetParameters(new ReportParameter("ToAddressLine3", sTCInvoiceReport.ToAddressLine3));
                viewer.LocalReport.SetParameters(new ReportParameter("FromAddressLine1", sTCInvoiceReport.fromAddressLine1));
                viewer.LocalReport.SetParameters(new ReportParameter("FromAddressLine2", sTCInvoiceReport.fromAddressLine2));
                viewer.LocalReport.SetParameters(new ReportParameter("FromAddressLine3", sTCInvoiceReport.fromAddressLine3));
                viewer.LocalReport.SetParameters(new ReportParameter("JobDate", !string.IsNullOrEmpty(sTCInvoiceReport.JobDate.ToString()) ? sTCInvoiceReport.JobDate.ToString("dd MMMM yyyy") : string.Empty));
                viewer.LocalReport.SetParameters(new ReportParameter("JobDescription", sTCInvoiceReport.JobDescription));
                viewer.LocalReport.SetParameters(new ReportParameter("JobTitle", sTCInvoiceReport.JobTitle));
                viewer.LocalReport.SetParameters(new ReportParameter("JobAddress", sTCInvoiceReport.JobAddress));
                viewer.LocalReport.SetParameters(new ReportParameter("LogoPath", LogoPath));
                viewer.LocalReport.SetParameters(new ReportParameter("InvoiceFooter", sTCInvoiceReport.InvoiceFooter));
                //viewer.LocalReport.SetParameters(new ReportParameter("ToName", sTCInvoiceReport.ToName));
                //viewer.LocalReport.SetParameters(new ReportParameter("FromName", FromName));
                //viewer.LocalReport.SetParameters(new ReportParameter("IsStcInvoice", IsStcInvoice));
                viewer.LocalReport.SetParameters(new ReportParameter("ABN", lblABN));
                viewer.LocalReport.SetParameters(new ReportParameter("AccountName", sTCInvoiceReport.AccountName));
                viewer.LocalReport.SetParameters(new ReportParameter("BSB", sTCInvoiceReport.BSB));
                viewer.LocalReport.SetParameters(new ReportParameter("AccountNumber", sTCInvoiceReport.AccountNumber));
                viewer.LocalReport.SetParameters(new ReportParameter("ToCompanyName", sTCInvoiceReport.ToCompanyName));
                if (sTCInvoiceReport.IsWholeSaler)
                {
                    string Reference_WholeSaler = sTCInvoiceReport.SolarCompanyName + " - " + sTCInvoiceReport.RefNumber + " " + sTCInvoiceReport.STCPVDCode + " " + sTCInvoiceReport.Quantity + "@" + sTCInvoiceReport.Sale + Gst;
                    viewer.LocalReport.SetParameters(new ReportParameter("AccountNameReseller", sTCInvoiceReport.AccountNameReseller));
                    viewer.LocalReport.SetParameters(new ReportParameter("BSBReseller", sTCInvoiceReport.BSBReseller));
                    viewer.LocalReport.SetParameters(new ReportParameter("AccountNumberReseller", sTCInvoiceReport.AccountNumberReseller));
                    viewer.LocalReport.SetParameters(new ReportParameter("Reference_WholeSaler", Reference_WholeSaler));
                    viewer.LocalReport.SetParameters(new ReportParameter("CompanyABNReseller", sTCInvoiceReport.CompanyABN));
                    viewer.LocalReport.SetParameters(new ReportParameter("FromCompanyName", sTCInvoiceReport.FromCompanyName));
                }
                else
                {
                    viewer.LocalReport.SetParameters(new ReportParameter("ToCompanyName", sTCInvoiceReport.ToCompanyName));
                }
                if (sTCInvoiceReport.IsPeakPay)
                {
                    viewer.LocalReport.SetParameters(new ReportParameter("PeakPayFee", Convert.ToString(sTCInvoiceReport.PeakPayFee)));
                    viewer.LocalReport.SetParameters(new ReportParameter("PeakPayGst", Convert.ToString(sTCInvoiceReport.PeakPayGst)));
                }
                viewer.LocalReport.Refresh();
                byte[] bytes = viewer.LocalReport.Render(ExportType, null, out mimeType, out encoding, out extension, out streamIds, out warnings);
                path = ByteArrayToFile(InvoiceNo, bytes, sTCInvoiceReport.JobID.ToString(), IsWindowService);
                Common.Log("end report creation:Path--" + path);
                return path;
            }
            catch(Exception ex)
            {
                Common.Log(DateTime.Now.ToString() + "Exception: " + ex.Message);
                return "";
            }
        }

        //public int GenerateRemittance(DataSet remittanceData, string resellerId)
        //{
        //    string Filename = "Remittance_Invoice";
        //    string ExportType = "pdf";
        //    string pathName = string.Empty;
        //    string STCInvoicePaymentID = string.Empty;
        //    int response = 0;

        //    List<STCInvoicePayment> lstFilePath = new List<Entity.STCInvoicePayment>();

        //    DataTable dt = remittanceData.Tables[0];
        //    IDictionary<string, string> pathList = new Dictionary<string, string>();

        //    //ManualResetEvent[] completionEvents = new ManualResetEvent[dt.Rows.Count];
        //    //Task[] tasks = new Task[dt.Rows.Count];

        //    //int index = 0;
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        //int pageIndex = index;

        //        var serverPath = HttpContext.Current.Server.MapPath("~/");
        //        //tasks[pageIndex] = new Task(() =>
        //        //{
        //        DataTable dtTest = dt.Clone();
        //        dtTest.Rows.Add(dr.ItemArray);

        //        Microsoft.Reporting.WebForms.Warning[] warnings;
        //        string[] streamIds;
        //        string mimeType = string.Empty;
        //        string encoding = string.Empty;
        //        string extension = string.Empty;
        //        ReportViewer viewer = new ReportViewer();
        //        //XmlDocument oXD = new XmlDocument();

        //        ////oXD.Load(serverPath + "Reports\\StcRemittance.rdlc");
        //        //oXD.Load(serverPath + "Reports\\StcRemittance_WholeSaler.rdlc");

        //        DataSet ds = new DataSet();
        //        int report = 1;

        //        string PaymentDate = string.Empty;
        //        string SentDate = string.Empty;
        //        string ABN = string.Empty;
        //        string InvoiceDate = string.Empty;
        //        string Reference = string.Empty;
        //        string Reference_WholeSaler = string.Empty;
        //        string InvoiceTotal = string.Empty;
        //        string AmountPaid = string.Empty;
        //        string Stillwing = string.Empty;
        //        string JobId = string.Empty;
        //        string STCInvoiceID = string.Empty;
        //        //string XeroPaymentID = string.Empty;
        //        string CompanyName = string.Empty;
        //        string LogoPath = string.Empty;
        //        string Logo = string.Empty;
        //        int resellerUserId = 0;
        //        int solarCompanyId = 0;
        //        int solarCompanyUserId = 0;
        //        int rankNum = 0;
        //        string Gst = string.Empty;
        //        decimal NoOfStc = 0;
        //        decimal StcPrice = 0;
        //        bool IsWholeSaler = false;

        //        string STCInvoiceNumAsRefXERO = string.Empty;
        //        string STCInvoiceNum = string.Empty;
        //        string refNumberOwnerNameAddress = string.Empty;
        //        string STCPVDCode = string.Empty;
        //        string SolarCompanyName = string.Empty;


        //        if (dr != null)
        //        {

        //            PaymentDate = !string.IsNullOrEmpty(dr["PaymentDate"].ToString()) ? Convert.ToDateTime(dr["PaymentDate"]).ToString("dd MMMM yyyy") : string.Empty;
        //            SentDate = !string.IsNullOrEmpty(dr["SentDate"].ToString()) ? Convert.ToDateTime(dr["SentDate"]).ToString("dd MMMM yyyy") : string.Empty;
        //            ABN = !string.IsNullOrEmpty(dr["ABN"].ToString()) ? dr["ABN"].ToString() : string.Empty;
        //            InvoiceDate = !string.IsNullOrEmpty(dr["InvoiceCreatedDate"].ToString()) ? Convert.ToDateTime(dr["InvoiceCreatedDate"]).ToString("dd MMMM yyyy") : string.Empty;
        //            Reference = !string.IsNullOrEmpty(dr["Reference"].ToString()) ? dr["Reference"].ToString() : string.Empty;
        //            InvoiceTotal = !string.IsNullOrEmpty(dr["InvoiceTotal"].ToString()) ? dr["InvoiceTotal"].ToString() : string.Empty;
        //            AmountPaid = !string.IsNullOrEmpty(dr["TotalPaid"].ToString()) ? dr["TotalPaid"].ToString() : string.Empty;
        //            Stillwing = !string.IsNullOrEmpty(dr["StillOwing"].ToString()) ? dr["StillOwing"].ToString() : string.Empty;

        //            JobId = !string.IsNullOrEmpty(dr["JobId"].ToString()) ? dr["JobId"].ToString() : string.Empty;
        //            //XeroPaymentID = !string.IsNullOrEmpty(dr["XeroPaymentID"].ToString()) ? dr["XeroPaymentID"].ToString() : string.Empty;
        //            STCInvoiceID = !string.IsNullOrEmpty(dr["STCInvoiceID"].ToString()) ? dr["STCInvoiceID"].ToString() : string.Empty;
        //            CompanyName = !string.IsNullOrEmpty(dr["CompanyName"].ToString()) ? dr["CompanyName"].ToString() : string.Empty;
        //            STCInvoicePaymentID = !string.IsNullOrEmpty(dr["STCInvoicePaymentID"].ToString()) ? dr["STCInvoicePaymentID"].ToString() : string.Empty;
        //            Logo = !string.IsNullOrEmpty(dr["Logo"].ToString()) ? dr["Logo"].ToString() : string.Empty;
        //            resellerUserId = !string.IsNullOrEmpty(dr["ResellerUserId"].ToString()) ? Convert.ToInt32(dr["ResellerUserId"]) : 0;
        //            solarCompanyId = !string.IsNullOrEmpty(dr["SolarCompanyId"].ToString()) ? Convert.ToInt32(dr["SolarCompanyId"]) : 0;
        //            solarCompanyUserId = !string.IsNullOrEmpty(dr["SolarCompanyUserId"].ToString()) ? Convert.ToInt32(dr["SolarCompanyUserId"]) : 0;
        //            rankNum = !string.IsNullOrEmpty(dr["RankNumber"].ToString()) ? Convert.ToInt32(dr["RankNumber"]) : 0;
        //            Gst = dr["IsGst"].ToString() == "True" ? "+Gst" : "";
        //            NoOfStc = !string.IsNullOrEmpty(dr["CalculatedSTC"].ToString()) ? Convert.ToDecimal(dr["CalculatedSTC"]) : 0;
        //            StcPrice = !string.IsNullOrEmpty(dr["STCPrice"].ToString()) ? Convert.ToDecimal(dr["STCPrice"]) : 0;
        //            IsWholeSaler = Convert.ToBoolean(dr["IsWholeSaler"]);

        //            STCInvoiceNum = !string.IsNullOrEmpty(dr["STCInvoiceNumber"].ToString()) ? dr["STCInvoiceNumber"].ToString() : string.Empty;
        //            refNumberOwnerNameAddress = !string.IsNullOrEmpty(dr["description"].ToString()) ? dr["description"].ToString() : string.Empty;
        //            STCPVDCode = !string.IsNullOrEmpty(dr["STCPVDCode"].ToString()) ? dr["STCPVDCode"].ToString() : string.Empty;

        //            STCInvoiceNumAsRefXERO = STCInvoiceNum + " - " + NoOfStc + "STC@" + StcPrice + Gst + " - " + refNumberOwnerNameAddress + ", " + STCPVDCode;

        //        }
        //        if (Logo != "" && Logo != null)
        //        {
        //            //var LogoP = Path.Combine(ProjectSession.UploadedDocumentPath + "UserDocuments" + "\\" + resellerUserId, Logo);
        //            var LogoP = Path.Combine(ProjectConfiguration.UploadedDocumentPath + "UserDocuments" + "\\" + resellerUserId, Logo);
        //            LogoPath = new Uri(LogoP).AbsoluteUri;
        //        }
        //        else
        //        {
        //            LogoPath = "";
        //        }

        //        string FromAddressLine1 = string.Empty;
        //        string FromAddressLine2 = string.Empty;
        //        string FromAddressLine3 = string.Empty;
        //        string FromCompanyName = string.Empty;
        //        string ToAddressLine1 = string.Empty;
        //        string ToAddressLine2 = string.Empty;
        //        string ToAddressLine3 = string.Empty;
        //        string ToCompanyName = string.Empty;

        //        DataSet dsAddress = _stcInvoiceBal.GetSolarCompanyAndResellerAddress(solarCompanyId, string.IsNullOrEmpty(resellerId) ? 0 : Convert.ToInt32(resellerId));
        //        if (dsAddress != null && dsAddress.Tables.Count > 0 && dsAddress.Tables[0] != null && dsAddress.Tables[0].Rows.Count > 0)
        //        {
        //            ToAddressLine1 = !string.IsNullOrEmpty(dsAddress.Tables[0].Rows[0]["ToAddressLine1"].ToString()) ? dsAddress.Tables[0].Rows[0]["ToAddressLine1"].ToString() : string.Empty;
        //            ToAddressLine2 = !string.IsNullOrEmpty(dsAddress.Tables[0].Rows[0]["ToAddressLine2"].ToString()) ? dsAddress.Tables[0].Rows[0]["ToAddressLine2"].ToString() : string.Empty;
        //            ToAddressLine3 = !string.IsNullOrEmpty(dsAddress.Tables[0].Rows[0]["ToAddressLine3"].ToString()) ? dsAddress.Tables[0].Rows[0]["ToAddressLine3"].ToString() : string.Empty;
        //            ToCompanyName = !string.IsNullOrEmpty(dsAddress.Tables[0].Rows[0]["ToCompanyName"].ToString()) ? dsAddress.Tables[0].Rows[0]["ToCompanyName"].ToString() : string.Empty;
        //            SolarCompanyName = !string.IsNullOrEmpty(dsAddress.Tables[0].Rows[0]["SolarCompanyName"].ToString()) ? dsAddress.Tables[0].Rows[0]["SolarCompanyName"].ToString() : string.Empty;
        //        }
        //        if (dsAddress != null && dsAddress.Tables.Count > 0 && dsAddress.Tables[1] != null && dsAddress.Tables[1].Rows.Count > 0)
        //        {
        //            FromAddressLine1 = !string.IsNullOrEmpty(dsAddress.Tables[1].Rows[0]["FormAddressline1"].ToString()) ? dsAddress.Tables[1].Rows[0]["FormAddressline1"].ToString() : string.Empty;
        //            FromAddressLine2 = !string.IsNullOrEmpty(dsAddress.Tables[1].Rows[0]["FormAddressline2"].ToString()) ? dsAddress.Tables[1].Rows[0]["FormAddressline2"].ToString() : string.Empty;
        //            FromAddressLine3 = !string.IsNullOrEmpty(dsAddress.Tables[1].Rows[0]["FormAddressline3"].ToString()) ? dsAddress.Tables[1].Rows[0]["FormAddressline3"].ToString() : string.Empty;
        //            FromCompanyName = !string.IsNullOrEmpty(dsAddress.Tables[1].Rows[0]["FromCompanyName"].ToString()) ? dsAddress.Tables[1].Rows[0]["FromCompanyName"].ToString() : string.Empty;
        //        }


        //        //Reference_WholeSaler = ToCompanyName + " - " + Reference + " " + NoOfStc + "@" + StcPrice + Gst;
        //        Reference_WholeSaler = SolarCompanyName + " - " + Reference + " " + NoOfStc + "@" + StcPrice + Gst;

        //        if (IsWholeSaler)
        //        {
        //            viewer.LocalReport.ReportPath = serverPath + "Reports\\StcRemittance_WholeSaler.rdlc";
        //        }
        //        else
        //        {
        //            viewer.LocalReport.ReportPath = serverPath + "Reports\\StcRemittance.rdlc"; //@"Reports//StcRemittance.rdlc";
        //        }
        //        viewer.LocalReport.EnableExternalImages = true;
        //        // LocalReport.EnableExternalImages = true;
        //        ReportDataSource rds1 = new ReportDataSource("dt", dtTest);
        //        viewer.LocalReport.DataSources.Add(rds1);

        //        viewer.LocalReport.SetParameters(new ReportParameter("PaymentDate", PaymentDate));
        //        //viewer.LocalReport.SetParameters(new ReportParameter("Reference", Reference));
        //        viewer.LocalReport.SetParameters(new ReportParameter("SentDate", SentDate));
        //        viewer.LocalReport.SetParameters(new ReportParameter("ABN", ABN));
        //        viewer.LocalReport.SetParameters(new ReportParameter("InvoiceDate", InvoiceDate));
        //        viewer.LocalReport.SetParameters(new ReportParameter("InvoiceTotal", InvoiceTotal));
        //        viewer.LocalReport.SetParameters(new ReportParameter("AmountPaid", AmountPaid));
        //        viewer.LocalReport.SetParameters(new ReportParameter("Stillwing", Stillwing));
        //        viewer.LocalReport.SetParameters(new ReportParameter("CompanyName", CompanyName));
        //        viewer.LocalReport.SetParameters(new ReportParameter("LogoPath", LogoPath));

        //        viewer.LocalReport.SetParameters(new ReportParameter("FormAddressline1", FromAddressLine1));
        //        viewer.LocalReport.SetParameters(new ReportParameter("FormAddressline2", FromAddressLine2));
        //        viewer.LocalReport.SetParameters(new ReportParameter("FormAddressline3", FromAddressLine3));
        //        viewer.LocalReport.SetParameters(new ReportParameter("FromCompanyName", FromCompanyName));
        //        viewer.LocalReport.SetParameters(new ReportParameter("ToAddressLine1", ToAddressLine1));
        //        viewer.LocalReport.SetParameters(new ReportParameter("ToAddressLine2", ToAddressLine2));
        //        viewer.LocalReport.SetParameters(new ReportParameter("ToAddressLine3", ToAddressLine3));
        //        viewer.LocalReport.SetParameters(new ReportParameter("ToCompanyName", ToCompanyName));
        //        if (IsWholeSaler)
        //            viewer.LocalReport.SetParameters(new ReportParameter("Reference_WholeSaler", Reference_WholeSaler));
        //        else
        //            //viewer.LocalReport.SetParameters(new ReportParameter("Reference", Reference));
        //            viewer.LocalReport.SetParameters(new ReportParameter("Reference", STCInvoiceNumAsRefXERO));

        //        viewer.LocalReport.Refresh();

        //        byte[] bytes = viewer.LocalReport.Render(ExportType, null, out mimeType, out encoding, out extension, out streamIds, out warnings);
        //        //Save Report

        //        pathName = ByteArrayToFile("Remittance_" + STCInvoicePaymentID, bytes, JobId, false);
        //        pathList.Add(STCInvoicePaymentID, pathName);

        //        if (!string.IsNullOrEmpty(STCInvoicePaymentID) && !string.IsNullOrEmpty(pathName))
        //            lstFilePath.Add(new Entity.STCInvoicePayment { STCInvoicePaymentID = Convert.ToInt64(STCInvoicePaymentID), FilePath = pathName, SolarCompanyId = solarCompanyId, SolarCompanyUserId = solarCompanyUserId, ResellerUserId = resellerUserId, RankNumber = rankNum });

        //        //completionEvents[pageIndex].Set();
        //        //});

        //        //tasks[pageIndex].Start();

        //        //index++;
        //    }

        //    //Task.WaitAll(tasks);

        //    if (lstFilePath.Count > 0)
        //    {
        //        string STCInvoicePaymentJson = Newtonsoft.Json.JsonConvert.SerializeObject(lstFilePath);
        //        response = _stcInvoiceBal.UpdateFilePath(STCInvoicePaymentJson);

        //        //for (int i = 0; i < lstFilePath.Count; i++)
        //        //{
        //        //    string emailId = string.Empty;
        //        //    int toUserId = 0;
        //        //    int fromUserId = 0;
        //        //    //if (!string.IsNullOrEmpty(Convert.ToString(lstFilePath[i].SolarCompanyUserId)))
        //        //    //{
        //        //    //    int.TryParse(QueryString.GetValueFromQueryString(Convert.ToString(lstFilePath[i].SolarCompanyUserId), "id"), out toUserId);
        //        //    //}
        //        //    var dsToUsers = _userBAL.GetUserById(lstFilePath[i].SolarCompanyUserId);
        //        //    FormBot.Entity.User toUserDetail = DBClient.DataTableToList<FormBot.Entity.User>(dsToUsers.Tables[0]).FirstOrDefault();

        //        //    //if (!string.IsNullOrEmpty(Convert.ToString(lstFilePath[i].ResellerUserId)))
        //        //    //{
        //        //    //    int.TryParse(QueryString.GetValueFromQueryString(Convert.ToString(lstFilePath[i].ResellerUserId), "id"), out fromUserId);
        //        //    //}
        //        //    var dsFromUsers = _userBAL.GetUserById(lstFilePath[i].ResellerUserId);
        //        //    FormBot.Entity.User FromUserDetail = DBClient.DataTableToList<FormBot.Entity.User>(dsFromUsers.Tables[0]).FirstOrDefault();

        //        //    string newFileName = "Remittance_" + lstFilePath[i].RankNumber + ".pdf";
        //        //    bool mailStatus = sendRemittanceFile(lstFilePath[i].FilePath, toUserDetail.Email, FromUserDetail.Email, newFileName, ProjectSession.LoggedInName, toUserDetail.FirstName + " " + toUserDetail.LastName);
        //        //}
        //    }
        //    return response;
        //}

        public int GenerateRemittanceNew(Remittance remittanceData, string resellerId)
        {
            Microsoft.Reporting.WebForms.Warning[] warnings;
            string[] streamIds;
            string mimeType = string.Empty;
            string encoding = string.Empty;
            string extension = string.Empty;
            string Reference_WholeSaler = string.Empty;
            string STCInvoiceNumAsRefXERO = string.Empty;
            string Gst = string.Empty;
            string LogoPath = string.Empty;
            string Filename = "Remittance_Invoice";
            string ExportType = "pdf";
            string pathName = string.Empty;
            string STCInvoicePaymentID = string.Empty;
            int response = 0;
            ReportViewer viewer = new ReportViewer();
            List<STCInvoicePayment> lstFilePath = new List<Entity.STCInvoicePayment>();
            IDictionary<string, string> pathList = new Dictionary<string, string>();
            var serverPath = HttpContext.Current.Server.MapPath("~/");

            Gst = remittanceData.IsGst.ToString() == "True" ? "+Gst" : "";

            STCInvoiceNumAsRefXERO = remittanceData.STCInvoiceNumber + " - " + remittanceData.CalculatedSTC + "STC@" + remittanceData.STCPrice + Gst + " - " + remittanceData.description + ", " + remittanceData.STCPVDCode;
            if (remittanceData.Logo != "" && remittanceData.Logo != null)
            {

                var LogoP = Path.Combine(ProjectConfiguration.UploadedDocumentPath + "UserDocuments" + "\\" + remittanceData.ResellerUserId, remittanceData.Logo);
                LogoPath = new Uri(LogoP).AbsoluteUri;
            }
            else
            {
                LogoPath = "";
            }

            if (remittanceData.IsWholeSaler)
            {
                Reference_WholeSaler = remittanceData.ToCompanyName + " - " + remittanceData.Reference + " " + remittanceData.CalculatedSTC + "@" + remittanceData.STCPrice + Gst;
                viewer.LocalReport.ReportPath = serverPath + "Reports\\StcRemittance_WholeSaler.rdlc";
            }
            else
            {
                viewer.LocalReport.ReportPath = serverPath + "Reports\\StcRemittance.rdlc"; //@"Reports//StcRemittance.rdlc";
            }
            viewer.LocalReport.EnableExternalImages = true;

            DataTable dt = new DataTable();
            dt.Columns.Add("InvoiceDate", typeof(string));
            dt.Columns.Add("Reference", typeof(string));
            dt.Columns.Add("InvoiceTotal", typeof(decimal));
            dt.Columns.Add("AmountPaid", typeof(decimal));
            dt.Columns.Add("Stillwing", typeof(decimal));

            dt.Rows.Add(remittanceData.InvoiceCreatedDate, remittanceData.Reference, remittanceData.InvoiceTotal, remittanceData.AmountPaid, remittanceData.StillOwing);
            // LocalReport.EnableExternalImages = true;
            ReportDataSource rds1 = new ReportDataSource("dt", dt);
            viewer.LocalReport.DataSources.Add(rds1);

            viewer.LocalReport.SetParameters(new ReportParameter("PaymentDate", remittanceData.PaymentDate.ToString("dd MMMM yyyy")));
            //viewer.LocalReport.SetParameters(new ReportParameter("Reference", Reference));
            viewer.LocalReport.SetParameters(new ReportParameter("SentDate", remittanceData.SentDate.ToString("dd MMMM yyyy")));
            viewer.LocalReport.SetParameters(new ReportParameter("ABN", remittanceData.ABN));
            viewer.LocalReport.SetParameters(new ReportParameter("InvoiceDate", remittanceData.InvoiceCreatedDate.ToString("dd MMMM yyyy")));
            viewer.LocalReport.SetParameters(new ReportParameter("InvoiceTotal", remittanceData.InvoiceTotal.ToString()));
            viewer.LocalReport.SetParameters(new ReportParameter("AmountPaid", remittanceData.AmountPaid.ToString()));
            viewer.LocalReport.SetParameters(new ReportParameter("Stillwing", remittanceData.StillOwing.ToString()));
            viewer.LocalReport.SetParameters(new ReportParameter("CompanyName", remittanceData.CompanyName));
            viewer.LocalReport.SetParameters(new ReportParameter("LogoPath", LogoPath));

            viewer.LocalReport.SetParameters(new ReportParameter("FormAddressline1", remittanceData.fromAddressLine1));
            viewer.LocalReport.SetParameters(new ReportParameter("FormAddressline2", remittanceData.fromAddressLine2));
            viewer.LocalReport.SetParameters(new ReportParameter("FormAddressline3", remittanceData.fromAddressLine3));
            viewer.LocalReport.SetParameters(new ReportParameter("FromCompanyName", remittanceData.FromCompanyName));
            viewer.LocalReport.SetParameters(new ReportParameter("ToAddressLine1", remittanceData.ToAddressLine1));
            viewer.LocalReport.SetParameters(new ReportParameter("ToAddressLine2", remittanceData.ToAddressLine2));
            viewer.LocalReport.SetParameters(new ReportParameter("ToAddressLine3", remittanceData.ToAddressLine3));
            viewer.LocalReport.SetParameters(new ReportParameter("ToCompanyName", remittanceData.ToCompanyName));
            if (remittanceData.IsWholeSaler)
                viewer.LocalReport.SetParameters(new ReportParameter("Reference_WholeSaler", Reference_WholeSaler));
            else
                viewer.LocalReport.SetParameters(new ReportParameter("Reference", STCInvoiceNumAsRefXERO));

            viewer.LocalReport.Refresh();

            byte[] bytes = viewer.LocalReport.Render(ExportType, null, out mimeType, out encoding, out extension, out streamIds, out warnings);

            pathName = ByteArrayToFile("Remittance_" + remittanceData.STCInvoicePaymentID, bytes, remittanceData.JobId.ToString(), false);
            pathList.Add(remittanceData.STCInvoicePaymentID.ToString(), pathName);

            if (!string.IsNullOrEmpty(remittanceData.STCInvoicePaymentID.ToString()) && !string.IsNullOrEmpty(pathName))
                lstFilePath.Add(new Entity.STCInvoicePayment { STCInvoicePaymentID = Convert.ToInt64(remittanceData.STCInvoicePaymentID), FilePath = pathName, SolarCompanyId = remittanceData.SolarCompanyId, SolarCompanyUserId = remittanceData.SolarCompanyUserId, ResellerUserId = remittanceData.ResellerUserId, RankNumber = Convert.ToInt32(remittanceData.RankNumber) });

            if (lstFilePath.Count > 0)
            {
                string STCInvoicePaymentJson = Newtonsoft.Json.JsonConvert.SerializeObject(lstFilePath);
                response = _stcInvoiceBal.UpdateFilePath(STCInvoicePaymentJson);
            }
            return response;
        }

        public Entity.Settings.Settings GetSettingsData(string SolarCompanyId, string UserTypeId, int UserId, int ResellerId)
        {
            SettingsBAL settingsBAL = new SettingsBAL();
            Entity.Settings.Settings settings = new Entity.Settings.Settings();
            int? solarCompanyId = (!string.IsNullOrEmpty(SolarCompanyId)) ? Convert.ToInt32(SolarCompanyId) : 0;

            settings = settingsBAL.GetChargesPartsPaymentCodeAndSettings(Convert.ToInt32(UserId), Convert.ToInt32(UserTypeId), solarCompanyId, Convert.ToInt32(ResellerId));
            return settings;
        }

        public String ByteArrayToFile(string _FileName, byte[] _ByteArray, string jobID, bool isWindowService)
        {
            try
            {
                string physicalPath = string.Empty;
                string filePath = string.Empty;
                if (isWindowService)
                {
                    physicalPath = Convert.ToString(ConfigurationManager.AppSettings["ProofUploadFolder"]) + "\\" + "JobDocuments" + "\\" + jobID + "\\" + "Invoice\\Report" + "\\" + _FileName;
                    filePath = "JobDocuments" + "\\" + jobID + "\\" + "Invoice\\Report" + "\\" + _FileName + ".pdf";
                }
                else
                {
                    //physicalPath = ProjectSession.ProofDocumentsURL + "\\" + "JobDocuments" + "\\" + jobID + "\\" + "Invoice\\Report" + "\\" + _FileName;
                    physicalPath = ProjectConfiguration.ProofDocumentsURL + "\\" + "JobDocuments" + "\\" + jobID + "\\" + "Invoice\\Report" + "\\" + _FileName;
                    filePath = "JobDocuments" + "\\" + jobID + "\\" + "Invoice\\Report" + "\\" + _FileName + ".pdf";
                }

                if (!Directory.Exists(Path.GetDirectoryName(physicalPath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(physicalPath));
                // Open file for reading

                System.IO.File.WriteAllBytes(physicalPath + ".pdf", _ByteArray);

                //System.IO.FileStream _FileStream =
                //   new System.IO.FileStream(physicalPath, System.IO.FileMode.Create,
                //                            System.IO.FileAccess.Write);
                //// Writes a block of bytes to this stream using data from
                //// a byte array.
                //_FileStream.Write(_ByteArray, 0, _ByteArray.Length);

                //// close file stream
                //_FileStream.Close();

                return filePath;
            }
            catch (Exception _Exception)
            {
                // Error
                Console.WriteLine("Exception caught in process: {0}",
                                  _Exception.ToString());
            }

            // error occured, return false
            return string.Empty;
        }

        ///// <summary>
        ///// Deletes the directory.
        ///// </summary>
        ///// <param name="path">The path.</param>
        //private void DeleteDirectory(string path)
        //{
        //    if (System.IO.File.Exists(path))
        //    {
        //        ////Delete all files from the Directory
        //        System.IO.File.Delete(path);
        //    }
        //}

        /// <summary>
        /// Moves the deleted documents.
        /// </summary>
        /// <param name="path">The path.</param>
        public string MoveDeletedDocuments(string sourcePath, string JobId, string UserId = null)
        {
            //string fileName = Path.GetFileName(sourcePath);
            string fileName = Path.GetFileNameWithoutExtension(sourcePath) + "_" + DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss") + Path.GetExtension(sourcePath);

            //string pathInfo = sourcePath.Split(new string[] { "JobDocuments\\" }, StringSplitOptions.None)[1].ToString();
            //string JobId = Convert.ToString(Directory.GetParent(pathInfo).Parent);
            string destPath = string.Empty;
            if (System.IO.File.Exists(sourcePath))
            {
                string destinationDirectory = string.Empty;
                //string destinationDirectory = ProjectSession.ProofDocumentsURL + "\\" + "JobDocuments" + "\\" + JobId + "\\" + "DeletedDocuments";
                if (!string.IsNullOrEmpty(UserId))
                {
                    //destinationDirectory = ProjectSession.ProofDocumentsURL + "\\" + "UserDocuments" + "\\" + UserId + "\\" + "DeletedDocuments";
                    destinationDirectory = ProjectConfiguration.ProofDocumentsURL + "\\" + "UserDocuments" + "\\" + UserId + "\\" + "DeletedDocuments";
                }
                else
                {
                    //destinationDirectory = ProjectSession.ProofDocumentsURL + "\\" + "JobDocuments" + "\\" + JobId + "\\" + "DeletedDocuments";
                    destinationDirectory = ProjectConfiguration.ProofDocumentsURL + "\\" + "JobDocuments" + "\\" + JobId + "\\" + "DeletedDocuments";
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
                if (!Directory.Exists(destinationDirectory))
                {
                    Directory.CreateDirectory(destinationDirectory);
                }
                destPath = System.IO.Path.Combine(destinationDirectory, fileName);
                System.IO.File.Copy(sourcePath, destPath, true);

                System.IO.File.Delete(sourcePath);
            }
            if (!string.IsNullOrEmpty(destPath) && string.IsNullOrEmpty(UserId))
                return (destPath.Split(new string[] { "JobDocuments\\" }, StringSplitOptions.None)[1].ToString());
            else
                return "";
        }

    }
}

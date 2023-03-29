using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using PdfSharp;
using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using Directory = System.IO.Directory;

namespace FormBot.Helper.Helper
{
    public static class Common
    {
        private static object locker = new object();
        private static readonly ILogger _log;
        static Common()
        {
            _log = new Logger();
        }

        public static bool IsExistsInArray(int[] array, int checkId)
        {
            return Array.IndexOf(array, checkId) > -1;
        }

        public static string GetDescription(object enumValue, string defDesc)
        {
            FieldInfo fi = enumValue.GetType().GetField(enumValue.ToString());

            if (fi != null)
            {
                object[] attrs = fi.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return defDesc;
        }

        public static string GetSubDescription(object enumValue, string defDesc)
        {
            FieldInfo fi = enumValue.GetType().GetField(enumValue.ToString());

            if (fi != null)
            {
                object[] attrs = fi.GetCustomAttributes(typeof(FormBot.Helper.SystemEnums.SubDescriptionAttribute), true);
                if (attrs != null && attrs.Length > 0)
                {
                    return ((FormBot.Helper.SystemEnums.SubDescriptionAttribute)attrs[0]).SubDescription;
                }
            }

            return defDesc;
        }

        public static DataTable GetInvoiceDueDate()
        {
            DataTable dtInvoiceDate = new DataTable();

            dtInvoiceDate.Columns.Add("Id", typeof(int));
            dtInvoiceDate.Columns.Add("Name", typeof(string));

            dtInvoiceDate.Rows.Add(new object[] { 1, "Same day as job" });
            dtInvoiceDate.Rows.Add(new object[] { 2, "Same day as invoice" });
            dtInvoiceDate.Rows.Add(new object[] { 3, "7 days after invoice" });
            dtInvoiceDate.Rows.Add(new object[] { 4, "14 days after invoice" });
            dtInvoiceDate.Rows.Add(new object[] { 5, "30 days after invoice" });
            dtInvoiceDate.Rows.Add(new object[] { 6, "60 days after invoice" });
            dtInvoiceDate.Rows.Add(new object[] { 7, "20th of month" });
            dtInvoiceDate.Rows.Add(new object[] { 8, "20th of following month" });
            dtInvoiceDate.Rows.Add(new object[] { 9, "30th of month" });
            dtInvoiceDate.Rows.Add(new object[] { 10, "30th of following month" });
            dtInvoiceDate.Rows.Add(new object[] { 11, "1st of following month +45 days" });

            return dtInvoiceDate;
        }

        public static void WriteErrorLog(string str)
        {
            try
            {
                StreamWriter swData2 = new StreamWriter(ProjectSession.EmailErrorLogFilePath, true);
                swData2.WriteLine(str);
                swData2.Close();
                swData2.Dispose();

            }
            catch (Exception)
            {
            }
        }

        public static DateTime SettingDueDate(int InvoiceDueDateId)
        {
            DateTime dueDate = DateTime.Now;
            if (InvoiceDueDateId == 1)
            {
                dueDate = DateTime.Now;
            }
            else if (InvoiceDueDateId == 2)
            {
                dueDate = DateTime.Now;
            }
            else if (InvoiceDueDateId == 3)
            {
                dueDate = DateTime.Now.AddDays(7);
            }
            else if (InvoiceDueDateId == 4)
            {
                dueDate = DateTime.Now.AddDays(14);
            }
            else if (InvoiceDueDateId == 5)
            {
                dueDate = DateTime.Now.AddDays(30);
            }
            else if (InvoiceDueDateId == 6)
            {
                dueDate = DateTime.Now.AddDays(60);
            }
            else if (InvoiceDueDateId == 7)
            {
                dueDate = Convert.ToDateTime(string.Format("{0}-{1}-20", DateTime.Now.Year, DateTime.Now.Month));
            }
            else if (InvoiceDueDateId == 8)
            {
                dueDate = Convert.ToDateTime(string.Format("{0}-{1}-20", DateTime.Now.Year, DateTime.Now.AddMonths(1).Month));
            }
            else if (InvoiceDueDateId == 9)
            {
                dueDate = Convert.ToDateTime(string.Format("{0}-{1}-30", DateTime.Now.Year, DateTime.Now.Month));
            }
            else if (InvoiceDueDateId == 10)
            {
                dueDate = Convert.ToDateTime(string.Format("{0}-{1}-30", DateTime.Now.Year, DateTime.Now.AddMonths(1).Month));
            }
            else if (InvoiceDueDateId == 11)
            {
                dueDate = Convert.ToDateTime(string.Format("{0}-{1}-{2}", DateTime.Now.Year, DateTime.Now.AddMonths(1).Month, DateTime.Now.AddMonths(1).AddDays(45)));
            }
            return dueDate;
        }

        public static string StringToCSVCell(string dbColumn)
        {
            if (!string.IsNullOrEmpty(dbColumn))
            {
                bool mustQuote = (dbColumn.Contains(",") || dbColumn.Contains("\"") || dbColumn.Contains("\r") || dbColumn.Contains("\n"));
                if (mustQuote)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("\"");
                    foreach (char nextChar in dbColumn)
                    {
                        sb.Append(nextChar);
                        if (nextChar == '"')
                            sb.Append("\"");
                    }
                    sb.Append("\"");
                    return sb.ToString();
                }
                if (dbColumn.StartsWith("0"))
                {
                    dbColumn ="\t"+ dbColumn.ToString().PadLeft(4, '0');
                }

            }
            return dbColumn;
        }

        private static void WriteToLogFile(string content)
        {
            //set up a filestream
            FileStream fs = new FileStream(ProjectSession.EmailErrorLogFilePath, FileMode.OpenOrCreate, FileAccess.Write);
            //set up a streamwriter for adding text
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.BaseStream.Seek(0, SeekOrigin.End);
                //add the text
                sw.WriteLine(content);
                //add the text to the underlying filestream
                sw.Flush();
                //close the writer
                sw.Close();
            }
            //StreamWriter sw = new StreamWriter(fs);
            //find the end of the underlying filestream            
        }
        public static IDictionary<string, string> GetEnumValuesWithDescription<T>(this Type type) where T : struct, IConvertible
        {
            Type enumType = typeof(T);
            Type descriptionAttributeType = typeof(DescriptionAttribute);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (string memberName in Enum.GetNames(enumType))
            {
                MemberInfo member = enumType.GetMember(memberName).Single();

                string memberDescription = ((DescriptionAttribute)Attribute.GetCustomAttribute(member, descriptionAttributeType)).Description;


                dict.Add(memberDescription, memberDescription);
            }
            return dict;
        }


        //public static void UploadPdfAndSave(string type, string JobId, string VisitChecklistItemId, string PDFName, string ImagePath, string UserId)
        //{
        //    string Type = type == "2" ? "CES" : "OTHER";
        //    string DocPath = Path.Combine("JobDocuments", JobId, Type, VisitChecklistItemId, PDFName.ToLower().Contains(".pdf") ? PDFName : (PDFName + ".pdf"));

        //    string Source = Path.Combine(ConfigurationManager.AppSettings["ProofUploadFolder"].ToString(), ImagePath);
        //    string Destination = Path.Combine(ConfigurationManager.AppSettings["ProofUploadFolder"].ToString(), DocPath);

        //    bool isFileExists = File.Exists(Destination);
        //    DataSet dsFiles = _createJobBAL.GetCesPhotosByVisitCheckListId(Convert.ToInt32(VisitChecklistItemId));
        //    generatePDFfromImage(dsFiles.Tables[0], Destination);

        //    //delete pdf file
        //    if (dsFiles.Tables[0].Rows.Count < 1)
        //    {

        //    }

        //    if (!isFileExists)
        //        _createJobBAL.InsertCESDocuments(Convert.ToInt32(JobId), DocPath, Convert.ToInt32(UserId), Type);

        //}

        public static void generatePDFfromImage(DataTable dt, string Destination)
        {

            //Bitmap bmp = WebsiteThumbnailImageGenerator.GetWebSiteThumbnail(address);

            if (!Directory.Exists(Directory.GetParent(Destination).ToString()))
            {
                Directory.CreateDirectory(Directory.GetParent(Destination).ToString());
            }
            else
            {
                if (File.Exists(Destination))
                {
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    File.Delete(Destination);
                }
            }

            PdfSharp.Pdf.PdfDocument doc = new PdfSharp.Pdf.PdfDocument();
            //PdfPage pg = new PdfPage();
            //pg.Size = (PdfSharp.PageSize)PageSize;
            foreach (DataRow dr in dt.Rows)
            {

                string Source = Path.Combine(ConfigurationManager.AppSettings["ProofUploadFolder"].ToString(), dr["Path"].ToString());

                if (!File.Exists(Source))
                {
                    continue;
                }
                double height2 = 0;// bmp.Height;
                PdfSharp.Pdf.PdfPage pg = new PdfSharp.Pdf.PdfPage();
                pg.Size = (PdfSharp.PageSize)PageSize.A4;
                bool FitInSinglePage = false;

                doc.Pages.Add(pg);
                XGraphics xgr = XGraphics.FromPdfPage(pg);
                XImage img = XImage.FromFile(Source);

                double resHeight = 0;
                if (pg.Width.Value < img.PointWidth)
                {
                    resHeight = (pg.Width.Value * img.PointHeight) / img.PointWidth;
                }
                else
                {
                    resHeight = img.PixelHeight;
                }
                double heightScale = 1;
                if (pg.Height.Value > img.PointHeight)
                {
                    heightScale = img.PointHeight;
                }
                else
                {
                    heightScale = img.PointHeight;
                }
                if (FitInSinglePage)
                {
                    xgr.DrawImage(img, 0, 0, pg.Width.Value, pg.Height.Value);
                    //doc.Save(Destination);
                    //   doc.Close();
                    img.Dispose();
                    img = null;
                    xgr.Dispose();
                    xgr = null;

                    doc.Dispose();
                    doc = null;
                    //  System.IO.File.Delete(FullPath + ".png");
                    break;
                }
                else
                {
                    xgr.DrawImage(img, 0, 0, pg.Width.Value, resHeight);
                }

                height2 = resHeight;
                int i = 1;

                double calcHeight = resHeight;
                if (img.PointHeight > pg.Height.Value)
                {
                    height2 = height2 - pg.Height.Value;
                    while (height2 % pg.Height.Value > 0)
                    {
                        pg = new PdfSharp.Pdf.PdfPage();
                        pg.Size = (PdfSharp.PageSize)PageSize.A4;

                        doc.Pages.Add(pg);
                        //if (pg.Height.Value > height2)
                        //{
                        //    heightScale = img.PointHeight;
                        //}
                        //else
                        //{
                        //    heightScale = img.PointHeight;
                        //}
                        xgr = XGraphics.FromPdfPage(doc.Pages[i]);
                        xgr.DrawImage(img, 0, -pg.Height.Value * i, pg.Width.Value, resHeight);
                        i = i + 1;
                        height2 = height2 - pg.Height.Value;
                    }
                }

                img.Dispose();
                img = null;
                xgr.Dispose();
                xgr = null;
            }


            doc.Save(Destination);
            doc.Close();

            doc.Dispose();
            doc = null;
            //System.IO.File.Delete(FullPath + ".png");

        }
        public static void generatePDFfromImageCustomImageHightAndWidthRatio(DataTable dt, string Destination)
        {

            //Bitmap bmp = WebsiteThumbnailImageGenerator.GetWebSiteThumbnail(address);

            if (!Directory.Exists(Directory.GetParent(Destination).ToString()))
            {
                Directory.CreateDirectory(Directory.GetParent(Destination).ToString());
            }
            else
            {
                if (File.Exists(Destination))
                {
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    File.Delete(Destination);
                }
            }

            PdfSharp.Pdf.PdfDocument doc = new PdfSharp.Pdf.PdfDocument();
            //PdfPage pg = new PdfPage();
            //pg.Size = (PdfSharp.PageSize)PageSize;
            foreach (DataRow dr in dt.Rows)
            {

                string Source = Path.Combine(ConfigurationManager.AppSettings["ProofUploadFolder"].ToString(), dr["Path"].ToString());

                if (!File.Exists(Source))
                {
                    continue;
                }
                double height2 = 0;// bmp.Height;
                PdfSharp.Pdf.PdfPage pg = new PdfSharp.Pdf.PdfPage();
                pg.Size = (PdfSharp.PageSize)PageSize.A4;
                bool FitInSinglePage = false;

                doc.Pages.Add(pg);
                XGraphics xgr = XGraphics.FromPdfPage(pg);
                XImage img = XImage.FromFile(Source);

                double resHeight = 0;
                double resWidth = 0;
                if (pg.Width.Value < img.PointWidth)
                {
                    resHeight = (pg.Width.Value * img.PointHeight) / img.PointWidth;
                }
                else
                {
                    resHeight = img.PixelHeight;
                }
                double heightScale = 1;
                if (pg.Height.Value > img.PointHeight)
                {
                    heightScale = img.PointHeight;
                }
                else
                {
                    heightScale = img.PointHeight;
                }



                if (pg.Width.Value < img.PointWidth)
                {
                    resWidth = pg.Width.Value;//(pg.Height.Value * img.PointWidth) / img.PixelHeight;
                }
                else
                {
                    resWidth = img.PointWidth;
                }

                if (FitInSinglePage)
                {
                    xgr.DrawImage(img, 0, 0, pg.Width.Value, pg.Height.Value);
                    //doc.Save(Destination);
                    //   doc.Close();
                    img.Dispose();
                    img = null;
                    xgr.Dispose();
                    xgr = null;

                    doc.Dispose();
                    doc = null;
                    //  System.IO.File.Delete(FullPath + ".png");
                    break;
                }
                else
                {
                    xgr.DrawImage(img, 0, 0, resWidth, resHeight);
                }

                height2 = resHeight;
                int i = 1;

                double calcHeight = resHeight;
                if (img.PointHeight > pg.Height.Value)
                {
                    height2 = height2 - pg.Height.Value;
                    while (height2 % pg.Height.Value > 0)
                    {
                        pg = new PdfSharp.Pdf.PdfPage();
                        pg.Size = (PdfSharp.PageSize)PageSize.A4;

                        doc.Pages.Add(pg);
                        //if (pg.Height.Value > height2)
                        //{
                        //    heightScale = img.PointHeight;
                        //}
                        //else
                        //{
                        //    heightScale = img.PointHeight;
                        //}
                        xgr = XGraphics.FromPdfPage(doc.Pages[i]);
                        xgr.DrawImage(img, 0, -pg.Height.Value * i, pg.Width.Value, resHeight);
                        i = i + 1;
                        height2 = height2 - pg.Height.Value;
                    }
                }

                img.Dispose();
                img = null;
                xgr.Dispose();
                xgr = null;
            }


            doc.Save(Destination);
            doc.Close();

            doc.Dispose();
            doc = null;
            //System.IO.File.Delete(FullPath + ".png");

        }
        public static DateTime? GetSettlementDate(int settlementterm, ref string Days)
        {
            try
            {
                DateTime? STCSettlementDate = null;
                string dayName = DateTime.Now.DayOfWeek.ToString();
                TimeSpan cutOffTime = TimeSpan.Parse(ProjectConfiguration.SettlementCutOffTime);
                TimeSpan onApprovalCutOffTime = TimeSpan.Parse(ProjectConfiguration.OnApprovalSettlementCutOffTime);
                TimeSpan currentTime = DateTime.Now.TimeOfDay;
                DateTime currentDate = DateTime.Now;

                //As per disucssion with Hus, display exact settlement term with which job has been traded in job history
                Days = Enum.GetName(typeof(SystemEnums.STCSettlementTerm), settlementterm);

                if (FormBot.Helper.SystemEnums.STCSettlementTerm.Days3.GetHashCode() == settlementterm
                    || FormBot.Helper.SystemEnums.STCSettlementTerm.Commercial.GetHashCode() == settlementterm
                    || FormBot.Helper.SystemEnums.STCSettlementTerm.RapidPay.GetHashCode() == settlementterm)
                {
                    //Days = "3 Days";
                    if (dayName.ToLower() == "saturday")
                    {
                        STCSettlementDate = currentDate.AddDays(4);
                    }
                    else if (dayName.ToLower() == "sunday")
                    {
                        STCSettlementDate = currentDate.AddDays(3);
                    }
                    else
                    {
                        if (currentTime <= cutOffTime)
                        {
                            if (dayName.ToLower() == "monday" || dayName.ToLower() == "tuesday" || dayName.ToLower() == "wednesday")
                            {
                                STCSettlementDate = currentDate.AddDays(2);
                            }
                            else if (dayName.ToLower() == "thursday" || dayName.ToLower() == "friday")
                            {
                                STCSettlementDate = currentDate.AddDays(4);
                            }
                        }
                        else
                        {
                            if (dayName.ToLower() == "monday" || dayName.ToLower() == "tuesday")
                            {
                                STCSettlementDate = currentDate.AddDays(3);
                            }
                            else if (dayName.ToLower() == "wednesday" || dayName.ToLower() == "thursday" || dayName.ToLower() == "friday")
                            {
                                STCSettlementDate = currentDate.AddDays(5);
                            }
                        }
                    }
                }
                else if (FormBot.Helper.SystemEnums.STCSettlementTerm.Hour24.GetHashCode() == settlementterm)
                {
                    //Days = "24 Hours";
                    if (dayName.ToLower() == "saturday")
                    {
                        STCSettlementDate = currentDate.AddDays(3);
                    }
                    else if (dayName.ToLower() == "sunday")
                    {
                        STCSettlementDate = currentDate.AddDays(2);
                    }
                    else
                    {
                        if (currentTime <= cutOffTime)
                        {
                            if (dayName.ToLower() == "monday" || dayName.ToLower() == "tuesday" || dayName.ToLower() == "wednesday" || dayName.ToLower() == "thursday")
                            {
                                STCSettlementDate = currentDate.AddDays(1);
                            }
                            else if (dayName.ToLower() == "friday")
                            {
                                STCSettlementDate = currentDate.AddDays(3);
                            }
                        }
                        else
                        {
                            if (dayName.ToLower() == "monday" || dayName.ToLower() == "tuesday" || dayName.ToLower() == "wednesday")
                            {
                                STCSettlementDate = currentDate.AddDays(2);
                            }
                            else if (dayName.ToLower() == "thursday" || dayName.ToLower() == "friday")
                            {
                                STCSettlementDate = currentDate.AddDays(4);
                            }
                        }
                    }
                }
                else if (FormBot.Helper.SystemEnums.STCSettlementTerm.Days7.GetHashCode() == settlementterm)
                {
                    //Days = "7 Days";
                    if (dayName.ToLower() == "monday" || dayName.ToLower() == "tuesday" || dayName.ToLower() == "wednesday" || dayName.ToLower() == "thursday" || dayName.ToLower() == "friday")
                    {
                        STCSettlementDate = currentDate.AddDays(7);
                    }
                    else if (dayName.ToLower() == "saturday")
                    {
                        STCSettlementDate = currentDate.AddDays(2);
                    }
                    else if (dayName.ToLower() == "sunday")
                    {
                        STCSettlementDate = currentDate.AddDays(1);
                    }
                    else
                    {
                        STCSettlementDate = null;
                    }
                }
                else if (FormBot.Helper.SystemEnums.STCSettlementTerm.CERApproved.GetHashCode() == settlementterm
                    || FormBot.Helper.SystemEnums.STCSettlementTerm.OptiPay.GetHashCode() == settlementterm
                    || FormBot.Helper.SystemEnums.STCSettlementTerm.InvoiceStc.GetHashCode() == settlementterm)
                {
                    //Days = "";
                    STCSettlementDate = null;
                }
                else if (FormBot.Helper.SystemEnums.STCSettlementTerm.PartialPayments.GetHashCode() == settlementterm)
                {
                    //Days = "";
                    STCSettlementDate = DateTime.Now.AddDays(1);
                }
                else if (FormBot.Helper.SystemEnums.STCSettlementTerm.UpFront.GetHashCode() == settlementterm)
                {
                    //Days = "";
                    STCSettlementDate = DateTime.Now.AddDays(1);
                }

                return STCSettlementDate;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static DataTable GetCustomFieldDetail()
        {
            DataTable dtCustomField = new DataTable();
            dtCustomField.Columns.Add("JobId", typeof(int));
            dtCustomField.Columns.Add("JobCustomFieldId", typeof(int));
            dtCustomField.Columns.Add("CustomValue", typeof(string));
            dtCustomField.Columns.Add("CreatedBy", typeof(int));
            dtCustomField.Columns.Add("CreatedDate", typeof(DateTime));
            dtCustomField.Columns.Add("ModifiedBy", typeof(int));
            dtCustomField.Columns.Add("ModifiedDate", typeof(DateTime));
            dtCustomField.Columns.Add("IsDeleted", typeof(bool));
            dtCustomField.Columns.Add("VendorJobCustomFieldId", typeof(string));
            dtCustomField.Columns.Add("CustomField", typeof(string));

            return dtCustomField;
        }

        public static DataTable GetJobNotesDetail()
        {
            DataTable dtJobNotesField = new DataTable();
            dtJobNotesField.Columns.Add("JobId", typeof(int));
            dtJobNotesField.Columns.Add("Notes", typeof(string));
            dtJobNotesField.Columns.Add("CreatedBy", typeof(int));
            dtJobNotesField.Columns.Add("CreatedDate", typeof(DateTime));
            dtJobNotesField.Columns.Add("IsSeen", typeof(bool));
            dtJobNotesField.Columns.Add("JobSchedulingId", typeof(int));
            dtJobNotesField.Columns.Add("VendorJobNoteId", typeof(int));

            return dtJobNotesField;
        }

        public static DataTable CreateUserWiseColumnsTable()
        {
            DataTable dtColumns = new DataTable();
            dtColumns.Columns.Add("UserID", typeof(int));
            dtColumns.Columns.Add("ColumnID", typeof(int));
            dtColumns.Columns.Add("Width", typeof(float));
            dtColumns.Columns.Add("MenuID", typeof(int));
            dtColumns.Columns.Add("OrderNumber", typeof(int));
            dtColumns.Columns.Add("PageSize", typeof(int));
            return dtColumns;
        }

        /// <summary>
        /// Convert image to base64
        /// </summary>
        /// <returns></returns>
        public static string ImageToBase64(string path)
        {
            string base64String = null;
            using (System.Drawing.Image image = System.Drawing.Image.FromFile(path))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();
                    base64String = Convert.ToBase64String(imageBytes);
                    return base64String;
                }
            }
        }

        /// <summary>
        /// Get Latitude and longitude value from property of image
        /// </summary>
        /// <param name="propItem"></param>
        /// <returns></returns>
        private static double? GetLatitudeAndLongitude(PropertyItem propItem)
        {
            try
            {
                uint degreesNumerator = BitConverter.ToUInt32(propItem.Value, 0);
                uint degreesDenominator = BitConverter.ToUInt32(propItem.Value, 4);
                uint minutesNumerator = BitConverter.ToUInt32(propItem.Value, 8);
                uint minutesDenominator = BitConverter.ToUInt32(propItem.Value, 12);
                uint secondsNumerator = BitConverter.ToUInt32(propItem.Value, 16);
                uint secondsDenominator = BitConverter.ToUInt32(propItem.Value, 20);
                return (Convert.ToDouble(degreesNumerator) / Convert.ToDouble(degreesDenominator)) + (Convert.ToDouble(Convert.ToDouble(minutesNumerator) / Convert.ToDouble(minutesDenominator)) / 60) +
                       (Convert.ToDouble((Convert.ToDouble(secondsNumerator) / Convert.ToDouble(secondsDenominator)) / 3600));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void GetLatitudeLongitudeValueFromImage(string imagePath, ref string latitude, ref string longitude)
        {
            //start           
            double? imageLat = null;
            double? imageLon = null;
            try
            {
                var directories = ImageMetadataReader.ReadMetadata(imagePath);

                var gps = directories.OfType<GpsDirectory>().FirstOrDefault();

                if (gps != null)
                {
                    var location = gps.GetGeoLocation();

                    if (location != null)
                    {
                        imageLat = location.Latitude;
                        imageLon = location.Longitude;
                    }
                }
                latitude = imageLat != null ? Convert.ToString(imageLat) : null;
                longitude = imageLon != null ? Convert.ToString(imageLon) : null;
            }
            catch (Exception ex)
            {
                //WriteToLogFile("GetLatLon " + DateTime.Now.ToString() + ex.Message);
                _log.LogException("GetLatLon " + DateTime.Now.ToString(), ex);
            }
            //end

            //Bitmap bmp = new Bitmap(imagePath);
            //try
            //{
            //    // set Variable Values
            //    double? imageLat = null;
            //    double? imageLon = null;
            //    foreach (PropertyItem propItem in bmp.PropertyItems)
            //    {
            //        switch (propItem.Type)
            //        {
            //            case 5:
            //                if (propItem.Id == 2) // Latitude Array
            //                    imageLat = GetLatitudeAndLongitude(propItem);
            //                if (propItem.Id == 4) //Longitude Array
            //                    imageLon = GetLatitudeAndLongitude(propItem);
            //                break;

            //        }
            //    }
            //    latitude = imageLat != null ? Convert.ToString(imageLat) : null;
            //    longitude = imageLon != null ? Convert.ToString(imageLon) : null;
            //    bmp.Dispose();
            //}
            //catch (Exception ex)
            //{
            //    bmp.Dispose();
            //}
        }

        public static void Log_backup(string text)
        {
            string directoryPath = AppDomain.CurrentDomain.BaseDirectory + "Log";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string path = Path.Combine(directoryPath, "ServiceLog - " + DateTime.Now.ToString("ddMMyyyy") + ".txt");
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(DateTime.Now.ToString() + Environment.NewLine + text + Environment.NewLine + "------------------------------------------------------------------------");
                writer.Close();
            }
        }
        public static void Log(string text)
        {
            string directoryPath = AppDomain.CurrentDomain.BaseDirectory + "Log";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string path = Path.Combine(directoryPath, "ServiceLog - " + DateTime.Now.ToString("ddMMyyyy") + ".txt");

            FileInfo fi = new FileInfo(path);

            lock (locker)
            {
                using (FileStream file = new FileStream(fi.FullName, FileMode.Append, FileAccess.Write, FileShare.Read))
                using (StreamWriter streamWriter = new StreamWriter(file))
                {
                    streamWriter.WriteLine(DateTime.Now.ToString() + Environment.NewLine + text + Environment.NewLine + "------------------------------------------------------------------------");
                    streamWriter.Close();
                }
            }
        }

        public static Dictionary<string, int> numberTable = new Dictionary<string, int>{
        {"zero",0},{"one",1},{"two",2},{"three",3},{"four",4},{"five",5},{"six",6},
        {"seven",7},{"eight",8},{"nine",9},{"ten",10},{"eleven",11},{"twelve",12},{"thirteen",13},{"fourteen",14},{"fifteen",15}
        };
        public static void SaveJobHistorytoXML(int JobID, string JobHistoryMessage, string Filter, string Category, string CreatedBy, bool isimportant = false, string description = null, string NoteID = null, string HistoryType = "Public", string CreatedDate = null, bool IsDeleted = false)
        {
            try
            {
                var JobHistoryXMLPath = Path.Combine(Path.Combine(ProjectConfiguration.ProofDocumentsURL, "StaticTemplate/SPV/JobHistory.xml"));
                string fullDirectoryPath = Path.Combine(ProjectConfiguration.ProofDocumentsURL, "JobDocuments", JobID.ToString(), "JobHistory");
                if (!Directory.Exists(fullDirectoryPath))
                    Directory.CreateDirectory(fullDirectoryPath);
                string fullFilePath = Path.Combine(fullDirectoryPath, "JobHistory_" + JobID.ToString() + ".xml");
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
                if (!string.IsNullOrEmpty(JobHistoryMessage))
                {
                    if (JobHistoryMessage.ToLower().Contains(" from ") && JobHistoryMessage.ToLower().Contains(" to "))
                    {
                        int indexFrom = JobHistoryMessage.ToLower().LastIndexOf(" from ");
                        int startIndexTo = JobHistoryMessage.ToLower().IndexOf(" to ");
                        int LastindexTO = JobHistoryMessage.ToLower().LastIndexOf(" to ");
                        string substrFrom = JobHistoryMessage.Substring(indexFrom + 5, startIndexTo - (indexFrom + 5));
                        string substrTo = JobHistoryMessage.Substring(LastindexTO + 3);
                        JobHistoryMessage = JobHistoryMessage.Replace(substrFrom, "<b style=\"color: black\">" + substrFrom + "</b>");

                        JobHistoryMessage = JobHistoryMessage.Replace(substrTo, "<b style=\"color: black\">" + substrTo + "</b>");

                    }
                    else if (JobHistoryMessage.ToLower().Contains(" to "))
                    {
                        int LastindexTO = JobHistoryMessage.ToLower().LastIndexOf(" to ");
                        string substrTo = JobHistoryMessage.Substring(LastindexTO + 3);
                        JobHistoryMessage = JobHistoryMessage.Replace(substrTo, "<b style=\"color: black\">" + substrTo + "</b>");
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

                        root.Add(new XElement("JobID", Convert.ToString(JobID)));
                        root.Add(new XElement("JobHistoryMessage", JobHistoryMessage));
                        root.Add(new XElement("Filter", Filter));
                        root.Add(new XElement("Category", Category));
                        root.Add(new XElement("Description", description));
                        root.Add(new XElement("IsImportant", isimportant.ToString()));
                        root.Add(new XElement("CreatedBy", CreatedBy));
                        root.Add(new XElement("CreatedDate", Date));
                        root.Add(new XElement("HistoryType", HistoryType));
                        root.Add(new XElement("NoteID", NoteID));
                        root.Add(new XElement("IsDeleted", Convert.ToString(IsDeleted)));
                        olddoc.Element("JobHistory").Add(root);
                        xml.Dispose();
                        xml.Close();
                        olddoc.Save(fullFilePath);
                        
                    }

                    //XDocument olddoc = XDocument.Load(fullFilePath);
                    //XElement root = new XElement("History");

                    //root.Add(new XElement("JobID", Convert.ToString(JobID)));
                    //root.Add(new XElement("JobHistoryMessage", JobHistoryMessage));
                    //root.Add(new XElement("Filter", Filter));
                    //root.Add(new XElement("Category", Category));
                    //root.Add(new XElement("Description", description));
                    //root.Add(new XElement("IsImportant", isimportant.ToString()));
                    //root.Add(new XElement("CreatedBy", CreatedBy));
                    //root.Add(new XElement("CreatedDate", Date));
                    //root.Add(new XElement("HistoryType", HistoryType));
                    //root.Add(new XElement("NoteID", NoteID));
                    //root.Add(new XElement("IsDeleted", Convert.ToString(IsDeleted)));
                    //olddoc.Element("JobHistory").Add(root);
                    //olddoc.Save(fullFilePath);

                }
                //System.IO.File.Delete(fullFilePath);
                else
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.PreserveWhitespace = true;
                    xmlDoc.Load(JobHistoryXMLPath);
                    XDocument doc = XDocument.Parse(xmlDoc.InnerXml);
                    xmlDoc.InnerXml = doc.ToString();
                    xmlDoc.InnerXml = xmlDoc.InnerXml.Replace("[[JobID]]", JobID.ToString())
                                   .Replace("[[JobHistoryMessage]]", HttpUtility.HtmlEncode(JobHistoryMessage))
                                   .Replace("[[Filter]]", Filter)
                                   .Replace("[[Category]]", Category)
                                   .Replace("[[Description]]", HttpUtility.HtmlEncode(description))
                                   .Replace("[[IsImportant]]", isimportant.ToString())
                                    .Replace("[[CreatedBy]]", CreatedBy)
                                    .Replace("[[CreatedDate]]", Date)
                                    .Replace("[[HistoryType]]", HistoryType)
                                    .Replace("[[NoteID]]", NoteID)
                                    .Replace("[[IsDeleted]]", Convert.ToString(IsDeleted));


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
                FormBot.Helper.Log.WriteError(e, "Exception in SaveJobHistoryInXML.." + DateTime.Now.ToString());
                Console.WriteLine(e.Message);
            }
        }
        public static void SaveSTCJobHistorytoXML(int JobID, string JobHistoryMessage, string Description, int STCStatusID, string Filter, string Category, string CreatedBy, bool isimportant = false)
        {
            try
            {
                var JobHistoryXMLPath = Path.Combine(Path.Combine(ProjectConfiguration.ProofDocumentsURL, "StaticTemplate/SPV/STCJobHistory.xml"));
                string fullDirectoryPath = Path.Combine(ProjectConfiguration.ProofDocumentsURL, "JobDocuments", JobID.ToString(), "JobHistory");
                if (!Directory.Exists(fullDirectoryPath))
                    Directory.CreateDirectory(fullDirectoryPath);
                string fullFilePath = Path.Combine(fullDirectoryPath, "STCJobHistory_" + JobID.ToString() + ".xml");
                //JobHistoryMessage = JobHistoryMessage + " on " + DateTime.Now.ToString("MMM dd yyyy h:mmtt");
                if (System.IO.File.Exists(fullFilePath))
                {
                    XDocument olddoc = XDocument.Load(fullFilePath);
                    XElement root = new XElement("History");

                    root.Add(new XElement("JobID", Convert.ToString(JobID)));
                    root.Add(new XElement("JobHistoryMessage", JobHistoryMessage));
                    root.Add(new XElement("Description", Description));
                    root.Add(new XElement("STCStatusID", STCStatusID.ToString()));
                    root.Add(new XElement("Filter", Filter));
                    root.Add(new XElement("Category", Category));
                    root.Add(new XElement("IsImportant", isimportant.ToString()));
                    root.Add(new XElement("CreatedBy", CreatedBy));
                    root.Add(new XElement("CreatedDate", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz")));
                    olddoc.Element("JobSTCHistory").Add(root);
                    olddoc.Save(fullFilePath);
                }
                //System.IO.File.Delete(fullFilePath);
                else
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.PreserveWhitespace = true;
                    xmlDoc.Load(JobHistoryXMLPath);
                    XDocument doc = XDocument.Parse(xmlDoc.InnerXml);
                    xmlDoc.InnerXml = doc.ToString();
                    xmlDoc.InnerXml = xmlDoc.InnerXml.Replace("[[JobID]]", JobID.ToString())
                                   .Replace("[[JobHistoryMessage]]", HttpUtility.HtmlEncode(JobHistoryMessage))
                                   .Replace("[[Description]]", HttpUtility.HtmlEncode(Description))
                                   .Replace("[[STCStatusID]]", STCStatusID.ToString())
                                   .Replace("[[Filter]]", Filter)
                                   .Replace("[[Category]]", Category)
                                   .Replace("[[IsImportant]]", isimportant.ToString())
                                    .Replace("[[CreatedBy]]", CreatedBy)
                                    .Replace("[[CreatedDate]]", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));

                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Encoding = new UTF8Encoding(false);
                    settings.Indent = true;
                    using (XmlWriter writer = XmlWriter.Create(fullFilePath, settings))
                    {
                        xmlDoc.Save(writer);
                    }
                }
            }
            catch (Exception e)
            {
                FormBot.Helper.Log.WriteError(e, "Exception in SaveSTCJobHistoryInXML.." + DateTime.Now.ToString());
                Console.WriteLine(e.Message);
            }
        }

        public static void SaveUserHistorytoXML(int UserID, string UserHistoryMessage, string ModifiedBy, string ModifiedDate, int HistoryType = 1, bool IsWarning = false, string NoteID = null, bool IsDeleted = false)
        {
            try
            {
                var UserHistoryXMLPath = Path.Combine(Path.Combine(ProjectConfiguration.ProofDocumentsURL, "StaticTemplate/SPV/UserHistory.xml"));
                string fullDirectoryPath = Path.Combine(ProjectConfiguration.ProofDocumentsURL, "UserDocuments", UserID.ToString(), "UserHistory");
                if (!Directory.Exists(fullDirectoryPath))
                    Directory.CreateDirectory(fullDirectoryPath);
                string fullFilePath = Path.Combine(fullDirectoryPath, "UserHistory_" + UserID.ToString() + ".xml");
                if (System.IO.File.Exists(fullFilePath))
                {
                    XDocument olddoc = XDocument.Load(fullFilePath);
                    XElement root = new XElement("History");

                    root.Add(new XElement("UserID", Convert.ToString(UserID)));
                    root.Add(new XElement("UserHistoryMessage", UserHistoryMessage));

                    root.Add(new XElement("ModifiedBy", ModifiedBy));
                    root.Add(new XElement("ModifiedDate", ModifiedDate));
                    root.Add(new XElement("HistoryType", HistoryType.ToString()));
                    root.Add(new XElement("IsWarning", Convert.ToString(IsWarning)));
                    root.Add(new XElement("NoteID", NoteID));
                    root.Add(new XElement("IsDeleted", Convert.ToString(IsDeleted)));
                    olddoc.Element("UserHistory").Add(root);
                    olddoc.Save(fullFilePath);
                }
                //System.IO.File.Delete(fullFilePath);
                else
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.PreserveWhitespace = true;
                    xmlDoc.Load(UserHistoryXMLPath);
                    XDocument doc = XDocument.Parse(xmlDoc.InnerXml);
                    xmlDoc.InnerXml = doc.ToString();
                    xmlDoc.InnerXml = xmlDoc.InnerXml.Replace("[[UserID]]", UserID.ToString())
                                   .Replace("[[UserHistoryMessage]]", HttpUtility.HtmlEncode(UserHistoryMessage))

                                    .Replace("[[ModifiedBy]]", ModifiedBy)
                                    .Replace("[[ModifiedDate]]", ModifiedDate)
                                    .Replace("[[HistoryType]]", HistoryType.ToString())
                                    .Replace("[[IsWarning]]", Convert.ToString(IsWarning))
                                    .Replace("[[NoteID]]", NoteID)
                                    .Replace("[[IsDeleted]]", Convert.ToString(IsDeleted));


                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Encoding = new UTF8Encoding(false);
                    settings.Indent = true;
                    using (XmlWriter writer = XmlWriter.Create(fullFilePath, settings))
                    {
                        xmlDoc.Save(writer);
                    }
                }

            }

            catch (Exception e)
            {
                FormBot.Helper.Log.WriteError(e, "Exception in UserHistoryInXML.." + DateTime.Now.ToString());
                Console.WriteLine(e.Message);
            }

        }
/// <summary>
        /// Gets the wattage from model number.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns>string of wattage</returns>
        public static string GetWattageFromModelNumber(string pattern)
        {
            pattern = Regex.Replace(pattern, @"\([^()]*\)", string.Empty);
            string exprforXJData = @"XJ([\d]){3}([A-Z])";
            string exprforRisenData = @"-?([\d]){3}";
            string exprforYingliData = @"YL([\d]){3}D-?";
            string exprForCharacter = @"[\d]{3}[A-Z|/]{0,1}";
            string exprForDigitsAfterSlash = @"\/(\d+)$";
            string exprForThreeDigit = @"DM\d{3}";
            string exprForTwoDigit = @"([A-Z][\d]{2}[A-Z|/]{0,1})";
            string exprForOneDigit = @"([\d]{2}[A-Z|/]{0,1})";
            string digitExpr = @"([\d]+)";
            string digitafter_specificchar = @"(\d+)W$";
            string exprForDigitsAfterOneChar = @"NE([\d]+)[A-Z][\d]+";
            string exprForDigitsAfterPVChar = @"PV([\d]+)([A-Z]*)[\d]+";
            string exprForDigitsAfterYLchar = @"YL([\d]+){3}([A-Z]*)[\d]+";
            string testregx = @"(\d{3}$)(?=[^\/]*$)";

            MatchCollection specificWattString = Regex.IsMatch(pattern, digitafter_specificchar) ? Regex.Matches(pattern, digitafter_specificchar) : null;
            string result = string.Empty;
            if (specificWattString != null)
            {
                foreach (Match item in specificWattString)
                {
                    string output = Regex.Replace(item.Groups[0].Value, "[^0-9]+", string.Empty);
                    bool success = Int32.TryParse(output, out int number);

                    if (Convert.ToInt32(output) >= 100 || result == "")
                        result = output;

                }
            }
            MatchCollection digitsAfterYLSpecific = Regex.IsMatch(pattern, exprForDigitsAfterYLchar) ? Regex.Matches(pattern, exprForDigitsAfterYLchar) : null;
            if (digitsAfterYLSpecific != null)
            {
                foreach (Match item in digitsAfterYLSpecific)
                {
                    MatchCollection last3digits = Regex.Matches(item.Groups[0].Value, digitExpr);
                    foreach (Match m in last3digits)
                    {
                        string output = Regex.Replace(m.Groups[0].Value, "[^0-9]+", string.Empty);
                        bool success = Int32.TryParse(output, out int number);
                        result = output;
                        break;
                    }
                }
            }
            MatchCollection DigitsAfterOneChar = Regex.IsMatch(pattern, exprForDigitsAfterOneChar) ? Regex.Matches(pattern, exprForDigitsAfterOneChar) : Regex.IsMatch(pattern, exprForDigitsAfterPVChar) ? Regex.Matches(pattern, exprForDigitsAfterPVChar) : null;
            if (DigitsAfterOneChar != null)
            {
                foreach (Match item in DigitsAfterOneChar)
                {
                    MatchCollection last3digits = Regex.Matches(item.Groups[0].Value, testregx);
                    foreach (Match m in last3digits)
                    {
                        string output = Regex.Replace(m.Groups[0].Value, "[^0-9]+", string.Empty);
                        bool success = Int32.TryParse(output, out int number);

                        result = output;
                    }

                }
            }

            if (string.IsNullOrEmpty(result))
            {
                MatchCollection digitCollection = Regex.IsMatch(pattern, exprforXJData) ? Regex.Matches(pattern, exprforXJData) : null;
                MatchCollection digitCollectionRSM = Regex.IsMatch(pattern, exprforRisenData) ? Regex.Matches(pattern, exprforRisenData) : null;
                MatchCollection digitCollectionYingli = Regex.IsMatch(pattern, exprforYingliData) ? Regex.Matches(pattern, exprforYingliData) : null;
                if (digitCollection != null)
                {
                    string output = Regex.Replace(digitCollection[0].Groups[0].Value, "[^0-9]+", string.Empty);
                    bool success = Int32.TryParse(output, out int number);
                    result = success ? output : string.Empty;
                }
                else if (digitCollectionRSM != null && (pattern.StartsWith("RSM") || pattern.StartsWith("JAM") || pattern.StartsWith("AC-") || pattern.StartsWith("UL-") || pattern.StartsWith("SK") || pattern.StartsWith("VSUN")))
                {
                    string output = string.Empty;
                    if (pattern.StartsWith("RSM") || pattern.StartsWith("SK"))
                    {
                        if (digitCollectionRSM.Count == 1)
                            output = Regex.Replace(digitCollectionRSM[0].Groups[0].Value, "[^0-9]+", string.Empty);
                        else
                            output = Regex.Replace(digitCollectionRSM[1].Groups[0].Value, "[^0-9]+", string.Empty);
                    }
                    else if (pattern.StartsWith("JAM") || pattern.StartsWith("AC-") || pattern.StartsWith("UL-") || pattern.StartsWith("VSUN"))
                        output = Regex.Replace(digitCollectionRSM[0].Groups[0].Value, "[^0-9]+", string.Empty);
                    bool success = Int32.TryParse(output, out int number);
                    result = success ? output : string.Empty;
                }
                else if (digitCollectionYingli != null && pattern.StartsWith("YL"))
                {
                    string output = Regex.Replace(digitCollectionYingli[0].Groups[0].Value, "[^0-9]+", string.Empty);
                    bool success = Int32.TryParse(output, out int number);
                    result = success ? output : string.Empty;
                }
                else
                {
                    digitCollection = Regex.IsMatch(pattern, exprForCharacter) && Regex.IsMatch(pattern, exprForDigitsAfterSlash) ? Regex.Matches(pattern, exprForDigitsAfterSlash) : Regex.IsMatch(pattern, exprForCharacter) ? Regex.Matches(pattern, exprForCharacter) : Regex.IsMatch(pattern, exprForThreeDigit) ? Regex.Matches(pattern, exprForThreeDigit) : Regex.IsMatch(pattern, exprForTwoDigit) ? Regex.Matches(pattern, exprForTwoDigit) : Regex.Matches(pattern, exprForOneDigit);

                    if (digitCollection != null)
                    {
                        foreach (Match item in digitCollection)
                        {
                            string output = Regex.Replace(item.Groups[0].Value, "[^0-9]+", string.Empty);
                            bool success = Int32.TryParse(output, out int number);
                            if (success)
                            {
                                result = !string.IsNullOrEmpty(result) && Convert.ToInt32(result) > Convert.ToInt32(output) ? result : output;
                                //if ((Convert.ToInt32(output) >= 100 && Convert.ToInt32(output) <= 500) || (result == "") )
                                //    result = output;
                            }
                            else
                            {
                                result = output;
                            }
                        }

                        MatchCollection matchCollection2 = Regex.Matches(result, digitExpr);
                        foreach (Match m in matchCollection2)
                        {
                            MatchCollection mathCollection;
                            if (m.Length > 3)
                            {
                                mathCollection = Regex.IsMatch(pattern, exprForCharacter) && Regex.IsMatch(pattern, exprForDigitsAfterSlash) ? Regex.Matches(pattern, exprForDigitsAfterSlash) : Regex.IsMatch(pattern, exprForCharacter) ? Regex.Matches(pattern, exprForCharacter) : Regex.IsMatch(pattern, exprForThreeDigit) ? Regex.Matches(pattern, exprForThreeDigit) : Regex.IsMatch(pattern, exprForTwoDigit) ? Regex.Matches(pattern, exprForTwoDigit) : Regex.Matches(pattern, exprForOneDigit);
                                // mathCollection = Regex.IsMatch(pattern, exprForCharacter) && Regex.IsMatch(pattern, abc) ? Regex.Matches(pattern, abc) : null;
                            }
                            else
                            {
                                mathCollection = Regex.IsMatch(pattern, exprForCharacter) ? Regex.Matches(pattern, exprForCharacter) : Regex.IsMatch(pattern, exprForThreeDigit) ? Regex.Matches(pattern, exprForThreeDigit) : Regex.IsMatch(pattern, exprForTwoDigit) ? Regex.Matches(pattern, exprForTwoDigit) : Regex.Matches(pattern, exprForOneDigit);
                            }
                            foreach (Match m2 in mathCollection)
                            {
                                // result = m.Groups[0].Value;
                                string output = Regex.Replace(m2.Groups[0].Value, "[^0-9]+", string.Empty);
                                bool success = Int32.TryParse(output, out int number);
                                if (success)
                                {
                                    if ((Convert.ToInt32(output) >= 200 && Convert.ToInt32(output) <= 500) || result == "")
                                        result = output;
                                }
                                else
                                {
                                    result = output;
                                }

                            }

                        }


                        MatchCollection digitCollection_Digit = Regex.Matches(result, digitExpr);
                        //string testregx= @"(\d{3}$)(?=[^\/]*$)";

                        foreach (Match m in digitCollection_Digit)
                        {
                            if (m.Length > 3)
                            {
                                MatchCollection digitCollection1 = Regex.Matches(result, testregx);
                                foreach (Match m1 in digitCollection1)
                                {
                                    // result = m.Groups[0].Value;
                                    bool success = Int32.TryParse(m1.Groups[0].Value, out int number);
                                    if (success)
                                    {
                                        if ((Convert.ToInt32(m1.Groups[0].Value) >= 100 && Convert.ToInt32(m1.Groups[0].Value) <= 500) || result == "")
                                            result = m1.Groups[0].Value;
                                    }
                                }
                            }
                            else
                                result = m.Groups[0].Value;
                        }

                    }
                }
            }
            return result;
        }

        public static List<SelectListItem> GetInvoicer()
        {
            var Invoicerlist = new List<SelectListItem>
            {
                new SelectListItem{ Text="Select", Value = "0" },
                new SelectListItem{ Text="EMERGING ENERGY SOLUTIONS GROUP PTY. LTD.", Value = "2965" },
            };
            return Invoicerlist;
        }

        public static List<SelectListItem> GetUsageTypeList()
        {
            var UsageTypeList = new List<SelectListItem>
            {
                new SelectListItem{ Text="Reseller", Value = "1" },
                new SelectListItem{ Text="Wholesaler", Value = "2" },
                new SelectListItem{Text="SAAS", Value = "3"}
            };
            return UsageTypeList;
        }
        public static object ResetValue(object value, Type type)
        {
            if (type == typeof(bool) || type == typeof(bool?) || type == typeof(byte) || type == typeof(char) || type == typeof(int) || type == typeof(int?)
               || type == typeof(double) || type == typeof(double?) || type == typeof(short) || type == typeof(long) || type == typeof(sbyte)
               || type == typeof(decimal) || type == typeof(decimal?) || type == typeof(ushort) || type == typeof(uint) || type == typeof(byte[])
               || type == typeof(DateTime) || type == typeof(DateTime?))
                return ChangeType(value, type);
            else
                return value;
        }
        public static object ChangeType(object value, Type conversion)
        {
            var t = conversion;

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null || value == "" || value.ToString().Trim() == "")
                {
                    return null;
                }

                t = Nullable.GetUnderlyingType(t);
            }

            return t == typeof(bool) ? (value.ToString() == "1" || value.ToString().ToLower() == "true") : Convert.ChangeType(value, t);
        }
    }
}

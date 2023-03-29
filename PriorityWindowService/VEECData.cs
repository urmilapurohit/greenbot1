using FormBot.BAL.Service;
using FormBot.Helper;
using HtmlAgilityPack;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PriorityWindowService
{
    class VEECData
    {
        ICreateVeecBAL _veec = new CreateVeecBAL();
        public void SyncVEECScheduleActivityPremiseFromVEECPortal()
        {
            try
            {
                string postUrl = "https://www.veet.vic.gov.au/Public/SAPList.aspx";
                //WriteErrorLog("timer", "here");
                CookieContainer ck = new CookieContainer();
                Dictionary<string, string> postParameters = new Dictionary<string, string>();
                HttpWebRequest request = WebRequest.Create(postUrl) as HttpWebRequest;
                WebProxy myProxy = new WebProxy();
                // Set the Method property of the request to POST.
                request.Method = "GET";
                request.CookieContainer = ck;
                // Create POST data and convert it to a byte array.
                // Get the response.
                Uri newUri = new Uri("http://192.168.0.251:8080");
                // Associate the newUri object to 'myProxy' object so that new myProxy settings can be set.
                myProxy.Address = newUri;
                // Create a NetworkCredential object and associate it with the 
                // Proxy property of request object.
                request.Proxy = myProxy;
                WebResponse response = request.GetResponse();
                string cookie = response.Headers[HttpResponseHeader.SetCookie];
                Stream dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                // Display the content.
                // Clean up the streams.
                reader.Close();
                dataStream.Close();
                response.Close();
                HtmlDocument do2c = new HtmlDocument();
                do2c.LoadHtml(responseFromServer);
                postParameters.Add("__EVENTTARGET", "");
                postParameters.Add("__EVENTARGUMENT", "");
                postParameters.Add("__VIEWSTATE", do2c.GetElementbyId("__VIEWSTATE").GetAttributeValue("value", ""));
                postParameters.Add("__VIEWSTATEGENERATOR", do2c.GetElementbyId("__VIEWSTATEGENERATOR").GetAttributeValue("value", ""));
                postParameters.Add("__EVENTVALIDATION", do2c.GetElementbyId("__EVENTVALIDATION").GetAttributeValue("value", ""));
                postParameters.Add("ctl00$MenuHead", "{&quot;selectedItemIndexPath&quot;:&quot;&quot;,&quot;checkedState&quot;:&quot;&quot;}");
                postParameters.Add("ctl00$ContentPlaceHolder1$rbtnDisplay", "0");
                postParameters.Add("ctl00$ContentPlaceHolder1$rbtnDisplay$RB0", "C");
                postParameters.Add("ctl00$ContentPlaceHolder1$rbtnDisplay$RB1", "U");
                postParameters.Add("ctl00$ContentPlaceHolder1$rbtnDisplay$RB2", "U");
                postParameters.Add("ctl00$ContentPlaceHolder1$rbtnDisplay$RB3", "U");
                postParameters.Add("ctl00$ContentPlaceHolder1$rbtnDisplay$RB4", "U");
                postParameters.Add("ctl00$ContentPlaceHolder1$gvSAP", "{&quot;selection&quot;:&quot;&quot;,&quot;callbackState&quot;:&quot;BwUHAwIERGF0YQbuDwAAAAD/AQAA/wEAAAAAAAAZAAAAABMAAAAGRVJFUElEBkVSRVBJRAMAAAEIU3RhdHVzSUQJU3RhdHVzIElEAwAAAQ5FeHRlcm5hbFN0YXR1cw9FeHRlcm5hbCBTdGF0dXMHAAABDlNpdGVEZXNjcmlwdG9yD1NpdGUgRGVzY3JpcHRvcgcAAAEOUmVnaXN0ZXJlZE5hbWUPUmVnaXN0ZXJlZCBOYW1lBwAAAQhVbml0VHlwZQlVbml0IFR5cGUHAAABClVuaXROdW1iZXILVW5pdCBOdW1iZXIHAAABCUxldmVsVHlwZQpMZXZlbCBUeXBlBwAAAQtMZXZlbE51bWJlcgxMZXZlbCBOdW1iZXIHAAABDFN0cmVldE51bWJlcg1TdHJlZXQgTnVtYmVyBwAAAQpTdHJlZXROYW1lC1N0cmVldCBOYW1lBwAAAQpTdHJlZXRUeXBlC1N0cmVldCBUeXBlBwAAARBTdHJlZXRUeXBlU3VmZml4ElN0cmVldCBUeXBlIFN1ZmZpeAcAAAEKVG93blN1YnVyYgtUb3duIFN1YnVyYgcAAAEFU3RhdGUFU3RhdGUHAAABCFBvc3Rjb2RlCFBvc3Rjb2RlBwAAARRWRUVUTm90aWZpY2F0aW9uRGF0ZRZWRUVUIE5vdGlmaWNhdGlvbiBEYXRlCAAAARRWRUVUQ29tbWVuY2VtZW50RGF0ZRZWRUVUIENvbW1lbmNlbWVudCBEYXRlCAAAARRWRUVUUmVnaXN0cmF0aW9uRGF0ZRZWRUVUIFJlZ2lzdHJhdGlvbiBEYXRlCAAAAQwAAAAGU3RhdHVzBU5vdGVzC0lzUHVibGlzaGVkCURhdGVBZGRlZAdBZGRlZEJ5C0xhc3RVcGRhdGVkDUxhc3RVcGRhdGVkQnkMc3RyRGF0ZUFkZGVkEnN0ckRhdGVMYXN0VXBkYXRlZBNzdHJOb3RpZmljYXRpb25EYXRlE3N0clJlZ2lzdHJhdGlvbkRhdGUTc3RyQ29tbWVuY2VtZW50RGF0ZQAAAAAHAAcABwAHAAcABv//AwYyBQMHAQcCAAcCAAcCC0NVQiBQVFkgTFREBwIABwIABwIABwIABwIFMzYtODAHAgZDaHVyY2gHAgZTdHJlZXQHAgAHAgpBYmJvdHNmb3JkBwIDVklDBwIEMzA2NwgC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&quot;,&quot;groupLevelState&quot;:{},&quot;keys&quot;:[&quot;1330&quot;,&quot;1332&quot;,&quot;1716&quot;,&quot;1704&quot;,&quot;1416&quot;,&quot;1337&quot;,&quot;1455&quot;,&quot;1550&quot;,&quot;1270&quot;,&quot;1232&quot;,&quot;1551&quot;,&quot;1673&quot;,&quot;1674&quot;,&quot;1271&quot;,&quot;1457&quot;,&quot;1234&quot;,&quot;1235&quot;,&quot;1693&quot;,&quot;1689&quot;,&quot;1581&quot;,&quot;1447&quot;,&quot;1451&quot;,&quot;1423&quot;,&quot;1350&quot;,&quot;1335&quot;],&quot;toolbar&quot;:null}");
                postParameters.Add("ctl00$ContentPlaceHolder1$gvSAP$DXFREditorcol1", "");
                postParameters.Add("ctl00$ContentPlaceHolder1$gvSAP$DXFREditorcol2", "");
                postParameters.Add("ctl00$ContentPlaceHolder1$gvSAP$DXFREditorcol3", "");
                postParameters.Add("ctl00$ContentPlaceHolder1$gvSAP$DXFREditorcol4", "");
                postParameters.Add("ctl00$ContentPlaceHolder1$gvSAP$DXFREditorcol5", "");
                postParameters.Add("ctl00$ContentPlaceHolder1$gvSAP$DXFREditorcol6", "");
                postParameters.Add("ctl00$ContentPlaceHolder1$gvSAP$DXFREditorcol7", "");
                postParameters.Add("ctl00$ContentPlaceHolder1$gvSAP$DXFREditorcol8", "");
                postParameters.Add("ctl00$ContentPlaceHolder1$gvSAP$DXFREditorcol9", "");
                postParameters.Add("ctl00$ContentPlaceHolder1$gvSAP$DXFREditorcol10", "");
                postParameters.Add("ctl00$ContentPlaceHolder1$gvSAP$DXFREditorcol11", "");
                postParameters.Add("ctl00$ContentPlaceHolder1$gvSAP$DXFREditorcol12", "");
                postParameters.Add("ctl00$ContentPlaceHolder1$gvSAP$DXFREditorcol13", "");
                postParameters.Add("ctl00$ContentPlaceHolder1$gvSAP$DXFREditorcol14", "");
                postParameters.Add("ctl00$ContentPlaceHolder1$gvSAP$DXFREditorcol15", "");
                postParameters.Add("ctl00$ContentPlaceHolder1$gvSAP$DXFREditorcol16$State", "{&quot;rawValue&quot;:&quot;N&quot;}");
                postParameters.Add("ctl00$ContentPlaceHolder1$gvSAP$DXFREditorcol16", "");
                postParameters.Add("ctl00$ContentPlaceHolder1$gvSAP$DXFREditorcol16$DDDState", "{&quot;windowsState&quot;:&quot;0:0:-1:0:0:0:-10000:-10000:1:0:0:0&quot;}");
                postParameters.Add("ctl00$ContentPlaceHolder1$gvSAP$DXFREditorcol16$DDD$C", "{&quot;visibleDate&quot;:&quot;10/03/2018&quot;,&quot;initialVisibleDate&quot;:&quot;10/03/2018&quot;,&quot;selectedDates&quot;:[]}");
                postParameters.Add("ctl00$ContentPlaceHolder1$gvSAP$DXFREditorcol16$DDD$C$FNPState", "{&quot;windowsState&quot;:&quot;0:0:-1:0:0:0:-10000:-10000:1:0:0:0&quot;}");
                postParameters.Add("ctl00$ContentPlaceHolder1$gvSAP$DXFREditorcol17$State", "{&quot;rawValue&quot;:&quot;N&quot;}");
                postParameters.Add("ctl00$ContentPlaceHolder1$gvSAP$DXFREditorcol17", "");
                postParameters.Add("ctl00$ContentPlaceHolder1$gvSAP$DXFREditorcol17$DDDState", "{&quot;windowsState&quot;:&quot;0:0:-1:0:0:0:-10000:-10000:1:0:0:0&quot;}");
                postParameters.Add("ctl00$ContentPlaceHolder1$gvSAP$DXFREditorcol17$DDD$C", "{&quot;visibleDate&quot;:&quot;10/03/2018&quot;,&quot;initialVisibleDate&quot;:&quot;10/03/2018&quot;,&quot;selectedDates&quot;:[]}");
                postParameters.Add("ctl00$ContentPlaceHolder1$gvSAP$DXFREditorcol17$DDD$C$FNPState", "{&quot;windowsState&quot;:&quot;0:0:-1:0:0:0:-10000:-10000:1:0:0:0&quot;}");
                postParameters.Add("ctl00$ContentPlaceHolder1$gvSAP$DXFREditorcol18$State", "{&quot;rawValue&quot;:&quot;N&quot;}");
                postParameters.Add("ctl00$ContentPlaceHolder1$gvSAP$DXFREditorcol18	", "");
                postParameters.Add("ctl00$ContentPlaceHolder1$gvSAP$DXFREditorcol18$DDDState", "{&quot;windowsState&quot;:&quot;0:0:-1:0:0:0:-10000:-10000:1:0:0:0&quot;}");
                postParameters.Add("ctl00$ContentPlaceHolder1$gvSAP$DXFREditorcol18$DDD$C", "{&quot;visibleDate&quot;:&quot;10/03/2018&quot;,&quot;initialVisibleDate&quot;:&quot;10/03/2018&quot;,&quot;selectedDates&quot;:[]}");
                postParameters.Add("ctl00$ContentPlaceHolder1$gvSAP$DXFREditorcol18$DDD$C$FNPState", "{&quot;windowsState&quot;:&quot;0:0:-1:0:0:0:-10000:-10000:1:0:0:0&quot;}");
                postParameters.Add("ctl00$ContentPlaceHolder1$btnExportXLSX", "Export to XLSX");
                postParameters.Add("ctl00$MenuFoot", "{&quot;selectedItemIndexPath&quot;:&quot;&quot;,&quot;checkedState&quot;:&quot;&quot;}");
                postParameters.Add("DXScript", "1_16,1_17,1_28,1_66,1_19,1_20,1_22,1_29,1_36,1_225,1_228,1_24,1_254,1_265,1_266,1_252,1_268,1_276,1_278,1_279,1_274,1_280,1_226,1_26,1_27,1_231,1_233,1_44,1_235,1_42,1_224");
                postParameters.Add("DXCss", "0_1144,1_69,1_71,0_1148,0_1002,1_250,0_1006,0_1011,0_1015,1_251,../App_Themes/Glass/Public170801.css,../App_Themes/Glass/Chart/styles.css,../App_Themes/Glass/Editors/sprite.css,../App_Themes/Glass/Editors/styles.css,../App_Themes/Glass/GridView/sprite.css,../App_Themes/Glass/GridView/styles.css,../App_Themes/Glass/HtmlEditor/sprite.css,../App_Themes/Glass/HtmlEditor/styles.css,../App_Themes/Glass/Portlets.css,../App_Themes/Glass/Scheduler/sprite.css,../App_Themes/Glass/Scheduler/styles.css,../App_Themes/Glass/SpellChecker/styles.css,../App_Themes/Glass/TreeList/sprite.css,../App_Themes/Glass/TreeList/styles.css,../App_Themes/Glass/Web/sprite.css,../App_Themes/Glass/Web/styles.css");
                string stringPost = string.Empty;
                foreach (var item in postParameters)
                {
                    stringPost += HttpUtility.UrlEncode(item.Key) + "=" + HttpUtility.UrlEncode(item.Value) + "&";
                }
                stringPost = stringPost.TrimEnd(new char[] { '&' });
                //WriteErrorLog("timer", "here1");
                request = WebRequest.Create(postUrl) as HttpWebRequest;
                //request.Headers.Add("X-MicrosoftAjax", "Delta=true");
                //request.Headers.Add("X-Requested-With", "XMLHttpRequest");
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.99 Safari/537.36";
                // Set the Method property of the request to POST.
                request.Method = "POST";
                request.CookieContainer = ck;
                request.Proxy = myProxy;
                // Create POST data and convert it to a byte array.
                string postStringData = stringPost;
                byte[] bytes = Encoding.UTF8.GetBytes(postStringData);
                request.ContentType = "application/x-www-form-urlencoded";
                // Set the ContentLength property of the WebRequest.
                request.ContentLength = bytes.Length;
                // Get the request stream.
                dataStream = request.GetRequestStream();
                // Write the data to the request stream.
                dataStream.Write(bytes, 0, bytes.Length);
                // Close the Stream object.
                dataStream.Close();
                // Get the response.
                response = request.GetResponse();
                dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                reader = new StreamReader(dataStream);
                // Read the content.
                MemoryStream ms = new MemoryStream();
                dataStream.CopyTo(ms);
                string FilePath = Path.Combine(ProjectSession.ProofDocuments, "VEECDocuments\\" + DateTime.Now.Ticks + ".xlsx");
                File.WriteAllBytes(FilePath, ms.ToArray());
                _veec.DeleteVEECActivitySchedulePremises();
                // SQL Server Connection String
                Workbook workbook = new Workbook();
                //Load the file
                workbook.LoadFromFile(FilePath);
                //Initailize worksheet
                Worksheet sheet = workbook.Worksheets[0];
                //Export datatable
                DataTable dt = sheet.ExportDataTable();
                string sqlConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                // Bulk Copy to SQL Server 
                SqlBulkCopy bulkInsert = new SqlBulkCopy(sqlConnectionString);
                bulkInsert.DestinationTableName = "VEECScheduledActivityPremises";
                bulkInsert.WriteToServer(dt);
            }
            catch(Exception ex)
            {
                Log(ex.Message);
            }
        }

        private void Log(string text)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "ServiceLog.txt";
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(DateTime.Now.ToString() + Environment.NewLine + text + Environment.NewLine + "------------------------------------------------------------------------");
                writer.Close();
            }
        }
    }
}

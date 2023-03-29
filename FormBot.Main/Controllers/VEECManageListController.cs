using FormBot.BAL.Service.VEECManageList;
using FormBot.Entity.VEECManageList;
using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using HtmlAgilityPack;
using System.Text;
using Microsoft.VisualBasic.FileIO;
using System.IO;
using System.Data;

namespace FormBot.Main.Controllers
{
    public class VEECManageListController : Controller
    {
        #region Properties

        private readonly IVEECManageListBAL _vEECManageListBAL;
        private const string FAILURE = "failure";
        private const string SUCCESS = "success";
        private const string FILEPATH = "CERFiles";

        #endregion

        #region Constructor

        public VEECManageListController(IVEECManageListBAL vEECManageListBAL)
        {
            this._vEECManageListBAL = vEECManageListBAL;
        }

        #endregion

        /// <summary>
        /// get method Accredited the installers.
        /// </summary>
        /// <returns>blank view</returns>
        [HttpGet]
        [UserAuthorization]
        public ActionResult VEECProductBrandsList()
        {
            return View();
        }

        public void GetVEECProductBrandsList(string Brand = "", string Model = "",string ProductType = "", string ProductCategory = "", string TechnologyClass = "", string ApplicationDate = "", string EffectiveFrom = "", string EffectiveTo = "")
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);
            try
            {
                IList<VEECProductBrands> lstVEECProductBrandsList = _vEECManageListBAL.VEECProductBrandsList(pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, Brand, Model,ProductType, ProductCategory, TechnologyClass, ApplicationDate, EffectiveFrom, EffectiveTo);
                if (lstVEECProductBrandsList.Count > 0)
                {
                    gridParam.TotalDisplayRecords = lstVEECProductBrandsList.FirstOrDefault().TotalRecords;
                    gridParam.TotalRecords = lstVEECProductBrandsList.FirstOrDefault().TotalRecords;

                }

                HttpContext.Response.Write(Grid.PrepareDataSet(lstVEECProductBrandsList, gridParam));
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                HttpContext.Response.Write(Grid.PrepareDataSet(new List<VEECProductBrands>(), gridParam));
            }
        }


        public void ImportProductDetails()
        {

            string postUrl = "https://www.veet.vic.gov.au/Public/ProductRegistrySearch.aspx";

            HttpWebRequest request = WebRequest.Create(postUrl) as HttpWebRequest;
            CookieContainer ck = new CookieContainer();

            request.Method = "GET";

            WebResponse response = request.GetResponse();
            System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream());
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            response.Close();

            HtmlDocument do2c = new HtmlDocument();
            do2c.LoadHtml(responseFromServer);

            Dictionary<string, string> postParameters = new Dictionary<string, string>();

            postParameters.Add("__EVENTARGUMENT", "Click");
            postParameters.Add("__EVENTTARGET", "ctl00$ContentPlaceHolder1$btnExportCSV");
            postParameters.Add("__VIEWSTATE", do2c.GetElementbyId("__VIEWSTATE").GetAttributeValue("value", ""));
            postParameters.Add("__VIEWSTATEGENERATOR", do2c.GetElementbyId("__VIEWSTATEGENERATOR").GetAttributeValue("value", ""));
            postParameters.Add("__EVENTVALIDATION", do2c.GetElementbyId("__EVENTVALIDATION").GetAttributeValue("value", ""));
            postParameters.Add("ContentPlaceHolder1_ActivityCMB_VI", "AO");
            postParameters.Add("ctl00$ContentPlaceHolder1$ActivityCMB", "34 - Lighting Upgrade");
            postParameters.Add("ctl00$ContentPlaceHolder1$ActivityCMB$DDD$L", "AO");
            postParameters.Add("ctl00$ContentPlaceHolder1$ActivityCMB$DDDState", "{&quot;windowsState&quot;:&quot;0:0:-1:0:0:0:-10000:-10000:1:0:0:0&quot;}");
            postParameters.Add("ctl00$ContentPlaceHolder1$gvProducts", "{&quot;selection&quot;:&quot;&quot;,&quot;callbackState&quot;:&quot;BwUHAwIERGF0YQZ0LwAAAABwFAAAcBQAAAAAAAAUAAAAAF0AAAAGTVJlY0lEB01SZWMgSUQDAAABB0FwcERhdGUIQXBwIERhdGUIAAABBUJyYW5kBUJyYW5kBwAAAQVNb2RlbAVNb2RlbAcAAAEMRGF0ZUFkZGVkTWluDkRhdGUgQWRkZWQgTWluCAAAAQ5EYXRlRGVsZXRlZE1pbhBEYXRlIERlbGV0ZWQgTWluCAAAARBzdHJTdGF0dXNBcHByb3ZlEnN0ciBTdGF0dXMgQXBwcm92ZQcAAAEMV2ludGVyUlZhbHVlDVdpbnRlciBSVmFsdWUHAAABCU1lYXN1cmUxNQlNZWFzdXJlMTUHAAABE0NvbWJpbmF0aW9uUHJvZHVjdHMUQ29tYmluYXRpb24gUHJvZHVjdHMHAAABE1dhcnJhbnR5UGVyaW9kWWVhcnMVV2FycmFudHkgUGVyaW9kIFllYXJzAwAAARRJc0NvbWJpbmF0aW9uUHJvZHVjdBZJcyBDb21iaW5hdGlvbiBQcm9kdWN0CgAAAQlNZWFzdXJlMTYJTWVhc3VyZTE2BwAAAQtQcm9kdWN0TmFtZQxQcm9kdWN0IE5hbWUHAAABCEVmZmljYWN5CEVmZmljYWN5BwAAAQ1SYXRlZExpZmV0aW1lDlJhdGVkIExpZmV0aW1lBwAAAQtQb3dlckZhY3RvcgxQb3dlciBGYWN0b3IFAAABCklzRGltbWFibGULSXMgRGltbWFibGUKAAABCUJlYW1BbmdsZQpCZWFtIEFuZ2xlAwAAAQpEcml2ZXJUeXBlC0RyaXZlciBUeXBlBwAAAQ1Jc1Jlc2lkZW50aWFsDklzIFJlc2lkZW50aWFsCgAAAQxJc0NvbW1lcmNpYWwNSXMgQ29tbWVyY2lhbAoAAAENUHJvZHVjdFR5cGUyMQ5Qcm9kdWN0IFR5cGUyMQcAAAEWUGVyZm9ybWFuY2VSZXF1aXJlbWVudBdQZXJmb3JtYW5jZSBSZXF1aXJlbWVudAcAAAENSXNFeHRlcm5hbFVzZQ9JcyBFeHRlcm5hbCBVc2UKAAABDU5vbWluYWxSYXRpbmcOTm9taW5hbCBSYXRpbmcFAAABBkVFUl9GTAZFRVJfRkwFAAABCEVFUl81MHBjCEVFUl81MHBjBQAAAQhFRVJfMjBwYwhFRVJfMjBwYwUAAAEDRUVSA0VFUgUAAAEGUlZhbHVlBlJWYWx1ZQUAAAEHU1BDVHlwZQhTUEMgVHlwZQcAAAELQ29udHJvbFR5cGUMQ29udHJvbCBUeXBlBwAAARJUZXN0VHJpYWxDb21wbGV0ZWQUVGVzdCBUcmlhbCBDb21wbGV0ZWQHAAABEUFiYXRlbWVudEZhY3RvcjI5EkFiYXRlbWVudCBGYWN0b3IyOQUAAAENWmlnQmVlRW5hYmxlZA9aaWcgQmVlIEVuYWJsZWQKAAABFVppZ0JlZUZpcm13YXJlVmVyc2lvbhhaaWcgQmVlIEZpcm13YXJlIFZlcnNpb24HAAABEFppZ0JlZURCTmFtZUxpc3QUWmlnIEJlZSBEQiBOYW1lIExpc3QHAAABC0NhYmluZXRUeXBlDENhYmluZXQgVHlwZQcAAAEGQXJlYTMyBkFyZWEzMgC9AVN5c3RlbS5OdWxsYWJsZWAxW1tTeXN0ZW0uRGVjaW1hbCwgbXNjb3JsaWIsIFZlcnNpb249NC4wLjAuMCwgQ3VsdHVyZT1uZXV0cmFsLCBQdWJsaWNLZXlUb2tlbj1iNzdhNWM1NjE5MzRlMDg5XV0sIG1zY29ybGliLCBWZXJzaW9uPTQuMC4wLjAsIEN1bHR1cmU9bmV1dHJhbCwgUHVibGljS2V5VG9rZW49Yjc3YTVjNTYxOTM0ZTA4OQAAARBSZWZyaWdlcmF0b3JUeXBlEVJlZnJpZ2VyYXRvciBUeXBlBwAAAQpJbnB1dFBvd2VyC0lucHV0IFBvd2VyBQAAAQtPdXRwdXRQb3dlcgxPdXRwdXQgUG93ZXIAvQFTeXN0ZW0uTnVsbGFibGVgMVtbU3lzdGVtLkRlY2ltYWwsIG1zY29ybGliLCBWZXJzaW9uPTQuMC4wLjAsIEN1bHR1cmU9bmV1dHJhbCwgUHVibGljS2V5VG9rZW49Yjc3YTVjNTYxOTM0ZTA4OV1dLCBtc2NvcmxpYiwgVmVyc2lvbj00LjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPWI3N2E1YzU2MTkzNGUwODkAAAEPT3V0cHV0UG93ZXJIaWdoEU91dHB1dCBQb3dlciBIaWdoAL0BU3lzdGVtLk51bGxhYmxlYDFbW1N5c3RlbS5EZWNpbWFsLCBtc2NvcmxpYiwgVmVyc2lvbj00LjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPWI3N2E1YzU2MTkzNGUwODldXSwgbXNjb3JsaWIsIFZlcnNpb249NC4wLjAuMCwgQ3VsdHVyZT1uZXV0cmFsLCBQdWJsaWNLZXlUb2tlbj1iNzdhNWM1NjE5MzRlMDg5AAABDUFpckZsb3dWb2x1bWUPQWlyIEZsb3cgVm9sdW1lAL0BU3lzdGVtLk51bGxhYmxlYDFbW1N5c3RlbS5EZWNpbWFsLCBtc2NvcmxpYiwgVmVyc2lvbj00LjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPWI3N2E1YzU2MTkzNGUwODldXSwgbXNjb3JsaWIsIFZlcnNpb249NC4wLjAuMCwgQ3VsdHVyZT1uZXV0cmFsLCBQdWJsaWNLZXlUb2tlbj1iNzdhNWM1NjE5MzRlMDg5AAABDlJvdG9yTW90b3JUeXBlEFJvdG9yIE1vdG9yIFR5cGUHAAABDk91dHB1dFBvd2VyTmV3EE91dHB1dCBQb3dlciBOZXcAvQFTeXN0ZW0uTnVsbGFibGVgMVtbU3lzdGVtLkRlY2ltYWwsIG1zY29ybGliLCBWZXJzaW9uPTQuMC4wLjAsIEN1bHR1cmU9bmV1dHJhbCwgUHVibGljS2V5VG9rZW49Yjc3YTVjNTYxOTM0ZTA4OV1dLCBtc2NvcmxpYiwgVmVyc2lvbj00LjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPWI3N2E1YzU2MTkzNGUwODkAAAELUHJvZHVjdFR5cGUMUHJvZHVjdCBUeXBlBwAAAQ9UZWNobm9sb2d5Q2xhc3MQVGVjaG5vbG9neSBDbGFzcwcAAAEPUHJvZHVjdENhdGVnb3J5EFByb2R1Y3QgQ2F0ZWdvcnkHAAABA0xDUANMQ1AAvQFTeXN0ZW0uTnVsbGFibGVgMVtbU3lzdGVtLkRlY2ltYWwsIG1zY29ybGliLCBWZXJzaW9uPTQuMC4wLjAsIEN1bHR1cmU9bmV1dHJhbCwgUHVibGljS2V5VG9rZW49Yjc3YTVjNTYxOTM0ZTA4OV1dLCBtc2NvcmxpYiwgVmVyc2lvbj00LjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPWI3N2E1YzU2MTkzNGUwODkAAAEDTkxQA05MUAC9AVN5c3RlbS5OdWxsYWJsZWAxW1tTeXN0ZW0uRGVjaW1hbCwgbXNjb3JsaWIsIFZlcnNpb249NC4wLjAuMCwgQ3VsdHVyZT1uZXV0cmFsLCBQdWJsaWNLZXlUb2tlbj1iNzdhNWM1NjE5MzRlMDg5XV0sIG1zY29ybGliLCBWZXJzaW9uPTQuMC4wLjAsIEN1bHR1cmU9bmV1dHJhbCwgUHVibGljS2V5VG9rZW49Yjc3YTVjNTYxOTM0ZTA4OQAAAQ9SYXRlZExpZmV0aW1lMzQQUmF0ZWQgTGlmZXRpbWUzNAC7AVN5c3RlbS5OdWxsYWJsZWAxW1tTeXN0ZW0uSW50MzIsIG1zY29ybGliLCBWZXJzaW9uPTQuMC4wLjAsIEN1bHR1cmU9bmV1dHJhbCwgUHVibGljS2V5VG9rZW49Yjc3YTVjNTYxOTM0ZTA4OV1dLCBtc2NvcmxpYiwgVmVyc2lvbj00LjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPWI3N2E1YzU2MTkzNGUwODkAAAEKVlJVVm9sdGFnZQtWUlUgVm9sdGFnZQC7AVN5c3RlbS5OdWxsYWJsZWAxW1tTeXN0ZW0uSW50MzIsIG1zY29ybGliLCBWZXJzaW9uPTQuMC4wLjAsIEN1bHR1cmU9bmV1dHJhbCwgUHVibGljS2V5VG9rZW49Yjc3YTVjNTYxOTM0ZTA4OV1dLCBtc2NvcmxpYiwgVmVyc2lvbj00LjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPWI3N2E1YzU2MTkzNGUwODkAAAEKTnVtT2ZMYW1wcwxOdW0gT2YgTGFtcHMAuwFTeXN0ZW0uTnVsbGFibGVgMVtbU3lzdGVtLkludDMyLCBtc2NvcmxpYiwgVmVyc2lvbj00LjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPWI3N2E1YzU2MTkzNGUwODldXSwgbXNjb3JsaWIsIFZlcnNpb249NC4wLjAuMCwgQ3VsdHVyZT1uZXV0cmFsLCBQdWJsaWNLZXlUb2tlbj1iNzdhNWM1NjE5MzRlMDg5AAABCFRvdGFsTENQCVRvdGFsIExDUAC9AVN5c3RlbS5OdWxsYWJsZWAxW1tTeXN0ZW0uRGVjaW1hbCwgbXNjb3JsaWIsIFZlcnNpb249NC4wLjAuMCwgQ3VsdHVyZT1uZXV0cmFsLCBQdWJsaWNLZXlUb2tlbj1iNzdhNWM1NjE5MzRlMDg5XV0sIG1zY29ybGliLCBWZXJzaW9uPTQuMC4wLjAsIEN1bHR1cmU9bmV1dHJhbCwgUHVibGljS2V5VG9rZW49Yjc3YTVjNTYxOTM0ZTA4OQAAAQ5JbkJ1aWxkTENEVHlwZRFJbiBCdWlsZCBMQ0QgVHlwZQcAAAEMUHJvZHVjdE5vdGVzDVByb2R1Y3QgTm90ZXMHAAABEElzTmV3TEVEU3RhbmRhcmQTSXMgTmV3IExFRCBTdGFuZGFyZAoAAAEKU3RhclJhdGluZwtTdGFyIFJhdGluZwC9AVN5c3RlbS5OdWxsYWJsZWAxW1tTeXN0ZW0uRGVjaW1hbCwgbXNjb3JsaWIsIFZlcnNpb249NC4wLjAuMCwgQ3VsdHVyZT1uZXV0cmFsLCBQdWJsaWNLZXlUb2tlbj1iNzdhNWM1NjE5MzRlMDg5XV0sIG1zY29ybGliLCBWZXJzaW9uPTQuMC4wLjAsIEN1bHR1cmU9bmV1dHJhbCwgUHVibGljS2V5VG9rZW49Yjc3YTVjNTYxOTM0ZTA4OQAAAQ9TdG9yYWdlQ2FwYWNpdHkQU3RvcmFnZSBDYXBhY2l0eQUAAAEWSGVhdGluZ0NhcGFjaXR5THBlck1pbhlIZWF0aW5nIENhcGFjaXR5IExwZXIgTWluBQAAARFIZWF0aW5nQ2FwYWNpdHlLVxNIZWF0aW5nIENhcGFjaXR5IEtXBQAAARFTb2xhckNvbnRyaWJ1dGlvbhJTb2xhciBDb250cmlidXRpb24FAAABCFBlYWtMb2FkCVBlYWsgTG9hZAUAAAECQnMCQnMFAAABAkJlAkJlBQAAAQpTeXN0ZW1TaXplC1N5c3RlbSBTaXplBwAAAQRGdWVsBEZ1ZWwHAAABClN5c3RlbVR5cGULU3lzdGVtIFR5cGUHAAABCFRhbmtTaXplCVRhbmsgU2l6ZQUAAAEPVGFua01vZGVsTnVtYmVyEVRhbmsgTW9kZWwgTnVtYmVyBwAAAQ1Db2xsZWN0b3JUeXBlDkNvbGxlY3RvciBUeXBlBwAAARRDb2xsZWN0b3JNb2RlbE51bWJlchZDb2xsZWN0b3IgTW9kZWwgTnVtYmVyBwAAAQ9OdW1PZkNvbGxlY3RvcnMRTnVtIE9mIENvbGxlY3RvcnMDAAABDFRlc3RTdGFuZGFyZA1UZXN0IFN0YW5kYXJkBwAAAQNDT1ADQ09QBQAAAQhIZWF0U3RhcglIZWF0IFN0YXIFAAABBlV2YWx1ZQZVdmFsdWUFAAABBFR5cGUEVHlwZQcAAAEIVG90YWxWb2wJVG90YWwgVm9sBQAAAQVGRlZvbAZGRiBWb2wFAAABBUZaVm9sBkZaIFZvbAUAAAEPU3RhclJhdGluZ0luZGV4EVN0YXIgUmF0aW5nIEluZGV4BQAAAQpTY3JlZW5BcmVhC1NjcmVlbiBBcmVhBQAAAQNDRUMDQ0VDAL0BU3lzdGVtLk51bGxhYmxlYDFbW1N5c3RlbS5EZWNpbWFsLCBtc2NvcmxpYiwgVmVyc2lvbj00LjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPWI3N2E1YzU2MTkzNGUwODldXSwgbXNjb3JsaWIsIFZlcnNpb249NC4wLjAuMCwgQ3VsdHVyZT1uZXV0cmFsLCBQdWJsaWNLZXlUb2tlbj1iNzdhNWM1NjE5MzRlMDg5AAABCEZ1ZWxUeXBlCUZ1ZWwgVHlwZQcAAAENUmF0ZWRDYXBhY2l0eQ5SYXRlZCBDYXBhY2l0eQUAAAEMSW5wdXRQb3dlcjI2DUlucHV0IFBvd2VyMjYFAAABBFBBRUMEUEFFQwUAAAELUmF0ZWRPdXRwdXQMUmF0ZWQgT3V0cHV0BQAAAQpOdW1PZlBvbGVzDE51bSBPZiBQb2xlcwMAAAEMU3VwZXJQcmVtaXVtDVN1cGVyIFByZW1pdW0KAAABHwAAAANVSUQFQXBwSUQGU3RhdHVzD0lzUHVibGljVmlzaWJsZQlBY2NvdW50SUQJRGF0ZUFkZGVkDERhdGVBcHByb3ZlZAtEYXRlRGVsZXRlZAtMYXN0VXBkYXRlZAdBZGRlZEJ5CkFwcHJvdmVkQnkJRGVsZXRlZEJ5DUxhc3RVcGRhdGVkQnkJUmVmZXJlbmNlBU5vdGVzDEFjdGl2aXR5Q29kZRFJc0FwcHJvdmVk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//8HBAb//wcFBv//BwYG//8HBwb//wcIBv//BwkG//8HCgb//wcLBv//BwwG//8HDQb//wcOBv//Bw8G//8HEAb//wcRBv//BxIG//8HEwb//wcUBv//BxUG//8HFgb//wcXBv//BxgG//8HGQb//wcaBv//BxsG//8HHAb//wcdBv//Bx4G//8HHwb//wcgBv//ByEG//8HIgb//wcjBv//ByQG//8HJQb//wcmBv//BycG//8HKAb//wcpBv//ByoG//8HKwb//wcsBv//By0G//8HLgb//wcvBv//BzAG//8HMQb//wcyBv//BzMG//8HNAb//wc1Bv//BzYG//8HNwb//wc4Bv//BzkG//8HOgb//wc7Bv//BzwG//8HPQb//wc+Bv//Bz8G//8HQAb//wdBBv//B0IG//8HQwb//wdEBv//B0UG//8HRgb//wdHBv//B0gG//8HSQb//wdKBv//B0sG//8HTAb//wdNBv//B04G//8HTwb//wdQBv//B1EG//8HUgb//wdTBv//B1QG//8HVQb//wdWBv//B1cG//8HWAb//wdZBv//B1oG//8HWwb//wdcBv//AgAFAAAAgAkCBk1SZWNJRAcBAgZNUmVjSUQDCQIAAgADB34AXFN5c3RlbS5TdHJpbmdbXSwgbXNjb3JsaWIsIFZlcnNpb249NC4wLjAuMCwgQ3VsdHVyZT1uZXV0cmFsLCBQdWJsaWNLZXlUb2tlbj1iNzdhNWM1NjE5MzRlMDg5AgABAAAA/////wEAAAAAAAAAEQEAAAAAAAAACwIABwACAQZwFAcAAgACAQcABwAHAAcABwACCFBhZ2VTaXplAwcUAg1TaG93RmlsdGVyUm93CgIB&quot;,&quot;keys&quot;:[&quot;12589&quot;,&quot;12590&quot;,&quot;12584&quot;,&quot;12586&quot;,&quot;12587&quot;,&quot;12585&quot;,&quot;12588&quot;,&quot;11865&quot;,&quot;8294&quot;,&quot;1258&quot;,&quot;1259&quot;,&quot;1251&quot;,&quot;1250&quot;,&quot;3062&quot;,&quot;1254&quot;,&quot;1253&quot;,&quot;1252&quot;,&quot;1257&quot;,&quot;1256&quot;,&quot;1255&quot;]}");
            postParameters.Add("ctl00$ContentPlaceHolder1$gvProducts$DXFREditorcol1", "");
            postParameters.Add("ctl00$ContentPlaceHolder1$gvProducts$DXFREditorcol2", "");
            postParameters.Add("ctl00$ContentPlaceHolder1$gvProducts$DXFREditorcol38", "");
            postParameters.Add("ctl00$ContentPlaceHolder1$gvProducts$DXFREditorcol39", "");
            postParameters.Add("ctl00$ContentPlaceHolder1$gvProducts$DXFREditorcol40", "");
            postParameters.Add("ctl00$ContentPlaceHolder1$gvProducts$DXFREditorcol41", "");
            postParameters.Add("ctl00$ContentPlaceHolder1$gvProducts$DXFREditorcol43", "");
            postParameters.Add("ctl00$ContentPlaceHolder1$gvProducts$DXFREditorcol44", "");
            postParameters.Add("ctl00$ContentPlaceHolder1$gvProducts$DXFREditorcol45", "");
            postParameters.Add("ctl00$ContentPlaceHolder1$gvProducts$DXFREditorcol46", "");
            postParameters.Add("ctl00$ContentPlaceHolder1$gvProducts$DXFREditorcol47", "");
            postParameters.Add("ctl00$ContentPlaceHolder1$gvProducts$DXFREditorcol89", "");
            postParameters.Add("ctl00$ContentPlaceHolder1$gvProducts$DXFREditorcol90", "");
            postParameters.Add("ctl00$ContentPlaceHolder1$gvProducts$DXFREditorcol90$DDD$C", "{&quot;visibleDate&quot;:&quot;07/11/2018&quot;,&quot;initialVisibleDate&quot;:&quot;07/11/2018&quot;,&quot;selectedDates&quot;:[]}");
            postParameters.Add("ctl00$ContentPlaceHolder1$gvProducts$DXFREditorcol90$DDD$C$FNPState", "{&quot;windowsState&quot;:&quot;0:0:-1:0:0:0:-10000:-10000:1:0:0:0&quot;}");
            postParameters.Add("ctl00$ContentPlaceHolder1$gvProducts$DXFREditorcol90$DDDState", "{&quot;windowsState&quot;:&quot;0:0:-1:0:0:0:-10000:-10000:1:0:0:0&quot;}");
            postParameters.Add("ctl00$ContentPlaceHolder1$gvProducts$DXFREditorcol90$State", "{&quot;rawValue&quot;:&quot;N&quot;}");
            postParameters.Add("ctl00$ContentPlaceHolder1$gvProducts$DXFREditorcol91", "");
            postParameters.Add("ctl00$ContentPlaceHolder1$gvProducts$DXFREditorcol91$DDD$C", "{&quot;visibleDate&quot;:&quot;07/11/2018&quot;,&quot;initialVisibleDate&quot;:&quot;07/11/2018&quot;,&quot;selectedDates&quot;:[]}");
            postParameters.Add("ctl00$ContentPlaceHolder1$gvProducts$DXFREditorcol91$DDD$C$FNPState", "{&quot;windowsState&quot;:&quot;0:0:-1:0:0:0:-10000:-10000:1:0:0:0&quot;}");
            postParameters.Add("ctl00$ContentPlaceHolder1$gvProducts$DXFREditorcol91$DDDState", "{&quot;windowsState&quot;:&quot;0:0:-1:0:0:0:-10000:-10000:1:0:0:0&quot;}");
            postParameters.Add("ctl00$ContentPlaceHolder1$gvProducts$DXFREditorcol91$State", "{&quot;rawValue&quot;:&quot;N&quot;}");
            postParameters.Add("ctl00$ContentPlaceHolder1$gvProducts$DXFREditorcol92", "");
            postParameters.Add("ctl00$ContentPlaceHolder1$gvProducts$DXFREditorcol92$DDD$C", "{&quot;visibleDate&quot;:&quot;07/11/2018&quot;,&quot;initialVisibleDate&quot;:&quot;07/11/2018&quot;,&quot;selectedDates&quot;:[]}");
            postParameters.Add("ctl00$ContentPlaceHolder1$gvProducts$DXFREditorcol92$DDD$C$FNPState", "{&quot;windowsState&quot;:&quot;0:0:-1:0:0:0:-10000:-10000:1:0:0:0&quot;}");
            postParameters.Add("ctl00$ContentPlaceHolder1$gvProducts$DXFREditorcol92$DDDState", "{&quot;windowsState&quot;:&quot;0:0:-1:0:0:0:-10000:-10000:1:0:0:0&quot;}");
            postParameters.Add("ctl00$ContentPlaceHolder1$gvProducts$DXFREditorcol92$State", "{&quot;rawValue&quot;:&quot;N&quot;}");
            postParameters.Add("ctl00$ContentPlaceHolder1$gvProducts$DXHFPState", "{&quot;windowsState&quot;:&quot;0:0:-1:0:0:0:-10000:-10000:1:0:0:0&quot;}");
            postParameters.Add("ctl00$ContentPlaceHolder1$hidValues", "{&quot;data&quot;:&quot;12|#|ActivityCode|4|3|1AOAsAtDate|6|13|1531267200000StatusGroup|4|2|10#&quot;}");
            postParameters.Add("ctl00$ContentPlaceHolder1$rbtnDisplay", "0");
            postParameters.Add("ctl00$ContentPlaceHolder1$rbtnDisplay$RB0", "C");
            postParameters.Add("ctl00$ContentPlaceHolder1$rbtnDisplay$RB1", "U");
            postParameters.Add("ctl00$MenuFoot", "{&quot;selectedItemIndexPath&quot;:&quot;&quot;,&quot;checkedState&quot;:&quot;&quot;}");
            postParameters.Add("ctl00$MenuHead", "{&quot;selectedItemIndexPath&quot;:&quot;&quot;,&quot;checkedState&quot;:&quot;&quot;}");
            //postParameters.Add("DXCss", "0_1116,1_50,1_51,0_1120,1_16,0_978,1_17,0_974,0_983,0_987,../App_Themes/Glass/Public170801.css,../App_Themes/Glass/Chart/styles.css,../App_Themes/Glass/Editors/sprite.css,../App_Themes/Glass/Editors/styles.css,../App_Themes/Glass/GridView/sprite.css,../App_Themes/Glass/GridView/styles.css,../App_Themes/Glass/HtmlEditor/sprite.css,../App_Themes/Glass/HtmlEditor/styles.css,../App_Themes/Glass/Portlets.css,../App_Themes/Glass/Scheduler/sprite.css,../App_Themes/Glass/Scheduler/styles.css,../App_Themes/Glass/SpellChecker/styles.css,../App_Themes/Glass/TreeList/sprite.css,../App_Themes/Glass/TreeList/styles.css,../App_Themes/Glass/Web/sprite.css,../App_Themes/Glass/Web/styles.css");
            //postParameters.Add("DXScript", "1_304,1_185,1_298,1_211,1_188,1_182,1_290,1_296,1_279,1_209,1_217,1_208,1_206,1_288,1_212,1_198,1_196,1_254,1_256,1_263,1_235,1_248,1_244,1_242,1_251,1_239,1_247,1_201,1_190,1_223,1_207,1_199,1_286,1_270");

            string strPost = string.Empty;
            foreach (var item in postParameters)
            {
                strPost += HttpUtility.UrlEncode(item.Key) + "=" + HttpUtility.UrlEncode(item.Value) + "&";
            }
            strPost = strPost.TrimEnd(new char[] { '&' });

            string postData = string.Empty;
            byte[] byteArray;

            request = (HttpWebRequest)WebRequest.Create(postUrl);
            request.Method = "POST";
            request.Host = "www.veet.vic.gov.au";
            request.KeepAlive = true;
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.99 Safari/537.36";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            request.Referer = "https://www.veet.vic.gov.au/Public/ProductRegistrySearch.aspx";
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Accept-Language", "en-US,en;q=0.8");

            // Create POST data and convert it to a byte array.
            postData = strPost;
            byteArray = Encoding.UTF8.GetBytes(postData);

            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;
            // Get the request stream.
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();

            System.IO.StreamReader strReader = new System.IO.StreamReader(request.GetResponse().GetResponseStream());

            DataTable dtProductBrands = GetCSVTable(strReader);

            _vEECManageListBAL.InsertProductBrands(dtProductBrands);

        }

        public static DataTable GetCSVTable(StreamReader strReader)
        {
            DataTable csvData = new DataTable("CSVTable");
            using (TextFieldParser csvReader = new TextFieldParser(strReader))
            {
                csvReader.SetDelimiters(new string[] { "," });
                csvReader.HasFieldsEnclosedInQuotes = true;
                string[] colFields;
                bool tableCreated = false;
                while (tableCreated == false)
                {
                    colFields = csvReader.ReadFields();
                    foreach (string column in colFields)
                    {
                        DataColumn datecolumn = new DataColumn(column.Replace(" ", ""));
                        datecolumn.AllowDBNull = true;
                        csvData.Columns.Add(datecolumn);
                    }
                    tableCreated = true;
                }
                bool endOFData = false;
                while (endOFData == false)
                {
                    string[] fieldData = csvReader.ReadFields();
                    if (fieldData != null && fieldData[0].ToLower() != "")
                    {
                        for (int i = 0; i < fieldData.Length; i++)
                        {
                            if (i == 12 || i == 13 || i == 14)
                            {
                                if (string.IsNullOrEmpty(fieldData[i]))
                                {
                                    fieldData[i] = null;
                                }
                                else
                                {
                                    fieldData[i] = DateTime.Parse(fieldData[i]).ToString("yyyyMMdd");
                                }
                            }
                            else if (fieldData[i] == "")
                            {
                                fieldData[i] = null;
                            }
                        }
                        csvData.Rows.Add(fieldData);
                    }
                    else
                    {
                        endOFData = true;
                    }
                }
            }
            return csvData;
        }
    }
}
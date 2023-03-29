using FormBot.BAL.Service;
using FormBot.Entity;
using FormBot.Entity.Email;
using FormBot.Helper;
using FormBot.Main.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FormBot.Main.Controllers
{
    public class SolarSubContractorRequestController : Controller
    {
        #region Properties
        private readonly ISolarSubContractorBAL _solarSubContractorBALBAL;
        #endregion

        #region Constructor
        public SolarSubContractorRequestController(ISolarSubContractorBAL solarSubContractorBALBAL)
        {
            this._solarSubContractorBALBAL = solarSubContractorBALBAL;
        }
        #endregion

        [HttpGet]
        [UserAuthorization]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Gets the solar sub contractor request list.
        /// </summary>
        /// <param name="refNumber">The reference number.</param>
        /// <param name="companyName">Name of the company.</param>
        /// <param name="fromdate">The fromdate.</param>
        /// <param name="todate">The todate.</param>
        public void GetSolarSubContractorRequestList(string refNumber = "", string companyName = "", string fromdate = "", string todate = "")
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);
            DateTime? FromDate = null, ToDate = null;
            if (!string.IsNullOrEmpty(fromdate) && !string.IsNullOrEmpty(todate))
            {
                FromDate = Convert.ToDateTime(fromdate);
                ToDate = Convert.ToDateTime(todate);
            }
            IList<SolarSubContractor> lstSolarSubContractorRequest = _solarSubContractorBALBAL.GetSolarSubContractorRequestList(pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, refNumber, companyName, FromDate, ToDate);
            if (lstSolarSubContractorRequest.Count > 0)
            {
                gridParam.TotalDisplayRecords = lstSolarSubContractorRequest.FirstOrDefault().TotalRecords;
                gridParam.TotalRecords = lstSolarSubContractorRequest.FirstOrDefault().TotalRecords;
            }

            HttpContext.Response.Write(Grid.PrepareDataSet(lstSolarSubContractorRequest, gridParam));
        }

        /// <summary>
        /// Deletes the remove SSC request.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <returns>ActionResult</returns>
        public ActionResult DeleteRemoveSSCRequest(string jobId)
        {
            //try
            //{
            int sscJOBID = 0;
            if (!string.IsNullOrEmpty(jobId))
                int.TryParse(QueryString.GetValueFromQueryString(jobId, "id"), out sscJOBID);
            int jobID = sscJOBID;
            _solarSubContractorBALBAL.SuccessfullRemoveSSCRequest(jobID);
            return this.Json(new { success = true }, JsonRequestBehavior.AllowGet);
            //}
            //catch (Exception ex)
            //{
            //    return this.Json(new { success = false, errormessage = ex.Message }, JsonRequestBehavior.AllowGet);
            //}
        }
    }
}
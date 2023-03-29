using FormBot.BAL.Service;
using FormBot.BAL.Service.InvoicerDetails;
using FormBot.Entity;
using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Xero.Api.Example.Applications.Public;
using Xero.Api.Infrastructure.Exceptions;
using Xero.NetStandard.OAuth2.Api;

namespace FormBot.Main.Controllers
{
    public class InvoicerDetailsController : Controller
    {
        //#region Properties
        //private readonly IInvoicerDetails _invoicerDetails;
        //#endregion

        //#region Constructor

        //public InvoicerDetailsController(IInvoicerDetails InvoicerDetails)
        //{
        //    this._invoicerDetails = InvoicerDetails;
        //}
        //#endregion

        InvoicerDetails _invoicerDetails = new InvoicerDetails();

        // GET: InvoicerDetails
        [UserAuthorization]
        public ActionResult Index()
        {
            FormBot.Entity.Invoicer invoicer = new FormBot.Entity.Invoicer();
            invoicer.UserTypeID = ProjectSession.UserTypeId;

            ViewBag.PostalAddressType = new SelectList(EnumExtensions.GetPostalAddressEnumList(), "ID", "Name");
            if (Convert.ToBoolean(TempData["isRecallSync"]) == true)
            {
                ViewBag.isRecallSync = true;
            }

            return View(invoicer);
        }

        /// <summary>
        /// Gets the invoicer details list.
        /// </summary>
        public void GetInvoicerList(string InvoicerName, string AccountCode)
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);

            IList<FormBot.Entity.Invoicer> lstInvoicerDetails = _invoicerDetails.GetInvoicerList(pageNumber, gridParam.PageSize, InvoicerName, AccountCode);

            if (lstInvoicerDetails.Count > 0)
            {
                gridParam.TotalDisplayRecords = lstInvoicerDetails.FirstOrDefault().TotalRecords;
                gridParam.TotalRecords = lstInvoicerDetails.FirstOrDefault().TotalRecords;
            }

            HttpContext.Response.Write(Grid.PrepareDataSet(lstInvoicerDetails, gridParam));
        }

        [HttpPost]
        public JsonResult SaveInvoicerDetails(Invoicer invoicer)
        {
            try
            {
                _invoicerDetails.SaveInvoicerDetails(invoicer);
            }
            catch (Exception ex)
            {
                return this.Json(new { success = false, errormessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            return this.Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get account codes from xero
        /// </summary>
        /// <returns>JsonResult</returns>
        [HttpGet]
        public JsonResult GetAccountCodesFromXero()
        {
            // get the previous url and store it in tempdata (It's value is used inside /Authorization/Callback method to redirect to views based on visited page)
            TempData["PreviousUrl"] = System.Web.HttpContext.Current.Request.UrlReferrer;
            TempData.Keep("PreviousUrl");

            TempData["isRecallSync"] = true;
            TempData.Keep("isRecallSync");
            if (ProjectSession.LoggedInUserId == 0)
                return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);

            try
            {
                if (!TokenUtilities.TokenExists())
                {
                    return Json(new { status = false, error = "invalid_grant" }, JsonRequestBehavior.AllowGet);
                }
                var token = TokenUtilities.GetStoredToken();
                string accessToken = token.AccessToken;
                string xeroTenantId = token.Tenants[0].TenantId.ToString();
                AccountingApi accountingApi = new AccountingApi();
                var accountsResponse = Task.Run(async () => await accountingApi.GetAccountsAsync(accessToken, xeroTenantId));
                accountsResponse.Wait();
                var accountsCodes = accountsResponse.Result._Accounts;

                if (accountsCodes.Count > 0)
                {
                    for (int i = 0; i < accountsCodes.Count; i++)
                    {
                        _invoicerDetails.SaveAccountCodes(accountsCodes[i].Code, accountsCodes[i].Name, accountsCodes[i].TaxType, i);
                    }
                    TempData["isRecallSync"] = false;
                    TempData.Keep("isRecallSync");
                }

                return Json(new { status = true, accountsCodes }, JsonRequestBehavior.AllowGet);
            }
            catch (RenewTokenException e)
            {
                //Response.Write(e.Message);
                return Json(new { status = false, error = "RenewTokenException" }, JsonRequestBehavior.AllowGet);
            }
            catch (XeroApiException e)
            {
                //Response.Write(e.Message);
                int errorCount = ((Xero.Api.Infrastructure.Exceptions.ValidationException)(e)).ValidationErrors.Count;
                string errorMsg = string.Join(", ", ((Xero.Api.Infrastructure.Exceptions.ValidationException)(e)).ValidationErrors.AsEnumerable().Select(a => a.Message));

                return Json(new { status = false, error = errorMsg }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                //Response.Write(e.Message);
                return Json(new { status = false, error = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
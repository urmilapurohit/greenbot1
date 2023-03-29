using FormBot.Entity.Settings;
using FormBot.Helper;
using FormBot.Helper.Helper;
using FormBot.Main.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Xero.Api.Example.Applications.Public;
using Xero.Api.Infrastructure.OAuth;
using FormBot.BAL;
using System.IO;
using FormBot.BAL.Service;
using Xero.Api.Infrastructure.Exceptions;
using FormBot.BAL.Service.CommonRules;
using Xero.NetStandard.OAuth2.Token;
using System.Threading.Tasks;
using Xero.NetStandard.OAuth2.Api;

namespace FormBot.Main.Controllers
{
    public class SettingsController : BaseController
    {
        #region Properties

        //private IMvcAuthenticator _authenticator;
        //private ApiUser _user;
        private readonly ISettingsBAL _settingsBAL;

        #endregion

        #region Constructor

        public SettingsController(ISettingsBAL settingsBAL)
        {
            //ProjectSession.XeroAuthorizeURL = ProjectConfiguration.XeroAuthorizeURL + "Settings/Authorize";
            //XeroApiHelper.RegenerateToken();
            //_user = XeroApiHelper.User();
            //_authenticator = XeroApiHelper.MvcAuthenticator();
            this._settingsBAL = settingsBAL;
        }

        #endregion

        #region Method

        // GET: Settings

        /// <summary>
        /// Sync account using xero api
        /// </summary>
        /// <returns>ActionResult</returns>
        public ActionResult SyncAccount()
        {
            if (ProjectSession.LoggedInUserId == 0)
                return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);

            Entity.Settings.Settings settings = new Entity.Settings.Settings();
            try
            {
                DataSet dsAccount = SyncXeroAccount();
                if (dsAccount == null)
                {
                    return Json(new { status = false, error = "specified method is not supported." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //settings = GetSettingsData();
                    //settings.SyncValue = 1;
                    TempData["SyncMsg"] = "Success";
                    return Json(new { status = true }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (RenewTokenException)
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

        /// <summary>
        /// Sync account using xero api
        /// </summary>
        /// <returns>ActionResult</returns>
        public DataSet SyncXeroAccount()
        {
            DataSet dsAccount = null;
            if (!TokenUtilities.TokenExists())
            {
                XeroConnectController xcc = new XeroConnectController();
                xcc.Connect();
            }
            var token = TokenUtilities.GetStoredToken();
            string accessToken = token.AccessToken;
            string xeroTenantId = token.Tenants[0].TenantId.ToString();

            AccountingApi accountingApi = new AccountingApi();

            //var api = XeroApiHelper.xeroApiHelperSession.CoreApi();
            var accountsResult = Task.Run(async () => await accountingApi.GetAccountsAsync(accessToken, xeroTenantId));
            accountsResult.Wait();
            var accounts = accountsResult.Result._Accounts;

            var taxRatesResult = Task.Run(async () => await accountingApi.GetTaxRatesAsync(accessToken, xeroTenantId));
            taxRatesResult.Wait();
            var taxRates = taxRatesResult.Result._TaxRates;

            var objAccounts = accounts.ToList();

            var objTaxRates = taxRates.ToList();

            int? solarCompanyId = ProjectSession.SolarCompanyId;

            if (ProjectSession.UserTypeId == 7 || ProjectSession.UserTypeId == 9 || ProjectSession.UserTypeId == 9)
                solarCompanyId = null;

            List<XeroAccount> lstAccount = objAccounts.AsEnumerable().Select(row =>
            new XeroAccount
            {
                Code = !string.IsNullOrEmpty(row.Code) ? row.Code : "",
                Name = !string.IsNullOrEmpty(row.Code) ? row.Name : "",
                XeroAccountId = row.AccountID.ToString(),
                TaxType = row.TaxType,
                EnablePayments = row.EnablePaymentsToAccount

            }).ToList();

            List<TaxRates> lstTaxRates = objTaxRates.AsEnumerable().Select(row =>
            new TaxRates
            {
                DisplayTaxRate = row.DisplayTaxRate != null ? Convert.ToDecimal(row.DisplayTaxRate) : 0,
                Name = row.Name,
                TaxType = row.TaxType

            }).ToList();

            string totalAccountIds = string.Empty;
            if (lstAccount.Count > 0)
            {
                totalAccountIds = string.Join(",", lstAccount.AsEnumerable().Select(r => r.XeroAccountId));
            }

            string accountJson = string.Empty;
            string taxRatesJson = string.Empty;
            accountJson = Newtonsoft.Json.JsonConvert.SerializeObject(lstAccount);
            taxRatesJson = Newtonsoft.Json.JsonConvert.SerializeObject(lstTaxRates);

            //SettingsBAL settingsBAL = new SettingsBAL();

            dsAccount = _settingsBAL.InsertAccountUsingSyncXero(accountJson, taxRatesJson, ProjectSession.LoggedInUserId, DateTime.Now, solarCompanyId, ProjectSession.UserTypeId, totalAccountIds, ProjectSession.LoggedInUserId, DateTime.Now);
            return dsAccount;
        }

        /// <summary>
        /// Get xero account list
        /// </summary>
        public void GetAccountList()
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);

            //SettingsBAL settingsBAL = new SettingsBAL();
            IList<XeroAccount> lstJobParts = _settingsBAL.GetXeroAccountList(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, ProjectSession.SolarCompanyId);
            if (lstJobParts.Count > 0)
            {
                gridParam.TotalDisplayRecords = lstJobParts.FirstOrDefault().TotalRecords;
                gridParam.TotalRecords = lstJobParts.FirstOrDefault().TotalRecords;
            }

            //isAllPart = false;
            HttpContext.Response.Write(Grid.PrepareDataSet(lstJobParts, gridParam));
        }

        /// <summary>
        /// Get account for partial view
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        public PartialViewResult GetAccountPartialView()
        {
            //Entity.Settings.Settings settings = GetSettingsData();
            return PartialView("_Account", new XeroAccount());
        }

        /// <summary>
        /// Get settings data
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [UserAuthorization]
        public ActionResult Settings()
        {
            Entity.Settings.Settings settings = GetSettingsData();
            return View(settings);
        }

        [HttpGet]
        [UserAuthorization]
        public ActionResult ChangeReseller()
        {

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ChangeReseller(string SolarCompanyId, string NewResellerId)
        {
            try
            {
                int oldResellerId = _settingsBAL.ChangeReseller(Convert.ToInt32(SolarCompanyId), Convert.ToInt32(NewResellerId));
                await CommonBAL.SetCacheDataForSTCSubmissionFromSolarCompanyId(Convert.ToInt32(NewResellerId));
                await CommonBAL.SetCacheDataForSTCSubmissionFromSolarCompanyId(Convert.ToInt32(oldResellerId));
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
            // return View();
        }

        [HttpGet]
        public ActionResult GetCompanyList()
        {
            DataSet ds = _settingsBAL.GetSolarCompanies();
            return Json(new { status = true, data = Newtonsoft.Json.JsonConvert.SerializeObject(ds) }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get settings data
        /// </summary>
        /// <returns>ActionResult</returns>
        public Entity.Settings.Settings GetSettingsData()
        {
            //SettingsBAL settingsBAL = new SettingsBAL();
            Entity.Settings.Settings settings = new Entity.Settings.Settings();
            int? solarCompanyId = 0;
            int? resellerId = 0;

            if (ProjectSession.UserTypeId == 7 || ProjectSession.UserTypeId == 9)
                solarCompanyId = null;
            else
                solarCompanyId = ProjectSession.SolarCompanyId;

            if (ProjectSession.UserTypeId == 2 || ProjectSession.UserTypeId == 5)
                resellerId = ProjectSession.ResellerId;
            else
                resellerId = 0;

            settings = _settingsBAL.GetChargesPartsPaymentCodeAndSettings(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, solarCompanyId, resellerId);
            return settings;
        }

        /// <summary>
        /// Save settings data
        /// </summary>
        /// <param name="settings"></param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public JsonResult Settings(Entity.Settings.Settings settings)
        {
            try
            {
                RemoveRequiredValidationField(settings);
                if (ModelState.IsValid)
                {
                    //SettingsBAL settingsBAL = new SettingsBAL();

                    if (ProjectSession.UserTypeId == 2 || ProjectSession.UserTypeId == 5)
                    {
                        settings.SolarCompanyId = null;
                        settings.ResellerId = ProjectSession.ResellerId;
                    }
                    else
                    {
                        settings.SolarCompanyId = ProjectSession.SolarCompanyId;
                        settings.ResellerId = null;
                    }

                    if (settings.Signature != null && settings.Signature != "")
                        settings.Logo = settings.Signature;

                    if (settings.Logo != settings.OldLogo)
                        DeleteFile(Convert.ToString(settings.UserId), settings.OldLogo);

                    if (settings.SettingsId > 0)
                    {
                        settings.ModifiedBy = ProjectSession.LoggedInUserId;
                        settings.ModifiedDate = DateTime.Now;
                        settings.CreatedDate = null;
                    }
                    else
                    {
                        settings.CreatedBy = ProjectSession.LoggedInUserId;
                        settings.CreatedDate = DateTime.Now;
                        settings.ModifiedDate = null;
                    }
                    int settingId = _settingsBAL.InsertUpdateInvoiceSettings(settings);

                    ProjectSession.IsTaxInclusive = settings.IsTaxInclusive;
                    ProjectSession.PartAccountTax = settings.PartAccountTax;

                    return Json(new { status = settingId, logo = settings.Logo }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string msg = string.Empty;
                    ModelState.Values.AsEnumerable().ToList().ForEach(d =>
                    {
                        if (d.Errors.Count > 0)
                            msg = d.Errors[0].ErrorMessage;
                    });
                    //return Json(0 + "#" + msg, JsonRequestBehavior.AllowGet);
                    return Json(new { status = 0 + "#" + msg, data = settings.Logo }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Remove required validation field
        /// </summary>
        /// <param name="settings"></param>
        public void RemoveRequiredValidationField(Entity.Settings.Settings settings)
        {
            if (ProjectSession.UserTypeId != 2 && ProjectSession.UserTypeId != 5)
            {
                if (settings.IsXeroAccount)
                {
                    ModelState.Remove("PartCode");
                    ModelState.Remove("PartName");
                    ModelState.Remove("PartTax");
                    ModelState.Remove("PaymentCode");
                    ModelState.Remove("PaymentName");
                    ModelState.Remove("ChargeTax");
                }
                else
                {
                    ModelState.Remove("XeroPartsCodeId");
                    ModelState.Remove("XeroPaymentsCodeId");
                    ModelState.Remove("XeroAccountCodeId");
                    ModelState.Remove("XeroChargeCodeId");
                }

                if (!settings.IsTaxInclusive)
                    ModelState.Remove("TaxRate");
            }
            else
            {
                ModelState.Remove("PartCode");
                ModelState.Remove("PartName");
                ModelState.Remove("PartTax");
                ModelState.Remove("PaymentCode");
                ModelState.Remove("PaymentName");
                ModelState.Remove("PaymentTax");
                ModelState.Remove("XeroPartsCodeId");
                ModelState.Remove("XeroPaymentsCodeId");
                ModelState.Remove("XeroAccountCodeId");
                ModelState.Remove("TaxRate");
                ModelState.Remove("XeroChargeCodeId");
            }
            ModelState.Remove("PaymentTax");
        }

        /// <summary>
        /// Deletes the directory.
        /// </summary>
        /// <param name="path">The path.</param>
        private void DeleteDirectory(string path)
        {
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
        }

        /// <summary>
        /// Delete settings logo from folder
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="FolderName"></param>
        /// <param name="OldLogo"></param>
        /// <returns>ActionResult</returns>
        public JsonResult DeleteLogoFromFolderSettings(string fileName, string FolderName, string OldLogo)
        {
            if (OldLogo != fileName && fileName != null)
            {
                DeleteFile(FolderName, fileName);
                this.ShowMessage(SystemEnums.MessageType.Success, "File has been deleted successfully.", false);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="FolderName"></param>
        /// <param name="fileName"></param>
        public void DeleteFile(string FolderName, string fileName)
        {
            DeleteDirectory(Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + "UserDocuments" + "\\" + FolderName + "\\" + fileName));
        }

        /// <summary>
        /// Get All Parts Code
        /// </summary>
        /// <returns>JsonResult</returns>
        public JsonResult GetAllPartsCode()
        {
            Entity.Settings.Settings settings = new Entity.Settings.Settings();
            int? solarCompanyId = 0;
            int? resellerId = 0;

            if (ProjectSession.UserTypeId == 7 || ProjectSession.UserTypeId == 9)
                solarCompanyId = null;
            else
                solarCompanyId = ProjectSession.SolarCompanyId;

            if (ProjectSession.UserTypeId == 2 || ProjectSession.UserTypeId == 5)
                resellerId = ProjectSession.ResellerId;
            else
                resellerId = 0;

            settings = _settingsBAL.GetChargesPartsPaymentCodeAndSettings(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, solarCompanyId, resellerId);

            List<SelectListItem> Items = settings.lstXeroPartsCodeId.Select(a => new SelectListItem { Text = a.Name, Value = a.Code.ToString() }).ToList();
            return Json(Items, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
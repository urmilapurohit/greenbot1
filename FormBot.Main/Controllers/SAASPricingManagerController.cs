using FormBot.BAL.Service;
using FormBot.Entity;
using FormBot.Helper;
using FormBot.Helper.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FormBot.Main.Controllers
{
    public class SAASPricingManagerController : Controller
    {
        #region Properties
        private readonly ISAASPricingManagerBAL _saaspricingManagerBAL;
        #endregion

        #region Constructor

        public SAASPricingManagerController(ISAASPricingManagerBAL pricingManager)
        {
            this._saaspricingManagerBAL = pricingManager;
        }

        #endregion

        // GET: SAASPricingManager
        [UserAuthorization]
        public ActionResult Index()
        {
            return View("Index");
        }

        /// <summary>
        /// Gets the global price list.
        /// </summary>
        public JsonResult GetGlobalPricingList(bool IsIsArchive)
        {
            IList<FormBot.Entity.GlobalBillableTerms> lstGlobalPrice = _saaspricingManagerBAL.GetGlobalPricingList(IsIsArchive);

            return Json(lstGlobalPrice, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGlobalBillingTermsList()
        {
            IList<FormBot.Entity.SAASPricingManager> lstGlobalBillingTerms = _saaspricingManagerBAL.GetGlobalBillingTermsList();

            List<SAASPricingManager> tlistFiltered = lstGlobalBillingTerms
                .Where(item => item.IsEnable == true)
                .ToList();

            return Json(tlistFiltered, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the SAAS price list.
        /// </summary>
        public void GetSAASPricingList(string RoleID, string TermCode, string UserType, string BillerCode, string TermName)
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);

            IList<FormBot.Entity.SAASPricingManager> lstPrice = _saaspricingManagerBAL.GetSAASPricingList(pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, RoleID, TermCode, UserType, BillerCode, TermName);

            if (lstPrice.Count > 0)
            {
                gridParam.TotalDisplayRecords = lstPrice.FirstOrDefault().TotalRecords;
                gridParam.TotalRecords = lstPrice.FirstOrDefault().TotalRecords;
            }

            HttpContext.Response.Write(Grid.PrepareDataSet(lstPrice, gridParam));
        }

        /// <summary>
        /// Gets the users role wise.
        /// </summary>
        public void GetRoleWiseUserSAAS(int RoleId, string UserName, string RoleName, string TermCode)
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);

            IList<FormBot.Entity.SAASPricingManager> lstRoleWiseUsers = _saaspricingManagerBAL.GetRoleWiseUserSAAS(pageNumber, gridParam.PageSize, RoleId, UserName, RoleName, TermCode);

            if (lstRoleWiseUsers.Count > 0)
            {
                gridParam.TotalDisplayRecords = lstRoleWiseUsers.FirstOrDefault().TotalRecords;
                gridParam.TotalRecords = lstRoleWiseUsers.FirstOrDefault().TotalRecords;
            }

            HttpContext.Response.Write(Grid.PrepareDataSet(lstRoleWiseUsers, gridParam));
        }

        /// <summary>
        /// Gets all saas users list .
        /// </summary>
        public void GetAllRoleUserList(string RoleID, string TermCode, string UserType, string BillableCode, string ResellerId, string SolarCompany)
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);

            IList<FormBot.Entity.SAASPricingManager> lstUsers = _saaspricingManagerBAL.GetAllRoleUserList(pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, RoleID, TermCode, UserType, BillableCode, ResellerId, SolarCompany);

            if (lstUsers.Count > 0)
            {
                gridParam.TotalDisplayRecords = lstUsers.FirstOrDefault().TotalRecords;
                gridParam.TotalRecords = lstUsers.FirstOrDefault().TotalRecords;
            }

            HttpContext.Response.Write(Grid.PrepareDataSet(lstUsers, gridParam));
        }

        [HttpPost]
        public JsonResult SavePricingSAAS(string SAASPricingId, string SAASUserId, string SettlementTermId, bool IsEnable, string Price, string IsGst, string BillingPeriod, string SettlementPeriod)
        {
            try
            {
                _saaspricingManagerBAL.SavePriceForSAAS(Convert.ToInt32(SAASPricingId), Convert.ToInt32(SAASUserId), Convert.ToInt32(SettlementTermId), IsEnable, Convert.ToDecimal(Price), false, Convert.ToInt32(BillingPeriod), Convert.ToInt32(SettlementPeriod));
            }
            catch (Exception ex)
            {
                return this.Json(new { success = false, errormessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            return this.Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveUserBillableSettings(string RoleId, string GlobalTermId, string Price, bool IsGST, int DATAOPMODE, int UserId = 0, int BillableSettingsId = 0, string strBillableSettingsId = "")
        {
            try
            {
                string UserHistoryMessage = "";
                IList<FormBot.Entity.SAASPricingManager> lstHistoryData = _saaspricingManagerBAL.SaveUserBillableSettings(Convert.ToInt32(RoleId), Convert.ToInt32(GlobalTermId), Convert.ToDecimal(Price), DATAOPMODE, UserId, BillableSettingsId, IsGST);

                //lstHistoryData = _saaspricingManagerBAL.GetBillingManagerHistoryData(GlobalTermId, RoleId, UserId, DATAOPMODE, BillableSettingsId, strBillableSettingsId, Convert.ToDecimal(Price));
                if (DATAOPMODE == 1)
                {
                    for (int i = 0; i < lstHistoryData.Count; i++)
                    {
                        UserHistoryMessage = "has added billing term (" + lstHistoryData[i].BillerCode + ") " + lstHistoryData[i].TermName + " having term code " + lstHistoryData[i].TermCode + " with global price at " + lstHistoryData[i].Price;
                        Common.SaveUserHistorytoXML(lstHistoryData[i].UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                    }
                }
                else if (DATAOPMODE == 2)
                {
                    UserHistoryMessage = "has added billing term (" + lstHistoryData[0].BillerCode + ") " + lstHistoryData[0].TermName + " having term code " + lstHistoryData[0].TermCode + " with global price at " + lstHistoryData[0].Price;
                    Common.SaveUserHistorytoXML(lstHistoryData[0].UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                }
                else if (DATAOPMODE == 2)
                {
                    UserHistoryMessage = "has deleted billing term having term code " + lstHistoryData[0].TermCode + " and biller code (" + lstHistoryData[0].BillerCode + ")";
                    Common.SaveUserHistorytoXML(lstHistoryData[0].UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                }
                else
                {
                    for (int i = 0; i < lstHistoryData.Count; i++)
                    {
                        UserHistoryMessage = "has deleted billing term having term code " + lstHistoryData[i].TermCode + " and biller code (" + lstHistoryData[i].BillerCode + ")";
                        Common.SaveUserHistorytoXML(lstHistoryData[i].UserId, UserHistoryMessage, ProjectSession.LoggedInName, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                    }
                }
            }
            catch (Exception ex)
            {
                return this.Json(new { success = false, errormessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            return this.Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
    }
}
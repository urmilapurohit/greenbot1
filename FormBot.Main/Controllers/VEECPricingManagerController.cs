using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FormBot.Helper;
using FormBot.BAL.Service;
using System.Reflection;
using System.ComponentModel;
using System.Data;
using FormBot.Helper.Helper;

namespace FormBot.Main.Controllers
{
    public class VEECPricingManagerController : Controller
    {
        #region Properties
        private readonly IVEECPricingManagerBAL _veecPricingManagerBAL;
        private readonly ISolarCompanyBAL _solarcompanyBAL;
        #endregion

        #region Constructor

        public VEECPricingManagerController(IVEECPricingManagerBAL veecPricingManager, ISolarCompanyBAL solarcompanyBAL)
        {
            this._veecPricingManagerBAL = veecPricingManager;
            this._solarcompanyBAL = solarcompanyBAL;
        }

        #endregion

        // GET: VEECPricingManager
        public ActionResult Index()
        {
            FormBot.Entity.VEECPricingManager veecPricingManager = new Entity.VEECPricingManager();
            veecPricingManager.UserTypeID = ProjectSession.UserTypeId;

            return View("Index", veecPricingManager);
        }

        [HttpGet]
        public ActionResult _ManageVEECPrice(string PricingMode, string PricingType, string ResellerId, string solarcompany, string ram, string name, string OwnerName, string SolarCompanyId, string InstallationAddress, string RefNumber)
        {
            int ramId = 0;
            int ResId = 0;

            if (ProjectSession.UserTypeId == 1 || ProjectSession.UserTypeId == 3)
            {
                ResId = !string.IsNullOrEmpty(ResellerId) ? Convert.ToInt32(ResellerId) : 0;
            }
            else
            {
                ResId = ProjectSession.ResellerId;
            }

            if (ProjectSession.UserTypeId == 1 || ProjectSession.UserTypeId == 3 || ProjectSession.UserTypeId == 2)
            {
                ramId = !string.IsNullOrEmpty(ram) ? Convert.ToInt32(ram) : 0;
            }
            else
            {
                ramId = ProjectSession.LoggedInUserId;
            }

            FormBot.Entity.VEECPricingManager model = _veecPricingManagerBAL.GetVEECGlobalPriceForReseller(Convert.ToInt32(ResellerId));
            model.PricingType = Convert.ToInt32(PricingType);
            model.PricingMode = Convert.ToInt32(PricingMode);

            if (model.PricingMode == 1)
            {
                if (model.PricingType == 1)
                {
                    if (ProjectSession.UserTypeId == 1 || ProjectSession.UserTypeId == 2 || ProjectSession.UserTypeId == 3)
                    {
                        model.lstRightSide = _solarcompanyBAL.GetSolarCompanyForVEECGlobalPricing(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, ResId, 1).Select(a => new SelectListItem { Text = a.CompanyName, Value = a.SolarCompanyId.ToString() }).ToList();
                        model.lstLeftSide = _solarcompanyBAL.GetSolarCompanyForVEECGlobalPricing(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, ResId, 2).Select(a => new SelectListItem { Text = a.CompanyName, Value = a.SolarCompanyId.ToString() }).ToList();
                    }
                }
                else
                {
                    model.lstLeftSide = new List<SelectListItem>();
                    model.lstRightSide = _solarcompanyBAL.GetSolarCompanyForCustomPricing(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, ResId, solarcompany, ramId, name).Select(a => new SelectListItem { Text = a.CompanyName, Value = a.SolarCompanyId.ToString() }).ToList();
                }
            }
            else
            {
                if (model.PricingType == 2)
                {
                    int ScID = !string.IsNullOrEmpty(SolarCompanyId) ? Convert.ToInt32(SolarCompanyId) : 0;
                    if (ProjectSession.UserTypeId == 1 || ProjectSession.UserTypeId == 2 || ProjectSession.UserTypeId == 3 || ProjectSession.UserTypeId == 5)
                    {
                        model.lstLeftSide = new List<SelectListItem>();
                        model.lstRightSide = _veecPricingManagerBAL.GetVEECsForCustomPricing(ScID, OwnerName, InstallationAddress, RefNumber).Select(a => new SelectListItem { Text = a.RefNumber, Value = a.VEECId.ToString() }).ToList();
                    }
                }
            }

            Type enumType = typeof(FormBot.Helper.SystemEnums.STCSettlementTerm);
            Type descriptionAttributeType = typeof(DescriptionAttribute);
            Dictionary<int, string> dict = new Dictionary<int, string>();


            foreach (string memberName in Enum.GetNames(enumType))
            {
                MemberInfo member = enumType.GetMember(memberName).Single();
                string memberDescription = ((DescriptionAttribute)Attribute.GetCustomAttribute(member, descriptionAttributeType)).Description;
                int value = Convert.ToInt32(Enum.Parse(typeof(FormBot.Helper.SystemEnums.STCSettlementTerm), memberName));

                if (model.PricingMode == 1)
                {
                    if (value != 5 && value != 6 && value != 10)
                    {
                        dict.Add(value, memberDescription);
                    }
                }
            }

            model.SettlementTermList = dict;

            return PartialView(model);
        }

        public void GetSolarCompanyForVEECPricingManager(string reseller = "", string RAM = "", string solarcompany = "", string name = "")
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);
            int rid = 0;
            int ramid = 0;
            if (ProjectSession.UserTypeId == 2)
            {
                rid = ProjectSession.ResellerId;
            }
            else
            {
                rid = !string.IsNullOrEmpty(reseller) ? Convert.ToInt32(reseller) : 0;
            }
            if (ProjectSession.UserTypeId == 5)
            {
                ramid = ProjectSession.LoggedInUserId;
            }
            else
            {
                ramid = !string.IsNullOrEmpty(RAM) ? Convert.ToInt32(RAM) : 0;
            }

            IList<FormBot.Entity.VEECPricingManager> lstSCAUser = _veecPricingManagerBAL.GetSCAUserForVEECPricingManager(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, rid, ramid, solarcompany, name);

            if (lstSCAUser.Count > 0)
            {
                gridParam.TotalDisplayRecords = lstSCAUser.FirstOrDefault().TotalRecords;
                gridParam.TotalRecords = lstSCAUser.FirstOrDefault().TotalRecords;
            }

            HttpContext.Response.Write(Grid.PrepareDataSet(lstSCAUser, gridParam));
        }

        public void GetVEECsForVEECPricingManager(string reseller = "", string RAM = "", string solarcompanyid = "", string veecRef = "", string homeownername = "", string veecInstallationAddress = "")
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);
            int rid = 0;
            int ramid = 0;
            int scId = !string.IsNullOrEmpty(solarcompanyid) ? Convert.ToInt32(solarcompanyid) : 0;

            if (ProjectSession.UserTypeId == 1 || ProjectSession.UserTypeId == 3)
            {
                rid = !string.IsNullOrEmpty(reseller) ? Convert.ToInt32(reseller) : 0;
            }
            else
            {
                rid = ProjectSession.ResellerId;
            }

            if (ProjectSession.UserTypeId == 1 || ProjectSession.UserTypeId == 3 || ProjectSession.UserTypeId == 2)
            {
                ramid = !string.IsNullOrEmpty(RAM) ? Convert.ToInt32(RAM) : 0;
            }
            else
            {
                ramid = ProjectSession.LoggedInUserId;
            }

            IList<FormBot.Entity.VEECPricingManager> lstVEECs = _veecPricingManagerBAL.GetVEECsForVEECPricingManager(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, rid, ramid, scId, veecRef, homeownername, veecInstallationAddress);

            if (lstVEECs.Count > 0)
            {
                gridParam.TotalDisplayRecords = lstVEECs.FirstOrDefault().TotalRecords;
                gridParam.TotalRecords = lstVEECs.FirstOrDefault().TotalRecords;
            }

            HttpContext.Response.Write(Grid.PrepareDataSet(lstVEECs, gridParam));
        }

        public ActionResult SaveVEECPriceForSolarCompany(string PricingType, string items, string expiryDate, string ResellerId, string solarCompanyId, string optiPay)
        {
            int pType = Convert.ToInt32(PricingType);
            decimal optiPayPrice = !string.IsNullOrEmpty(optiPay) ? Convert.ToDecimal(optiPay) : 0;

            int RId = 0;
            if (ProjectSession.UserTypeId == 1 || ProjectSession.UserTypeId == 3)
            {
                RId = !string.IsNullOrEmpty(ResellerId) ? Convert.ToInt32(ResellerId) : 0;
            }
            else if (ProjectSession.UserTypeId == 2 || ProjectSession.UserTypeId == 5)
            {
                RId = ProjectSession.ResellerId;
            }

            if (pType == 1)
            {
                if (ProjectSession.UserTypeId == 1 || ProjectSession.UserTypeId == 2 || ProjectSession.UserTypeId == 3)
                {
                    _veecPricingManagerBAL.SaveVEECGlobalPriceForSolarCompany(RId, optiPayPrice);
                }
            }
            else
            {
                DateTime ExpiryDate = DateTime.Now;
                if (!string.IsNullOrEmpty(expiryDate))
                {
                    ExpiryDate = Convert.ToDateTime(expiryDate);
                }
                List<int> lstSolaCompany = new List<int>();
                foreach (string company in items.Split(','))
                {
                    int cID = 0;
                    if (!string.IsNullOrEmpty(company))
                    {
                        cID = Convert.ToInt32(company);
                        lstSolaCompany.Add(cID);
                    }
                }

                if (lstSolaCompany != null && lstSolaCompany.Count > 0)
                {
                    _veecPricingManagerBAL.SaveVEECCustomPriceForSolarCompany(lstSolaCompany, ExpiryDate, RId, optiPayPrice);
                }
            }

            return this.Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveVEECPriceForVEEC(string PricingType, string items, string expiryDate, string ResellerId, string solarCompanyId, string optiPay)
        {
            int SolarCompnyId = !string.IsNullOrEmpty(solarCompanyId) ? Convert.ToInt32(solarCompanyId) : 0;
            decimal optiPayPrice = !string.IsNullOrEmpty(optiPay) ? Convert.ToDecimal(optiPay) : 0;

            DateTime ExpiryDate = DateTime.Now;
            if (!string.IsNullOrEmpty(expiryDate))
            {
                ExpiryDate = Convert.ToDateTime(expiryDate);
            }
            List<int> lstVEECs = new List<int>();
            foreach (string veec in items.Split(','))
            {
                int jID = 0;
                if (!string.IsNullOrEmpty(veec))
                {
                    jID = Convert.ToInt32(veec);
                    lstVEECs.Add(jID);
                }
            }

            if (lstVEECs != null && lstVEECs.Count > 0)
            {
                _veecPricingManagerBAL.SaveVEECCustomPriceForVeec(lstVEECs, ExpiryDate, SolarCompnyId, optiPayPrice);
            }
            return this.Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetVEECGlobalPriceForReseller(string rId)
        {
            if (!string.IsNullOrEmpty(rId))
            {
                int Rid = Convert.ToInt32(rId);
                FormBot.Entity.VEECPricingManager veecPricingManager = _veecPricingManagerBAL.GetVEECGlobalPriceForReseller(Rid);
                if (veecPricingManager.CustomSettlementTerm > 0)
                {
                    veecPricingManager.CustomTermText = "Custom - " + Common.GetDescription((FormBot.Helper.SystemEnums.STCSettlementTerm)veecPricingManager.CustomSettlementTerm, "");
                }
                return this.Json(new { success = true, price = veecPricingManager }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return this.Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
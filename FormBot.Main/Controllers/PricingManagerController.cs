using FormBot.BAL.Service;
using FormBot.BAL.Service.CommonRules;
using FormBot.Entity;
using FormBot.Entity.Job;
using FormBot.Helper;
using FormBot.Helper.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;

namespace FormBot.Main.Controllers
{
    public class PricingManagerController : Controller
    {
        #region Properties
        private readonly IPricingManagerBAL _pricingManagerBAL;
        private readonly ISolarCompanyBAL _solarcompanyBAL;
        private readonly ICreateJobBAL _jobBAL;
        private readonly ICreateJobHistoryBAL _createJobHistoryBAL;
        #endregion

        #region Constructor

        public PricingManagerController(IPricingManagerBAL pricingManager, ISolarCompanyBAL solarcompanyBAL, ICreateJobBAL jobBAL, ICreateJobHistoryBAL createJobHistoryBAL)
        {
            this._pricingManagerBAL = pricingManager;
            this._solarcompanyBAL = solarcompanyBAL;
            this._jobBAL = jobBAL;
            this._createJobHistoryBAL = createJobHistoryBAL;
        }

        #endregion

        #region Events

        [HttpGet]
        [UserAuthorization]
        public ActionResult Index()
        {
            FormBot.Entity.PricingManager pricingManager = new Entity.PricingManager();
            pricingManager.UserTypeID = ProjectSession.UserTypeId;

            FormBot.Entity.PricingManager pmanager = _pricingManagerBAL.GetGlobalPriceForReseller(ProjectSession.ResellerId);


            return View("Index", pricingManager);
        }

        [HttpGet]
        //[UserAuthorization]
        public ActionResult _ManagePrice(string PricingMode, string PricingType, string ResellerId, string solarcompany, string ram, string name, string OwnerName, string SolarCompanyId, string OwnerAddress, string RefNumber, string systemsize)
        {
            //var model = new FormBot.Entity.PricingManager();
            FormBot.Entity.PricingManager model = _pricingManagerBAL.GetGlobalPriceForReseller(Convert.ToInt32(ResellerId));
            model.PricingType = Convert.ToInt32(PricingType);
            model.PricingMode = Convert.ToInt32(PricingMode);

            int ramId = 0;
            int ResId = 0;

            int SystemSize = !string.IsNullOrEmpty(systemsize) ? Convert.ToInt32(systemsize) : 0;

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

            if (model.PricingMode == 1)
            {
                if (model.PricingType == 1)
                {
                    if (ProjectSession.UserTypeId == 1 || ProjectSession.UserTypeId == 2 || ProjectSession.UserTypeId == 3)
                    {
                        //model.Hour24 = pmanager.Hour24;

                        model.lstRightSide = _solarcompanyBAL.GetSolarCompanyForGlobalPricing(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, ResId, 1).Select(a => new SelectListItem { Text = a.CompanyName, Value = a.SolarCompanyId.ToString() }).ToList();
                        model.lstLeftSide = _solarcompanyBAL.GetSolarCompanyForGlobalPricing(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, ResId, 2).Select(a => new SelectListItem { Text = a.CompanyName, Value = a.SolarCompanyId.ToString() }).ToList();
                    }
                }
                else
                {
                    // model.Hour24 = pmanager.Hour24;
                    model.lstLeftSide = new List<SelectListItem>();
                    model.lstRightSide = _solarcompanyBAL.GetSolarCompanyForCustomPricing(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, ResId, solarcompany, ramId, name).Select(a => new SelectListItem { Text = a.CompanyName, Value = a.SolarCompanyId.ToString() }).ToList();
                }
            }
            else
            {
                if (model.PricingType == 2)
                {
                    int ScID = !string.IsNullOrEmpty(SolarCompanyId) ? Convert.ToInt32(SolarCompanyId) : 0;
                    model.SystemSize = SystemSize;
                    if (ProjectSession.UserTypeId == 1 || ProjectSession.UserTypeId == 2 || ProjectSession.UserTypeId == 3 || ProjectSession.UserTypeId == 5)
                    {
                        //model.Hour24 = pmanager.Hour24;
                        model.lstLeftSide = new List<SelectListItem>();
                        model.lstRightSide = _jobBAL.GetJobsForCustomPricing(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, ResId, ScID, SystemSize, OwnerName, OwnerAddress, RefNumber).Select(a => new SelectListItem { Text = a.JobID + " - " + a.RefNumber + "-" + a.Ownername + " - " + a.STC, Value = a.JobID.ToString() }).ToList();
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

                if (model.PricingMode == 1 || SystemSize == 1)
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

        [HttpGet]
        //[UserAuthorization]
        public ActionResult _ManagePriceSAAS(string ResellerId)
        {
            List<FormBot.Entity.PricingManagerSAAS> model = _pricingManagerBAL.GetGlobalPriceForSAAS(Convert.ToInt32(ResellerId), false);
            return PartialView(model);
        }

        [HttpPost]
        public JsonResult SavePricingSAAS(List<PricingManagerSAAS> PricingManager)
        {
            try
            {
                for (int i = 0; i <= PricingManager.Count - 1; i++)
                {
                    _pricingManagerBAL.SavePriceForSAAS(PricingManager[i]);
                }
            }
            catch (Exception ex)
            {
                return this.Json(new { success = false, errormessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            return this.Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        //[HttpGet]
        //public ActionResult _OptiPayOptions(string ResellerId)
        //{
        //    FormBot.Entity.PricingManager model = _pricingManagerBAL.GetGlobalPriceForReseller(Convert.ToInt32(ResellerId));
        //    return View("_OptiPayOptions");
        //}

        #endregion

        #region Mehods

        public void GetSolarCompanyForPricingManager(string reseller = "", string RAM = "", string solarcompany = "", string name = "", string wholeSaler = "")
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
            if (!string.IsNullOrEmpty(wholeSaler))
            {
                rid = Convert.ToInt32(wholeSaler);
                ramid = 0;
            }
            IList<FormBot.Entity.PricingManager> lstSCAUser = _pricingManagerBAL.GetSCAUserForPricingManager(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, rid, ramid, solarcompany, name);

            if (lstSCAUser.Count > 0)
            {
                gridParam.TotalDisplayRecords = lstSCAUser.FirstOrDefault().TotalRecords;
                gridParam.TotalRecords = lstSCAUser.FirstOrDefault().TotalRecords;
            }

            HttpContext.Response.Write(Grid.PrepareDataSet(lstSCAUser, gridParam));
        }

        public void GetJobsForPricingManager(string reseller = "", string RAM = "", string systemsize = "", string solarcompanyid = "", string jobref = "", string homeownername = "", string homeowneraddress = "", string wholeSaler = "")
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);
            int rid = 0;
            int ramid = 0;
            int scId = !string.IsNullOrEmpty(solarcompanyid) ? Convert.ToInt32(solarcompanyid) : 0;
            int SystemSize = !string.IsNullOrEmpty(systemsize) ? Convert.ToInt32(systemsize) : 1;

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
            if (!string.IsNullOrEmpty(wholeSaler))
            {
                rid = Convert.ToInt32(wholeSaler);
                ramid = 0;
            }
            IList<FormBot.Entity.PricingManager> lstJobs = _pricingManagerBAL.GetJobsForPricingManager(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, rid, ramid, SystemSize, scId, jobref, homeownername, homeowneraddress);

            if (lstJobs.Count > 0)
            {
                gridParam.TotalDisplayRecords = lstJobs.FirstOrDefault().TotalRecords;
                gridParam.TotalRecords = lstJobs.FirstOrDefault().TotalRecords;
            }

            HttpContext.Response.Write(Grid.PrepareDataSet(lstJobs, gridParam));
        }

        public ActionResult SavePriceForSolarCompany(string PricingType, string items, string day1price, string day3price, string day7price, string onapprovalprice,
            string expiryDate, string ResellerId, string solarCompanyId, string upFront, string partialPayment, string initialstc, string systemSize,
            string rapidPay, string optiPay, string commercial, string custom, string invoiceStcPrice, string customSettlementTerm, string UnderKW, string KWvalue, string CommercialJob, string NonCommercialJob,
            string IsCustomUnderKw, string CustomKWValue, string IsCustomCommercialJob, string IsCustomNonCommercialJob, string IsPriceDay1, string IsPriceDay3, string IsPriceDay7, string IsPriceApproval,
            string IsPriceRapidPay, string IsPriceOptiPay, string IsPriceCommercial, string IsPriceCustom, string IsPriceInvoiceStc, string peakPay, string IsTimePeriod, string timePeriod, string IsFee, string fee,
            string IsPricePeakPay, string IsCustomTimePeriod, string customTimePeriod, string IsCustomFee, string customFee, bool IsPeakPayGst, string peakPayGst, bool IsPeakPayCommercialJob, bool IsPeakPayNonCommercialJob,
            string peakPayStcPrice, bool IsCustomPeakPayGst, string customPeakPayGst, bool IsCustomPeakPayCommercialJob, bool IsCustomPeakPayNonCommercialJob, string customPeakPayStcPrice)
        {
            //try
            //{
            int pType = Convert.ToInt32(PricingType);
            decimal Day1Price = Convert.ToDecimal(day1price);
            decimal Day3Price = Convert.ToDecimal(day3price);
            decimal Day7Price = Convert.ToDecimal(day7price);
            decimal OnApprovalPrice = Convert.ToDecimal(onapprovalprice);

            decimal rapidPayPrice = !string.IsNullOrEmpty(rapidPay) ? Convert.ToDecimal(rapidPay) : 0;
            decimal optiPayPrice = !string.IsNullOrEmpty(optiPay) ? Convert.ToDecimal(optiPay) : 0;
            decimal commercialPrice = !string.IsNullOrEmpty(commercial) ? Convert.ToDecimal(commercial) : 0;
            decimal customPrice = !string.IsNullOrEmpty(custom) ? Convert.ToDecimal(custom) : 0;

            decimal InvoiceStcPrice = !string.IsNullOrEmpty(invoiceStcPrice) ? Convert.ToDecimal(invoiceStcPrice) : 0;

            bool KW = Convert.ToBoolean(UnderKW);
            int Value_KW = !string.IsNullOrEmpty(KWvalue) ? Convert.ToInt32(KWvalue) : 0;
            bool CommercialJobClaim = Convert.ToBoolean(CommercialJob);
            bool NonCommercialJobClaim = Convert.ToBoolean(NonCommercialJob);

            bool customKW = Convert.ToBoolean(IsCustomUnderKw);
            int customValue_KW = !string.IsNullOrEmpty(CustomKWValue) ? Convert.ToInt32(CustomKWValue) : 0;
            bool customCommercialJobClaim = Convert.ToBoolean(IsCustomCommercialJob);
            bool customNonCommercialJobClaim = Convert.ToBoolean(IsCustomNonCommercialJob);

            decimal PeakPayPrice = !string.IsNullOrEmpty(peakPay) ? Convert.ToDecimal(peakPay) : 0;
            bool IsTimePeriod_Value = Convert.ToBoolean(IsTimePeriod);
            int TimePeriodValue = !string.IsNullOrEmpty(timePeriod) ? Convert.ToInt32(timePeriod) : 0;
            bool IsFee_Value = Convert.ToBoolean(IsFee);
            decimal FeeValue = !string.IsNullOrEmpty(fee) ? Convert.ToDecimal(fee) : 0;
            bool IsPeakPayGst_Value = Convert.ToBoolean(IsPeakPayGst);
            decimal PeakPayGst_Value = !string.IsNullOrEmpty(peakPayGst) ? Convert.ToDecimal(peakPayGst) : 0;
            bool IsPeakPay_CommercialJob = Convert.ToBoolean(IsPeakPayCommercialJob);
            bool IsPeakPay_NonCommercialJob = Convert.ToBoolean(IsPeakPayNonCommercialJob);
            int PeakPayStcPrice_value = !string.IsNullOrEmpty(peakPayStcPrice) ? Convert.ToInt32(peakPayStcPrice) : 0;

            bool IsCustomTimePeriod_Value = Convert.ToBoolean(IsCustomTimePeriod);
            int CustomTimePeriodValue = !string.IsNullOrEmpty(customTimePeriod) ? Convert.ToInt32(customTimePeriod) : 0;
            bool IsCustomFee_Value = Convert.ToBoolean(IsCustomFee);
            decimal CustomFeeValue = !string.IsNullOrEmpty(customFee) ? Convert.ToDecimal(customFee) : 0;
            bool IsCustomPeakPayGst_Value = Convert.ToBoolean(IsCustomPeakPayGst);
            decimal CustomPeakPayGst_Value = !string.IsNullOrEmpty(customPeakPayGst) ? Convert.ToDecimal(customPeakPayGst) : 0;
            bool IsCustomPeakPay_CommercialJob = Convert.ToBoolean(IsCustomPeakPayCommercialJob);
            bool IsCustomPeakPay_NonCommercialJob = Convert.ToBoolean(IsCustomPeakPayNonCommercialJob);
            int CustomPeakPayStcPrice_value = !string.IsNullOrEmpty(customPeakPayStcPrice) ? Convert.ToInt32(customPeakPayStcPrice) : 0;

            bool IsDay1Price = Convert.ToBoolean(IsPriceDay1);
            bool IsDay3Price = Convert.ToBoolean(IsPriceDay3);
            bool IsDay7Price = Convert.ToBoolean(IsPriceDay7);
            bool IsOnApprovalPrice = Convert.ToBoolean(IsPriceApproval);
            bool IsRapidPayPrice = Convert.ToBoolean(IsPriceRapidPay);
            bool IsOptiPayPrice = Convert.ToBoolean(IsPriceOptiPay);
            bool IsCommercialPrice = Convert.ToBoolean(IsPriceCommercial);
            bool IsCustomPrice = Convert.ToBoolean(IsPriceCustom);
            bool IsInvoiceStcPrice = Convert.ToBoolean(IsPriceInvoiceStc);
            bool IsPeakPayPrice = Convert.ToBoolean(IsPricePeakPay);

            int customSettlementTermId = !string.IsNullOrEmpty(customSettlementTerm) ? Convert.ToInt32(customSettlementTerm) : 0;


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
                    _pricingManagerBAL.SaveGlobalPriceForSolarCompany(RId, Day1Price, Day3Price, Day7Price, OnApprovalPrice, rapidPayPrice, optiPayPrice, commercialPrice, customPrice, InvoiceStcPrice, customSettlementTermId, KW, Value_KW,
                        CommercialJobClaim, NonCommercialJobClaim, customKW, customValue_KW, customCommercialJobClaim, customNonCommercialJobClaim, IsDay1Price, IsDay3Price, IsDay7Price, IsOnApprovalPrice, IsRapidPayPrice, IsOptiPayPrice,
                        IsCommercialPrice, IsCustomPrice, IsInvoiceStcPrice, PeakPayPrice, IsTimePeriod_Value, TimePeriodValue, IsFee_Value, FeeValue, IsPeakPayPrice, IsCustomTimePeriod_Value, CustomTimePeriodValue, IsCustomFee_Value, CustomFeeValue,
                        IsPeakPayGst_Value, PeakPayGst_Value, IsPeakPay_CommercialJob, IsPeakPay_NonCommercialJob, PeakPayStcPrice_value, IsCustomPeakPayGst_Value, CustomPeakPayGst_Value, IsCustomPeakPay_CommercialJob, IsCustomPeakPay_NonCommercialJob,
                        CustomPeakPayStcPrice_value);

                    //CommonBAL.SetCacheDataForSTCSubmissionFromSolarCompanyId(RId);

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
                    _pricingManagerBAL.SaveCustomPriceForSolarCompany(lstSolaCompany, Day1Price, Day3Price, Day7Price, OnApprovalPrice, ExpiryDate, RId, rapidPayPrice, optiPayPrice, commercialPrice, customPrice, InvoiceStcPrice,
                        customSettlementTermId, KW, Value_KW, CommercialJobClaim, NonCommercialJobClaim, customKW, customValue_KW, customCommercialJobClaim, customNonCommercialJobClaim, IsDay1Price, IsDay3Price, IsDay7Price,
                        IsOnApprovalPrice, IsRapidPayPrice, IsOptiPayPrice, IsCommercialPrice, IsCustomPrice, IsInvoiceStcPrice, PeakPayPrice, IsTimePeriod_Value, TimePeriodValue, IsFee_Value, FeeValue, IsPeakPayPrice,
                        IsCustomTimePeriod_Value, CustomTimePeriodValue, IsCustomFee_Value, CustomFeeValue, IsPeakPayGst_Value, PeakPayGst_Value, IsPeakPay_CommercialJob, IsPeakPay_NonCommercialJob, PeakPayStcPrice_value,
                        IsCustomPeakPayGst_Value, CustomPeakPayGst_Value, IsCustomPeakPay_CommercialJob, IsCustomPeakPay_NonCommercialJob, CustomPeakPayStcPrice_value);

                    //foreach (int solarCompID in lstSolaCompany)
                    //{
                    //    if (CacheConfiguration.IsContainsKey(CacheConfiguration.dsJobIndex + "_" + solarCompID))
                    //        CacheConfiguration.Remove(CacheConfiguration.dsJobIndex + "_" + solarCompID);

                    //    DataSet dsAllColumnsData = _jobBAL.GetJobList_ForCachingData(Convert.ToString(solarCompID), ProjectSession.LoggedInUserId, SystemEnums.MenuId.JobView.GetHashCode());
                    //    CacheConfiguration.Set(CacheConfiguration.dsJobIndex + "_" + solarCompID, dsAllColumnsData.Tables[0]);
                    //}

                    //CommonBAL.SetCacheDataForSTCSubmissionFromSolarCompanyId(0, String.Join(",", lstSolaCompany));
                }
            }
            //}
            //catch (Exception ex)
            //{
            //    return this.Json(new { success = false, errormessage = ex.Message }, JsonRequestBehavior.AllowGet);
            //}
            return this.Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> SavePriceForSolarJob(string PricingType, string items, string day1price, string day3price, string day7price, string onapprovalprice, string expiryDate,
            string ResellerId, string solarCompanyId, string upFront, string partialPayment, string initialstc, string systemSize, string rapidPay, string optiPay, string commercial,
            string custom, string invoiceStcPrice, string customSettlementTerm, string UnderKW, string KWvalue, string CommercialJob, string NonCommercialJob, string IsCustomUnderKw,
            string CustomKWValue, string IsCustomCommercialJob, string IsCustomNonCommercialJob, string IsPriceDay1, string IsPriceDay3, string IsPriceDay7, string IsPriceApproval,
            string IsPriceRapidPay, string IsPriceOptiPay, string IsPriceCommercial, string IsPriceCustom, string IsPriceInvoiceStc, string IsPricePartialPayment, string peakPay,
            string IsTimePeriod, string timePeriod, string IsFee, string fee, string IsPricePeakPay, string IsCustomTimePeriod, string customTimePeriod, string IsCustomFee, string customFee,
            bool IsPeakPayGst, string peakPayGst, bool IsPeakPayCommercialJob, bool IsPeakPayNonCommercialJob, string peakPayStcPrice, bool IsCustomPeakPayGst, string customPeakPayGst,
            bool IsCustomPeakPayCommercialJob, bool IsCustomPeakPayNonCommercialJob, string customPeakPayStcPrice, string IsSettelmentDayUpFront, string UpFrontSettelmentDay, string IsSettelmentDateUpFront, string UpFrontSettelmentDate)
        {
            //try
            //{
            int SystemSize = Convert.ToInt32(systemSize);
            decimal Day1Price = 0;
            decimal Day3Price = !string.IsNullOrEmpty(day3price) ? Convert.ToDecimal(day3price) : 0;
            decimal Day7Price = !string.IsNullOrEmpty(day7price) ? Convert.ToDecimal(day7price) : 0;
            decimal OnApprovalPrice = !string.IsNullOrEmpty(onapprovalprice) ? Convert.ToDecimal(onapprovalprice) : 0;
            decimal PartialPayment = !string.IsNullOrEmpty(partialPayment) ? Convert.ToDecimal(partialPayment) : 0;
            decimal InitialSTC = !string.IsNullOrEmpty(initialstc) ? Convert.ToDecimal(initialstc) : 0;
            int SolarCompnyId = !string.IsNullOrEmpty(solarCompanyId) ? Convert.ToInt32(solarCompanyId) : 0;

            decimal rapidPayPrice = !string.IsNullOrEmpty(rapidPay) ? Convert.ToDecimal(rapidPay) : 0;
            decimal optiPayPrice = !string.IsNullOrEmpty(optiPay) ? Convert.ToDecimal(optiPay) : 0;
            decimal commercialPrice = !string.IsNullOrEmpty(commercial) ? Convert.ToDecimal(commercial) : 0;
            decimal customPrice = !string.IsNullOrEmpty(custom) ? Convert.ToDecimal(custom) : 0;

            decimal InvoiceStcPrice = !string.IsNullOrEmpty(invoiceStcPrice) ? Convert.ToDecimal(invoiceStcPrice) : 0;

            bool customKW = Convert.ToBoolean(IsCustomUnderKw);
            int customValue_KW = !string.IsNullOrEmpty(CustomKWValue) ? Convert.ToInt32(CustomKWValue) : 0;
            bool customCommercialJobClaim = Convert.ToBoolean(IsCustomCommercialJob);
            bool customNonCommercialJobClaim = Convert.ToBoolean(IsCustomNonCommercialJob);

            int customSettlementTermId = !string.IsNullOrEmpty(customSettlementTerm) ? Convert.ToInt32(customSettlementTerm) : 0;

            bool KW = Convert.ToBoolean(UnderKW);
            int Value_KW = !string.IsNullOrEmpty(KWvalue) ? Convert.ToInt32(KWvalue) : 0;
            bool CommercialJobClaim = Convert.ToBoolean(CommercialJob);
            bool NonCommercialJobClaim = Convert.ToBoolean(NonCommercialJob);


            decimal PeakPayPrice = !string.IsNullOrEmpty(peakPay) ? Convert.ToDecimal(peakPay) : 0;
            bool IsTimePeriod_Value = Convert.ToBoolean(IsTimePeriod);
            int TimePeriodValue = !string.IsNullOrEmpty(timePeriod) ? Convert.ToInt32(timePeriod) : 0;
            bool IsFee_Value = Convert.ToBoolean(IsFee);
            decimal FeeValue = !string.IsNullOrEmpty(fee) ? Convert.ToDecimal(fee) : 0;
            bool IsPeakPayGst_Value = Convert.ToBoolean(IsPeakPayGst);
            decimal PeakPayGst_Value = !string.IsNullOrEmpty(peakPayGst) ? Convert.ToDecimal(peakPayGst) : 0;
            bool IsPeakPay_CommercialJob = Convert.ToBoolean(IsPeakPayCommercialJob);
            bool IsPeakPay_NonCommercialJob = Convert.ToBoolean(IsPeakPayNonCommercialJob);
            int PeakPayStcPrice_value = !string.IsNullOrEmpty(peakPayStcPrice) ? Convert.ToInt32(peakPayStcPrice) : 0;

            bool IsCustomTimePeriod_Value = Convert.ToBoolean(IsCustomTimePeriod);
            int CustomTimePeriodValue = !string.IsNullOrEmpty(customTimePeriod) ? Convert.ToInt32(customTimePeriod) : 0;
            bool IsCustomFee_Value = Convert.ToBoolean(IsCustomFee);
            decimal CustomFeeValue = !string.IsNullOrEmpty(customFee) ? Convert.ToDecimal(customFee) : 0;
            bool IsCustomPeakPayGst_Value = Convert.ToBoolean(IsCustomPeakPayGst);
            decimal CustomPeakPayGst_Value = !string.IsNullOrEmpty(customPeakPayGst) ? Convert.ToDecimal(customPeakPayGst) : 0;
            bool IsCustomPeakPay_CommercialJob = Convert.ToBoolean(IsCustomPeakPayCommercialJob);
            bool IsCustomPeakPay_NonCommercialJob = Convert.ToBoolean(IsCustomPeakPayNonCommercialJob);
            int CustomPeakPayStcPrice_value = !string.IsNullOrEmpty(customPeakPayStcPrice) ? Convert.ToInt32(customPeakPayStcPrice) : 0;

            bool IsDay1Price = Convert.ToBoolean(IsPriceDay1);
            bool IsDay3Price = Convert.ToBoolean(IsPriceDay3);
            bool IsDay7Price = Convert.ToBoolean(IsPriceDay7);
            bool IsOnApprovalPrice = Convert.ToBoolean(IsPriceApproval);
            bool IsRapidPayPrice = Convert.ToBoolean(IsPriceRapidPay);
            bool IsOptiPayPrice = Convert.ToBoolean(IsPriceOptiPay);
            bool IsCommercialPrice = Convert.ToBoolean(IsPriceCommercial);
            bool IsCustomPrice = Convert.ToBoolean(IsPriceCustom);
            bool IsInvoiceStcPrice = Convert.ToBoolean(IsPriceInvoiceStc);
            bool IsPartialPaymentPrice = Convert.ToBoolean(IsPricePartialPayment);
            bool IsPeakPayPrice = Convert.ToBoolean(IsPricePeakPay);
            bool IsUpFrontSettelmentDay = Convert.ToBoolean(IsSettelmentDayUpFront);
            int UpFrontSettelmentDayValue = !string.IsNullOrEmpty(UpFrontSettelmentDay) ? Convert.ToInt32(UpFrontSettelmentDay) : 0;
            bool IsUpFrontSettelmentDate = Convert.ToBoolean(IsSettelmentDateUpFront);

            DateTime UpFrontSettelmentDateValue = DateTime.Now;
            if (!string.IsNullOrEmpty(UpFrontSettelmentDate))
            {
                UpFrontSettelmentDateValue = Convert.ToDateTime(UpFrontSettelmentDate);
            }

            DateTime ExpiryDate = DateTime.Now;
            if (!string.IsNullOrEmpty(expiryDate))
            {
                ExpiryDate = Convert.ToDateTime(expiryDate);
            }
            List<int> lstJobs = new List<int>();
            foreach (string job in items.Split(','))
            {
                int jID = 0;
                if (!string.IsNullOrEmpty(job))
                {
                    jID = Convert.ToInt32(job);
                    lstJobs.Add(jID);
                }
            }

            if (SystemSize == 1)
            {
                Day1Price = !string.IsNullOrEmpty(day1price) ? Convert.ToDecimal(day1price) : 0;
            }
            else
            {
                Day1Price = !string.IsNullOrEmpty(upFront) ? Convert.ToDecimal(upFront) : 0;
            }

            if (lstJobs != null && lstJobs.Count > 0)
            {
                _pricingManagerBAL.SaveCustomPriceForSolarJob(lstJobs, Day1Price, Day3Price, Day7Price, OnApprovalPrice, PartialPayment, InitialSTC, ExpiryDate, SolarCompnyId, rapidPayPrice, optiPayPrice, commercialPrice, customPrice,
                    InvoiceStcPrice, customSettlementTermId, KW, Value_KW, CommercialJobClaim, NonCommercialJobClaim, customKW, customValue_KW, customCommercialJobClaim, customNonCommercialJobClaim, IsDay1Price, IsDay3Price, IsDay7Price,
                     IsOnApprovalPrice, IsRapidPayPrice, IsOptiPayPrice, IsCommercialPrice, IsCustomPrice, IsInvoiceStcPrice, IsPartialPaymentPrice, PeakPayPrice, IsTimePeriod_Value, TimePeriodValue, IsFee_Value, FeeValue, IsPeakPayPrice,
                     IsCustomTimePeriod_Value, CustomTimePeriodValue, IsCustomFee_Value, CustomFeeValue, IsPeakPayGst_Value, PeakPayGst_Value, IsPeakPay_CommercialJob, IsPeakPay_NonCommercialJob, PeakPayStcPrice_value, IsCustomPeakPayGst_Value,
                     CustomPeakPayGst_Value, IsCustomPeakPay_CommercialJob, IsCustomPeakPay_NonCommercialJob, CustomPeakPayStcPrice_value, IsUpFrontSettelmentDay, UpFrontSettelmentDayValue, IsUpFrontSettelmentDate, UpFrontSettelmentDateValue);

                foreach (int jobID in lstJobs)
                {
                    await CommonBAL.SetCacheDataForJobID(0, jobID);

                    //DataTable dt = _jobBAL.GetStcJobDetailsIdList_ForCachingData(0, 0, jobID).Tables[0];
                    //foreach (DataRow dr in dt.Rows)
                    //{
                    //    //CommonBAL.SetCacheDataForSTCSubmission(Convert.ToInt32(dr["STCJobDetailsID"]), 0);
                    //}
                }
            }

            //}
            //catch (Exception ex)
            //{
            //    return this.Json(new { success = false, errormessage = ex.Message }, JsonRequestBehavior.AllowGet);
            //}
            return this.Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetGlobalPriceForReseller(string rId)
        {
            //try
            //{
            if (!string.IsNullOrEmpty(rId))
            {
                int Rid = Convert.ToInt32(rId);
                FormBot.Entity.PricingManager pmanager = _pricingManagerBAL.GetGlobalPriceForReseller(Rid);
                if (pmanager.CustomSettlementTerm > 0)
                {
                    pmanager.CustomTermText = "Custom - " + Common.GetDescription((FormBot.Helper.SystemEnums.STCSettlementTerm)pmanager.CustomSettlementTerm, "");
                }
                return this.Json(new { success = false, price = pmanager }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return this.Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            //}
            //catch (Exception ex)
            //{
            //    return this.Json(new { success = false, errormessage = ex.Message }, JsonRequestBehavior.AllowGet);
            //}
            //return this.Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetGlobalPriceForSAAS(string rId)
        {
            if (!string.IsNullOrEmpty(rId))
            {
                int Rid = Convert.ToInt32(rId);
                List<FormBot.Entity.PricingManagerSAAS> pmanager = _pricingManagerBAL.GetGlobalPriceForSAAS(Rid, true);
                StringBuilder sb = new StringBuilder();
                if (pmanager != null && pmanager.Count > 0)
                {
                    for (int i = 0; i <= pmanager.Count - 1; i++)
                    {
                        if (pmanager[i].IsEnable)
                        {
                            sb.Append("<li id='liDay1' settlementterm=" + pmanager[i].SettlementTerms + ">");
                            sb.Append("<span class='time'>" + pmanager[i].SettlementTerms + "</span>");
                            sb.Append("<span class='processing-text'></span>");
                            sb.Append("<span id = 'day1span' class='price'>$" + pmanager[i].Price + "</span>");
                            sb.Append("</li>");
                        }
                    }
                }
                return this.Json(new { success = false, response = sb.ToString() }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return this.Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<ActionResult> UpdateGST(string jobId, bool status)
        {
            //try
            //{
            int jID = 0;
            if (!string.IsNullOrEmpty(jobId))
            {
                int.TryParse(QueryString.GetValueFromQueryString(jobId, "id"), out jID);
            }

            _pricingManagerBAL.UpdateGST(jID, status);

            JobHistory jobHistory = new JobHistory();
            jobHistory.Name = ProjectSession.LoggedInName;
            jobHistory.FunctionalityName = "PricingManger";
            jobHistory.JobID = jID;
            bool isHistorySaved = _createJobHistoryBAL.LogJobHistory(jobHistory, HistoryCategory.ModifiedIsGst);
            //bool isHistorySaved = _createJobHistoryBAL.LogJobHistory(jobHistory, HistoryCategory.ModifiedIsGst);
            string JobHistoryMessage = "modified Gst from " + jobHistory.FunctionalityName;
            Common.SaveJobHistorytoXML(jobHistory.JobID, JobHistoryMessage, "General", "ModifiedIsGst", ProjectSession.LoggedInName, false);
            SortedList<string, string> data = new SortedList<string, string>();
            data.Add("IsGst", status.ToString());
            await CommonBAL.SetCacheDataForSTCSubmission(null, jID, data);
            return this.Json(new { success = true });
            //}
            //catch (Exception ex)
            //{
            //    return this.Json(new { success = false, errormessage = ex.Message });
            //}
        }


        #endregion
    }
}
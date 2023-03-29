using FormBot.BAL.Service;
using FormBot.Entity;
using System.Collections.Generic;
using FormBot.Helper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Web.Mvc;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using FormBot.Main.Models;
using System.Linq;
using FormBot.BAL;
using System.Web;
using Microsoft.Owin;
using FormBot.BAL.Service.CommonRules;
using System.Data;

namespace FormBot.Main.Controllers
{
    public class RAMSolarCompanyController : Controller
    {
        #region Properties
        private readonly IRAMSolarCompanyMappingBAL _ramSolarCompanyMappingBAL;
        private readonly ICreateJobBAL _createJobBAL;
        public UserManager<ApplicationUser> UserManager { get; private set; }
        #endregion

        #region Constructor
        public RAMSolarCompanyController(IRAMSolarCompanyMappingBAL rAMSolarCompanyMappingBAL,ICreateJobBAL createJobBAL)
        {
            this._ramSolarCompanyMappingBAL = rAMSolarCompanyMappingBAL;
            this._createJobBAL = createJobBAL;
        }
        #endregion

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>The List of Resellers</returns>
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// get RAMSolarCompany
        /// </summary>
        /// <returns>Returns List of Solar Company </returns>
        [HttpGet]
        public ActionResult _Create()
        {
            var model = new RAMSolarCompany();
            //model.UserTypeID = _ramSolarCompanyMappingBAL.GetRAMByUserId(ProjectSession.LoggedInUserId);
            var lstAssignedRAMSolarCompanyList = _ramSolarCompanyMappingBAL.GetAssignRAMSolarCompanyList(ProjectSession.LoggedInUserId, 0);
            if (lstAssignedRAMSolarCompanyList.Count > 0)
            {
                var lstUserList = _ramSolarCompanyMappingBAL.GetAllRAMSolarCompanyList(ProjectSession.LoggedInUserId);
                model.LstRAMSolarCompanyUser = lstUserList;
                model.LstRAMSolarCompanyAssignedUser = lstAssignedRAMSolarCompanyList;
                model.RAMSolarCompanyAssignedUser = lstAssignedRAMSolarCompanyList.Select(d => d.Value).ToArray();
            }
            else
            {
                var lstUserList = _ramSolarCompanyMappingBAL.GetAllRAMSolarCompanyList(ProjectSession.LoggedInUserId);
                model.LstRAMSolarCompanyUser = lstUserList;
                model.LstRAMSolarCompanyAssignedUser = new List<SelectListItem>();
            }
            return PartialView(model);
        }

        /// <summary>
        /// get Reseller Account Manager:
        /// </summary>
        /// <returns>Returns Reseller</returns>
        [HttpGet]
        public JsonResult GetResellerAccountManagerType()
        {
            List<SelectListItem> items = _ramSolarCompanyMappingBAL.GetResellerAccountManager(ProjectSession.LoggedInUserId).Select(a => new SelectListItem { Text = a.FirstName + " " + a.LastName, Value = a.UserId.ToString() }).ToList();
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Save SCA Assignment To RAM.
        /// </summary>
        /// <param name="ramId">The ram identifier.</param>
        /// <param name="assignedsc">The assigned solar company.</param>
        /// <returns>
        /// Returns test.
        /// </returns>
        [HttpGet] 
        ////[UserAuthorization]
        public async Task<ActionResult> SaveSCAAssignmentToRAM(string ramId, string assignedsc)
        {
            //try
            //{
            //&& !string.IsNullOrEmpty(assignedsc)
            if (Convert.ToInt32(ramId) > 0) 
            {
                DataTable dtSCABeforeDeleted = _createJobBAL.GetRAMSolarCompanyMappingForCache(Convert.ToInt32(ramId));
                List<int> scaIdBeforeDeleted = new List<int>();
                if (dtSCABeforeDeleted.Rows.Count > 0)
                {
                    for(int i = 0; i < dtSCABeforeDeleted.Rows.Count; i++)
                    {
                        scaIdBeforeDeleted.Add(Convert.ToInt32(dtSCABeforeDeleted.Rows[i]["SolarCompanyID"]));
                    }
                }

                _ramSolarCompanyMappingBAL.CreateRAMSolarCompanyMapping(Convert.ToInt32(ramId), assignedsc);
                DataTable dtSCAAfterDelete = _createJobBAL.GetRAMSolarCompanyMappingForCache(Convert.ToInt32(ramId));
                List<int> scaIdAfterDeleted = new List<int>();
                if (dtSCAAfterDelete.Rows.Count > 0)
                {
                    for (int i = 0; i < dtSCAAfterDelete.Rows.Count; i++)
                    {
                        scaIdAfterDeleted.Add(Convert.ToInt32(dtSCAAfterDelete.Rows[i]["SolarCompanyID"]));
                    }
                }
                List<int> deletedScaId = scaIdBeforeDeleted.Except(scaIdAfterDeleted).ToList();
                foreach(var id in deletedScaId)
                {
                    SortedList<string, string> data = new SortedList<string, string>();
                    data.Add("RamId", null);
                    data.Add("SolarCompanyId", id.ToString());
                    data.Add("AccountManager", null);
                    await CommonBAL.SetCacheDataOnSCARARAMForSTCSubmission(Convert.ToInt32(id), data);
                }
                DataSet ds = _createJobBAL.GetUserNameSolarCompanyForCache(Convert.ToInt32(ramId), 0);
               
                string userName = string.Empty;
                if (ds.Tables.Count>0)
                {
                    DataTable dt = ds.Tables[0];
                    
                    if (dt.Rows.Count > 0)
                    {
                        userName = dt.Rows[0]["UserFNameLname"].ToString();
                    }
                }
                List<string> lstSolarCompanyIds = assignedsc.Split(',').ToList();
                foreach (var id in lstSolarCompanyIds)
                {
                    if(id!="")
                    {
                        SortedList<string, string> data = new SortedList<string, string>();
                            data.Add("RamId", ramId);
                            data.Add("SolarCompanyId", id.ToString());
                            data.Add("AccountManager", userName);
                      await CommonBAL.SetCacheDataOnSCARARAMForSTCSubmission(Convert.ToInt32(id), data);
                      // CommonBAL.SetCacheDataForSTCSubmissionFromSolarCompanyId(ProjectSession.ResellerId, id);
                    }
                    //else
                    //    CommonBAL.SetCacheDataForSTCSubmissionFromSolarCompanyId(ProjectSession.ResellerId, "0");
                }
                return this.Json(new { success = true }, JsonRequestBehavior.AllowGet);
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
        }

        /// <summary>
        /// Gets the solar company by ram chanage drop down.
        /// </summary>
        /// <param name="ramId">The ram identifier.</param>
        /// <returns>ActionResult</returns>
        [HttpGet]
        public ActionResult GetSolarCompanyByRAMChanageDropDown(string ramId)
        {
            if (ramId != "")
            {
                var model = new RAMSolarCompany();
                model.nodeListAssigned = _ramSolarCompanyMappingBAL.GetAssignRAMSolarCompanyList(ProjectSession.LoggedInUserId, Convert.ToInt32(ramId));
                model.nodeList = _ramSolarCompanyMappingBAL.GetAllRAMSolarCompanyList(ProjectSession.LoggedInUserId);
                return this.Json(model, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var model = new RAMSolarCompany();
                model.nodeListAssigned = _ramSolarCompanyMappingBAL.GetAssignRAMSolarCompanyList(ProjectSession.LoggedInUserId, 0);
                model.nodeList = _ramSolarCompanyMappingBAL.GetAllRAMSolarCompanyList(ProjectSession.LoggedInUserId);
                return this.Json(model, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Get Assigned SolarCompany To RAM
        /// </summary>
        /// <returns>JsonResult</returns>
        public JsonResult GetAssignedSolarCompanyToRAM(bool isAll = false)
        {
            List<SelectListItem> items = _ramSolarCompanyMappingBAL.GetAssignedSolarCompanyToRAM(ProjectSession.LoggedInUserId).Select(a => new SelectListItem { Text = a.CompanyName, Value = a.SolarCompanyId.ToString() }).ToList();
            if (isAll && items.Count > 1)
                items.Add(new SelectListItem() { Value = "-1", Text = "All" });
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Reseller Account Manager Type
        /// </summary>
        /// <param name="resellerId">resellerId</param>
        /// <returns>JsonResult</returns>
        [HttpGet]
        public JsonResult GetRAMByReseller(string resellerId)
        {
            List<SelectListItem> items = _ramSolarCompanyMappingBAL.GetRAMByReseller(!string.IsNullOrEmpty(resellerId) ? Convert.ToInt32(resellerId) : 0).Select(a => new SelectListItem { Text = a.Name, Value = a.UserId.ToString() }).ToList();
            return Json(items, JsonRequestBehavior.AllowGet);
        }

    }
}
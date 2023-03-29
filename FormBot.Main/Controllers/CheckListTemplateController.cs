using FormBot.BAL.Service.CheckList;
using FormBot.Entity.CheckList;
using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FormBot.Main.Controllers
{
    public class CheckListTemplateController : Controller
    {
        #region Properties
        private readonly ICheckListTemplateBAL _checkListTemplateBAL;
        #endregion

        #region Constructor
        public CheckListTemplateController(ICheckListTemplateBAL checkListTemplateBAL)
        {
            this._checkListTemplateBAL = checkListTemplateBAL;
        }
        #endregion

        #region Events
        /// <summary>
        /// Get check list template dropdownlist.
        /// </summary>
        /// <returns>Items</returns>
        [HttpGet]
        public JsonResult GetCheckListTemplate(string solarCompanyId = "", int JobType = 0)
        {
            int? companyId = 0;
            if (ProjectSession.UserTypeId == 4 || ProjectSession.UserTypeId == 8 || ProjectSession.UserTypeId == 6)
                companyId = ProjectSession.SolarCompanyId;
            else
            {
                companyId = solarCompanyId==""?0: Convert.ToInt32(solarCompanyId);

            }

            List<SelectListItem> Items = _checkListTemplateBAL.GetData(companyId,ProjectSession.UserTypeId, JobType).Select(a => new SelectListItem { Text = a.CheckListTemplateName, Value = a.Id.ToString() }).ToList();
            return Json(Items, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddTemplate(CheckListTemplate CheckListTemplate)
        {
            if (ProjectSession.LoggedInUserId == 0)
                return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);

            try
            {
                if (ModelState.IsValid)
                {
                    int CopyOfCheckListTemplateId = 0;
                    if (!string.IsNullOrEmpty(CheckListTemplate.CopyOfCheckListTemplateId))
                    {
                        int.TryParse(QueryString.GetValueFromQueryString(CheckListTemplate.CopyOfCheckListTemplateId, "id"), out CopyOfCheckListTemplateId);
                    }

                    int deletedItemId = 0;
                    if (!string.IsNullOrEmpty(CheckListTemplate.DeleteDefaultItemId))
                    {
                        int.TryParse(QueryString.GetValueFromQueryString(CheckListTemplate.DeleteDefaultItemId, "id"), out deletedItemId);
                    }

                    int? companyId = 0;
                    if (ProjectSession.UserTypeId == 4 || ProjectSession.UserTypeId == 8 || ProjectSession.UserTypeId == 6)
                        companyId = ProjectSession.SolarCompanyId;
                    else
                        companyId = null;

                    CheckListTemplate.SolarCompanyId = companyId;
                    int templateId = _checkListTemplateBAL.CheckListTemplateInsertUpdate(CheckListTemplate, CopyOfCheckListTemplateId, deletedItemId);
                    string templateEncodedId = QueryString.QueryStringEncode("id=" + Convert.ToString(templateId));

                    return Json(new { status = true, id = templateId, templateEncodedId = templateEncodedId }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string error = String.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage + " " + v.Exception));
                    return Json(new { status = false, error = error, id = 0 }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return Json(new { status = false, error = ex.Message, id = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult Delete(string checkListTemplateIds)
        {
            if (ProjectSession.LoggedInUserId == 0)
                return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);

            try
            {
                List<string> lstTemplateIds = new List<string>();
                if (!string.IsNullOrEmpty(checkListTemplateIds))
                    lstTemplateIds = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(checkListTemplateIds);

                List<int> lstNewTemplateId = new List<int>();

                for (int i = 0; i < lstTemplateIds.Count; i++)
                {
                    int templateId = 0;
                    if (!string.IsNullOrEmpty(lstTemplateIds[i]))
                    {
                        int.TryParse(QueryString.GetValueFromQueryString(lstTemplateIds[i], "id"), out templateId);
                    }
                    lstNewTemplateId.Add(templateId);
                }

                string templateIds = string.Join(",", lstNewTemplateId.ToArray());

                //int templateId = 0;
                //if (!string.IsNullOrEmpty(id))
                //{
                //    int.TryParse(QueryString.GetValueFromQueryString(id, "id"), out templateId);
                //}

                _checkListTemplateBAL.CheckListTemplateDelete(templateIds);
                return Json(new { status = true, id = 1 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, error = ex.Message, id = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        public void GetCheckListTemplateList(string templateName, int solarCompanyId = 0)
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);
            IList<CheckListTemplate> lstCheckListTemplate = new List<CheckListTemplate>();

            if (ProjectSession.UserTypeId == 4 || ProjectSession.UserTypeId == 6 || ProjectSession.UserTypeId == 8)
                solarCompanyId = ProjectSession.SolarCompanyId;
            lstCheckListTemplate = _checkListTemplateBAL.CheckListTemplateList(pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, templateName, solarCompanyId, ProjectSession.UserTypeId);

            if (lstCheckListTemplate.Count > 0)
            {
                gridParam.TotalDisplayRecords = lstCheckListTemplate.FirstOrDefault().TotalRecords;
                gridParam.TotalRecords = lstCheckListTemplate.FirstOrDefault().TotalRecords;
            }

            HttpContext.Response.Write(Grid.PrepareDataSet(lstCheckListTemplate, gridParam));
        }

        [HttpGet]
        public ActionResult GetCheckListTemplateData(string id)
        {
            try
            {
                int templateId = 0;
                if (!string.IsNullOrEmpty(id))
                {
                    int.TryParse(QueryString.GetValueFromQueryString(id, "id"), out templateId);
                }
                CheckListTemplate checkListTemplate = _checkListTemplateBAL.GetCheckListTemplate(templateId);
                if (checkListTemplate != null)
                {
                    return Json(new { data = checkListTemplate, status = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = false }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, error = ex.Message, id = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }
}
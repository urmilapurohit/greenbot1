using FormBot.BAL.Service;
using FormBot.Entity.VEECCheckList;
using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FormBot.Main.Controllers
{
    public class VEECCheckListTemplateController : Controller
    {
        #region Properties
        private readonly IVEECCheckListTemplateBAL _veecCheckListTemplateBAL;
        #endregion

        #region Constructor

     
        public VEECCheckListTemplateController(IVEECCheckListTemplateBAL veecCheckListTemplateBAL)
        {
            this._veecCheckListTemplateBAL = veecCheckListTemplateBAL;
        }
        #endregion

        #region Events


        public JsonResult GetVEECCheckListTemplate(string solarCompanyId = "")
        {
            int? companyId = 0;
            if (ProjectSession.UserTypeId == 4)
                companyId = ProjectSession.SolarCompanyId;
            else
                companyId = null;

            List<SelectListItem> Items = _veecCheckListTemplateBAL.GetData(companyId, ProjectSession.UserTypeId).Select(a => new SelectListItem { Text = a.VEECCheckListTemplateName, Value = a.Id.ToString() }).ToList();
            return Json(Items, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Get VEEC CheckListTemplateList
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="solarCompanyId"></param>
        public void GetCheckListTemplateList(string templateName, string solarCompanyId = "")
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);
            IList<VEECCheckListTemplate> lstCheckListTemplate = new List<VEECCheckListTemplate>();

            int? companyId = 0;
            if (ProjectSession.UserTypeId == 4)
                companyId = ProjectSession.SolarCompanyId;
            else
                companyId = null;

            lstCheckListTemplate = _veecCheckListTemplateBAL.CheckListTemplateList(pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, templateName, companyId, ProjectSession.UserTypeId);

            if (lstCheckListTemplate.Count > 0)
            {
                gridParam.TotalDisplayRecords = lstCheckListTemplate.FirstOrDefault().TotalRecords;
                gridParam.TotalRecords = lstCheckListTemplate.FirstOrDefault().TotalRecords;
            }

            HttpContext.Response.Write(Grid.PrepareDataSet(lstCheckListTemplate, gridParam));
        }

        [HttpPost]
        public JsonResult AddTemplate(VEECCheckListTemplate veecCheckListTemplate)
        {
            if (ProjectSession.LoggedInUserId == 0)
                return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);

            try
            {
                if (ModelState.IsValid)
                {
                    int CopyOfCheckListTemplateId = 0;
                    if (!string.IsNullOrEmpty(veecCheckListTemplate.CopyOfCheckListTemplateId))
                    {
                        int.TryParse(QueryString.GetValueFromQueryString(veecCheckListTemplate.CopyOfCheckListTemplateId, "id"), out CopyOfCheckListTemplateId);
                    }

                    int deletedItemId = 0;
                    if (!string.IsNullOrEmpty(veecCheckListTemplate.DeleteDefaultItemId))
                    {
                        int.TryParse(QueryString.GetValueFromQueryString(veecCheckListTemplate.DeleteDefaultItemId, "id"), out deletedItemId);
                    }

                    int? companyId = 0;
                    if (ProjectSession.UserTypeId == 4)
                        companyId = ProjectSession.SolarCompanyId;
                    else
                        companyId = null;

                    veecCheckListTemplate.SolarCompanyId = companyId;
                    int templateId = _veecCheckListTemplateBAL.VEECCheckListTemplateInsertUpdate(veecCheckListTemplate, CopyOfCheckListTemplateId, deletedItemId);
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

                _veecCheckListTemplateBAL.VEECCheckListTemplateDelete(templateIds);
                return Json(new { status = true, id = 1 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, error = ex.Message, id = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetVEECCheckListTemplateData(string id)
        {
            try
            {
                int templateId = 0;
                if (!string.IsNullOrEmpty(id))
                {
                    int.TryParse(QueryString.GetValueFromQueryString(id, "id"), out templateId);
                }
                VEECCheckListTemplate checkListTemplate = _veecCheckListTemplateBAL.GetCheckListTemplate(templateId);
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